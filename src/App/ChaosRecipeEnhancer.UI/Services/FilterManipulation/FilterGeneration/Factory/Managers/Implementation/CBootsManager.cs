using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

internal class CBootsManager : ABaseItemClassManager
{
	#region Constructors

	public CBootsManager()
	{
		ClassName = "Boots";
		ClassFilterName = "\"Boots\"";
		ClassColor = Settings.Default.LootFilterBootsColor;
		AlwaysActive = Settings.Default.LootFilterBootsAlwaysActive;
	}

	#endregion

	#region Methods

	public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
	{
		activeItems.BootsActive = newValue;
		return activeItems;
	}

	#endregion
}