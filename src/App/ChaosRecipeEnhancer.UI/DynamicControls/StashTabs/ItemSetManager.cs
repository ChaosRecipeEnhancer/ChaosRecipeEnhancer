using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;

internal interface ISelectedStashTabHandler
{
    public StashTab SelectedStashTab { get; set; }
}

public class ItemSetManager : ViewModelBase, ISelectedStashTabHandler
{
    private readonly List<ItemSet> _itemSetList = new();

    private StashTab _selectedStashTab;

    public ItemSetManager()
    {
        Settings.Default.PropertyChanged += OnSettingsChanged;
    }

    public StashTab SelectedStashTab
    {
        get => _selectedStashTab;
        set
        {
            if (SetProperty(ref _selectedStashTab, value) && _selectedStashTab is not null)
                if (Settings.Default.SelectedStashTabName is not null)
                    Settings.Default.SelectedStashTabName[0] = _selectedStashTab.TabName;
        }
    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.FullSetThreshold) || e.PropertyName == nameof(Settings.SetTrackerOverlayItemCounterDisplayMode)) _selectedStashTab.UpdateDisplay();
    }

    public void UpdateData()
    {
        if (_selectedStashTab is null) return;

        CalculateItemAmounts();
        GenerateItemSets();
        ActivateAllCellsForNextSet();

        _selectedStashTab.FullSets = _itemSetList.Count(x => x.EmptyItemSlots.Count == 0);
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
        foreach (var item in _selectedStashTab.ItemsForChaosRecipe)
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

        _selectedStashTab.UpdateAmounts(amounts);
    }

    private void ActivateAllCellsForNextSet()
    {
        // Sets are filled from first index so if first set has missing items we have no full sets
        if (_itemSetList.Count == 0 || _itemSetList[0].EmptyItemSlots.Count > 0) return;

        foreach (var i in _itemSetList[0].ItemList) _selectedStashTab.ActivateItemCells(i);
    }

    public void OnItemCellClicked(Cell cell)
    {
        // Sets are filled from first index so if first set has missing items we have no full sets
        if (cell is null || _itemSetList[0].EmptyItemSlots.Count > 0) return;

        _ = _itemSetList[0].ItemList.Remove(cell.Item);
        _selectedStashTab.DeactivateItemCells(cell.Item);

        if (_itemSetList[0].ItemList.Count == 0)
        {
            _itemSetList.RemoveAt(0);
            ActivateAllCellsForNextSet();
        }
    }

    private void GenerateItemSets()
    {
        _itemSetList.Clear();
        for (var i = 0; i < Settings.Default.FullSetThreshold; i++)
        {
            var itemSet = new ItemSet();
            while (true)
            {
                Item closestMissingItem = null;
                var minDistance = double.PositiveInfinity;

                foreach (var item in _selectedStashTab.ItemsForChaosRecipe.Where(item => itemSet.NeedsItem(item) && itemSet.GetItemDistance(item) < minDistance))
                {
                    minDistance = itemSet.GetItemDistance(item);
                    closestMissingItem = item;
                }

                if (closestMissingItem is not null)
                {
                    _ = itemSet.AddItem(closestMissingItem);
                    _ = _selectedStashTab.ItemsForChaosRecipe.Remove(closestMissingItem);
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