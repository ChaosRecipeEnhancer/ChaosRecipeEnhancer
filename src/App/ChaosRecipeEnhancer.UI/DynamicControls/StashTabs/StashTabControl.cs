using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.UserControls;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;

public sealed class StashTabControl : ViewModelBase
{
    private bool _quad;

    public StashTabControl(string name, int index)
    {
        TabName = name;
        TabIndex = index;

        // validate settings, somehow need them set
        // might need to change where we initialize the stash tab control
        StashTabUri = ApiEndpoints.IndividualTabContentsEndpoint(Settings.TargetStash, Settings.PathOfExileAccountName, Settings.LeagueName, index);
    }

    public string TabName { get; }

    public int TabIndex { get; }

    public Uri StashTabUri { get; } // might not need this

    public List<Item> ItemsForChaosRecipe { get; } = new(); // items for chaos recipe ONLY in this tab

    public ObservableCollection<StashTabCell> OverlayCellsList { get; } = new(); // i think these cells might be related to the items themselves, will review

    public bool Quad
    {
        get => _quad;
        set => SetProperty(ref _quad, value);
    }

    /// <summary>
    /// For a given list of items, filter out the ones that are not valid for the chaos recipe.
    /// </summary>
    /// <param name="itemList">List of <see cref="Item"/> to be filtered</param>
    public void FilterItemsForChaosRecipe(List<Item> itemList)
    {
        // (re)initialize the cell list
        OverlayCellsList.Clear();
        var size = Quad ? 24 : 12;

        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                // adds 12x12 or 24x24 cells to the list (based on quad or not)
                OverlayCellsList.Add(new StashTabCell(j, i));
            }
        }

        // (re)initialize the item list
        ItemsForChaosRecipe.Clear();

        foreach (var item in itemList)
        {
            // skip items that are not identified (with the setting disabled) or items that are not rare
            // frametype 2 = rare, for some reason; is there an easier way to derive the item rarity?
            if (item.identified && !Settings.IncludeIdentifiedItemsEnabled || item.frameType != 2) continue;

            item.GetItemClass();

            // the case where the item is not a weapon, armor, or jewelry
            if (item.ItemType == null) continue;

            // checking if item is valid for chaos recipe
            if (item.ilvl >= 60 && item.ilvl <= 74)
            {
                item.StashTabIndex = TabIndex;
                ItemsForChaosRecipe.Add(item);
            }

            // review does this account for items that are ilvl 74? (since they are still valid)
        }
    }

    /// <summary>
    /// For a given item, de-activate the cells that it no longer occupies within our <see cref="StashTabGridControl"/>.
    /// </summary>
    /// <param name="item"><see cref="Item"/>, which contains cell positions</param>
    public void DeactivateItemCells(Item item)
    {
        var itemCoordinates = new List<List<int>>();

        for (var i = 0; i < item.w; i++)
        {
            for (var j = 0; j < item.h; j++)
            {
                itemCoordinates.Add(new List<int>
                {
                    item.x + i,
                    item.y + j
                });
            }
        }

        foreach (var cell in OverlayCellsList)
        {
            foreach (var coordinate in itemCoordinates)
            {
                if (coordinate[0] == cell.XIndex && coordinate[1] == cell.YIndex)
                {
                    cell.Deactivate();
                }
            }
        }
    }

    /// <summary>
    /// For a given item, activate the cells that it will occupy within our <see cref="StashTabGridControl"/>.
    /// </summary>
    /// <param name="item"><see cref="Item"/>, which contains cell positions</param>
    public void ActivateItemCells(Item item)
    {
        var allCoordinates = new List<List<int>>();

        for (var i = 0; i < item.w; i++)
        {
            for (var j = 0; j < item.h; j++)
            {
                allCoordinates.Add(new List<int>
                {
                    item.x + i,
                    item.y + j
                });
            }
        }

        foreach (var cell in OverlayCellsList)
        {
            foreach (var coordinate in allCoordinates)
            {
                if (coordinate[0] == cell.XIndex && coordinate[1] == cell.YIndex)
                {
                    cell.Activate(ref item);
                }
            }
        }
    }
}