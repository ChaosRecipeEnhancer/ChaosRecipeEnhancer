using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Config;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation;

public interface IFilterManipulationService
{
    public Task GenerateSectionsAndUpdateFilterAsync(HashSet<string> missingItemClasses, bool missingChaosItem);
    public void RemoveChaosRecipeSectionAsync();
}

public class FilterManipulationService : IFilterManipulationService
{
    private readonly IUserSettings _userSettings;
    private ABaseItemClassManager _itemClassManager;
    private readonly List<string> _customStyle = [];

    public FilterManipulationService(IUserSettings userSettings)
    {
        _userSettings = userSettings;

        LoadCustomStyle();
    }

    // TODO: [Refactor] mechanism for receiving missing items from some other service and populating based on that limited information
    public async Task GenerateSectionsAndUpdateFilterAsync(HashSet<string> missingItemClasses, bool missingChaosItem)
    {
        var activeItemTypes = new ActiveItemTypes();
        var visitor = new CItemClassManagerFactory();
        var sectionList = new HashSet<string>();

        foreach (ItemClass item in Enum.GetValues(typeof(ItemClass)))
        {
            _itemClassManager = visitor.GetItemClassManager(item);

            var stillMissing = _itemClassManager.CheckIfMissing(missingItemClasses);

            // weapons might be buggy, will try to do some tests
            if (_itemClassManager.AlwaysActive && !_itemClassManager.AlwaysHidden || stillMissing)
            {
                // if we need chaos only gear to complete a set (60-74), add that to our filter section
                sectionList.Add(GenerateSection(missingChaosItem));

                // find better way to handle active items and sound notification on changes
                activeItemTypes = _itemClassManager.SetActiveTypes(activeItemTypes, true);
            }
            else
            {
                activeItemTypes = _itemClassManager.SetActiveTypes(activeItemTypes, false);
            }
        }

        if (_userSettings.LootFilterManipulationEnabled) await UpdateFilterAsync(sectionList);
    }

