using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using ChaosRecipeEnhancer.DataModels.GGGModels;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Model
{
    public class ApiAdapter
    {
        public static bool IsFetching { get; set; }
        private static StashTabPropsList PropsList { get; set; }
        public static bool FetchError { get; set; }
        public static bool FetchingDone { get; set; }

        public static async Task<bool> GenerateUri()
        {
            FetchError = false;
            FetchingDone = false;
            Trace.WriteLine("generating uris!!");

            if (Settings.Default.PathOfExileAccountName != "" && Settings.Default.LeagueName != "")
            {
                var accName = Settings.Default.PathOfExileAccountName.Trim();
                var league = Settings.Default.LeagueName.Trim();

                if (await GetProps(accName, league))
                    if (!FetchError)
                    {
                        GenerateStashTabs();
                        GenerateStashTabUris(accName, league);
                        return true;
                    }
            }
            else
            {
                MessageBox.Show("Missing Settings!" + Environment.NewLine +
                                "Please set account name, stash tab name/index and league.");
            }

            IsFetching = false;
            return false;
        }

        public static IEnumerable<string> GetAllLeagueNames()
        {
            var leagueIds = new List<string>();

            using (var wc = new WebClient())
            {
                try
                {
                    var json = wc.DownloadString("https://api.pathofexile.com/leagues?type=main");
                    var document = JsonDocument.Parse(json);
                    var allLeagueData = document.RootElement.EnumerateArray();

                    leagueIds.AddRange(allLeagueData.Select(league => league.GetProperty("id").GetString()));
                }
                catch (WebException e)
                {
                    MessageBox.Show("Error: " + e.Message);
                }
            }

            return leagueIds;
        }

        private static void GenerateStashTabs()
        {
            var ret = new List<StashTab>();

            if (Settings.Default.StashTabIndices != null) StashTabList.GetStashTabIndices();

            // mode = Individual Stash Tab Indices
            if (Settings.Default.StashTabQueryMode == 0)
            {
                if (PropsList != null)
                {
                    foreach (var tab in PropsList.tabs)
                        for (var index = StashTabList.StashTabIndices.Count - 1; index > -1; index--)
                        {
                            if (StashTabList.StashTabIndices[index] != tab.Index) continue;

                            StashTabList.StashTabIndices.RemoveAt(index);

                            if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
                                ret.Add(new StashTab(tab.Name, tab.Index));
                        }

                    StashTabList.StashTabs = ret;
                    GetAllTabNames();
                }
            }
            // mode = Individual Stash Tab Prefix
            else if (Settings.Default.StashTabQueryMode == 1)
            {
                if (PropsList != null)
                {
                    var individualStashTabPrefix = Settings.Default.StashTabPrefix;

                    GetAllTabNames();

                    foreach (var tab in PropsList.tabs)
                        if (tab.Name.StartsWith(individualStashTabPrefix))
                            if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
                                ret.Add(new StashTab(tab.Name, tab.Index));

                    StashTabList.StashTabs = ret;
                }
            }
            // mode = Individual Stash Tab Suffix
            else if (Settings.Default.StashTabQueryMode == 2)
            {
                if (PropsList != null)
                {
                    var individualStashTabSuffix = Settings.Default.StashTabSuffix;

                    GetAllTabNames();

                    foreach (var tab in PropsList.tabs)
                        if (tab.Name.EndsWith(individualStashTabSuffix))
                            if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
                                ret.Add(new StashTab(tab.Name, tab.Index));

                    StashTabList.StashTabs = ret;
                }
            }

            /**
             * TODO: [Refactor] Repurpose the code below for searching by stash tab folder name
             *
             * So, the API we're using for querying items doesn't support the 'Parent' field for stash tabs.
             *
             * The sample response we get back is as follows:
             * 
             *      {
             *         "numTabs": 58,
             *         "tabs": [
             *           {
             *             "n": "Test 1 High iLvl",
             *             "i": 0,
             *             "id": "6a2da4a0dc12c816974f129864667fee59f652fcdc5980bfb62b1736a6e92917",
             *             "type": "PremiumStash",
             *             "selected": true,
             *             "colour": { "r": 221, "g": 221, "b": 221 },
             *             "srcL": "https://web.poecdn.com/gen/image/WzIzLDEseyJ0IjoibCIsImMiOi0yMjM2OTYzfV0/839620a564/Stash_TabL.png",
             *             "srcC": "https://web.poecdn.com/gen/image/WzIzLDEseyJ0IjoibSIsImMiOi0yMjM2OTYzfV0/e6a1df0898/Stash_TabL.png",
             *             "srcR": "https://web.poecdn.com/gen/image/WzIzLDEseyJ0IjoiciIsImMiOi0yMjM2OTYzfV0/1cc0d93243/Stash_TabL.png"
             *           },
             *          {...}
             *      }
             *
             * As you can see, no sort of 'Parent' field exists, so we aren't able to do select by folder quite yet...
             *
             * Below is the code I had written for search by parent mode. We may reuse it at some point later. But it
             * currently won't work.
             */

            // mode = Folder Name
            // if (Settings.Default.StashTabQueryMode == 3)
            // {
            //     if (PropsList != null)
            //     {
            //         var stashTabFolderName = Settings.Default.StashFolderName;
            //
            //         GetAllTabNames();
            //
            //         foreach (var tab in PropsList.tabs)
            //         {
            //             if (tab.Parent == stashTabFolderName)
            //             {
            //                 if (tab.Type == "PremiumStash" || tab.Type == "QuadStash" || tab.Type == "NormalStash")
            //                     ret.Add(new StashTab(tab.Name, tab.Index));
            //             }
            //         }
            //
            //         StashTabList.StashTabs = ret;
            //     }
            // }

            Trace.WriteLine(StashTabList.StashTabs.Count, "stash tab count");
        }

        private static void GenerateStashTabUris(string accName, string league)
        {
            foreach (var i in StashTabList.StashTabs)
            {
                var stashTab = i.TabIndex.ToString();

                i.StashTabUri = Settings.Default.TargetStash == 0
                    // URL for accessing personal stash
                    ? new Uri(
                        $"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&realm=pc&league={league}&tabIndex={stashTab}")
                    // URL for accessing guild stash
                    : new Uri(
                        $"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={accName}&realm=pc&league={league}&tabIndex={stashTab}");
            }
        }

        private static void GetAllTabNames()
        {
            foreach (var s in StashTabList.StashTabs)
            foreach (var props in PropsList.tabs)
                if (s.TabIndex == props.Index)
                    s.TabName = props.Name;
        }

        private static async Task<bool> GetProps(string accName, string league)
        {
            if (IsFetching) return false;

            if (Settings.Default.PathOfExileWebsiteSessionId == "")
            {
                MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set PoE Session Id.");
                return false;
            }

            // check rate limit
            if (RateLimit.CheckForBan()) return false;

            // -1 for 1 request + 3 times if rate limit high exceeded
            if (RateLimit.CurrentRequests >= RateLimit.MaximumRequests - 4)
            {
                RateLimit.RateLimitExceeded = true;
                return false;
            }

            IsFetching = true;

            Uri propsUri = null;

            // If accessing personal stash
            if (Settings.Default.TargetStash == 0)
            {
                Trace.WriteLine("[ApiAdapter:GetProps()] Generating propsUri for My Stash");

                propsUri = new Uri(
                    $"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&realm=pc&league={league}&tabs=1&tabIndex=0");

                Trace.WriteLine($"[ApiAdapter:GetProps()] ${propsUri}");
            }
            // Else if accessing guild stash
            else if (Settings.Default.TargetStash == 1)
            {
                Trace.WriteLine("[ApiAdapter:GetProps()] Generating propsUri for Guild Stash");

                propsUri = new Uri(
                    $"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={accName}&realm=pc&league={league}&tabs=1&tabIndex=0");

                Trace.WriteLine($"[ApiAdapter:GetProps()] ${propsUri}");
            }
            // Else error out
            else
            {
                throw new ArgumentException("Invalid TargetStash settings provided; please check your user settings");
            }

            var sessionId = Settings.Default.PathOfExileWebsiteSessionId;

            var cC = new CookieContainer();
            cC.Add(propsUri, new Cookie("POESESSID", sessionId));

            using (var handler = new HttpClientHandler { CookieContainer = cC })
            using (var client = new HttpClient(handler))
            {
                // add user agent
                client.DefaultRequestHeaders.Add("User-Agent",
                    $"EnhancePoEApp/v{Assembly.GetExecutingAssembly().GetName().Version}");
                using (var res = await client.GetAsync(propsUri))
                {
                    if (res.IsSuccessStatusCode)
                    {
                        using (var content = res.Content)
                        {
                            var resContent = await content.ReadAsStringAsync();
                            PropsList = JsonSerializer.Deserialize<StashTabPropsList>(resContent);

                            Trace.WriteLine(res.Headers, "res headers");

                            // get new rate limit values
                            var rateLimit = res.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
                            var rateLimitState = res.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
                            var responseTime = res.Headers.GetValues("Date").FirstOrDefault();
                            RateLimit.DeserializeRateLimits(rateLimit, rateLimitState);
                            RateLimit.DeserializeResponseSeconds(responseTime);
                        }
                    }
                    else
                    {
                        MessageBox.Show(res.StatusCode == HttpStatusCode.Forbidden
                            ? "Connection forbidden. Please check your account name and POE Session ID. You may have to refresh your POE Session ID sometimes."
                            : res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);

                        Trace.WriteLine($"[ApiAdapter:GetProps] Response Headers ${res.Headers}");
                        Trace.WriteLine($"[ApiAdapter:GetProps] Response Content ${res.Content}");
                        Trace.WriteLine($"[ApiAdapter:GetProps] Response Status Code ${res.StatusCode}");
                        Trace.WriteLine($"[ApiAdapter:GetProps] Response Reason Phrase ${res.ReasonPhrase}");
                        Trace.WriteLine($"[ApiAdapter:GetProps] Response Request Message ${res.RequestMessage}");

                        FetchError = true;
                        return false;
                    }
                }
            }

            IsFetching = false;
            return true;
        }

        public static async Task<bool> GetItems()
        {
            if (IsFetching)
            {
                Trace.WriteLine("already fetching");
                return false;
            }

            if (Settings.Default.PathOfExileWebsiteSessionId == "")
            {
                MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set PoE Session Id.");
                return false;
            }

            if (FetchError) return false;

            // TODO: can someone explain the -4 here? --cat
            // check rate limit
            if (RateLimit.CurrentRequests >= RateLimit.MaximumRequests - StashTabList.StashTabs.Count - 4)
            {
                RateLimit.RateLimitExceeded = true;
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
                client.DefaultRequestHeaders.Add("User-Agent",
                    $"EnhancePoEApp/v{Assembly.GetExecutingAssembly().GetName().Version}");
                foreach (var i in StashTabList.StashTabs)
                {
                    // check rate limit ban
                    if (RateLimit.CheckForBan()) return false;
                    if (usedUris.Contains(i.StashTabUri)) continue;

                    cookieContainer.Add(i.StashTabUri, new Cookie("POESESSID", sessionId));
                    using (var res = await client.GetAsync(i.StashTabUri))
                    {
                        usedUris.Add(i.StashTabUri);
                        if (res.IsSuccessStatusCode)
                        {
                            using (var content = res.Content)
                            {
                                // get new rate limit values
                                var rateLimit = res.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
                                var rateLimitState = res.Headers.GetValues("X-Rate-Limit-Account-State")
                                    .FirstOrDefault();
                                var responseTime = res.Headers.GetValues("Date").FirstOrDefault();
                                RateLimit.DeserializeRateLimits(rateLimit, rateLimitState);
                                RateLimit.DeserializeResponseSeconds(responseTime);

                                // deserialize response
                                var resContent = await content.ReadAsStringAsync();
                                var deserializedContent = JsonSerializer.Deserialize<ItemList>(resContent);

                                if (deserializedContent != null)
                                {
                                    i.ItemList = deserializedContent.items;
                                    i.Quad = deserializedContent.quadLayout;
                                }

                                i.CleanItemList();
                            }
                        }
                        else
                        {
                            FetchError = true;
                            MessageBox.Show(res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK,
                                MessageBoxImage.Error);
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