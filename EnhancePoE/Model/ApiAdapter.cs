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
using EnhancePoE.Model;
using EnhancePoE.Properties;

namespace EnhancePoE
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

            if (Settings.Default.accName != "" && Settings.Default.League != "")
            {
                var accName = Settings.Default.accName.Trim();
                var league = Settings.Default.League.Trim();

                if (await GetProps(accName, league))
                    if (!FetchError)
                    {
                        GenerateStashTabs();
                        GenerateStashTabUris(accName, league);
                        return true;
                    }

                // https://www.pathofexile.com/character-window/get-stash-items?accountName=kosace&tabIndex=0&league=Heist
            }
            else
            {
                MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set Accountname, Stash Tab and League.");
            }

            IsFetching = false;
            return false;
        }

        public static List<string> GetAllLeagueNames()
        {
            var leagueIds = new List<string>();

            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString("https://api.pathofexile.com/leagues?type=main&compact=1");
                var document = JsonDocument.Parse(json);
                var allLeagueData = document.RootElement.EnumerateArray();
                foreach (var league in allLeagueData)
                {
                    string id = league.GetProperty("id").GetString();
                    leagueIds.Add(id);
                }
            }

            return leagueIds;
        }

        private static void GenerateStashTabs()
        {
            var ret = new List<StashTab>();

            // mode = ID
            if (Settings.Default.StashtabMode == 0)
            {
                StashTabList.GetStashTabIndices();
                if (PropsList != null)
                {
                    foreach (var p in PropsList.tabs)
                        for (var i = StashTabList.StashTabIndices.Count - 1; i > -1; i--)
                            if (StashTabList.StashTabIndices[i] == p.i)
                            {
                                StashTabList.StashTabIndices.RemoveAt(i);
                                ret.Add(new StashTab(p.n, p.i));
                            }

                    StashTabList.StashTabs = ret;
                    GetAllTabNames();
                }
            }
            // mode = Name
            else
            {
                if (PropsList != null)
                {
                    var stashName = Settings.Default.StashTabName;
                    foreach (var p in PropsList.tabs)
                        if (p.n.StartsWith(stashName))
                            ret.Add(new StashTab(p.n, p.i));

                    StashTabList.StashTabs = ret;
                }
            }

            Trace.WriteLine(StashTabList.StashTabs.Count, "stash tab count");
        }

        private static void GenerateStashTabUris(string accName, string league)
        {
            foreach (var i in StashTabList.StashTabs)
            {
                var stashTab = i.TabIndex.ToString();
                i.StashTabUri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&tabIndex={stashTab}&league={league}");
            }
        }

        private static void GetAllTabNames()
        {
            foreach (var s in StashTabList.StashTabs)
            {
                foreach (var p in PropsList.tabs)
                {
                    if (s.TabIndex == p.i)
                        s.TabName = p.n;
                }
            }
        }

        private static async Task<bool> GetProps(string accName, string league)
        {
            if (IsFetching) return false;

            if (Settings.Default.SessionId == "")
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
            var propsUri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&tabs=1&league={league}");

            var sessionId = Settings.Default.SessionId;

            var cC = new CookieContainer();
            cC.Add(propsUri, new Cookie("POESESSID", sessionId));

            using (var handler = new HttpClientHandler { CookieContainer = cC })
            using (var client = new HttpClient(handler))
            {
                // add user agent
                client.DefaultRequestHeaders.Add("User-Agent", $"EnhancePoEApp/v{Assembly.GetExecutingAssembly().GetName().Version}");
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
                        if (res.StatusCode == HttpStatusCode.Forbidden)
                            MessageBox.Show("Connection forbidden. Please check your account name and POE Session ID. You may have to refresh your POE Session ID sometimes.", "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);
                        else
                            MessageBox.Show(res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);

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

            if (Settings.Default.SessionId == "")
            {
                MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set PoE Session Id.");
                return false;
            }

            if (FetchError) return false;

            // check rate limit
            if (RateLimit.CurrentRequests >= RateLimit.MaximumRequests - StashTabList.StashTabs.Count - 4) // TODO: can someone explain the -4 here? --cat
            {
                RateLimit.RateLimitExceeded = true;
                return false;
            }

            IsFetching = true;
            var usedUris = new List<Uri>();

            var sessionId = Settings.Default.SessionId;

            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                // add user agent
                client.DefaultRequestHeaders.Add("User-Agent", $"EnhancePoEApp/v{Assembly.GetExecutingAssembly().GetName().Version}");
                foreach (var i in StashTabList.StashTabs)
                {
                    // check rate limit ban
                    if (RateLimit.CheckForBan()) return false;

                    if (!usedUris.Contains(i.StashTabUri))
                    {
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
                                    var rateLimitState = res.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
                                    var responseTime = res.Headers.GetValues("Date").FirstOrDefault();
                                    RateLimit.DeserializeRateLimits(rateLimit, rateLimitState);
                                    RateLimit.DeserializeResponseSeconds(responseTime);

                                    // deserialize response
                                    var resContent = await content.ReadAsStringAsync();
                                    var deserializedContent = JsonSerializer.Deserialize<ItemList>(resContent);
                                    i.ItemList = deserializedContent.items;
                                    i.Quad = deserializedContent.quadLayout;
                                    i.CleanItemList();
                                }
                            }
                            else
                            {
                                FetchError = true;
                                MessageBox.Show(res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
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