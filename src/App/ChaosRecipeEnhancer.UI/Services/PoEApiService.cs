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

        // add required headers

        // this useragent isn't strictly required, but it's good practice to include it (they ask for it in the spec)
        // the api will not spit back an error if we don't include it
        client.DefaultRequestHeaders.UserAgent.ParseAdd(ApiEndpoints.UserAgent);

        // the auth token is required for all calls (we only use authenticated endpoints as of 3.24)
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authStateManager.AuthToken);

        // send request
        var response = await client.GetAsync(requestUri);
        var responseString = response.Content.ReadAsStringAsync().Result;

        _log.Information($"Fetch Result {requestUri}: {response.StatusCode}");
        _log.Debug($"Response: {responseString}");

        // for some weird ass reason the status codes come back 200 even when it's not valid (for leagues endpoint)
        // so here's a hacky work-around
        if (response.StatusCode == HttpStatusCode.OK && !_authStateManager.ValidateAuthToken())
        {
            _log.Information("Status code is 200 but auth token is no good; manually replacing status code");
            response.StatusCode = HttpStatusCode.Unauthorized;
        }

        switch (response.StatusCode)
        {
            case HttpStatusCode.TooManyRequests:
                _log.Error("429 Too Many Requests - You are rate limited.");

                var timeoutString = GenerateTimeoutString(response);

                GlobalErrorHandler.Spawn(
                    (responseString)[..(responseString.Length > 500 ? 500 : responseString.Length)] + (responseString.Length > 500 ? "...\n\n(Truncated for brevity)" : string.Empty),
                    "Error: API Service - 429 Too Many Requests (Rate Limit)",
                    preamble: $"You are making too many requests in a short period of time - You are rate limited. Wait at least {timeoutString} and try again."
                );

                return null;
            case HttpStatusCode.Forbidden:
                _log.Error("403 Forbidden - It looks like your auth token is invalid.");

                GlobalErrorHandler.Spawn(
                    (responseString)[..(responseString.Length > 500 ? 500 : responseString.Length)] + (responseString.Length > 500 ? "...\n\n(Truncated for brevity)" : string.Empty),
                    "Error: API Service - 403 Forbidden",
                    preamble: "It looks like your auth token has expired. Please navigate to the 'Account > Path of Exile Account > Login via Path of Exile' to log back in and get a new auth token."
                );

                // usually we will be here if we weren't able to make a successful api request based on an expired auth token
                _authStateManager.Logout();

                // manually updating this value as a work-around to some issues with importing old settings from previous installations
                _userSettings.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionNotValidated;

                return null;
            case HttpStatusCode.Unauthorized:
                _log.Error("401 Unauthorized - It looks like your auth token is invalid.");

                GlobalErrorHandler.Spawn(
                    (responseString)[..(responseString.Length > 500 ? 500 : responseString.Length)] + (responseString.Length > 500 ? "...\n\n(Truncated for brevity)" : string.Empty),
                    "Error: API Service - 401 Unauthorized",
                    preamble: "It looks like your auth token is invalid. Please navigate to the 'Account > Path of Exile Account > Login via Path of Exile' to log back in and get a new auth token."
                );

                // usually we will be here if we weren't able to make a successful api request based on an invalid auth token
                _authStateManager.Logout();

                return null;
            case HttpStatusCode.ServiceUnavailable:

                _log.Error("503 Service Unavailable - The Path of Exile API is currently unavailable. Please try again later.");

                GlobalErrorHandler.Spawn(
                    (responseString)[..(responseString.Length > 500 ? 500 : responseString.Length)] + (responseString.Length > 500 ? "...\n\n(Truncated for brevity)" : string.Empty),
                    "Error: API Service - 503 PoE API Unavailable",
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
            var resultTime = response.Headers.GetValues("Date").FirstOrDefault();

            GlobalRateLimitState.DeserializeRateLimits(rateLimit, rateLimitState);
            GlobalRateLimitState.DeserializeResponseSeconds(resultTime);

            using var resultHttpContent = response.Content;

            var resultString = await resultHttpContent.ReadAsStringAsync();
            return resultString;
        }
        catch (InvalidOperationException e)
        {
            // Status code 500 is a server error
            _log.Error(e, "Error deserializing response from Path of Exile API after retries");

            GlobalErrorHandler.Spawn(
                e.Message,
                "Error: API Service - 500 PoE API (Temporary) Error",
                preamble: "The Path of Exile API seems to be having unspecified intermittent issues. " +
                "The response we got back from the API was not in the expected format, even after retries.\n\n" +
                "Please try again later."
            );

            return null;
        }
    }

    private static string GenerateTimeoutString(HttpResponseMessage response)
    {
        const string retryHeader = "Retry-After";
        
        var retryAfter = response.Headers.GetValues(retryHeader).Select(int.Parse).ToList();
        
        // Multiple timeout rules may apply, take the longest timeout
        if (retryAfter.Count > 1)
        {
            retryAfter = retryAfter.OrderByDescending(x => x).ToList();
        }

        var latestTimeoutInSeconds = retryAfter.First();

        var timeSpan = TimeSpan.FromSeconds(latestTimeoutInSeconds);

        var formattedTimeoutString = "";

        if (timeSpan.Hours > 0)
        {
            formattedTimeoutString += $"{timeSpan.Hours} hour{(timeSpan.Hours > 1 ? "s" : "")}, ";
        }

        if (timeSpan.Minutes > 0)
        {
            formattedTimeoutString += $"{timeSpan.Minutes} minute{(timeSpan.Minutes > 1 ? "s" : "")}, ";
        }

        if (timeSpan.Seconds > 0)
        {
            formattedTimeoutString += $"{timeSpan.Seconds} second{(timeSpan.Seconds > 1 ? "s" : "")}";
        }
        
        // If there is a trailing comma and space, remove it (e.g. "1 minute, )" -> "1 minute")
        if (formattedTimeoutString.EndsWith(", "))
        {
            formattedTimeoutString = formattedTimeoutString.Substring(0, formattedTimeoutString.Length - 2);
        }

        // If the timeout is only seconds, we do not need an 'and' before the seconds
        if (!formattedTimeoutString.Contains(','))
        {
            return formattedTimeoutString;
        }
        var lastCommaIndex = formattedTimeoutString.LastIndexOf(',');
        formattedTimeoutString = formattedTimeoutString.Remove(lastCommaIndex, 1).Insert(lastCommaIndex, " and");

        return formattedTimeoutString;
    }

    #endregion
}