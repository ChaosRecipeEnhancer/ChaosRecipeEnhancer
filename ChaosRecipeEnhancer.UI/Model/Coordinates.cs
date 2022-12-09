using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ChaosRecipeEnhancer.UI.UserControls.StashTabOverlayDisplays;
using ChaosRecipeEnhancer.UI.View;

namespace ChaosRecipeEnhancer.UI.Model
{
    public static class Coordinates
    {
        private static bool CheckIfClicked(Point point, Button button)
        {
            var clickX = MouseHook.ClickLocationX;
            var clickY = MouseHook.ClickLocationY;

            // adjust btn x,y position a bit
            point.X -= 1;
            point.Y -= 1;

            // +1 border thickness
            var btnX = Convert.ToInt32(Math.Ceiling(point.X + button.ActualWidth + 1));
            var btnY = Convert.ToInt32(Math.Ceiling(point.Y + button.ActualHeight + 1));

            if (clickX > point.X
                && clickY > point.Y
                && clickX < btnX
                && clickY < btnY)
                return true;

            return false;
        }

        private static Point GetCoordinates(Button item)
        {
            if (item != null)
            {
                var locationFromScreen = item.PointToScreen(new Point(0, 0));
                return locationFromScreen;
            }

            return new Point(0, 0);
        }

        private static bool CheckIfTabNameContainerClicked(StashTab stashTab)
        {
            var clickX = MouseHook.ClickLocationX;
            var clickY = MouseHook.ClickLocationY;

            var pt = GetTabNameContainerCoordinates(stashTab.TabNameContainer);

            // adjust btn x,y position a bit
            pt.X -= 1;
            pt.Y -= 1;

            // can be null if user closes overlay while fetching with stash tab overlay open
            if (stashTab.TabNameContainer != null)
            {
                var tabX = Convert.ToInt32(Math.Floor(pt.X + stashTab.TabNameContainer.ActualWidth + 1));
                var tabY = Convert.ToInt32(Math.Floor(pt.Y + stashTab.TabNameContainer.ActualHeight + 1));

                if (clickX > pt.X
                    && clickY > pt.Y
                    && clickX < tabX
                    && clickY < tabY)
                    return true;
            }

            return false;
        }

        private static bool CheckIfEditButtonClicked(Button editButton)
        {
            var clickX = MouseHook.ClickLocationX;
            var clickY = MouseHook.ClickLocationY;

            var pt = GetEditButtonCoordinates(editButton);

            // adjust btn x,y position a bit
            pt.X -= 1;
            pt.Y -= 1;

            var btnX = Convert.ToInt32(Math.Floor(pt.X + editButton.ActualWidth + 1));
            var btnY = Convert.ToInt32(Math.Floor(pt.Y + editButton.ActualHeight + 1));

            if (clickX > pt.X
                && clickY > pt.Y
                && clickX < btnX
                && clickY < btnY)
                return true;

            return false;
        }

        private static Point GetTabNameContainerCoordinates(TextBlock tabNameContainer)
        {
            if (tabNameContainer != null)
            {
                var locationFromScreen = tabNameContainer.PointToScreen(new Point(0, 0));
                return locationFromScreen;
            }

            return new Point(0, 0);
        }

        private static Point GetEditButtonCoordinates(Button button)
        {
            if (button != null)
            {
                var locationFromScreen = button.PointToScreen(new Point(0, 0));
                return locationFromScreen;
            }

            return new Point(0, 0);
        }

        private static List<InteractiveCell> GetAllActiveCells(int index)
        {
            var activeCells = new List<InteractiveCell>();

            foreach (var cell in StashTabList.StashTabs[index].OverlayCellsList)
                if (cell.Active)
                    activeCells.Add(cell);

            return activeCells;
        }

        public static void OverlayClickEvent(StashTabOverlayView stashTabOverlayView)
        {
            if (StashTabList.StashTabs.Count == 0)
            {
                stashTabOverlayView.Hide();
            }
            
            if (stashTabOverlayView.IsOpen)
            {
                var selectedIndex = stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex;
                var isHit = false;
                var hitIndex = -1;

                var activeCells = GetAllActiveCells(selectedIndex);

                var buttonList = new List<ButtonAndCell>();

                if (CheckIfEditButtonClicked(stashTabOverlayView.EditModeButton))
                    stashTabOverlayView.HandleEditButton(stashTabOverlayView);

                if (StashTabList.StashTabs[selectedIndex].Quad)
                {
                    var control = stashTabOverlayView.StashTabOverlayTabControl.SelectedContent as QuadStashGrid;

                    if (control != null)
                    {
                        foreach (var cell in activeCells)
                            buttonList.Add(new ButtonAndCell
                            {
                                Button = control.GetButtonFromCell(cell),
                                InteractiveCell = cell
                            });
                    }

                    for (var b = 0; b < buttonList.Count; b++)
                        if (CheckIfClicked(GetCoordinates(buttonList[b].Button), buttonList[b].Button))
                        {
                            isHit = true;
                            hitIndex = b;
                        }

                    Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Quad Tab Current Tab Index: {stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex}");

                    if (isHit)
                        Data.ActivateNextCell(true, buttonList[hitIndex].InteractiveCell, stashTabOverlayView.StashTabOverlayTabControl);

                    for (var stash = 0; stash < StashTabList.StashTabs.Count; stash++)
                        if (CheckIfTabNameContainerClicked(StashTabList.StashTabs[stash]))
                            stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex = stash;
                }
                else
                {
                    var control = stashTabOverlayView.StashTabOverlayTabControl.SelectedContent as NormalStashGrid;

                    if (control != null)
                    {
                        foreach (var cell in activeCells)
                            buttonList.Add(new ButtonAndCell
                            {
                                Button = control.GetButtonFromCell(cell),
                                InteractiveCell = cell
                            });
                    }

                    for (var b = 0; b < buttonList.Count; b++)
                        if (CheckIfClicked(GetCoordinates(buttonList[b].Button), buttonList[b].Button))
                        {
                            isHit = true;
                            hitIndex = b;
                        }

                    Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Normal Tab Current Tab Index: {stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex}");

                    if (isHit)
                        Data.ActivateNextCell(true, buttonList[hitIndex].InteractiveCell, stashTabOverlayView.StashTabOverlayTabControl);

                    for (var stash = 0; stash < StashTabList.StashTabs.Count; stash++)
                        if (CheckIfTabNameContainerClicked(StashTabList.StashTabs[stash]))
                            stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex = stash;
                }
            }
        }
    }

    internal class ButtonAndCell
    {
        public Button Button { get; set; }
        public InteractiveCell InteractiveCell { get; set; }
    }
}