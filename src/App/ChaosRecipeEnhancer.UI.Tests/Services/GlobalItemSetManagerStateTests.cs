using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.Services;

[Collection("GlobalItemSetManagerState")]
public class GlobalItemSetManagerStateTests : IDisposable
{
    public GlobalItemSetManagerStateTests()
    {
        ResetState();
    }

    public void Dispose()
    {
        ResetState();
    }

    private static void ResetState()
    {
        GlobalItemSetManagerState.ResetCompletedSetCountAndItemAmounts();
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = new List<EnhancedItem>();
        GlobalItemSetManagerState.SetsInProgress = new List<EnhancedItemSet>();
        GlobalItemSetManagerState.SetThreshold = 0;
        GlobalItemSetManagerState.NeedsLowerLevel = false;
    }

    #region UpdateStashMetadata

    [Fact]
    public void UpdateStashMetadata_GivenMetadataList_SetsStashTabMetadata()
    {
        // Arrange
        var metadata = new List<UnifiedStashTabMetadata>
        {
            new() { Id = "tab1", Name = "Tab 1", Type = "NormalStash", Index = 0 },
            new() { Id = "tab2", Name = "Tab 2", Type = "QuadStash", Index = 1 }
        };

        // Act
        GlobalItemSetManagerState.UpdateStashMetadata(metadata);

        // Assert
        GlobalItemSetManagerState.StashTabMetadataListStashesResponse.Should().NotBeNull();
        GlobalItemSetManagerState.StashTabMetadataListStashesResponse.Should().HaveCount(2);
        GlobalItemSetManagerState.StashTabMetadataListStashesResponse[0].Id.Should().Be("tab1");
    }

    #endregion

    #region UpdateStashContents

    [Fact]
    public void UpdateStashContents_GivenNoSelectedTabs_ReturnsEarly()
    {
        // Arrange
        var selectedTabIds = new List<string>();
        var items = new List<EnhancedItem> { EnhancedItemHelpers.GetAmuletItemModel() };

        // Act
        GlobalItemSetManagerState.UpdateStashContents(5, selectedTabIds, items);

        // Assert
        GlobalItemSetManagerState.SetThreshold.Should().Be(0);
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe.Should().BeEmpty();
    }

    [Fact]
    public void UpdateStashContents_GivenValidInput_SetsPropertiesCorrectly()
    {
        // Arrange
        var selectedTabIds = new List<string> { "tab1" };
        var items = new List<EnhancedItem> { EnhancedItemHelpers.GetAmuletItemModel() };

        // Act
        GlobalItemSetManagerState.UpdateStashContents(5, selectedTabIds, items);

        // Assert
        GlobalItemSetManagerState.SetThreshold.Should().Be(5);
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe.Should().HaveCount(1);
        GlobalItemSetManagerState.NeedsFetching.Should().BeFalse();
    }

    #endregion

    #region CalculateItemAmounts

    [Fact]
    public void CalculateItemAmounts_GivenMixedItems_CountsAllTypesCorrectly()
    {
        // Arrange
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = new List<EnhancedItem>
        {
            CreateItemWithClass(GameTerminology.Rings),
            CreateItemWithClass(GameTerminology.Rings),
            CreateItemWithClass(GameTerminology.Amulets),
            CreateItemWithClass(GameTerminology.Belts),
            CreateItemWithClass(GameTerminology.BodyArmors),
            CreateItemWithClass(GameTerminology.OneHandWeapons),
            CreateItemWithClass(GameTerminology.TwoHandWeapons),
            CreateItemWithClass(GameTerminology.Gloves),
            CreateItemWithClass(GameTerminology.Helmets),
            CreateItemWithClass(GameTerminology.Boots),
        };

        // Act
        GlobalItemSetManagerState.CalculateItemAmounts();

        // Assert
        GlobalItemSetManagerState.RingsAmount.Should().Be(2);
        GlobalItemSetManagerState.AmuletsAmount.Should().Be(1);
        GlobalItemSetManagerState.BeltsAmount.Should().Be(1);
        GlobalItemSetManagerState.ChestsAmount.Should().Be(1);
        GlobalItemSetManagerState.WeaponsSmallAmount.Should().Be(1);
        GlobalItemSetManagerState.WeaponsBigAmount.Should().Be(1);
        GlobalItemSetManagerState.GlovesAmount.Should().Be(1);
        GlobalItemSetManagerState.HelmetsAmount.Should().Be(1);
        GlobalItemSetManagerState.BootsAmount.Should().Be(1);
        GlobalItemSetManagerState.NeedsFetching.Should().BeFalse();
    }

