using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Tests.Helpers;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities;

[Collection("EnhancedItemUtilities")]
public class EnhancedItemUtilitiesExaltedOrbTests
{

    [Fact]
    public void FilterItemsForRecipe_ExaltedOrb_IncludesInfluencedLevel60()
    {
        // Arrange
        Settings.Default.ActiveRecipeType = (int)RecipeType.ExaltedOrb;
        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(false, 60, ItemFrameType.Rare, ItemIconConstants.AmuletUrl, new BaseItemInfluences { Shaper = true }),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void FilterItemsForRecipe_ExaltedOrb_IncludesInfluencedLevel80()
    {
        // Arrange
        Settings.Default.ActiveRecipeType = (int)RecipeType.ExaltedOrb;
        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(false, 80, ItemFrameType.Rare, ItemIconConstants.AmuletUrl, new BaseItemInfluences { Elder = true }),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void FilterItemsForRecipe_ExaltedOrb_ExcludesUninfluencedLevel60()
    {
        // Arrange
        Settings.Default.ActiveRecipeType = (int)RecipeType.ExaltedOrb;
        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(false, 60, ItemFrameType.Rare, ItemIconConstants.AmuletUrl),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterItemsForRecipe_ExaltedOrb_ExcludesInfluencedLevel59()
    {
        // Arrange
        Settings.Default.ActiveRecipeType = (int)RecipeType.ExaltedOrb;
        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(false, 59, ItemFrameType.Rare, ItemIconConstants.AmuletUrl, new BaseItemInfluences { Shaper = true }),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterItemsForRecipe_ExaltedOrb_ExcludesNonRare()
    {
        // Arrange
        Settings.Default.ActiveRecipeType = (int)RecipeType.ExaltedOrb;
        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(false, 60, ItemFrameType.Magic, ItemIconConstants.AmuletUrl, new BaseItemInfluences { Shaper = true }),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterItemsForRecipe_ExaltedOrb_IncludesDualInfluenced()
    {
        // Arrange
        Settings.Default.ActiveRecipeType = (int)RecipeType.ExaltedOrb;
        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(false, 65, ItemFrameType.Rare, ItemIconConstants.AmuletUrl, new BaseItemInfluences { Shaper = true, Elder = true }),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public void FilterItemsForRecipe_ExaltedOrb_MixedItems_FiltersCorrectly()
    {
        // Arrange
        Settings.Default.ActiveRecipeType = (int)RecipeType.ExaltedOrb;
        Settings.Default.IncludeIdentifiedItemsEnabled = false;
        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(false, 60, ItemFrameType.Rare, ItemIconConstants.AmuletUrl, new BaseItemInfluences { Shaper = true }),    // eligible
            CreateEnhancedItem(false, 80, ItemFrameType.Rare, ItemIconConstants.RingUrl, new BaseItemInfluences { Elder = true }),        // eligible
            CreateEnhancedItem(false, 59, ItemFrameType.Rare, ItemIconConstants.HelmetUrl, new BaseItemInfluences { Shaper = true }),     // excluded - level too low
            CreateEnhancedItem(false, 65, ItemFrameType.Rare, ItemIconConstants.BeltUrl),                                                // excluded - no influence
            CreateEnhancedItem(false, 65, ItemFrameType.Magic, ItemIconConstants.BootsUrl, new BaseItemInfluences { Shaper = true }),     // excluded - not rare
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(item => item.IsInfluenced && item.ItemLevel >= 60);
    }

    #region Helper Methods

    private static EnhancedItem CreateEnhancedItem(bool identified, int itemLevel, ItemFrameType frameType, string iconUrl, BaseItemInfluences influences = null)
    {
        return new EnhancedItem(1, 1, identified, itemLevel, frameType, 0, 0, influences ?? new BaseItemInfluences(), iconUrl);
    }

    #endregion
}
