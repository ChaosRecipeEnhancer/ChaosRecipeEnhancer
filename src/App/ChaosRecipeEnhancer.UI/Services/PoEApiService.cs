using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using ChaosRecipeEnhancer.UI.Models.Config;
using ChaosRecipeEnhancer.UI.Models.Exceptions;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.Services;

/// <summary>
/// Defines the contract for the PoE API service.
/// </summary>
public interface IPoeApiService
{
    /// <summary>
    /// Gets a list of league asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of league names.</returns>
    public Task<List<string>> GetLeaguesAsync();

    /// <summary>
    /// Retrieves the metadata for all personal stash tabs asynchronously.
    /// </summary>
    /// <remarks>This API call counts towards the shared CRE rate limit.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of stashes response.</returns>
    public Task<ListStashesResponse> GetAllPersonalStashTabMetadataAsync();

    /// <summary>
    /// Retrieves the contents of a personal stash tab by its ID asynchronously.
    /// </summary>
    /// <remarks>This API call counts towards the shared CRE rate limit.</remarks>
    /// <param name="stashId">The ID of the stash tab.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the get stash response.</returns>
    public Task<GetStashResponse> GetPersonalStashTabContentsByStashIdAsync(string stashId);
}

/// <summary>
/// Provides methods to interact with the Path of Exile API.
/// </summary>
public class PoeApiService : IPoeApiService
{
    #region Fields

    private readonly ILogger _log = Log.ForContext<PoeApiService>();
    private readonly IUserSettings _userSettings;
    private readonly IAuthStateManager _authStateManager;
    private readonly IHttpClientFactory _httpClientFactory;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PoeApiService"/> class.
    /// </summary>
    /// <param name="userSettings">The user settings.</param>
    /// <param name="authStateManager">The authentication state manager.</param>
    public PoeApiService(IHttpClientFactory httpClientFactory, IUserSettings userSettings, IAuthStateManager authStateManager)
    {
        _httpClientFactory = httpClientFactory;
        _userSettings = userSettings;
        _authStateManager = authStateManager;
    }

    #endregion

    #region Properties

    public bool CustomLeagueEnabled => _userSettings.CustomLeagueEnabled;

    #endregion

    #region Domain Methods

    /// <inheritdoc />
    public async Task<List<string>> GetLeaguesAsync()
    {
        List<string> leagueNames;

        if (CustomLeagueEnabled)
        {
            var results = await GetPersonalLeaguesAsync();

            leagueNames = results.Leagues
                .Where(league => !string.IsNullOrEmpty(league.PrivateLeagueUrl))
                .Select(league => league.Id)
                .ToList();
        }
        else
        {
            var results = await GetPublicLeaguesAsync();
            leagueNames = results.Select(league => league.Id).ToList();
        }

        return leagueNames;
    }

    /// <summary>
    /// Get a list of public leagues asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of public leagues.</returns>
    private async Task<IEnumerable<League>> GetPublicLeaguesAsync()
    {
        var responseRaw = await GetAsync(PoeApiConfig.PublicLeagueEndpoint);

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<League[]>((string)responseRaw);
    }

    /// <summary>
    /// Get a list of personal leagues asynchronously.
    /// </summary>
    /// <remarks>This API call counts towards the shared CRE rate limit.</remarks>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of personal leagues.</returns>
    private async Task<LeagueResponse> GetPersonalLeaguesAsync()
    {
        var responseRaw = await GetAuthenticatedAsync(PoeApiConfig.PersonalLeaguesEndpoint());

        var response = responseRaw is null
            ? null
            : JsonSerializer.Deserialize<LeagueResponse>((string)responseRaw);

        return response;
    }

