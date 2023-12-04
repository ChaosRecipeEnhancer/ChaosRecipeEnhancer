using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.Models;

public class EnhancedItemTests
{
    [Theory]
    [ClassData(typeof(ItemClassData))]
    public void GetItemClass_GivenURL_AssignsExpectedToItemType(string url, string expected)
    {
        // Arrange
        var item = new EnhancedItem(
            1,
            1,
            false,
            null,
            0,
            0,
            0,
            new BaseItemInfluences(),
            url
        );

        // Act
        item.GetItemClass();

        // Assert
        item.DerivedItemClass.Should().Be(expected);
    }

    [Theory]
    [InlineData(59, false)]
    [InlineData(60, true)]
    [InlineData(74, true)]
    [InlineData(75, false)]
    public void IsChaosRecipeEligible_GivenItemLevel_ReturnsExpected(int itemLevel, bool expected)
    {
        // Arrange
        var item = new EnhancedItem(
            1,
            1,
            false,
            itemLevel,
            0,
            0,
            0,
            new BaseItemInfluences(),
            ItemIconConstants.AmuletUrl
        );

        // Act
        var result = item.IsChaosRecipeEligible;

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void BaseItemConstructor_GivenValidBaseItem_SetsDerivedItemClass()
    {
        // Arrange
        var baseItem = new BaseItem();
        baseItem.Icon = ItemIconConstants.AmuletUrl;

        // Act
        var result = new EnhancedItem(baseItem);

        // Assert
        result.DerivedItemClass.Should().Be(GameTerminology.Amulets);
    }
}