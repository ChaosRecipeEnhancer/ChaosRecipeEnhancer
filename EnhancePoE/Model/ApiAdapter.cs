using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using System.Net;
using EnhancePoE.Model;
using System.Linq;
using System.Reflection;

namespace EnhancePoE
{
    public class ApiAdapter
    {

        public static bool IsFetching { get; set; } = false;
        private static StashTabPropsList PropsList { get; set; }
        public static bool FetchError { get; set; } = false;
        public static bool FetchingDone { get; set; } = false;
        public static async Task<bool> GenerateUri()
        {
            FetchError = false;
            FetchingDone = false;
            Trace.WriteLine("generating uris!!");
            if (Properties.Settings.Default.accName != ""
                && Properties.Settings.Default.League != "")
            {
                ChaosRecipeEnhancer.aTimer.Enabled = false;
                Trace.WriteLine("stopping timer");
                Trace.WriteLine(ChaosRecipeEnhancer.aTimer.Interval);
                //Trace.WriteLine(ChaosRecipeEnhancer.aTimer.)
                string accName = Properties.Settings.Default.accName.Trim();
                string league = Properties.Settings.Default.League.Trim();

                if(await GetProps(accName, league))
                {
                    if (!FetchError)
                    {
                        GenerateStashTabs();
                        GenerateStashtabUris(accName, league);
                        return true;
                    }
                }

                // https://www.pathofexile.com/character-window/get-stash-items?accountName=kosace&tabIndex=0&league=Heist
            }
            else
            {
                MessageBox.Show("Missing Settings!" +  Environment.NewLine + "Please set Accountname, Stash Tab and League.");
            }
            return false;
        }

        private static void GenerateStashTabs()
        {
            List<StashTab> ret = new List<StashTab>();

            // mode = ID
            if (Properties.Settings.Default.StashtabMode == 0)
            {
                StashTabList.GetStashTabIndices();
                if(PropsList != null)
                {
                    foreach (StashTabProps p in PropsList.tabs)
                    {
                        for (int i = StashTabList.StashTabIndices.Count - 1; i > -1; i--)
                        {
                            if (StashTabList.StashTabIndices[i] == p.i)
                            {
                                StashTabList.StashTabIndices.RemoveAt(i);
                                ret.Add(new StashTab(p.n, p.i));
                            }
                        }
                    }
                    StashTabList.StashTabs = ret;
                    GetAllTabNames();
                }
            }
            // mode = Name
            else
            {
                if(PropsList != null)
                {
                    string stashName = Properties.Settings.Default.StashTabName;
                    foreach (StashTabProps p in PropsList.tabs)
                    {
                        if (p.n.StartsWith(stashName))
                        {
                            ret.Add(new StashTab(p.n, p.i));
                        }
                    }
                    StashTabList.StashTabs = ret;
                }
            }
            Trace.WriteLine(StashTabList.StashTabs.Count, "stash tab count");
        }

        private static void GenerateStashtabUris(string accName, string league)
        {
            foreach (StashTab i in StashTabList.StashTabs)
            {
                string stashTab = i.TabIndex.ToString();
                i.StashTabUri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&tabIndex={stashTab}&league={league}");
            }
        }

        private static void GetAllTabNames()
        {
            foreach(StashTab s in StashTabList.StashTabs)
            {
                foreach(StashTabProps p in PropsList.tabs)
                {
                    if(s.TabIndex == p.i)
                    {
                        s.TabName = p.n;
                    }
                }
            }
        }

