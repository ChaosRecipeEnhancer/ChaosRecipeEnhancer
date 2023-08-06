using System.Collections.Generic;
using ChaosRecipeEnhancer.UI.BusinessLogic.Constants;
using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

internal class COneHandWeaponsManager : ABaseItemClassManager
{
	#region Constructors

	public COneHandWeaponsManager()
	{
		ClassName = "OneHandWeapons";
		ClassFilterName = "\"One Hand\"";
		ClassColor = Settings.Default.LootFilterWeaponColor;
		AlwaysActive = Settings.Default.LootFilterWeaponsAlwaysActive;
	}

	#endregion

	#region Methods

	public override string SetBaseType()
	{
		// Seems like we omit claws by design as they don't fit the sizing rules (2 x 2 = 4 stash units)
		var baseType = "Class ";
		baseType += "\"Daggers\" \"One Hand Axes\" \"One Hand Maces\" \"One Hand Swords\" \"Rune Daggers\" \"Sceptres\" \"Thrusting One Hand Swords\" \"Wands\"";
		baseType += StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Width <= 1" +
					StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Height <= 3";

		return baseType;
	}

	public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
	{
		activeItems.WeaponActive = newValue;
		return activeItems;
	}

	public override bool CheckIfMissing(HashSet<string> missingItemClasses)
	{
		// bad, dont like, no good ideas for now tho
		return missingItemClasses.Contains(ClassName) || missingItemClasses.Contains("TwoHandWeapons");
	}

	#endregion
}