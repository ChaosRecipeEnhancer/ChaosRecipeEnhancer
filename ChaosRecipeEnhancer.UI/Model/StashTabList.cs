using System;
using System.Collections.Generic;
using System.Windows;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Model
{
    /// <summary>
    ///     UI representation for a collection of stash tabs within our app (NOT the GGG StashTab object model).
    /// </summary>
    internal static class StashTabList
    {
        public static List<StashTab> StashTabs { get; set; } = new List<StashTab>();
        public static List<int> StashTabIndices { get; set; }

        public static void GetStashTabIndices()
        {
            if (Settings.Default.StashTabIndices != "")
            {
                var ret = new List<int>();
                var indices = Settings.Default.StashTabIndices;
                string[] sep = { "," };
                var split = indices.Split(sep, StringSplitOptions.None);

                foreach (var s in split)
                    if (int.TryParse(s.Trim(), out var parsedIndex))
                    {
                        if (!ret.Contains(parsedIndex)) ret.Add(parsedIndex);
                    }
                    else
                    {
                        MessageBox.Show("Stashtab Index has to be a number!", "Error: Stash Tab Overlay", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }

                if (ret.Count == 0)
                    MessageBox.Show("Stashtab Indices empty!", "Error: Stash Tab Overlay", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                StashTabIndices = ret;
            }
            else
            {
                MessageBox.Show("No valid Stash Tab indices could be found in the user settings.", "Error: Stash Tab Overlay", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}