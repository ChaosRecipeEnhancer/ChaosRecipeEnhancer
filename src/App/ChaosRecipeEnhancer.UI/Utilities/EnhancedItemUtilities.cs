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
            // if it's not rare ignore item (could keep identified items if passed as true)
            // maybe i could optimize here by preemptively removing all non-rare items
            // in the calling request? idk if it would truly 'optimize' or if it would
            // just offload the work to another service lol
            if ((item.Identified && !Settings.Default.IncludeIdentifiedItemsEnabled) || item.FrameType != ItemFrameType.Rare)
            {
                continue;
            }

            // if the derived class is not what we're looking for
            // (think rare maps, rare jewels, etc... NOT 'gear')
            if (item.DerivedItemClass == null)
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
            }
        }

        return filteredItems;
    }
}
