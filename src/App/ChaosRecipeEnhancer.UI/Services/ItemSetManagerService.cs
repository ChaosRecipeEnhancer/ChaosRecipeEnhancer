using System.Collections.Generic;
using System.Linq;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Enums;

namespace ChaosRecipeEnhancer.UI.Services;

public interface IItemSetManagerService
{
    public bool UpdateData(
        int setThreshold,
        List<int> selectedTabIndices,
        List<EnhancedItem> filteredStashContents,
        bool includeIdentified = false,
        bool chaosRecipe = true
    );

    public bool RetrieveNeedsFetching();
    public int RetrieveCompletedSets();

    public int RetrieveRingsAmount();
    public int RetrieveAmuletsAmount();
    public int RetrieveBeltsAmount();
    public int RetrieveChestsAmount();
    public int RetrieveWeaponsSmallAmount();
    public int RetrieveWeaponsBigAmount();
    public int RetrieveGlovesAmount();
    public int RetrieveHelmetsAmount();
    public int RetrieveBootsAmount();
}

public class ItemSetManagerService : IItemSetManagerService
{
    private int _setThreshold;
    private List<int> _currentSelectedTabs;
    private List<EnhancedItemSet> _setsInProgress = new();
    private List<EnhancedItem> _currentItemsFilteredForRecipe = new(); // filtered for chaos recipe

    // item amounts by kind that will be exposed
    // while ItemSetManagerService doesn't know,
    // others need to render this out to users
    public int RingsAmount { get; set; }
    public int AmuletsAmount { get; set; }
    public int BeltsAmount { get; set; }
    public int ChestsAmount { get; set; }
    public int WeaponsSmallAmount { get; set; }
    public int WeaponsBigAmount { get; set; }
    public int GlovesAmount { get; set; }
    public int HelmetsAmount { get; set; }
    public int BootsAmount { get; set; }

    // full set amounts will also need to be rendered
    public int CompletedSets { get; set; }

    public bool NeedsFetching { get; set; }

    public ItemSetManagerService()
    {
        NeedsFetching = true;
    }

    // this is the primary method being called by external entities
    // return true if successful update, false if some error (likely caller error missing important data)
    public bool UpdateData(
        int setThreshold,
        List<int> selectedTabIndices,
        List<EnhancedItem> filteredStashContents, // i feel like implicitly this comes pre-filtered
        bool includeIdentified = false,
        bool chaosRecipe = true
    )
    {
        // if no tabs are selected (this isn't a realistic case)
        if (selectedTabIndices.Count == 0) return false;

        // setting some new properties
        _setThreshold = setThreshold;
        _currentSelectedTabs = selectedTabIndices;

        // will this ever be done independent of one another?
        // not doubting, just want to be 100% sure lol
        // i guess separating out for readability and debugging is fine
        FilterItemsForRecipe(filteredStashContents, includeIdentified, chaosRecipe);
        CalculateItemAmounts();
        GenerateInProgressItemSets();

        // deriving completed sets by iteratively checking every one of our sets in progress for any missing slots
        // if a set has no missing slots, it is complete
        CompletedSets = _setsInProgress.Count(x => x.EmptyItemSlots.Count == 0);

        NeedsFetching = false;
        return true;
    }

    // this should probably be called right after we update our CurrentStashContents
    private void FilterItemsForRecipe(List<EnhancedItem> filteredStashContents, bool includeIdentified = false, bool chaosRecipe = true)
    {
        // iterate through each item in the provided list
        foreach (var item in filteredStashContents)
        {
            // if it's not rare ignore item (could keep identified items if passed as true)
            // maybe i could optimize here by pre-emptively removing all non-rare items
            // in the calling request? idk if it would truly 'optimize' or if it would
            // just offload the work to another service lol
            if ((item.Identified && !includeIdentified) || item.FrameType != ItemFrameType.Rare)
            {
                continue;
            }

            // if the derived class is not what we're looking for
            // (think rare maps, rare jewels, etc... NOT 'gear')
            if (item.DerivedItemClass == null)
            {
                continue;
            }

            // if an item falls within the ilvl bounds for whichever recipe we're calling
            // chaos recipe ilvl 60 through 74
            // regal recipe ilvl 75+
            if (item.ItemLevel >= 60 && // lower bound for all recipes
                                        // either enforce chaos recipe upper bound or 'ignore' upper bound
                (item.ItemLevel <= 74 || !chaosRecipe))
            {
                // simple check if item is in our tabs
                // checks like this make me want to filter before we get here, save some cycles
                if (_currentSelectedTabs.Contains(item.StashTabIndex))
                {
                    _currentItemsFilteredForRecipe.Add(item);
                }
            }
        }
    }

    private void CalculateItemAmounts()
    {
        foreach (var item in _currentItemsFilteredForRecipe)
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
                case GameTerminology.BodyArmor:
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
    }

    private void GenerateInProgressItemSets()
    {
        _setsInProgress.Clear();
        for (var i = 0; i < _setThreshold; i++)
        {
            // create new 'empty' enhanced item set
            var enhancedItemSet = new EnhancedItemSet();

            // iterate until the end of time (jk)
            while (true)
            {
                // the closest item is assumed to be in another galaxy
                EnhancedItem closestMissingItem = null;
                var minDistance = double.PositiveInfinity;

                // find the for real closes item
                // this is a nested for over each other item in our current item filtered for recipe
                // probably could be optimized? maybe?
                foreach (var item in _currentItemsFilteredForRecipe
                             .Where(item => enhancedItemSet.IsItemClassNeeded(item) && // item is of a class we need
                                            enhancedItemSet.GetItemDistance(item) < minDistance)) // item is closer than the current closest
                {
                    minDistance = enhancedItemSet.GetItemDistance(item);
                    closestMissingItem = item;
                }

                if (closestMissingItem is not null)
                {
                    // if we found a new closer we're good to add it to our enhanced set
                    _ = enhancedItemSet.AddItem(closestMissingItem);
                    // promptly remove it from our pool of 'available' items
                    // do i actually want to do this? lol
                    _ = _currentItemsFilteredForRecipe.Remove(closestMissingItem);
                }
                // you didn't find a closer item, gg break out of infinite loop
                else
                {
                    break;
                }
            }

            // add new enhanced item set to our list of sets in progress
            _setsInProgress.Add(enhancedItemSet);
        }
    }

    // workaround to expose properties as functions on our interface
    public bool RetrieveNeedsFetching() => NeedsFetching;
    public int RetrieveCompletedSets() => CompletedSets;

    public int RetrieveRingsAmount() => RingsAmount;
    public int RetrieveAmuletsAmount() => AmuletsAmount;
    public int RetrieveBeltsAmount() => BeltsAmount;
    public int RetrieveChestsAmount() => ChestsAmount;
    public int RetrieveWeaponsSmallAmount() => WeaponsSmallAmount;
    public int RetrieveWeaponsBigAmount() => WeaponsBigAmount;
    public int RetrieveGlovesAmount() => GlovesAmount;
    public int RetrieveHelmetsAmount() => HelmetsAmount;
    public int RetrieveBootsAmount() => BootsAmount;

}