    /// <inheritdoc />
    public async Task<ListStashesResponse> GetAllPersonalStashTabMetadataAsync()
    {
        var responseRaw = await GetAuthenticatedAsync(
            PoeApiConfig.PersonalStashTabPropsEndpoint()
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<ListStashesResponse>((string)responseRaw);
    }

    /// <inheritdoc />
    public async Task<GetStashResponse> GetPersonalStashTabContentsByStashIdAsync(string stashId)
    {
        var responseRaw = await GetAuthenticatedAsync(
            PoeApiConfig.PersonalIndividualTabContentsEndpoint(stashId)
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<GetStashResponse>((string)responseRaw);
    }

    #endregion

    #region Private Utility Methods

    /// <summary>
    /// Sends an authenticated GET request to the specified URI.
    /// </summary>
    /// <param name="requestUri">The request URI.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the response content.</returns>
    private async Task<object> GetAuthenticatedAsync(Uri requestUri)
    {
        if (GlobalRateLimitState.CheckForBan()) return null;

        // -1 for 1 request + 3 times if rate limit high exceeded
        if (GlobalRateLimitState.RateLimitState[0] >= GlobalRateLimitState.MaximumRequests - 4)
        {
            GlobalRateLimitState.RateLimitExceeded = true;
            return null;
        }

        // create new http client that will be disposed of after request
        var client = _httpClientFactory.CreateClient(PoeApiConfig.PoeApiHttpClientName);

        // add required headers

        // as of some point between 3.24 and 3.25, this is now a required field so definitely include it!
        // ty to Novynn for ur help ur a g
        client.DefaultRequestHeaders.UserAgent.ParseAdd(PoeApiConfig.UserAgent);

        // the auth token is required for all calls (we only use authenticated endpoints as of 3.24)
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authStateManager.AuthToken);

        // send request
        var response = await client.GetAsync(requestUri);
        var responseString = response.Content.ReadAsStringAsync().Result;

        _log.Information($"Fetch Result {requestUri}: {response.StatusCode}");
        _log.Information($"Response: {responseString}");

        // for some weird ass reason the status codes come
        // back 200 even when it's not valid (for leagues endpoint)
        // so here's a hacky work-around
        // HACK: GGG Fix ur shit
        if (response.StatusCode == HttpStatusCode.OK && !_authStateManager.ValidateAuthToken())
        {
            _log.Information("Status code is 200 but auth token is no good; manually replacing status code");
            response.StatusCode = HttpStatusCode.Unauthorized;
        }

        if (!CheckIfResponseStatusCodeIsValid(response, responseString)) return null;

        // get new rate limit values
        // these might end up being:
        //
        //      `X-Rate-Limit-Ip`
        //      `X-Rate-Limit-Ip-State`
        //      `X-Rate-Limit-Client`
        //      `X-Rate-Limit-Client-State`
        //
        // keep an eye on this if you get some weird issues...

        var rateLimit = response.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
        var rateLimitState = response.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
        var resultTime = response.Headers.GetValues("Date").FirstOrDefault();

        GlobalRateLimitState.DeserializeRateLimits(rateLimit, rateLimitState);
        GlobalRateLimitState.DeserializeResponseSeconds(resultTime);

        using var resultHttpContent = response.Content;

        var resultString = await resultHttpContent.ReadAsStringAsync();

        return resultString;
    }

    private async Task<object> GetAsync(Uri requestUri)
    {
        if (GlobalRateLimitState.CheckForBan()) return null;

        // -1 for 1 request + 3 times if rate limit high exceeded
        if (GlobalRateLimitState.RateLimitState[0] >= GlobalRateLimitState.MaximumRequests - 4)
        {
            GlobalRateLimitState.RateLimitExceeded = true;
            return null;
        }

        // create new http client that will be disposed of after request
        using var client = new HttpClient();

        // as of some point between 3.24 and 3.25, this is now a required field so definitely include it!
        // ty to Novynn for ur help ur a g
        client.DefaultRequestHeaders.UserAgent.ParseAdd(PoeApiConfig.UserAgent);

        var response = await client.GetAsync(requestUri);
        var responseString = response.Content.ReadAsStringAsync().Result;

        _log.Information($"Fetch Result {requestUri}: {response.StatusCode}");
        _log.Information($"Response: {responseString}");

        if (!CheckIfResponseStatusCodeIsValid(response, responseString)) return null;

        return responseString;
    }

    private bool CheckIfResponseStatusCodeIsValid(HttpResponseMessage response, string responseString)
    {
        switch (response.StatusCode)
        {
            case HttpStatusCode.Forbidden:
                GlobalErrorHandler.HandleError403FromApi(responseString);

                // usually we will be here if we weren't able to make a successful api request based on an expired auth token
                _authStateManager.Logout();

                return false;

            case HttpStatusCode.Unauthorized:
                GlobalErrorHandler.HandleError401FromApi(responseString);

                // if we're here, the auth token is invalid
                // so we need to log out and reset auth state
                _authStateManager.Logout();

                return false;

            case HttpStatusCode.TooManyRequests:
                var retryAfterSeconds = GlobalErrorHandler.HandleError429FromApi(response, responseString);
                throw new RateLimitException(retryAfterSeconds);

            case HttpStatusCode.InternalServerError:
                GlobalErrorHandler.HandleError500FromApi(responseString);
                return false;

            case HttpStatusCode.ServiceUnavailable:
                GlobalErrorHandler.HandleError503FromApi(responseString);
                return false;

            default:

                // handle any other 4xx or 5XX errors
                if ((int)response.StatusCode >= 400 && (int)response.StatusCode < 500)
                {
                    GlobalErrorHandler.HandleUnspecifiedErrorFromApi(responseString);
                    return false;
                }

                break;
        }

        return true;
    }

    #endregion
}