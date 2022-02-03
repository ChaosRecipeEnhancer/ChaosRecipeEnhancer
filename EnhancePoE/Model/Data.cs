using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using EnhancePoE.Model;
using EnhancePoE.Model.Storage;
using EnhancePoE.Properties;

namespace EnhancePoE
{
    public static class Data
    {
        public static ActiveItemTypes ActiveItems { get; set; } = new ActiveItemTypes();
        public static ActiveItemTypes PreviousActiveItems { get; set; }
        public static MediaPlayer Player { get; set; } = new MediaPlayer();

        public static MediaPlayer PlayerSet { get; set; } = new MediaPlayer();

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

        public static CancellationTokenSource cs { get; set; } = new CancellationTokenSource();
        public static CancellationToken CancelationToken { get; set; } = cs.Token;

        public static void GetSetTargetAmount(StashTab stash)
        {
            if (Settings.Default.Sets > 0)
            {
                SetTargetAmount = Settings.Default.Sets;
            }
            else
            {
                if (stash.Quad)
                    SetTargetAmount += 16;
                else
                    SetTargetAmount += 4;
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
            var ret = new List<ItemSet>();
            for (var i = 0; i < SetTargetAmount; i++) ret.Add(new ItemSet());

            ItemSetList = ret;
            Trace.WriteLine(ItemSetList.Count, "item set list count");
            if (Settings.Default.ExaltedRecipe) GenerateInfluencedItemSets();
        }


        // tries to add item, if item added returns
        private static bool AddItemToItemSet(ItemSet set, bool chaosItems = false, bool honorOrder = true)
        {
            string listName;
            switch (chaosItems)
            {
                case false:
                    listName = "ItemList";
                    break;
                case true:
                    listName = "ItemListChaos";
                    break;
                default:
                    throw new Exception("How did you manage to provide a value neither true or false for a boolean?");
            }

            Item minItem = null;
            var minDistance = double.PositiveInfinity;
            
            // TODO: crashes here after some time
            foreach (var s in StashTabList.StashTabs)
            foreach (var i in (List<Item>)Utility.GetPropertyValue(s, listName))
                if (set.GetNextItemClass() == i.ItemType || !honorOrder && set.IsValidItem(i))
                    if (set.GetItemDistance(i) < minDistance)
                    {
                        //Trace.WriteLine(minDistance, "minDistance");
                        minDistance = set.GetItemDistance(i);
                        minItem = i;
                    }

            if (minItem != null)
            {
                set.AddItem(minItem);
                var tab = GetStashTabFromItem(minItem);
                ((List<Item>)Utility.GetPropertyValue(tab, listName)).Remove(minItem);
                //tab.ItemListChaos.Remove(minItem);
                return true;
            }

            // Looks ugly but in case we allow TwoHandWeapons we need to consider that adding a 1H fails but we might have a 2H (this only applies if we honor the order)
            if (honorOrder)
            {
                var nextItemType = set.GetNextItemClass();
                if (nextItemType == "TwoHandWeapons")
                {
                    nextItemType = "OneHandWeapons";
                    foreach (var s in StashTabList.StashTabs)
                    foreach (var i in (List<Item>)Utility.GetPropertyValue(s, listName))
                        if (nextItemType == i.ItemType)
                            if (set.GetItemDistance(i) < minDistance)
                            {
                                //Trace.WriteLine(minDistance, "minDistance");
                                minDistance = set.GetItemDistance(i);
                                minItem = i;
                            }

                    if (minItem != null)
                    {
                        set.AddItem(minItem);
                        var tab = GetStashTabFromItem(minItem);
                        ((List<Item>)Utility.GetPropertyValue(tab, listName)).Remove(minItem);
                        //tab.ItemListChaos.Remove(minItem);
                        return true;
                    }
                }
            }

            return false;
        }

        private static void FillItemSets()
        {
            foreach (var i in ItemSetList)
            {
                // Try to fill the set in order until one chaos item is present, lastEmptySlots counter prevents infinite loops
                var lastEmptySlots = 0;
                while (i.EmptyItemSlots.Count > 0 && lastEmptySlots != i.EmptyItemSlots.Count)
                {
                    lastEmptySlots = i.EmptyItemSlots.Count;
                    if (i.SetCanProduceChaos == false && !Settings.Default.RegalRecipe)
                        if (AddItemToItemSet(i, true))
                            continue;

                    if (!AddItemToItemSet(i))
                        if (Settings.Default.FillWithChaos && !Settings.Default.RegalRecipe)
                            AddItemToItemSet(i, true);
                }

                /* At this point in time the following conditions may be met, exclusively
                 * 1.) We obtained a full set and it contains one chaos item
                 * 1.1) We obtained a full set and it contains multiple chaos items (only if filling with chaos items is allowed)
                 * 2.) We obtained a full set without a chaos item -> We aren't lacking a regal item in this set but we don't have enough chaos items. 
                 * 3.) We couldn't obtain a full set. That means that at least one item slot is missing. We need to check which of the remaining slots we can still fill. We could still be missing a chaos item.
                 */
                if (i.EmptyItemSlots.Count == 0 && (i.SetCanProduceChaos || Settings.Default.RegalRecipe))
                    // Set full, continue
                    continue;

                if (i.EmptyItemSlots.Count > 0)
                {
                    lastEmptySlots = 0;
                    while (i.EmptyItemSlots.Count > 0 && i.EmptyItemSlots.Count != lastEmptySlots)
                    {
                        lastEmptySlots = i.EmptyItemSlots.Count;
                        if (!i.SetCanProduceChaos && !Settings.Default.RegalRecipe)
                            if (AddItemToItemSet(i, true, false))
                                continue;

                        if (!AddItemToItemSet(i, false, false))
                            // couldn't add a regal item. Try chaos item if filling with chaos is allowed
                            if (Settings.Default.FillWithChaos && !Settings.Default.RegalRecipe)
                                AddItemToItemSet(i, true, false);
                    }
                    // At this point the set will contain a chaos item as long as we had at least one left. If not we didn't have any chaos items left.
                    // If the set is not full at this time we're missing at least one regal item. If it has not chaos item we're also missing chaos items. 
                    // Technically it could be only the chaos item that's missing but that can be neglected since when mixing you'll always be short on chaos items. 
                    // If not in "endgame" mode (always show chaos) have the loot filter apply to chaos and regal items the same way.
                }
            }

            if (Settings.Default.ExaltedRecipe) FillItemSetsInfluenced();
        }

        private static void FillItemSetsInfluenced()
        {
            foreach (var tab in StashTabList.StashTabs)
            {
                foreach (var i in tab.ItemListShaper)
                {
                    if (ItemSetShaper.EmptyItemSlots.Count == 0) break;

                    ItemSetShaper.AddItem(i);
                }

                foreach (var i in tab.ItemListElder)
                {
                    if (ItemSetElder.EmptyItemSlots.Count == 0) break;

                    ItemSetElder.AddItem(i);
                }

                foreach (var i in tab.ItemListCrusader)
                {
                    if (ItemSetCrusader.EmptyItemSlots.Count == 0) break;

                    ItemSetCrusader.AddItem(i);
                }

                foreach (var i in tab.ItemListWarlord)
                {
                    if (ItemSetWarlord.EmptyItemSlots.Count == 0) break;

                    ItemSetWarlord.AddItem(i);
                }

                foreach (var i in tab.ItemListRedeemer)
                {
                    if (ItemSetRedeemer.EmptyItemSlots.Count == 0) break;

                    ItemSetRedeemer.AddItem(i);
                }

                foreach (var i in tab.ItemListHunter)
                {
                    if (ItemSetHunter.EmptyItemSlots.Count == 0) break;

                    ItemSetHunter.AddItem(i);
                }
            }
        }

        public static void GetAlwaysActive()
        {
            if (Settings.Default.RingsAlwaysActive) ActiveItems.RingActive = true;

            if (Settings.Default.AmuletsAlwaysActive) ActiveItems.AmuletActive = true;

            if (Settings.Default.BeltsAlwaysActive) ActiveItems.BeltActive = true;

            if (Settings.Default.GlovesAlwaysActive) ActiveItems.GlovesActive = true;

            if (Settings.Default.BootsAlwaysActive) ActiveItems.BootsActive = true;

            if (Settings.Default.HelmetsAlwaysActive) ActiveItems.HelmetActive = true;

            if (Settings.Default.ChestsAlwaysActive) ActiveItems.ChestActive = true;

            if (Settings.Default.WeaponsAlwaysActive) ActiveItems.WeaponActive = true;
        }

        public static async Task CheckActives()
        {
            try
            {
                if (ApiAdapter.FetchError)
                {
                    MainWindow.overlay.WarningMessage = "Fetching Error...";
                    MainWindow.overlay.ShadowOpacity = 1;
                    MainWindow.overlay.WarningMessageVisibility = Visibility.Visible;
                    return;
                }

                if (StashTabList.StashTabs.Count == 0)
                {
                    MainWindow.overlay.WarningMessage = "No Stashtabs found...";
                    MainWindow.overlay.ShadowOpacity = 1;
                    MainWindow.overlay.WarningMessageVisibility = Visibility.Visible;
                    return;
                }

                if (Settings.Default.Sound)
                {
                    PreviousActiveItems = new ActiveItemTypes
                    {
                        BootsActive = ActiveItems.BootsActive,
                        GlovesActive = ActiveItems.GlovesActive,
                        HelmetActive = ActiveItems.HelmetActive,
                        WeaponActive = ActiveItems.WeaponActive,
                        ChestActive = ActiveItems.ChestActive,
                        RingActive = ActiveItems.RingActive,
                        AmuletActive = ActiveItems.AmuletActive,
                        BeltActive = ActiveItems.BeltActive
                    };
                }
                
                var exaltedActive = Settings.Default.ExaltedRecipe;
                var filterActive = Settings.Default.LootfilterActive;

                SetTargetAmount = 0;
                
                if (StashTabList.StashTabs.Count > 0)
                {
                    foreach (var s in StashTabList.StashTabs)
                    {
                        GetSetTargetAmount(s);
                    }
                }

                if (Settings.Default.ShowItemAmount != 0)
                {
                    Trace.WriteLine("Calculating Items");
                    CalculateItemAmounts();
                }

                GenerateItemSetList();
                FillItemSets();

                // check for full sets/ missing items
                var missingGearPieceForChaosRecipe = false;
                var fullSets = 0;
                
                // unique missing item classes
                var missingItemClasses = new HashSet<string>();
                var deactivatedItemClasses = new List<string> { "Helmets", "BodyArmours", "Gloves", "Boots", "Rings", "Amulets", "Belts", "OneHandWeapons", "TwoHandWeapons" };

                foreach (var set in ItemSetList)
                {
                    if (set.EmptyItemSlots.Count == 0)
                    {
                        // fix for: condition (fullSets == SetTargetAmount && missingChaos)
                        // never true cause fullsets < settargetamount when missingChaos @ikogan
                        fullSets++;

                        if (!set.SetCanProduceChaos && !Settings.Default.RegalRecipe) missingGearPieceForChaosRecipe = true;

                        //if (set.HasChaos || Properties.Settings.Default.RegalRecipe)
                        //{
                        //    fullSets++;
                        //}
                        //else
                        //{
                        //    missingChaos = true;
                        //}
                    }
                    else
                    {
                        // all classes which are active over all ilvls
                        foreach (var itemClass in set.EmptyItemSlots) missingItemClasses.Add(itemClass);
                    }
                }
                
                var sectionList = new HashSet<string>();

                if (filterActive)
                {
                    FilterGeneration.LoadCustomStyle();
                    if (Settings.Default.ExaltedRecipe) FilterGeneration.LoadCustomStyleInfluenced();
                }

                if (fullSets == SetTargetAmount && missingGearPieceForChaosRecipe)
                {
                    Trace.WriteLine("filter here 1");
                    
                    // activate only chaos items
                    if (filterActive)
                    {
                        if (!Settings.Default.ChestsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Body Armours\"", false, true));

                        if (!Settings.Default.HelmetsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Helmets\"", false, true));

                        if (!Settings.Default.GlovesAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Gloves\"", false, true));

                        if (!Settings.Default.BootsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Boots\"", false, true));

                        if (!Settings.Default.WeaponsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"OneHandWeapons\"", false, true));

