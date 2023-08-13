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

namespace ChaosRecipeEnhancer.UI.Services;

public interface IApiService
{
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
        if (_isFetching || RateLimit.CheckForBan()) return null;
        
        // -1 for 1 request + 3 times if rate limit high exceeded
        if (RateLimit.RateLimitState[0] >= RateLimit.MaximumRequests - 4)
        {
            RateLimit.RateLimitExceeded = true;
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

        // get new rate limit values
        var rateLimit = response.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
        var rateLimitState = response.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
        var responseTime = response.Headers.GetValues("Date").FirstOrDefault();
        RateLimit.DeserializeRateLimits(rateLimit, rateLimitState);
        RateLimit.DeserializeResponseSeconds(responseTime);

        if (!response.IsSuccessStatusCode)
        {
            _isFetching = false;
            return null;
        }
        
        using var responseHttpContent = response.Content;
        _isFetching = false;
        return await responseHttpContent.ReadAsStringAsync();
    }
    
    private async Task<object> GetAsync(Uri requestUri)
    {
        if (_isFetching || RateLimit.CheckForBan()) return null;
        
        // -1 for 1 request + 3 times if ratelimit high exceeded
        if (RateLimit.RateLimitState[0] >= RateLimit.MaximumRequests - 4)
        {
            RateLimit.RateLimitExceeded = true;
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