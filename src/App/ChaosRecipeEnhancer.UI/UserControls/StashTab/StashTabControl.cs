using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTab;

public class StashTabControl : ViewModelBase
{
    private SolidColorBrush _tabHeaderColor;
    private Thickness _tabHeaderWidth;

    public StashTabControl(string name, int index)
    {
        TabName = name;
        TabIndex = index;
        TabHeaderColor = Brushes.Transparent;
        TabHeaderWidth = new Thickness(Settings.Default.StashTabOverlayIndividualTabHeaderWidth, 2, Settings.Default.StashTabOverlayIndividualTabHeaderWidth, 2);
    }

    public int TabIndex { get; }
    public ObservableCollection<InteractiveStashTabCell> OverlayCellsList { get; } = new();
    public TextBlock TabNameContainer { get; set; }
    public string TabName { get; set; }
    public bool Quad { get; set; }

    public SolidColorBrush TabHeaderColor
    {
        get => _tabHeaderColor;
        set => SetProperty(ref _tabHeaderColor, value);
    }

    public Thickness TabHeaderWidth
    {
        get => _tabHeaderWidth;
        set => SetProperty(ref _tabHeaderWidth, value);
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
            foreach (var coordinate in allCoordinates)
            {
                if (coordinate.X == cell.XIndex && coordinate.Y == cell.YIndex)
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
            foreach (var coordinate in allCoordinates)
            {
                if (coordinate.X == cell.XIndex && coordinate.Y == cell.YIndex)
                {
                    cell.Active = true;
                    cell.ItemModel = itemModel;
                }
            }
        }
    }
}