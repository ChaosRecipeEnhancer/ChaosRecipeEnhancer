using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

internal class CBodyArmoursManager : ABaseItemClassManager
{
	#region Constructors

	public CBodyArmoursManager()
	{
		ClassName = "BodyArmours";
		ClassFilterName = "\"Body Armours\"";
		ClassColor = Settings.Default.LootFilterBodyArmourColor;
		AlwaysActive = Settings.Default.LootFilterBodyArmourAlwaysActive;
	}

	#endregion

	#region Methods

	public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
	{
		activeItems.ChestActive = newValue;
		return activeItems;
	}

	#endregion
}