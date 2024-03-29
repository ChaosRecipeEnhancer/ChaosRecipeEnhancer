using ChaosRecipeEnhancer.UI.Native;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Windows;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTab;

/// <summary>
/// Represents a combination of a button and its corresponding stash tab cell.
/// </summary>
internal class ButtonAndCell
{
    /// <summary>
    /// Gets the WPF Button associated with a stash tab cell.
    /// </summary>
    public Button Button { get; init; }

    /// <summary>
    /// Gets the stash tab cell associated with the button.
    /// </summary>
    public InteractiveStashTabCell InteractiveStashTabCell { get; init; }
}

/// <summary>
/// Provides functionality to determine interactions within the stash tab overlay window.
/// </summary>
public static class Coordinates
{
    /// <summary>
    /// Handles click events within the stash tab overlay, determining whether specific elements were clicked.
    /// </summary>
    /// <param name="stashTabOverlayWindow">The stash tab overlay window instance.</param>
    /// <param name="e">The mouse hook event arguments.</param>
    public static void OverlayClickEvent(StashTabOverlayWindow stashTabOverlayWindow, MouseHookEventArgs e)
    {
        try
        {
            if (StashTabControlManager.StashTabControls.Count == 0)
            {
                Log.Debug("OverlayClickEvent: No tabs are selected. Hiding overlay.");
                stashTabOverlayWindow.Hide();
                return;
            }

            if (!stashTabOverlayWindow.IsOpen)
            {
                Log.Debug("OverlayClickEvent: Overlay is not open.");
                return;
            }

            // first, check if the edit button was clicked, as this requires the least amount of processing
            if (CheckIfEditButtonClicked(stashTabOverlayWindow.EditModeButton, e.X, e.Y))
            {
                stashTabOverlayWindow.HandleEditButton();
                return;
            }

            // second, check if the tab name container was clicked as this requires a bit more processing
            for (int stash = 0; stash < StashTabControlManager.StashTabControls.Count; stash++)
            {
                if (CheckIfTabNameContainerClicked(StashTabControlManager.StashTabControls[stash], e.X, e.Y))
                {
                    stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex = stash;
                    break;
                }
            }

            // third, check if an item was clicked; this requires the most processing
            // controle can be of type NormalStashGrid or QuadStashGrid - both have the same interface
            if (stashTabOverlayWindow.StashTabOverlayTabControl.SelectedContent is not StashGridBase control) return;

            var activeCells = GetAllActiveCells(stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex);
            var items = GetItemsFromActiveCells(activeCells);

            foreach (var item in items)
            {
                var itemBounds = GetItemBounds(item, control);
                if (CheckIfItemClicked(itemBounds, e.X, e.Y))
                {
                    // Activate the next cell in the stash tab
                    stashTabOverlayWindow.ActivateNextCell(true, item.First());
                    return;
                }
            }
        }
        catch (InvalidOperationException ex)
        {
            Log.Error(ex, "OverlayClickEvent: An error occurred while handling the overlay click event.");
        }
    }

    /// <summary>
    /// Retrieves all active cells for a given stash tab.
    /// </summary>
    /// <param name="index">The index of the stash tab.</param>
    /// <returns>A list of all active stash tab cells.</returns>
    private static List<InteractiveStashTabCell> GetAllActiveCells(int index)
    {
        var activeCells = new List<InteractiveStashTabCell>();
        foreach (var cell in StashTabControlManager.StashTabControls[index].OverlayCellsList)
        {
            if (cell.Active)
            {
                activeCells.Add(cell);
            }
        }

        Log.Debug($"GetAllActiveCells: Found {activeCells.Count} active cells in stash tab at index {index}.");
        return activeCells;
    }

    /// <summary>
    /// Groups active stash tab cells into items based on their associated ItemModel's Id.
    /// </summary>
    /// <param name="activeCells">The list of active stash tab cells.</param>
    /// <returns>A list of lists, where each inner list represents an item composed of one or more stash tab cells.</returns>
    private static List<List<InteractiveStashTabCell>> GetItemsFromActiveCells(List<InteractiveStashTabCell> activeCells)
    {
        // Create a list to store the grouped items
        var items = new List<List<InteractiveStashTabCell>>();

        // Create a set to keep track of processed item IDs
        var processedItemIds = new HashSet<string>();

        // Iterate over each active cell
        foreach (var cell in activeCells)
        {
            // Check if the cell has an associated ItemModel and the item hasn't been processed before
            if (cell.ItemModel != null && !processedItemIds.Contains(cell.ItemModel.Id))
            {
                // Create a new list to represent the current item and add the current cell to it
                var item = new List<InteractiveStashTabCell> { cell };

                // Add the item's ID to the processed item IDs set
                processedItemIds.Add(cell.ItemModel.Id);

                // Iterate over the remaining active cells
                foreach (var otherCell in activeCells)
                {
                    // Check if the other cell is different from the current cell,
                    // has an associated ItemModel, and has the same ItemModel.Id as the current item
                    if (otherCell != cell && otherCell.ItemModel != null && otherCell.ItemModel.Id == cell.ItemModel.Id)
                    {
                        // Add the other cell to the current item's list
                        item.Add(otherCell);
                    }
                }

                // Add the current item's list to the items list
                items.Add(item);
            }
        }

        // Return the list of grouped items
        return items;
    }

