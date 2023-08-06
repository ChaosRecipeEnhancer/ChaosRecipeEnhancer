using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

internal class CBeltsManager : ABaseItemClassManager
{
	#region Constructors

	public CBeltsManager()
	{
		ClassName = "Belts";
		ClassFilterName = "\"Belts\"";
		ClassColor = Settings.Default.LootFilterBeltColor;
		AlwaysActive = Settings.Default.LootFilterBeltsAlwaysActive;
	}

	#endregion

	#region Methods

	public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
	{
		activeItems.BeltActive = newValue;
		return activeItems;
	}

	#endregion
}