using System;
using System.Collections.Generic;
using System.Linq;
using ChaosRecipeEnhancer.UI.Constants;

namespace ChaosRecipeEnhancer.UI.Models;

/// <summary>
/// This is an Item Set that will only ever have 1 of each item class.
/// In the case of one-handed weapons or rings, a pair of each will be needed.
/// </summary>
public class EnhancedItemSet
{
    public EnhancedItemSet()
    {
        EmptyItemSlots = new List<string>
        {
            GameTerminology.BodyArmor,
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
    }

    public EnhancedItemSet(EnhancedItemSet other)
    {
        Items = other.Items;
        EmptyItemSlots = other.EmptyItemSlots;
    }

    public List<EnhancedItem> Items { get; } = new();
    public List<string> EmptyItemSlots { get; }
    
    /// <summary>
    /// This will attempt to add an item. If it is not needed, it will not be added.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool AddItem(EnhancedItem item)
    {
        if (!EmptyItemSlots.Contains(item.DerivedItemClass))
        {
            return false;
        }

        switch (item.DerivedItemClass)
        {
            case "OneHandWeapons":
                _ = EmptyItemSlots.Remove("TwoHandWeapons");
                break;
            case "TwoHandWeapons":
                _ = EmptyItemSlots.Remove("OneHandWeapons");
                _ = EmptyItemSlots.Remove("OneHandWeapons");
                break;
        }

        _ = EmptyItemSlots.Remove(item.DerivedItemClass);

        Items.Add(item);

        return true;
    }

    public double GetItemDistance(EnhancedItem item)
    {
        var lastItemAdded = Items.LastOrDefault();
        return lastItemAdded is null
            ? 0
            : Math.Sqrt(Math.Pow(item.X - lastItemAdded.X, 2) + Math.Pow(item.Y - lastItemAdded.Y, 2))
              * Math.Pow(2, Math.Abs(item.StashTabIndex - lastItemAdded.StashTabIndex));
    }

    public bool IsItemClassNeeded(EnhancedItem item) => EmptyItemSlots.Contains(item.DerivedItemClass);
}