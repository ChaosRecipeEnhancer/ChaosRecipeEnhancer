using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.WPF.Properties;

namespace ChaosRecipeEnhancer.UI.WPF.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

internal class CHelmetManager : ABaseItemClassManager
{
	#region Constructors

	public CHelmetManager()
	{
		ClassName = "Helmets";
		ClassFilterName = "\"Helmets\"";
		ClassColor = Settings.Default.LootFilterHelmetColor;
		AlwaysActive = Settings.Default.LootFilterHelmetsAlwaysActive;
	}

	#endregion

	#region Methods

	public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
	{
		activeItems.HelmetActive = newValue;
		return activeItems;
	}

	#endregion
}