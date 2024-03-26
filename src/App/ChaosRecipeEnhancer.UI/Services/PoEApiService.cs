using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.State;
using Serilog;
using System;
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
public interface IPoEApiService
{
    /// <summary>
    /// Retrieves the list of leagues asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the league response.</returns>
    public Task<LeagueResponse> GetLeaguesAsync();

    /// <summary>
    /// Retrieves the metadata for all personal stash tabs asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of stashes response.</returns>
    public Task<ListStashesResponse> GetAllPersonalStashTabMetadataAsync();

    /// <summary>
    /// Retrieves the contents of a personal stash tab by its ID asynchronously.
    /// </summary>
    /// <param name="stashId">The ID of the stash tab.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the get stash response.</returns>
    public Task<GetStashResponse> GetPersonalStashTabContentsByStashIdAsync(string stashId);
}

/// <summary>
/// Provides methods to interact with the Path of Exile API.
/// </summary>
public class PoEApiService : IPoEApiService
{
    #region Fields

    private readonly ILogger _log = Log.ForContext<PoEApiService>();
    private readonly IUserSettings _userSettings;
    private readonly IAuthStateManager _authStateManager;
    private readonly IHttpClientFactory _httpClientFactory;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PoEApiService"/> class.
    /// </summary>
    /// <param name="userSettings">The user settings.</param>
    /// <param name="authStateManager">The authentication state manager.</param>
    public PoEApiService(IHttpClientFactory httpClientFactory, IUserSettings userSettings, IAuthStateManager authStateManager)
    {
        _httpClientFactory = httpClientFactory;
        _userSettings = userSettings;
        _authStateManager = authStateManager;
    }

    #endregion

    #region Domain Methods

    /// <inheritdoc />
    public async Task<LeagueResponse> GetLeaguesAsync()
    {
        var responseRaw = await GetAuthenticatedAsync(ApiEndpoints.LeaguesEndpoint());

        var response = responseRaw is null
            ? null
            : JsonSerializer.Deserialize<LeagueResponse>((string)responseRaw);

        return response;
    }

    /// <inheritdoc />
    public async Task<ListStashesResponse> GetAllPersonalStashTabMetadataAsync()
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.StashTabPropsEndpoint()
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<ListStashesResponse>((string)responseRaw);
    }

    /// <inheritdoc />
    public async Task<GetStashResponse> GetPersonalStashTabContentsByStashIdAsync(string stashId)
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.IndividualTabContentsEndpoint(stashId)
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
        var client = _httpClientFactory.CreateClient("PoEApiClient");

        // Add required headers
        client.DefaultRequestHeaders.UserAgent.ParseAdd(ApiEndpoints.UserAgent);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authStateManager.AuthToken);

        // send request
        var response = await client.GetAsync(requestUri);

        _log.Information($"Fetch Result {requestUri}: {response.StatusCode}");
        _log.Debug($"Response: {response.Content.ReadAsStringAsync().Result}");

        switch (response.StatusCode)
        {
            case HttpStatusCode.TooManyRequests:
                _log.Error("429 Too Many Requests - You are rate limited.");

                GlobalErrorHandler.Spawn(
                    "You are making too many requests in a short period of time - You are rate limited. Wait a minute and try again.",
                    "Error: API Service - Fetch Data 429"
                );

                return null;
            case HttpStatusCode.Forbidden:
                _log.Error("403 Forbidden - It looks like your auth token is invalid.");

                GlobalErrorHandler.Spawn(
                    "It looks like your auth token has expired. Please navigate to the 'Account > Path of Exile Account > Login via Path of Exile' to log back in and get a new auth token.",
                    "Error: API Service - Fetch Data 403"
                );

                // usually we will be here if we weren't able to make a successful api request based on an expired auth token
                _authStateManager.Logout();

                // manually updating this value as a work-around to some issues with importing old settings from previous installations
                _userSettings.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionNotValidated;

                return null;
            case HttpStatusCode.Unauthorized:
                _log.Error("401 Unauthorized - It looks like your auth token is invalid.");

                GlobalErrorHandler.Spawn(
                   "It looks like your auth token is invalid. Please navigate to the 'Account > Path of Exile Account > Login via Path of Exile' to log back in and get a new auth token.",
                   "Error: API Service - Fetch Data 401"
                );

                // usually we will be here if we weren't able to make a successful api request based on an invalid auth token
                _authStateManager.Logout();

                return null;
            case HttpStatusCode.ServiceUnavailable:

                _log.Error("503 Service Unavailable - The Path of Exile API is currently unavailable. Please try again later.");

                GlobalErrorHandler.Spawn(
                    (await response.Content.ReadAsStringAsync())[..500] + "...\n\n(Truncated for brevity)",
                    "Error: API Service - Path of Exile API Service Unavailable",
                    preamble: "The Path of Exile API is currently down. This is usually for maintenance, or DDoS, or league launch shennanigans - maybe all three!"
                );

                return null;
        }

        try
        {
            // get new rate limit values
            // these might end up being `X-Rate-Limit-Ip` and `X-Rate-Limit-Ip-State` instead, or possibly `X-Rate-Limit-Client` and `X-Rate-Limit-Client-State`
            // keep an eye on this if you get some weird issues...
            var rateLimit = response.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
            var rateLimitState = response.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
            var responseTime = response.Headers.GetValues("Date").FirstOrDefault();

            GlobalRateLimitState.DeserializeRateLimits(rateLimit, rateLimitState);
            GlobalRateLimitState.DeserializeResponseSeconds(responseTime);

            using var responseHttpContent = response.Content;

            var responseString = await responseHttpContent.ReadAsStringAsync();
            return responseString;
        }
        catch (InvalidOperationException e)
        {
            // Status code 500 is a server error
            _log.Error(e, "Error deserializing response from Path of Exile API after retries");

            GlobalErrorHandler.Spawn(
                e.Message, // Exception Message
                "Error: API Service - PoE Server Error 500 after retries", // Title
                preamble: "The Path of Exile API seems to be having unspecified intermittent issues. " +
                "The response we got back from the API was not in the expected format, even after retries.\n\n" +
                "Please try again later."
            );

            return null;
        }
    }

    #endregion
}