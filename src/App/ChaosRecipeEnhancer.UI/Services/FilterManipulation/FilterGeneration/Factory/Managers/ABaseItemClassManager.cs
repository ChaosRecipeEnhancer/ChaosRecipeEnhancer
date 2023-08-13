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
}