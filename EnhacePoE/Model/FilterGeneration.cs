using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EnhancePoE.Model
{
    public static class FilterGeneration
    {

        public static List<string> CustomStyle { get; set; } = new List<string>();

        public static string OpenLootfilter()
        {
            string location = Properties.Settings.Default.LootfilterLocation;
            if(location != "")
            {
                using(StreamReader reader = new StreamReader(location))
                {
                    string ret = reader.ReadToEnd();
                    //Trace.Write(ret);
                    reader.Close();
                    return ret;

                    //Trace.Write(reader.ReadToEnd());
                }
            }
            return null;
        }

        public static void WriteLootfilter(string filter)
        {
            string location = Properties.Settings.Default.LootfilterLocation;
            if(location != "" && filter != "")
            {
                using(StreamWriter writer = new StreamWriter(location))
                {
                    writer.Write(filter);
                    writer.Close();
                }
            }
        }

        public static void LoadCustomStyle()
        {
            string pathNormalItemsStyle = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Styles\NormalItemsStyle.txt");
            string[] boots = File.ReadAllLines(pathNormalItemsStyle);
            foreach (string line in boots)
            {
                if (line == "") { continue; }
                if (line.Contains("#")) { continue; }
                CustomStyle.Add(line.Trim());
            }
        }

        public static string GenerateSection(bool show, List<string>bases, string itemClass)
        {
            string result = "";
            if (show)
            {
                result += "Show";
            }
            else
            {
                result += "Hide";
            }
            string nl = "\n";
            string tab = "\t";
            result = result + nl + tab + "ItemLevel >= 60" + nl + tab + "ItemLevel <= 74" + nl + tab + "Rarity Rare" + nl + tab + "Identified False" + nl + tab;
            
            string baseType = "BaseType ";
            foreach(string b in bases)
            {
                baseType = baseType + "\"" + b + "\" ";
            }

            result = result + baseType + nl + tab;

            string bgColor = "SetBackgroundColor";

            List<int> colors = GetRGB(itemClass);
            for(int i = 0; i < colors.Count; i++)
            {
                bgColor = bgColor + " " + colors[i].ToString();
            }

            result = result + bgColor + nl + tab;

            foreach(string cs in CustomStyle)
            {
                result = result + cs + nl + tab;
            }

            result = result + nl + nl;

            return result;
        }

        public static List<int> GetRGB(string type)
        {
            //Trace.WriteLine(type);
            int r;
            int g;
            int b;
            int a;
            string color = "";
            List<int> colorList = new List<int>();
            if(type == "ring") 
            {
                color = Properties.Settings.Default.ColorJewellery;
            }
            if(type == "amulet") 
            { 
                color = Properties.Settings.Default.ColorJewellery;
            }
            if (type == "belt") 
            {
                color = Properties.Settings.Default.ColorBelt;
            }
            if(type == "helmet")
            {
                color = Properties.Settings.Default.ColorHelmet;
            }
            if(type == "weapon") 
            {
                color = Properties.Settings.Default.ColorWeapon;
            }
            if(type == "gloves") 
            {
                color = Properties.Settings.Default.ColorGloves;
            }
            if(type == "boots")
            {
                color = Properties.Settings.Default.ColorBoots;
            }
            if(type == "chest") 
            {
                color = Properties.Settings.Default.ColorChest;
            }
            if(color != "")
            {
                a = Convert.ToByte(color.Substring(1, 2), 16);
                r = Convert.ToByte(color.Substring(3, 2), 16);
                g = Convert.ToByte(color.Substring(5, 2), 16);
                b = Convert.ToByte(color.Substring(7, 2), 16);
            }
            else
            {
                a = 255;
                r = 255;
                g = 0;
                b = 0;
            }
            colorList.Add(r);
            colorList.Add(g);
            colorList.Add(b);
            colorList.Add(a);
            return colorList;
        }

        public static string GenerateLootFilter(string oldFilter, List<string> sections)
        {
            string nl = " \n";
            string result = "#Enhance PoE Lootfilter Start \n";
            string end = "#Enhance PoE Lootfilter End \n";

            string[] seperator = { end };
            string[] split = oldFilter.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string s in sections)
            {
                result += s + nl;
            }
            result += end;

            if (split.Length > 1)
            {
                result += split[1];
            }
            else
            {
                result += oldFilter;
            }

            return result;
        }

    }
}
