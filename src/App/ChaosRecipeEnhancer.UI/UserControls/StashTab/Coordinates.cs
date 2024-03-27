using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Utilities.Native;
using ChaosRecipeEnhancer.UI.Windows;
using Serilog;
using System;
using System.Collections.Generic;
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
    public static void OverlayClickEvent(StashTabOverlayWindow stashTabOverlayWindow)
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

        var isHit = false;
        var hitIndex = -1;
        var buttonList = new List<ButtonAndCell>();
        var selectedIndex = stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex;
        var activeCells = GetAllActiveCells(selectedIndex);

        if (CheckIfEditButtonClicked(stashTabOverlayWindow.EditModeButton))
        {
            stashTabOverlayWindow.HandleEditButton();
            Log.Information("OverlayClickEvent: Edit button click handled.");
        }

        dynamic control = stashTabOverlayWindow.StashTabOverlayTabControl.SelectedContent;

        if (control != null)
        {
            foreach (var cell in activeCells)
            {
                var button = control.GetButtonFromCell(cell);
                if (button != null)
                {
                    buttonList.Add(new ButtonAndCell
                    {
                        Button = button,
                        InteractiveStashTabCell = cell
                    });
                }
            }
        }

        foreach (var buttonCell in buttonList)
        {
            if (CheckIfItemClicked(buttonCell.Button))
            {
                Log.Information("OverlayClickEvent: Successful item hit detected.");
                isHit = true;
                hitIndex = buttonList.IndexOf(buttonCell);
                break;
            }
        }

        if (isHit)
        {
            stashTabOverlayWindow.ActivateNextCell(true, buttonList[hitIndex].InteractiveStashTabCell);
            Log.Information($"OverlayClickEvent: Activating next item at: {buttonList[hitIndex].InteractiveStashTabCell.ItemModel.X}, {buttonList[hitIndex].InteractiveStashTabCell.ItemModel.Y}.");
        }

        for (var stash = 0; stash < StashTabControlManager.StashTabControls.Count; stash++)
        {
            if (CheckIfTabNameContainerClicked(StashTabControlManager.StashTabControls[stash]))
            {
                stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex = stash;
                Log.Information($"OverlayClickEvent: Tab name container clicked. Switched to tab at index {stash}.");
                break;
            }
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
    /// Determines if a tab name container was clicked based on the mouse click coordinates.
    /// </summary>
    /// <param name="stashTabControl">The stash tab control.</param>
    /// <returns>True if the tab name container was clicked; otherwise, false.</returns>
    private static bool CheckIfTabNameContainerClicked(StashTabControl stashTabControl)
        => CheckIfClicked(stashTabControl.NameContainer);

    /// <summary>
    /// Determines if the edit button was clicked based on the mouse click coordinates.
    /// </summary>
    /// <param name="editButton">The edit button.</param>
    /// <returns>True if the edit button was clicked; otherwise, false.</returns>
    private static bool CheckIfEditButtonClicked(FrameworkElement editButton)
        => CheckIfClicked(editButton);

    /// <summary>
    /// Determines if an item represented by a button was clicked based on the mouse click coordinates.
    /// </summary>
    /// <param name="button">The button representing an item in the stash tab.</param>
    /// <returns>True if the item was clicked; otherwise, false.</returns>
    private static bool CheckIfItemClicked(FrameworkElement button)
        => CheckIfClicked(button);

    /// <summary>
    /// Checks if a given element was clicked by comparing the click coordinates with the element's screen position and dimensions.
    /// </summary>
    /// <param name="element">The framework element to check.</param>
    /// <param name="elementName">The name of the element for logging.</param>
    /// <returns>True if the element was clicked; otherwise, false.</returns>
    private static bool CheckIfClicked(FrameworkElement element)
    {
        if (element == null)
        {
            return false;
        }

        var clickX = MouseHookForGeneralInteractionInStashTabOverlay.ClickLocationX;
        var clickY = MouseHookForGeneralInteractionInStashTabOverlay.ClickLocationY;
        var elementPosition = VisualUtilities.GetScreenCoordinates(element);

        // Define the element's boundaries
        var leftBoundary = Convert.ToInt32(Math.Floor(elementPosition.X));
        var topBoundary = Convert.ToInt32(Math.Floor(elementPosition.Y));
        var rightBoundary = Convert.ToInt32(Math.Floor(elementPosition.X + element.ActualWidth));
        var bottomBoundary = Convert.ToInt32(Math.Floor(elementPosition.Y + element.ActualHeight));

        // Determine if the click coordinates are within the element's boundaries (like a rectangular hitbox)
        var isClicked =
            clickX > leftBoundary &&
            clickY > topBoundary &&
            clickX < rightBoundary &&
            clickY < bottomBoundary;

        return isClicked;
    }
}