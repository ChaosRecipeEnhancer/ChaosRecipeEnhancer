using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.App.FilterManipulation.Factory;
using ChaosRecipeEnhancer.App.FilterManipulation.Factory.Managers;
using ChaosRecipeEnhancer.App.Models;
using ChaosRecipeEnhancer.App.Models.Settings;
using ChaosRecipeEnhancer.DataModels.Constants;
using ChaosRecipeEnhancer.DataModels.Enums;

namespace ChaosRecipeEnhancer.App.FilterManipulation
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
            if (Settings.Default.ExaltedRecipe)
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

            if (!Settings.Default.LootFilterActive) return activeItemTypes;
            
            var visitor = new CItemClassManagerFactory();
            var sectionList = new HashSet<string>();
            var sectionListExalted = new HashSet<string>();

            foreach (EnumItemClass item in Enum.GetValues(typeof(EnumItemClass)))
            {
                _itemClassManager = visitor.GetItemClassManager(item);

                var stillMissing = _itemClassManager.CheckIfMissing(missingItemClasses);

                // weapons might be buggy, will try to do some tests
                if ((Settings.Default.ChaosRecipe || Settings.Default.RegalRecipe)
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

                if (Settings.Default.ExaltedRecipe)
                {
                    sectionListExalted.Add(GenerateSection(true));
                }
            }

            await UpdateFilterAsync(sectionList, sectionListExalted);

            return activeItemTypes;
        }

        public string GenerateSection(bool isInfluenced, bool onlyChaos = false)
        {
            var result = "Show";

            if (isInfluenced)
            {
                result += FilterConstants.NewLine + FilterConstants.Tab + "HasInfluence Crusader Elder Hunter Redeemer Shaper Warlord";
            }
            else
            {
                result += FilterConstants.NewLine + FilterConstants.Tab + "HasInfluence None";
            }

            result = result + FilterConstants.NewLine + FilterConstants.Tab + "Rarity Rare" + FilterConstants.NewLine + FilterConstants.Tab;
            if (!Settings.Default.IncludeIdentified) result += "Identified False" + FilterConstants.NewLine + FilterConstants.Tab;

            switch (isInfluenced)
            {
                case false when !_itemClassManager.AlwaysActive && onlyChaos && !Settings.Default.RegalRecipe:
                    result += "ItemLevel >= 60" + FilterConstants.NewLine + FilterConstants.Tab + "ItemLevel <= 74" + FilterConstants.NewLine +
                              FilterConstants.Tab;
                    break;
                case false when Settings.Default.RegalRecipe:
                    result += "ItemLevel > 75" + FilterConstants.NewLine + FilterConstants.Tab;
                    break;
                default:
                    result += "ItemLevel >= 60" + FilterConstants.NewLine + FilterConstants.Tab;
                    break;
            }

            var baseType = _itemClassManager.SetBaseType();

            result = result + baseType + FilterConstants.NewLine + FilterConstants.Tab;

            var colors = GetRGB();
            var bgColor = colors.Aggregate("SetBackgroundColor", (current, t) => current + " " + t);

            result = result + bgColor + FilterConstants.NewLine + FilterConstants.Tab;
            result = isInfluenced
                ? _customStyleInfluenced.Aggregate(result, (current, cs) => current + cs + FilterConstants.NewLine + FilterConstants.Tab)
                : _customStyle.Aggregate(result, (current, cs) => current + cs + FilterConstants.NewLine + FilterConstants.Tab);

            // Map Icon setting enabled
            if (Settings.Default.LootFilterIcons)
            {
                result = result + "MinimapIcon 2 White Star" + FilterConstants.NewLine + FilterConstants.Tab;
            }

            return result;
        }

        private static string GenerateLootFilter(string oldFilter, IEnumerable<string> sections, bool isChaos = true)
        {
            // TODO: [Remove] [Refactor] Why do we have this defined multiple times?
            const string newLine = "\n";
            
            // order has to be:
            // 1. exa start
            // 2. exa end
            // 3. chaos start
            // 4. chaos end
            
            var sectionName = isChaos ? "Chaos" : "Exalted";
            var sectionStart = "# Chaos Recipe Enhancer " + sectionName + " Recipe Start";
            var sectionEnd = "# Chaos Recipe Enhancer " + sectionName + " Recipe End";
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