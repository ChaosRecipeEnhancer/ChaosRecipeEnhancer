using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChaosRecipeEnhancer.UI.BusinessLogic.DataFetching;
using ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration;
using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.DynamicControls;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Extensions;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.View;

namespace ChaosRecipeEnhancer.UI.Model
{
    public static class Data
    {
        private static int SetTargetAmount { get; set; }
        private static List<ItemSet> ItemSetList { get; set; }
        private static ActiveItemTypes PreviousActiveItems { get; set; }
        private static ItemSet ItemSetShaper { get; set; }
        private static ItemSet ItemSetElder { get; set; }
        private static ItemSet ItemSetWarlord { get; set; }
        private static ItemSet ItemSetCrusader { get; set; }
        private static ItemSet ItemSetRedeemer { get; set; }
        private static ItemSet ItemSetHunter { get; set; }

        public static ActiveItemTypes ActiveItems { get; private set; } = new ActiveItemTypes();
        public static MediaPlayer Player { get; } = new MediaPlayer();
        public static MediaPlayer PlayerSet { get; } = new MediaPlayer();
        public static List<ItemSet> ItemSetListHighlight { get; } = new List<ItemSet>();
        public static CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();
        public static CancellationToken CancellationToken { get; set; } = CancellationTokenSource.Token;

        #region Settings Utility

        private static void GetSetTargetAmount(StashTabControl stash)
        {
            if (Settings.Default.FullSetThreshold > 0)
            {
                SetTargetAmount = Settings.Default.FullSetThreshold;
            }
            else
            {
                if (stash.Quad)
                    SetTargetAmount += 16;
                else
                    SetTargetAmount += 4;
            }
        }

        #endregion

        #region Item Set Composition Logic

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

            Trace.WriteLine(ItemSetList.Count, "[Data:GenerateItemSetList()]: item set list count");

            if (Settings.Default.ExaltedShardRecipeTrackingEnabled) GenerateInfluencedItemSets();
        }

        // tries to add item, if item added returns true
        private static bool AddItemToItemSet(ItemSet set, bool chaosItems = false, bool honorOrder = true)
        {
            var listName = chaosItems ? "ItemListChaos" : "ItemList";

            Item minItem = null;
            var minDistance = double.PositiveInfinity;

            // TODO: crashes here after some time
            foreach (var s in ReconstructedStashTabs.StashTabControls)
            foreach (var i in (List<Item>)ObjectExtensions.GetPropertyValue(s, listName))
                if (set.GetNextItemClass() == i.ItemType || (!honorOrder && set.IsValidItem(i)))
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
                ((List<Item>)ObjectExtensions.GetPropertyValue(tab, listName)).Remove(minItem);
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
                    foreach (var s in ReconstructedStashTabs.StashTabControls)
                    foreach (var i in (List<Item>)ObjectExtensions.GetPropertyValue(s, listName))
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
                        ((List<Item>)ObjectExtensions.GetPropertyValue(tab, listName)).Remove(minItem);
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
                    if (i.SetCanProduceChaos == false && !Settings.Default.RegalRecipeTrackingEnabled)
                        if (AddItemToItemSet(i, true))
                            continue;

                    if (!AddItemToItemSet(i))
                        if (Settings.Default.DoNotPreserveLowItemLevelGear &&
                            !Settings.Default.RegalRecipeTrackingEnabled)
                            AddItemToItemSet(i, true);
                }

                /* At this point in time the following conditions may be met, exclusively
                 * 1.) We obtained a full set and it contains one chaos item
                 * 1.1) We obtained a full set and it contains multiple chaos items (only if filling with chaos items is allowed)
                 * 2.) We obtained a full set without a chaos item -> We aren't lacking a regal item in this set but we don't have enough chaos items.
                 * 3.) We couldn't obtain a full set. That means that at least one item slot is missing. We need to check which of the remaining slots we can still fill. We could still be missing a chaos item.
                 */
                if (i.EmptyItemSlots.Count == 0 &&
                    (i.SetCanProduceChaos || Settings.Default.RegalRecipeTrackingEnabled))
                    // Set full, continue
                    continue;

