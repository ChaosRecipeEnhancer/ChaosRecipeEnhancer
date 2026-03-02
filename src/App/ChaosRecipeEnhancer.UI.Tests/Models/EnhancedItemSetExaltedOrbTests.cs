using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.Models;

public class EnhancedItemSetExaltedOrbTests
{
    #region TryAddItem ExaltedOrb Tests

    [Fact]
    public void TryAddItem_ExaltedOrb_AcceptsInfluencedItemLevel60()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        itemSet.RequiredInfluenceType = InfluenceType.Shaper;
        var item = new EnhancedItem(1, 1, false, 60, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = itemSet.TryAddItem(item, RecipeType.ExaltedOrb);

        // Assert
        result.Should().BeTrue();
        itemSet.Items.Should().Contain(item);
    }

    [Fact]
    public void TryAddItem_ExaltedOrb_RejectsItemLevel59()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        itemSet.RequiredInfluenceType = InfluenceType.Shaper;
        var item = new EnhancedItem(1, 1, false, 59, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = itemSet.TryAddItem(item, RecipeType.ExaltedOrb);

        // Assert
        result.Should().BeFalse();
        itemSet.Items.Should().NotContain(item);
    }

    [Fact]
    public void TryAddItem_ExaltedOrb_RejectsWrongInfluenceType()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        itemSet.RequiredInfluenceType = InfluenceType.Elder;
        var item = new EnhancedItem(1, 1, false, 60, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = itemSet.TryAddItem(item, RecipeType.ExaltedOrb);

        // Assert
        result.Should().BeFalse();
        itemSet.Items.Should().NotContain(item);
    }

    [Fact]
    public void TryAddItem_ExaltedOrb_AcceptsDualInfluenceMatchingRequired()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        itemSet.RequiredInfluenceType = InfluenceType.Elder;
        var item = new EnhancedItem(1, 1, false, 60, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true, Elder = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = itemSet.TryAddItem(item, RecipeType.ExaltedOrb);

        // Assert
        result.Should().BeTrue();
        itemSet.Items.Should().Contain(item);
    }

    [Fact]
    public void TryAddItem_ExaltedOrb_AcceptsWhenRequiredInfluenceIsNone()
    {
        // Arrange - RequiredInfluenceType defaults to None, so no influence check is performed
        var itemSet = new EnhancedItemSet();
        var item = new EnhancedItem(1, 1, false, 60, ItemFrameType.Rare, 0, 0, new BaseItemInfluences(), ItemIconConstants.AmuletUrl);

        // Act
        var result = itemSet.TryAddItem(item, RecipeType.ExaltedOrb);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region RequiredInfluenceType Tests

    [Fact]
    public void RequiredInfluenceType_DefaultsToNone()
    {
        // Arrange & Act
        var itemSet = new EnhancedItemSet();

        // Assert
        itemSet.RequiredInfluenceType.Should().Be(InfluenceType.None);
    }

    #endregion

    #region IsExaltedRecipeEligible Tests

    [Fact]
    public void IsExaltedRecipeEligible_FullSetAllSameInfluence_ReturnsTrue()
    {
        // Arrange
        var itemSet = new EnhancedItemSet();
        itemSet.RequiredInfluenceType = InfluenceType.Shaper;

        // Build a complete set using a two-handed weapon (fills TwoHandWeapons + both OneHandWeapons slots)
        itemSet.TryAddItem(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.BodyArmourUrl), RecipeType.ExaltedOrb);
        itemSet.TryAddItem(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.StaffUrl), RecipeType.ExaltedOrb);
        itemSet.TryAddItem(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.HelmetUrl), RecipeType.ExaltedOrb);
        itemSet.TryAddItem(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.GlovesUrl), RecipeType.ExaltedOrb);
        itemSet.TryAddItem(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.BootsUrl), RecipeType.ExaltedOrb);
        itemSet.TryAddItem(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.BeltUrl), RecipeType.ExaltedOrb);
        itemSet.TryAddItem(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl), RecipeType.ExaltedOrb);
        itemSet.TryAddItem(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.RingUrl), RecipeType.ExaltedOrb);
        itemSet.TryAddItem(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.RingUrl), RecipeType.ExaltedOrb);

        // Act
        var result = itemSet.IsExaltedRecipeEligible;

        // Assert
        itemSet.EmptyItemSlots.Should().BeEmpty();
        itemSet.Items.Should().HaveCount(9);
        result.Should().BeTrue();
    }

    [Fact]
    public void IsExaltedRecipeEligible_IncompleteSet_ReturnsFalse()
    {
        // Arrange - set with items where not all match the required influence type
        var itemSet = new EnhancedItemSet();
        itemSet.RequiredInfluenceType = InfluenceType.Shaper;

        // Manually add items — one has the wrong influence, making the set ineligible
        itemSet.Items.Add(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl));
        itemSet.Items.Add(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.RingUrl));
        itemSet.Items.Add(new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Elder = true }, ItemIconConstants.HelmetUrl));

        // Act
        var result = itemSet.IsExaltedRecipeEligible;

        // Assert
        result.Should().BeFalse();
    }

    #endregion
}
