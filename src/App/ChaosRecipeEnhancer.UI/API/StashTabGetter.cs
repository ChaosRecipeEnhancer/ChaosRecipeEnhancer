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
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Api;

public sealed class StashTabGetter
{
    private bool _isFetching;

    public async Task<List<StashTabControl>> FetchStashTabsAsync()
    {
        var accName = Settings.Default.PathOfExileAccountName.Trim();
        var league = Settings.Default.LeagueName.Trim();

        var stashTabPropsList = await GetStashPropsAsync(accName, league);
        if (stashTabPropsList is not null)
        {
            var stashTabs = new List<StashTabControl>();
            for (var i = 0; i < stashTabPropsList.tabs.Count; i++)
            {
                var stashTabProps = stashTabPropsList.tabs[i];

                // todo constants
                // todo our stash tab vs normal

                var uri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&tabIndex={i}&league={league}");
                stashTabs.Add(new StashTabControl(stashTabProps.n, stashTabProps.i));
            }

            return stashTabs;
        }

        _isFetching = false;
        return null;
    }

    public async Task<StashTabPropsList> GetStashPropsAsync(string accName, string league)
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

        // our tab vs guild tab
        // TODO constants
        var propsUri = Settings.Default.TargetStash == 0
            ? new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&league={league}&tabs=1&tabIndex=")
            : new Uri($"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={accName}&league={league}&tabs=1&tabIndex=");

        Console.WriteLine($"Generated StashProps Query URL: {propsUri}");

        using var res = await DoAuthenticatedGetRequestAsync(propsUri);

        if (res.IsSuccessStatusCode)
        {
            Settings.Default.PoEAccountConnectionStatus = 1;
            Settings.Default.Save();
        }
        else if (!res.IsSuccessStatusCode)
        {
            Settings.Default.PoEAccountConnectionStatus = 2;
            Settings.Default.Save();

            _ = MessageBox.Show(res.StatusCode == HttpStatusCode.Forbidden
                    ? "Connection forbidden. Please check your Account Name and Session ID."
                    : res.ReasonPhrase,
                "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);

            return null;
        }

        using var content = res.Content;
        var resContent = await content.ReadAsStringAsync();

        Console.WriteLine("Successfully Retreived StashProps data.");
        // Console.WriteLine(resContent);

        return JsonSerializer.Deserialize<StashTabPropsList>(resContent);
    }

    public async Task<bool> GetItemsAsync(StashManagerControl stashManager, int queryMode = 0, List<string> fullStashList = null)
    {
        // If already fetching or are currently banned from querying API
        if (_isFetching || RateLimit.CheckForBan()) return false;

        // If rate limit exceeded for the request
        if (RateLimit.RateLimitState[0] >= RateLimit.MaximumRequests - 4)
        {
            RateLimit.RateLimitExceeded = true;
            return false;
        }

        _isFetching = true;
        using var __ = new ScopeGuard(() => _isFetching = false);

        // var queryMode = Settings.Default.StashTabQueryMode;
        // // if we have to do a '1st time' initialization for this thing
        // if (stashManager.StashTabControls.Count == 0 &&
        //     ((Settings.Default.StashTabIndices is not null && queryMode == 0) ||
        //      (!string.IsNullOrWhiteSpace(Settings.Default.StashTabPrefix) && queryMode == 1) ||
        //      (!string.IsNullOrWhiteSpace(Settings.Default.StashTabSuffix) && queryMode == 2)))
        // {
        //     
        // }
        
        
        // make request for each stash tab (is this really necessary? can we not batch this?)
        // i am surprised we don't get rate limited for this for-loop
        foreach (var stashTabControl in stashManager.StashTabControls)
        {
            Console.WriteLine($"Generated StashTab Query URL: {stashTabControl.StashTabUri}");
            using var res = await DoAuthenticatedGetRequestAsync(stashTabControl.StashTabUri);
            if (!res.IsSuccessStatusCode)
            {
                _ = MessageBox.Show(res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);
                // hit a snag, return false to indicate failure
                return false;
            }

            using var content = res.Content;
            var resContent = await content.ReadAsStringAsync();
            var deserializedContent = JsonSerializer.Deserialize<ItemList>(resContent);

            stashTabControl.Quad = deserializedContent.quadLayout;
            stashTabControl.FilterItemsForChaosRecipe(deserializedContent.items);
            
            // yay we made it
            Console.WriteLine("Successfully Retreived StashTab data.");
            Console.WriteLine(deserializedContent);
        }
        
        return true;
    }

    private async Task<HttpResponseMessage> DoAuthenticatedGetRequestAsync(Uri uri)
    {
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(uri, new Cookie("POESESSID", Settings.Default.PathOfExileWebsiteSessionId));

        using var handler = new HttpClientHandler();
        handler.CookieContainer = cookieContainer;

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