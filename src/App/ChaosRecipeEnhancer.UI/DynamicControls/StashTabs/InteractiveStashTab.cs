using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;

public class InteractiveStashTab : ViewModelBase
{
    private int _amuletsAmount;

    private int _beltsAmount;

    private int _bootsAmount;

    private int _chestsAmount;

    private int _fullSets;

    private int _glovesAmount;

    private int _helmetsAmount;

    private bool _needsItemFetch = true;

    private bool _quad;

    private int _ringsAmount;
    private int _weaponsBigAmount;

    private int _weaponsSmallAmount;

    public InteractiveStashTab(string name, int index, Uri tabUri)
    {
        TabName = name;
        TabIndex = index;
        StashTabUri = tabUri;
    }

    public string TabName { get; }
    public int TabIndex { get; }
    public Uri StashTabUri { get; }

    public List<Item> ItemsForChaosRecipe { get; } = new();

    public ObservableCollection<Cell> OverlayCellsList { get; } = new();

    public bool Quad
    {
        get => _quad;
        set => SetProperty(ref _quad, value);
    }

    public bool NeedsItemFetch
    {
        get => _needsItemFetch;
        set => SetProperty(ref _needsItemFetch, value);
    }

    public int FullSets
    {
        get => _fullSets;
        set => SetProperty(ref _fullSets, value);
    }

    private bool ShowAmountNeeded => Settings.Default.SetTrackerOverlayItemCounterDisplayMode == 2;
    public int RingsAmount => ShowAmountNeeded ? Math.Max(Settings.Default.FullSetThreshold * 2 - _ringsAmount, 0) : _ringsAmount;
    public bool RingsActive => NeedsItemFetch || Settings.Default.FullSetThreshold * 2 - _ringsAmount > 0;
    public int AmuletsAmount => ShowAmountNeeded ? Math.Max(Settings.Default.FullSetThreshold - _amuletsAmount, 0) : _amuletsAmount;
    public bool AmuletsActive => NeedsItemFetch || Settings.Default.FullSetThreshold - _amuletsAmount > 0;
    public int BeltsAmount => ShowAmountNeeded ? Math.Max(Settings.Default.FullSetThreshold - _beltsAmount, 0) : _beltsAmount;
    public bool BeltsActive => NeedsItemFetch || Settings.Default.FullSetThreshold - _beltsAmount > 0;
    public int ChestsAmount => ShowAmountNeeded ? Math.Max(Settings.Default.FullSetThreshold - _chestsAmount, 0) : _chestsAmount;
    public bool ChestsActive => NeedsItemFetch || Settings.Default.FullSetThreshold - _chestsAmount > 0;
    public int WeaponsAmount => ShowAmountNeeded ? Math.Max(Settings.Default.FullSetThreshold * 2 - (_weaponsSmallAmount + _weaponsBigAmount * 2), 0) : _weaponsSmallAmount + _weaponsBigAmount * 2;
    public bool WeaponsActive => NeedsItemFetch || Settings.Default.FullSetThreshold * 2 - (_weaponsSmallAmount + _weaponsBigAmount * 2) > 0;
    public int GlovesAmount => ShowAmountNeeded ? Math.Max(Settings.Default.FullSetThreshold - _glovesAmount, 0) : _glovesAmount;
    public bool GlovesActive => NeedsItemFetch || Settings.Default.FullSetThreshold - _glovesAmount > 0;
    public int HelmetsAmount => ShowAmountNeeded ? Math.Max(Settings.Default.FullSetThreshold - _helmetsAmount, 0) : _helmetsAmount;
    public bool HelmetsActive => NeedsItemFetch || Settings.Default.FullSetThreshold - _helmetsAmount > 0;
    public int BootsAmount => ShowAmountNeeded ? Math.Max(Settings.Default.FullSetThreshold - _bootsAmount, 0) : _bootsAmount;
    public bool BootsActive => NeedsItemFetch || Settings.Default.FullSetThreshold - _bootsAmount > 0;

    public void FilterItemsForChaosRecipe(List<Item> itemList)
    {
        // (re)initialize the cell list
        OverlayCellsList.Clear();
        var size = Quad ? 24 : 12;
        for (var i = 0; i < size; i++)
        for (var j = 0; j < size; j++)
            OverlayCellsList.Add(new Cell(j, i));

        ItemsForChaosRecipe.Clear();
        foreach (var item in itemList)
        {
            if ((item.identified && !Settings.Default.IncludeIdentifiedItemsEnabled) || item.frameType != 2) continue;

            item.GetItemClass();
            if (item.ItemType == null) continue;

            if (item.ilvl >= 60 && item.ilvl <= 74)
            {
                item.StashTabIndex = TabIndex;
                ItemsForChaosRecipe.Add(item);
            }
        }
    }

    public void DeactivateItemCells()
    {
        foreach (var cell in OverlayCellsList) cell.Deactivate();
    }

    public void DeactivateItemCells(Item item)
    {
        var itemCoordinates = new List<List<int>>();

        for (var i = 0; i < item.w; i++)
        for (var j = 0; j < item.h; j++)
            itemCoordinates.Add(new List<int> { item.x + i, item.y + j });

        foreach (var cell in OverlayCellsList)
        foreach (var coordinate in itemCoordinates)
            if (coordinate[0] == cell.XIndex && coordinate[1] == cell.YIndex)
                cell.Deactivate();
    }

    public void ActivateItemCells(Item item)
    {
        var AllCoordinates = new List<List<int>>();

        for (var i = 0; i < item.w; i++)
        for (var j = 0; j < item.h; j++)
            AllCoordinates.Add(new List<int> { item.x + i, item.y + j });

        foreach (var cell in OverlayCellsList)
        foreach (var coordinate in AllCoordinates)
            if (coordinate[0] == cell.XIndex && coordinate[1] == cell.YIndex)
                cell.Activate(ref item);
    }

    // 0: rings
    // 1: amulets
    // 2: belts
    // 3: chests
    // 4: weaponsSmall
    // 5: weaponsBig
    // 6: gloves
    // 7: helmets
    // 8: boots
    public void UpdateAmounts(int[] amounts)
    {
        _ringsAmount = amounts[0];
        _amuletsAmount = amounts[1];
        _beltsAmount = amounts[2];
        _chestsAmount = amounts[3];
        _weaponsSmallAmount = amounts[4];
        _weaponsBigAmount = amounts[5];
        _glovesAmount = amounts[6];
        _helmetsAmount = amounts[7];
        _bootsAmount = amounts[8];
        NeedsItemFetch = false;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        OnPropertyChanged(nameof(RingsAmount));
        OnPropertyChanged(nameof(RingsActive));

        OnPropertyChanged(nameof(AmuletsAmount));
        OnPropertyChanged(nameof(AmuletsActive));

        OnPropertyChanged(nameof(BeltsAmount));
        OnPropertyChanged(nameof(BeltsActive));

        OnPropertyChanged(nameof(ChestsAmount));
        OnPropertyChanged(nameof(ChestsActive));

        OnPropertyChanged(nameof(WeaponsAmount));
        OnPropertyChanged(nameof(WeaponsActive));

        OnPropertyChanged(nameof(GlovesAmount));
        OnPropertyChanged(nameof(GlovesActive));

        OnPropertyChanged(nameof(HelmetsAmount));
        OnPropertyChanged(nameof(HelmetsActive));

        OnPropertyChanged(nameof(BootsAmount));
        OnPropertyChanged(nameof(BootsActive));
    }
}