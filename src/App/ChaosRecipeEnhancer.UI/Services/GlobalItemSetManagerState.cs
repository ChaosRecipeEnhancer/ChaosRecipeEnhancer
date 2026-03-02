using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using System.Collections.Generic;
using System.Linq;

namespace ChaosRecipeEnhancer.UI.Services;

public static class GlobalItemSetManagerState
{
    #region Properties

    #region Item Amount Properties

    // item amounts by kind that will be exposed
    // while GlobalItemSetManagerState doesn't know,
    // others need to render this out to users
    public static int RingsAmount { get; private set; }
    public static int AmuletsAmount { get; private set; }
    public static int BeltsAmount { get; private set; }
    public static int ChestsAmount { get; private set; }
    public static int WeaponsSmallAmount { get; private set; }
    public static int WeaponsBigAmount { get; private set; }
    public static int GlovesAmount { get; private set; }
    public static int HelmetsAmount { get; private set; }
    public static int BootsAmount { get; private set; }

    #endregion

    public static int SetThreshold { get; set; }
    public static List<EnhancedItemSet> SetsInProgress { get; set; } = new();
    public static List<EnhancedItem> CurrentItemsFilteredForRecipe { get; set; } = new();
    public static List<UnifiedStashTabMetadata> StashTabMetadataListStashesResponse { get; set; }

    // full set amounts will also need to be rendered
    public static int CompletedSetCount { get; private set; }
    public static bool NeedsFetching { get; private set; } = true;
    public static bool NeedsLowerLevel { get; set; }

    #endregion

    #region Methods

    public static void UpdateStashMetadata(List<UnifiedStashTabMetadata> metadata)
    {
        StashTabMetadataListStashesResponse = metadata;
    }

    public static void UpdateStashContents(int setThreshold, List<string> selectedTabIds, List<EnhancedItem> filteredStashContents)
    {
        // if no tabs are selected (this isn't a realistic case)
        if (selectedTabIds.Count == 0) return;

        // setting some new properties
        SetThreshold = setThreshold;
        CurrentItemsFilteredForRecipe = filteredStashContents;
        NeedsFetching = false;
    }

    public static void CalculateItemAmounts()
    {
        foreach (var item in CurrentItemsFilteredForRecipe)
        {
            switch (item.DerivedItemClass)
            {
                case GameTerminology.Rings:
                    RingsAmount++; // 0: rings
                    break;
                case GameTerminology.Amulets:
                    AmuletsAmount++; // 1: amulets
                    break;
                case GameTerminology.Belts:
                    BeltsAmount++; // 2: belts
                    break;
                case GameTerminology.BodyArmors:
                    ChestsAmount++; // 3: chests
                    break;
                case GameTerminology.OneHandWeapons:
                    WeaponsSmallAmount++; // 4: weaponsSmall
                    break;
                case GameTerminology.TwoHandWeapons:
                    WeaponsBigAmount++; // 5: weaponsBig
                    break;
                case GameTerminology.Gloves:
                    GlovesAmount++; // 6: gloves
                    break;
                case GameTerminology.Helmets:
                    HelmetsAmount++; // 7: helmets
                    break;
                case GameTerminology.Boots:
                    BootsAmount++; // 8: boots
                    break;
            }
        }

        NeedsFetching = false;
    }

    public static void ResetCompletedSetCountAndItemAmounts()
    {
        // reset set count
        CompletedSetCount = 0;

        // reset all item amounts
        RingsAmount = 0;
        AmuletsAmount = 0;
        BeltsAmount = 0;
        ChestsAmount = 0;
        WeaponsSmallAmount = 0;
        WeaponsBigAmount = 0;
        GlovesAmount = 0;
        HelmetsAmount = 0;
        BootsAmount = 0;
    }

    #region Generate Item Sets

