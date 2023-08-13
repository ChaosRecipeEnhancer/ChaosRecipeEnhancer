using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class COneHandWeaponsManager : ABaseItemClassManager
{
    public COneHandWeaponsManager()
    {
        ClassName = "OneHandWeapons";
        ClassFilterName = "\"One Hand\"";
        ClassColor = Settings.Default.LootFilterWeaponColor;
        AlwaysActive = Settings.Default.LootFilterWeaponsAlwaysActive;
    }

    public override string SetBaseType()
    {
        // Seems like we omit claws by design as they don't fit the sizing rules (2 x 2 = 4 stash units)
        var baseType = "Class ";
        baseType += "\"Daggers\" \"One Hand Axes\" \"One Hand Maces\" \"One Hand Swords\" \"Rune Daggers\" \"Sceptres\" \"Thrusting One Hand Swords\" \"Wands\"";
        baseType += StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Width <= 1" +
                    StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Height <= 3";

        return baseType;
    }
}