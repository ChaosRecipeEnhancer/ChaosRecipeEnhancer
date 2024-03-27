using ChaosRecipeEnhancer.UI.Constants;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChaosRecipeEnhancer.UI.Models;

/// <summary>
/// Represents a set of items for the Chaos or Regal vendor recipe in Path of Exile.
/// Ensures a complete set can be formed with one item per slot, with exceptions for rings and one-handed weapons.
/// </summary>
public class EnhancedItemSet
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EnhancedItemSet"/> class with empty item slots based on the game's requirements.
    /// </summary>
    public EnhancedItemSet()
    {
        EmptyItemSlots = EmptySlots.Ordered;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnhancedItemSet"/> class, copying the items and empty slots from another instance.
    /// </summary>
    /// <param name="other">The other instance of <see cref="EnhancedItemSet"/> to copy from.</param>
    public EnhancedItemSet(EnhancedItemSet other)
    {
        Items = other.Items;
        EmptyItemSlots = other.EmptyItemSlots;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this set contains at least one item eligible for the Chaos recipe.
    /// Chaos Recipe Item Sets only require 1 of their items to be between 60 and 74; the rest can be any level above 60.
    /// </summary>
    public bool HasChaosRecipeQualifier { get; set; }

    /// <summary>
    /// Gets a value indicating whether all items in this set are eligible for the Regal recipe.
    /// Regal Recipe Item Sets require all items to be 75 or higher.
    /// </summary>
    public bool IsRegalRecipeEligible => Items.All(d => d.IsRegalRecipeEligible);

    /// <summary>
    /// Gets or sets the collection of items currently in this set.
    /// </summary>
    public List<EnhancedItem> Items { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of item slots that are currently empty and need to be filled to complete the set.
    /// </summary>
    public List<string> EmptyItemSlots { get; set; }

    /// <summary>
    /// Attempts to add an item to the set if it is needed for the recipe being targeted.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <param name="regalRecipe">Whether the Regal recipe is being targeted.</param>
    /// <returns><c>true</c> if the item was added; otherwise, <c>false</c>.</returns>
    public bool TryAddItem(EnhancedItem item, bool regalRecipe)
    {
        // If we're targeting a Regal Recipe, we need to ensure all items are 75 or higher.
        if (regalRecipe)
        {
            // If the item is not eligible for the Regal Recipe, we can't add it.
            if (!item.IsRegalRecipeEligible) return false;
        }
        // If we're targeting a Chaos Recipe, we need to ensure at least one item is between 60 and 74.
        else
        {
            // If the item is not eligible for the Chaos Recipe, we can't add it.
            if (!(item.ItemLevel.Value >= 60)) return false;
        }

        if (!EmptyItemSlots.Contains(item.DerivedItemClass))
        {
            return false;
        }

        // we need to include this block (i know it looks confusing)
        // if we get a one handed weapon, we need to remove the two handed weapon slot
        // as we're only going to be targeting another one-handed going forward
        // if we get a two handed weapon, we need to remove both one handed weapon slots
        switch (item.DerivedItemClass)
        {
            case "OneHandWeapons":
                _ = EmptyItemSlots.Remove(GameTerminology.TwoHandWeapons);
                break;
            case "TwoHandWeapons":
                _ = EmptyItemSlots.Remove(GameTerminology.OneHandWeapons);
                _ = EmptyItemSlots.Remove(GameTerminology.OneHandWeapons);
                break;
        }

        _ = EmptyItemSlots.Remove(item.DerivedItemClass);

        Items.Add(item);

        return true;
    }

    /// <summary>
    /// Calculates the distance between the last item added to the set and the given item, considering their positions and stash tabs.
    /// </summary>
    /// <param name="item">The item to calculate the distance to.</param>
    /// <returns>The calculated distance as a <see cref="double"/>.</returns>
    public double GetItemDistance(EnhancedItem item)
    {
        var lastItemAdded = Items.LastOrDefault();
        return lastItemAdded is null
            ? 0
            : Math.Sqrt(Math.Pow(item.X - lastItemAdded.X, 2) + Math.Pow(item.Y - lastItemAdded.Y, 2))
              * Math.Pow(2, Math.Abs(item.StashTabIndex - lastItemAdded.StashTabIndex));
    }

    /// <summary>
    /// Checks if the given item's class is needed to complete the set.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns><c>true</c> if the item class is needed; otherwise, <c>false</c>.</returns>
    public bool IsItemClassNeeded(EnhancedItem item) => EmptyItemSlots.Contains(item.DerivedItemClass);

    /// <summary>
    /// Orders the items in the set based on a predefined priority of item classes.
    /// </summary>
    public void OrderItemsForPicking()
    {
        var orderedClasses = EmptySlots.Ordered;

        Items = [.. Items.OrderBy(d => orderedClasses.IndexOf(d.DerivedItemClass))];
    }
}