    public static void GenerateItemSets(RecipeType recipeType = RecipeType.ChaosOrb)
    {
        List<EnhancedItem> eligibleRecipeItems;
        switch (recipeType)
        {
            case RecipeType.RegalOrb:
                eligibleRecipeItems = CurrentItemsFilteredForRecipe
                    .Where(x => x.IsRegalRecipeEligible)
                    .ToList();
                break;
            case RecipeType.OrbOfChance:
                eligibleRecipeItems = CurrentItemsFilteredForRecipe
                    .Where(x => x.IsOrbOfChanceRecipeEligible)
                    .ToList();
                break;
            default:
                eligibleRecipeItems = CurrentItemsFilteredForRecipe
                    .Where(x => x.IsChaosRecipeEligible)
                    .ToList();
                break;
        }

        // sorting both of our item lists by item class
        // it's important to prioritize two-handed weapons at the beginning so our set composition is more efficient
        eligibleRecipeItems = eligibleRecipeItems.OrderByDescending(item => item.DerivedItemClass == GameTerminology.TwoHandWeapons)
            .ThenBy(item => item.DerivedItemClass)
            .ToList();

        CurrentItemsFilteredForRecipe = CurrentItemsFilteredForRecipe.OrderByDescending(item => item.DerivedItemClass == GameTerminology.TwoHandWeapons)
            .ThenBy(item => item.DerivedItemClass)
            .ToList();

        int trueSetThreshold;

        // if Early Set Turn In is enabled, we need to modify the upper bound to match how many sets we can possibly make
        // if we have less chaos items than the set threshold, we can only make as many sets as we have chaos items
        // otherwise we can make as many sets as the set threshold
        if (Settings.Default.VendorSetsEarly)
        {
            if (eligibleRecipeItems.Count > SetThreshold)
            {
                trueSetThreshold = SetThreshold;
            }
            else
            {
                trueSetThreshold = eligibleRecipeItems.Count;
            }
        }
        else
        {
            trueSetThreshold = SetThreshold;
        }

        if (recipeType == RecipeType.RegalOrb)
        {
            NeedsLowerLevel = false;
        }
        else if (eligibleRecipeItems.Count < trueSetThreshold || eligibleRecipeItems.Count == 0)
        {
            NeedsLowerLevel = true;
        }
        else
        {
            NeedsLowerLevel = false;
        }

        // This will only apply for:
        //  - Chaos Recipe with Vendor Sets Early ENABLED
        //  - Chaos Recipe with Vendor Sets Early DISABLED
        //  - Regal Recipe
        if (Settings.Default.DoNotPreserveLowItemLevelGear || recipeType != RecipeType.ChaosOrb)
        {
            GenerateItemSets_Greedy(eligibleRecipeItems, trueSetThreshold, recipeType);
        }
        // This will only apply for:
        //  - Chaos Recipe with Do Not Preserve Low Item Level Gear DISABLED
        else
        {
            // pre-filter the list of eligible recipe items
            CurrentItemsFilteredForRecipe = CurrentItemsFilteredForRecipe
                .Where(x => !x.IsChaosRecipeEligible)
                .ToList();

            GenerateItemSets_Conserve(eligibleRecipeItems, SetThreshold);
        }
    }

