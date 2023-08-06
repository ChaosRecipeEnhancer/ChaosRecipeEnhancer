using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.WPF.Properties;

namespace ChaosRecipeEnhancer.UI.WPF.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

internal class CRingsManager : ABaseItemClassManager
{
	#region Constructors

	public CRingsManager()
	{
		ClassName = "Rings";
		ClassFilterName = "\"Rings\"";
		ClassColor = Settings.Default.LootFilterRingColor;
		AlwaysActive = Settings.Default.LootFilterRingsAlwaysActive;
	}

	#endregion

	#region Methods

	public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
	{
		activeItems.RingActive = newValue;
		return activeItems;
	}

	#endregion
}