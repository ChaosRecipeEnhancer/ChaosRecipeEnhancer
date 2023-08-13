using System.Collections.Generic;
using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

internal class CTwoHandWeaponsManager : ABaseItemClassManager
{
	#region Constructors

	public CTwoHandWeaponsManager()
	{
		ClassName = "TwoHandWeapons";
		ClassFilterName = "\"Two Hand\"";
		ClassColor = Settings.Default.LootFilterWeaponColor;
		AlwaysActive = Settings.Default.LootFilterWeaponsAlwaysActive;
	}

	#endregion

	#region Methods

	public override string SetBaseType()
	{
		var baseType = "Class ";
		baseType += "\"Two Hand Swords\" \"Two Hand Axes\" \"Two Hand Maces\" \"Staves\" \"Warstaves\" \"Bows\"";
		baseType += StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Width <= 2" +
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
		return missingItemClasses.Contains(ClassName) || missingItemClasses.Contains("OneHandWeapons");
	}

	#endregion
}