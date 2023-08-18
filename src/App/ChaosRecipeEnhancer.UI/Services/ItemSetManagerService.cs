using System.Collections.Generic;
using System.Linq;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models;

namespace ChaosRecipeEnhancer.UI.Services;

public interface IItemSetManagerService
{
    public void UpdateStashMetadata(BaseStashTabMetadataList metadata);

    public bool UpdateData(
        int setThreshold,
        List<int> selectedTabIndices,
        List<EnhancedItem> filteredStashContents,
        bool includeIdentified = false,
        bool chaosRecipe = true
    );

    public void GenerateItemSets(bool chaosRecipe);
    public void CalculateItemAmounts();
    public void ResetCompletedSets();
    public void ResetItemAmounts();

    public BaseStashTabMetadataList RetrieveStashTabMetadataList();
    public bool RetrieveNeedsFetching();
    public bool RetrieveNeedsLowerLevel();
    public int RetrieveCompletedSetCount();
    public List<EnhancedItemSet> RetrieveSetsInProgress();

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
    private List<EnhancedItemSet> _setsInProgress = new();
    private List<EnhancedItem> _currentItemsFilteredForRecipe = new(); // filtered for chaos recipe
    private BaseStashTabMetadataList _stashTabMetadataList;

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
    public int CompletedSetCount { get; set; }
    public bool NeedsFetching { get; set; } = true;
    public bool NeedsLowerLevel { get; set; } = false;

    // temporary housing for this field that is needed by some components
    // i'd likely want to move this to its own service tbh

    public void UpdateStashMetadata(BaseStashTabMetadataList metadata)
    {
        _stashTabMetadataList = metadata;
    }

    // this is the primary method being called by external entities
    // return true if successful update, false if some error (likely caller error missing important data)
    public bool UpdateData(
        int setThreshold,
        List<int> selectedTabIndices,
        List<EnhancedItem> filteredStashContents,
        bool includeIdentified = false,
        bool chaosRecipe = true
    )
    {
        // if no tabs are selected (this isn't a realistic case)
        if (selectedTabIndices.Count == 0) return false;

        // setting some new properties
        _setThreshold = setThreshold;
        _currentItemsFilteredForRecipe = filteredStashContents;

        NeedsFetching = false;
        return true;
    }

    public void CalculateItemAmounts()
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

    public void ResetCompletedSets()
    {
        CompletedSetCount = 0;
    }

    public void ResetItemAmounts()
    {
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

    public void GenerateItemSets(bool chaosRecipe)
    {
        _setsInProgress.Clear();

        var listOfSets = new List<EnhancedItemSet>();

        // iterate once to create as many sets as possible that can produce recipe
        if (chaosRecipe)
        {
            var noChaosItemsLeft = false;

            // for every set
            for (var i = 0; i < _setThreshold; i++)
            {
                // create new 'empty' enhanced item set
                var enhancedItemSet = new EnhancedItemSet();

                // if there are still chaos items left in our stash
                if (!noChaosItemsLeft)
                {
                    // filter for chaos recipe eligible items
                    var eligibleChaosItems = _currentItemsFilteredForRecipe.Where(x => x.IsChaosRecipeEligible).ToList();

                    // if there are any chaos items left (do we need this?)
                    if (eligibleChaosItems.Count != 0)
                    {
                        // try to add a single eligible chaos item in the set (where we're iterate in our loop on line 166)
                        foreach (var attempt in eligibleChaosItems)
                        {
                            var addSuccessful = enhancedItemSet.TryAddItem(attempt);

                            // if we successfully add to set (i.e. it wasn't an item slot that was already taken)
                            if (addSuccessful)
                            {
                                // remove from our stash
                                _currentItemsFilteredForRecipe.Remove(attempt);

                                // break out of loop
                                break;
                            }
                        }
                    }
                    else
                    {
                        // set flag to indicate no chaos items left in our stash
                        noChaosItemsLeft = true;

                        // we now know we need lower level items
                        // this will allow us to notify dependencies of this
                        NeedsLowerLevel = true;
                    }
                    // we still need to loop to create the rest of the sets (even if they don't have chaos items in them)
                }

                listOfSets.Add(enhancedItemSet);

                if (i == _setThreshold - 1 && !noChaosItemsLeft)
                {
                    // we're on the last set and we know we still have chaos items
                    NeedsLowerLevel = false;
                }
            }
        }
        else
        {
            for (var i = 0; i < _setThreshold; i++)
            {
                listOfSets.Add(new EnhancedItemSet());
            }
        }

        for (var i = 0; i < _setThreshold; i++)
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
                foreach (var item in _currentItemsFilteredForRecipe
                             .Where(item => listOfSets[i].IsItemClassNeeded(item) && // item is of a class we need
                                            listOfSets[i].GetItemDistance(item) < minDistance)) // item is closer than the current closest
                {
                    minDistance = listOfSets[i].GetItemDistance(item);
                    closestMissingItem = item;
                }

                if (closestMissingItem is not null)
                {
                    // if we found a new closer we're good to add it to our enhanced set
                    _ = listOfSets[i].TryAddItem(closestMissingItem);
                    // promptly remove it from our pool of 'available' items
                    // do i actually want to do this? lol
                    _ = _currentItemsFilteredForRecipe.Remove(closestMissingItem);
                }
                // you didn't find a closer item, gg break out of infinite loop
                else
                {
                    break;
                }

                if (chaosRecipe)
                {
                    var canProduce = listOfSets[i].Items.FirstOrDefault(x => x.IsChaosRecipeEligible, null);

                    // my reason for separating out this logic is that it's a bit more readable and debuggable
                    if (canProduce is not null)
                    {
                        listOfSets[i].HasRecipeQualifier = true;

                        if (listOfSets[i].EmptyItemSlots.Count == 0)
                        {
                            CompletedSetCount++;
                        }
                    }
                }
            }

            // add new enhanced item set to our list of sets in progress
            _setsInProgress = listOfSets;
        }
    }

    #region Properties as Functions

    // workaround to expose properties as functions on our interface
    public BaseStashTabMetadataList RetrieveStashTabMetadataList() => _stashTabMetadataList;
    public bool RetrieveNeedsFetching() => NeedsFetching;
    public bool RetrieveNeedsLowerLevel() => NeedsLowerLevel;
    public int RetrieveCompletedSetCount() => CompletedSetCount;
    public List<EnhancedItemSet> RetrieveSetsInProgress() => _setsInProgress;

    // item amount public properties via functions
    public int RetrieveRingsAmount() => RingsAmount;
    public int RetrieveAmuletsAmount() => AmuletsAmount;
    public int RetrieveBeltsAmount() => BeltsAmount;
    public int RetrieveChestsAmount() => ChestsAmount;
    public int RetrieveWeaponsSmallAmount() => WeaponsSmallAmount;
    public int RetrieveWeaponsBigAmount() => WeaponsBigAmount;
    public int RetrieveGlovesAmount() => GlovesAmount;
    public int RetrieveHelmetsAmount() => HelmetsAmount;
    public int RetrieveBootsAmount() => BootsAmount;

    #endregion
}