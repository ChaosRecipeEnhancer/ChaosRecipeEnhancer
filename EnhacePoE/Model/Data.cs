using System;
using System.Collections.Generic;
//using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using EnhancePoE.Model;


namespace EnhancePoE
{
    // TODO: rework to static class
    public class Data
    {

        public bool GlovesActive { get; set; } = true;
        public bool HelmetActive { get; set; } = true;
        public bool BootsActive { get; set; } = true;
        public bool ChestActive { get; set; } = true;
        public bool WeaponActive { get; set; } = true;

        public static List<Item> GlobalNormalItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalShaperItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalElderItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalWarlordItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalHunterItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalRedeemerItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalCrusaderItemList { get; set; } = new List<Item>();


        public List<string> BootsBases { get; set; } = new List<string>();
        public List<string> GlovesBases { get; set; } = new List<string>();
        public List<string> HelmetBases { get; set; } = new List<string>();
        public List<string> ChestBases { get; set; } = new List<string>();
        public List<string> WeaponBases { get; set; } = new List<string>();
        public List<string> RingBases { get; set; } = new List<string>();
        public List<string> AmuletBases { get; set; } = new List<string>();
        public List<string> BeltBases { get; set; } = new List<string>();
        public List<string> TwoHandBases { get; set; } = new List<string>();

        public int SetAmount { get; set; } = 0;
        public int SetTargetAmount { get; set; } = 0;


