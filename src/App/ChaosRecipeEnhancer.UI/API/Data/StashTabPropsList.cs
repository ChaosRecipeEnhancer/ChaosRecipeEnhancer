using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Api.Data;

// property names from api
public class StashTabProps
{
    public string n { get; set; }
    public int i { get; set; }

    public override string ToString()
    {
        return $"[Index: {i}] {n}";
    }
}

public class StashTabPropsList
{
    public List<StashTabProps> tabs { get; set; }
}