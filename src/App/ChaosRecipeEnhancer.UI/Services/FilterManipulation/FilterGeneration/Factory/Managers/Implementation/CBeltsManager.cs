using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CBeltsManager : ABaseItemClassManager
{
    public CBeltsManager()
    {
        ClassName = "Belts";
        ClassFilterName = "\"Belts\"";
        ClassColor = Settings.Default.LootFilterBeltColor;
        AlwaysActive = Settings.Default.LootFilterBeltsAlwaysActive;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.BeltActive = newValue;
        return activeItems;
    }
}