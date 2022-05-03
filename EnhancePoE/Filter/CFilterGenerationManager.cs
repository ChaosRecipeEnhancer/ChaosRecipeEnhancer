using EnhancePoE.Const;
using EnhancePoE.Enums;
using EnhancePoE.Model;
using EnhancePoE.Model.Storage;
using EnhancePoE.Properties;
using EnhancePoE.Visitors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnhancePoE.Filter
{
    //add interfaces
    public class CFilterGenerationManager
    {
        private CBaseItemClassManager ItemClassManager;
        public CFilterGenerationManager()
        {
            LoadCustomStyle();
            if (Settings.Default.ExaltedRecipe)
            {
                LoadCustomStyleInfluenced();
            }
        }
        private readonly List<string> CustomStyle = new List<string>();
        private readonly List<string> CustomStyleInfluenced = new List<string>();
        public async Task<ActiveItemTypes> GenerateSectionsAndUpdateFilter(HashSet<string> missingItemClasses)
        {
            ActiveItemTypes res = new ActiveItemTypes();                      

            if (Settings.Default.LootFilterActive)
            {
                CItemClassManagerFactory visitor = new CItemClassManagerFactory();
                var sectionList = new HashSet<string>();
                var sectionListExalted = new HashSet<string>(); 

                foreach (EnumItemClass item in Enum.GetValues(typeof(EnumItemClass)))
                {
                    this.ItemClassManager = visitor.GetItemClassManager(item);

                    var className = this.ItemClassManager.ClassName;
                    var stillMissing = this.ItemClassManager.CheckIfMissing(missingItemClasses);
                    //weapons might be buggy, will try to do some testts
                    if ((Settings.Default.ChaosRecipe || Settings.Default.RegalRecipe)
                        && (this.ItemClassManager.AlwaysActive || stillMissing))
                    {
                        sectionList.Add(this.GenerateSection(false));
                        //find better way to handle active items and sound notification on changes
                        res = this.ItemClassManager.SetActiveTypes(res, true);
                    }
                    else
                    {
                        res = this.ItemClassManager.SetActiveTypes(res, false);
                    }
                    if (Settings.Default.ExaltedRecipe)
                    {
                        sectionListExalted.Add(this.GenerateSection(true));
                    }
                }
                await this.UpdateFilterAsync(sectionList, sectionListExalted);
            }

            return res;
        }
        public string GenerateSection(bool isInfluenced)
        {
            var result = "Show";

            if (isInfluenced)
                result += CConst.newLine + CConst.tab + "HasInfluence Crusader Elder Hunter Redeemer Shaper Warlord";
            else
                result += CConst.newLine + CConst.tab + "HasInfluence None";


            result = result + CConst.newLine + CConst.tab + "Rarity Rare" + CConst.newLine + CConst.tab;
            if (!Settings.Default.IncludeIdentified) result += "Identified False" + CConst.newLine + CConst.tab;

            switch (isInfluenced)
            {
                case false when !ItemClassManager.AlwaysActive && !Settings.Default.RegalRecipe:
                    result += "ItemLevel >= 60" + CConst.newLine + CConst.tab + "ItemLevel <= 74" + CConst.newLine + CConst.tab;
                    break;
                case false when Settings.Default.RegalRecipe:
                    result += "ItemLevel > 75" + CConst.newLine + CConst.tab;
                    break;
                default:
                    result += "ItemLevel >= 60" + CConst.newLine + CConst.tab;
                    break;
            }
            result = ItemClassManager.SetSocketRules(result);

            var baseType = ItemClassManager.SetBaseType(); ;

            result = result + baseType + CConst.newLine + CConst.tab;

            var colors = GetRGB();
            var bgColor = colors.Aggregate("SetBackgroundColor", (current, t) => current + " " + t);

            result = result + bgColor + CConst.newLine + CConst.tab;
            result = isInfluenced
                ? CustomStyleInfluenced.Aggregate(result, (current, cs) => current + cs + CConst.newLine + CConst.tab)
                : CustomStyle.Aggregate(result, (current, cs) => current + cs + CConst.newLine + CConst.tab);

            if (Settings.Default.LootFilterIcons) result = result + "MinimapIcon 2 White Star" + CConst.newLine + CConst.tab;

            return result;
        }
        public string GenerateLootFilter(string oldFilter, IEnumerable<string> sections, bool isChaos = true)
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
        private async Task UpdateFilterAsync(HashSet<string>  sectionList, HashSet<string> sectionListExalted)
        {
            var filterStorage = FilterStorageFactory.Create(Settings.Default);

            var oldFilter = await filterStorage.ReadLootFilterAsync();
            if (oldFilter == null) return;

            var newFilter = this.GenerateLootFilter(oldFilter, sectionList);
            oldFilter = newFilter;
            newFilter = this.GenerateLootFilter(oldFilter, sectionListExalted, false);

            await filterStorage.WriteLootFilterAsync(newFilter);
        }
        private IEnumerable<int> GetRGB()
        {
            int r;
            int g;
            int b;
            int a;
            var color = ItemClassManager.ClassColor;
            var colorList = new List<int>();

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
        private void LoadCustomStyle()
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
        private void LoadCustomStyleInfluenced()
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
    }
}