        private static async Task<bool> GetProps(string accName, string league)
        {
            //Trace.WriteLine(IsFetching, "isfetching props");
            if(IsFetching)
            {
                return false;
            }
            if (Properties.Settings.Default.SessionId == "")
            {
                MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set PoE Session Id.");
                return false;
            }
            // check rate limit
            if (RateLimit.CheckForBan())
            {
                return false;
            }
            // -1 for 1 request + 3 times if ratelimit high exceeded
            if(RateLimit.RateLimitState[0] >= RateLimit.MaximumRequests - 4)
            {
                RateLimit.RateLimitExceeded = true;
                return false;
            }
            IsFetching = true;
            Uri propsUri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&tabs=1&league={league}");

            string sessionId = Properties.Settings.Default.SessionId;

            var cC = new CookieContainer();
            cC.Add(propsUri, new Cookie("POESESSID", sessionId));

            using (var handler = new HttpClientHandler() { CookieContainer = cC })
            using (HttpClient client = new HttpClient(handler))
            {
                //Trace.WriteLine("is here");
                // add user agent
                client.DefaultRequestHeaders.Add("User-Agent", $"EnhancePoEApp/v{Assembly.GetExecutingAssembly().GetName().Version}");
                using (HttpResponseMessage res = await client.GetAsync(propsUri))
                {
                    //Trace.WriteLine("is NOT here");
                    if (res.IsSuccessStatusCode)
                    {
                        //Trace.WriteLine("is not herre");
                        using (HttpContent content = res.Content)
                        {
                            string resContent = await content.ReadAsStringAsync();
                            //Trace.Write(resContent);
                            PropsList = JsonSerializer.Deserialize<StashTabPropsList>(resContent);

                            Trace.WriteLine(res.Headers, "res headers");

                            // get new rate limit values
                            //RateLimit.IncreaseRequestCounter();
                            string rateLimit = res.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
                            string rateLimitState = res.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
                            string responseTime = res.Headers.GetValues("Date").FirstOrDefault();
                            RateLimit.DeserializeRateLimits(rateLimit, rateLimitState);
                            RateLimit.DeserializeResponseSeconds(responseTime);
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);
                        FetchError = true;
                        return false;
                    }
                }
            }

            //await Task.Delay(1000);
            IsFetching = false;
            return true;
        }

        public async static Task<bool> GetItems()
        {
            if (IsFetching)
            {
                Trace.WriteLine("already fetching");
                return false;
            }
            if (Properties.Settings.Default.SessionId == "")
            {
                MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set PoE Session Id.");
                return false;
            }
            if (FetchError)
            {
                return false;
            }
            // check rate limit
            if (RateLimit.RateLimitState[0] >= RateLimit.MaximumRequests - StashTabList.StashTabs.Count - 4)
            {
                RateLimit.RateLimitExceeded = true;
                return false;
            }
            IsFetching = true;
            List<Uri> usedUris = new List<Uri>();

            string sessionId = Properties.Settings.Default.SessionId;

            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (HttpClient client = new HttpClient(handler))
            {
                // add user agent
                client.DefaultRequestHeaders.Add("User-Agent", $"EnhancePoEApp/v{Assembly.GetExecutingAssembly().GetName().Version}");
                foreach (StashTab i in StashTabList.StashTabs)
                {
                    // check rate limit ban
                    if (RateLimit.CheckForBan())
                    {
                        return false;
                    }
                    if (!usedUris.Contains(i.StashTabUri))
                    {
                        cookieContainer.Add(i.StashTabUri, new Cookie("POESESSID", sessionId));
                        using (HttpResponseMessage res = await client.GetAsync(i.StashTabUri))
                        {
                            usedUris.Add(i.StashTabUri);
                            if (res.IsSuccessStatusCode)
                            {
                                using (HttpContent content = res.Content)
                                {
                                    // get new rate limit values
                                    //RateLimit.IncreaseRequestCounter();
                                    string rateLimit = res.Headers.GetValues("X-Rate-Limit-Account").FirstOrDefault();
                                    string rateLimitState = res.Headers.GetValues("X-Rate-Limit-Account-State").FirstOrDefault();
                                    string responseTime = res.Headers.GetValues("Date").FirstOrDefault();
                                    RateLimit.DeserializeRateLimits(rateLimit, rateLimitState);
                                    RateLimit.DeserializeResponseSeconds(responseTime);

                                    // deserialize response
                                    string resContent = await content.ReadAsStringAsync();
                                    ItemList deserializedContent = JsonSerializer.Deserialize<ItemList>(resContent);
                                    i.ItemList = deserializedContent.items;
                                    i.Quad = deserializedContent.quadLayout;
                                    i.CleanItemList();
                                }
                            }
                            else
                            {
                                FetchError = true;
                                System.Windows.MessageBox.Show(res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);
                                return false;
                            }
                        }
                    }
                    //await Task.Delay(1000);
                }
            }

            IsFetching = false;
            FetchingDone = true;
            return true;
        }
    }
}
