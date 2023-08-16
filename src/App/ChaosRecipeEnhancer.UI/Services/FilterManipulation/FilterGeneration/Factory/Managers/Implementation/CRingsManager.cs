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

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.RingActive = newValue;
        return activeItems;
    }
}