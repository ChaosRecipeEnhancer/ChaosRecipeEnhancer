using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Const;
using ChaosRecipeEnhancer.UI.Enums;
using ChaosRecipeEnhancer.UI.Factory;
using ChaosRecipeEnhancer.UI.Factory.Managers;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Model.Storage;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Filter
{
    //add interfaces
    public class CFilterGenerationManager
    {
        #region Fields

        private ABaseItemClassManager _itemClassManager;
        private readonly List<string> _customStyle = new List<string>();
        private readonly List<string> _customStyleInfluenced = new List<string>();

        #endregion

        #region Constructors

        public CFilterGenerationManager()
        {
            LoadCustomStyle();
            if (Settings.Default.ExaltedShardRecipeTrackingEnabled)
            {
                LoadCustomStyleInfluenced();
            }
        }

        #endregion

        #region Methods

        public async Task<ActiveItemTypes> GenerateSectionsAndUpdateFilterAsync(HashSet<string> missingItemClasses,
            bool missingChaosItem)
        {
            var activeItemTypes = new ActiveItemTypes();

            if (Settings.Default.LootFilterManipulationEnabled)
            {
                var visitor = new CItemClassManagerFactory();
                var sectionList = new HashSet<string>();
                var sectionListExalted = new HashSet<string>();

                foreach (EnumItemClass item in Enum.GetValues(typeof(EnumItemClass)))
                {
                    _itemClassManager = visitor.GetItemClassManager(item);

                    var stillMissing = _itemClassManager.CheckIfMissing(missingItemClasses);

                    // weapons might be buggy, will try to do some tests
                    if ((Settings.Default.ChaosRecipeTrackingEnabled || Settings.Default.RegalRecipeTrackingEnabled)
                        && (_itemClassManager.AlwaysActive || stillMissing))
                    {
                        // if we need chaos only gear to complete a set (60-74), add that to our filter section
                        if (missingChaosItem)
                        {
                            sectionList.Add(GenerateSection(false, true));
                        }
                        // else add any gear piece 60+ to our section for that item class
                        else
                        {
                            sectionList.Add(GenerateSection(false));
                        }

                        // find better way to handle active items and sound notification on changes
                        activeItemTypes = _itemClassManager.SetActiveTypes(activeItemTypes, true);
                    }
                    else
                    {
                        activeItemTypes = _itemClassManager.SetActiveTypes(activeItemTypes, false);
                    }

                    if (Settings.Default.ExaltedShardRecipeTrackingEnabled)
                    {
                        sectionListExalted.Add(GenerateSection(true));
                    }
                }

                await UpdateFilterAsync(sectionList, sectionListExalted);
            }

            return activeItemTypes;
        }

        public string GenerateSection(bool isInfluenced, bool onlyChaos = false)
        {
            var result = "Show";

            if (isInfluenced)
            {
                result += CConst.newLine + CConst.tab + "HasInfluence Crusader Elder Hunter Redeemer Shaper Warlord";
            }
            else
            {
                result += CConst.newLine + CConst.tab + "HasInfluence None";
            }

            result = result + CConst.newLine + CConst.tab + "Rarity Rare" + CConst.newLine + CConst.tab;
            if (!Settings.Default.IncludeIdentifiedItemsEnabled) result += "Identified False" + CConst.newLine + CConst.tab;

            switch (isInfluenced)
            {
                case false when !_itemClassManager.AlwaysActive && onlyChaos && !Settings.Default.RegalRecipeTrackingEnabled:
                    result += "ItemLevel >= 60" + CConst.newLine + CConst.tab + "ItemLevel <= 74" + CConst.newLine +
                              CConst.tab;
                    break;
                case false when Settings.Default.RegalRecipeTrackingEnabled:
                    result += "ItemLevel > 75" + CConst.newLine + CConst.tab;
                    break;
                default:
                    result += "ItemLevel >= 60" + CConst.newLine + CConst.tab;
                    break;
            }

            // TODO: [Remove] I don't think we need this
            // result = _itemClassManager.SetSocketRules(result);

            var baseType = _itemClassManager.SetBaseType();

            result = result + baseType + CConst.newLine + CConst.tab;

            var colors = GetRGB();
            var bgColor = colors.Aggregate("SetBackgroundColor", (current, t) => current + " " + t);

            result = result + bgColor + CConst.newLine + CConst.tab;
            result = isInfluenced
                ? _customStyleInfluenced.Aggregate(result, (current, cs) => current + cs + CConst.newLine + CConst.tab)
                : _customStyle.Aggregate(result, (current, cs) => current + cs + CConst.newLine + CConst.tab);

            // Map Icon setting enabled
            if (Settings.Default.LootFilterIconsEnabled)
            {
                result = result + "MinimapIcon 2 White Star" + CConst.newLine + CConst.tab;
            }

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
            sectionBody = sections.Aggregate(sectionBody, (current, s) => current + s + newLine);
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

        private async Task UpdateFilterAsync(IEnumerable<string> sectionList, IEnumerable<string> sectionListExalted)
        {
            var filterStorage = FilterStorageFactory.Create(Settings.Default);

            var oldFilter = await filterStorage.ReadLootFilterAsync();
            if (oldFilter == null) return;

            var newFilter = GenerateLootFilter(oldFilter, sectionList);
            oldFilter = newFilter;
            newFilter = GenerateLootFilter(oldFilter, sectionListExalted, false);

            await filterStorage.WriteLootFilterAsync(newFilter);
        }

        private IEnumerable<int> GetRGB()
        {
            int r;
            int g;
            int b;
            int a;
            var color = _itemClassManager.ClassColor;
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
            _customStyle.Clear();
            var pathNormalItemsStyle =
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                    @"Assets\FilterStyles\NormalItemsStyle.txt");
            var style = File.ReadAllLines(pathNormalItemsStyle);
            foreach (var line in style)
            {
                if (line == "") continue;
                if (line.Contains("#")) continue;
                _customStyle.Add(line.Trim());
            }
        }

        private void LoadCustomStyleInfluenced()
        {
            _customStyleInfluenced.Clear();
            var pathInfluencedItemsStyle =
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                    @"Assets\FilterStyles\InfluencedItemsStyle.txt");
            var style = File.ReadAllLines(pathInfluencedItemsStyle);
            foreach (var line in style)
            {
                if (line == "") continue;
                if (line.Contains("#")) continue;
                _customStyleInfluenced.Add(line.Trim());
            }
        }

        #endregion
    }
}