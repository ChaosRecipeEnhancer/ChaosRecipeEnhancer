using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using EnhancePoE.Model;


namespace EnhancePoE
{
    // TODO: rework to static class
    public static class Data
    {

        public static ActiveItemTypes ActiveItems { get; set; } = new ActiveItemTypes();
        public static ActiveItemTypes PreviousActiveItems { get; set; }
        public static MediaPlayer Player { get; set; } = new MediaPlayer();
        public static List<Item> GlobalNormalItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalShaperItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalElderItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalWarlordItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalHunterItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalRedeemerItemList { get; set; } = new List<Item>();
        public static List<Item> GlobalCrusaderItemList { get; set; } = new List<Item>();

        public static int SetAmount { get; set; } = 0;
        public static int SetTargetAmount { get; set; } = 0;


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


        public static void GetSetTargetAmount(StashTab stash)
        {
            if(Properties.Settings.Default.Sets > 0)
            {
                SetTargetAmount = Properties.Settings.Default.Sets;
            }
            else
            {
                if (stash.Quad)
                {
                    SetTargetAmount += 16;
                }
                else
                {
                    SetTargetAmount += 4;
                }
            }
        }

        public static void CheckActives()
        {
            if (Properties.Settings.Default.Sound)
            {
                PreviousActiveItems = new ActiveItemTypes
                {
                    BootsActive = ActiveItems.BootsActive,
                    GlovesActive = ActiveItems.GlovesActive,
                    HelmetActive = ActiveItems.HelmetActive,
                    WeaponActive = ActiveItems.WeaponActive,
                    ChestActive = ActiveItems.ChestActive
                };
            }

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

            if (StashTabList.StashTabs != null)
            {
                foreach (StashTab s in StashTabList.StashTabs)
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
                        sectionList.Add(FilterGeneration.GenerateSection(false, "TwoHandWeapons"));
                    }
                }
                else
                {
                    if (filterActive)
                    {
                        sectionList.Add(FilterGeneration.GenerateSection(true, "TwoHandWeapons"));
                    }
                }
            }
            if(globalAmount["weapons"] >= SetTargetAmount)
            {
                ActiveItems.WeaponActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, "OneHandWeapons"));
                }
            }
            else
            {
                ActiveItems.WeaponActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, "OneHandWeapons"));
                }
            }
            if(globalAmount["chests"] >= SetTargetAmount)
            {
                ActiveItems.ChestActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, "Body Armours"));
                }
            }
            else
            {
                ActiveItems.ChestActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, "Body Armours"));

                }
            }
            if (globalAmount["boots"] >= SetTargetAmount)
            {
                ActiveItems.BootsActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, "Boots"));

                }
            }
            else
            {
                ActiveItems.BootsActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, "Boots"));

                }
            }
            if (globalAmount["gloves"] >= SetTargetAmount)
            {
                ActiveItems.GlovesActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, "Gloves"));

                }
            }
            else
            {
                ActiveItems.GlovesActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, "Gloves"));

                }
            }
            if (globalAmount["helmets"] >= SetTargetAmount)
            {
                ActiveItems.HelmetActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, "Helmets"));
                }
            }
            else
            {
                ActiveItems.HelmetActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, "Helmets"));
                }
            }

            MainWindow.overlay.Dispatcher.Invoke(() =>
            {
                MainWindow.overlay.FullSetsTextBlock.Text = globalAmount["sets"].ToString();
            });

            if (filterActive)
            {
                sectionList.Add(FilterGeneration.GenerateSection(true, "Rings"));
                sectionList.Add(FilterGeneration.GenerateSection(true, "Amulets"));
                sectionList.Add(FilterGeneration.GenerateSection(true, "Belts"));

                string oldFilter = FilterGeneration.OpenLootfilter();
                string newFilter = FilterGeneration.GenerateLootFilter(oldFilter, sectionList);
                FilterGeneration.WriteLootfilter(newFilter);

                if (Properties.Settings.Default.ExaltedRecipe)
                {
                    List<string> sectionListInfluenced = new List<string>();
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Rings", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Amulets", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Belts", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Helmets", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Gloves", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Boots", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Body Armours", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "OneHandWeapons", true));
                    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "TwoHandWeapons", true));

                    string oldFilter2 = FilterGeneration.OpenLootfilter();
                    string newFilter2 = FilterGeneration.GenerateLootFilterInfluenced(oldFilter2, sectionListInfluenced);
                    FilterGeneration.WriteLootfilter(newFilter2);
                }
            }

            if (Properties.Settings.Default.Sound)
            {
                if(!(PreviousActiveItems.GlovesActive == ActiveItems.GlovesActive 
                    && PreviousActiveItems.BootsActive == ActiveItems.BootsActive
                    && PreviousActiveItems.HelmetActive == ActiveItems.HelmetActive
                    && PreviousActiveItems.ChestActive == ActiveItems.ChestActive
                    && PreviousActiveItems.WeaponActive == ActiveItems.WeaponActive))
                {
                    Player.Dispatcher.Invoke(() =>
                    {
                        PlayNotificationSound();
                    });
                }
            }
        }

        public static void PlayNotificationSound()
        {
            double volume = Properties.Settings.Default.Volume / 100.0;
            Player.Volume = volume;
            Player.Position = TimeSpan.Zero;
            Player.Play();
        }

        private static Dictionary<string, int> GetFullSets(List<Item> itemList)
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
                if (item.ItemType == "Rings")
                {
                    ringsAmount++;
                }
                else if (item.ItemType == "Amulets")
                {
                    amuletAmount++;
                }
                else if (item.ItemType == "Belts")
                {
                    beltAmount++;
                }
                else if (item.ItemType == "Boots")
                {
                    bootsAmount++;
                }
                else if (item.ItemType == "Gloves")
                {
                    glovesAmount++;
                }
                else if (item.ItemType == "BodyArmours")
                {
                    chestAmount++;
                }
                else if (item.ItemType == "Helmets")
                {
                    helmetAmount++;
                }
                else if (item.ItemType == "OneHandWeapons")
                {
                    weaponsAmount++;
                }
                else if(item.ItemType == "TwoHandWeapons")
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

        private static Dictionary<string, List<Item>> GetItemOrderList(List<Item> itemList)
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
                        if (itemList[i].ItemType == "TwoHandWeapons" && !newItemOrderList.Contains(itemList[i]))
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
                    for(int i = itemList.Count - 1; i > -1; i--)
                    {
                        switch (end)
                        {
                            case 0:
                                if (itemList[i].ItemType == "BodyArmours" && !newItemOrderList.Contains(itemList[i]))
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
                                else if ((itemList[i].ItemType == "OneHandWeapons") && !newItemOrderList.Contains(itemList[i]))
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
                                else if (itemList[i].ItemType == "OneHandWeapons" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;

                            case 3:
                                if (itemList[i].ItemType == "Gloves" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 4:
                                if (itemList[i].ItemType == "Helmets" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 5:
                                if (itemList[i].ItemType == "Boots" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 6:
                                if (itemList[i].ItemType == "Belts" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 7:
                                if (itemList[i].ItemType == "Rings" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 8:
                                if (itemList[i].ItemType == "Rings" && !newItemOrderList.Contains(itemList[i]))
                                {
                                    newItemOrderList.Add(itemList[i]);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 9:
                                if (itemList[i].ItemType == "Amulets" && !newItemOrderList.Contains(itemList[i]))
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
        public static void ActivateNextCell(bool active)
        {
            if (active)
            {
                if (GlobalItemOrderList.Count > 0)
                {

                    foreach (StashTab s in StashTabList.StashTabs)
                    {
                        s.TabHeaderColor = Brushes.Transparent;
                        foreach (Cell c in s.OverlayCellsList)
                        {
                            c.Active = false;
                        }
                    }
                    StashTabList.StashTabs[GlobalItemOrderList[0].StashTabIndex].ActivateItemCells(GlobalItemOrderList[0]);
                    if (Properties.Settings.Default.ColorStash != "")
                    {
                        StashTabList.StashTabs[GlobalItemOrderList[0].StashTabIndex].TabHeaderColor = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorStash));
                    }
                    else
                    {
                        StashTabList.StashTabs[GlobalItemOrderList[0].StashTabIndex].TabHeaderColor = Brushes.Red;
                    }
                    GlobalItemOrderList.RemoveAt(0);
                }
                else
                {
                    foreach(StashTab s in StashTabList.StashTabs)
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
        //private static void ReIndexAllStashTabs()
        //{
        //    if(StashTabList.StashTabs != null)
        //    {
        //        for (int i = 0; i < StashTabList.StashTabs.Count; i++)
        //        {
        //            StashTabList.StashTabs[i].TabIndex = i;
        //        }
        //        ReIndexAllItems();
        //    }
        //}
        //private static void ReIndexAllItems()
        //{
        //    if(StashTabList.StashTabs != null)
        //    {
        //        foreach(StashTab s in StashTabList.StashTabs)
        //        {
        //            if(s.ItemList != null)
        //            {
        //                foreach (Item i in s.ItemList)
        //                {
        //                    i.StashTabIndex = s.TabIndex;
        //                }
        //            }
        //            if (Properties.Settings.Default.ExaltedRecipe)
        //            {
        //                foreach(Item i in s.ItemListShaper)
        //                {
        //                    i.StashTabIndex = s.TabIndex;
        //                }
        //                foreach (Item i in s.ItemListElder)
        //                {
        //                    i.StashTabIndex = s.TabIndex;
        //                }
        //                foreach (Item i in s.ItemListCrusader)
        //                {
        //                    i.StashTabIndex = s.TabIndex;
        //                }
        //                foreach (Item i in s.ItemListHunter)
        //                {
        //                    i.StashTabIndex = s.TabIndex;
        //                }
        //                foreach (Item i in s.ItemListWarlord)
        //                {
        //                    i.StashTabIndex = s.TabIndex;
        //                }
        //                foreach (Item i in s.ItemListRedeemer)
        //                {
        //                    i.StashTabIndex = s.TabIndex;
        //                }
        //            }
        //        }
        //    }
        //}

        private static void ClearAllItemOrderLists()
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

        public static void PrepareSelling()
        {
            ClearAllItemOrderLists();
            //ReIndexAllStashTabs();
            foreach(StashTab s in StashTabList.StashTabs)
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

    public class ActiveItemTypes
    {
        public bool GlovesActive { get; set; } = true;
        public bool HelmetActive { get; set; } = true;
        public bool BootsActive { get; set; } = true;
        public bool ChestActive { get; set; } = true;
        public bool WeaponActive { get; set; } = true;
    }
}
