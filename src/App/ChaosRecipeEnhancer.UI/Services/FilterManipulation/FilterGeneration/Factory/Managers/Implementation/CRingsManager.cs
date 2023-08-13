using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

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