using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Windows;

namespace ChaosRecipeEnhancer.UI.Services;

public interface IApiService
{
    public Task<IEnumerable<BaseLeagueMetadata>> GetLeaguesAsync();

    public Task<BaseStashTabMetadataList> GetAllPersonalStashTabMetadataAsync(string accountName, string leagueName, string secret);

    public Task<BaseStashTabMetadataList> GetAllGuildStashTabMetadataAsync(string accountName, string leagueName, string secret);

    public Task<BaseStashTabContents> GetPersonalStashTabContentsByIndexAsync(string accountName, string leagueName, int tabIndex, string secret);

    public Task<BaseStashTabContents> GetGuildStashTabContentsByIndexAsync(string accountName, string leagueName, int tabIndex, string secret);
}

public class ApiService : IApiService
{
    private bool _isFetching;

    public async Task<IEnumerable<BaseLeagueMetadata>> GetLeaguesAsync()
    {
        var responseRaw = await GetAsync(ApiEndpoints.LeagueEndpoint);

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<BaseLeagueMetadata[]>((string)responseRaw);
    }

    public async Task<BaseStashTabMetadataList> GetAllPersonalStashTabMetadataAsync(string accountName, string leagueName, string secret)
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.StashTabPropsEndpoint(TargetStash.Personal, accountName, leagueName),
            secret
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<BaseStashTabMetadataList>((string)responseRaw);
    }

    public async Task<BaseStashTabMetadataList> GetAllGuildStashTabMetadataAsync(string accountName, string leagueName, string secret)
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.StashTabPropsEndpoint(TargetStash.Guild, accountName, leagueName),
            secret
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<BaseStashTabMetadataList>((string)responseRaw);
    }

    public async Task<BaseStashTabContents> GetPersonalStashTabContentsByIndexAsync(string accountName, string leagueName, int tabIndex, string secret)
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.IndividualTabContentsEndpoint(TargetStash.Personal, accountName, leagueName, tabIndex),
            secret
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<BaseStashTabContents>((string)responseRaw);
    }

    public async Task<BaseStashTabContents> GetGuildStashTabContentsByIndexAsync(string accountName, string leagueName, int tabIndex, string secret)
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.IndividualTabContentsEndpoint(TargetStash.Guild, accountName, leagueName, tabIndex),
            secret
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<BaseStashTabContents>((string)responseRaw);
    }

    private async Task<object> GetAuthenticatedAsync(Uri requestUri, string secret)
    {
        if (_isFetching || RateLimitManager.CheckForBan()) return null;

        // -1 for 1 request + 3 times if rate limit high exceeded
        if (RateLimitManager.RateLimitState[0] >= RateLimitManager.MaximumRequests - 4)
        {
            RateLimitManager.RateLimitExceeded = true;
            return null;
        }

        _isFetching = true;
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(requestUri, new Cookie("POESESSID", secret));
        using var handler = new HttpClientHandler();
        handler.CookieContainer = cookieContainer;

        using var client = new HttpClient(handler);

        // add user agent
        client.DefaultRequestHeaders.Add("User-Agent", $"CRE/v{Assembly.GetExecutingAssembly().GetName().Version}");

        // send request
        var response = await client.GetAsync(requestUri);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            _isFetching = false;

            ErrorWindow.Spawn(
                "You are making too many requests in a short period of time - You are rate limited. Wait a minute and try again.",
                "Error: Set Tracker Overlay - Fetch Data 429"
            );

            return null;
        }

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            _isFetching = false;

            ErrorWindow.Spawn(
                "It looks like your Session ID has expired. Please navigate to the 'Account > Path of Exile Account > PoE Session ID' setting and enter a new value, and try again.",
                "Error: Set Tracker Overlay - Fetch Data 403"
            );

            // usually we will be here if we weren't able to make a successful api request based on an expired session ID
            Settings.Default.PathOfExileWebsiteSessionId = string.Empty;
            Settings.Default.PoEAccountConnectionStatus = 0;
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            ErrorWindow.Spawn(
                "There was an error fetching data. Are the servers down?",
                $"Error: Set Tracker Overlay - Fetch Data {response.StatusCode}"
            );

            _isFetching = false;
            return null;
        }

        // get new rate limit values
        var rateLimit = response.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
        var rateLimitState = response.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
        var responseTime = response.Headers.GetValues("Date").FirstOrDefault();
        RateLimitManager.DeserializeRateLimits(rateLimit, rateLimitState);
        RateLimitManager.DeserializeResponseSeconds(responseTime);

        using var responseHttpContent = response.Content;
        _isFetching = false;
        return await responseHttpContent.ReadAsStringAsync();
    }

    private async Task<object> GetAsync(Uri requestUri)
    {
        if (_isFetching || RateLimitManager.CheckForBan()) return null;

        // -1 for 1 request + 3 times if ratelimit high exceeded
        if (RateLimitManager.RateLimitState[0] >= RateLimitManager.MaximumRequests - 4)
        {
            RateLimitManager.RateLimitExceeded = true;
            return null;
        }

        _isFetching = true;
        using var client = new HttpClient();
        var response = await client.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            _isFetching = false;
            return null;
        }

        _isFetching = false;
        return await response.Content.ReadAsStringAsync();
    }
}