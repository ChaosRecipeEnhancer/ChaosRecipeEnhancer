using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EnhancePoE.Model
{
    static class StashTabList
    {
        public static List<StashTab> StashTabs { get; set; } = null;
        public static List<int> StashTabIndices { get; set; }

        public static void GetStashTabIndices()
        {
            if(Properties.Settings.Default.StashTabIndices != "")
            {
                List<int> ret = new List<int>();
                string indices = Properties.Settings.Default.StashTabIndices;
                string[] sep = { "," };
                string[] split = indices.Split(sep, System.StringSplitOptions.None);
                foreach(string s in split)
                {
                    if (Int32.TryParse(s.Trim(), out int parsedIndex))
                    {
                        if (!ret.Contains(parsedIndex))
                        {
                            ret.Add(parsedIndex);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Stashtab Index has to be a number!", "Stashtab Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                if(ret.Count == 0) { MessageBox.Show("Stashtab Indices empty!", "Stashtab Error", MessageBoxButton.OK, MessageBoxImage.Error); }
                StashTabIndices = ret;
                //Trace.WriteLine(ret, "ret");
                //foreach(int i in StashTabIndices)
                //{
                //    Trace.WriteLine(i, "stash tab index");
                //}
            }
            else
            {
                MessageBox.Show("Stashtab Indices empty!", "Stashtab Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
