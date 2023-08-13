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
}