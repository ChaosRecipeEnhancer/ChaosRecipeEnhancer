using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.State;
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
            ApiEndpoints.StashTabPropsEndpoint(TargetStash.Personal, accountName, leagueName)
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<BaseStashTabMetadataList>((string)responseRaw);
    }

    public async Task<BaseStashTabMetadataList> GetAllGuildStashTabMetadataAsync(string accountName, string leagueName, string secret)
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.StashTabPropsEndpoint(TargetStash.Guild, accountName, leagueName)
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<BaseStashTabMetadataList>((string)responseRaw);
    }

    public async Task<BaseStashTabContents> GetPersonalStashTabContentsByIndexAsync(string accountName, string leagueName, int tabIndex, string secret)
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.IndividualTabContentsEndpoint(TargetStash.Personal, accountName, leagueName, tabIndex)
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<BaseStashTabContents>((string)responseRaw);
    }

    public async Task<BaseStashTabContents> GetGuildStashTabContentsByIndexAsync(string accountName, string leagueName, int tabIndex, string secret)
    {
        var responseRaw = await GetAuthenticatedAsync(
            ApiEndpoints.IndividualTabContentsEndpoint(TargetStash.Guild, accountName, leagueName, tabIndex)
        );

        return responseRaw is null
            ? null
            : JsonSerializer.Deserialize<BaseStashTabContents>((string)responseRaw);
    }

    private async Task<object> GetAuthenticatedAsync(Uri requestUri)
    {
        if (_isFetching || GlobalRateLimitState.CheckForBan()) return null;

        // -1 for 1 request + 3 times if rate limit high exceeded
        if (GlobalRateLimitState.RateLimitState[0] >= GlobalRateLimitState.MaximumRequests - 4)
        {
            GlobalRateLimitState.RateLimitExceeded = true;
            return null;
        }

        _isFetching = true;

        // create new http client that will be disposed of after request
        using var client = new HttpClient();

        // Add required headers
        var userAgent = $"OAuth chaosrecipeenhancer/{Assembly.GetExecutingAssembly().GetName().Version} (contact: dev@chaos-recipe.com)";
        client.DefaultRequestHeaders.UserAgent.ParseAdd(userAgent);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalAuthState.Instance.AuthToken);

        // send request
        var response = await client.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            _isFetching = false;

            Trace.WriteLine($"Error fetching {requestUri}: {response.StatusCode}");
            Trace.WriteLine($"Response: {response.Content.ReadAsStringAsync().Result}");

            ErrorWindow.Spawn("It seems like your auth token has expired. Please re-authenticate and try again.", "Error: Unable to Fetch Data");

            GlobalAuthState.Instance.PurgeLocalAuthToken();

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
        _isFetching = false;
        return await responseHttpContent.ReadAsStringAsync();
    }

    private async Task<object> GetAsync(Uri requestUri)
    {
        if (_isFetching || GlobalRateLimitState.CheckForBan()) return null;

        // -1 for 1 request + 3 times if ratelimit high exceeded
        if (GlobalRateLimitState.RateLimitState[0] >= GlobalRateLimitState.MaximumRequests - 4)
        {
            GlobalRateLimitState.RateLimitExceeded = true;
            return null;
        }

        _isFetching = true;

        // create new http client that will be disposed of after request
        using var client = new HttpClient();

        var response = await client.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            _isFetching = false;

            Trace.WriteLine($"Error fetching {requestUri}: {response.StatusCode}");
            Trace.WriteLine($"Response: {response.Content.ReadAsStringAsync().Result}");

            ErrorWindow.Spawn("It seems like your auth token has expired. Please re-authenticate and try again.", "Error: Unable to Fetch Data");

            GlobalAuthState.Instance.PurgeLocalAuthToken();

            return null;
        }

        _isFetching = false;
        return await response.Content.ReadAsStringAsync();
    }
}