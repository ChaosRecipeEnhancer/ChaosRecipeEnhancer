using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers;

namespace ChaosRecipeEnhancer.UI.Tests.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers;

public class ItemClassManager : ABaseItemClassManager
{
    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        return new ActiveItemTypes();
    }
}