using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.BusinessLogic.Constants;
using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.BusinessLogic.StashTabs;
using ChaosRecipeEnhancer.UI.DynamicControls;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.DataFetching
{
    public abstract class ApiAdapter
    {
        public static bool IsFetching { get; set; }
        public static bool FetchError { get; set; }
        private static bool FetchingDone { get; set; }
        private static StashTabListModel ListModel { get; set; }

        // TODO: [Refactor] Take a good look at this method - it's a bit redundant in nature. Lots of repeated API calls.
        public static async Task<bool> FetchItemData()
        {
            FetchError = false;
            FetchingDone = false;

            if (Settings.Default.PathOfExileAccountName != "" && Settings.Default.LeagueName != "")
            {
                var accName = Settings.Default.PathOfExileAccountName.Trim();
                var league = Settings.Default.LeagueName.Trim();

                // 
                if (await FetchEntireStashTabList(accName, league))
                {
                    if (!FetchError)
                    {
                        GenerateReconstructedStashTabsFromApiResponse();
                        GenerateStashTabApiRequestUrls(accName, league);
                        return true;
                    }
                }
            }
            else
            {
                ErrorWindow.Spawn("You are missing one of the following settings: PoE Account Name, League, and Stash Tab Name(s) or Indices.", "Error: Stash Data Fetch Failed");
            }

            IsFetching = false;
            return false;
        }

        public static IEnumerable<string> FetchLeagueNames()
        {
            var leagueNames = new List<string>();

            using (var webClient = new WebClient())
            {
                try
                {
                    var json = webClient.DownloadString("https://api.pathofexile.com/leagues?type=main");
                    var document = JsonDocument.Parse(json);
                    var allLeagueData = document.RootElement.EnumerateArray();

                    leagueNames.AddRange(allLeagueData.Select(league => league.GetProperty("id").GetString()));
                }
                catch (WebException e)
                {
                    ErrorWindow.Spawn(e.Message + StringConstruction.DoubleNewLineCharacter +
                                      "The PoE servers are down (perhaps due to patching, maintenance, or server instability). " +
                                      "While the app is down, you will not be able to fetch. The app is NOT broken! Once the servers are back up, our app will work as usual. ",
                        "Warning: Path of Exile API Request");
                }
            }

            return leagueNames;
        }

        private static void GenerateReconstructedStashTabsFromApiResponse()
        {
            var reconstructedStashTabs = new List<StashTabControl>();

            if (Settings.Default.StashTabIndices != null) StashTabControlManager.GetStashTabIndices();

            // mode = Individual Stash Tab Indices
            if (Settings.Default.StashTabQueryMode == 0)
            {
                if (ListModel != null)
                {
                    foreach (var tab in ListModel.StashTabs)
                    {
                        for (var index = StashTabControlManager.StashTabIndices.Count - 1; index > -1; index--)
                        {
                            if (StashTabControlManager.StashTabIndices[index] != tab.Index) continue;

                            StashTabControlManager.StashTabIndices.RemoveAt(index);

                            if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
                                reconstructedStashTabs.Add(new StashTabControl(tab.Name, tab.Index));
                        }
                    }

                    StashTabControlManager.StashTabControls = reconstructedStashTabs;
                    ParseAllStashTabNamesFromApiResponse();
                }
            }
            // mode = Individual Stash Tab Prefix
            else if (Settings.Default.StashTabQueryMode == 1)
            {
                if (ListModel != null)
                {
                    var individualStashTabPrefix = Settings.Default.StashTabPrefix;

                    ParseAllStashTabNamesFromApiResponse();

                    foreach (var tab in ListModel.StashTabs)
                    {
                        if (tab.Name.StartsWith(individualStashTabPrefix))
                        {
                            if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
                                reconstructedStashTabs.Add(new StashTabControl(tab.Name, tab.Index));
                        }
                    }

                    StashTabControlManager.StashTabControls = reconstructedStashTabs;
                }
            }
            // mode = Individual Stash Tab Suffix
            else if (Settings.Default.StashTabQueryMode == 2)
            {
                if (ListModel != null)
                {
                    var individualStashTabSuffix = Settings.Default.StashTabSuffix;

                    ParseAllStashTabNamesFromApiResponse();

                    foreach (var tab in ListModel.StashTabs)
                    {
                        if (tab.Name.EndsWith(individualStashTabSuffix))
                        {
                            if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
                            {
                                reconstructedStashTabs.Add(new StashTabControl(tab.Name, tab.Index));
                            }
                        }
                    }

                    StashTabControlManager.StashTabControls = reconstructedStashTabs;
                }
            }
        }

        private static void GenerateStashTabApiRequestUrls(string accName, string league)
        {
            foreach (var i in StashTabControlManager.StashTabControls)
            {
                // ternary operation based on which stash we're targeting for queries (they use separate endpoints)
                i.StashTabApiRequestUrl = Settings.Default.TargetStash == 0
                    // URL for accessing personal stash
                    ? new Uri(
                        $"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&realm=pc&league={league}&tabIndex={i.TabIndex.ToString()}")
                    // URL for accessing guild stash
                    : new Uri(
                        $"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={accName}&realm=pc&league={league}&tabIndex={i.TabIndex.ToString()}");
            }
        }

        private static void ParseAllStashTabNamesFromApiResponse()
        {
            foreach (var s in StashTabControlManager.StashTabControls)
            {
                foreach (var props in ListModel.StashTabs)
                {
                    if (s.TabIndex == props.Index)
                    {
                        s.TabName = props.Name;
                    }
                }
            }
        }

        private static async Task<bool> FetchEntireStashTabList(string accName, string league)
        {
            if (IsFetching) return false;

            if (string.IsNullOrWhiteSpace(Settings.Default.PathOfExileWebsiteSessionId))
            {
                ErrorWindow.Spawn("You must set your PoE Session ID in the user settings to fetch data.","Error: Stash Data Fetch Failed");
                return false;
            }

            // check rate limit
            if (RateLimit.CheckForBan()) return false;

            // -1 for 1 request + 3 times if rate limit high exceeded
            if (RateLimit.CurrentRequests >= RateLimit.MaximumRequests - 4)
            {
                RateLimit.rateLimitExceeded = true;
                return false;
            }

            IsFetching = true;

            Uri propsUri;

            // If accessing personal stash
            if (Settings.Default.TargetStash == 0)
            {
                propsUri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&realm=pc&league={league}&tabs=1&tabIndex=0");
            }
            // Else if accessing guild stash
            else if (Settings.Default.TargetStash == 1)
            {
                propsUri = new Uri($"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={accName}&realm=pc&league={league}&tabs=1&tabIndex=0");
            }
            // Else error out
            else
            {
                throw new ArgumentException("Invalid TargetStash settings provided; please check your user settings");
            }

            var sessionId = Settings.Default.PathOfExileWebsiteSessionId;
            var cookieContainer = new CookieContainer();
            cookieContainer.Add(propsUri, new Cookie("POESESSID", sessionId));

            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                // add user agent
                client.DefaultRequestHeaders.Add("User-Agent", $"EnhancePoEApp/v{Assembly.GetExecutingAssembly().GetName().Version}");

                using (var response = await client.GetAsync(propsUri))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        using (var content = response.Content)
                        {
                            var resContent = await content.ReadAsStringAsync();

                            ListModel = JsonSerializer.Deserialize<StashTabListModel>(resContent);

                            // get new rate limit values
                            var rateLimit = response.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
                            var rateLimitState = response.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
                            var responseTime = response.Headers.GetValues("Date").FirstOrDefault();

                            RateLimit.DeserializeRateLimits(rateLimit, rateLimitState);
                            RateLimit.DeserializeResponseSeconds(responseTime);
                        }
                    }
                    else
                    {
                        string statusMessage;

                        if (response.StatusCode == HttpStatusCode.Forbidden)
                        {
                            statusMessage = response.StatusCode + ": " + response.ReasonPhrase + StringConstruction.DoubleNewLineCharacter +
                                            "Connection forbidden. Please check your Account Name and Session ID. You may have to log back into the site and get a new Session ID.";
                        }
                        else if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                        {
                            statusMessage = response.StatusCode + ": " + response.ReasonPhrase + StringConstruction.DoubleNewLineCharacter +
                                            "The PoE site servers seem to be down. This may be due to patching or issues on GGG's end. The app is working as expected.";
                        }
                        else
                        {
                            statusMessage = response.StatusCode + ": " + response.ReasonPhrase;
                        }

                        ErrorWindow.Spawn(statusMessage, "Error: Stash Data Fetch Failed");
                        FetchError = true;
                        return false;
                    }
                }
            }

            IsFetching = false;
            return true;
        }

        public static async Task<bool> FetchItemsForAllStashTabs()
        {
            if (IsFetching) return false;

            if (Settings.Default.PathOfExileWebsiteSessionId == "")
            {
                ErrorWindow.Spawn("You must set your PoE Session ID in the user settings to fetch data.", "Error: Stash Data Fetch Failed");
                return false;
            }

            if (FetchError) return false;

            // check rate limit
            if (RateLimit.CurrentRequests >= RateLimit.MaximumRequests - StashTabControlManager.StashTabControls.Count - 4)
            {
                RateLimit.rateLimitExceeded = true;
                return false;
            }

            IsFetching = true;

            var usedUris = new List<Uri>();
            var sessionId = Settings.Default.PathOfExileWebsiteSessionId;
            var cookieContainer = new CookieContainer();

            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                // add user agent
                client.DefaultRequestHeaders.Add("User-Agent", $"EnhancePoEApp/v{Assembly.GetExecutingAssembly().GetName().Version}");

                foreach (var i in StashTabControlManager.StashTabControls)
                {
                    // check rate limit ban
                    if (RateLimit.CheckForBan()) return false;
                    if (usedUris.Contains(i.StashTabApiRequestUrl)) continue;

                    cookieContainer.Add(i.StashTabApiRequestUrl, new Cookie("POESESSID", sessionId));

                    using (var response = await client.GetAsync(i.StashTabApiRequestUrl))
                    {
                        usedUris.Add(i.StashTabApiRequestUrl);

                        if (response.IsSuccessStatusCode)
                        {
                            using (var content = response.Content)
                            {
                                // get new rate limit values
                                var rateLimit = response.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
                                var rateLimitState = response.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
                                var responseTime = response.Headers.GetValues("Date").FirstOrDefault();

                                RateLimit.DeserializeRateLimits(rateLimit, rateLimitState);
                                RateLimit.DeserializeResponseSeconds(responseTime);

                                // deserialize response
                                var resContent = await content.ReadAsStringAsync();
                                var deserializedContent = JsonSerializer.Deserialize<EnhancedItemListModel>(resContent);

                                if (deserializedContent != null)
                                {
                                    i.ItemList = deserializedContent.Items;
                                    i.Quad = deserializedContent.IsQuadLayout;
                                }

                                i.CleanItemList();
                            }
                        }
                        else
                        {
                            FetchError = true;
                            ErrorWindow.Spawn(response.StatusCode + ": " + response.ReasonPhrase, "Error: Stash Data Fetch Failed");
                            return false;
                        }
                    }
                }
            }

            IsFetching = false;
            FetchingDone = true;
            return true;
        }
    }
}