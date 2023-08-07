using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Api;

public sealed class StashTabGetter
{
    private bool _isFetching;

    public async Task<List<StashTab>> FetchStashTabsAsync()
    {
        var accName = Settings.Default.PathOfExileAccountName.Trim();
        var league = Settings.Default.LeagueName.Trim();

        var stashTabPropsList = await GetStashPropsAsync(accName, league);
        if (stashTabPropsList is not null)
        {
            var stashTabs = new List<StashTab>();
            for (var i = 0; i < stashTabPropsList.tabs.Count; i++)
            {
                var stashTabProps = stashTabPropsList.tabs[i];
                var uri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&tabIndex={i}&league={league}");
                stashTabs.Add(new StashTab(stashTabProps.n, stashTabProps.i, uri));
            }

            return stashTabs;
        }

        _isFetching = false;
        return null;
    }

    private async Task<StashTabPropsList> GetStashPropsAsync(string accName, string league)
    {
        if (_isFetching || RateLimit.CheckForBan()) return null;

        // -1 for 1 request + 3 times if ratelimit high exceeded
        if (RateLimit.RateLimitState[0] >= RateLimit.MaximumRequests - 4)
        {
            RateLimit.RateLimitExceeded = true;
            return null;
        }

        _isFetching = true;
        using var __ = new ScopeGuard(() => _isFetching = false);

        var propsUri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&tabs=1&league={league}&tabIndex=");
        using var res = await DoAuthenticatedGetRequestAsync(propsUri);
        if (!res.IsSuccessStatusCode)
        {
            _ = MessageBox.Show(res.StatusCode == HttpStatusCode.Forbidden ? "Connection forbidden. Please check your Account Name and Session ID." : res.ReasonPhrase,
                "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);
            return null;
        }

        using var content = res.Content;
        var resContent = await content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<StashTabPropsList>(resContent);
    }

    public async Task<bool> GetItemsAsync(StashTab stashTab)
    {
        if (_isFetching || RateLimit.CheckForBan()) return false;

        if (RateLimit.RateLimitState[0] >= RateLimit.MaximumRequests - 4)
        {
            RateLimit.RateLimitExceeded = true;
            return false;
        }

        _isFetching = true;
        using var __ = new ScopeGuard(() => _isFetching = false);

        using var res = await DoAuthenticatedGetRequestAsync(stashTab.StashTabUri);
        if (!res.IsSuccessStatusCode)
        {
            _ = MessageBox.Show(res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        using var content = res.Content;
        var resContent = await content.ReadAsStringAsync();
        var deserializedContent = JsonSerializer.Deserialize<ItemList>(resContent);

        stashTab.Quad = deserializedContent.quadLayout;
        stashTab.FilterItemsForChaosRecipe(deserializedContent.items);

        return true;
    }

    private async Task<HttpResponseMessage> DoAuthenticatedGetRequestAsync(Uri uri)
    {
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(uri, new Cookie("POESESSID", Settings.Default.PathOfExileWebsiteSessionId));

        using var handler = new HttpClientHandler { CookieContainer = cookieContainer };
        using var client = new HttpClient(handler);

        // add user agent
        client.DefaultRequestHeaders.Add("User-Agent", $"CRE/v{Assembly.GetExecutingAssembly().GetName().Version}");

        var res = await client.GetAsync(uri);

        // get new rate limit values
        var rateLimit = res.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
        var rateLimitState = res.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
        var responseTime = res.Headers.GetValues("Date").FirstOrDefault();
        RateLimit.DeserializeRateLimits(rateLimit, rateLimitState);
        RateLimit.DeserializeResponseSeconds(responseTime);

        return res; // Needs to be disposed
    }
}