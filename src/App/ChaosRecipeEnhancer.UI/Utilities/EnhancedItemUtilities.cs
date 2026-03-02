using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using System.Collections.Generic;

namespace ChaosRecipeEnhancer.UI.Utilities;

/// <summary>
/// Provides utility methods for filtering and processing EnhancedItem objects.
/// </summary>
public static class EnhancedItemUtilities
{
    public static List<EnhancedItem> FilterItemsForRecipe(List<EnhancedItem> unfilteredStashContents)
    {
        var filteredItems = new List<EnhancedItem>();

        // iterate through each item in the provided list
        foreach (var item in unfilteredStashContents)
        {
            // The derived class is not what we're looking for
            // (think rare maps, rare jewels, etc... NOT 'gear')
            if (item.DerivedItemClass == null)
            {
                continue;
            }

            // Skip non-rare items always
            if (item.FrameType != ItemFrameType.Rare)
            {
                continue;
            }

            // If the item is identified and the user has chosen to exclude identified items, skip it.
            if (item.Identified && !Settings.Default.IncludeIdentifiedItemsEnabled)
            {
                continue;
            }

            var activeRecipeType = (RecipeType)Settings.Default.ActiveRecipeType;

            switch (activeRecipeType)
            {
                case RecipeType.ChaosOrb:
                    if (item.ItemLevel >= 60) filteredItems.Add(item);
                    break;
                case RecipeType.RegalOrb:
                    if (item.ItemLevel >= 75) filteredItems.Add(item);
                    break;
                case RecipeType.OrbOfChance:
                    if (item.ItemLevel >= 1 && item.ItemLevel <= 59) filteredItems.Add(item);
                    break;
                case RecipeType.ExaltedOrb:
                    // Exalted Orb Recipe requires influenced rares with iLvl 60+.
                    if (item.ItemLevel >= 60 && item.IsInfluenced) filteredItems.Add(item);
                    break;
            }
        }

        return filteredItems;
    }
}
