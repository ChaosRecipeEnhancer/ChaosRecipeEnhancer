using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace EnhancePoE.Model
{
    public static class FilterGeneration
    {

        public static List<string> CustomStyle { get; set; } = new List<string>();
        public static List<string> CustomStyleInfluenced { get; set; } = new List<string>();

        public static string OpenLootfilter()
        {
            string location = Properties.Settings.Default.LootfilterLocation;
            if(location != "")
            {
                using(StreamReader reader = new StreamReader(location))
                {
                    string ret = reader.ReadToEnd();
                    reader.Close();
                    return ret;
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
            CustomStyle.Clear();
            string pathNormalItemsStyle = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Styles\NormalItemsStyle.txt");
            string[] style = File.ReadAllLines(pathNormalItemsStyle);
            foreach (string line in style)
            {
                if (line == "") { continue; }
                if (line.Contains("#")) { continue; }
                CustomStyle.Add(line.Trim());
            }
        }

        public static void LoadCustomStyleInfluenced()
        {
            CustomStyleInfluenced.Clear();
            string pathInfluencedItemsStyle = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Styles\InfluencedItemsStyle.txt");
            string[] style = File.ReadAllLines(pathInfluencedItemsStyle);
            foreach (string line in style)
            {
                if (line == "") { continue; }
                if (line.Contains("#")) { continue; }
                CustomStyleInfluenced.Add(line.Trim());
            }
        }

        public static string GenerateSection(bool show, string itemClass, bool influenced = false, bool onlyChaos = false)
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
            if (influenced)
            {
                result += nl + tab + "HasInfluence Crusader Elder Hunter Redeemer Shaper Warlord";
            }
            else
            {
                result += nl + tab + "HasInfluence None";
            }
            result = result  + nl + tab  + "Rarity Rare" + nl + tab + "Identified False" + nl + tab;
            if (!influenced && onlyChaos)
            {
                result += "ItemLevel >= 60" + nl + tab + "ItemLevel <= 74" + nl + tab;
            }
            else
            {
                result += "ItemLevel >= 60" + nl + tab;
            }


            string baseType = "Class ";

            if(itemClass == "OneHandWeapons")
            {
                baseType += "\"Daggers\" \"One Hand Axes\" \"One Hand Maces\" \"One Hand Swords\" \"Rune Daggers\" \"Sceptres\" \"Thrusting One Hand Swords\" \"Wands\"";
                baseType += nl + tab + "Width <= 1" + nl + tab + "Height <= 3";
            }
            else if(itemClass == "TwoHandWeapons")
            {
                baseType += "\"Two Hand Swords\" \"Two Hand Axes\" \"Two Hand Maces\" \"Staves\" \"Warstaves\" \"Bows\"";
                baseType += nl + tab + "Width <= 2" + nl + tab + "Height <= 3";
            }
            else
            {
                baseType += itemClass;
            }

            result = result + baseType + nl + tab;

            string bgColor = "SetBackgroundColor";

            List<int> colors = GetRGB(itemClass);
            for(int i = 0; i < colors.Count; i++)
            {
                bgColor = bgColor + " " + colors[i].ToString();
            }

            result = result + bgColor + nl + tab;

            if (influenced)
            {
                foreach (string cs in CustomStyleInfluenced)
                {
                    result = result + cs + nl + tab;
                }
            }
            else
            {
                foreach (string cs in CustomStyle)
                {
                    result = result + cs + nl + tab;
                }
            }

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
            if(type == "Rings") 
            {
                color = Properties.Settings.Default.ColorRing;
            }
            if(type == "Amulets") 
            { 
                color = Properties.Settings.Default.ColorAmulet;
            }
            if (type == "Belts") 
            {
                color = Properties.Settings.Default.ColorBelt;
            }
            if(type == "Helmets")
            {
                color = Properties.Settings.Default.ColorHelmet;
            }
            if(type == "OneHandWeapons") 
            {
                color = Properties.Settings.Default.ColorWeapon;
            }
            if(type == "Gloves") 
            {
                color = Properties.Settings.Default.ColorGloves;
            }
            if(type == "Boots")
            {
                color = Properties.Settings.Default.ColorBoots;
            }
            if(type == "Body Armours") 
            {
                color = Properties.Settings.Default.ColorChest;
            }
            if(type == "TwoHandWeapons")
            {
                color = Properties.Settings.Default.ColorWeapon;
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


        // refactor this shit
        public static string GenerateLootFilter(string oldFilter, HashSet<string> sections)
        {
            // order has to be:
            // 1. exa start
            // 2. exa end
            // 3. chaos start
            // 4. chaos end

            string nl = "\n";
            string result;
            string chaosSection = "";
            string chaosStart = "#Chaos Recipe Enhancer by kosace Chaos Recipe Start";
            string chaosEnd = "#Chaos Recipe Enhancer by kosace Chaos Recipe End";

            //string exaltedStart = "#Chaos Recipe Enhancer by kosace Exalted Recipe Start\n";
            //string exaltedEnd = "#Chaos Recipe Enhancer by kosace Exalted Recipe End\n";

            string beforeChaos = "";
            string afterChaos = "";

            // generate chaos recipe section
            chaosSection += chaosStart + nl + nl;
            foreach (string s in sections)
            {
                chaosSection += s + nl;
            }
            chaosSection += chaosEnd + nl;

            string[] sep = { chaosEnd };
            string[] split = oldFilter.Split(sep, System.StringSplitOptions.None);

            if(split.Length > 1)
            {
                afterChaos = split[1];
                //Trace.WriteLine(split[1], "afterchaos");
                string[] sep2 = { chaosStart };
                string[] split2 = split[0].Split(sep2, System.StringSplitOptions.None);
                
                if(split2.Length > 1)
                {
                    beforeChaos = split2[0];
                    //Trace.WriteLine(split2[0], "beforechaos");
                    
                }
                else
                {
                    afterChaos = oldFilter;
                }
            }
            else
            {
                //Trace.WriteLine(split[0], "length = 1");
                afterChaos = oldFilter;
            }

            result = beforeChaos + chaosSection + afterChaos;

            return result;
        }

        public static string GenerateLootFilterInfluenced(string oldFilter, List<string> sections)
        {
            // order has to be:
            // 1. exa start
            // 2. exa end
            // 3. chaos start
            // 4. chaos end

            string nl = "\n";
            string result;
            string exaltedSection = "";
            string exaltedStart = "#Chaos Recipe Enhancer by kosace Exalted Recipe Start";
            string exaltedEnd = "#Chaos Recipe Enhancer by kosace Exalted Recipe End";

            string beforeExalted = "";
            string afterExalted = "";

            // generate chaos recipe section
            exaltedSection += exaltedStart + nl + nl;
            foreach (string s in sections)
            {
                exaltedSection += s + nl;
            }
            exaltedSection += exaltedEnd + nl;

            string[] sep = { exaltedEnd };
            string[] split = oldFilter.Split(sep, System.StringSplitOptions.None);

            if (split.Length > 1)
            {
                afterExalted = split[1];

                string[] sep2 = { exaltedStart };
                string[] split2 = split[0].Split(sep2, System.StringSplitOptions.None);

                if (split2.Length > 1)
                {
                    beforeExalted = split2[0];
                }
                else
                {
                    afterExalted = oldFilter;
                }
            }
            else
            {
                afterExalted = oldFilter;
            }

            result = beforeExalted + exaltedSection + afterExalted;

            return result;
        }
    }
}
