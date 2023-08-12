using System.Collections.Specialized;
using System.Reflection;
using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Tests.Helpers;

namespace ChaosRecipeEnhancer.UI.Tests.DynamicControls.StashTabs;

public class ItemSetManagerTests
{
    [Fact]
    public void SetSelectedStashTab_GivenStashTab_SetsSelectedStashTab()
    {
        // Arrange
        var itemSetManager = new ItemSetManager();
        var stashTab = new StashTab("Test", 0, new Uri("https://www.test.com"));
        Settings.Default.SelectedStashTabs = new StringCollection { "ADifferentName" };

        // Act
        itemSetManager.SelectedStashTab = stashTab;

        // Assert
        itemSetManager.SelectedStashTab.Should().Be(stashTab);
        Settings.Default.SelectedStashTabs[0].Should().Be(stashTab.TabName);
    }

    [Fact]
    public void UpdateData_GivenStashTabWithFullSet_UpdatesAmounts()
    {
        // Arrange
        var itemSetManager = new ItemSetManager();
        var stashTab = CreateFullSetStashTab();

        itemSetManager.SelectedStashTab = stashTab;
        
        // Act
        itemSetManager.UpdateData();
        
        // Assert
        stashTab.RingAmount.Should().Be(2);
        stashTab.AmuletsAmount.Should().Be(1);
        stashTab.BeltsAmount.Should().Be(1);
        stashTab.WeaponAmount.Should().Be(2);
        stashTab.GlovesAmount.Should().Be(1);
        stashTab.HelmetsAmount.Should().Be(1);
        stashTab.BootsAmount.Should().Be(1);
    }