    [Fact]
    public void CalculateItemAmounts_GivenEmptyList_AllCountsRemainZero()
    {
        // Arrange
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = new List<EnhancedItem>();

        // Act
        GlobalItemSetManagerState.CalculateItemAmounts();

        // Assert
        GlobalItemSetManagerState.RingsAmount.Should().Be(0);
        GlobalItemSetManagerState.AmuletsAmount.Should().Be(0);
        GlobalItemSetManagerState.BeltsAmount.Should().Be(0);
        GlobalItemSetManagerState.ChestsAmount.Should().Be(0);
        GlobalItemSetManagerState.WeaponsSmallAmount.Should().Be(0);
        GlobalItemSetManagerState.WeaponsBigAmount.Should().Be(0);
        GlobalItemSetManagerState.GlovesAmount.Should().Be(0);
        GlobalItemSetManagerState.HelmetsAmount.Should().Be(0);
        GlobalItemSetManagerState.BootsAmount.Should().Be(0);
    }

    #endregion

    #region ResetCompletedSetCountAndItemAmounts

    [Fact]
    public void ResetCompletedSetCountAndItemAmounts_ResetsAllCountersToZero()
    {
        // Arrange - set some non-zero values
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = new List<EnhancedItem>
        {
            CreateItemWithClass(GameTerminology.Rings),
            CreateItemWithClass(GameTerminology.Amulets),
            CreateItemWithClass(GameTerminology.Boots),
        };
        GlobalItemSetManagerState.CalculateItemAmounts();

        // Sanity check
        GlobalItemSetManagerState.RingsAmount.Should().BeGreaterThan(0);

        // Act
        GlobalItemSetManagerState.ResetCompletedSetCountAndItemAmounts();

        // Assert
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(0);
        GlobalItemSetManagerState.RingsAmount.Should().Be(0);
        GlobalItemSetManagerState.AmuletsAmount.Should().Be(0);
        GlobalItemSetManagerState.BeltsAmount.Should().Be(0);
        GlobalItemSetManagerState.ChestsAmount.Should().Be(0);
        GlobalItemSetManagerState.WeaponsSmallAmount.Should().Be(0);
        GlobalItemSetManagerState.WeaponsBigAmount.Should().Be(0);
        GlobalItemSetManagerState.GlovesAmount.Should().Be(0);
        GlobalItemSetManagerState.HelmetsAmount.Should().Be(0);
        GlobalItemSetManagerState.BootsAmount.Should().Be(0);
    }

    #endregion

    #region RetrieveCurrentItemCountsForFilterManipulation

    [Fact]
    public void RetrieveCurrentItemCountsForFilterManipulation_ReturnsCorrectDictionary()
    {
        // Arrange
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = new List<EnhancedItem>
        {
            CreateItemWithClass(GameTerminology.Rings),
            CreateItemWithClass(GameTerminology.Rings),
            CreateItemWithClass(GameTerminology.Helmets),
        };
        GlobalItemSetManagerState.CalculateItemAmounts();

        // Act
        var result = GlobalItemSetManagerState.RetrieveCurrentItemCountsForFilterManipulation();

        // Assert
        result.Should().HaveCount(1);
        var dict = result[0];
        dict[ItemClass.Rings].Should().Be(2);
        dict[ItemClass.Helmets].Should().Be(1);
        dict[ItemClass.Amulets].Should().Be(0);
        dict[ItemClass.Belts].Should().Be(0);
        dict[ItemClass.BodyArmours].Should().Be(0);
        dict[ItemClass.OneHandWeapons].Should().Be(0);
        dict[ItemClass.TwoHandWeapons].Should().Be(0);
        dict[ItemClass.Gloves].Should().Be(0);
        dict[ItemClass.Boots].Should().Be(0);
    }

