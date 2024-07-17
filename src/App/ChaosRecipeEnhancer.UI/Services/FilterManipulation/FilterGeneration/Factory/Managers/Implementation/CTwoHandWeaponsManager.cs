using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Properties;
using System.Collections.Generic;

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

    public override string SetBaseType(bool lootFilterSpaceSavingHideLargeWeapons)
    {
        var baseType = "Class ";
        baseType += "\"Two Hand Swords\" \"Two Hand Axes\" \"Two Hand Maces\" \"Staves\" \"Warstaves\" \"Bows\"";

        // Additional size restrictions if space saving is enabled
        if (lootFilterSpaceSavingHideLargeWeapons)
        {
            baseType +=
                StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Width <= 2" +
                StringConstruction.NewLineCharacter + StringConstruction.TabCharacter + "Height <= 3";
        }

        return baseType;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.WeaponActive = newValue;
        return activeItems;
    }

    public override bool CheckIfMissing(HashSet<string> missingItemClasses)
    {
        // bad, dont like, no good ideas for now tho
        return missingItemClasses.Contains(ClassName) || missingItemClasses.Contains("OneHandWeapons");
    }

}