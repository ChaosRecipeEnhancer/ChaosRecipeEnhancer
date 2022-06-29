using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChaosRecipeEnhancer.App.Native;
using ChaosRecipeEnhancer.UI.UserControls;
using ChaosRecipeEnhancer.UI.View;

namespace ChaosRecipeEnhancer.UI.Model
{
    public class Coordinates
    {
        #region Fields

        private MouseManager _mouseManager;
        private int _x;
        private int _y;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Coordinates"/> class.
        /// </summary>
        /// <param name="mouseManager"></param>
        public Coordinates(MouseManager mouseManager)
        {
            _mouseManager = mouseManager;
        }

        #endregion

        private bool CheckForHit(Point point, FrameworkElement frameworkElement)
        {
            var clickX = _mouseManager.X;
            var clickY = _mouseManager.Y;

            // adjust btn x,y position a bit
            point.X -= 1;
            point.Y -= 1;

            // +1 border thickness
            var btnX = Convert.ToInt32(Math.Ceiling(point.X + frameworkElement.ActualWidth + 1));
            var btnY = Convert.ToInt32(Math.Ceiling(point.Y + frameworkElement.ActualHeight + 1));

            return clickX > point.X
                   && clickY > point.Y
                   && clickX < btnX
                   && clickY < btnY;
        }

        private static Point GetCoordinates(Visual visual)
        {
            if (visual == null) return new Point(0, 0);

            var locationFromScreen = visual.PointToScreen(new Point(0, 0));
            return locationFromScreen;
        }

        /// <summary>
        /// Checks if the <see cref="StashTabOverlayWindow"/> header was clicked (to move it)
        /// </summary>
        /// <param name="stashTab"></param>
        /// <returns></returns>
        private bool CheckForHeaderHit(StashTab stashTab)
        {
            // can be null if user closes overlay while fetching with stash tab overlay open
            if (stashTab.TabHeader == null) return false;

            var clickX = _mouseManager.X;
            var clickY = _mouseManager.Y;

            var point = GetTabHeaderCoordinates(stashTab.TabHeader);

            // adjust btn x,y position a bit
            point.X -= 1;
            point.Y -= 1;

            var tabX = Convert.ToInt32(Math.Floor(point.X + stashTab.TabHeader.ActualWidth + 1));
            var tabY = Convert.ToInt32(Math.Floor(point.Y + stashTab.TabHeader.ActualHeight + 1));

            return clickX > point.X
                   && clickY > point.Y
                   && clickX < tabX
                   && clickY < tabY;
        }

        private bool CheckForEditButtonHit(FrameworkElement frameworkElement)
        {
            var clickX = _mouseManager.X;
            var clickY = _mouseManager.Y;

            var pt = GetEditButtonCoordinates(frameworkElement);

            // adjust btn x,y position a bit
            pt.X -= 1;
            pt.Y -= 1;

            var btnX = Convert.ToInt32(Math.Floor(pt.X + frameworkElement.ActualWidth + 1));
            var btnY = Convert.ToInt32(Math.Floor(pt.Y + frameworkElement.ActualHeight + 1));

            return clickX > pt.X
                   && clickY > pt.Y
                   && clickX < btnX
                   && clickY < btnY;
        }

        private static Point GetTabHeaderCoordinates(Visual visual)
        {
            if (visual == null) return new Point(0, 0);

            var locationFromScreen = visual.PointToScreen(new Point(0, 0));
            return locationFromScreen;
        }

        private static Point GetEditButtonCoordinates(Visual visual)
        {
            if (visual == null) return new Point(0, 0);

            var locationFromScreen = visual.PointToScreen(new Point(0, 0));
            return locationFromScreen;
        }

        private static List<Cell> GetAllActiveCells(int index)
        {
            return StashTabList
                .StashTabs[index]
                .OverlayCellsList
                .Where(cell => cell.Active)
                .ToList();
        }

        public void OverlayClickEvent(StashTabOverlayWindow stashTabOverlayWindow)
        {
            if (stashTabOverlayWindow.IsOpen)
            {
                var selectedIndex = stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex;
                var isHit = false;
                var hitIndex = -1;

                var activeCells = GetAllActiveCells(selectedIndex);

                var buttonList = new List<ButtonAndCell>();

                if (CheckForEditButtonHit(stashTabOverlayWindow.EditModeButton))
                    stashTabOverlayWindow.HandleEditButton(stashTabOverlayWindow);

                if (StashTabList.StashTabs[selectedIndex].Quad)
                {
                    var ctrl = stashTabOverlayWindow.StashTabOverlayTabControl
                        .SelectedContent as DynamicGridControlQuad;

                    foreach (var cell in activeCells)
                        buttonList.Add(new ButtonAndCell
                        {
                            Button = ctrl.GetButtonFromCell(cell),
                            Cell = cell
                        });

                    for (var b = 0; b < buttonList.Count; b++)
                        if (CheckForHit(GetCoordinates(buttonList[b].Button), buttonList[b].Button))
                        {
                            isHit = true;
                            hitIndex = b;
                        }

                    if (isHit) Data.ActivateNextCell(true, buttonList[hitIndex].Cell);

                    for (var stash = 0; stash < StashTabList.StashTabs.Count; stash++)
                        if (CheckForHeaderHit(StashTabList.StashTabs[stash]))
                            stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex = stash;
                }
                else
                {
                    var ctrl = stashTabOverlayWindow.StashTabOverlayTabControl.SelectedContent as DynamicGridControl;

                    foreach (var cell in activeCells)
                    {
                        buttonList.Add(new ButtonAndCell
                        {
                            Button = ctrl.GetButtonFromCell(cell),
                            Cell = cell
                        });
                    }

                    for (var b = 0; b < buttonList.Count; b++)
                    {
                        if (CheckForHit(GetCoordinates(buttonList[b].Button), buttonList[b].Button))
                        {
                            isHit = true;
                            hitIndex = b;
                        }
                    }

                    if (isHit) Data.ActivateNextCell(true, buttonList[hitIndex].Cell);

                    for (var stash = 0; stash < StashTabList.StashTabs.Count; stash++)
                        if (CheckForHeaderHit(StashTabList.StashTabs[stash]))
                            stashTabOverlayWindow.StashTabOverlayTabControl.SelectedIndex = stash;
                }
            }
        }
    }

    internal class ButtonAndCell
    {
        public Button Button { get; set; }
        public Cell Cell { get; set; }
    }
}