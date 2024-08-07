﻿using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Properties;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTab;

public class StashTabControl : CreViewModelBase
{
    private SolidColorBrush _tabHeaderColor;

    public StashTabControl(string id, string name, int index)
    {
        Id = id;
        Name = name;
        Index = index;

        var tabHeaderColor = !string.IsNullOrWhiteSpace(Settings.Default.StashTabOverlayTabDefaultBackgroundColor)
            ? (Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayTabDefaultBackgroundColor)
            : Colors.Transparent;

        var opacity = Settings.Default.StashTabOverlayTabOpacity;
        var tabHeaderColorWithOpacity = new Color
        {
            A = (byte)(opacity * 255),
            R = tabHeaderColor.R,
            G = tabHeaderColor.G,
            B = tabHeaderColor.B
        };

        TabHeaderColor = new SolidColorBrush(tabHeaderColorWithOpacity);
    }

    public string Id { get; set; }
    public TextBlock NameContainer { get; set; }
    public string Name { get; set; }
    public int Index { get; }
    public bool Quad { get; set; }

    public ObservableCollection<InteractiveStashTabCell> OverlayCellsList { get; } = [];

    public SolidColorBrush TabHeaderColor
    {
        get => _tabHeaderColor;
        set => SetProperty(ref _tabHeaderColor, value);
    }

    public void SetTabHeaderColorForHighlightingFromUserSettings()
    {
        Color tabHeaderColor = Colors.Transparent; // Default to transparent color

        // Check if the setting value is not null or whitespace
        if (!string.IsNullOrWhiteSpace(Settings.Default.StashTabOverlayHighlightColor))
        {
            try
            {
                // Attempt to convert the setting string to a Color
                tabHeaderColor = (Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayHighlightColor);
            }
            catch (FormatException e)
            {
                Log.Error(e, "Failed to convert the setting string to a Color object.");
                // If the conversion fails, tabHeaderColor remains as the default transparent color
            }
        }

        // Ensure opacity is within a valid range [0, 1]
        var opacity = Math.Max(0, Math.Min(1, Settings.Default.StashTabOverlayTabOpacity));

        var tabHeaderColorWithOpacity = new Color
        {
            A = (byte)(opacity * 255), // Apply opacity
            R = tabHeaderColor.R,
            G = tabHeaderColor.G,
            B = tabHeaderColor.B
        };

        TabHeaderColor = new SolidColorBrush(tabHeaderColorWithOpacity);
    }

    public void MarkItemWithPickIndicator(EnhancedItem item)
    {
        // Find the cells in the grid corresponding to the item's position
        var cellsToMark = OverlayCellsList.Where(c => c.ItemModel == item).ToList();

        foreach (var cell in cellsToMark)
        {
            cell.ButtonText = "●";
        }
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