using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.BusinessLogic.Items;

public class ItemSetTests
{
    private readonly List<string> orderedList = new()
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

    [Fact]
    public void EmptyItemSlots_WhenInitialized_HasCorrectValues()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();

        // Act
        var emptyItemSlots = itemSet.EmptyItemSlots;

        // Assert
        emptyItemSlots.Count.Should().Be(11);
        emptyItemSlots.Should().BeEquivalentTo(orderedList);
    }

    [Fact]
    public void AddItem_GivenOneHandedWeapon_RemovesWeaponTypesFromEmptyItemSlots()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var itemModel = EnhancedItemHelpers.GetOneHandedItemModel();
        // Clone the list
        var expectedList = orderedList.ToList();
        expectedList.Remove("OneHandWeapons");
        expectedList.Remove("TwoHandWeapons");

        // Act
        itemSet.TryAddItem(itemModel);

        // Assert
        itemSet.EmptyItemSlots.Count.Should().Be(9);
        itemSet.EmptyItemSlots.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public void AddItem_GivenTwoHandedWeapon_RemovesAllWeaponTypesFromEmptyItemSlots()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var itemModel = EnhancedItemHelpers.GetTwoHandedItemModel();
        // Clone the list
        var expectedList = orderedList.ToList();
        expectedList.Remove("OneHandWeapons");
        expectedList.Remove("OneHandWeapons");
        expectedList.Remove("TwoHandWeapons");

        // Act
        itemSet.TryAddItem(itemModel);

        // Assert
        itemSet.EmptyItemSlots.Count.Should().Be(8);
        itemSet.EmptyItemSlots.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public void AddItem_GivenAmulet_RemovesAmuletFromEmptyItemSlots()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var itemModel = EnhancedItemHelpers.GetAmuletItemModel();
        // Clone the list
        var expectedList = orderedList.ToList();
        expectedList.Remove("Amulets");

        // Act
        itemSet.TryAddItem(itemModel);

        // Assert
        itemSet.EmptyItemSlots.Count.Should().Be(10);
        itemSet.EmptyItemSlots.Should().BeEquivalentTo(expectedList);
    }

    [Fact]
    public void AddItem_GivenItem_AddsToItemList()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var itemModel = EnhancedItemHelpers.GetAmuletItemModel();

        // Act
        itemSet.TryAddItem(itemModel);

        // Assert
        itemSet.Items.Count.Should().Be(1);
        itemSet.Items[0].Should().BeEquivalentTo(itemModel);
    }

    [Fact]
    public void GetItemDistance_GivenDifferentTabIndex_Returns40()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var initialItemModel = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.AmuletUrl, 1, 1, 0);
        var itemModel = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.RingUrl, 1, 1, 1);

        itemSet.TryAddItem(initialItemModel);

        // Act
        var distance = itemSet.GetItemDistance(itemModel);

        // Assert
        distance.Should().Be(40);
    }

    [Fact]
    public void GetItemDistance_GivenTwoDifferentPositions_ReturnsHypotenuseBetweenPositions()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var initialItemModel = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.AmuletUrl, 2, 2, 0);
        var itemModel = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.RingUrl, 5, 6, 0);

        itemSet.TryAddItem(initialItemModel);

        // Act
        var distance = itemSet.GetItemDistance(itemModel);

        // Assert
        distance.Should().Be(5);
    }

    // [Fact]
    // public void IsValidItem_GivenItemClassNotAdded_ReturnsTrue()
    // {
    //     // Arrange
    //     var itemSet = new EnhancedItemSet();
    //     var itemModel = EnhancedItemHelpers.GetAmuletItemModel();
    //
    //     // Act
    //     var isValid = itemSet.IsValidItem(itemModel);
    //
    //     // Assert
    //     isValid.Should().BeTrue();
    // }
    //
    // [Fact]
    // public void IsValidItem_GivenItemClassAlreadyAdded_ReturnsFalse()
    // {
    //     // Arrange
    //     var itemSet = new EnhancedItemSet();
    //     var itemModel = EnhancedItemHelpers.GetAmuletItemModel();
    //
    //     itemSet.TryAddItem(itemModel);
    //
    //     // Act
    //     var isValid = itemSet.IsValidItem(itemModel);
    //
    //     // Assert
    //     isValid.Should().BeFalse();
    // }
    //
    // [Fact]
    // public void GetNextItemClass_GivenNothingAdded_ReturnsBodyArmours()
    // {
    //     // Arrange
    //     var itemSet = new EnhancedItemSet();
    //
    //     // Act
    //     var nextItemClass = itemSet.GetNextItemClass();
    //
    //     // Assert
    //     nextItemClass.Should().Be("BodyArmours");
    // }
    //
    // [Fact]
    // public void GetItemClass_GivenBodyArmourAdded_ReturnsTwoHandWeapons()
    // {
    //     // Arrange
    //     var itemSet = new EnhancedItemSet();
    //     var itemModel = EnhancedItemHelpers.GetBodyArmourModel();
    //     itemSet.TryAddItem(itemModel);
    //
    //     // Act
    //     var nextItemClass = itemSet.GetNextItemClass();
    //
    //     // Assert
    //     nextItemClass.Should().Be("TwoHandWeapons");
    // }
}