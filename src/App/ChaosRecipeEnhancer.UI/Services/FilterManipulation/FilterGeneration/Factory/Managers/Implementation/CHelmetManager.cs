using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CHelmetManager : ABaseItemClassManager
{
    public CHelmetManager()
    {
        ClassName = "Helmets";
        ClassFilterName = "\"Helmets\"";
        ClassColor = Settings.Default.LootFilterHelmetColor;
        AlwaysActive = Settings.Default.LootFilterHelmetsAlwaysActive;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.HelmetActive = newValue;
        return activeItems;
    }
}