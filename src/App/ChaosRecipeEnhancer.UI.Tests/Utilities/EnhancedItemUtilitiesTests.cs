using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Tests.Helpers;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities;

public class EnhancedItemUtilitiesTests
{
    public EnhancedItemUtilitiesTests()
    {
        // Reset settings to known state
        Settings.Default.ChaosRecipeTrackingEnabled = true;
        Settings.Default.IncludeIdentifiedItemsEnabled = false;
    }

    #region Chaos Recipe Filtering

    [Fact]
    public void FilterItemsForRecipe_ChaosRecipeEnabled_ReturnsRareUnidentifiedItemsAbove60()
    {
        // Arrange
        Settings.Default.ChaosRecipeTrackingEnabled = true;
        Settings.Default.IncludeIdentifiedItemsEnabled = false;

        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(identified: false, itemLevel: 65, frameType: ItemFrameType.Rare, iconUrl: ItemIconConstants.AmuletUrl),
            CreateEnhancedItem(identified: false, itemLevel: 70, frameType: ItemFrameType.Rare, iconUrl: ItemIconConstants.RingUrl),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public void FilterItemsForRecipe_ChaosRecipeEnabled_ExcludesItemsBelow60()
    {
        // Arrange
        Settings.Default.ChaosRecipeTrackingEnabled = true;

        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(identified: false, itemLevel: 59, frameType: ItemFrameType.Rare, iconUrl: ItemIconConstants.AmuletUrl),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterItemsForRecipe_ChaosRecipeEnabled_IncludesLevel60()
    {
        // Arrange
        Settings.Default.ChaosRecipeTrackingEnabled = true;

        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(identified: false, itemLevel: 60, frameType: ItemFrameType.Rare, iconUrl: ItemIconConstants.AmuletUrl),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().HaveCount(1);
    }

    #endregion

    #region Regal Recipe Filtering

    [Fact]
    public void FilterItemsForRecipe_RegalRecipeEnabled_ReturnsOnlyItemsAbove75()
    {
        // Arrange
        Settings.Default.ChaosRecipeTrackingEnabled = false;

        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(identified: false, itemLevel: 80, frameType: ItemFrameType.Rare, iconUrl: ItemIconConstants.AmuletUrl),
            CreateEnhancedItem(identified: false, itemLevel: 74, frameType: ItemFrameType.Rare, iconUrl: ItemIconConstants.RingUrl),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().HaveCount(1);
        result[0].ItemLevel.Should().Be(80);
    }

    [Fact]
    public void FilterItemsForRecipe_RegalRecipeEnabled_ExcludesItemsBelow75()
    {
        // Arrange
        Settings.Default.ChaosRecipeTrackingEnabled = false;

        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(identified: false, itemLevel: 74, frameType: ItemFrameType.Rare, iconUrl: ItemIconConstants.AmuletUrl),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Identified Item Filtering

    [Fact]
    public void FilterItemsForRecipe_IdentifiedItem_ExcludedWhenSettingDisabled()
    {
        // Arrange
        Settings.Default.ChaosRecipeTrackingEnabled = true;
        Settings.Default.IncludeIdentifiedItemsEnabled = false;

        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(identified: true, itemLevel: 65, frameType: ItemFrameType.Rare, iconUrl: ItemIconConstants.AmuletUrl),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterItemsForRecipe_IdentifiedItem_IncludedWhenSettingEnabled()
    {
        // Arrange
        Settings.Default.ChaosRecipeTrackingEnabled = true;
        Settings.Default.IncludeIdentifiedItemsEnabled = true;

        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(identified: true, itemLevel: 65, frameType: ItemFrameType.Rare, iconUrl: ItemIconConstants.AmuletUrl),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().HaveCount(1);
    }

    #endregion

    #region Non-Rare and Null Class Filtering

    [Fact]
    public void FilterItemsForRecipe_NonRareItem_AlwaysExcluded()
    {
        // Arrange
        Settings.Default.ChaosRecipeTrackingEnabled = true;

        var items = new List<EnhancedItem>
        {
            CreateEnhancedItem(identified: false, itemLevel: 65, frameType: ItemFrameType.Magic, iconUrl: ItemIconConstants.AmuletUrl),
            CreateEnhancedItem(identified: false, itemLevel: 65, frameType: ItemFrameType.Normal, iconUrl: ItemIconConstants.RingUrl),
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterItemsForRecipe_NullDerivedClass_Excluded()
    {
        // Arrange
        Settings.Default.ChaosRecipeTrackingEnabled = true;

        var item = new EnhancedItem
        {
            Identified = false,
            ItemLevel = 65,
            FrameType = ItemFrameType.Rare,
            DerivedItemClass = null
        };

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(new List<EnhancedItem> { item });

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterItemsForRecipe_EmptyList_ReturnsEmpty()
    {
        // Arrange
        var items = new List<EnhancedItem>();

        // Act
        var result = EnhancedItemUtilities.FilterItemsForRecipe(items);

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Helper Methods

    private static EnhancedItem CreateEnhancedItem(bool identified, int itemLevel, ItemFrameType frameType, string iconUrl)
    {
        return new EnhancedItem(1, 1, identified, itemLevel, frameType, 0, 0, new BaseItemInfluences(), iconUrl);
    }

    #endregion
}
