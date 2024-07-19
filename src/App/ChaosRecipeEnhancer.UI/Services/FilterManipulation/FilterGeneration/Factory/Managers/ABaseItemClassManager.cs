using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers;

public abstract class ABaseItemClassManager
{
    public string ClassName { get; set; }
    public string ClassFilterName { get; set; }

    public int FontSize { get; set; }
    public string FontColor { get; set; }
    public string BorderColor { get; set; }

    // 'Primary' Color (Used for Filter Background Color + Set Tracker Color)
    public string ClassColor { get; set; }

    public bool MapIconEnabled { get; set; }
    public int MapIconColor { get; set; }
    public int MapIconSize { get; set; }
    public int MapIconShape { get; set; }

    public bool BeamEnabled { get; set; }
    public bool BeamTemporary { get; set; }
    public int BeamColor { get; set; }

    public bool AlwaysActive { get; set; }
    public bool AlwaysHidden { get; set; }

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
