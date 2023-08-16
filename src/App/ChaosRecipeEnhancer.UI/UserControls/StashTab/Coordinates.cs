using ChaosRecipeEnhancer.UI.Extensions.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;
using ChaosRecipeEnhancer.UI.Windows;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTab;

public static class Coordinates
{
    private static bool CheckIfItemClicked(Point point, FrameworkElement button)
    {
        var clickX = NativeMouseExtensions.ClickLocationX;
        var clickY = NativeMouseExtensions.ClickLocationY;

        // adjust btn x,y position a bit
        point.X -= 1;
        point.Y -= 1;

        // +1 border thickness
        var btnX = Convert.ToInt32(Math.Ceiling(point.X + button.ActualWidth + 1));
        var btnY = Convert.ToInt32(Math.Ceiling(point.Y + button.ActualHeight + 1));

        return clickX > point.X
               && clickY > point.Y
               && clickX < btnX
               && clickY < btnY;
    }

    private static Point GetItemCoordinates(Visual item)
    {
        return item is null
            ? new Point(0, 0)
            : item.PointToScreen(new Point(0, 0));
    }

    private static bool CheckIfTabNameContainerClicked(StashTabControl stashTabControl)
    {
        var clickX = NativeMouseExtensions.ClickLocationX;
        var clickY = NativeMouseExtensions.ClickLocationY;

        var pt = GetTabNameContainerCoordinates(stashTabControl.TabNameContainer);

        // adjust btn x,y position a bit
        pt.X -= 1;
        pt.Y -= 1;

        // can be null if user closes overlay while fetching with stash tab overlay open
        if (stashTabControl.TabNameContainer == null) return false;

        var tabX = Convert.ToInt32(Math.Floor(pt.X + stashTabControl.TabNameContainer.ActualWidth + 1));
        var tabY = Convert.ToInt32(Math.Floor(pt.Y + stashTabControl.TabNameContainer.ActualHeight + 1));

        return clickX > pt.X
               && clickY > pt.Y
               && clickX < tabX
               && clickY < tabY;
    }

    private static Point GetTabNameContainerCoordinates(Visual tabNameContainer)
    {
        return tabNameContainer is null
            ? new Point(0, 0)
            : tabNameContainer.PointToScreen(new Point(0, 0));
    }

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

        return activeCells;
    }

    public static void OverlayClickEvent(StashTabOverlayWindow stashTabOverlayWindow)
    {
        // if no tabs are selected, hide overlay
        if (StashTabControlManager.StashTabControls.Count == 0)
        {
            stashTabOverlayWindow.Hide();
        }

        // if overlay is open
        if (stashTabOverlayWindow.IsOpen)
        {
            var isHit = false;
            var hitIndex = -1;
            var buttonList = new List<ButtonAndCell>();
            var selectedIndex = stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex;
            var activeCells = GetAllActiveCells(selectedIndex);

            // if currently selected tab is a quad tab
            if (StashTabControlManager.StashTabControls[selectedIndex].Quad)
            {
                Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Quad Tab Selected - Current Tab Index: {stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex}");

                // force us to view grid as normal tab grid
                var control = stashTabOverlayWindow.StashTabOverlayTabControl.SelectedContent as QuadStashGrid;

                // if the control is null, we will populate it with new cells
                if (control != null)
                {
                    foreach (var cell in activeCells)
                    {
                        buttonList.Add(new ButtonAndCell
                        {
                            Button = control.GetButtonFromCell(cell),
                            InteractiveStashTabCell = cell
                        });
                    }
                }

                // iterate through every cell to see if an item associated with that cell was clicked
                for (var b = 0; b < buttonList.Count; b++)
                {
                    if (CheckIfItemClicked(GetItemCoordinates(buttonList[b].Button), buttonList[b].Button))
                    {
                        Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Successful Hit: {buttonList[hitIndex].InteractiveStashTabCell}");

                        isHit = true;
                        hitIndex = b;
                    }
                }

                // if we hit an item, activate the next item (and its associated cells)
                if (isHit)
                {
                    stashTabOverlayWindow.ActivateNextCell(true, buttonList[hitIndex].InteractiveStashTabCell, stashTabOverlayWindow.StashTabOverlayTabControl);
                    Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Activating next Item: {buttonList[hitIndex].InteractiveStashTabCell}");

                }

                for (var stash = 0; stash < StashTabControlManager.StashTabControls.Count; stash++)
                {
                    if (CheckIfTabNameContainerClicked(StashTabControlManager.StashTabControls[stash]))
                    {
                        stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex = stash;
                    }
                }
            }
            // if currently selected tab is a normal tab
            else
            {
                Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Quad Tab Selected - Current Tab Index: {stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex}");

                // force us to view grid as normal tab grid
                var control = stashTabOverlayWindow.StashTabOverlayTabControl.SelectedContent as NormalStashGrid;

                // simple null check on quad stash grid control
                if (control != null)
                {
                    foreach (var cell in activeCells)
                    {
                        buttonList.Add(new ButtonAndCell
                        {
                            Button = control.GetButtonFromCell(cell),
                            InteractiveStashTabCell = cell
                        });
                    }
                }

                for (var b = 0; b < buttonList.Count; b++)
                {
                    if (CheckIfItemClicked(GetItemCoordinates(buttonList[b].Button), buttonList[b].Button))
                    {
                        isHit = true;
                        hitIndex = b;
                    }
                }

                Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Normal Tab Current Tab Index: {stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex}");

                if (isHit)
                {
                    stashTabOverlayWindow.ActivateNextCell(true, buttonList[hitIndex].InteractiveStashTabCell, stashTabOverlayWindow.StashTabOverlayTabControl);
                }

                for (var stash = 0; stash < StashTabControlManager.StashTabControls.Count; stash++)
                {
                    if (CheckIfTabNameContainerClicked(StashTabControlManager.StashTabControls[stash]))
                    {
                        stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex = stash;
                    }
                }
            }
        }
    }
}

internal class ButtonAndCell
{
    // actual wpf button we click
    public Button Button { get; init; }

    // represents our item in the stash tab grid
    public InteractiveStashTabCell InteractiveStashTabCell { get; init; }
}