    private static void GenerateItemSets_Conserve(List<EnhancedItem> eligibleRecipeItems, int trueSetThreshold)
    {
        SetsInProgress.Clear();
        var listOfSets = new List<EnhancedItemSet>();

        // for every set we will start by trying to add a recipe item
        // (and reporting if we need more low level items if we're doing the chaos recipe)

        // this loop is NOT responsible for filling sets their entirety
        // however, if Early Set Turn In is enabled, we need to modify the upper bound to match how many sets we can possibly make
        // rather than trying to 'fill' the sets, we're just trying to make as many sets as possible
        for (var i = 0; i < trueSetThreshold; i++)
        {
            // create new 'empty' enhanced item set
            var enhancedItemSet = new EnhancedItemSet();

            // if there are still recipe items left in our stash
            if (eligibleRecipeItems.Count != 0)
            {
                // try to add a single eligible recipe item in the set (where we're iterate in our loop on line 166)
                foreach (var item in eligibleRecipeItems)
                {
                    var addSuccessful = enhancedItemSet.TryAddItem(item, RecipeType.ChaosOrb);

                    // if we successfully add to set (i.e. it wasn't an item slot that was already taken)
                    if (addSuccessful)
                    {
                        // remove from the list of eligible recipe items
                        eligibleRecipeItems.Remove(item);

                        // break out of loop
                        break;
                    }
                }
            }

            listOfSets.Add(enhancedItemSet);
        }

        // for every set we still need to loop to create the rest of the sets (even if they don't have chaos items in them)
        // when composing sets we will always try to add the closest item to the set to enhance user experience during item picking
        // we once again see the modified upper bound based on Early Set Turn In
        for (var i = 0; i < trueSetThreshold; i++)
        {
            // iterate until the end of time (jk)
            while (true)
            {
                // the closest item is assumed to be in another galaxy
                EnhancedItem closestMissingItem = null;
                var minDistance = double.PositiveInfinity;

                // find the for real closes item
                // this is a nested for over each other item in our current item filtered for recipe
                // probably could be optimized? maybe?
                foreach (var item in CurrentItemsFilteredForRecipe
                             .Where(item => listOfSets[i].IsItemClassNeeded(item) && // item is of a class we need
                                            listOfSets[i].GetItemDistance(item) < minDistance)) // item is closer than the current closest
                {
                    minDistance = listOfSets[i].GetItemDistance(item);
                    closestMissingItem = item;
                }

                if (closestMissingItem is not null)
                {
                    // if we found a new closer we're good to add it to our enhanced set
                    if (listOfSets[i].TryAddItem(closestMissingItem, RecipeType.ChaosOrb))
                    {
                        CurrentItemsFilteredForRecipe.Remove(closestMissingItem);
                    }
                    else
                    {
                        // Item was found by class but rejected by TryAddItem.
                        // No progress can be made — break to avoid infinite loop.
                        break;
                    }
                }
                // you didn't find a closer item, gg break out of infinite loop
                else
                {
                    break;
                }
            }
        }

        SetsInProgress = listOfSets;
        CompletedSetCount = listOfSets.Count(set => set.EmptyItemSlots.Count == 0 && set.IsChaosRecipeEligible);
    }

