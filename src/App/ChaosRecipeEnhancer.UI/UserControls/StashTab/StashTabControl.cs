using ChaosRecipeEnhancer.UI.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTab;

public class StashTabControl : CreViewModelBase
{
    private SolidColorBrush _tabHeaderColor;

    public StashTabControl(string name, int index)
    {
        Name = name;
        Index = index;
        TabHeaderColor = Brushes.Transparent;
    }

    public ObservableCollection<InteractiveStashTabCell> OverlayCellsList { get; } = [];
    public TextBlock NameContainer { get; set; }
    public string Name { get; set; }
    public int Index { get; }
    public bool Quad { get; set; }

    public SolidColorBrush TabHeaderColor
    {
        get => _tabHeaderColor;
        set => SetProperty(ref _tabHeaderColor, value);
    }

    /// <summary>
    /// Creates an N x N grid of interactive <see cref="InteractiveStashTabCell"/> objects. All objects are initialized to inactive.
    /// </summary>
    /// <param name="size">Represent the dimensions of our Cell object grid (Size = N)</param>
    private void GenerateInteractiveStashCellGrid(int size)
    {
        for (var i = 0; i < size; i++)
        {
            for (var j = 0; j < size; j++)
            {
                OverlayCellsList.Add(new InteractiveStashTabCell
                {
                    Active = false,
                    XIndex = j,
                    YIndex = i
                });
            }
        }
    }

    public void PrepareOverlayList()
    {
        // If quad tab, set grid to 24 x 24, else set to 12 x 12 grid
        var size = Quad ? 24 : 12;
        GenerateInteractiveStashCellGrid(size);
    }

    public void DeactivateSingleItemCells(EnhancedItem itemModel)
    {
        // Initializing a list of tuples that represent our X,Y coordinates
        var allCoordinates = new List<(int X, int Y)>();

        // For a given in-game Item, populate a list of tuples that represent
        // their X,Y coordinates on our stash grid
        for (var i = 0; i < itemModel.Width; i++)
        {
            for (var j = 0; j < itemModel.Height; j++)
            {
                allCoordinates.Add((itemModel.X + i, itemModel.Y + j));
            }
        }

        foreach (var cell in OverlayCellsList)
        {
            foreach (var (X, Y) in allCoordinates)
            {
                if (X == cell.XIndex && Y == cell.YIndex)
                    cell.Active = false;
            }
        }
    }

    public void ActivateItemCells(EnhancedItem itemModel)
    {
        // Initializing a list of tuples that represent our X,Y coordinates
        var allCoordinates = new List<(int X, int Y)>();

        // For a given in-game Item, populate a list of tuples that represent
        // their X,Y coordinates on our stash grid
        for (var i = 0; i < itemModel.Width; i++)
        {
            for (var j = 0; j < itemModel.Height; j++)
            {
                allCoordinates.Add((itemModel.X + i, itemModel.Y + j));
            }
        }

        foreach (var cell in OverlayCellsList)
        {
            foreach (var (X, Y) in allCoordinates)
            {
                if (X == cell.XIndex && Y == cell.YIndex)
                {
                    cell.Active = true;
                    cell.ItemModel = itemModel;
                }
            }
        }
    }
}