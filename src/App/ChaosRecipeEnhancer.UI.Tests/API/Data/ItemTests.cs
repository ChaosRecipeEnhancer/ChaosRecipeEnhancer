using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.API.Data;

public class EnhancedItemTests
{
    [Theory]
    [ClassData(typeof(ItemClassData))]
    public void GetItemClass_GivenURL_AssignsExpectedToItemType(string url, string expected)
    {
        // Arrange
        var item = new EnhancedItem { Icon = url };

        // Act
        item.GetItemClass();

        // Assert
        item.DerivedItemClass.Should().Be(expected);
    }

    [Fact]
    public void GetItemClass_GivenShieldURL_ReturnsOneHandWeapons()
    {
        // Arrange
        var item = new EnhancedItem
        {
            Icon = ItemIconConstants.ShieldUrl
        };

        // Act
        item.GetItemClass();

        // Assert
        item.DerivedItemClass.Should().Be(GameTerminology.OneHandWeapons);
    }
}