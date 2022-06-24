using System;
using System.Collections.Generic;
using System.Windows;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Model
{
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
                {
                    if (int.TryParse(s.Trim(), out var parsedIndex))
                    {
                        if (!ret.Contains(parsedIndex)) ret.Add(parsedIndex);
                    }
                    else
                    {
                        MessageBox.Show("Stashtab Index has to be a number!", "Stashtab Error", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }

                if (ret.Count == 0)
                    MessageBox.Show("Stashtab Indices empty!", "Stashtab Error", MessageBoxButton.OK,
                        MessageBoxImage.Error);

                StashTabIndices = ret;
            }
            else
            {
                MessageBox.Show("Stashtab Indices empty!", "Stashtab Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}