                if (i.EmptyItemSlots.Count > 0)
                {
                    lastEmptySlots = 0;
                    while (i.EmptyItemSlots.Count > 0 && i.EmptyItemSlots.Count != lastEmptySlots)
                    {
                        lastEmptySlots = i.EmptyItemSlots.Count;
                        if (!i.SetCanProduceChaos && !Settings.Default.RegalRecipeTrackingEnabled)
                            if (AddItemToItemSet(i, true, false))
                                continue;

                        if (!AddItemToItemSet(i, false, false))
                            // couldn't add a regal item. Try chaos item if filling with chaos is allowed
                            if (Settings.Default.DoNotPreserveLowItemLevelGear &&
                                !Settings.Default.RegalRecipeTrackingEnabled)
                                AddItemToItemSet(i, true, false);
                    }
                    // At this point the set will contain a chaos item as long as we had at least one left. If not we didn't have any chaos items left.
                    // If the set is not full at this time we're missing at least one regal item. If it has not chaos item we're also missing chaos items.
                    // Technically it could be only the chaos item that's missing but that can be neglected since when mixing you'll always be short on chaos items.
                    // If not in "endgame" mode (always show chaos) have the loot filter apply to chaos and regal items the same way.
                }
            }

            if (Settings.Default.ExaltedShardRecipeTrackingEnabled) FillItemSetsInfluenced();
        }

        private static void FillItemSetsInfluenced()
        {
            foreach (var tab in ReconstructedStashTabs.StashTabControls)
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

        #endregion

        #region Set Tracker Overlay Logic

        public static async Task CheckActives(SetTrackerOverlayView setTrackerOverlay)
        {
            try
            {
                if (ApiAdapter.FetchError)
                {
                    setTrackerOverlay.WarningMessage = "Fetching Error...";
                    setTrackerOverlay.ShadowOpacity = 1;
                    setTrackerOverlay.WarningMessageVisibility = Visibility.Visible;
                    return;
                }

                if (ReconstructedStashTabs.StashTabControls.Count == 0)
                {
                    setTrackerOverlay.WarningMessage = "No Stashtabs found...";
                    setTrackerOverlay.ShadowOpacity = 1;
                    setTrackerOverlay.WarningMessageVisibility = Visibility.Visible;
                    return;
                }

                if (Settings.Default.SoundEnabled) PreviousActiveItems = new ActiveItemTypes(ActiveItems);

                // calculate target amount if user has 0 set in it
                // (e.g. 2 quad tabs queried w 0 set threshold = 24 set threshold)
                // else just stick to the default amount (their defined in settings)
                SetTargetAmount = 0;
                if (ReconstructedStashTabs.StashTabControls.Count > 0)
                    foreach (var s in ReconstructedStashTabs.StashTabControls)
                        GetSetTargetAmount(s);

                if (Settings.Default.SetTrackerOverlayItemCounterDisplayMode != 0)
                {
                    Trace.WriteLine("Calculating Items");
                    CalculateItemAmounts(setTrackerOverlay);
                }

                // generate {SetThreshold} empty sets to be filled
                GenerateItemSetList();

                // proceed to fill those newly created empty sets
                FillItemSets();

                // check for full sets/ missing items
                var missingGearPieceForChaosRecipe = false;
                var fullSets = 0;

                // unique missing item classes
                var missingItemClasses = new HashSet<string>();

                // for every set in our itemsetlist check if their EmptyItemSlots is 0 if not add to our full set count
                foreach (var set in ItemSetList)
                    if (set.EmptyItemSlots.Count == 0)
                    {
                        // fix for: condition (fullSets == SetTargetAmount && missingChaos)
                        // never true cause fullsets < settargetamount when missingChaos @ikogan
                        fullSets++;

                        if (!set.SetCanProduceChaos && !Settings.Default.RegalRecipeTrackingEnabled)
                            missingGearPieceForChaosRecipe = true;
                    }
                    else
                    {
                        // all classes which are active over all ilvls
                        foreach (var itemClass in set.EmptyItemSlots) missingItemClasses.Add(itemClass);
                    }

                var filterManager = new CFilterGenerationManager();

                // i need to pass in the missingGearPieceForChaosRecipe
                ActiveItems =
                    await filterManager.GenerateSectionsAndUpdateFilterAsync(missingItemClasses,
                        missingGearPieceForChaosRecipe);

                //Trace.WriteLine(fullSets, "full sets");
                setTrackerOverlay.Dispatcher.Invoke(() => { setTrackerOverlay.FullSetsText = fullSets.ToString(); });

                // invoke chaos missing
                if (missingGearPieceForChaosRecipe && !Settings.Default.RegalRecipeTrackingEnabled)
                {
                    setTrackerOverlay.WarningMessage = "Need lower level items!";
                    setTrackerOverlay.ShadowOpacity = 1;
                    setTrackerOverlay.WarningMessageVisibility = Visibility.Visible;
                }

                // invoke exalted recipe ready
                if (Settings.Default.ExaltedShardRecipeTrackingEnabled)
                    if (ItemSetShaper.EmptyItemSlots.Count == 0
                        || ItemSetElder.EmptyItemSlots.Count == 0
                        || ItemSetCrusader.EmptyItemSlots.Count == 0
                        || ItemSetWarlord.EmptyItemSlots.Count == 0
                        || ItemSetHunter.EmptyItemSlots.Count == 0
                        || ItemSetRedeemer.EmptyItemSlots.Count == 0)
                    {
                        setTrackerOverlay.WarningMessage = "Exalted Recipe ready!";
                        setTrackerOverlay.ShadowOpacity = 1;
                        setTrackerOverlay.WarningMessageVisibility = Visibility.Visible;
                    }

                // invoke set full
                if (fullSets == SetTargetAmount && !missingGearPieceForChaosRecipe)
                {
                    setTrackerOverlay.WarningMessage = "Sets full!";
                    setTrackerOverlay.ShadowOpacity = 1;
                    setTrackerOverlay.WarningMessageVisibility = Visibility.Visible;
                }

                Trace.WriteLine(fullSets, "full sets");

                // If the state of any gear slot changed, we play a sound
                if (Settings.Default.SoundEnabled)
                    if (!(PreviousActiveItems.GlovesActive == ActiveItems.GlovesActive
                          && PreviousActiveItems.BootsActive == ActiveItems.BootsActive
                          && PreviousActiveItems.HelmetActive == ActiveItems.HelmetActive
                          && PreviousActiveItems.ChestActive == ActiveItems.ChestActive
                          && PreviousActiveItems.WeaponActive == ActiveItems.WeaponActive
                          && PreviousActiveItems.RingActive == ActiveItems.RingActive
                          && PreviousActiveItems.AmuletActive == ActiveItems.AmuletActive
                          && PreviousActiveItems.BeltActive == ActiveItems.BeltActive))
                        Player.Dispatcher.Invoke(() =>
                        {
                            Trace.WriteLine("Gear Slot State Changed; Playing sound!");
                            PlayNotificationSound();
                        });
            }
            catch (OperationCanceledException ex) when (ex.CancellationToken == CancellationToken)
            {
                Trace.WriteLine("abort");
            }
        }

        public static void CalculateItemAmounts(SetTrackerOverlayView setTrackerOverlay)
        {
            if (ReconstructedStashTabs.StashTabControls != null)
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
                foreach (var tab in ReconstructedStashTabs.StashTabControls)
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

                if (Settings.Default.SetTrackerOverlayItemCounterDisplayMode == 1)
                {
                    Trace.WriteLine("we are here");

                    // calculate amounts needed for full sets
                    //amounts[0] = amounts[0] / 2;
                    foreach (var a in amounts) Trace.WriteLine(a);

                    amounts[4] = weaponsSmall + weaponBig;
                    setTrackerOverlay.RingsAmount = amounts[0];
                    setTrackerOverlay.AmuletsAmount = amounts[1];
                    setTrackerOverlay.BeltsAmount = amounts[2];
                    setTrackerOverlay.ChestsAmount = amounts[3];
                    setTrackerOverlay.WeaponsAmount = amounts[4];
                    setTrackerOverlay.GlovesAmount = amounts[5];
                    setTrackerOverlay.HelmetsAmount = amounts[6];
                    setTrackerOverlay.BootsAmount = amounts[7];
                }
                else if (Settings.Default.SetTrackerOverlayItemCounterDisplayMode == 2)
                {
                    amounts[4] = weaponsSmall + weaponBig;
                    setTrackerOverlay.RingsAmount = SetTargetAmount * 2 - amounts[0];
                    setTrackerOverlay.AmuletsAmount = SetTargetAmount - amounts[1];
                    setTrackerOverlay.BeltsAmount = SetTargetAmount - amounts[2];
                    setTrackerOverlay.ChestsAmount = SetTargetAmount - amounts[3];
                    setTrackerOverlay.WeaponsAmount = SetTargetAmount * 2 - (weaponsSmall + weaponBig * 2);
                    setTrackerOverlay.GlovesAmount = SetTargetAmount - amounts[5];
                    setTrackerOverlay.HelmetsAmount = SetTargetAmount - amounts[6];
                    setTrackerOverlay.BootsAmount = SetTargetAmount - amounts[7];
                }
            }
        }

        #endregion

        #region Stash Tab Overlay Logic

        public static StashTabControl GetStashTabFromItem(Item item)
        {
            foreach (var s in ReconstructedStashTabs.StashTabControls)
                if (item.StashTabIndex == s.TabIndex)
                    return s;

            return null;
        }

        public static void ActivateNextCell(bool active, InteractiveStashCell stashCell, TabControl tabControl)
        {
            if (!active) return;

            var currentlySelectedStashOverlayTabName = tabControl != null
                ? ((TextBlock)((HeaderedContentControl)tabControl.SelectedItem).Header).Text
                : "";

            // activate cell by cell / item by item
            if (Settings.Default.StashTabOverlayHighlightMode == 0)
            {
                foreach (var s in ReconstructedStashTabs.StashTabControls.ToList())
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
                    if (ItemSetListHighlight.Count > 0)
                        PlayerSet.Dispatcher.Invoke(() => { PlayNotificationSoundSetPicked(); });
                }

                // next item if itemlist not empty
                if (ItemSetListHighlight.Count > 0)
                {
                    if (ItemSetListHighlight[0].ItemList.Count > 0 &&
                        ItemSetListHighlight[0].EmptyItemSlots.Count == 0)
                    {
                        var highlightItem = ItemSetListHighlight[0].ItemList[0];
                        var currentTab = GetStashTabFromItem(highlightItem);

                        if (currentTab == null) return;

                        currentTab.ActivateItemCells(highlightItem);

                        if (currentTab.TabName != currentlySelectedStashOverlayTabName &&
                            !string.IsNullOrEmpty(Settings.Default.StashTabOverlayHighlightColor))
                            currentTab.TabHeaderColor = new SolidColorBrush(
                                (Color)ColorConverter.ConvertFromString(Settings.Default
                                    .StashTabOverlayHighlightColor));
                        else
                            currentTab.TabHeaderColor = Brushes.Transparent;

                        ItemSetListHighlight[0].ItemList.RemoveAt(0);
                    }
                }
            }
            // activate set by set
            else if (Settings.Default.StashTabOverlayHighlightMode == 1)
            {
                if (ItemSetListHighlight.Count > 0)
                {
                    Trace.WriteLine(ItemSetListHighlight[0].ItemList.Count,
                        "[Data: ActivateNextCell()]: item list count");
                    Trace.WriteLine(ItemSetListHighlight.Count, "[Data: ActivateNextCell()]: item set list count");

                    // check for full sets
                    if (ItemSetListHighlight[0].EmptyItemSlots.Count == 0)
                    {
                        if (stashCell != null)
                        {
                            var highlightItem = stashCell.PathOfExileItemData;
                            var currentTab = GetStashTabFromItem(highlightItem);

                            if (currentTab != null)
                            {
                                currentTab.TabHeaderColor = Brushes.Transparent;
                                currentTab.DeactivateSingleItemCells(stashCell.PathOfExileItemData);
                                ItemSetListHighlight[0].ItemList.Remove(highlightItem);
                            }
                        }

                        foreach (var i in ItemSetListHighlight[0].ItemList.ToList())
                        {
                            var currTab = GetStashTabFromItem(i);
                            currTab.ActivateItemCells(i);
                        }

                        // mark item order
                        if (ItemSetListHighlight[0] != null)
                        {
                            if (ItemSetListHighlight[0].ItemList.Count > 0)
                            {
                                var currentStashTab = GetStashTabFromItem(ItemSetListHighlight[0].ItemList[0]);
                                currentStashTab.MarkNextItem(ItemSetListHighlight[0].ItemList[0]);
                                currentStashTab.TabHeaderColor = new SolidColorBrush(
                                    (Color)ColorConverter.ConvertFromString(Settings.Default
                                        .StashTabOverlayHighlightColor));

                                // if (tabControl != null)
                                // {
                                //     Trace.WriteLine($"[Data: ActivateNextCell()]: TabControl Current Tab Item {tabControl.SelectedItem}");
                                //     Trace.WriteLine($"[Data: ActivateNextCell()]: TabControl Current Tab Item Header Text {((TextBlock)((HeaderedContentControl)tabControl.SelectedItem).Header).Text}");
                                // }
                            }
                        }

                        // Set has been completed
                        if (ItemSetListHighlight[0].ItemList.Count == 0)
                        {
                            ItemSetListHighlight.RemoveAt(0);

                            // activate next set
                            ActivateNextCell(true, null, null);
                            PlayerSet.Dispatcher.Invoke(() => { PlayNotificationSoundSetPicked(); });
                        }
                    }
                }
            }
            // activate whole set
            else if (Settings.Default.StashTabOverlayHighlightMode == 2)
            {
                //activate all cells at once
                if (ItemSetListHighlight.Count <= 0) return;

                // Why do I switch all of the foreach loops to reference {Some List}.ToList()?
                // REF: https://stackoverflow.com/a/604843
                foreach (var set in ItemSetListHighlight.ToList())
                {
                    if (set.EmptyItemSlots.Count != 0) continue;

                    if (stashCell == null) continue;

                    var highlightItem = stashCell.PathOfExileItemData;
                    var currentTab = GetStashTabFromItem(highlightItem);

                    if (currentTab == null) continue;

                    currentTab.DeactivateSingleItemCells(stashCell.PathOfExileItemData);
                    ItemSetListHighlight[0].ItemList.Remove(highlightItem);

                    var itemsRemainingInStashTab = false;

                    foreach (var item in ItemSetListHighlight.ToList()[0].ItemList.ToList())
                    {
                        if (item.StashTabIndex == currentTab.TabIndex) itemsRemainingInStashTab = true;
                    }

                    if (!itemsRemainingInStashTab) currentTab.TabHeaderColor = Brushes.Transparent;

                    // Set has been completed
                    if (ItemSetListHighlight[0].ItemList.Count != 0) continue;

                    ItemSetListHighlight.RemoveAt(0);

                    // activate next set, if one exists
                    if (ItemSetListHighlight.Count != 0) ActivateNextCell(true, null, null);
                    else PlayerSet.Dispatcher.Invoke(() => { PlayNotificationSoundSetPicked(); });
                }
            }
        }

        public static void PrepareSelling()
        {
            //ClearAllItemOrderLists();
            ItemSetListHighlight.Clear();
            if (ApiAdapter.IsFetching) return;

            if (ItemSetList == null) return;

            foreach (var s in ReconstructedStashTabs.StashTabControls)
                s.PrepareOverlayList();

            foreach (var itemSet in ItemSetList)
                itemSet.OrderItems();

            if (Settings.Default.ExaltedShardRecipeTrackingEnabled)
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

            foreach (var set in ItemSetList)
            {
                if (set.SetCanProduceChaos || Settings.Default.RegalRecipeTrackingEnabled)
                    ItemSetListHighlight.Add(new ItemSet
                    {
                        ItemList = new List<Item>(set.ItemList),
                        EmptyItemSlots = new List<string>(set.EmptyItemSlots)
                    });
            }
        }

        #endregion

        #region Sound Utility

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

        #endregion
    }
}