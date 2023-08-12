using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;

namespace ChaosRecipeEnhancer.UI.Tests.DynamicControls.StashTabs;

public class ItemSetTests
{
    private readonly List<string> _initialEmptyItemSlots = new() { "BodyArmours", "TwoHandWeapons", "OneHandWeapons", "OneHandWeapons", "Helmets", "Gloves", "Boots", "Belts", "Rings", "Rings", "Amulets" };

    [Fact]
    public void DefaultConstructor_InitialisesEmptyListAndDefaultEmptyItemSlots()
    {
        // Arrange & Act
        var itemSet = new ItemSet();
        
        // Assert
        itemSet.Should().NotBeNull();
        itemSet.EmptyItemSlots.Should().BeEquivalentTo(_initialEmptyItemSlots);
    }

    [Fact]
    public void ItemSetConstructor_GivenItemSet_ClonesItemSet()
    {
        // Arrange
        var itemSet = new ItemSet();
        itemSet.EmptyItemSlots.Clear();
        itemSet.EmptyItemSlots.Add("Test");
        
        // Act
        var clonedItemSet = new ItemSet(itemSet);
        
        // Assert
        clonedItemSet.Should().NotBeNull();
        clonedItemSet.EmptyItemSlots.Should().BeEquivalentTo(itemSet.EmptyItemSlots);
    }

    [Fact]
    public void AddItem_ItemTypeNotInList_ReturnsFalse()
    {
        // Arrange
        var itemSet = new ItemSet();
        var item = new Item { ItemType = "Test" };
        
        // Act
        var result = itemSet.AddItem(item);
        
        // Assert
        result.Should().BeFalse();
    }
    
    [Fact]
    public void AddItem_GivenOneHandWeapon_RemovesTwoHandAndOneHandWeapon()
    {
        // Arrange
        var itemSet = new ItemSet();
        var item = new Item { ItemType = "OneHandWeapons" };
        
        // Clone expected
        var expectedEmptyItemSlots = _initialEmptyItemSlots.ToList();
        expectedEmptyItemSlots.Remove("TwoHandWeapons");
        expectedEmptyItemSlots.Remove("OneHandWeapons");
        
        // Act
        var result = itemSet.AddItem(item);
        
        // Assert
        itemSet.EmptyItemSlots.Should().BeEquivalentTo(expectedEmptyItemSlots);
        result.Should().BeTrue();
    }

    [Fact]
    public void AddItem_GivenTwoHandWeapon_RemovesTwoHandAndTwoOneHandWeapon()
    {
        // Arrange
        var itemSet = new ItemSet();
        var item = new Item { ItemType = "TwoHandWeapons" };
        
        // Act
        var result = itemSet.AddItem(item);
        
        // Assert
        itemSet.EmptyItemSlots.Should().BeEquivalentTo(_initialEmptyItemSlots.Where(x => x != "TwoHandWeapons" && x != "OneHandWeapons"));
        result.Should().BeTrue();
    }

    [Fact]
    public void AddItem_GivenHelmet_AddsItemToList()
    {
        // Arrange
        var itemSet = new ItemSet();
        var item = new Item { ItemType = "Helmets" };
        
        // Act
        var result = itemSet.AddItem(item);
        
        // Assert
        itemSet.EmptyItemSlots.Should().BeEquivalentTo(_initialEmptyItemSlots.Where(x => x != "Helmets"));
        itemSet.ItemList.Should().Contain(item);
        result.Should().BeTrue();
    }

    [Fact]
    public void GetItemDistance_GivenNoPreviousItem_Returns0()
    {
        // Arrange
        var itemSet = new ItemSet();
        var item = new Item { ItemType = "Helmets" };
        
        // Act
        var result = itemSet.GetItemDistance(item);
        
        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void GetItemDistance_GivenPreviousItem_CalculatesDistanceUsingPythagoras()
    {
        // Arrange
        var itemSet = new ItemSet();
        var item = new Item { ItemType = "Helmets", x = 3, y = 4 };
        var previousItem = new Item { ItemType = "Helmets", x = 0, y = 0 };
        itemSet.AddItem(previousItem);
        
        // Act
        var result = itemSet.GetItemDistance(item);
        
        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public void NeedsItem_ContainsItemType_ReturnsTrue()
    {
        // Arrange
        var itemSet = new ItemSet();
        var item = new Item { ItemType = "Helmets" };
        
        // Act
        var result = itemSet.NeedsItem(item);
        
        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void NeedsItem_DoesNotContainItemType_ReturnsFalse()
    {
        // Arrange
        var itemSet = new ItemSet();
        var item = new Item { ItemType = "Test" };
        
        // Act
        var result = itemSet.NeedsItem(item);
        
        // Assert
        result.Should().BeFalse();
    }
}