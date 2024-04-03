using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Models;

// Not sure on the naming of this - best I could come up with
public static class EmptySlots
{
    public static List<string> Ordered
    {
        get
        {
            List<string> originalList =
            [
                GameTerminology.BodyArmors,
                GameTerminology.OneHandWeapons, // have to double up on 1-handers
                GameTerminology.OneHandWeapons,
                GameTerminology.TwoHandWeapons,
                GameTerminology.Helmets,
                GameTerminology.Gloves,
                GameTerminology.Boots,
                GameTerminology.Belts,
                GameTerminology.Amulets,
                GameTerminology.Rings, // have to double up on rings
                GameTerminology.Rings

            ];

            // Return a copy to ensure that the original is not mutable
            return [.. originalList];
        }
    }
}