using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterStorage;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation;

public class FilterManipulationService
{
    private ABaseItemClassManager _itemClassManager;
    private readonly List<string> _customStyle = new();

    public FilterManipulationService()
    {
        LoadCustomStyle();
    }

    public Settings Settings { get; } = Settings.Default;

    // TODO: [Refactor] mechanism for receiving missing items from some other service and populating based on that limited information
    // public async Task<ActiveItemTypes> GenerateSectionsAndUpdateFilterAsync(HashSet<string> missingItemClasses, bool missingChaosItem)
    // {
    //     var activeItemTypes = new ActiveItemTypes();
    //     var visitor = new CItemClassManagerFactory();
    //     var sectionList = new HashSet<string>();
    //
    //     foreach (ItemClass item in Enum.GetValues(typeof(ItemClass)))
    //     {
    //         _itemClassManager = visitor.GetItemClassManager(item);
    //
    //         var stillMissing = _itemClassManager.CheckIfMissing(missingItemClasses);
    //
    //         // weapons might be buggy, will try to do some tests
    //         if (_itemClassManager.AlwaysActive || stillMissing)
    //         {
    //             // if we need chaos only gear to complete a set (60-74), add that to our filter section
    //             sectionList.Add(GenerateSection());
    //
    //             // find better way to handle active items and sound notification on changes
    //             activeItemTypes = _itemClassManager.SetActiveTypes(activeItemTypes, true);
    //         }
    //         else
    //         {
    //             activeItemTypes = _itemClassManager.SetActiveTypes(activeItemTypes, false);
    //         }
    //     }
    //
    //     if (Settings.Default.LootFilterManipulationEnabled) await UpdateFilterAsync(sectionList);
    //
    //     return activeItemTypes;
    // }

    private string GenerateSection()
    {
        var result = "Show";

        result += StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "HasInfluence None";

        result = result + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Rarity Rare" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

        if (!Settings.Default.IncludeIdentifiedItemsEnabled) result += "Identified False" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

        result += Settings.ChaosRecipeTrackingEnabled switch
        {
            false when !_itemClassManager.AlwaysActive => "ItemLevel >= 60" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "ItemLevel <= 74" +
                                                          StringConstruction.NewLineCharacter + StringConstruction.TabCharacter,
            false => "ItemLevel >= 75" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter,
            _ => "ItemLevel >= 60" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter
        };

        var baseType = _itemClassManager.SetBaseType();

        result = result + baseType + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

        var colors = GetColorRGBAValues();
        var bgColor = colors.Aggregate("SetBackgroundColor", (current, t) => current + " " + t);

        result = result + bgColor + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

        result = _customStyle.Aggregate(result,
            (current, cs) =>
                current + cs + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter);

        // Map Icon setting enabled
        if (Settings.Default.LootFilterIconsEnabled)
            // TODO: [Filter Manipulation] [Enhancement] Add ability to modify map icon for items added to loot filter
            result = result + "MinimapIcon 2 White Star" + StringConstruction.NewLineCharacter +
                     StringConstruction.TabCharacter;

        return result;
    }

    private static string GenerateLootFilter(string oldFilter, IEnumerable<string> sections)
    {
        // can't use our Env const due to compile time requirement
        const string newLine = "\n";

        var beforeSection = "";
        var sectionStart = "# Chaos Recipe START - Filter Manipulation by Chaos Recipe Enhancer";
        var sectionBody = "";
        var sectionEnd = "# Chaos Recipe END - Filter Manipulation by Chaos Recipe Enhancer";
        var afterSection = "";

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

    private static async Task UpdateFilterAsync(IEnumerable<string> sectionList)
    {
        var filterStorage = FilterStorageFactory.Create(Settings.Default);
        var oldFilter = await filterStorage.ReadLootFilterAsync();

        // return if no old filter detected (usually caused by user error no path selected)
        // in our case the manager doesn't care about setting error this should likely be an Exception
        if (oldFilter == null) return;

        var newFilter = GenerateLootFilter(oldFilter, sectionList);
        await filterStorage.WriteLootFilterAsync(newFilter);
    }

    private IEnumerable<int> GetColorRGBAValues()
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
                FilterAssets.DefaultNormalItemFilterStyleFilePath);

        var style = File.ReadAllLines(pathNormalItemsStyle);

        foreach (var line in style)
        {
            if (line == "") continue;
            if (line.Contains("#")) continue;
            _customStyle.Add(line.Trim());
        }
    }
}