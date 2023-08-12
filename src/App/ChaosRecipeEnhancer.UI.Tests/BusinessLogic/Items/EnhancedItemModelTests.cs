using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Tests.API.Data;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.BusinessLogic.Items;

public class EnhancedItemModelTests
{
    [Theory]
    [ClassData(typeof(ItemClassData))]
    public void GetItemClass_GivenURL_AssignsExpectedToItemType(string url, string expected)
    {
        // Arrange
        var item = new EnhancedItemModel(1, 1, false, null, 0, 0, 0, new ItemInfluencesModel(), url);

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