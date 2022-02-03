using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EnhancePoE.Properties;

namespace EnhancePoE.Model
{
    public static class FilterGeneration
    {
        public static List<string> CustomStyle { get; set; } = new List<string>();
        public static List<string> CustomStyleInfluenced { get; set; } = new List<string>();

        public static void LoadCustomStyle()
        {
            CustomStyle.Clear();
            var pathNormalItemsStyle = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Styles\NormalItemsStyle.txt");
            var style = File.ReadAllLines(pathNormalItemsStyle);
            foreach (var line in style)
            {
                if (line == "") continue;
                if (line.Contains("#")) continue;
                CustomStyle.Add(line.Trim());
            }
        }

        public static void LoadCustomStyleInfluenced()
        {
            CustomStyleInfluenced.Clear();
            var pathInfluencedItemsStyle = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Styles\InfluencedItemsStyle.txt");
            var style = File.ReadAllLines(pathInfluencedItemsStyle);
            foreach (var line in style)
            {
                if (line == "") continue;
                if (line.Contains("#")) continue;
                CustomStyleInfluenced.Add(line.Trim());
            }
        }

        public static string GenerateSection(bool show, string itemClass, bool influenced = false, bool onlyChaos = false)
        {
            var result = "";
            if (show)
                result += "Show";
            else
                //result += "Hide";
                return "";
            var nl = "\n";
            var tab = "\t";
            if (influenced)
                result += nl + tab + "HasInfluence Crusader Elder Hunter Redeemer Shaper Warlord";
            else
                result += nl + tab + "HasInfluence None";


            result = result + nl + tab + "Rarity Rare" + nl + tab;
            if (!Settings.Default.IncludeIdentified) result += "Identified False" + nl + tab;
            if (!influenced && onlyChaos && !Settings.Default.RegalRecipe)
                result += "ItemLevel >= 60" + nl + tab + "ItemLevel <= 74" + nl + tab;
            else if (!influenced && Settings.Default.RegalRecipe)
                result += "ItemLevel > 75" + nl + tab;
            else
                result += "ItemLevel >= 60" + nl + tab;

            if (itemClass == "Body Armours") result += "Sockets <= 5" + nl + tab + "LinkedSockets <= 5" + nl + tab;

            var baseType = "Class ";

            if (itemClass == "OneHandWeapons")
            {
                baseType += "\"Daggers\" \"One Hand Axes\" \"One Hand Maces\" \"One Hand Swords\" \"Rune Daggers\" \"Sceptres\" \"Thrusting One Hand Swords\" \"Wands\"";
                baseType += nl + tab + "Width <= 1" + nl + tab + "Height <= 3";
            }
            else if (itemClass == "TwoHandWeapons")
            {
                baseType += "\"Two Hand Swords\" \"Two Hand Axes\" \"Two Hand Maces\" \"Staves\" \"Warstaves\" \"Bows\"";
                baseType += nl + tab + "Width <= 2" + nl + tab + "Height <= 3";
                baseType += nl + tab + "Sockets <= 5" + nl + tab + "LinkedSockets <= 5";
            }
            else
            {
                baseType += itemClass;
            }

            result = result + baseType + nl + tab;

            var bgColor = "SetBackgroundColor";

            var colors = GetRGB(itemClass);
            for (var i = 0; i < colors.Count; i++) bgColor = bgColor + " " + colors[i];

            result = result + bgColor + nl + tab;

            if (influenced)
                foreach (var cs in CustomStyleInfluenced)
                    result = result + cs + nl + tab;
            else
                foreach (var cs in CustomStyle)
                    result = result + cs + nl + tab;

            if (Settings.Default.LootfilterIcons) result = result + "MinimapIcon 2 White Star" + nl + tab;

            return result;
        }

        public static List<int> GetRGB(string type)
        {
            //Trace.WriteLine(type);
            int r;
            int g;
            int b;
            int a;
            var color = "";
            var colorList = new List<int>();
            if (type == "\"Rings\"") color = Settings.Default.ColorRing;
            if (type == "\"Amulets\"") color = Settings.Default.ColorAmulet;
            if (type == "\"Belts\"") color = Settings.Default.ColorBelt;
            if (type == "\"Helmets\"") color = Settings.Default.ColorHelmet;
            if (type == "\"OneHandWeapons\"") color = Settings.Default.ColorWeapon;
            if (type == "\"Gloves\"") color = Settings.Default.ColorGloves;
            if (type == "\"Boots\"") color = Settings.Default.ColorBoots;
            if (type == "\"Body Armours\"") color = Settings.Default.ColorChest;
            if (type == "\"TwoHandWeapons\"") color = Settings.Default.ColorWeapon;
            if (color != "")
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

            const string newLine = "\n";
            string result;
            var chaosSection = "";
            const string chaosStart = "#Chaos Recipe Enhancer by kosace Chaos Recipe Start";
            const string chaosEnd = "#Chaos Recipe Enhancer by kosace Chaos Recipe End";
            
            var beforeChaos = "";
            var afterChaos = "";

            // generate chaos recipe section
            chaosSection += chaosStart + newLine + newLine;
            foreach (var s in sections) chaosSection += s + newLine;
            chaosSection += chaosEnd + newLine;

            string[] sep = { chaosEnd + newLine };
            var split = oldFilter.Split(sep, StringSplitOptions.None);

            if (split.Length > 1)
            {
                afterChaos = split[1];
                string[] sep2 = { chaosStart };
                var split2 = split[0].Split(sep2, StringSplitOptions.None);

                if (split2.Length > 1)
                    beforeChaos = split2[0];
                else
                    afterChaos = oldFilter;
            }
            else
            {
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

            var nl = "\n";
            string result;
            var exaltedSection = "";
            var exaltedStart = "#Chaos Recipe Enhancer by kosace Exalted Recipe Start";
            var exaltedEnd = "#Chaos Recipe Enhancer by kosace Exalted Recipe End";

            var beforeExalted = "";
            var afterExalted = "";

            // generate chaos recipe section
            exaltedSection += exaltedStart + nl + nl;
            foreach (var s in sections) exaltedSection += s + nl;
            exaltedSection += exaltedEnd + nl;

            string[] sep = { exaltedEnd + nl };
            var split = oldFilter.Split(sep, StringSplitOptions.None);

            if (split.Length > 1)
            {
                afterExalted = split[1];

                string[] sep2 = { exaltedStart };
                var split2 = split[0].Split(sep2, StringSplitOptions.None);

                if (split2.Length > 1)
                    beforeExalted = split2[0];
                else
                    afterExalted = oldFilter;
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