    /// <summary>
    /// Retrieves the bounds of a collection of stash tab cells.
    /// </summary>
    /// <param name="cells">The collection of stash tab cells.</param>
    /// <param name="control">The stash tab grid control.</param>
    /// <returns>The bounds of the collection of stash tab cells.</returns>
    private static Rect GetItemBounds(IEnumerable<InteractiveStashTabCell> cells, StashGridBase control)
    {
        double left = double.MaxValue;
        double top = double.MaxValue;
        double right = 0;
        double bottom = 0;
        bool validButtonFound = false;

        foreach (var cell in cells)
        {
            var button = control.GetButtonFromCell(cell);
            if (button != null)
            {
                validButtonFound = true;
                var position = VisualUtilities.GetScreenCoordinates(button);
                left = Math.Min(left, position.X);
                top = Math.Min(top, position.Y);
                right = Math.Max(right, position.X + button.ActualWidth);
                bottom = Math.Max(bottom, position.Y + button.ActualHeight);
            }
        }

        // If no valid button was found (usually happens when we switch stash tabs)
        if (!validButtonFound)
        {
            // Return a default Rect
            return Rect.Empty;
        }

        return new Rect(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Determines if a tab name container was clicked based on the mouse click coordinates.
    /// </summary>
    /// <param name="stashTabControl">The stash tab control.</param>
    /// <returns>True if the tab name container was clicked; otherwise, false.</returns>
    private static bool CheckIfTabNameContainerClicked(StashTabControl stashTabControl, int clickX, int clickY)
        // Give a bit of additional horizontal / vertical offset to make the stash tab name container easier to click
        => CheckIfClicked(stashTabControl.NameContainer, clickX, clickY, Settings.Default.StashTabOverlayIndividualTabHeaderPadding, 2, 6);

    /// <summary>
    /// Determines if the edit button was clicked based on the mouse click coordinates.
    /// </summary>
    /// <param name="editButton">The edit button.</param>
    /// <returns>True if the edit button was clicked; otherwise, false.</returns>
    private static bool CheckIfEditButtonClicked(FrameworkElement editButton, int clickX, int clickY)
        => CheckIfClicked(editButton, clickX, clickY);

    /// <summary>
    /// Determines if an item was clicked based on the mouse click coordinates.
    /// </summary>
    /// <param name="itemBounds">The bounds of the item.</param>
    /// <param name="clickX">The X-coordinate of the mouse click.</param>
    /// <param name="clickY">The Y-coordinate of the mouse click.</param>
    /// <returns>True if the item was clicked; otherwise, false.</returns>
    private static bool CheckIfItemClicked(Rect itemBounds, int clickX, int clickY)
        => CheckIfClicked(itemBounds, clickX, clickY, 1);

    /// <summary>
    /// Checks if a given rectangle was clicked by comparing the click coordinates with the rectangle's position and dimensions.
    /// </summary>
    /// <param name="bounds">The rectangle to check.</param>
    /// <param name="clickX">The X-coordinate of the mouse click.</param>
    /// <param name="clickY">The Y-coordinate of the mouse click.</param>
    /// <param name="offset">The padding to add to the rectangle's boundaries.</param>
    /// <returns>True if the rectangle was clicked; otherwise, false.</returns>
    private static bool CheckIfClicked(Rect bounds, int clickX, int clickY, double offset = 0)
        => clickX > bounds.Left - offset &&
           clickX < bounds.Right + offset &&
           clickY > bounds.Top - offset &&
           clickY < bounds.Bottom + offset;

    /// <summary>
    /// Checks if a given element was clicked by comparing the click coordinates with the element's screen position and dimensions.
    /// </summary>
    /// <param name="element">The framework element to check.</param>
    /// <param name="clickX">The X-coordinate of the mouse click.</param>
    /// <param name="clickY">The Y-coordinate of the mouse click.</param>
    /// <param name="offset">The padding to add to the element's boundaries.</param>
    /// <param name="horizontalOffset">The horizontal offset to add to the element's boundaries.</param>
    /// <param name="verticalOffset">The vertical offset to add to the element's boundaries.</param>
    /// <returns>True if the element was clicked; otherwise, false.</returns>
    private static bool CheckIfClicked(FrameworkElement element, int clickX, int clickY, double offset = 0, double horizontalOffset = 0, double verticalOffset = 0)
    {
        if (element == null)
        {
            return false;
        }

        var elementPosition = VisualUtilities.GetScreenCoordinates(element);

        // Define the element's boundaries using integer comparisons
        var leftBoundary = (int)(elementPosition.X - offset - horizontalOffset);
        var topBoundary = (int)elementPosition.Y - verticalOffset;
        var rightBoundary = (int)(elementPosition.X + element.ActualWidth + offset + horizontalOffset);
        var bottomBoundary = (int)(elementPosition.Y + element.ActualHeight + verticalOffset);

        // Determine if the click coordinates are within the element's boundaries
        var isClicked =
            clickX > leftBoundary &&
            clickX < rightBoundary &&
            clickY > topBoundary &&
            clickY < bottomBoundary;

        return isClicked;
    }
}