    private string GenerateSection(bool missingChaosItem)
    {
        var result = string.Empty;

        // 'Base' Stuff
        // Ensure no influence
        // Ensure item is rare
        result += StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "HasInfluence None";
        result = result + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Rarity Rare" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

        // Identified Item Setting
        if (!_userSettings.IncludeIdentifiedItemsEnabled) result += "Identified False" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;


        // Adding ItemLevel section
        // Chaos Recipe
        if (_userSettings.ChaosRecipeTrackingEnabled)
        {
            result += "ItemLevel >= 60" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

            if (missingChaosItem)
            {
                result += "ItemLevel <= 74" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;
            }
        }
        // Regal Recipe
        else
        {
            result += "ItemLevel >= 75" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;
        }

        // Base ItemClass Type Setting
        string baseType;

        // weapons get special treatment due to space saving options
        if (_itemClassManager.ClassName.Equals("OneHandWeapons"))
        {
            baseType = _itemClassManager.SetBaseType(
                _userSettings.LootFilterSpaceSavingHideLargeWeapons,
                _userSettings.LootFilterSpaceSavingHideOffHand
            );
        }
        else if (_itemClassManager.ClassName.Equals("TwoHandWeapons"))
        {
            baseType = _itemClassManager.SetBaseType(_userSettings.LootFilterSpaceSavingHideLargeWeapons);
        }
        else
        {
            baseType = _itemClassManager.SetBaseType();
        }

        result = result + baseType + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

        // Adding Filter Template (Assets/FilterStyles/NormalItemStyle.txt)
        result = _customStyle.Aggregate(result,
            (current, cs) =>
                current + cs + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter);

        // Always Show / Always Hide Settings
        string showOrHide;
        if (_itemClassManager.AlwaysActive)
        {
            showOrHide = "Show";
        }
        else if (_itemClassManager.AlwaysHidden)
        {
            showOrHide = "Hide";
        }
        else
        {
            showOrHide = "Show";
        }

        // Add showOrHide to beginning of result string
        result = showOrHide + result;

        //Font Size Setting
        result = result + $"SetFontSize {_itemClassManager.FontSize}" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

        // Font Color Setting
        var rgbaTextColors = GetColorRGBAValues(_itemClassManager.FontColor);
        var textColor = rgbaTextColors.Aggregate("SetTextColor", (current, t) => current + " " + t);
        result = result + textColor + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

        // Border Color Setting
        var rgbaBorderColors = GetColorRGBAValues(_itemClassManager.BorderColor);
        var bdColor = rgbaBorderColors.Aggregate("SetBorderColor", (current, t) => current + " " + t);
        result = result + bdColor + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

        // Background Color Setting
        var rgbaBackgroundColors = GetColorRGBAValues(_itemClassManager.ClassColor);
        var bgColor = rgbaBackgroundColors.Aggregate("SetBackgroundColor", (current, t) => current + " " + t);
        result = result + bgColor + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

        // Map Icon Setting
        if (_itemClassManager.MapIconEnabled)
        {
            // TODO: [Filter Manipulation] [Enhancement] Add ability to modify map icon for items added to loot filter
            result = result + $"MinimapIcon {GetFilterMapIconSize(_itemClassManager.MapIconSize)} {GetFilterColor(_itemClassManager.MapIconColor)} {GetFilterMapIconShape(_itemClassManager.MapIconShape)}" + StringConstruction.NewLineCharacter +
                     StringConstruction.TabCharacter;
        }

        // Beam Setting
        if (_itemClassManager.BeamEnabled)
        {
            result = result + $"PlayEffect {GetFilterColor(_itemClassManager.BeamColor)} {(_itemClassManager.BeamTemporary ? "Temp" : "")}" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;
        }

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

    public async void RemoveChaosRecipeSectionAsync()
    {
        var filterStorage = FilterStorageFactory.Create(Settings.Default);
        var oldFilterContent = await filterStorage.ReadLootFilterAsync();

        // return if no old filter detected (usually caused by user error no path selected)
        // in our case the manager doesn't care about setting error this should likely be an Exception
        if (oldFilterContent == null) return;

        // Define the pattern for Chaos Recipe sections
        const string pattern = @"# Chaos Recipe START - Filter Manipulation by Chaos Recipe Enhancer[\s\S]*?# Chaos Recipe END - Filter Manipulation by Chaos Recipe Enhancer\s*";
        var regex = new Regex(pattern, RegexOptions.Multiline);

        // Remove the Chaos Recipe sections from the content
        var cleanedContent = regex.Replace(oldFilterContent, "");
        await filterStorage.WriteLootFilterAsync(cleanedContent);
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

    private IEnumerable<int> GetColorRGBAValues(string hexColorSetting)
    {
        int r;
        int g;
        int b;
        int a;
        var color = hexColorSetting;
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

        var pathNormalItemsStyle = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty, FilterConfig.DefaultNormalItemFilterStyleFilePath);

        var style = File.ReadAllLines(pathNormalItemsStyle);

        foreach (var line in style)
        {
            if (line == "") continue;
            if (line.Contains("#")) continue;
            _customStyle.Add(line.Trim());
        }
    }

    private static string GetFilterMapIconSize(int mapIconSizeSetting)
    {
        return mapIconSizeSetting switch
        {
            0 => "0",   // Large
            1 => "1",   // Medium
            2 => "2",   // Small
            _ => "1"    // Default to Large
        };
    }

    private static string GetFilterColor(int colorSetting)
    {
        return colorSetting switch
        {
            0 => "Blue",
            1 => "Brown",
            2 => "Cyan",
            3 => "Green",
            4 => "Grey",
            5 => "Orange",
            6 => "Pink",
            7 => "Purple",
            8 => "Red",
            9 => "White",
            10 => "Yellow",
            _ => "Yellow" // Default to Yellow
        };
    }

    private static string GetFilterMapIconShape(int mapIconShapeSetting)
    {
        return mapIconShapeSetting switch
        {
            0 => "Circle",
            1 => "Cross",
            2 => "Diamond",
            3 => "Hexagon",
            4 => "Kite",
            5 => "Moon",
            6 => "Pentagon",
            7 => "Raindrop",
            8 => "Square",
            9 => "Star",
            10 => "Triangle",
            11 => "UpsideDownHouse",
            _ => "Circle" // Default to Circle
        };
    }
}