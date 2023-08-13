using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;

internal interface ISelectedStashTabHandler
{
    public StashManagerControl StashManagerControl { get; set; }
}

public class ItemSetManager : ViewModelBase, ISelectedStashTabHandler
{
    private readonly List<ItemSet> _itemSetList = new();
    private StashManagerControl _stashManager;

    public ItemSetManager()
    {
        _stashManager = new StashManagerControl();
        Settings.Default.PropertyChanged += OnSettingsChanged;
    }

    public StashManagerControl StashManagerControl
    {
        get => _stashManager;
        set => SetProperty(ref _stashManager, value);
    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        // if settings change that affect the display of the set tracker, update it
        if (e.PropertyName == nameof(Settings.FullSetThreshold) ||
            e.PropertyName == nameof(Settings.SetTrackerOverlayItemCounterDisplayMode))
        {
            _stashManager.UpdateSetTrackerDisplay();
        }
    }

    public void UpdateData()
    {
        // no need to update our data if we're not tracking sets
        // realistically this case will never happen, but it's here for completeness
        if (_stashManager.StashTabControls is null || _stashManager.StashTabControls.Count == 0) return;

        CalculateItemAmounts();
        GenerateItemSets();
        ActivateAllCellsForNextSet();

        _stashManager.FullSets = _itemSetList.Count(x => x.EmptyItemSlots.Count == 0);
    }

    private void CalculateItemAmounts()
    {
        // 0: rings
        // 1: amulets
        // 2: belts
        // 3: chests
        // 4: weaponsSmall
        // 5: weaponsBig
        // 6: gloves
        // 7: helmets
        // 8: boots
        var amounts = new int[9];

        // iterate through every stash tab in our stash and check what items we have to complete chaos recipes
        // it's worth noting that we take care of the logic as a part of this process that calculates
        // items that require sets (2x the amount) like rings or 1-handed weapons (zemoto is a legend)
        foreach (var tab in _stashManager.StashTabControls)
        {
            foreach (var item in tab.ItemsForChaosRecipe)
                if (item.ItemType == "Rings")
                    amounts[0]++;
                else if (item.ItemType == "Amulets")
                    amounts[1]++;
                else if (item.ItemType == "Belts")
                    amounts[2]++;
                else if (item.ItemType == "BodyArmours")
                    amounts[3]++;
                else if (item.ItemType == "OneHandWeapons")
                    amounts[4]++;
                else if (item.ItemType == "TwoHandWeapons")
                    amounts[5]++;
                else if (item.ItemType == "Gloves")
                    amounts[6]++;
                else if (item.ItemType == "Helmets")
                    amounts[7]++;
                else if (item.ItemType == "Boots") amounts[8]++;
        }

        _stashManager.UpdateItemCounts(amounts);
    }


    // we can't simply iterate through these, there has to be a better data structure for this
    // since the items have to map to the correct tab

    private void ActivateAllCellsForNextSet()
    {
        // Sets are filled from first index so if first set has missing items we have no full sets
        if (_itemSetList.Count == 0 || _itemSetList[0].EmptyItemSlots.Count > 0) return;

        foreach (var tab in _stashManager.StashTabControls)
        {
            // we're using _itemSetList as a queue, so this looks more confusing that it should be
            foreach (var i in _itemSetList[0].ItemList)
            {
                // activate a given item in the stash tab ONLY if it belongs to the tab
                if (i.StashTabIndex == tab.TabIndex)
                {
                    tab.ActivateItemCells(i);
                }
            }
        }
    }

    public void OnItemCellClicked(StashTabCell stashTabCell)
    {
        // Sets are filled from first index so if first set has missing items we have no full sets
        if (stashTabCell is null || _itemSetList[0].EmptyItemSlots.Count > 0) return;

        // remove the first item from our 'queue'
        _ = _itemSetList[0].ItemList.Remove(stashTabCell.Item);

        // de-activate the cells pertaining to the removed item in its associated stash
        _stashManager.StashTabControls[stashTabCell.Item.StashTabIndex].DeactivateItemCells(stashTabCell.Item);

        if (_itemSetList[0].ItemList.Count == 0)
        {
            _itemSetList.RemoveAt(0);
            ActivateAllCellsForNextSet();
        }
    }

    private void GenerateItemSets()
    {
        // reset our list of sets
        _itemSetList.Clear();

        // we have N item class sets to generate, so we'll assign as many items of that class into a set as possible
        for (var i = 0; i < Settings.FullSetThreshold; i++)
        {
            // generate an item set for a given item class (e.g. gloves)
            var itemSet = new ItemSet();

            while (true)
            {
                Item closestMissingItem = null;
                var minDistance = double.PositiveInfinity;

                // find the closest item to the set (taking to account items in other tabs)
                foreach (var tab in _stashManager.StashTabControls)
                {
                    foreach (var item in tab.ItemsForChaosRecipe.Where(item => itemSet.NeedsItem(item) && itemSet.GetItemDistance(item) < minDistance))
                    {
                        minDistance = itemSet.GetItemDistance(item);
                        closestMissingItem = item;
                    }
                }

                if (closestMissingItem is not null)
                {
                    _ = itemSet.AddItem(closestMissingItem);
                    _ = _stashManager.StashTabControls[closestMissingItem.StashTabIndex].ItemsForChaosRecipe.Remove(closestMissingItem);
                }
                else
                {
                    break;
                }
            }

            _itemSetList.Add(itemSet);
        }
    }
}