    #endregion

    #region FlattenStashTabs

    [Fact]
    public void FlattenStashTabs_GivenFolderWithChildren_ReturnsChildTabs()
    {
        // Arrange
        var tabs = new List<UnifiedStashTabMetadata>
        {
            new() { Id = "folder1", Name = "Folder", Type = "Folder", Children = new List<UnifiedStashTabMetadata>
            {
                new() { Id = "child1", Name = "Child 1", Type = "NormalStash" },
                new() { Id = "child2", Name = "Child 2", Type = "QuadStash" }
            }},
            new() { Id = "tab1", Name = "Tab 1", Type = "NormalStash" }
        };

        // Act
        var result = GlobalItemSetManagerState.FlattenStashTabs(tabs);

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(t => t.Id == "child1");
        result.Should().Contain(t => t.Id == "child2");
        result.Should().Contain(t => t.Id == "tab1");
        result.Should().NotContain(t => t.Id == "folder1");
    }

    [Fact]
    public void FlattenStashTabs_GivenFolderWithNullChildren_SkipsFolder()
    {
        // Arrange
        var tabs = new List<UnifiedStashTabMetadata>
        {
            new() { Id = "folder1", Name = "Folder", Type = "Folder", Children = null },
            new() { Id = "tab1", Name = "Tab 1", Type = "NormalStash" }
        };

        // Act
        var result = GlobalItemSetManagerState.FlattenStashTabs(tabs);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(t => t.Id == "tab1");
    }

    [Fact]
    public void FlattenStashTabs_GivenNoFolders_ReturnsAllTabs()
    {
        // Arrange
        var tabs = new List<UnifiedStashTabMetadata>
        {
            new() { Id = "tab1", Name = "Tab 1", Type = "NormalStash" },
            new() { Id = "tab2", Name = "Tab 2", Type = "QuadStash" }
        };

        // Act
        var result = GlobalItemSetManagerState.FlattenStashTabs(tabs);

        // Assert
        result.Should().HaveCount(2);
    }

    #endregion

    #region GenerateItemSets - Greedy (DoNotPreserveLowItemLevelGear = true)

    [Fact]
    public void GenerateItemSets_GivenCompleteChaosSet_ProducesOneCompletedSet()
    {
        // Arrange
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        GlobalItemSetManagerState.SetThreshold = 1;
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = CreateFullChaosSet(65);

        // Act
        GlobalItemSetManagerState.GenerateItemSets(regalRecipe: false);

        // Assert
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(1);
        GlobalItemSetManagerState.NeedsLowerLevel.Should().BeFalse();
    }

    [Fact]
    public void GenerateItemSets_GivenRegalRecipeWithFullSet_ProducesOneCompletedSet()
    {
        // Arrange
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        GlobalItemSetManagerState.SetThreshold = 1;
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = CreateFullChaosSet(80);

        // Act
        GlobalItemSetManagerState.GenerateItemSets(regalRecipe: true);

        // Assert
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(1);
        GlobalItemSetManagerState.NeedsLowerLevel.Should().BeFalse();
    }

    [Fact]
    public void GenerateItemSets_ChaosRecipe_WithNoEligibleItems_SetsNeedsLowerLevelTrue()
    {
        // Arrange - all items are ilvl 80 (regal eligible, NOT chaos-only eligible)
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        GlobalItemSetManagerState.SetThreshold = 1;
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = CreateFullChaosSet(80);

        // Act
        GlobalItemSetManagerState.GenerateItemSets(regalRecipe: false);

        // Assert - no chaos eligible items (60-74), so NeedsLowerLevel should be true
        GlobalItemSetManagerState.NeedsLowerLevel.Should().BeTrue();
    }

