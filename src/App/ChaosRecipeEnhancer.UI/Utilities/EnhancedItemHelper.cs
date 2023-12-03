using System.Collections.Generic;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Utilities;

public static class EnhancedItemHelper
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

            // if an item falls within the ilvl bounds for whichever recipe we're calling
            // chaos recipe ilvl 60+
            if (item.ItemLevel >= 60)
            {
                // simple check if item is in our tabs
                // checks like this make me want to filter before we get here, save some cycles
                filteredItems.Add(item);
            }
        }

        return filteredItems;
    }
}