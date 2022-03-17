using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var pathNormalItemsStyle = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"Styles\NormalItemsStyle.txt");
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
            var pathInfluencedItemsStyle = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, @"Styles\InfluencedItemsStyle.txt");
            var style = File.ReadAllLines(pathInfluencedItemsStyle);
            foreach (var line in style)
            {
                if (line == "") continue;
                if (line.Contains("#")) continue;
                CustomStyleInfluenced.Add(line.Trim());
            }
        }

        public static string GenerateSection(string itemClass, bool influenced = false, bool alwaysActive = false)
        {
            var result = "Show";

            const string newLine = "\n";
            const string tab = "\t";

            if (influenced)
                result += newLine + tab + "HasInfluence Crusader Elder Hunter Redeemer Shaper Warlord";
            else
                result += newLine + tab + "HasInfluence None";


            result = result + newLine + tab + "Rarity Rare" + newLine + tab;
            if (!Settings.Default.IncludeIdentified) result += "Identified False" + newLine + tab;

            switch (influenced)
            {
                case false when !alwaysActive && !Settings.Default.RegalRecipe:
                    result += "ItemLevel >= 60" + newLine + tab + "ItemLevel <= 74" + newLine + tab;
                    break;
                case false when Settings.Default.RegalRecipe:
                    result += "ItemLevel > 75" + newLine + tab;
                    break;
                default:
                    result += "ItemLevel >= 60" + newLine + tab;
                    break;
            }

            if (itemClass == "\"Body Armours\"") result += "Sockets <= 5" + newLine + tab + "LinkedSockets <= 5" + newLine + tab;

            var baseType = "Class ";

            switch (itemClass)
            {
                case "\"One Hand\"":
                    // Seems like we omit claws by design as they don't fit the rule of 
                    baseType += "\"Daggers\" \"One Hand Axes\" \"One Hand Maces\" \"One Hand Swords\" \"Rune Daggers\" \"Sceptres\" \"Thrusting One Hand Swords\" \"Wands\"";
                    baseType += newLine + tab + "Width <= 1" + newLine + tab + "Height <= 3";
                    break;
                case "\"Two Hand\"":
                    // TODO: There have been issues reported with users not being able to fit 2 sets in their due to the size of some 2-handers, but looks like we have the WxH rules set here...
                    baseType += "\"Two Hand Swords\" \"Two Hand Axes\" \"Two Hand Maces\" \"Staves\" \"Warstaves\" \"Bows\"";
                    baseType += newLine + tab + "Width <= 2" + newLine + tab + "Height <= 3";
                    baseType += newLine + tab + "Sockets <= 5" + newLine + tab + "LinkedSockets <= 5";
                    break;
                default:
                    baseType += itemClass;
                    break;
            }

            result = result + baseType + newLine + tab;

            var colors = GetRGB(itemClass);
            var bgColor = colors.Aggregate("SetBackgroundColor", (current, t) => current + " " + t);

            result = result + bgColor + newLine + tab;
            result = influenced
                ? CustomStyleInfluenced.Aggregate(result, (current, cs) => current + cs + newLine + tab)
                : CustomStyle.Aggregate(result, (current, cs) => current + cs + newLine + tab);

            if (Settings.Default.LootFilterIcons) result = result + "MinimapIcon 2 White Star" + newLine + tab;

            return result;
        }

        public static IEnumerable<int> GetRGB(string type)
        {
            int r;
            int g;
            int b;
            int a;
            var color = "";
            var colorList = new List<int>();

            switch (type)
            {
                case "\"Rings\"":
                    color = Settings.Default.ColorRing;
                    break;
                case "\"Amulets\"":
                    color = Settings.Default.ColorAmulet;
                    break;
                case "\"Belts\"":
                    color = Settings.Default.ColorBelt;
                    break;
                case "\"Helmets\"":
                    color = Settings.Default.ColorHelmet;
                    break;
                case "\"One Hand\"":
                    color = Settings.Default.ColorWeapon;
                    break;
                case "\"Gloves\"":
                    color = Settings.Default.ColorGloves;
                    break;
                case "\"Boots\"":
                    color = Settings.Default.ColorBoots;
                    break;
                case "\"Body Armours\"":
                    color = Settings.Default.ColorChest;
                    break;
                case "\"Two Hand\"":
                    color = Settings.Default.ColorWeapon;
                    break;
            }

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
        public static string GenerateLootFilter(string oldFilter, IEnumerable<string> sections, bool isChaos = true)
        {
            // order has to be:
            // 1. exa start
            // 2. exa end
            // 3. chaos start
            // 4. chaos end
            string sectionName = isChaos ? "Chaos" : "Exalted";
            const string newLine = "\n";
            string sectionStart = "#Chaos Recipe Enhancer by kosace " + sectionName + " Recipe Start";
            string sectionEnd = "#Chaos Recipe Enhancer by kosace " + sectionName + " Recipe End";
            var sectionBody = "";
            var beforeSection = "";
            string afterSection;

            // generate chaos recipe section
            sectionBody += sectionStart + newLine + newLine;
            sectionBody = sections.Aggregate(sectionBody, (current, s) => current + (s + newLine));
            sectionBody += sectionEnd + newLine;

            string[] sep = { sectionEnd + newLine };
            var split = oldFilter.Split(sep, StringSplitOptions.None);

            if (split.Length > 1)
            {
                afterSection = split[1];

                string[] sep2 = { sectionStart };
                var split2 = split[0].Split(sep2, StringSplitOptions.None);

                if (split2.Length > 1)
                    beforeSection = split2[0];
                else
                    afterSection = oldFilter;
            }
            else
            {
                afterSection = oldFilter;
            }

            return beforeSection + sectionBody + afterSection;
        }
    }
}