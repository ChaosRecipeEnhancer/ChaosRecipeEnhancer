using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CAmuletsManager : ABaseItemClassManager
{
    public CAmuletsManager()
    {
        ClassName = "Amulets";
        ClassFilterName = "\"Amulets\"";
        ClassColor = Settings.Default.LootFilterAmuletColor;
        AlwaysActive = Settings.Default.LootFilterAmuletsAlwaysActive;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.AmuletActive = newValue;
        return activeItems;
    }
}