    [Fact]
    public void UpdateData_GivenStashTabWithFullSet_GeneratesItemSets()
    {
        // Arrange
        Settings.Default.FullSetThreshold = 1;
        var itemSetManager = new ItemSetManager();
        var stashTab = CreateFullSetStashTab();
        itemSetManager.SelectedStashTab = stashTab;
        
        // Act
        itemSetManager.UpdateData();
        
        // Assert
        // I generally hate testing private fields with reflection, but I don't see a better way to do this & it seemed
        // Important to assert a full set is added
        itemSetManager.GetType().GetField("_itemSetList", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(itemSetManager).Should().BeOfType<List<ItemSet>>()
            .Which.Should().HaveCount(1);
    }

    [Fact]
    public void UpdateData_GivenStashTabWithFullSet_ActivatesAllCellForNextSet()
    {
        // Arrange
        Settings.Default.FullSetThreshold = 1;
        var itemSetManager = new ItemSetManager();
        var stashTab = CreateFullSetStashTab();
        itemSetManager.SelectedStashTab = stashTab;
        var expectedActiveCells = stashTab.ItemsForChaosRecipe.Select(x => x.w * x.h).Sum();
        // Act
        itemSetManager.UpdateData();
        
        // Assert
        stashTab.OverlayCellsList.Where(x => x.Active && x.Item.ItemType == "Amulets").Should().HaveCount(1);
        stashTab.OverlayCellsList.Where(x => x.Active && x.Item.ItemType == "Rings").Should().HaveCount(2);
        stashTab.OverlayCellsList.Where(x => x.Active && x.Item.ItemType == "Belts").Should().HaveCount(2);
        stashTab.OverlayCellsList.Where(x => x.Active && x.Item.ItemType == "TwoHandWeapons").Should().HaveCount(6);
        stashTab.OverlayCellsList.Where(x => x.Active && x.Item.ItemType == "Gloves").Should().HaveCount(4);
        stashTab.OverlayCellsList.Where(x => x.Active && x.Item.ItemType == "Helmets").Should().HaveCount(4);
        stashTab.OverlayCellsList.Where(x => x.Active && x.Item.ItemType == "Boots").Should().HaveCount(4);
        stashTab.OverlayCellsList.Where(x => x.Active && x.Item.ItemType == "BodyArmours").Should().HaveCount(6);
        stashTab.OverlayCellsList.Where(x => x.Active).Should().HaveCount(expectedActiveCells);
    }
    
    [Fact]
    public void OnItemCellClicked_GivenCellContainingItem_DeactivatesCell()
    {
        // Arrange
        Settings.Default.FullSetThreshold = 1;
        var itemSetManager = new ItemSetManager();
        var stashTab = CreateFullSetStashTab();
        itemSetManager.SelectedStashTab = stashTab;
        itemSetManager.UpdateData();
        var cell = stashTab.OverlayCellsList.First(x => x.Item.ItemType == "Rings");
        
        // Act
        itemSetManager.OnItemCellClicked(cell);
        
        // Assert
        cell.Active.Should().BeFalse();
    }

    [Fact]
    public void OnItemCellClicked_AllItemsRemoved_ActivatesNextSet()
    {
        // Arrange
        Settings.Default.FullSetThreshold = 2;
        var itemSetManager = new ItemSetManager();
        
        var itemSetOne = GenerateItemList(0);
        var itemSetTwo = GenerateItemList(5);
        
        var stashTab = new StashTab("Test", 0, new Uri("https://www.test.com"))
        {
            Quad = true
        };
        
        var allItems = itemSetOne.Concat(itemSetTwo).ToList();
        
        stashTab.FilterItemsForChaosRecipe(allItems);
        
        itemSetManager.SelectedStashTab = stashTab;
        itemSetManager.UpdateData();
        
        var expectedActiveCells = itemSetOne.Select(x => x.w * x.h).Sum();
        
        // Act
        while (stashTab.OverlayCellsList.Count(x => x.Active && x.XIndex == 0) > 0)
        {
            itemSetManager.OnItemCellClicked(stashTab.OverlayCellsList.First(x => x.Active && x.XIndex == 0));
        }
        
        // Assert
        stashTab.OverlayCellsList.Where(x => x.Active).Should().HaveCount(expectedActiveCells);
        itemSetManager.GetType().GetField("_itemSetList", BindingFlags.NonPublic | BindingFlags.Instance)
            .GetValue(itemSetManager).Should().BeOfType<List<ItemSet>>()
            .Which.Should().HaveCount(1);
    }

    private static StashTab CreateFullSetStashTab(int horizontalShift = 0)
    {
        var stashTab = new StashTab("Test", 0, new Uri("https://www.test.com"))
        {
            Quad = true
        };
        var itemList = GenerateItemList(horizontalShift);
        stashTab.FilterItemsForChaosRecipe(itemList);
        return stashTab;
    }

    private static List<Item> GenerateItemList(int horizontalShift)
    {
        var ring1 = new Item
        {
            ItemType = "Rings",
            h = 1,
            w = 1,
            x = 0 + horizontalShift,
            y = 0,
            ilvl = 74,
            frameType = 2,
            icon = ItemIconConstants.RingUrl
        };
        var ring2 = new Item
        {
            ItemType = "Rings",
            h = 1,
            w = 1,
            x = 0 + horizontalShift,
            y = 1,
            ilvl = 74,
            frameType = 2,
            icon = ItemIconConstants.RingUrl
        };
        var amulet = new Item
        {
            ItemType = "Amulets",
            h = 1,
            w = 1,
            x = 0 + horizontalShift,
            y = 2,
            ilvl = 74,
            frameType = 2,
            icon = ItemIconConstants.AmuletUrl
        };
        var belt = new Item
        {
            ItemType = "Belts",
            h = 1,
            w = 2,
            x = 0 + horizontalShift,
            y = 3,
            ilvl = 74,
            frameType = 2,
            icon = ItemIconConstants.BeltUrl
        };
        var bodyArmour = new Item
        {
            ItemType = "BodyArmours",
            h = 3,
            w = 2,
            x = 0 + horizontalShift,
            y = 4,
            ilvl = 74,
            frameType = 2,
            icon = ItemIconConstants.BodyArmourUrl
        };
        var twoHandWeapon = new Item
        {
            ItemType = "TwoHandWeapons",
            h = 3,
            w = 2,
            x = 0 + horizontalShift,
            y = 7,
            ilvl = 74,
            frameType = 2,
            icon = ItemIconConstants.TwoHandedAxeUrl
        };
        var glove = new Item
        {
            ItemType = "Gloves",
            h = 2,
            w = 2,
            x = 0 + horizontalShift,
            y = 10,
            ilvl = 74,
            frameType = 2,
            icon = ItemIconConstants.GlovesUrl
        };
        var helmet = new Item
        {
            ItemType = "Helmets",
            h = 2,
            w = 2,
            x = 0 + horizontalShift,
            y = 12,
            ilvl = 74,
            frameType = 2,
            icon = ItemIconConstants.HelmetUrl
        };
        var boot = new Item
        {
            ItemType = "Boots",
            h = 2,
            w = 2,
            x = 0 + horizontalShift,
            y = 14,
            ilvl = 74,
            frameType = 2,
            icon = ItemIconConstants.BootsUrl
        };
        return new List<Item>
            { ring1, ring2, amulet, belt, bodyArmour, twoHandWeapon, glove, helmet, boot };
    }
}