    [Fact]
    public void GenerateItemSets_RegalRecipe_NeverSetsNeedsLowerLevel()
    {
        // Arrange - items at level 60 (NOT regal eligible).
        // Before the infinite loop fix, this would hang forever because
        // the greedy algorithm found items by class but TryAddItem rejected
        // them for being below ilvl 75, leaving them in the pool indefinitely.
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        GlobalItemSetManagerState.SetThreshold = 1;
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = CreateFullChaosSet(60);

        // Act
        GlobalItemSetManagerState.GenerateItemSets(regalRecipe: true);

        // Assert
        GlobalItemSetManagerState.NeedsLowerLevel.Should().BeFalse();
    }

    [Fact]
    public void GenerateItemSets_GivenTwoHandWeapon_FillsWeaponSlotCorrectly()
    {
        // Arrange
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        GlobalItemSetManagerState.SetThreshold = 1;

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 65),
            CreateItemWithClassAndLevel(GameTerminology.TwoHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 65),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 65),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 65),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 65),
            CreateItemWithClassAndLevel(GameTerminology.Amulets, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(regalRecipe: false);

        // Assert
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        var set = GlobalItemSetManagerState.SetsInProgress[0];
        set.EmptyItemSlots.Should().BeEmpty();
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(1);
    }

    [Fact]
    public void GenerateItemSets_GivenOneHandWeapons_PairsTwoOneHanders()
    {
        // Arrange
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        GlobalItemSetManagerState.SetThreshold = 1;

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 65),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 65),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 65),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 65),
            CreateItemWithClassAndLevel(GameTerminology.Amulets, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(regalRecipe: false);

        // Assert
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        var set = GlobalItemSetManagerState.SetsInProgress[0];
        set.EmptyItemSlots.Should().BeEmpty();
        set.Items.Where(i => i.DerivedItemClass == GameTerminology.OneHandWeapons).Should().HaveCount(2);
    }

    [Fact]
    public void GenerateItemSets_VendorSetsEarly_CapsThresholdToEligibleCount()
    {
        // Arrange
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = true;
        GlobalItemSetManagerState.SetThreshold = 5;

        // Only provide 2 chaos-eligible items
        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.Amulets, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(regalRecipe: false);

        // Assert - should only create 2 sets (capped to eligible count), not 5
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(2);
    }

    #endregion

    #region Helper Methods

    private static EnhancedItem CreateItemWithClass(string itemClass)
    {
        return new EnhancedItem
        {
            DerivedItemClass = itemClass,
            ItemLevel = 65,
            X = 0,
            Y = 0,
            StashTabIndex = 0
        };
    }

    private static EnhancedItem CreateItemWithClassAndLevel(string itemClass, int level, int x = 0, int y = 0, int tabIndex = 0)
    {
        return new EnhancedItem
        {
            DerivedItemClass = itemClass,
            ItemLevel = level,
            X = x,
            Y = y,
            StashTabIndex = tabIndex
        };
    }

    private static List<EnhancedItem> CreateFullChaosSet(int itemLevel)
    {
        return
        [
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, itemLevel),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, itemLevel),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, itemLevel),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, itemLevel),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, itemLevel),
            CreateItemWithClassAndLevel(GameTerminology.Boots, itemLevel),
            CreateItemWithClassAndLevel(GameTerminology.Belts, itemLevel),
            CreateItemWithClassAndLevel(GameTerminology.Amulets, itemLevel),
            CreateItemWithClassAndLevel(GameTerminology.Rings, itemLevel),
            CreateItemWithClassAndLevel(GameTerminology.Rings, itemLevel),
        ];
    }

    #endregion
}