                        if (!Settings.Default.WeaponsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"TwoHandWeapons\"", false, true));

                        if (!Settings.Default.RingsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Rings\"", false, true));

                        if (!Settings.Default.AmuletsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Amulets\"", false, true));

                        if (!Settings.Default.BeltsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Belts\"", false, true));
                    }

                    ActiveItems.GlovesActive = false;
                    ActiveItems.HelmetActive = false;
                    ActiveItems.ChestActive = false;
                    ActiveItems.BootsActive = false;
                    ActiveItems.WeaponActive = false;
                    ActiveItems.RingActive = false;
                    ActiveItems.AmuletActive = false;
                    ActiveItems.BeltActive = false;

                    ActiveItems.ChaosMissing = true;
                }
                else if (fullSets == SetTargetAmount && !missingGearPieceForChaosRecipe)
                {
                    Trace.WriteLine("filter here 2");
   
                    ActiveItems.GlovesActive = false;
                    ActiveItems.HelmetActive = false;
                    ActiveItems.ChestActive = false;
                    ActiveItems.BootsActive = false;
                    ActiveItems.WeaponActive = false;
                    ActiveItems.RingActive = false;
                    ActiveItems.AmuletActive = false;
                    ActiveItems.BeltActive = false;

                    ActiveItems.ChaosMissing = false;
                }
                else
                {
                    Trace.WriteLine("filter here 3");
                    
                    // activate missing classes
                    foreach (var itemClass in missingItemClasses)
                    {
                        switch (itemClass)
                        {
                            case "BodyArmours":
                                if (!Settings.Default.ChestsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Body Armours\""));

                                ActiveItems.ChestActive = true;
                                break;
                            case "Helmets":
                                if (!Settings.Default.HelmetsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Helmets\""));

                                ActiveItems.HelmetActive = true;
                                break;
                            case "Gloves":
                                if (!Settings.Default.GlovesAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Gloves\""));

                                ActiveItems.GlovesActive = true;
                                break;
                            case "Boots":
                                if (!Settings.Default.BootsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Boots\""));

                                ActiveItems.BootsActive = true;
                                break;
                            case "Rings":
                                if (!Settings.Default.RingsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Rings\""));

                                ActiveItems.RingActive = true;
                                break;
                            case "Amulets":
                                if (!Settings.Default.AmuletsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Amulets\""));

                                ActiveItems.AmuletActive = true;
                                break;
                            case "Belts":
                                if (!Settings.Default.BeltsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Belts\""));

                                ActiveItems.BeltActive = true;
                                break;
                            case "OneHandWeapons":
                            case "TwoHandWeapons":
                                if (!Settings.Default.WeaponsAlwaysActive)
                                {
                                    sectionList.Add(FilterGeneration.GenerateSection(true, "\"TwoHandWeapons\""));
                                    sectionList.Add(FilterGeneration.GenerateSection(true, "\"OneHandWeapons\""));
                                }

                                ActiveItems.WeaponActive = true;
                                deactivatedItemClasses.Remove("OneHandWeapons");
                                deactivatedItemClasses.Remove("TwoHandWeapons");
                                break;
                        }

                        deactivatedItemClasses.Remove(itemClass);
                        //ActiveItems.ChaosMissing = true;
                    }

                    //deactivate rest
                    foreach (var itemClass in deactivatedItemClasses)
                        switch (itemClass)
                        {
                            case "BodyArmours":
                                //sectionList.Add(FilterGeneration.GenerateSection(false, "Body Armours"));
                                ActiveItems.ChestActive = false;
                                break;
                            case "Helmets":
                                //sectionList.Add(FilterGeneration.GenerateSection(false, "Helmets"));
                                ActiveItems.HelmetActive = false;
                                break;
                            case "Gloves":
                                //sectionList.Add(FilterGeneration.GenerateSection(false, "Gloves"));
                                ActiveItems.GlovesActive = false;
                                break;
                            case "Boots":
                                //sectionList.Add(FilterGeneration.GenerateSection(false, "Boots"));
                                ActiveItems.BootsActive = false;
                                break;
                            case "OneHandWeapons":
                                //sectionList.Add(FilterGeneration.GenerateSection(false, "OneHandWeapons"));
                                ActiveItems.WeaponActive = false;
                                break;
                            case "TwoHandWeapons":
                                ActiveItems.WeaponActive = false;
                                //sectionList.Add(FilterGeneration.GenerateSection(false, "TwoHandWeapons"));
                                break;
                            case "Rings":
                                ActiveItems.RingActive = false;
                                //sectionList.Add(FilterGeneration.GenerateSection(false, "TwoHandWeapons"));
                                break;
                            case "Amulets":
                                ActiveItems.AmuletActive = false;
                                //sectionList.Add(FilterGeneration.GenerateSection(false, "TwoHandWeapons"));
                                break;
                            case "Belts":
                                ActiveItems.BeltActive = false;
                                //sectionList.Add(FilterGeneration.GenerateSection(false, "TwoHandWeapons"));
                                break;
                        }
                }

                GetAlwaysActive();
                if (filterActive)
                {
                    if (Settings.Default.RingsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Rings\""));

                    if (Settings.Default.AmuletsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Amulets\""));

                    if (Settings.Default.BeltsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Belts\""));

                    if (Settings.Default.GlovesAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Gloves\""));

                    if (Settings.Default.BootsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Boots\""));

                    if (Settings.Default.HelmetsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Helmets\""));

                    if (Settings.Default.ChestsAlwaysActive) sectionList.Add(FilterGeneration.GenerateSection(true, "\"Body Armours\""));

                    if (Settings.Default.WeaponsAlwaysActive)
                    {
                        sectionList.Add(FilterGeneration.GenerateSection(true, "\"OneHandWeapons\""));
                        sectionList.Add(FilterGeneration.GenerateSection(true, "\"TwoHandWeapons\""));
                    }
                }

                if (!Settings.Default.ChaosRecipe && !Settings.Default.RegalRecipe)
                {
                    sectionList.Clear();
                    Trace.WriteLine("section list cleared");
                }

                // always on
                //if (filterActive)
                //{
                //    sectionList.Add(FilterGeneration.GenerateSection(true, "Rings"));
                //    sectionList.Add(FilterGeneration.GenerateSection(true, "Amulets"));
                //    sectionList.Add(FilterGeneration.GenerateSection(true, "Belts"));
                //}


                //Trace.WriteLine(fullSets, "full sets");
                MainWindow.overlay.Dispatcher.Invoke(() => { MainWindow.overlay.FullSetsText = fullSets.ToString(); });

                // invoke chaos missing
                if (missingGearPieceForChaosRecipe && !Settings.Default.RegalRecipe)
                {
                    MainWindow.overlay.WarningMessage = "Need lower level items!";
                    MainWindow.overlay.ShadowOpacity = 1;
                    MainWindow.overlay.WarningMessageVisibility = Visibility.Visible;
                }

                // invoke exalted recipe ready
                if (Settings.Default.ExaltedRecipe)
                    if (ItemSetShaper.EmptyItemSlots.Count == 0
                        || ItemSetElder.EmptyItemSlots.Count == 0
                        || ItemSetCrusader.EmptyItemSlots.Count == 0
                        || ItemSetWarlord.EmptyItemSlots.Count == 0
                        || ItemSetHunter.EmptyItemSlots.Count == 0
                        || ItemSetRedeemer.EmptyItemSlots.Count == 0)
                    {
                        MainWindow.overlay.WarningMessage = "Exalted Recipe ready!";
                        MainWindow.overlay.ShadowOpacity = 1;
                        MainWindow.overlay.WarningMessageVisibility = Visibility.Visible;
                    }

                // invoke set full
                if (fullSets == SetTargetAmount && !missingGearPieceForChaosRecipe)
                {
                    MainWindow.overlay.WarningMessage = "Sets full!";
                    MainWindow.overlay.ShadowOpacity = 1;
                    MainWindow.overlay.WarningMessageVisibility = Visibility.Visible;
                }

                Trace.WriteLine(fullSets, "full sets");


                if (filterActive)
                {
                    var filterStorage = FilterStorageFactory.Create(Settings.Default);

                    var oldFilter = await filterStorage.ReadLootFilterAsync();
                    if (oldFilter == null) return;

                    var newFilter = FilterGeneration.GenerateLootFilter(oldFilter, sectionList);
                    oldFilter = newFilter;

                    var sectionListInfluenced = new List<string>();
                    if (Settings.Default.ExaltedRecipe)
                    {
                        sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Rings", true));
                        sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Amulets", true));
                        sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Belts", true));
                        sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Helmets", true));
                        sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Gloves", true));
                        sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Boots", true));
                        sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "Body Armours", true));
                        sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "OneHandWeapons", true));
                        sectionListInfluenced.Add(FilterGeneration.GenerateSection(true, "TwoHandWeapons", true));

                        newFilter = FilterGeneration.GenerateLootFilterInfluenced(oldFilter, sectionListInfluenced);
                    }
                    else
                    {
                        newFilter = FilterGeneration.GenerateLootFilterInfluenced(oldFilter, sectionListInfluenced);
                    }

                    await filterStorage.WriteLootFilterAsync(newFilter);
                }

                // If the state of any gear slot changed, we play a sound
                if (Settings.Default.Sound)
                    if (!(PreviousActiveItems.GlovesActive == ActiveItems.GlovesActive
                          && PreviousActiveItems.BootsActive == ActiveItems.BootsActive
                          && PreviousActiveItems.HelmetActive == ActiveItems.HelmetActive
                          && PreviousActiveItems.ChestActive == ActiveItems.ChestActive
                          && PreviousActiveItems.WeaponActive == ActiveItems.WeaponActive
                          && PreviousActiveItems.RingActive == ActiveItems.RingActive
                          && PreviousActiveItems.AmuletActive == ActiveItems.AmuletActive
                          && PreviousActiveItems.BeltActive == ActiveItems.BeltActive))
                        Player.Dispatcher.Invoke(() => { PlayNotificationSound(); });
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == CancelationToken)
            {
                Trace.WriteLine("abort");
            }
        }

        public static void CalculateItemAmounts()
        {
            if (StashTabList.StashTabs != null)
            {
                Trace.WriteLine("calculating items amount");
                
                // 0: rings
                // 1: amulets
                // 2: belts
                // 3: chests
                // 4: weapons
                // 5: gloves
                // 6: helmets
                // 7: boots
                
                var amounts = new int[8];
                var weaponsSmall = 0;
                var weaponBig = 0;
                foreach (var tab in StashTabList.StashTabs)
                {
                    Trace.WriteLine("tab amount " + tab.ItemList.Count);
                    Trace.WriteLine("tab amount " + tab.ItemListChaos.Count);
                    
                    if (tab.ItemList.Count > 0)
                        foreach (var i in tab.ItemList)
                        {
                            Trace.WriteLine(i.ItemType);
                            if (i.ItemType == "Rings")
                                amounts[0]++;
                            else if (i.ItemType == "Amulets")
                                amounts[1]++;
                            else if (i.ItemType == "Belts")
                                amounts[2]++;
                            else if (i.ItemType == "BodyArmours")
                                amounts[3]++;
                            else if (i.ItemType == "TwoHandWeapons")
                                weaponBig++;
                            else if (i.ItemType == "OneHandWeapons")
                                weaponsSmall++;
                            else if (i.ItemType == "Gloves")
                                amounts[5]++;
                            else if (i.ItemType == "Helmets")
                                amounts[6]++;
                            else if (i.ItemType == "Boots") amounts[7]++;
                        }

                    if (tab.ItemListChaos.Count > 0)
                        foreach (var i in tab.ItemListChaos)
                        {
                            Trace.WriteLine(i.ItemType);
                            if (i.ItemType == "Rings")
                                amounts[0]++;
                            else if (i.ItemType == "Amulets")
                                amounts[1]++;
                            else if (i.ItemType == "Belts")
                                amounts[2]++;
                            else if (i.ItemType == "BodyArmours")
                                amounts[3]++;
                            else if (i.ItemType == "TwoHandWeapons")
                                weaponBig++;
                            else if (i.ItemType == "OneHandWeapons")
                                weaponsSmall++;
                            else if (i.ItemType == "Gloves")
                                amounts[5]++;
                            else if (i.ItemType == "Helmets")
                                amounts[6]++;
                            else if (i.ItemType == "Boots") amounts[7]++;
                        }
                }

                //foreach (int i in amounts)
                //{
                //    Trace.WriteLine(i, "amount");
                //}


                if (Settings.Default.ShowItemAmount == 1)
                {
                    Trace.WriteLine("we are here");

                    // calculate amounts needed for full sets
                    //amounts[0] = amounts[0] / 2;
                    foreach (var a in amounts) Trace.WriteLine(a);

                    amounts[4] = weaponsSmall + weaponBig;
                    MainWindow.overlay.RingsAmount = amounts[0];
                    MainWindow.overlay.AmuletsAmount = amounts[1];
                    MainWindow.overlay.BeltsAmount = amounts[2];
                    MainWindow.overlay.ChestsAmount = amounts[3];
                    MainWindow.overlay.WeaponsAmount = amounts[4];
                    MainWindow.overlay.GlovesAmount = amounts[5];
                    MainWindow.overlay.HelmetsAmount = amounts[6];
                    MainWindow.overlay.BootsAmount = amounts[7];
                }
                else if (Settings.Default.ShowItemAmount == 2)
                {
                    amounts[4] = weaponsSmall + weaponBig;
                    MainWindow.overlay.RingsAmount = SetTargetAmount * 2 - amounts[0];
                    MainWindow.overlay.AmuletsAmount = SetTargetAmount - amounts[1];
                    MainWindow.overlay.BeltsAmount = SetTargetAmount - amounts[2];
                    MainWindow.overlay.ChestsAmount = SetTargetAmount - amounts[3];
                    MainWindow.overlay.WeaponsAmount = SetTargetAmount * 2 - (weaponsSmall + weaponBig * 2);
                    MainWindow.overlay.GlovesAmount = SetTargetAmount - amounts[5];
                    MainWindow.overlay.HelmetsAmount = SetTargetAmount - amounts[6];
                    MainWindow.overlay.BootsAmount = SetTargetAmount - amounts[7];
                }
            }
        }

        public static void PlayNotificationSound()
        {
            var volume = Settings.Default.Volume / 100.0;
            Player.Volume = volume;
            Player.Position = TimeSpan.Zero;
            Player.Play();
        }

        public static void PlayNotificationSoundSetPicked()
        {
            var volume = Settings.Default.Volume / 100.0;
            PlayerSet.Volume = volume;
            PlayerSet.Position = TimeSpan.Zero;
            PlayerSet.Play();
        }

        public static StashTab GetStashTabFromItem(Item item)
        {
            foreach (var s in StashTabList.StashTabs)
                if (item.StashTabIndex == s.TabIndex)
                    return s;

            return null;
        }

        // 
        public static void ActivateNextCell(bool active, Cell cell)
        {
            if (active)
            {
                if (Settings.Default.HighlightMode == 0)
                {
                    //activate cell by cell
                    foreach (var s in StashTabList.StashTabs)
                    {
                        s.DeactivateItemCells();
                        s.TabHeaderColor = Brushes.Transparent;
                    }

                    // remove and sound if itemlist empty
                    if (ItemSetListHighlight.Count > 0)
                    {
                        if (ItemSetListHighlight[0].ItemList.Count == 0)
                        {
                            ItemSetListHighlight.RemoveAt(0);
                            PlayerSet.Dispatcher.Invoke(() => { PlayNotificationSoundSetPicked(); });
                        }
                    }
                    else
                    {
                        if (ItemSetListHighlight.Count > 0) PlayerSet.Dispatcher.Invoke(() => { PlayNotificationSoundSetPicked(); });
                    }

                    // next item if itemlist not empty
                    if (ItemSetListHighlight.Count > 0)
                        if (ItemSetListHighlight[0].ItemList.Count > 0 && ItemSetListHighlight[0].EmptyItemSlots.Count == 0)
                        {
                            var highlightItem = ItemSetListHighlight[0].ItemList[0];
                            var currentTab = GetStashTabFromItem(highlightItem);
                            if (currentTab != null)
                            {
                                currentTab.ActivateItemCells(highlightItem);
                                if (Settings.Default.ColorStash != "")
                                    currentTab.TabHeaderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.ColorStash));
                                else
                                    currentTab.TabHeaderColor = Brushes.Red;

                                ItemSetListHighlight[0].ItemList.RemoveAt(0);
                            }
                            //check for full sets
                            //if (ItemSetListHighlight[0].ItemList.Count <= 0)
                            //{
                            //}
                        }
                }
                else if (Settings.Default.HighlightMode == 1)
                {
                    // activate whole set 
                    if (ItemSetListHighlight.Count > 0)
                    {
                        Trace.WriteLine(ItemSetListHighlight[0].ItemList.Count, "item list count");
                        Trace.WriteLine(ItemSetListHighlight.Count, "itemset list ocunt");
                        // check for full sets

                        if (ItemSetListHighlight[0].EmptyItemSlots.Count == 0)
                        {
                            if (cell != null)
                            {
                                var highlightItem = cell.CellItem;
                                var currentTab = GetStashTabFromItem(highlightItem);
                                if (currentTab != null)
                                {
                                    currentTab.DeactivateSingleItemCells(cell.CellItem);
                                    currentTab.TabHeaderColor = Brushes.Transparent;
                                    ItemSetListHighlight[0].ItemList.Remove(highlightItem);
                                }
                            }
                            //Trace.WriteLine("IS HERE IS HERE IS HERE");

                            foreach (var i in ItemSetListHighlight[0].ItemList)
                            {
                                //currentTab.ActivateItemCells(i);
                                var currTab = GetStashTabFromItem(i);
                                currTab.ActivateItemCells(i);
                                //currTab.ShowNumbersOnActiveCells();
                                if (Settings.Default.ColorStash != "")
                                    currTab.TabHeaderColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(Settings.Default.ColorStash));
                                else
                                    currTab.TabHeaderColor = Brushes.Red;
                            }

                            // mark item order
                            if (ItemSetListHighlight[0] != null)
                                if (ItemSetListHighlight[0].ItemList.Count > 0)
                                {
                                    var cTab = GetStashTabFromItem(ItemSetListHighlight[0].ItemList[0]);
                                    cTab.MarkNextItem(ItemSetListHighlight[0].ItemList[0]);
                                }

                            if (ItemSetListHighlight[0].ItemList.Count == 0)
                            {
                                //Trace.WriteLine("itemsetlist gets removes0");
                                ItemSetListHighlight.RemoveAt(0);
                                // activate next set
                                ActivateNextCell(true, null);
                                PlayerSet.Dispatcher.Invoke(() => { PlayNotificationSoundSetPicked(); });
                            }
                        }
                    }
                }
                else if (Settings.Default.HighlightMode == 2)
                {
                    //activate all cells at once
                    if (ItemSetListHighlight.Count > 0)
                        foreach (var set in ItemSetListHighlight)
                            if (set.EmptyItemSlots.Count == 0)
                                if (cell != null)
                                {
                                    var highlightItem = cell.CellItem;
                                    var currentTab = GetStashTabFromItem(highlightItem);
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

        public static void PrepareSelling()
        {
            //ClearAllItemOrderLists();
            ItemSetListHighlight.Clear();
            if (ApiAdapter.IsFetching) return;

            if (ItemSetList == null) return;

            //Trace.WriteLine(StashTabList.StashTabs.Count, "stashtab count");
            foreach (var s in StashTabList.StashTabs) s.PrepareOverlayList();

            foreach (var itemSet in ItemSetList)
                itemSet.OrderItems();
            //GlobalItemOrderList.AddRange(itemSet.ItemList);

            if (Settings.Default.ExaltedRecipe)
            {
                ItemSetShaper.OrderItems();
                ItemSetElder.OrderItems();
                ItemSetWarlord.OrderItems();
                ItemSetCrusader.OrderItems();
                ItemSetHunter.OrderItems();
                ItemSetRedeemer.OrderItems();
                if (ItemSetShaper.EmptyItemSlots.Count == 0)
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetShaper.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetShaper.EmptyItemSlots)
                    });

                if (ItemSetElder.EmptyItemSlots.Count == 0)
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetElder.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetElder.EmptyItemSlots)
                    });

                if (ItemSetCrusader.EmptyItemSlots.Count == 0)
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetCrusader.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetCrusader.EmptyItemSlots)
                    });

                if (ItemSetHunter.EmptyItemSlots.Count == 0)
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetHunter.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetHunter.EmptyItemSlots)
                    });

                if (ItemSetWarlord.EmptyItemSlots.Count == 0)
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetWarlord.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetWarlord.EmptyItemSlots)
                    });

                if (ItemSetRedeemer.EmptyItemSlots.Count == 0)
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(ItemSetRedeemer.ItemList),
                        EmptyItemSlots = new List<string>(ItemSetRedeemer.EmptyItemSlots)
                    });
            }

            //ItemSetListHighlight = new List<ItemSet>(ItemSetList);
            foreach (var set in ItemSetList)
                if (set.SetCanProduceChaos || Settings.Default.RegalRecipe)
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(set.ItemList),
                        EmptyItemSlots = new List<string>(set.EmptyItemSlots)
                    });
        }
    }


    public class ActiveItemTypes
    {
        public bool GlovesActive { get; set; } = true;
        public bool HelmetActive { get; set; } = true;
        public bool BootsActive { get; set; } = true;
        public bool ChestActive { get; set; } = true;
        public bool WeaponActive { get; set; } = true;
        public bool RingActive { get; set; } = true;
        public bool AmuletActive { get; set; } = true;
        public bool BeltActive { get; set; } = true;
        public bool ChaosMissing { get; set; } = true;
    }

    public class ItemTypeAmounts
    {
        public int Gloves { get; set; } = 0;
        public int Helmets { get; set; } = 0;
        public int Boots { get; set; } = 0;
        public int Chests { get; set; } = 0;
        public int Weapons { get; set; } = 0;
        public int Amulets { get; set; } = 0;
        public int Rings { get; set; } = 0;
        public int Belts { get; set; } = 0;
    }
}