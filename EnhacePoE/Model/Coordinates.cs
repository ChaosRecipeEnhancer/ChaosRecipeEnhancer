using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EnhancePoE.Model
{
    public static class Coordinates
    {
        private static bool CheckForHit(System.Windows.Point pt, System.Windows.Controls.Button btn)
        {
            int clickX = MouseHook.ClickLocationX;
            int clickY = MouseHook.ClickLocationY;

            //Trace.WriteLine);


            // adjust btn x,y position a bit
            pt.X -= 1;
            pt.Y -= 1;

            // +1 border thickness
            int btnX = Convert.ToInt32(Math.Ceiling(pt.X + btn.ActualWidth + 1));
            int btnY = Convert.ToInt32(Math.Ceiling(pt.Y + btn.ActualHeight + 1));

            if (clickX > pt.X
                && clickY > pt.Y
                && clickX < btnX
                && clickY < btnY)
            {
                return true;
            }
            return false;
        }

        private static System.Windows.Point GetCoordinates(System.Windows.Controls.Button item)
        {
            if (item != null)
            {
                System.Windows.Point locationFromScreen = item.PointToScreen(new System.Windows.Point(0, 0));
                return locationFromScreen;
            }
            return new System.Windows.Point(0, 0);
        }

        private static bool CheckForHeaderHit(StashTab s)
        {
            int clickX = MouseHook.ClickLocationX;
            int clickY = MouseHook.ClickLocationY;

            System.Windows.Point pt = GetTabHeaderCoordinates(s.TabHeader);

            int tabX = Convert.ToInt32(Math.Floor(pt.X + s.TabHeader.ActualWidth));
            int tabY = Convert.ToInt32(Math.Floor(pt.Y + s.TabHeader.ActualHeight));
            if (clickX > pt.X
                && clickY > pt.Y
                && clickX < tabX
                && clickY < tabY)
            {
                return true;
            }
            return false;
        }

        private static System.Windows.Point GetTabHeaderCoordinates(System.Windows.Controls.TextBlock item)
        {
            if (item != null)
            {
                System.Windows.Point locationFromScreen = item.PointToScreen(new System.Windows.Point(0, 0));
                return locationFromScreen;
            }
            return new System.Windows.Point(0, 0);
        }

        private static List<Cell> GetAllActiveCells(int index)
        {
            List<Cell> activeCells = new List<Cell>();
            foreach (Cell cell in MainWindow.stashTabsModel.StashTabs[index].OverlayCellsList)
            {
                if (cell.Active)
                {
                    activeCells.Add(cell);
                }
            }
            return activeCells;
        }



        // mouse hook action
        public static void Event(object sender, EventArgs e) => OverlayClickEvent();




        // 
        private static void OverlayClickEvent()
        {

            if (MainWindow.stashTabOverlay.IsOpen)
            {
                int selectedIndex = MainWindow.stashTabOverlay.StashTabOverlayTabControl.SelectedIndex;
                bool isHit = false;


                List<Cell> activeCells = GetAllActiveCells(selectedIndex);

                List<System.Windows.Controls.Button> buttonList = new List<System.Windows.Controls.Button>();

                if (MainWindow.stashTabsModel.StashTabs[selectedIndex].Quad)
                {
                    var ctrl = MainWindow.stashTabOverlay.StashTabOverlayTabControl.SelectedContent as UserControls.DynamicGridControlQuad;
                    foreach (Cell cell in activeCells)
                    {
                        buttonList.Add(ctrl.GetButtonFromCell(cell));
                    }
                    foreach (System.Windows.Controls.Button b in buttonList)
                    {
                        if (CheckForHit(GetCoordinates(b), b))
                        {
                            isHit = true;
                        }
                    }

                    if (isHit)
                    {
                        ChaosRecipeEnhancer.currentData.ActivateNextCell(true);
                    }

                    for (int stash = 0; stash < MainWindow.stashTabsModel.StashTabs.Count; stash++)
                    {
                        if (CheckForHeaderHit(MainWindow.stashTabsModel.StashTabs[stash]))
                        {
                            MainWindow.stashTabOverlay.StashTabOverlayTabControl.SelectedIndex = stash;
                        }
                    }
                }
                else
                {
                    var ctrl = MainWindow.stashTabOverlay.StashTabOverlayTabControl.SelectedContent as UserControls.DynamicGridControl;
                    foreach (Cell cell in activeCells)
                    {
                        buttonList.Add(ctrl.GetButtonFromCell(cell));
                    }
                    foreach (System.Windows.Controls.Button b in buttonList)
                    {
                        if (CheckForHit(GetCoordinates(b), b))
                        {
                            isHit = true;
                        }
                    }

                    if (isHit)
                    {
                        ChaosRecipeEnhancer.currentData.ActivateNextCell(true);
                    }
                    for (int stash = 0; stash < MainWindow.stashTabsModel.StashTabs.Count; stash++)
                    {
                        if (CheckForHeaderHit(MainWindow.stashTabsModel.StashTabs[stash]))
                        {
                            MainWindow.stashTabOverlay.StashTabOverlayTabControl.SelectedIndex = stash;
                        }
                    }
                }
            }
        }
    }
}
