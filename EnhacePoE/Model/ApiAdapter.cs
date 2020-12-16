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

        private static bool IsFetching { get; set; }
        public static void GenerateUri()
        {
            if (Properties.Settings.Default.accName != ""
                && Properties.Settings.Default.League != "")
            {
                string accName = Properties.Settings.Default.accName;
                string league = Properties.Settings.Default.League;

                foreach(StashTab i in MainWindow.stashTabsModel.StashTabs)
                {
                    string stashTab = i.TabNumber.ToString();
                    i.StashTabUri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&tabIndex={stashTab}&league={league}");
                }

                // https://www.pathofexile.com/character-window/get-stash-items?accountName=kosace&tabIndex=0&league=Heist

            }
            else
            {
                MessageBox.Show("Missing Settings!" +  Environment.NewLine + "Please set Accountname, Stash Tab and League.");
            }
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
            IsFetching = true;
            List<Uri> usedUris = new List<Uri>();

            // start loading bar
            MainWindow.overlay.OverlayProgressBar.IsIndeterminate = true;

            string sessionId = Properties.Settings.Default.SessionId;

            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (HttpClient client = new HttpClient(handler))
            {
                foreach (StashTab i in MainWindow.stashTabsModel.StashTabs)
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
                                    i.RemoveQualityFromItems();
                                    i.CleanItemList();
                                }
                            }
                            else
                            {
                                System.Windows.MessageBox.Show(res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                    }
                    await Task.Delay(1000);
                }
            }

            MainWindow.overlay.OverlayProgressBar.IsIndeterminate = false;
            IsFetching = false;
        }
    }
}
