using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;

namespace ChaosRecipeEnhancer.UI.Tests.DynamicControls.StashTabs;

public class InteractiveStashTabCellTests
{
    [Fact]
    public void Constructor_GivenValues_InitialisesItem()
    {
        // Arrange & Act
        var cell = new InteractiveStashTabCell(5, 6);
        
        // Assert
        cell.Should().NotBeNull();
        cell.XIndex.Should().Be(5);
        cell.YIndex.Should().Be(6);
    }

    [Fact]
    public void Activate_GivenItem_SetsActiveTrueAndSetsItem()
    {
        // Arrange
        var cell = new InteractiveStashTabCell(5, 6);
        var itemToAdd = new Item();
        
        // Act
        cell.Activate(ref itemToAdd);
        
        // Assert
        cell.Active.Should().BeTrue();
        cell.Item.Should().Be(itemToAdd);
    }

    [Fact]
    public void Deactivate_AfterActivate_SetsActiveFalseAndItemNull()
    {
        // Arrange
        var cell = new InteractiveStashTabCell(5, 6);
        var itemToAdd = new Item();
        cell.Activate(ref itemToAdd);
        
        // Act
        cell.Deactivate();
        
        // Assert
        cell.Active.Should().BeFalse();
        cell.Item.Should().BeNull();
    }
}