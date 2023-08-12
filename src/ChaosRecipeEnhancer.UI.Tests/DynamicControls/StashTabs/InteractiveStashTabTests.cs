using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.DynamicControls.StashTabs;

public class InteractiveStashTabTests
{
    private readonly InteractiveStashTab _baseTab = new("TestTab", 0, new Uri("https://www.test.com"));
    private readonly int[] _fullSetAmounts = { 2, 1, 1, 1, 2, 1, 1, 1, 1 };

    #region Amounts And Active
    [Fact]
    public void RingsAmount_ShowAmountNeededFalse_ReturnsRingAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.RingsAmount;

        // Assert
        result.Should().Be(_fullSetAmounts[0]);
    }

    [Fact]
    public void RingsAmount_FullSetThresholdReached_ReturnsZero()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.RingsAmount;

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void RingsAmount_FullSetThresholdNotReached_ReturnsFullSetThresholdDoubledMinusRingsAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 3;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.RingsAmount;

        // Assert
        result.Should().Be(4);
    }
    
    [Fact]
    public void RingsActive_NeedsItemFetchTrue_ReturnsTrue()
    {
        // Arrange
        _baseTab.NeedsItemFetch = true;
        
        // Act
        var result = _baseTab.RingsActive;

        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void RingsActive_ItemNeedsFetchFalse_ReturnsFullSetThresholdDoubledMinusRingsAmount(int threshold, bool expected)
    {
        // Arrange
        _baseTab.NeedsItemFetch = false;
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = threshold;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.RingsActive;

        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void AmuletsAmount_ShowAmountNeededFalse_ReturnsAmuletAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.AmuletsAmount;

        // Assert
        result.Should().Be(_fullSetAmounts[1]);
    }
    
    [Fact]
    public void AmuletsAmount_FullSetThresholdReached_ReturnsZero()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.AmuletsAmount;

        // Assert
        result.Should().Be(0);
    }
    
    [Fact]
    public void AmuletsAmount_FullSetThresholdNotReached_ReturnsFullSetThresholdMinusAmuletsAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 3;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.AmuletsAmount;

        // Assert
        result.Should().Be(2);
    }
    
    [Fact]
    public void AmuletsActive_NeedsItemFetchTrue_ReturnsTrue()
    {
        // Arrange
        _baseTab.NeedsItemFetch = true;
        
        // Act
        var result = _baseTab.AmuletsActive;

        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void AmuletsActive_ItemNeedsFetchFalse_ReturnsFullSetThresholdMinusAmuletsAmount(int threshold, bool expected)
    {
        // Arrange
        _baseTab.NeedsItemFetch = false;
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = threshold;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.AmuletsActive;

        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void BeltsAmount_ShowAmountNeededFalse_ReturnsBeltAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.BeltsAmount;

        // Assert
        result.Should().Be(_fullSetAmounts[2]);
    }
    
    [Fact]
    public void BeltsAmount_FullSetThresholdReached_ReturnsZero()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.BeltsAmount;

        // Assert
        result.Should().Be(0);
    }
    
    [Fact]
    public void BeltsAmount_FullSetThresholdNotReached_ReturnsFullSetThresholdMinusBeltsAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 3;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.BeltsAmount;

        // Assert
        result.Should().Be(2);
    }
    
    [Fact]
    public void BeltsActive_NeedsItemFetchTrue_ReturnsTrue()
    {
        // Arrange
        _baseTab.NeedsItemFetch = true;
        
        // Act
        var result = _baseTab.BeltsActive;

        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void BeltsActive_ItemNeedsFetchFalse_ReturnsFullSetThresholdMinusBeltsAmount(int threshold, bool expected)
    {
        // Arrange
        _baseTab.NeedsItemFetch = false;
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = threshold;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.BeltsActive;

        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void ChestsAmount_ShowAmountNeededFalse_ReturnsChestAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.ChestsAmount;

        // Assert
        result.Should().Be(_fullSetAmounts[3]);
    }
    
    [Fact]
    public void ChestsAmount_FullSetThresholdReached_ReturnsZero()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.ChestsAmount;

        // Assert
        result.Should().Be(0);
    }
    
    [Fact]
    public void ChestsAmount_FullSetThresholdNotReached_ReturnsFullSetThresholdMinusChestsAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 3;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.ChestsAmount;

        // Assert
        result.Should().Be(2);
    }
    
    [Fact]
    public void ChestsActive_NeedsItemFetchTrue_ReturnsTrue()
    {
        // Arrange
        _baseTab.NeedsItemFetch = true;
        
        // Act
        var result = _baseTab.ChestsActive;

        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void ChestsActive_ItemNeedsFetchFalse_ReturnsFullSetThresholdMinusChestsAmount(int threshold, bool expected)
    {
        // Arrange
        _baseTab.NeedsItemFetch = false;
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = threshold;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.ChestsActive;

        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void WeaponsAmount_ShowAmountNeededFalse_ReturnsWeaponAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.WeaponsAmount;

        // Assert
        result.Should().Be(_fullSetAmounts[4] + _fullSetAmounts[5] * 2);
    }
    
    [Fact]
    public void WeaponsAmount_FullSetThresholdReached_ReturnsZero()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.WeaponsAmount;

        // Assert
        result.Should().Be(0);
    }
    
    [Fact]
    public void WeaponsAmount_FullSetThresholdNotReached_ReturnsFullSetThresholdMinusWeaponAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 3;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.WeaponsAmount;

        // Assert
        result.Should().Be(2);
    }
    
    
    [Fact]
    public void WeaponsActive_NeedsItemFetchTrue_ReturnsTrue()
    {
        // Arrange
        _baseTab.NeedsItemFetch = true;
        
        // Act
        var result = _baseTab.WeaponsActive;

        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, false)]
    [InlineData(3, true)]
    public void WeaponsActive_ItemNeedsFetchFalse_ReturnsFullSetThresholdMinusWeaponAmount(int threshold, bool expected)
    {
        // Arrange
        _baseTab.NeedsItemFetch = false;
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = threshold;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.WeaponsActive;

        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void GlovesAmount_ShowAmountNeededFalse_ReturnsGloveAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.GlovesAmount;

        // Assert
        result.Should().Be(_fullSetAmounts[6]);
    }
    
    [Fact]
    public void GlovesAmount_FullSetThresholdReached_ReturnsZero()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.GlovesAmount;

        // Assert
        result.Should().Be(0);
    }
    
    [Fact]
    public void GlovesAmount_FullSetThresholdNotReached_ReturnsFullSetThresholdMinusGloveAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 3;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.GlovesAmount;

        // Assert
        result.Should().Be(2);
    }
    
    [Fact]
    public void GlovesActive_NeedsItemFetchTrue_ReturnsTrue()
    {
        // Arrange
        _baseTab.NeedsItemFetch = true;
        
        // Act
        var result = _baseTab.GlovesActive;

        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void GlovesActive_ItemNeedsFetchFalse_ReturnsFullSetThresholdMinusGloveAmount(int threshold, bool expected)
    {
        // Arrange
        _baseTab.NeedsItemFetch = false;
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = threshold;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.GlovesActive;

        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void HelmetsAmount_ShowAmountNeededFalse_ReturnsHelmetAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.HelmetsAmount;

        // Assert
        result.Should().Be(_fullSetAmounts[7]);
    }
    
    [Fact]
    public void HelmetsAmount_FullSetThresholdReached_ReturnsZero()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.HelmetsAmount;

        // Assert
        result.Should().Be(0);
    }
    
    [Fact]
    public void HelmetsAmount_FullSetThresholdNotReached_ReturnsFullSetThresholdMinusHelmetAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 3;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.HelmetsAmount;

        // Assert
        result.Should().Be(2);
    }
    
    [Fact]
    public void HelmetsActive_NeedsItemFetchTrue_ReturnsTrue()
    {
        // Arrange
        _baseTab.NeedsItemFetch = true;
        
        // Act
        var result = _baseTab.HelmetsActive;

        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void HelmetsActive_ItemNeedsFetchFalse_ReturnsFullSetThresholdMinusHelmetAmount(int threshold, bool expected)
    {
        // Arrange
        _baseTab.NeedsItemFetch = false;
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = threshold;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.HelmetsActive;

        // Assert
        result.Should().Be(expected);
    }
    
    [Fact]
    public void BootsAmount_ShowAmountNeededFalse_ReturnsBootAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.BootsAmount;

        // Assert
        result.Should().Be(_fullSetAmounts[8]);
    }
    
    [Fact]
    public void BootsAmount_FullSetThresholdReached_ReturnsZero()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 1;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.BootsAmount;

        // Assert
        result.Should().Be(0);
    }
    
    [Fact]
    public void BootsAmount_FullSetThresholdNotReached_ReturnsFullSetThresholdMinusBootAmount()
    {
        // Arrange
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = 3;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.BootsAmount;

        // Assert
        result.Should().Be(2);
    }
    
    [Fact]
    public void BootsActive_NeedsItemFetchTrue_ReturnsTrue()
    {
        // Arrange
        _baseTab.NeedsItemFetch = true;
        
        // Act
        var result = _baseTab.BootsActive;

        // Assert
        result.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(1, false)]
    [InlineData(2, true)]
    public void BootsActive_ItemNeedsFetchFalse_ReturnsFullSetThresholdMinusBootAmount(int threshold, bool expected)
    {
        // Arrange
        _baseTab.NeedsItemFetch = false;
        Settings.Default.SetTrackerOverlayItemCounterDisplayMode = 2;
        Settings.Default.FullSetThreshold = threshold;
        SetFullSetAmounts();
        
        // Act
        var result = _baseTab.BootsActive;

        // Assert
        result.Should().Be(expected);
    }
    #endregion

    [Fact]
    public void FilterItemsForChaosRecipe_QuadFalse_Creates12by12Grid()
    {
        // Arrange
        var items = new List<Item>();
        
        // Act
        _baseTab.FilterItemsForChaosRecipe(items);

        // Assert
        _baseTab.OverlayCellsList.Count.Should().Be((int)Math.Pow(12, 2));
    }

    [Fact]
    public void FilterItemsForChaosRecipe_QuadTrue_Creates24by24Grid()
    {
        // Arrange
        var items = new List<Item>();
        _baseTab.Quad = true;
        
        // Act
        _baseTab.FilterItemsForChaosRecipe(items);

        // Assert
        _baseTab.OverlayCellsList.Count.Should().Be((int)Math.Pow(24, 2));
    }

    [Fact]
    public void FilterItemsForChaosRecipe_GivenUnidentifiedItemIlvl60_AddsItemToItemsForChaosRecipe()
    {
        // Arrange
        var item = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 60
        };
        
        // Act
        _baseTab.FilterItemsForChaosRecipe(new List<Item> { item });
        
        // Assert
        _baseTab.ItemsForChaosRecipe.Should().Contain(item);
    }
    
    [Fact]
    public void FilterItemsForChaosRecipe_GivenUnidentifiedItemIlvl74_AddsItemToItemsForChaosRecipe()
    {
        // Arrange
        var item = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74
        };
        
        // Act
        _baseTab.FilterItemsForChaosRecipe(new List<Item> { item });
        
        // Assert
        _baseTab.ItemsForChaosRecipe.Should().Contain(item);
    }
    
    [Fact]
    public void FilterItemsForChaosRecipe_GivenUnidentifiedItemIlvl100_DoesNotAddItemToItemsForChaosRecipe()
    {
        // Arrange
        var item = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 100
        };
        
        // Act
        _baseTab.FilterItemsForChaosRecipe(new List<Item> { item });
        
        // Assert
        _baseTab.ItemsForChaosRecipe.Should().BeEmpty();
    }

    [Fact]
    public void FilterItemsForChaosRecipe_GivenEmptyListAfterNonEmptyList_ClearsItemsForChaosRecipe()
    {
        // Arrange
        var item = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74
        };
        _baseTab.FilterItemsForChaosRecipe(new List<Item> { item });
        
        // Act
        _baseTab.FilterItemsForChaosRecipe(new List<Item>());
        
        // Assert
        _baseTab.ItemsForChaosRecipe.Should().BeEmpty();
    }

    [Fact]
    public void FilterItemForChaosRecipe_FrameTypeNot2_Continues()
    {
        // Arrange
        var itemWrongFrameType = new Item
        {
            frameType = 3,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74
        };
        
        var itemCorrectFrameType = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74
        };
        
        // Act
        _baseTab.FilterItemsForChaosRecipe(new List<Item> { itemWrongFrameType, itemCorrectFrameType });
        
        // Assert
        _baseTab.ItemsForChaosRecipe.Should().Contain(itemCorrectFrameType);
        _baseTab.ItemsForChaosRecipe.Should().NotContain(itemWrongFrameType);
    }

    [Fact]
    public void FilterItemForChaosRecipe_SettingsIncludeIdentifiedFalse_Continues()
    {
        // Arrange
        Settings.Default.IncludeIdentifiedItemsEnabled = false;
        var itemIdentified = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74,
            identified = true
        };
        
        var itemUnidentified = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74,
            identified = false
        };
        
        // Act
        _baseTab.FilterItemsForChaosRecipe(new List<Item> { itemIdentified, itemUnidentified });
        
        // Assert
        _baseTab.ItemsForChaosRecipe.Should().Contain(itemUnidentified);
        _baseTab.ItemsForChaosRecipe.Should().NotContain(itemIdentified);
    }

    [Fact]
    public void FilterItemForChaosRecipe_SettingsIncludeIdentifiedTrue_AddsItem()
    {
        // Arrange
        Settings.Default.IncludeIdentifiedItemsEnabled = true;
        var itemIdentified = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74,
            identified = true
        };
        
        var itemUnidentified = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74,
            identified = false
        };
        
        // Act
        _baseTab.FilterItemsForChaosRecipe(new List<Item> { itemIdentified, itemUnidentified });
        
        // Assert
        _baseTab.ItemsForChaosRecipe.Should().Contain(itemUnidentified);
        _baseTab.ItemsForChaosRecipe.Should().Contain(itemIdentified);
    }

    [Fact]
    public void DeactivateItemCells_NoItemPassed_DeactivatesAllCells()
    {
        // Arrange
        var item = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74
        };
        _baseTab.FilterItemsForChaosRecipe(new List<Item> { item });
        
        // Act
        _baseTab.DeactivateItemCells();
        
        // Assert
        _baseTab.OverlayCellsList.Should().OnlyContain(x => !x.Active);
    }

    [Fact]
    public void DeactivateItemCells_ItemPassedAfterActivation_DeactivatesItemCells()
    {
        // Arrange
        var item1 = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74,
            w = 2,
            h = 2,
            x = 0,
            y = 0
        };
        
        var item2 = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74,
            w = 2,
            h = 2,
            x = 2,
            y = 1
        };
        _baseTab.FilterItemsForChaosRecipe(new List<Item> { item1, item2 });
        _baseTab.ActivateItemCells(item1);
        _baseTab.ActivateItemCells(item2);
        
        // Act
        _baseTab.DeactivateItemCells(item1);
        
        // Assert
        _baseTab.OverlayCellsList.Count(x => x.Active).Should().Be(4);
    }

    [Fact]
    public void ActivateItemCells_ItemPassed_ActivatesItemCells()
    {
        // Arrange
        var item = new Item
        {
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl,
            ilvl = 74,
            w = 2,
            h = 2,
            x = 0,
            y = 0
        };
        _baseTab.FilterItemsForChaosRecipe(new List<Item> { item });
        
        // Act
        _baseTab.ActivateItemCells(item);
        
        // Assert
        _baseTab.OverlayCellsList.Count(x => x.Active).Should().Be(4);
    }
    
    private void SetFullSetAmounts()
    {
        _baseTab.UpdateAmounts(_fullSetAmounts);
    }
}