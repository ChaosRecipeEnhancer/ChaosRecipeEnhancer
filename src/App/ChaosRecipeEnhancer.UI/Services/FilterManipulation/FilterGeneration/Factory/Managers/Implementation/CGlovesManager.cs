using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CGlovesManager : ABaseItemClassManager
{
    public CGlovesManager()
    {
        ClassName = "Gloves";
        ClassFilterName = "\"Gloves\"";
        ClassColor = Settings.Default.LootFilterGlovesColor;
        AlwaysActive = Settings.Default.LootFilterGlovesAlwaysActive;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.GlovesActive = newValue;
        return activeItems;
    }
}