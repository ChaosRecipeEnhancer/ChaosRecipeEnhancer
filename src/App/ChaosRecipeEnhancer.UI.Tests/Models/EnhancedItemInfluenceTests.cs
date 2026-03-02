using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.Models;

public class EnhancedItemInfluenceTests
{
    #region IsInfluenced Tests

    [Fact]
    public void IsInfluenced_NoInfluences_ReturnsFalse()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences(), ItemIconConstants.AmuletUrl);

        // Act
        var result = item.IsInfluenced;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsInfluenced_ShaperInfluence_ReturnsTrue()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.IsInfluenced;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsInfluenced_NullInfluences_ReturnsFalse()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences(), ItemIconConstants.AmuletUrl);
        item.BaseItemInfluences = null;

        // Act
        var result = item.IsInfluenced;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsInfluenced_DualInfluence_ReturnsTrue()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true, Elder = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.IsInfluenced;

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region GetInfluenceTypes Tests

    [Fact]
    public void GetInfluenceTypes_NoInfluences_ReturnsEmptyList()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences(), ItemIconConstants.AmuletUrl);

        // Act
        var result = item.GetInfluenceTypes();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void GetInfluenceTypes_SingleInfluence_ReturnsSingleType()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.GetInfluenceTypes();

        // Assert
        result.Should().ContainSingle().Which.Should().Be(InfluenceType.Shaper);
    }

    [Fact]
    public void GetInfluenceTypes_DualInfluence_ReturnsBothTypes()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true, Elder = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.GetInfluenceTypes();

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(InfluenceType.Shaper);
        result.Should().Contain(InfluenceType.Elder);
    }

    [Fact]
    public void GetInfluenceTypes_AllInfluences_ReturnsSixTypes()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences
        {
            Shaper = true,
            Elder = true,
            Crusader = true,
            Redeemer = true,
            Hunter = true,
            Warlord = true
        }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.GetInfluenceTypes();

        // Assert
        result.Should().HaveCount(6);
        result.Should().Contain(InfluenceType.Shaper);
        result.Should().Contain(InfluenceType.Elder);
        result.Should().Contain(InfluenceType.Crusader);
        result.Should().Contain(InfluenceType.Redeemer);
        result.Should().Contain(InfluenceType.Hunter);
        result.Should().Contain(InfluenceType.Warlord);
    }

    [Fact]
    public void GetInfluenceTypes_NullInfluences_ReturnsEmptyList()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences(), ItemIconConstants.AmuletUrl);
        item.BaseItemInfluences = null;

        // Act
        var result = item.GetInfluenceTypes();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region HasInfluenceType Tests

    [Fact]
    public void HasInfluenceType_MatchingType_ReturnsTrue()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.HasInfluenceType(InfluenceType.Shaper);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasInfluenceType_NonMatchingType_ReturnsFalse()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.HasInfluenceType(InfluenceType.Elder);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasInfluenceType_NullInfluences_ReturnsFalse()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences(), ItemIconConstants.AmuletUrl);
        item.BaseItemInfluences = null;

        // Act
        var result = item.HasInfluenceType(InfluenceType.Shaper);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasInfluenceType_None_ReturnsFalse()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 65, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.HasInfluenceType(InfluenceType.None);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region IsExaltedRecipeEligible Tests

    [Fact]
    public void IsExaltedRecipeEligible_InfluencedLevel60_ReturnsTrue()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 60, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.IsExaltedRecipeEligible;

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsExaltedRecipeEligible_InfluencedLevel59_ReturnsFalse()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 59, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Shaper = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.IsExaltedRecipeEligible;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsExaltedRecipeEligible_UninfluencedLevel60_ReturnsFalse()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 60, ItemFrameType.Rare, 0, 0, new BaseItemInfluences(), ItemIconConstants.AmuletUrl);

        // Act
        var result = item.IsExaltedRecipeEligible;

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsExaltedRecipeEligible_InfluencedLevel100_ReturnsTrue()
    {
        // Arrange
        var item = new EnhancedItem(1, 1, false, 100, ItemFrameType.Rare, 0, 0, new BaseItemInfluences { Elder = true }, ItemIconConstants.AmuletUrl);

        // Act
        var result = item.IsExaltedRecipeEligible;

        // Assert
        result.Should().BeTrue();
    }

    #endregion
}
