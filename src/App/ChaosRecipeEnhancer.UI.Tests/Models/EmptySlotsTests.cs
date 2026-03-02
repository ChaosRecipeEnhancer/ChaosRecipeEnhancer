using ChaosRecipeEnhancer.UI.Models;

namespace ChaosRecipeEnhancer.UI.Tests.Models;

public class EmptySlotsTests
{
    [Fact]
    public void Ordered_WhenAccessed_Returns11Slots()
    {
        // Arrange & Act
        var slots = EmptySlots.Ordered;

        // Assert
        slots.Should().HaveCount(11);
    }

    [Fact]
    public void Ordered_WhenAccessed_ContainsAllRequiredItemTypes()
    {
        // Arrange & Act
        var slots = EmptySlots.Ordered;

        // Assert
        slots.Should().Contain("BodyArmours");
        slots.Should().Contain("OneHandWeapons");
        slots.Should().Contain("TwoHandWeapons");
        slots.Should().Contain("Helmets");
        slots.Should().Contain("Gloves");
        slots.Should().Contain("Boots");
        slots.Should().Contain("Belts");
        slots.Should().Contain("Amulets");
        slots.Should().Contain("Rings");
    }

    [Fact]
    public void Ordered_WhenAccessedTwice_ReturnsDifferentListInstances()
    {
        // Arrange & Act
        var slots1 = EmptySlots.Ordered;
        var slots2 = EmptySlots.Ordered;

        // Assert
        slots1.Should().NotBeSameAs(slots2);
        slots1.Should().Equal(slots2);
    }

    [Fact]
    public void Ordered_ContainsExactlyTwoOneHandWeapons()
    {
        // Arrange & Act
        var slots = EmptySlots.Ordered;
        var oneHandWeaponCount = slots.Count(x => x == "OneHandWeapons");

        // Assert
        oneHandWeaponCount.Should().Be(2);
    }

    [Fact]
    public void Ordered_ContainsExactlyTwoRings()
    {
        // Arrange & Act
        var slots = EmptySlots.Ordered;
        var ringCount = slots.Count(x => x == "Rings");

        // Assert
        ringCount.Should().Be(2);
    }

    [Fact]
    public void Ordered_ContainsSingleInstancesOfOtherItems()
    {
        // Arrange & Act
        var slots = EmptySlots.Ordered;

        // Assert
        slots.Count(x => x == "BodyArmours").Should().Be(1);
        slots.Count(x => x == "TwoHandWeapons").Should().Be(1);
        slots.Count(x => x == "Helmets").Should().Be(1);
        slots.Count(x => x == "Gloves").Should().Be(1);
        slots.Count(x => x == "Boots").Should().Be(1);
        slots.Count(x => x == "Belts").Should().Be(1);
        slots.Count(x => x == "Amulets").Should().Be(1);
    }
}
