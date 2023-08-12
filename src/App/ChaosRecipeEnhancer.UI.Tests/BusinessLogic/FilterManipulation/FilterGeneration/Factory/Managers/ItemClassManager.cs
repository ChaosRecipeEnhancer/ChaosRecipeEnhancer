using ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers;
using ChaosRecipeEnhancer.UI.BusinessLogic.Items;

namespace ChaosRecipeEnhancer.UI.Tests.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers;

public class ItemClassManager : ABaseItemClassManager
{
    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        return new ActiveItemTypes();
    }
}