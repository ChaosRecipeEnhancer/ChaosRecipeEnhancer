using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using ChaosRecipeEnhancer.UI.Models.Enums;
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

    [Theory]
    [InlineData(59, false, false)] // Below the threshold for both recipes
    [InlineData(60, true, false)]  // Eligible for Chaos, not for Regal
    [InlineData(74, true, false)]  // Eligible for Chaos, not for Regal
    [InlineData(75, false, true)]  // Eligible for Regal, not for Chaos
    public void ItemEligibility_IsCorrect(int itemLevel, bool expectedChaosEligibility, bool expectedRegalEligibility)
    {
        var item = new EnhancedItem
        {
            ItemLevel = itemLevel
        };

        item.IsChaosRecipeEligible.Should().Be(expectedChaosEligibility);
        item.IsRegalRecipeEligible.Should().Be(expectedRegalEligibility);
    }

    [Theory]
    [InlineData("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQXJtb3Vycy9Cb290cy9Cb290c0RleEludDQiLCJ3IjoyLCJoIjoyLCJzY2FsZSI6MX1d/bad1ba72df/BootsDexInt4.png", "Boots")]
    [InlineData("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQXJtb3Vycy9HbG92ZXMvR2xvdmVzSW50MyIsInciOjIsImgiOjIsInNjYWxlIjoxfV0/abe163b992/GlovesInt3.png", "Gloves")]
    [InlineData("https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvUmluZ3MvVG9wYXpSdWJ5IiwidyI6MSwiaCI6MSwic2NhbGUiOjF9XQ/8878077651/TopazRuby.png", "Rings")]
    public void GetItemClass_SetsDerivedItemClassCorrectly(string iconUrl, string expectedItemClass)
    {
        var item = new EnhancedItem { Icon = iconUrl };
        item.GetItemClass();

        item.DerivedItemClass.Should().Be(expectedItemClass);
    }

    [Fact]
    public void Constructors_InitializePropertiesCorrectly()
    {
        // Using a BaseItem for the copy constructor test
        var baseItem = new BaseItem
        {
            Width = 1,
            Height = 1,
            Identified = true,
            ItemLevel = 70,
            FrameType = ItemFrameType.Magic,
            X = 0,
            Y = 0,
            BaseItemInfluences = new BaseItemInfluences(),
            Icon = "https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQW11bGV0cy9BbXVsZXQ3IiwidyI6MSwiaCI6MSwic2NhbGUiOjF9XQ/58942b1ab3/Amulet7.png"
        };

        var itemFromBase = new EnhancedItem(baseItem);

        // Verifying properties inherited from BaseItem are set
        itemFromBase.Width.Should().Be(1);
        itemFromBase.Height.Should().Be(1);
        itemFromBase.Identified.Should().BeTrue();
        itemFromBase.ItemLevel.Should().Be(70);
        itemFromBase.FrameType.Should().Be(ItemFrameType.Magic);
        itemFromBase.X.Should().Be(0);
        itemFromBase.Y.Should().Be(0);
        itemFromBase.BaseItemInfluences.Should().NotBeNull();
        itemFromBase.Icon.Should().NotBeNullOrWhiteSpace();

        // Verifying GetItemClass was called and set DerivedItemClass
        itemFromBase.DerivedItemClass.Should().NotBeNullOrWhiteSpace();
        itemFromBase.DerivedItemClass.Should().Be(GameTerminology.Amulets);
    }

}