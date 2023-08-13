using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CTwoHandWeaponsManager : ABaseItemClassManager
{
    public CTwoHandWeaponsManager()
    {
        ClassName = "TwoHandWeapons";
        ClassFilterName = "\"Two Hand\"";
        ClassColor = Settings.Default.LootFilterWeaponColor;
        AlwaysActive = Settings.Default.LootFilterWeaponsAlwaysActive;
    }

    public override string SetBaseType()
    {
        var baseType = "Class ";
        baseType += "\"Two Hand Swords\" \"Two Hand Axes\" \"Two Hand Maces\" \"Staves\" \"Warstaves\" \"Bows\"";
        baseType += StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Width <= 2" +
                    StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Height <= 3";

        return baseType;
    }
}