using System;
using System.Collections.Generic;
using System.Linq;
using ChaosRecipeEnhancer.UI.Api.Data;

namespace ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;

public class ItemSet
{
    public ItemSet()
    {
        ItemList = new List<Item>();
        EmptyItemSlots = new List<string>
        {
            "BodyArmours",
            "TwoHandWeapons",
            "OneHandWeapons",
            "OneHandWeapons",
            "Helmets",
            "Gloves",
            "Boots",
            "Belts",
            "Rings",
            "Rings",
            "Amulets"
        };
    }

    public ItemSet(ItemSet other)
    {
        ItemList = other.ItemList;
        EmptyItemSlots = other.EmptyItemSlots;
    }

    public List<Item> ItemList { get; }
    public List<string> EmptyItemSlots { get; }

    public bool AddItem(Item item)
    {
        if (!EmptyItemSlots.Contains(item.ItemType)) return false;

        if (item.ItemType == "OneHandWeapons")
        {
            _ = EmptyItemSlots.Remove("TwoHandWeapons");
        }
        else if (item.ItemType == "TwoHandWeapons")
        {
            _ = EmptyItemSlots.Remove("OneHandWeapons");
            _ = EmptyItemSlots.Remove("OneHandWeapons");
        }

        _ = EmptyItemSlots.Remove(item.ItemType);

        ItemList.Add(item);

        return true;
    }

    public double GetItemDistance(Item item)
    {
        var lastItemAdded = ItemList.LastOrDefault();

        // if there are no items gg ez it's done
        return lastItemAdded is null ? 0
            // we will add a significant multiplier for items in different tabs
            // the magnitude of the multiplier will be amplified by items in tabs further from the other tab
            : (Math.Sqrt(Math.Pow(item.x - lastItemAdded.x, 2) + Math.Pow(item.y - lastItemAdded.y, 2)))
              // this was Mario's addition, he's a legend 8^)
              * Math.Abs(lastItemAdded.StashTabIndex - item.StashTabIndex);
    }

    public bool NeedsItem(Item item)
    {
        return EmptyItemSlots.Contains(item.ItemType);
    }
}