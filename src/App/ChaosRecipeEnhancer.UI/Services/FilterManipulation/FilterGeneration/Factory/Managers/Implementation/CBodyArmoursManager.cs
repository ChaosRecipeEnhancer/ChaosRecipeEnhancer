using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CBodyArmoursManager : ABaseItemClassManager
{
    public CBodyArmoursManager()
    {
        ClassName = "BodyArmours";
        ClassFilterName = "\"Body Armours\"";
        ClassColor = Settings.Default.LootFilterBodyArmourColor;
        AlwaysActive = Settings.Default.LootFilterBodyArmourAlwaysActive;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.BootsActive = newValue;
        return activeItems;
    }
}