    private static void GenerateItemSets_Greedy(List<EnhancedItem> eligibleRecipeItems, int trueSetThreshold, RecipeType recipeType = RecipeType.ChaosOrb)
    {
        // Clear any existing progress in item set generation
        SetsInProgress.Clear();
        var listOfSets = new List<EnhancedItemSet>();
        bool containsRequiredRecipeItems = false;

        // Iteratively create item sets based on the number of available recipe items
        for (var i = 0; i < trueSetThreshold; i++)
        {
            // Initialize a new item set
            var enhancedItemSet = new EnhancedItemSet();

            // Add a recipe item to the set if any are available
            if (eligibleRecipeItems.Count > 0)
            {
                var recipeItem = eligibleRecipeItems.First();

                if (enhancedItemSet.TryAddItem(recipeItem, recipeType))
                {
                    // Remove the added recipe item from the available pools
                    eligibleRecipeItems.Remove(recipeItem);
                    CurrentItemsFilteredForRecipe.Remove(recipeItem);

                    // let's attempt to fill in gaps with one-handed weapons if possible
                    if (recipeItem.DerivedItemClass == GameTerminology.OneHandWeapons)
                    {
                        var oneHandedWeapon = CurrentItemsFilteredForRecipe
                            .FirstOrDefault(x => x.DerivedItemClass == GameTerminology.OneHandWeapons);

                        if (oneHandedWeapon is not null)
                        {
                            enhancedItemSet.TryAddItem(oneHandedWeapon, recipeType);
                            eligibleRecipeItems.Remove(oneHandedWeapon);
                            CurrentItemsFilteredForRecipe.Remove(oneHandedWeapon);
                        }
                    }
                }
            }

            // Continuously try to fill the rest of the set with available items
            while (true)
            {
                // Initialize variables to find the closest missing item for the set
                EnhancedItem closestMissingItem = null;
                var minDistance = double.PositiveInfinity;

                // Iterate over all available items to find the closest needed item
                foreach (var item in CurrentItemsFilteredForRecipe
                             .Where(item => enhancedItemSet.IsItemClassNeeded(item) && // Check if the item class is needed for the set
                                            enhancedItemSet.GetItemDistance(item) < minDistance)) // Check if the item is closer than the current closest
                {
                    // Update closest item and distance if a closer item is found
                    minDistance = enhancedItemSet.GetItemDistance(item);
                    closestMissingItem = item;
                }

                // If a closest missing item is found, add it to the set
                if (closestMissingItem != null)
                {
                    if (enhancedItemSet.TryAddItem(closestMissingItem, recipeType))
                    {
                        // Remove the item from the pool of available items
                        eligibleRecipeItems.Remove(closestMissingItem);
                        CurrentItemsFilteredForRecipe.Remove(closestMissingItem);

                        // let's attempt to fill in gaps with one-handed weapons if possible
                        if (closestMissingItem.DerivedItemClass == GameTerminology.OneHandWeapons)
                        {
                            var oneHandedWeapon = CurrentItemsFilteredForRecipe
                                .FirstOrDefault(x => x.DerivedItemClass == GameTerminology.OneHandWeapons);

                            if (oneHandedWeapon is not null)
                            {
                                enhancedItemSet.TryAddItem(oneHandedWeapon, recipeType);
                                CurrentItemsFilteredForRecipe.Remove(oneHandedWeapon);
                            }
                        }
                    }
                    else
                    {
                        // Item was found by class but rejected (e.g. wrong ilvl for recipe).
                        // No progress can be made — break to avoid infinite loop.
                        break;
                    }
                }
                else
                {
                    // Break the loop if no suitable item is found
                    break;
                }

                // Check if the current set is complete (i.e., no empty item slots)
                if (enhancedItemSet.EmptyItemSlots.Count == 0)
                {
                    break;
                }
            }

            // Add the newly created set to the list of sets
            listOfSets.Add(enhancedItemSet);
        }

        if (recipeType == RecipeType.ChaosOrb || recipeType == RecipeType.OrbOfChance)
        {
            for (var i = 0; i < trueSetThreshold; i++)
            {
                var canProduce = recipeType == RecipeType.OrbOfChance
                    ? listOfSets[i].Items.FirstOrDefault(x => x.IsOrbOfChanceRecipeEligible, null)
                    : listOfSets[i].Items.FirstOrDefault(x => x.IsChaosRecipeEligible, null);

                if (canProduce is not null)
                {
                    containsRequiredRecipeItems = true;
                }
            }
        }

        // Update the sets in progress with the newly created list of sets
        SetsInProgress = listOfSets;

        // Update the count of completed sets based on the number of sets with no empty item slots

        if (recipeType != RecipeType.RegalOrb && !containsRequiredRecipeItems)
        {
            NeedsLowerLevel = true;
            CompletedSetCount = 0;
        }
        else
        {
            CompletedSetCount = recipeType switch
            {
                RecipeType.RegalOrb => listOfSets.Count(set => set.EmptyItemSlots.Count == 0 && set.IsRegalRecipeEligible),
                RecipeType.OrbOfChance => listOfSets.Count(set => set.EmptyItemSlots.Count == 0 && set.IsOrbOfChanceRecipeEligible),
                _ => listOfSets.Count(set => set.EmptyItemSlots.Count == 0 && set.IsChaosRecipeEligible)
            };
        }
    }

    #endregion

    public static List<Dictionary<ItemClass, int>> RetrieveCurrentItemCountsForFilterManipulation()
    {
        var result = new List<Dictionary<ItemClass, int>>
        {
            new()
            {
                {ItemClass.Rings, RingsAmount},
                {ItemClass.Amulets, AmuletsAmount},
                {ItemClass.Belts, BeltsAmount},
                {ItemClass.BodyArmours, ChestsAmount},
                {ItemClass.OneHandWeapons, WeaponsSmallAmount},
                {ItemClass.TwoHandWeapons, WeaponsBigAmount},
                {ItemClass.Gloves, GlovesAmount},
                {ItemClass.Helmets, HelmetsAmount},
                {ItemClass.Boots, BootsAmount}
            }
        };

        return result;
    }

    public static List<UnifiedStashTabMetadata> FlattenStashTabs(List<UnifiedStashTabMetadata> tabs)
    {
        var allTabs = new List<UnifiedStashTabMetadata>();

        foreach (var tab in tabs)
        {
            if (tab.Type != "Folder")
            {
                allTabs.Add(tab); // If not folder, add tab
            }
            else if (tab.Children != null)
            {
                allTabs.AddRange(tab.Children); // Add the children if any
            }
        }

        return allTabs;
    }

    #endregion
}
