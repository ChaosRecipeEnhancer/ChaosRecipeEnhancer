using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Extensions.Native;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.UserControls.StashTabOverlayDisplays;
using ChaosRecipeEnhancer.UI.View;

namespace ChaosRecipeEnhancer.UI.DynamicControls
{
    public static class Coordinates
    {
        private static bool CheckIfClicked(Point point, FrameworkElement button)
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

        private static Point GetCoordinates(Visual item)
        {
            if (item == null) return new Point(0, 0);
            
            var locationFromScreen = item.PointToScreen(new Point(0, 0));
            return locationFromScreen;
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

        private static bool CheckIfEditButtonClicked(FrameworkElement editButton)
        {
            var clickX = NativeMouseExtensions.ClickLocationX;
            var clickY = NativeMouseExtensions.ClickLocationY;

            var pt = GetEditButtonCoordinates(editButton);

            // adjust btn x,y position a bit
            pt.X -= 1;
            pt.Y -= 1;

            var btnX = Convert.ToInt32(Math.Floor(pt.X + editButton.ActualWidth + 1));
            var btnY = Convert.ToInt32(Math.Floor(pt.Y + editButton.ActualHeight + 1));

            return clickX > pt.X
                   && clickY > pt.Y
                   && clickX < btnX
                   && clickY < btnY;
        }

        private static Point GetTabNameContainerCoordinates(Visual tabNameContainer)
        {
            if (tabNameContainer == null) return new Point(0, 0);
            
            var locationFromScreen = tabNameContainer.PointToScreen(new Point(0, 0));
            return locationFromScreen;
        }

        private static Point GetEditButtonCoordinates(Visual button)
        {
            if (button == null) return new Point(0, 0);
            
            var locationFromScreen = button.PointToScreen(new Point(0, 0));
            return locationFromScreen;

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

        public static void OverlayClickEvent(StashTabOverlayView stashTabOverlayView)
        {
            if (StashTabControlManager.StashTabControls.Count == 0)
            {
                stashTabOverlayView.Hide();
            }
            
            if (stashTabOverlayView.IsOpen)
            {
                var isHit = false;
                var hitIndex = -1;
                var buttonList = new List<ButtonAndCell>();
                var selectedIndex = stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex;
                var activeCells = GetAllActiveCells(selectedIndex);

                if (CheckIfEditButtonClicked(stashTabOverlayView.EditModeButton))
                {
                    stashTabOverlayView.HandleEditButton(stashTabOverlayView);
                }

                if (StashTabControlManager.StashTabControls[selectedIndex].Quad)
                {
                    var control = stashTabOverlayView.StashTabOverlayTabControl.SelectedContent as QuadStashGrid;

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
                        if (CheckIfClicked(GetCoordinates(buttonList[b].Button), buttonList[b].Button))
                        {
                            isHit = true;
                            hitIndex = b;
                        }
                    }

                    Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Quad Tab Current Tab Index: {stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex}");

                    if (isHit)
                        Data.ActivateNextCell(true, buttonList[hitIndex].InteractiveStashTabCell, stashTabOverlayView.StashTabOverlayTabControl);

                    for (var stash = 0; stash < StashTabControlManager.StashTabControls.Count; stash++)
                    {
                        if (CheckIfTabNameContainerClicked(StashTabControlManager.StashTabControls[stash]))
                        {
                            stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex = stash;
                        }
                    }
                }
                else
                {
                    var control = stashTabOverlayView.StashTabOverlayTabControl.SelectedContent as NormalStashGrid;

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
                        if (CheckIfClicked(GetCoordinates(buttonList[b].Button), buttonList[b].Button))
                        {
                            isHit = true;
                            hitIndex = b;
                        }
                    }

                    Trace.WriteLine($"[Coordinates:OverlayClickEvent()]: Normal Tab Current Tab Index: {stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex}");

                    if (isHit)
                        Data.ActivateNextCell(true, buttonList[hitIndex].InteractiveStashTabCell, stashTabOverlayView.StashTabOverlayTabControl);

                    for (var stash = 0; stash < StashTabControlManager.StashTabControls.Count; stash++)
                    {
                        if (CheckIfTabNameContainerClicked(StashTabControlManager.StashTabControls[stash]))
                        {
                            stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex = stash;
                        }
                    }
                }
            }
        }
    }

    internal class ButtonAndCell
    {
        public Button Button { get; set; }
        public InteractiveStashTabCell InteractiveStashTabCell { get; set; }
    }
}