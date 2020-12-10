using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

using System.Web;
using System.Web.Script;
using System.Web.Script.Serialization;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using EnhancePoE.Model;
using System.Threading;

namespace EnhancePoE
{
    public class ApiAdapter
    {

        public static bool activated = false;

        //static Data currentData = new Data();

        public static void Activate()
        {
            activated = true;
        }

        public static void Deactivate()
        {
            activated = false;
        }

        //public class FetchedStashTab
        //{
        //    private int stashNumber { get; set; }
        //    private string stashName { get; set; }
        //    private Uri stashUri { get; set; }
        //}

        public static void GenerateUri()
        {
            Trace.WriteLine("Generating Uris...");
            if (Properties.Settings.Default.accName != ""
                && Properties.Settings.Default.League != "")
            {
                string accName = Properties.Settings.Default.accName;
                //
                string league = Properties.Settings.Default.League;

                //List<Uri> uriList = new List<Uri>();
                //HashSet<int> stashTabNumbers = new HashSet<int>();


                foreach(StashTab i in MainWindow.stashTabsModel.StashTabs)
                {
                    string stashTab = i.TabNumber.ToString();
                    i.StashTabUri = new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accName}&tabIndex={stashTab}&league={league}");
                    Trace.WriteLine(i.StashTabUri);
                    //stashTabNumbers.Add(i.TabNumber);
                }

                //Trace.WriteLine(stashTabNumbers.Count());

                //foreach(int number in stashTabNumbers)
                //{
                //    string stashTab = number.ToString();
                //    uriList.Add();
                //}

                //return uriList;
            }
            else
            {
                MessageBox.Show("Missing Settings!" +  Environment.NewLine + "Please set Accountname, Stash Tab and League.");
            }
            //return null;
        }

        public async static Task GetItems()
        {
            Trace.WriteLine("getting data...");

            if (Properties.Settings.Default.SessionId == "")
            {
                MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please set PoE Session Id.");
                return;
            }

            //List<Uri> uriList = new List<Uri>();

            List<Uri> usedUris = new List<Uri>();
            //foreach(StashTab s in MainWindow.stashTabsModel.StashTabs)
            //{
            //    uriList.Add(s.StashTabUri);
            //}
            //if (uriList?.Any() != true) { return null; }

            MainWindow.overlay.OverlayProgressBar.IsIndeterminate = true;

            //List<List<Item>> listOfItemlists = new List<List<Item>>();

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
                                    //Trace.WriteLine(content.GetType());

                                    //PostsObject myobject = await JsonConvert.DeserializeAsync<PostsObject>(content);

                                    string resContent = await content.ReadAsStringAsync();


                                    //Trace.WriteLine(mycontent);

                                    Trace.WriteLine(i.StashTabUri);

                                    ItemList deserializedContent = JsonSerializer.Deserialize<ItemList>(resContent);

                                    //foreach (Item i in deserializedContent.items)
                                    //{
                                    //    Trace.WriteLine(i.name);
                                    //}
                                    i.ItemList = deserializedContent.items;
                                    i.RemoveQualityFromItems();
                                    i.CleanItemList();
                                    //i.RemoveQualityFromItems();
                                    //Trace.WriteLine(deserializedContent);

                                    //Trace.WriteLine(myobject.id.ToString());
                                    //Trace.WriteLine(myobject.userId.ToString());
                                    //Trace.WriteLine(myobject.title);
                                    //Trace.WriteLine(myobject.body);
                                }
                            }
                            else
                            {
                                //Trace.WriteLine("error fetching data");
                                System.Windows.MessageBox.Show(res.ReasonPhrase, "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);

                            }
                        }
                    }
                    Trace.WriteLine("make api call and wait");
                    //Thread.Sleep(TimeSpan.FromSeconds(1));
                    await Task.Delay(1000);
                }
            }

            MainWindow.overlay.OverlayProgressBar.IsIndeterminate = false;

            //return listOfItemlists;
        }
    }


}
