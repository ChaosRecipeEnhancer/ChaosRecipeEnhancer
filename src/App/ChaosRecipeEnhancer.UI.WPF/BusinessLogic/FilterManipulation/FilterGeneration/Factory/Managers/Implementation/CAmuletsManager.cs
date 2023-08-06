using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.WPF.Properties;

namespace ChaosRecipeEnhancer.UI.WPF.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

internal class CAmuletsManager : ABaseItemClassManager
{
	#region Constructors

	public CAmuletsManager()
	{
		ClassName = "Amulets";
		ClassFilterName = "\"Amulets\"";
		ClassColor = Settings.Default.LootFilterAmuletColor;
		AlwaysActive = Settings.Default.LootFilterAmuletsAlwaysActive;
	}

	#endregion

	#region Methods

	public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
	{
		activeItems.AmuletActive = newValue;
		return activeItems;
	}

	#endregion
}