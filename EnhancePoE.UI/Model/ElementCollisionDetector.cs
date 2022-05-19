using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EnhancePoE.UI.UserControls;
using EnhancePoE.UI.View;

namespace EnhancePoE.UI.Model
{
    public static class ElementCollisionDetector
    {
        private static bool CheckForHit(FrameworkElement element)
        {
            if (element == null)
                return false;

            var loc = element.PointToScreen(new Point(0, 0));
            loc.Offset(-1, -1);

            return MouseHook.IsInside(new Rect(loc.X, loc.Y, element.ActualWidth + 1, element.ActualHeight + 1));
        }

        private static List<Cell> GetAllActiveCells(int index) =>
            StashTabList.StashTabs[index].OverlayCellsList.Where(x => x.Active).ToList();

        private static IDynamicGridControl GetGridControl(StashTab tab, object content)
        {
            if (tab.Quad)
                return content as DynamicGridControlQuad;

            return content as DynamicGridControl;
        }

        public static void OverlayClickEvent(StashTabOverlayView stashTabOverlayView)
        {
            if (!stashTabOverlayView.IsOpen)
            {
                return;
            }

            var selectedIndex = stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex;

            var activeCells = GetAllActiveCells(selectedIndex);

            var gridControl = GetGridControl(StashTabList.StashTabs[selectedIndex], stashTabOverlayView.StashTabOverlayTabControl.SelectedContent);

            var buttonList = activeCells.Select(cell => new ButtonAndCell
            {
                Button = gridControl.GetButtonFromCell(cell),
                Cell = cell
            }).ToList();

            if (CheckForHit(stashTabOverlayView.EditModeButton)) 
                stashTabOverlayView.HandleEditButton(stashTabOverlayView);

            var hitButtons = buttonList.Where(x => CheckForHit(x.Button));

            if (hitButtons.Any()) 
                Data.ActivateNextCell(true, hitButtons.First().Cell);

            var hitHeaders = StashTabList.StashTabs.Where(stashTab => CheckForHit(stashTab.TabHeader))
                .Select((_, index) => index);

            if (hitHeaders.Any())
                stashTabOverlayView.StashTabOverlayTabControl.SelectedIndex = hitHeaders.First();
        }
    }

    internal class ButtonAndCell
    {
        public Button Button { get; set; }
        public Cell Cell { get; set; }
    }
}