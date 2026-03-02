namespace ChaosRecipeEnhancer.UI.Models.Enums;

/// <summary>
/// Represents the influence types that can be present on an item.
/// Used for identifying item-level influence (e.g., Shaper, Elder).
/// </summary>
public enum InfluenceType
{
    None = 0,
    Shaper = 1,
    Elder = 2,
    Crusader = 3,
    Redeemer = 4,
    Hunter = 5,
    Warlord = 6
}

/// <summary>
/// Represents the user's target influence selection for the Exalted Orb recipe.
/// Auto mode tries all 6 influence types and picks the best.
/// </summary>
public enum TargetInfluenceSelection
{
    Auto = 0,
    Shaper = 1,
    Elder = 2,
    Crusader = 3,
    Redeemer = 4,
    Hunter = 5,
    Warlord = 6
}
