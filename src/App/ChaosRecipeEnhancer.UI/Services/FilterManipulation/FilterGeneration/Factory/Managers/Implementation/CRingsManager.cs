using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CRingsManager : ABaseItemClassManager
{
    public CRingsManager()
    {
        ClassName = "Rings";
        ClassFilterName = "\"Rings\"";
        ClassColor = Settings.Default.LootFilterRingColor;
        AlwaysActive = Settings.Default.LootFilterRingsAlwaysActive;
    }
}