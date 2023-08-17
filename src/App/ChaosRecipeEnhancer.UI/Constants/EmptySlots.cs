﻿using System.Collections.Generic;
using System.Linq;

namespace ChaosRecipeEnhancer.UI.Constants;

// Not sure on the naming of this - best I could come up with
public static class EmptySlots
{
    public static List<string> Ordered
    {
        get
        {
            List<string> originalList = new()
            {
                GameTerminology.BodyArmors,
                GameTerminology.TwoHandWeapons,
                GameTerminology.Helmets,
                GameTerminology.Gloves,
                GameTerminology.Boots,
                GameTerminology.Belts,
                GameTerminology.Amulets,
                GameTerminology.Rings, // have to double up on rings
                GameTerminology.Rings,
                GameTerminology.OneHandWeapons, // have to double up on 1-handers
                GameTerminology.OneHandWeapons
            };
        
            // Return a copy to ensure that the original is not mutable
            return originalList.ToList();
        }
    }
}