        public static List<Item> GlobalItemOrderList { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListRest { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListShaper { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListRestShaper { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListElder { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListRestElder { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListWarlord { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListRestWarlord { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListCrusader { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListRestCrusader { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListRedeemer { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListRestRedeemer { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListHunter { get; set; } = new List<Item>();
        public static List<Item> GlobalItemOrderListRestHunter { get; set; } = new List<Item>();


        private void InitializeBases()
        {

            string pathBoots = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\BootsBases.txt");
            string[] boots = File.ReadAllLines(pathBoots);
            foreach (string line in boots)
            {
                if (line == "") { continue; }
                this.BootsBases.Add(line.Trim());
            }

            string pathGloves = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\GlovesBases.txt");
            string[] gloves = File.ReadAllLines(pathGloves);
            foreach (string line in gloves)
            {
                if (line == "") { continue; }
                this.GlovesBases.Add(line.Trim());
            }

            string pathHelmet = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\HelmetBases.txt");
            string[] helmet = File.ReadAllLines(pathHelmet);
            foreach (string line in helmet)
            {
                if (line == "") { continue; }
                this.HelmetBases.Add(line.Trim());
            }

            string pathWeapon = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\WeaponSmallBases.txt");
            string[] weapon = File.ReadAllLines(pathWeapon);
            foreach (string line in weapon)
            {
                if (line == "") { continue; }
                this.WeaponBases.Add(line.Trim());
            }

            string pathChest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\BodyArmourBases.txt");
            string[] chest = File.ReadAllLines(pathChest);
            foreach (string line in chest)
            {
                if (line == "") { continue; }
                this.ChestBases.Add(line.Trim());
            }

            string pathRing = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\RingBases.txt");
            string[] ring = File.ReadAllLines(pathRing);
            foreach (string line in ring)
            {
                if (line == "") { continue; }
                this.RingBases.Add(line.Trim());
            }

            string pathAmulet = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\AmuletBases.txt");
            string[] amulet = File.ReadAllLines(pathAmulet);
            foreach (string line in amulet)
            {
                if (line == "") { continue; }
                this.AmuletBases.Add(line.Trim());
            }

            string pathBelt = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\BeltBases.txt");
            string[] belt = File.ReadAllLines(pathBelt);
            foreach (string line in belt)
            {
                if (line == "") { continue; }
                this.BeltBases.Add(line.Trim());
            }

            string pathTwoHand = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\TwoHandBases.txt");
            string[] twohand = File.ReadAllLines(pathTwoHand);
            foreach(string line in twohand)
            {
                if(line == "") { continue; }
                this.TwoHandBases.Add(line.Trim());
            }
        }

        public Data()
        {
            InitializeBases();
        }

        public void GetSetTargetAmount(StashTab stash)
        {
            if(Properties.Settings.Default.Sets > 0)
            {
                this.SetTargetAmount = Properties.Settings.Default.Sets;
            }
            else
            {
                if (stash.Quad)
                {
                    this.SetTargetAmount += 16;
                }
                else
                {
                    this.SetTargetAmount += 4;
                }
            }
        }

        public void CheckActives()
        {

            bool exaltedActive = Properties.Settings.Default.ExaltedRecipe;
            bool filterActive = Properties.Settings.Default.LootfilterActive;

            SetTargetAmount = 0;
            GlobalNormalItemList.Clear();
            GlobalShaperItemList.Clear();
            GlobalElderItemList.Clear();
            GlobalCrusaderItemList.Clear();
            GlobalRedeemerItemList.Clear();
            GlobalWarlordItemList.Clear();
            GlobalHunterItemList.Clear();

            if(MainWindow.stashTabsModel != null)
            {
                foreach (StashTab s in MainWindow.stashTabsModel.StashTabs)
                {
                    GetSetTargetAmount(s);
                    if (s.ItemList != null)
                    {
                        GlobalNormalItemList.AddRange(s.ItemList);
                    }

                    if (exaltedActive)
                    {
                        GlobalShaperItemList.AddRange(s.ItemListShaper);
                        GlobalElderItemList.AddRange(s.ItemListElder);
                        GlobalWarlordItemList.AddRange(s.ItemListWarlord);
                        GlobalHunterItemList.AddRange(s.ItemListHunter);
                        GlobalRedeemerItemList.AddRange(s.ItemListRedeemer);
                        GlobalCrusaderItemList.AddRange(s.ItemListCrusader);
                    }
                }
            }

            Dictionary<string, int> globalAmount = GetFullSets(GlobalNormalItemList);

            if (exaltedActive)
            {
                globalAmount["sets"] += GetFullSets(GlobalShaperItemList)["sets"];
                globalAmount["sets"] += GetFullSets(GlobalElderItemList)["sets"];
                globalAmount["sets"] += GetFullSets(GlobalWarlordItemList)["sets"];
                globalAmount["sets"] += GetFullSets(GlobalCrusaderItemList)["sets"];
                globalAmount["sets"] += GetFullSets(GlobalHunterItemList)["sets"];
                globalAmount["sets"] += GetFullSets(GlobalRedeemerItemList)["sets"];
            }

            SetAmount = globalAmount["sets"];

            List<string> sectionList = new List<string>();

            if (filterActive)
            {
                FilterGeneration.LoadCustomStyle();
                if (Properties.Settings.Default.ExaltedRecipe)
                {
                    FilterGeneration.LoadCustomStyleInfluenced();
                }
            }

            if (Properties.Settings.Default.TwoHand)
            {
                if (globalAmount["weapons"] >= SetTargetAmount)
                {
                    if (filterActive)
                    {
                        sectionList.Add(FilterGeneration.GenerateSection(false, TwoHandBases, "weapon"));
                    }
                }
                else
                {
                    if (filterActive)
                    {
                        sectionList.Add(FilterGeneration.GenerateSection(true, TwoHandBases, "weapon"));
                    }
                }
            }
            if(globalAmount["weapons"] >= SetTargetAmount)
            {
                WeaponActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, WeaponBases, "weapon"));
                }
            }
            else
            {
                WeaponActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, WeaponBases, "weapon"));
                }
            }
            if(globalAmount["chests"] >= SetTargetAmount)
            {
                ChestActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, ChestBases, "chest"));

                }
            }
            else
            {
                ChestActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, ChestBases, "chest"));

                }
            }
            if (globalAmount["boots"] >= SetTargetAmount)
            {
                BootsActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, BootsBases, "boots"));

                }
            }
            else
            {
                BootsActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, BootsBases, "boots"));

                }
            }
            if (globalAmount["gloves"] >= SetTargetAmount)
            {
                GlovesActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, GlovesBases, "gloves"));

                }
            }
            else
            {
                GlovesActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, GlovesBases, "gloves"));

                }
            }
            if (globalAmount["helmets"] >= SetTargetAmount)
            {
                HelmetActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, HelmetBases, "helmet"));
                }
            }
            else
            {
                HelmetActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, HelmetBases, "helmet"));
                }
            }

            MainWindow.overlay.Dispatcher.Invoke(() =>
            {
                MainWindow.overlay.FullSetsTextBlock.Text = globalAmount["sets"].ToString();
            });

            if (filterActive)
            {
                sectionList.Add(FilterGeneration.GenerateSection(true, RingBases, "ring"));
                sectionList.Add(FilterGeneration.GenerateSection(true, AmuletBases, "amulet"));
                sectionList.Add(FilterGeneration.GenerateSection(true, BeltBases, "belt"));

                string oldFilter = FilterGeneration.OpenLootfilter();
                string newFilter = FilterGeneration.GenerateLootFilter(oldFilter, sectionList);
                FilterGeneration.WriteLootfilter(newFilter);

                if (Properties.Settings.Default.ExaltedRecipe)
                {
                    List<string> sectionListInfluenced = new List<string>();
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, RingBases, "ring", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, AmuletBases, "amulet", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, BeltBases, "belt", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, HelmetBases, "helmet", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, GlovesBases, "gloves", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, BootsBases, "boots", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, ChestBases, "chests", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, WeaponBases, "weapons", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, TwoHandBases, "weapons", true));

                    string oldFilter2 = FilterGeneration.OpenLootfilter();
                    string newFilter2 = FilterGeneration.GenerateLootFilterInfluenced(oldFilter2, sectionListInfluenced);
                    FilterGeneration.WriteLootfilter(newFilter2);
                }
            }
        }

        // TODO: combine with HasFullSets
        private Dictionary<string, int> GetFullSets(List<Item> itemList)
        {
            int ringsAmount = 0;
            int weaponsAmount = 0;
            int helmetAmount = 0;
            int bootsAmount = 0;
            int glovesAmount = 0;
            int chestAmount = 0;
            int amuletAmount = 0;
            int beltAmount = 0;
            int twoHandAmount = 0;

            foreach (Item item in itemList)
            {
                if (item.ItemType == "ring")
                {
                    ringsAmount++;
                }
                else if (item.ItemType == "amulet")
                {
                    amuletAmount++;
                }
                else if (item.ItemType == "belt")
                {
                    beltAmount++;
                }
                else if (item.ItemType == "boots")
                {
                    bootsAmount++;
                }
                else if (item.ItemType == "gloves")
                {
                    glovesAmount++;
                }
                else if (item.ItemType == "chest")
                {
                    chestAmount++;
                }
                else if (item.ItemType == "helmet")
                {
                    helmetAmount++;
                }
                else if (item.ItemType == "weapon")
                {
                    weaponsAmount++;
                }
                else if(item.ItemType == "twohand")
                {
                    twoHandAmount++;
                }
            }

            int rings = ringsAmount / 2;
            int weapons = weaponsAmount / 2;
            weapons += twoHandAmount;
            int sets = new[] { rings, weapons, helmetAmount, bootsAmount, glovesAmount, chestAmount, amuletAmount, beltAmount }.Min();

            Dictionary<string, int> ret = new Dictionary<string, int>();
            ret["rings"] = rings;
            ret["weapons"] = weapons;
            ret["sets"] = sets;
            ret["helmets"] = helmetAmount;
            ret["boots"] = bootsAmount;
            ret["gloves"] = glovesAmount;
            ret["chests"] = chestAmount;
            ret["amulets"] = amuletAmount;
            ret["belts"] = beltAmount;

            return ret;
        }

        private Dictionary<string, List<Item>> GetItemOrderList(List<Item> itemList)
        {

            List<Item> newItemOrderList = new List<Item>();
            List<Item> newItemOrderListRest = new List<Item>();

            while (GetFullSets(itemList)["sets"] > 0)
            {
                // 0: chest
                // 1: weapon 1
                // 2: weapon 2
                // 3: gloves
                // 4: helmet
                // 5: boots
                // 6: belt
                // 7: ring 1
                // 8: ring 2
                // 9: amulet

                bool twoHandPicked = false;
                if (Properties.Settings.Default.TwoHand)
                {
                    for (int i = itemList.Count - 1; i > -1; i--)
                    {
                        if (itemList[i].ItemType == "twohand" && !newItemOrderList.Contains(itemList[i]))
                        {
                            newItemOrderList.Add(itemList[i]);
                            itemList.RemoveAt(i);
                            twoHandPicked = true;
                            break;
                        }
                    }
                }

                int end = 0;
                while (end < 10)
                {
                    //Trace.WriteLine(end, "end: ");
                    for(int i = itemList.Count - 1; i > -1; i--)
                    {
                        //ItemWithStash _i = itemList[i];
                        switch (end)
                        {
                            case 0:
                                if (itemList[i].ItemType == "chest" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 1:
                                if (twoHandPicked)
                                {
                                    end++;
                                }
                                else if ((itemList[i].ItemType == "weapon") && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;

                            case 2:
                                if (twoHandPicked)
                                {
                                    end++;
                                }
                                else if (itemList[i].ItemType == "weapon" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;

                            case 3:
                                if (itemList[i].ItemType == "gloves" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 4:
                                if (itemList[i].ItemType == "helmet" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 5:
                                if (itemList[i].ItemType == "boots" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 6:
                                if (itemList[i].ItemType == "belt" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 7:
                                if (itemList[i].ItemType == "ring" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 8:
                                if (itemList[i].ItemType == "ring" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 9:
                                if (itemList[i].ItemType == "amulet" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                        }
                    }
                }
            }

            foreach(Item i in itemList)
            {
                newItemOrderListRest.Add(i);
            }

            Dictionary<string, List<Item>> ret = new Dictionary<string, List<Item>>();
            ret.Add("list", newItemOrderList);
            ret.Add("rest", newItemOrderListRest);

            return ret;
        }

        // 
        public void ActivateNextCell(bool active)
        {
            if (active)
            {
                if (GlobalItemOrderList.Count > 0)
                {

                    // INFLUENCE TEST
                    //if(GlobalItemOrderList[0].influences != null)
                    //{
                    //    Trace.WriteLine(GlobalItemOrderList[0].influences.shaper, "shaper");
                    //    Trace.WriteLine(GlobalItemOrderList[0].influences.elder, "elder");
                    //    Trace.WriteLine(GlobalItemOrderList[0].influences.crusader, "crusader");
                    //    Trace.WriteLine(GlobalItemOrderList[0].influences.redeemer, "redeemer");
                    //    Trace.WriteLine(GlobalItemOrderList[0].influences.warlord, "warlord");
                    //    Trace.WriteLine(GlobalItemOrderList[0].influences.hunter, "hunter");
                    //}
                    //else
                    //{
                    //    Trace.WriteLine("influences null");
                    //}

                    foreach (StashTab s in MainWindow.stashTabsModel.StashTabs)
                    {
                        s.TabHeaderColor = Brushes.Transparent;
                        foreach (Cell c in s.OverlayCellsList)
                        {
                            c.Active = false;
                        }
                    }
                    MainWindow.stashTabsModel.StashTabs[GlobalItemOrderList[0].StashTabIndex].ActivateItemCells(GlobalItemOrderList[0]);
                    if (Properties.Settings.Default.ColorStash != "")
                    {
                        MainWindow.stashTabsModel.StashTabs[GlobalItemOrderList[0].StashTabIndex].TabHeaderColor = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorStash));
                    }
                    else
                    {
                        MainWindow.stashTabsModel.StashTabs[GlobalItemOrderList[0].StashTabIndex].TabHeaderColor = Brushes.Red;
                    }
                    GlobalItemOrderList.RemoveAt(0);
                }
                else
                {
                    foreach(StashTab s in MainWindow.stashTabsModel.StashTabs)
                    {
                        s.TabHeaderColor = Brushes.Transparent;
                        foreach (Cell c in s.OverlayCellsList)
                        {
                            c.Active = false;
                        }
                    }
                }
            }
        }
        private void ReIndexAllStashTabs()
        {
            if(MainWindow.stashTabsModel != null)
            {
                for (int i = 0; i < MainWindow.stashTabsModel.StashTabs.Count; i++)
                {
                    MainWindow.stashTabsModel.StashTabs[i].TabIndex = i;
                }
                ReIndexAllItems();
            }
        }
        private void ReIndexAllItems()
        {
            if(MainWindow.stashTabsModel != null)
            {
                foreach(StashTab s in MainWindow.stashTabsModel.StashTabs)
                {
                    if(s.ItemList != null)
                    {
                        foreach (Item i in s.ItemList)
                        {
                            i.StashTabIndex = s.TabIndex;
                        }
                    }
                    if (Properties.Settings.Default.ExaltedRecipe)
                    {
                        foreach(Item i in s.ItemListShaper)
                        {
                            i.StashTabIndex = s.TabIndex;
                        }
                        foreach (Item i in s.ItemListElder)
                        {
                            i.StashTabIndex = s.TabIndex;
                        }
                        foreach (Item i in s.ItemListCrusader)
                        {
                            i.StashTabIndex = s.TabIndex;
                        }
                        foreach (Item i in s.ItemListHunter)
                        {
                            i.StashTabIndex = s.TabIndex;
                        }
                        foreach (Item i in s.ItemListWarlord)
                        {
                            i.StashTabIndex = s.TabIndex;
                        }
                        foreach (Item i in s.ItemListRedeemer)
                        {
                            i.StashTabIndex = s.TabIndex;
                        }
                    }
                }
            }
        }

        private void ClearAllItemOrderLists()
        {
            GlobalItemOrderList.Clear();
            GlobalItemOrderListRest.Clear();

            if (Properties.Settings.Default.ExaltedRecipe)
            {
                GlobalItemOrderListCrusader.Clear();
                GlobalItemOrderListElder.Clear();
                GlobalItemOrderListShaper.Clear();
                GlobalItemOrderListWarlord.Clear();
                GlobalItemOrderListRedeemer.Clear();
                GlobalItemOrderListHunter.Clear();

                GlobalItemOrderListRestCrusader.Clear();
                GlobalItemOrderListRestElder.Clear();
                GlobalItemOrderListRestHunter.Clear();
                GlobalItemOrderListRestRedeemer.Clear();
                GlobalItemOrderListRestShaper.Clear();
                GlobalItemOrderListRestWarlord.Clear();
            }
        }

        public void PrepareSelling()
        {
            ClearAllItemOrderLists();
            ReIndexAllStashTabs();
            foreach(StashTab s in MainWindow.stashTabsModel.StashTabs)
            {
                s.PrepareOverlayList();
                if (s.ItemList != null)
                {
                    if (GetFullSets(s.ItemList)["sets"] == 0)
                    {
                        foreach (Item i in s.ItemList)
                        {
                            GlobalItemOrderListRest.Add(i);
                        }
                    }
                    else
                    {
                        // copy list to prevent items deletion on stashtabs
                        List<Item> copyItemList = new List<Item>(s.ItemList);
                        Dictionary<string, List<Item>> itemorder = GetItemOrderList(copyItemList);
                        GlobalItemOrderList.AddRange(itemorder["list"]);
                        GlobalItemOrderListRest.AddRange(itemorder["rest"]);
                    }
                }

                if (Properties.Settings.Default.ExaltedRecipe)
                {
                    //shaper
                    if(GetFullSets(s.ItemListShaper)["sets"] == 0)
                    {
                        GlobalItemOrderListRestShaper.AddRange(s.ItemListShaper);
                    }
                    else
                    {
                        List<Item> copyItemList = new List<Item>(s.ItemListShaper);
                        Dictionary<string, List<Item>> itemorder = GetItemOrderList(copyItemList);
                        GlobalItemOrderList.AddRange(itemorder["list"]);
                        GlobalItemOrderListRestShaper.AddRange(itemorder["rest"]);
                    }
                    //elder
                    if (GetFullSets(s.ItemListElder)["sets"] == 0)
                    {
                        GlobalItemOrderListRestElder.AddRange(s.ItemListElder);
                    }
                    else
                    {
                        List<Item> copyItemList = new List<Item>(s.ItemListElder);
                        Dictionary<string, List<Item>> itemorder = GetItemOrderList(copyItemList);
                        GlobalItemOrderList.AddRange(itemorder["list"]);
                        GlobalItemOrderListRestElder.AddRange(itemorder["rest"]);
                    }
                    //warlord
                    if (GetFullSets(s.ItemListWarlord)["sets"] == 0)
                    {
                        GlobalItemOrderListRestWarlord.AddRange(s.ItemListWarlord);
                    }
                    else
                    {
                        List<Item> copyItemList = new List<Item>(s.ItemListWarlord);
                        Dictionary<string, List<Item>> itemorder = GetItemOrderList(copyItemList);
                        GlobalItemOrderList.AddRange(itemorder["list"]);
                        GlobalItemOrderListRestWarlord.AddRange(itemorder["rest"]);
                    }
                    //crusader
                    if (GetFullSets(s.ItemListCrusader)["sets"] == 0)
                    {
                        GlobalItemOrderListRestCrusader.AddRange(s.ItemListCrusader);
                    }
                    else
                    {
                        List<Item> copyItemList = new List<Item>(s.ItemListCrusader);
                        Dictionary<string, List<Item>> itemorder = GetItemOrderList(copyItemList);
                        GlobalItemOrderList.AddRange(itemorder["list"]);
                        GlobalItemOrderListRestCrusader.AddRange(itemorder["rest"]);
                    }
                    //hunter
                    if (GetFullSets(s.ItemListHunter)["sets"] == 0)
                    {
                        GlobalItemOrderListRestHunter.AddRange(s.ItemListHunter);
                    }
                    else
                    {
                        List<Item> copyItemList = new List<Item>(s.ItemListHunter);
                        Dictionary<string, List<Item>> itemorder = GetItemOrderList(copyItemList);
                        GlobalItemOrderList.AddRange(itemorder["list"]);
                        GlobalItemOrderListRestHunter.AddRange(itemorder["rest"]);
                    }
                    //redeemer
                    if (GetFullSets(s.ItemListRedeemer)["sets"] == 0)
                    {
                        GlobalItemOrderListRestRedeemer.AddRange(s.ItemListRedeemer);
                    }
                    else
                    {
                        List<Item> copyItemList = new List<Item>(s.ItemListRedeemer);
                        Dictionary<string, List<Item>> itemorder = GetItemOrderList(copyItemList);
                        GlobalItemOrderList.AddRange(itemorder["list"]);
                        GlobalItemOrderListRestRedeemer.AddRange(itemorder["rest"]);
                    }
                }
            }
            Dictionary<string, List<Item>> itemorderrest = GetItemOrderList(GlobalItemOrderListRest);
            GlobalItemOrderList.AddRange(itemorderrest["list"]);

            if (Properties.Settings.Default.ExaltedRecipe)
            {
                Dictionary<string, List<Item>> itemorderrestshaper = GetItemOrderList(GlobalItemOrderListRestShaper);
                GlobalItemOrderList.AddRange(itemorderrestshaper["list"]);

                Dictionary<string, List<Item>> itemorderrestelder = GetItemOrderList(GlobalItemOrderListRestElder);
                GlobalItemOrderList.AddRange(itemorderrestelder["list"]);

                Dictionary<string, List<Item>> itemorderrestcrusader = GetItemOrderList(GlobalItemOrderListRestCrusader);
                GlobalItemOrderList.AddRange(itemorderrestcrusader["list"]);

                Dictionary<string, List<Item>> itemorderrestwarlord = GetItemOrderList(GlobalItemOrderListRestWarlord);
                GlobalItemOrderList.AddRange(itemorderrestwarlord["list"]);

                Dictionary<string, List<Item>> itemorderresthunter = GetItemOrderList(GlobalItemOrderListRestHunter);
                GlobalItemOrderList.AddRange(itemorderresthunter["list"]);

                Dictionary<string, List<Item>> itemorderrestredeemer = GetItemOrderList(GlobalItemOrderListRestRedeemer);
                GlobalItemOrderList.AddRange(itemorderrestredeemer["list"]);
            }
        }
    }
}
