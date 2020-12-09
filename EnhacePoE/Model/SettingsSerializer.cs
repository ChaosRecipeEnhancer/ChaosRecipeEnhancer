using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace EnhancePoE.Model
{
    class SettingsSerializer
    {

        public static void SaveObjectAsString(string obj, string destination)
        {
            Properties.Settings.Default[destination] = obj;
        }

        public static StringCollection SerializeStashTab(TabItemViewModel tabItemVM)
        {
            StringCollection saveString = new StringCollection();
            foreach (StashTab t in tabItemVM.StashTabs)
            {

                StashTabSaveFile test = new StashTabSaveFile();
                test.TabIndex = t.TabIndex;
                test.TabName = t.TabName;
                test.TabNumber = t.TabNumber;
                test.Quad = t.Quad;

                string jsonString;
                jsonString = JsonSerializer.Serialize(test);
                saveString.Add(jsonString);
            }

            return saveString;
        }


        public static List<StashTab> DeserializeStashTab()
        {
            StringCollection saveStringCollection = Properties.Settings.Default.StashTabsString;
            List<StashTab> allSavedTabs = new List<StashTab>();
            foreach(string save in saveStringCollection)
            {
                StashTabSaveFile settingsTab = JsonSerializer.Deserialize<StashTabSaveFile>(save);
                allSavedTabs.Add(new StashTab(name: settingsTab.TabName, quad: settingsTab.Quad, index: settingsTab.TabIndex, number: settingsTab.TabNumber));
            }
            return allSavedTabs;
            

            //StashTab s = new StashTab();
        }

        public class StashTabSaveFile
        {
            public string TabName { get; set; }
            public int TabNumber { get; set; }
            public int TabIndex { get; set; }
            public bool Quad { get; set; }
        }
    }
}
