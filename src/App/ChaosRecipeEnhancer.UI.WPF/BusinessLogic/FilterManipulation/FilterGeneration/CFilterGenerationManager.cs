using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.Constants;
using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.Enums;
using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.FilterManipulation.FilterGeneration.Factory;
using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers;
using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.FilterManipulation.FilterStorage;
using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.WPF.Properties;

namespace ChaosRecipeEnhancer.UI.WPF.BusinessLogic.FilterManipulation.FilterGeneration;

public class CFilterGenerationManager
{
	#region Constructors

	public CFilterGenerationManager()
	{
		LoadCustomStyle();
		if (Settings.Default.ExaltedShardRecipeTrackingEnabled) LoadCustomStyleInfluenced();
	}

	#endregion

	#region Fields

	private ABaseItemClassManager _itemClassManager;
	private readonly List<string> _customStyle = new List<string>();
	private readonly List<string> _customStyleInfluenced = new List<string>();

	#endregion

	#region Methods

	public async Task<ActiveItemTypes> GenerateSectionsAndUpdateFilterAsync(HashSet<string> missingItemClasses,
		bool missingChaosItem)
	{
		var activeItemTypes = new ActiveItemTypes();
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
				sectionList.Add(missingChaosItem
					? GenerateSection(false, true)
					:
					// else add any gear piece 60+ to our section for that item class
					GenerateSection(false));

				// find better way to handle active items and sound notification on changes
				activeItemTypes = _itemClassManager.SetActiveTypes(activeItemTypes, true);
			}
			else
			{
				activeItemTypes = _itemClassManager.SetActiveTypes(activeItemTypes, false);
			}

			if (Settings.Default.ExaltedShardRecipeTrackingEnabled) sectionListExalted.Add(GenerateSection(true));
		}

		if (Settings.Default.LootFilterManipulationEnabled)
			await UpdateFilterAsync(sectionList, sectionListExalted);

		return activeItemTypes;
	}

	private string GenerateSection(bool isInfluenced, bool onlyChaos = false)
	{
		var result = "Show";

		if (isInfluenced)
			result += StringConstruction.NewLineCharacter + StringConstruction.TabCharacter +
					  "HasInfluence Crusader Elder Hunter Redeemer Shaper Warlord";
		else
			result += StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "HasInfluence None";

		result = result + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Rarity Rare" +
				 StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

		if (!Settings.Default.IncludeIdentifiedItemsEnabled)
			result += "Identified False" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

		switch (isInfluenced)
		{
			case false when !_itemClassManager.AlwaysActive && onlyChaos &&
							!Settings.Default.RegalRecipeTrackingEnabled:
				result += "ItemLevel >= 60" + StringConstruction.NewLineCharacter +
						  StringConstruction.TabCharacter + "ItemLevel <= 74" +
						  StringConstruction.NewLineCharacter +
						  StringConstruction.TabCharacter;
				break;
			case false when Settings.Default.RegalRecipeTrackingEnabled:
				result += "ItemLevel >= 75" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;
				break;
			default:
				result += "ItemLevel >= 60" + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;
				break;
		}

		var baseType = _itemClassManager.SetBaseType();

		result = result + baseType + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;

		var colors = GetRGB();
		var bgColor = colors.Aggregate("SetBackgroundColor", (current, t) => current + " " + t);

		result = result + bgColor + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter;
		result = isInfluenced
			? _customStyleInfluenced.Aggregate(result,
				(current, cs) =>
					current + cs + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter)
			: _customStyle.Aggregate(result,
				(current, cs) =>
					current + cs + StringConstruction.NewLineCharacter + StringConstruction.TabCharacter);

		// Map Icon setting enabled
		if (Settings.Default.LootFilterIconsEnabled)
			// legacytodo: [Enhancement] Add ability to modify map icon for items added to loot filter
			result = result + "MinimapIcon 2 White Star" + StringConstruction.NewLineCharacter +
					 StringConstruction.TabCharacter;

		return result;
	}

	private static string GenerateLootFilter(string oldFilter, IEnumerable<string> sections, bool isChaos = true)
	{
		// order has to be:
		// 1. exa start
		// 2. exa end
		// 3. chaos start
		// 4. chaos end
		var sectionName = isChaos ? "Chaos" : "Exalted";
		const string newLine = "\n";
		var sectionStart = "#Chaos Recipe Enhancer by kosace " + sectionName + " Recipe Start";
		var sectionEnd = "#Chaos Recipe Enhancer by kosace " + sectionName + " Recipe End";
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

	private static async Task UpdateFilterAsync(IEnumerable<string> sectionList,
		IEnumerable<string> sectionListExalted)
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
				FilterAssets.DefaultNormalItemFilterStyleFilePath);

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
				FilterAssets.DefaultInfluencedItemFilterStyleFilePath);

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