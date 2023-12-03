using System.Windows.Forms.VisualStyles;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Services;

namespace ChaosRecipeEnhancer.UI.Tests.Services;

public class ItemSetManagerServiceTests
{
    [Fact]
    public void UpdateData_GivenNonZeroSelectedTabs_SetsNeedsFetchingFalse_ReturnsTrue()
    {
        // Arrange
        var itemSetManagerService = new ItemSetManagerService();
        

        // Act
        var result = itemSetManagerService.UpdateStashContents(1, new List<int> { 0 }, new List<EnhancedItem>());

        // Assert
        result.Should().BeTrue();
        itemSetManagerService.NeedsFetching.Should().BeFalse();
    }
    
    [Fact]
    public void UpdateData_GivenZeroSelectedTabs_ReturnsFalse()
    {
        // Arrange
        var itemSetManagerService = new ItemSetManagerService();
        
        // Act
        var result = itemSetManagerService.UpdateStashContents(1, new List<int>(), new List<EnhancedItem>());

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void CalculateItemAmounts_GivenFullSet_IncrementsItemAmounts()
    {
        // Arrange
        var itemSetManagerService = new ItemSetManagerService();
        var itemSet = GenerateFullSet();
        itemSetManagerService.UpdateStashContents(1, new List<int> { 0 }, itemSet);
        
        // Act
        itemSetManagerService.CalculateItemAmounts();
        
        // Assert
        itemSetManagerService.AmuletsAmount.Should().Be(1);
        itemSetManagerService.BeltsAmount.Should().Be(1);
        itemSetManagerService.BootsAmount.Should().Be(1);
        itemSetManagerService.ChestsAmount.Should().Be(1);
        itemSetManagerService.GlovesAmount.Should().Be(1);
        itemSetManagerService.HelmetsAmount.Should().Be(1);
        itemSetManagerService.RingsAmount.Should().Be(2);
        itemSetManagerService.WeaponsSmallAmount.Should().Be(2);
    }

    [Fact]
    public void CalculateItemAmounts_GivenTwoHandedWeapon_IncrementsWeaponsBig()
    {
        // Arrange
        var itemSetManagerService = new ItemSetManagerService();
        var itemSet = new List<EnhancedItem> { new() { DerivedItemClass = GameTerminology.TwoHandWeapons, ItemLevel = 70 } };
        itemSetManagerService.UpdateStashContents(1, new List<int> { 0 }, itemSet);
        
        // Act
        itemSetManagerService.CalculateItemAmounts();
        
        // Assert
        itemSetManagerService.WeaponsBigAmount.Should().Be(1);
    }

    [Fact]
    public void CalculateItemAmounts_SetsNeedsFetchingFalse()
    {
        // Arrange
        var itemSetManagerService = new ItemSetManagerService();
        var itemSet = GenerateFullSet();
        itemSetManagerService.UpdateStashContents(1, new List<int> { 0 }, itemSet);
        
        // Act
        itemSetManagerService.CalculateItemAmounts();
        
        // Assert
        itemSetManagerService.NeedsFetching.Should().BeFalse();
    }

    [Fact]
    public void ResetCompletedSets_WhenCalled_SetsCompletedSetCountToZero()
    {
        // Arrange
        var itemSetManagerService = new ItemSetManagerService();
        var itemSet = GenerateFullSet();
        itemSetManagerService.UpdateStashContents(1, new List<int> { 0 }, itemSet);
        itemSetManagerService.CalculateItemAmounts();
        
        // Act
        itemSetManagerService.ResetCompletedSets();
        
        // Assert
        itemSetManagerService.CompletedSetCount.Should().Be(0);
    }

    [Fact]
    public void ResetItemAmounts_WhenCalled_SetsAllItemAmountsToZero()
    {
        // Arrange
        var itemSetManagerService = new ItemSetManagerService();
        var itemSet = GenerateFullSet();
        itemSet.Add(new EnhancedItem { DerivedItemClass = GameTerminology.TwoHandWeapons, ItemLevel = 70 });
        itemSetManagerService.UpdateStashContents(1, new List<int> { 0 }, itemSet);
        itemSetManagerService.CalculateItemAmounts();
        
        // Act
        itemSetManagerService.ResetItemAmounts();
        
        // Assert
        itemSetManagerService.AmuletsAmount.Should().Be(0);
        itemSetManagerService.BeltsAmount.Should().Be(0);
        itemSetManagerService.BootsAmount.Should().Be(0);
        itemSetManagerService.ChestsAmount.Should().Be(0);
        itemSetManagerService.GlovesAmount.Should().Be(0);
        itemSetManagerService.HelmetsAmount.Should().Be(0);
        itemSetManagerService.RingsAmount.Should().Be(0);
        itemSetManagerService.WeaponsSmallAmount.Should().Be(0);
        itemSetManagerService.WeaponsBigAmount.Should().Be(0);
    }
    
    private List<EnhancedItem> GenerateFullSet()
    {
        return new List<EnhancedItem>
        {
            new() { DerivedItemClass = GameTerminology.OneHandWeapons, ItemLevel = 70},
            new() { DerivedItemClass = GameTerminology.OneHandWeapons, ItemLevel = 70 },
            new() { DerivedItemClass = GameTerminology.BodyArmors, ItemLevel = 70 },
            new() { DerivedItemClass = GameTerminology.Helmets, ItemLevel = 70 },
            new() { DerivedItemClass = GameTerminology.Gloves, ItemLevel = 70 },
            new() { DerivedItemClass = GameTerminology.Boots, ItemLevel = 70 },
            new() { DerivedItemClass = GameTerminology.Belts, ItemLevel = 70 },
            new() { DerivedItemClass = GameTerminology.Amulets, ItemLevel = 70 },
            new() { DerivedItemClass = GameTerminology.Rings, ItemLevel = 70 },
            new() { DerivedItemClass = GameTerminology.Rings, ItemLevel = 70 },
        };
    }
}