using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Text.Json;
using System.Net;
using EnhancePoE.Model;

namespace EnhancePoE
{
    public class ApiAdapter
    {

        public static bool IsFetching { get; set; } = false;
        private static StashTabPropsList PropsList { get; set; }
        private static bool FetchError { get; set; } = false;
        public static bool FetchingDone { get; set; } = false;
        public static async Task GenerateUri()
        {
            FetchError = false;
            FetchingDone = false;
            Trace.WriteLine("generating uris!!");
            if (Properties.Settings.Default.accName != ""
                && Properties.Settings.Default.League != "")
            {

                string accName = Properties.Settings.Default.accName;
                string league = Properties.Settings.Default.League;

                await GetProps(accName, league);
                if (!FetchError)
                {
                    GenerateStashTabs();
                    GenerateStashtabUris(accName, league);
                }



                // https://www.pathofexile.com/character-window/get-stash-items?accountName=kosace&tabIndex=0&league=Heist
            }
            else
            {
                MessageBox.Show("Missing Settings!" +  Environment.NewLine + "Please set Accountname, Stash Tab and League.");
            }
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

        private static async Task GetProps(string accName, string league)
        {
            //Trace.WriteLine(IsFetching, "isfetching props");
            if(IsFetching)
            {
                return;
            }
            if (Properties.Settings.Default.SessionId == "")
            {
                MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set PoE Session Id.");
                return;
            }
            IsFetching = true;
            Uri propsUri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&tabs=1&league={league}");

            // start loading bar
            MainWindow.overlay.OverlayProgressBar.IsIndeterminate = true;

            string sessionId = Properties.Settings.Default.SessionId;

            var cC = new CookieContainer();
            cC.Add(propsUri, new Cookie("POESESSID", sessionId));

            using (var handler = new HttpClientHandler() { CookieContainer = cC })
            using (HttpClient client = new HttpClient(handler))
            {



                //Trace.WriteLine("is here");

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
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show(res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);
                        //Trace.WriteLine("fetching props failed!");
                        FetchError = true;
                        //Trace.WriteLine(res.StatusCode);
                    }
                }
            }

            await Task.Delay(1000);
            MainWindow.overlay.OverlayProgressBar.IsIndeterminate = false;
            IsFetching = false;
        }

        public async static Task GetItems()
        {
            if (IsFetching)
            {
                Trace.WriteLine("already fetching");
                return;
            }
            if (Properties.Settings.Default.SessionId == "")
            {
                MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set PoE Session Id.");
                return;
            }
            if (FetchError)
            {
                return;
            }
            IsFetching = true;
            List<Uri> usedUris = new List<Uri>();

            // start loading bar
            MainWindow.overlay.OverlayProgressBar.IsIndeterminate = true;

            string sessionId = Properties.Settings.Default.SessionId;

            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (HttpClient client = new HttpClient(handler))
            {
                foreach (StashTab i in StashTabList.StashTabs)
                {
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
                            }
                        }
                    }
                    await Task.Delay(1000);
                }
            }

            MainWindow.overlay.OverlayProgressBar.IsIndeterminate = false;
            IsFetching = false;
            FetchingDone = true;
        }
    }
}
