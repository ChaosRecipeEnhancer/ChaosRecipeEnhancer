using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.BusinessLogic.Items;

public class EnhancedItemModelTests
{
    [Theory]
    [ClassData(typeof(ItemClassData))]
    public void GetItemClass_GivenURL_AssignsExpectedToItemType(string url, string expected)
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, null, 0, 0, 0, new BaseItemInfluences(), url);

        // Act
        item.GetItemClass();

        // Assert
        item.DerivedItemClass.Should().Be(expected);
    }

    [Fact]
    public void GetItemClass_GivenShieldURL_AssignsShieldsToDerivedItemClass()
    {
        // Arrange
        var item = EnhancedItemHelpers.GetShieldItemModel();

        // Act
        item.GetItemClass();

        // Assert
        item.DerivedItemClass.Should().Be("Shields");
    }
}