using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.Windows;

namespace ChaosRecipeEnhancer.UI.Services;

public interface IApiService
{
    public Task<IEnumerable<BaseLeagueMetadata>> GetLeaguesAsync();
    public Task<ListStashesResponse> GetAllPersonalStashTabMetadataAsync();
    public Task<GetStashResponse> GetPersonalStashTabContentsByStashIdAsync(string stashId);
}

public class ApiService : IApiService
{
    public async Task<IEnumerable<BaseLeagueMetadata>> GetLeaguesAsync()
    {
        var responseRaw = await GetAsync(ApiEndpoints.LeagueEndpoint);

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<BaseLeagueMetadata[]>((string)responseRaw);
    }

    public async Task<ListStashesResponse> GetAllPersonalStashTabMetadataAsync()
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.StashTabPropsEndpoint()
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<ListStashesResponse>((string)responseRaw);
    }

    public async Task<GetStashResponse> GetPersonalStashTabContentsByStashIdAsync(string stashId)
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.IndividualTabContentsEndpoint(stashId)
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<GetStashResponse>((string)responseRaw);
    }

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
        using var client = new HttpClient();

        // Add required headers
        var userAgent = $"OAuth chaosrecipeenhancer/{Assembly.GetExecutingAssembly().GetName().Version} (contact: dev@chaos-recipe.com)";
        client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalAuthState.Instance.AuthToken);

        // send request
        var response = await client.GetAsync(requestUri);

        Trace.WriteLine($"Fetch Result {requestUri}: {response.StatusCode}");
        Trace.WriteLine($"Response: {response.Content.ReadAsStringAsync().Result}");

        switch (response.StatusCode)
        {
            case HttpStatusCode.TooManyRequests:
                ErrorWindow.Spawn(
                    "You are making too many requests in a short period of time - You are rate limited. Wait a minute and try again.",
                    "Error: Set Tracker Overlay - Fetch Data 429"
                );

                return null;
            case HttpStatusCode.Forbidden:
                ErrorWindow.Spawn(
                    "It looks like your auth token has expired. Please navigate to the 'Account > Path of Exile Account > Login via Path of Exile' to log back in and get a new auth token.",
                    "Error: Set Tracker Overlay - Fetch Data 403"
                );

                // usually we will be here if we weren't able to make a successful api request based on an expired auth token
                GlobalAuthState.Instance.PurgeLocalAuthToken();

                // manually updating this value as a work-around to some issues with importing old settings from previous installations
                Settings.Default.PoEAccountConnectionStatus = (int)ConnectionStatusTypes.ConnectionNotValidated;
                Settings.Default.Save();

                return null;
            case HttpStatusCode.Unauthorized:
                ErrorWindow.Spawn(
                   "It looks like your auth token is invalid. Please navigate to the 'Account > Path of Exile Account > Login via Path of Exile' to log back in and get a new auth token.",
                   "Error: Set Tracker Overlay - Fetch Data 401"
                );

                // usually we will be here if we weren't able to make a successful api request based on an invalid auth token
                GlobalAuthState.Instance.PurgeLocalAuthToken();

                return null;
            case HttpStatusCode.ServiceUnavailable:
                ErrorWindow.Spawn(
                    "The Path of Exile API is currently unavailable. Please try again later.",
                    "Error: Set Tracker Overlay - Fetch Data 503"
                );

                return null;
        }

        // get new rate limit values
        // these might end up being `X-Rate-Limit-Ip` and `X-Rate-Limit-Ip-State` instead, or possibly `X-Rate-Limit-Client` and `X-Rate-Limit-Client-State`
        // keep an eye on this if you get some weird issues...
        var rateLimit = response.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
        var rateLimitState = response.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
        var responseTime = response.Headers.GetValues("Date").FirstOrDefault();

        GlobalRateLimitState.DeserializeRateLimits(rateLimit, rateLimitState);
        GlobalRateLimitState.DeserializeResponseSeconds(responseTime);

        using var responseHttpContent = response.Content;

        return await responseHttpContent.ReadAsStringAsync();
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

        var response = await client.GetAsync(requestUri);

        switch (response.StatusCode)
        {
            case HttpStatusCode.TooManyRequests:
                ErrorWindow.Spawn(
                    "You are making too many requests in a short period of time - You are rate limited. Wait a minute and try again.",
                    "Error: Set Tracker Overlay - Fetch Data 429"
                );

                return null;
            case HttpStatusCode.ServiceUnavailable:
                ErrorWindow.Spawn(
                    "The Path of Exile API is currently unavailable. Please try again later.",
                    "Error: Set Tracker Overlay - Fetch Data 503"
                );

                return null;
        }

        return await response.Content.ReadAsStringAsync();
    }
}