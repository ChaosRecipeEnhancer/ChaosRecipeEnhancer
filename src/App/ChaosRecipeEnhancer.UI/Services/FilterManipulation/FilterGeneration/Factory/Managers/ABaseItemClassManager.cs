using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers;

public abstract class ABaseItemClassManager
{
    public string ClassName { get; set; }
    public string ClassFilterName { get; set; }
    public string ClassColor { get; set; }
    public bool AlwaysActive { get; set; }

    public virtual string SetBaseType()
    {
        var baseType = "Class " + ClassFilterName;
        return baseType;
    }

    public virtual string SetBaseType(bool lootFilterSpaceSavingHideLargeWeapons)
    {
        var baseType = "Class " + ClassFilterName;
        return baseType;
    }

    public virtual string SetBaseType(
        bool lootFilterSpaceSavingHideLargeWeapons,
        bool lootFilterSpaceSavingHideOffHand
    )
    {
        var baseType = "Class " + ClassFilterName;
        return baseType;
    }

    public virtual bool CheckIfMissing(HashSet<string> missingItemClasses)
    {
        return missingItemClasses.Contains(ClassName);
    }

    public abstract ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue);

}