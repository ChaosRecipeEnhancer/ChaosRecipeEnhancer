using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.API.Data;

public class ItemTests
{
    [Theory]
    [ClassData(typeof(ItemClassData))]
    public void GetItemClass_GivenURL_AssignsExpectedToItemType(string url, string expected)
    {
        // Arrange
        var item = new Item { icon = url };

        // Act
        item.GetItemClass();

        // Assert
        item.ItemType.Should().Be(expected);
    }

    [Fact]
    public void GetItemClass_GivenShieldURL_ReturnsOneHandWeapons()
    {
        // Arrange
        var item = new Item
        {
            icon =
                ItemIconConstants.ShieldUrl
        };

        // Act
        item.GetItemClass();

        // Assert
        item.ItemType.Should().Be("OneHandWeapons");
    }
}