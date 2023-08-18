using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.Models;

public class EnhancedItemSetTests
{
    [Fact]
    public void ParameterlessConstructor_WhenCalled_InitialisesEmptyItemSlots()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();

        // Act
        var result = itemSet.EmptyItemSlots;

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(11);
        result.Should().BeEquivalentTo(EmptySlots.Ordered);
    }

    [Fact]
    public void EnhancedItemSetConstructor_GivenSet_CopiesItemsAndEmptySlots()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var item = new EnhancedItem { DerivedItemClass = GameTerminology.Amulets };
        itemSet.Items.Add(item);
        itemSet.EmptyItemSlots.Remove(GameTerminology.Amulets);

        // Act
        var result = new EnhancedItemSet(itemSet);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().Should().BeEquivalentTo(item);
        result.EmptyItemSlots.Should().HaveCount(10);
        result.EmptyItemSlots.Should().NotContain(GameTerminology.Amulets);
    }
    
    [Fact]
    public void TryAddItem_GivenItemNotNeeded_ReturnsFalse()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var item = EnhancedItemHelpers.GetAmuletItemModel();
        itemSet.EmptyItemSlots.Remove(GameTerminology.Amulets);
        
        // Act
        var result = itemSet.TryAddItem(item);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void TryAddItem_GivenTwoHandWeapon_AddsItemAndRemovesTwoHandWeaponSlotAndBothOneHandWeaponSlots()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var item = EnhancedItemHelpers.GetTwoHandedItemModel();
        
        // Act
        var result = itemSet.TryAddItem(item);

        // Assert
        result.Should().BeTrue();
        itemSet.Items.Should().HaveCount(1);
        itemSet.Items.First().Should().BeEquivalentTo(item);
        itemSet.EmptyItemSlots.Should().HaveCount(8);
        itemSet.EmptyItemSlots.Should().NotContain(GameTerminology.TwoHandWeapons);
        itemSet.EmptyItemSlots.Should().NotContain(GameTerminology.OneHandWeapons);
    }
    
    [Fact]
    public void TryAddItem_GivenOneHandWeapon_AddsItemAndRemovesOneHandWeaponSlotAndTwoHandWeaponSlot()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var item = EnhancedItemHelpers.GetOneHandedItemModel();
        
        // Act
        var result = itemSet.TryAddItem(item);

        // Assert
        result.Should().BeTrue();
        itemSet.Items.Should().HaveCount(1);
        itemSet.Items.First().Should().BeEquivalentTo(item);
        itemSet.EmptyItemSlots.Should().HaveCount(9);
        itemSet.EmptyItemSlots.Where(x => x == GameTerminology.OneHandWeapons).Should().HaveCount(1);
        itemSet.EmptyItemSlots.Should().NotContain(GameTerminology.TwoHandWeapons);
    }

    [Fact]
    public void TryAddItem_GivenAmulet_AddsItemAndRemovesAmulet()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var item = EnhancedItemHelpers.GetAmuletItemModel();
        
        // Act
        var result = itemSet.TryAddItem(item);

        // Assert
        result.Should().BeTrue();
        itemSet.Items.Should().HaveCount(1);
        itemSet.Items.First().Should().BeEquivalentTo(item);
        itemSet.EmptyItemSlots.Should().HaveCount(10);
        itemSet.EmptyItemSlots.Should().NotContain(GameTerminology.Amulets);
    }
    
    [Fact]
    public void GetItemDistance_GivenNoItemYetAdded_ReturnsZero()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var item = EnhancedItemHelpers.GetAmuletItemModel();
        
        // Act
        var result = itemSet.GetItemDistance(item);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void GetItemDistance_GivenSameTab_ReturnsDistanceOfHypotenuse()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var item = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.AmuletUrl, 1, 1);
        var item2 = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.AmuletUrl, 4, 5);
        itemSet.TryAddItem(item);
        
        // Act
        var result = itemSet.GetItemDistance(item2);

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void GetItemDistance_GivenSameTab_ReturnsDistanceOfHypotenuseMultipliedByTwoToThePowerOfTabIndexDifference()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var item = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.AmuletUrl, 1, 1, 1);
        var item2 = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.AmuletUrl, 4, 5, 3);
        itemSet.TryAddItem(item);
        
        // Act
        var result = itemSet.GetItemDistance(item2);

        // Assert
        result.Should().Be(20);
    }
    
    [Fact]
    public void IsItemClassNeeded_GivenItemClassNotNeeded_ReturnsFalse()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var item = EnhancedItemHelpers.GetAmuletItemModel();
        itemSet.EmptyItemSlots.Remove(GameTerminology.Amulets);
        
        // Act
        var result = itemSet.IsItemClassNeeded(item);

        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void IsItemClassNeeded_GivenItemClassNeeded_ReturnsTrue()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var item = EnhancedItemHelpers.GetAmuletItemModel();
        
        // Act
        var result = itemSet.IsItemClassNeeded(item);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void OrderItemsForPicking_GivenItemsExist_ReturnsInCorrectOrder()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        var amulet = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.AmuletUrl, 1, 1, 1);
        var bodyArmour = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.BodyArmourUrl, 4, 5, 3);
        var gloves = EnhancedItemHelpers.GetItemWithXYAndTabIndex(ItemIconConstants.GlovesUrl, 2, 2, 2);
        itemSet.TryAddItem(amulet);
        itemSet.TryAddItem(bodyArmour);
        itemSet.TryAddItem(gloves);
        
        // Act
        itemSet.OrderItemsForPicking();

        // Assert
        itemSet.Items.Should().HaveCount(3);
        itemSet.Items[0].Should().BeEquivalentTo(bodyArmour);
        itemSet.Items[1].Should().BeEquivalentTo(gloves);
        itemSet.Items[2].Should().BeEquivalentTo(amulet);
    }
}