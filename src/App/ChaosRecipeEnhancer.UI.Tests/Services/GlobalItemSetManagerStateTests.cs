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
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = false;
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
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

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
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.RegalOrb);

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
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

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
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.RegalOrb);

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
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

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
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

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
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert - should only create 2 sets (capped to eligible count), not 5
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(2);
    }

    #endregion

    #region GenerateItemSets - PrioritizeRecentlyStashedItems

    // ── Greedy Path (DoNotPreserveLowItemLevelGear = true) ──────────────────

    [Fact]
    public void PrioritizeRecent_Greedy_Enabled_SelectsReversedSeedItem()
    {
        // Arrange
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;

        // Amulets sort first alphabetically, so the seed item is the first amulet.
        // With reverse, the bottom-right amulet (last in API order) becomes first.
        var topLeftAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 0, y: 0);
        var bottomRightAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 10, y: 10);

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 65),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 65),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 65),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 65),
            topLeftAmulet,
            bottomRightAmulet,
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        var set = GlobalItemSetManagerState.SetsInProgress[0];
        var selectedAmulet = set.Items.First(i => i.DerivedItemClass == GameTerminology.Amulets);
        selectedAmulet.Should().BeSameAs(bottomRightAmulet);
    }

    [Fact]
    public void PrioritizeRecent_Greedy_Disabled_SelectsDefaultSeedItem()
    {
        // Arrange
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = false;
        GlobalItemSetManagerState.SetThreshold = 1;

        var topLeftAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 0, y: 0);
        var bottomRightAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 10, y: 10);

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 65),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 65),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 65),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 65),
            topLeftAmulet,
            bottomRightAmulet,
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        var set = GlobalItemSetManagerState.SetsInProgress[0];
        var selectedAmulet = set.Items.First(i => i.DerivedItemClass == GameTerminology.Amulets);
        selectedAmulet.Should().BeSameAs(topLeftAmulet);
    }

    [Fact]
    public void PrioritizeRecent_Greedy_Enabled_TwoHandWeaponSeedIsReversed()
    {
        // Arrange — 2H weapons sort FIRST in the list (OrderByDescending), so the seed
        // item for the set should be the bottom-right 2H weapon when reversed.
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;

        var topLeft2H = CreateItemWithClassAndLevel(GameTerminology.TwoHandWeapons, 65, x: 0, y: 0);
        var bottomRight2H = CreateItemWithClassAndLevel(GameTerminology.TwoHandWeapons, 65, x: 10, y: 10);

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 65),
            topLeft2H,
            bottomRight2H,
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
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — bottom-right 2H should be selected as seed (fills both weapon slots)
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        var set = GlobalItemSetManagerState.SetsInProgress[0];
        var selected2H = set.Items.First(i => i.DerivedItemClass == GameTerminology.TwoHandWeapons);
        selected2H.Should().BeSameAs(bottomRight2H);
    }

    [Fact]
    public void PrioritizeRecent_Greedy_Disabled_TwoHandWeaponSeedIsDefault()
    {
        // Arrange
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = false;
        GlobalItemSetManagerState.SetThreshold = 1;

        var topLeft2H = CreateItemWithClassAndLevel(GameTerminology.TwoHandWeapons, 65, x: 0, y: 0);
        var bottomRight2H = CreateItemWithClassAndLevel(GameTerminology.TwoHandWeapons, 65, x: 10, y: 10);

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 65),
            topLeft2H,
            bottomRight2H,
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
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — top-left 2H should be selected as seed (default order)
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        var set = GlobalItemSetManagerState.SetsInProgress[0];
        var selected2H = set.Items.First(i => i.DerivedItemClass == GameTerminology.TwoHandWeapons);
        selected2H.Should().BeSameAs(topLeft2H);
    }

    [Fact]
    public void PrioritizeRecent_Greedy_Enabled_MultipleSets_EachSeedIsReversed()
    {
        // Arrange — with threshold=2 and 2 amulets, each set gets seeded.
        // The reversed list should seed set 1 with bottomRight, set 2 with topLeft.
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 2;

        var topLeftAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 0, y: 0);
        var bottomRightAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 10, y: 10);

        // Two full sets worth of items (minus the amulets we're tracking)
        var items = new List<EnhancedItem>
        {
            // Set 1 items
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 65),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 65),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 65),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            // Set 2 items
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 65),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 65),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 65),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            // Two amulets — one per set
            topLeftAmulet,
            bottomRightAmulet,
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — both sets should complete, first seed should be bottom-right amulet
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(2);
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(2);
        var firstSetAmulet = GlobalItemSetManagerState.SetsInProgress[0].Items
            .First(i => i.DerivedItemClass == GameTerminology.Amulets);
        firstSetAmulet.Should().BeSameAs(bottomRightAmulet);
    }

    [Fact]
    public void PrioritizeRecent_Greedy_Enabled_VendorSetsEarly_StillReversesSeeds()
    {
        // Arrange — VendorSetsEarly caps threshold to eligible item count when less than
        // the threshold. Provide only 3 eligible items so VendorSetsEarly caps to 3.
        // Reverse should still apply to seed selection.
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = true;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 5;

        var topLeftAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 0, y: 0);
        var bottomRightAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 10, y: 10);

        // Only 3 chaos-eligible items → VendorSetsEarly caps trueSetThreshold to 3
        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            topLeftAmulet,
            bottomRightAmulet,
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — capped to 3 sets, first seed should still be bottom-right amulet
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(3);
        var firstSetAmulet = GlobalItemSetManagerState.SetsInProgress[0].Items
            .First(i => i.DerivedItemClass == GameTerminology.Amulets);
        firstSetAmulet.Should().BeSameAs(bottomRightAmulet);
    }

    [Fact]
    public void PrioritizeRecent_Greedy_Enabled_RegalRecipe_StillReversesSeeds()
    {
        // Arrange — Regal recipe goes through Greedy path; reverse should apply
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;

        var topLeftAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 80, x: 0, y: 0);
        var bottomRightAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 80, x: 10, y: 10);

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 80),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 80),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 80),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 80),
            topLeftAmulet,
            bottomRightAmulet,
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.RegalOrb);

        // Assert
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        var set = GlobalItemSetManagerState.SetsInProgress[0];
        var selectedAmulet = set.Items.First(i => i.DerivedItemClass == GameTerminology.Amulets);
        selectedAmulet.Should().BeSameAs(bottomRightAmulet);
    }

    [Fact]
    public void PrioritizeRecent_Greedy_Enabled_SingleItemPerClass_StillCompletesSet()
    {
        // Arrange — reverse is a no-op with one item per class, but set should still complete
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = CreateFullChaosSet(65);

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — no extras to choose between, but must still work
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(1);
    }

    [Fact]
    public void PrioritizeRecent_Greedy_Enabled_EmptyStash_NoCrash()
    {
        // Arrange — nothing to reverse
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = new List<EnhancedItem>();

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — should handle gracefully
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(0);
    }

    [Fact]
    public void PrioritizeRecent_Greedy_Enabled_InsufficientItems_PartialSetNoHang()
    {
        // Arrange — not enough items for a full set, reverse should still not cause issues
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 10, y: 10),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 65),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — partial set, not completed
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(0);
        GlobalItemSetManagerState.SetsInProgress[0].Items.Should().NotBeEmpty();
    }

    // ── Conserve Path (DoNotPreserveLowItemLevelGear = false) ──────────────

    [Fact]
    public void PrioritizeRecent_Conserve_Enabled_SelectsReversedSeedItem()
    {
        // Arrange — Conserve path uses foreach iteration over eligibleRecipeItems
        // for seed selection. Reverse should cause bottom-right item to be first.
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = false;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;

        // In Conserve mode, chaos-eligible items (ilvl 60-74) are the seed items
        var topLeftAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 0, y: 0);
        var bottomRightAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 10, y: 10);

        // Mix of chaos-eligible and non-chaos-eligible items to exercise the Conserve filter
        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 80),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 80),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 80),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 80),
            topLeftAmulet,
            bottomRightAmulet,
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — bottom-right amulet should be the seed in Conserve mode too
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        var set = GlobalItemSetManagerState.SetsInProgress[0];
        var selectedAmulet = set.Items.First(i => i.DerivedItemClass == GameTerminology.Amulets);
        selectedAmulet.Should().BeSameAs(bottomRightAmulet);
    }

    [Fact]
    public void PrioritizeRecent_Conserve_Disabled_SelectsDefaultSeedItem()
    {
        // Arrange
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = false;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = false;
        GlobalItemSetManagerState.SetThreshold = 1;

        var topLeftAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 0, y: 0);
        var bottomRightAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 10, y: 10);

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 80),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 80),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 80),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 80),
            topLeftAmulet,
            bottomRightAmulet,
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — top-left amulet should be seed (default order)
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        var set = GlobalItemSetManagerState.SetsInProgress[0];
        var selectedAmulet = set.Items.First(i => i.DerivedItemClass == GameTerminology.Amulets);
        selectedAmulet.Should().BeSameAs(topLeftAmulet);
    }

    [Fact]
    public void PrioritizeRecent_Conserve_Enabled_CompletesSetCorrectly()
    {
        // Arrange — full set available with mixed ilvls, Conserve should still complete
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = false;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 80),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 80),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 80),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 80),
            CreateItemWithClassAndLevel(GameTerminology.Amulets, 65),  // chaos-eligible seed
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(1);
    }

    [Fact]
    public void PrioritizeRecent_Conserve_Enabled_MultipleSets_BothSeedReversed()
    {
        // Arrange — 2 sets in Conserve mode, each seeded with a chaos-eligible item
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = false;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 2;

        var topLeftAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 0, y: 0);
        var bottomRightAmulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 10, y: 10);

        var items = new List<EnhancedItem>
        {
            // Fill for 2 sets (non-chaos-eligible, ilvl 80)
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 80),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 80),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 80),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 80),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 80),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 80),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 80),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 80),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 80),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 80),
            // Seeds (chaos-eligible, ilvl 65)
            topLeftAmulet,
            bottomRightAmulet,
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — both sets should complete; first seed should be bottom-right amulet
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(2);
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(2);
        var firstSetAmulet = GlobalItemSetManagerState.SetsInProgress[0].Items
            .First(i => i.DerivedItemClass == GameTerminology.Amulets);
        firstSetAmulet.Should().BeSameAs(bottomRightAmulet);
    }

    [Fact]
    public void PrioritizeRecent_Conserve_Enabled_EmptyStash_NoCrash()
    {
        // Arrange
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = false;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = new List<EnhancedItem>();

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(0);
    }

    // ── Cross-cutting concerns ────────────────────────────────────────────

    [Fact]
    public void PrioritizeRecent_Enabled_DoesNotAffectCompletedSetCount()
    {
        // Arrange — a full set should still count as completed regardless of ordering
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = CreateFullChaosSet(65);

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert
        GlobalItemSetManagerState.CompletedSetCount.Should().Be(1);
        GlobalItemSetManagerState.SetsInProgress[0].EmptyItemSlots.Should().BeEmpty();
    }

    [Fact]
    public void PrioritizeRecent_Enabled_DoesNotAffectNeedsLowerLevel()
    {
        // Arrange — all ilvl 80 items (not chaos-eligible), NeedsLowerLevel should still be true
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = CreateFullChaosSet(80);

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert
        GlobalItemSetManagerState.NeedsLowerLevel.Should().BeTrue();
    }

    [Fact]
    public void PrioritizeRecent_Enabled_MultipleTabIndices_SeedFromLastTab()
    {
        // Arrange — items spread across 2 stash tabs.
        // With reverse, the item from the later position in the list (tab 1) becomes the seed.
        Properties.Settings.Default.DoNotPreserveLowItemLevelGear = true;
        Properties.Settings.Default.VendorSetsEarly = false;
        Properties.Settings.Default.PrioritizeRecentlyStashedItems = true;
        GlobalItemSetManagerState.SetThreshold = 1;

        var tab0Amulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 0, y: 0, tabIndex: 0);
        var tab1Amulet = CreateItemWithClassAndLevel(GameTerminology.Amulets, 65, x: 0, y: 0, tabIndex: 1);

        var items = new List<EnhancedItem>
        {
            CreateItemWithClassAndLevel(GameTerminology.BodyArmors, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.OneHandWeapons, 65),
            CreateItemWithClassAndLevel(GameTerminology.Helmets, 65),
            CreateItemWithClassAndLevel(GameTerminology.Gloves, 65),
            CreateItemWithClassAndLevel(GameTerminology.Boots, 65),
            CreateItemWithClassAndLevel(GameTerminology.Belts, 65),
            tab0Amulet,
            tab1Amulet,
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
            CreateItemWithClassAndLevel(GameTerminology.Rings, 65),
        };
        GlobalItemSetManagerState.CurrentItemsFilteredForRecipe = items;

        // Act
        GlobalItemSetManagerState.GenerateItemSets(RecipeType.ChaosOrb);

        // Assert — tab1 amulet should be selected (later in original list → first after reverse)
        GlobalItemSetManagerState.SetsInProgress.Should().HaveCount(1);
        var set = GlobalItemSetManagerState.SetsInProgress[0];
        var selectedAmulet = set.Items.First(i => i.DerivedItemClass == GameTerminology.Amulets);
        selectedAmulet.Should().BeSameAs(tab1Amulet);
    }

    [Fact]
    public void PrioritizeRecent_SettingDefaultIsFalse()
    {
        // Assert — the setting should default to false so existing users aren't affected
        Properties.Settings.Default.Reset();
        Properties.Settings.Default.PrioritizeRecentlyStashedItems.Should().BeFalse();
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
