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

        public static int SetAmount { get; set; } = 0;
        public static int SetTargetAmount { get; set; } = 0;

        public static List<ItemSet> ItemSetList { get; set; }
        public static List<ItemSet> ItemSetListHighlight { get; set; } = new List<ItemSet>();

        public static ItemSet ItemSetShaper { get; set; }
        public static ItemSet ItemSetElder { get; set; }
        public static ItemSet ItemSetWarlord { get; set; }
        public static ItemSet ItemSetCrusader { get; set; }
        public static ItemSet ItemSetRedeemer { get; set; }
        public static ItemSet ItemSetHunter { get; set; }

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

        private static void GenerateInfluencedItemSets()
        {
            ItemSetShaper = new ItemSet { InfluenceType = "shaper" };
            ItemSetElder = new ItemSet { InfluenceType = "elder" };
            ItemSetWarlord = new ItemSet { InfluenceType = "warlord" };
            ItemSetHunter = new ItemSet { InfluenceType = "hunter" };
            ItemSetCrusader = new ItemSet { InfluenceType = "crusader" };
            ItemSetRedeemer = new ItemSet { InfluenceType = "redeemer" };
        }

        private static void GenerateItemSetList()
        {
            List<ItemSet> ret = new List<ItemSet>();
            for(int i = 0; i < SetTargetAmount; i++)
            {
                ret.Add(new ItemSet());
            }
            ItemSetList = ret;
            Trace.WriteLine(ItemSetList.Count, "item set list count");
            if (Properties.Settings.Default.ExaltedRecipe)
            {
                GenerateInfluencedItemSets();
            }
        }

        // tries to add item, if item added returns
        private static bool AddChaosItemToItemSet(ItemSet set)
        {
            foreach (StashTab s in StashTabList.StashTabs)
            {
                foreach (Item item in s.ItemListChaos)
                {
                    if (set.AddItem(item))
                    {
                        s.ItemListChaos.Remove(item);
                        return true;
                    }
                }
            }
            return false;
        }

        // keeps adding items, breaks when full
        private static void FillItemSetWithRegalItems(ItemSet set)
        {
            foreach (StashTab s in StashTabList.StashTabs)
            {
                foreach(Item item in s.ItemList)
                {
                    if (set.AddItem(item))
                    {
                        s.ItemList.Remove(item);
                    }
                    if (set.EmptyItemSlots.Count == 0)
                    {
                        return;
                    }
                }
            }
        }

        // keeps adding items breaks when full
        private static void FillItemSetWithChaosItems(ItemSet set)
        {
            foreach (StashTab s in StashTabList.StashTabs)
            {
                foreach (Item item in s.ItemListChaos)
                {
                    if (set.AddItem(item))
                    {
                        s.ItemListChaos.Remove(item);
                    }
                    if (set.EmptyItemSlots.Count == 0)
                    {
                        return;
                    }
                }
            }
        }

        private static void FillItemSets()
        {
            foreach(ItemSet i in ItemSetList)
            {
                AddChaosItemToItemSet(i);
                FillItemSetWithRegalItems(i);
                FillItemSetWithChaosItems(i);
            }
            if (Properties.Settings.Default.ExaltedRecipe)
            {
                FillItemSetsInfluenced();
            }
        }

        private static void FillItemSetsInfluenced()
        {
            foreach(StashTab tab in StashTabList.StashTabs)
            {
                foreach(Item i in tab.ItemListShaper)
                {
                    if (ItemSetShaper.EmptyItemSlots.Count == 0)
                    {
                        break;
                    }
                    ItemSetShaper.AddItem(i);
                }
                foreach(Item i in tab.ItemListElder)
                {
                    if (ItemSetElder.EmptyItemSlots.Count == 0)
                    {
                        break;
                    }
                    ItemSetElder.AddItem(i);
                }
                foreach (Item i in tab.ItemListCrusader)
                {
                    if (ItemSetCrusader.EmptyItemSlots.Count == 0)
                    {
                        break;
                    }
                    ItemSetCrusader.AddItem(i);
                }
                foreach (Item i in tab.ItemListWarlord)
                {
                    if (ItemSetWarlord.EmptyItemSlots.Count == 0)
                    {
                        break;
                    }
                    ItemSetWarlord.AddItem(i);
                }
                foreach (Item i in tab.ItemListRedeemer)
                {
                    if (ItemSetRedeemer.EmptyItemSlots.Count == 0)
                    {
                        break;
                    }
                    ItemSetRedeemer.AddItem(i);
                }
                foreach (Item i in tab.ItemListHunter)
                {
                    if (ItemSetHunter.EmptyItemSlots.Count == 0)
                    {
                        break;
                    }
                    ItemSetHunter.AddItem(i);
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
            if (StashTabList.StashTabs != null)
            {
                foreach (StashTab s in StashTabList.StashTabs)
                {
                    GetSetTargetAmount(s);
                }
            }
            GenerateItemSetList();
            FillItemSets();

            // check for full sets/ missing items
            bool missingChaos = false;
            int fullSets = 0;
            // unique missing item classes
            HashSet<string> missingItemClasses = new HashSet<string>();
            List<string> deactivatedItemClasses = new List<string> {"Helmets", "BodyArmours", "Gloves", "Boots", "Rings", "Amulets", "Belts", "OneHandWeapons", "TwoHandWeapons" };

            foreach (ItemSet set in ItemSetList)
            {
                if(set.EmptyItemSlots.Count == 0)
                {
                    if (set.HasChaos)
                    {
                        fullSets++;
                    }
                    else
                    {
                        missingChaos = true;
                    }
                }
                else
                {
                    // all classes which are active over all ilvls
                    foreach(string itemClass in set.EmptyItemSlots)
                    {
                        missingItemClasses.Add(itemClass);
                    }
                }
            }

            List<string> sectionList = new List<string>();

            if (filterActive)
            {
                FilterGeneration.LoadCustomStyle();
                if (Properties.Settings.Default.ExaltedRecipe)
                {
                    FilterGeneration.LoadCustomStyleInfluenced();
                }
            }

            if (fullSets == SetTargetAmount && missingChaos)
            {
                // activate only chaos items
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, "Body Armours", false, true));
                    sectionList.Add(FilterGeneration.GenerateSection(true, "Helmets", false, true));
                    sectionList.Add(FilterGeneration.GenerateSection(true, "Gloves", false, true));
                    sectionList.Add(FilterGeneration.GenerateSection(true, "Boots", false, true));
                    sectionList.Add(FilterGeneration.GenerateSection(true, "OneHandWeapons", false, true));
                    sectionList.Add(FilterGeneration.GenerateSection(true, "TwoHandWeapons", false, true));
                }
                ActiveItems.GlovesActive = false;
                ActiveItems.HelmetActive = false;
                ActiveItems.ChestActive = false;
                ActiveItems.BootsActive = false;
                ActiveItems.WeaponActive = false;
                ActiveItems.ChaosMissing = true;
            }
            else if(fullSets == SetTargetAmount && !missingChaos)
            {
                //deactivate all
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, "Body Armours"));
                    sectionList.Add(FilterGeneration.GenerateSection(false, "Helmets"));
                    sectionList.Add(FilterGeneration.GenerateSection(false, "Gloves"));
                    sectionList.Add(FilterGeneration.GenerateSection(false, "Boots"));
                    sectionList.Add(FilterGeneration.GenerateSection(false, "OneHandWeapons"));
                    sectionList.Add(FilterGeneration.GenerateSection(false, "TwoHandWeapons"));
                }
                ActiveItems.GlovesActive = false;
                ActiveItems.HelmetActive = false;
                ActiveItems.ChestActive = false;
                ActiveItems.BootsActive = false;
                ActiveItems.WeaponActive = false;
                ActiveItems.ChaosMissing = false;
            }
            else
            {
                // activate missing classes
                foreach (string itemClass in missingItemClasses) 
                {
                    switch (itemClass)
                    {
                        case "BodyArmours":
                            sectionList.Add(FilterGeneration.GenerateSection(true, "Body Armours"));
                            ActiveItems.ChestActive = true;
                            break;
                        case "Helmets":
                            sectionList.Add(FilterGeneration.GenerateSection(true, "Helmets"));
                            ActiveItems.HelmetActive = true;
                            break;
                        case "Gloves":
                            sectionList.Add(FilterGeneration.GenerateSection(true, "Gloves"));
                            ActiveItems.GlovesActive = true;
                            break;
                        case "Boots":
                            sectionList.Add(FilterGeneration.GenerateSection(true, "Boots"));
                            ActiveItems.BootsActive = true;
                            break;
                        case "OneHandWeapons":
                            sectionList.Add(FilterGeneration.GenerateSection(true, "OneHandWeapons"));
                            ActiveItems.WeaponActive = true;
                            break;
                        case "TwoHandWeapons":
                            sectionList.Add(FilterGeneration.GenerateSection(true, "TwoHandWeapons"));
                            ActiveItems.WeaponActive = true;
                            break;
                    }
                    deactivatedItemClasses.Remove(itemClass);
                    //ActiveItems.ChaosMissing = true;
                }
                //deactivate rest
                foreach (string itemClass in deactivatedItemClasses)
                {
                    switch (itemClass)
                    {
                        case "BodyArmours":
                            sectionList.Add(FilterGeneration.GenerateSection(false, "Body Armours"));
                            ActiveItems.ChestActive = false;
                            break;
                        case "Helmets":
                            sectionList.Add(FilterGeneration.GenerateSection(false, "Helmets"));
                            ActiveItems.HelmetActive = false;
                            break;
                        case "Gloves":
                            sectionList.Add(FilterGeneration.GenerateSection(false, "Gloves"));
                            ActiveItems.GlovesActive = false;
                            break;
                        case "Boots":
                            sectionList.Add(FilterGeneration.GenerateSection(false, "Boots"));
                            ActiveItems.BootsActive = false;
                            break;
                        case "OneHandWeapons":
                            sectionList.Add(FilterGeneration.GenerateSection(false, "OneHandWeapons"));
                            ActiveItems.WeaponActive = false;
                            break;
                        case "TwoHandWeapons":
                            ActiveItems.WeaponActive = false;
                            sectionList.Add(FilterGeneration.GenerateSection(false, "TwoHandWeapons"));
                            break;
                    }
                }
            }

            // always on
            if (filterActive)
            {
                sectionList.Add(FilterGeneration.GenerateSection(true, "Rings"));
                sectionList.Add(FilterGeneration.GenerateSection(true, "Amulets"));
                sectionList.Add(FilterGeneration.GenerateSection(true, "Belts"));
            }

            //Trace.WriteLine(fullSets, "full sets");
            MainWindow.overlay.Dispatcher.Invoke(() =>
            {
                MainWindow.overlay.FullSetsTextBlock.Text = fullSets.ToString();
            });

            // invoke chaos missing
            if (missingChaos)
            {
                //ChaosRecipeEnhancer.WarningMessage = "Need lower level items!";
                MainWindow.overlay.WarningMessage = "Need lower level items!";
                MainWindow.overlay.ShadowOpacity = 1;
                MainWindow.overlay.WarningMessageVisibility = System.Windows.Visibility.Visible;
            }

            // invoke exalted recipe ready
            if (Properties.Settings.Default.ExaltedRecipe)
            {
                if(ItemSetShaper.EmptyItemSlots.Count == 0
                    || ItemSetElder.EmptyItemSlots.Count == 0
                    || ItemSetCrusader.EmptyItemSlots.Count == 0
                    || ItemSetWarlord.EmptyItemSlots.Count == 0
                    || ItemSetHunter.EmptyItemSlots.Count == 0
                    || ItemSetRedeemer.EmptyItemSlots.Count == 0)
                {
                    MainWindow.overlay.WarningMessage = "Exalted Recipe ready!";
                    MainWindow.overlay.ShadowOpacity = 1;
                    MainWindow.overlay.WarningMessageVisibility = System.Windows.Visibility.Visible;
                }
            }

            // invoke set full
            if(fullSets == SetTargetAmount)
            {
                MainWindow.overlay.WarningMessage = "Sets full!";
                MainWindow.overlay.ShadowOpacity = 1;
                MainWindow.overlay.WarningMessageVisibility = System.Windows.Visibility.Visible;
            }

            Trace.WriteLine(fullSets, "full sets");





            //Dictionary<string, int> globalAmount = GetFullSets(GlobalNormalItemList);

                //if (exaltedActive)
                //{
                //    globalAmount["sets"] += GetFullSets(GlobalShaperItemList)["sets"];
                //    globalAmount["sets"] += GetFullSets(GlobalElderItemList)["sets"];
                //    globalAmount["sets"] += GetFullSets(GlobalWarlordItemList)["sets"];
                //    globalAmount["sets"] += GetFullSets(GlobalCrusaderItemList)["sets"];
                //    globalAmount["sets"] += GetFullSets(GlobalHunterItemList)["sets"];
                //    globalAmount["sets"] += GetFullSets(GlobalRedeemerItemList)["sets"];
                //}

                //SetAmount = globalAmount["sets"];



                //if (Properties.Settings.Default.TwoHand)
                //{
                //    if (globalAmount["weapons"] >= SetTargetAmount)
                //    {
                //        if (filterActive)
                //        {
                //            sectionList.Add(FilterGeneration.GenerateSection(false, "TwoHandWeapons"));
                //        }
                //    }
                //    else
                //    {
                //        if (filterActive)
                //        {
                //            sectionList.Add(FilterGeneration.GenerateSection(true, "TwoHandWeapons"));
                //        }
                //    }
                ////}
                //if(globalAmount["weapons"] >= SetTargetAmount)
                //{
                //    ActiveItems.WeaponActive = false;
                //    if (filterActive)
                //    {
                //        sectionList.Add(FilterGeneration.GenerateSection(false, "OneHandWeapons"));
                //    }
                //}
                //else
                //{
                //    ActiveItems.WeaponActive = true;
                //    if (filterActive)
                //    {
                //        sectionList.Add(FilterGeneration.GenerateSection(true, "OneHandWeapons"));
                //    }
                //}
                //if(globalAmount["chests"] >= SetTargetAmount)
                //{
                //    ActiveItems.ChestActive = false;
                //    if (filterActive)
                //    {
                //        sectionList.Add(FilterGeneration.GenerateSection(false, "Body Armours"));
                //    }
                //}
                //else
                //{
                //    ActiveItems.ChestActive = true;
                //    if (filterActive)
                //    {
                //        sectionList.Add(FilterGeneration.GenerateSection(true, "Body Armours"));

                //    }
                //}
                //if (globalAmount["boots"] >= SetTargetAmount)
                //{
                //    ActiveItems.BootsActive = false;
                //    if (filterActive)
                //    {
                //        sectionList.Add(FilterGeneration.GenerateSection(false, "Boots"));

                //    }
                //}
                //else
                //{
                //    ActiveItems.BootsActive = true;
                //    if (filterActive)
                //    {
                //        sectionList.Add(FilterGeneration.GenerateSection(true, "Boots"));

                //    }
                //}
                //if (globalAmount["gloves"] >= SetTargetAmount)
                //{
                //    ActiveItems.GlovesActive = false;
                //    if (filterActive)
                //    {
                //        sectionList.Add(FilterGeneration.GenerateSection(false, "Gloves"));

                //    }
                //}
                //else
                //{
                //    ActiveItems.GlovesActive = true;
                //    if (filterActive)
                //    {
                //        sectionList.Add(FilterGeneration.GenerateSection(true, "Gloves"));

                //    }
                //}
                //if (globalAmount["helmets"] >= SetTargetAmount)
                //{
                //    ActiveItems.HelmetActive = false;
                //    if (filterActive)
                //    {
                //        sectionList.Add(FilterGeneration.GenerateSection(false, "Helmets"));
                //    }
                //}
                //else
                //{
                //    ActiveItems.HelmetActive = true;
                //    if (filterActive)
                //    {
                //        sectionList.Add(FilterGeneration.GenerateSection(true, "Helmets"));
                //    }
                //}



            if (filterActive)
            {
                //sectionList.Add(FilterGeneration.GenerateSection(true, "Rings"));
                //sectionList.Add(FilterGeneration.GenerateSection(true, "Amulets"));
                //sectionList.Add(FilterGeneration.GenerateSection(true, "Belts"));

                string oldFilter = FilterGeneration.OpenLootfilter();
                string newFilter = FilterGeneration.GenerateLootFilter(oldFilter, sectionList);
                FilterGeneration.WriteLootfilter(newFilter);

                //if (Properties.Settings.Default.ExaltedRecipe)
                //{
                //    List<string> sectionListInfluenced = new List<string>();
                //    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Rings", true));
                //    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Amulets", true));
                //    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Belts", true));
                //    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Helmets", true));
                //    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Gloves", true));
                //    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Boots", true));
                //    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Body Armours", true));
                //    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "OneHandWeapons", true));
                //    sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "TwoHandWeapons", true));

                //    string oldFilter2 = FilterGeneration.OpenLootfilter();
                //    string newFilter2 = FilterGeneration.GenerateLootFilterInfluenced(oldFilter2, sectionListInfluenced);
                //    FilterGeneration.WriteLootfilter(newFilter2);
                //}
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

        public static StashTab GetStashTabFromItem(Item item)
        {
            foreach(StashTab s in StashTabList.StashTabs)
            {
                if(item.StashTabIndex == s.TabIndex)
                {
                    return s;
                }
            }
            return null;
        }

        // 
        public static void ActivateNextCell(bool active, Cell cell)
        {
            if (active)
            {
                if(Properties.Settings.Default.HighlightMode == 0)
                {
                    //activate cell by cell
                    foreach(StashTab s in StashTabList.StashTabs)
                    {
                        s.DeactivateItemCells();
                        s.TabHeaderColor = Brushes.Transparent;
                    }
                    if(ItemSetListHighlight.Count > 0)
                    {
                        // check for full sets
                        if(ItemSetListHighlight[0].ItemList.Count > 0 && ItemSetListHighlight[0].EmptyItemSlots.Count == 0)
                        {
                            Item highlightItem = ItemSetListHighlight[0].ItemList[0];
                            StashTab currentTab = GetStashTabFromItem(highlightItem);
                            if(currentTab != null)
                            {
                                currentTab.ActivateItemCells(highlightItem);
                                if (Properties.Settings.Default.ColorStash != "")
                                {
                                    currentTab.TabHeaderColor = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorStash));
                                }
                                else
                                {
                                    currentTab.TabHeaderColor = Brushes.Red;
                                }
                                ItemSetListHighlight[0].ItemList.RemoveAt(0);
                                if (ItemSetListHighlight[0].ItemList.Count == 0)
                                {
                                    ItemSetListHighlight.RemoveAt(0);
                                }
                            }
                        }
                    }
                }
                else if(Properties.Settings.Default.HighlightMode == 1)
                {
                    // activate whole set 
                    if (ItemSetListHighlight.Count > 0)
                    {
                        Trace.WriteLine(ItemSetListHighlight[0].ItemList.Count, "item list count");
                        Trace.WriteLine(ItemSetListHighlight.Count, "itemset list ocunt");
                        // check for full sets

                        if (ItemSetListHighlight[0].EmptyItemSlots.Count == 0)
                        {
                            if(cell != null)
                            {
                                Item highlightItem = cell.CellItem;
                                StashTab currentTab = GetStashTabFromItem(highlightItem);
                                if (currentTab != null)
                                {
                                    currentTab.DeactivateSingleItemCells(cell.CellItem);
                                    currentTab.TabHeaderColor = Brushes.Transparent;
                                    ItemSetListHighlight[0].ItemList.Remove(highlightItem);
                                }
                            }
                            //Trace.WriteLine("IS HERE IS HERE IS HERE");

                            foreach (Item i in ItemSetListHighlight[0].ItemList)
                            {
                                //currentTab.ActivateItemCells(i);
                                StashTab currTab = GetStashTabFromItem(i);
                                currTab.ActivateItemCells(i);
                                //currTab.ShowNumbersOnActiveCells();
                                if (Properties.Settings.Default.ColorStash != "")
                                {
                                    currTab.TabHeaderColor = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorStash));
                                }
                                else
                                {
                                    currTab.TabHeaderColor = Brushes.Red;
                                }
                            }

                            // mark item order
                            if (ItemSetListHighlight[0] != null)
                            {
                                if(ItemSetListHighlight[0].ItemList.Count > 0)
                                {
                                    StashTab cTab = GetStashTabFromItem(ItemSetListHighlight[0].ItemList[0]);
                                    cTab.MarkNextItem(ItemSetListHighlight[0].ItemList[0]);
                                }
                            }
                            if (ItemSetListHighlight[0].ItemList.Count == 0)
                            {
                                //Trace.WriteLine("itemsetlist gets removes0");
                                ItemSetListHighlight.RemoveAt(0);
                                // activate next set
                                ActivateNextCell(true, null);
                            }
                        }
                    }
                }
                else if(Properties.Settings.Default.HighlightMode == 2)
                {
                    //activate all cells at once
                    if(ItemSetListHighlight.Count > 0)
                    {
                        foreach(ItemSet set in ItemSetListHighlight)
                        {
                            if(set.EmptyItemSlots.Count == 0)
                            {
                                if (cell != null)
                                {
                                    Item highlightItem = cell.CellItem;
                                    StashTab currentTab = GetStashTabFromItem(highlightItem);
                                    if (currentTab != null)
                                    {
                                        currentTab.DeactivateSingleItemCells(cell.CellItem);
                                        currentTab.TabHeaderColor = Brushes.Transparent;
                                        ItemSetListHighlight[0].ItemList.Remove(highlightItem);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void PrepareSelling()
        {
            //ClearAllItemOrderLists();
            ItemSetListHighlight.Clear();
            if (ApiAdapter.IsFetching)
            {
                return;
            }
            foreach (StashTab s in StashTabList.StashTabs)
            {
                s.PrepareOverlayList();
            }
            foreach(ItemSet itemSet in ItemSetList)
            {
                itemSet.OrderItems();
                //GlobalItemOrderList.AddRange(itemSet.ItemList);
            }
            if (Properties.Settings.Default.ExaltedRecipe)
            {
                ItemSetShaper.OrderItems();
                ItemSetElder.OrderItems();
                ItemSetWarlord.OrderItems();
                ItemSetCrusader.OrderItems();
                ItemSetHunter.OrderItems();
                ItemSetRedeemer.OrderItems();
                if(ItemSetShaper.EmptyItemSlots.Count == 0)
                {
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetShaper.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetShaper.EmptyItemSlots)
                    });
                }
                if(ItemSetElder.EmptyItemSlots.Count == 0) 
                {
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetElder.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetElder.EmptyItemSlots)
                    });
                }
                if(ItemSetCrusader.EmptyItemSlots.Count == 0) {
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetCrusader.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetCrusader.EmptyItemSlots)
                    });
                }
                if(ItemSetHunter.EmptyItemSlots.Count == 0) {
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetHunter.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetHunter.EmptyItemSlots)
                    });
                }
                if(ItemSetWarlord.EmptyItemSlots.Count == 0) {
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetWarlord.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetWarlord.EmptyItemSlots)
                    });
                }
                if(ItemSetRedeemer.EmptyItemSlots.Count == 0) {
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetRedeemer.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetRedeemer.EmptyItemSlots)
                    });
                }
            }
            //ItemSetListHighlight = new List<ItemSet>(ItemSetList);
            foreach(ItemSet set in ItemSetList)
            {
                ItemSetListHighlight.Add(new ItemSet
                {
                    ItemList = new List<Item>(set.ItemList),
                    EmptyItemSlots = new List<string>(set.EmptyItemSlots)
                });
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
        public bool ChaosMissing { get; set; } = true;
    }
}
