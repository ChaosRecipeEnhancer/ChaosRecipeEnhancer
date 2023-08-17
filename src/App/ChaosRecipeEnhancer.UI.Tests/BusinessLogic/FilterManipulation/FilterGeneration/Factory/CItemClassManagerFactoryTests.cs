using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory;

namespace ChaosRecipeEnhancer.UI.Tests.BusinessLogic.FilterManipulation.FilterGeneration.Factory;

public class CItemClassManagerFactoryTests
{
    [Theory]
    [InlineData(ItemClass.Amulets, "Amulets")]
    [InlineData(ItemClass.Belts, "Belts")]
    [InlineData(ItemClass.BodyArmours, "BodyArmours")]
    [InlineData(ItemClass.Boots, "Boots")]
    [InlineData(ItemClass.Gloves, "Gloves")]
    [InlineData(ItemClass.Helmets, "Helmets")]
    [InlineData(ItemClass.OneHandWeapons, "OneHandWeapons")]
    [InlineData(ItemClass.Rings, "Rings")]
    [InlineData(ItemClass.TwoHandWeapons, "TwoHandWeapons")]
    public void GetItemClassManager_GivenItemClass_ReturnsManagerWithClassName(ItemClass itemClass, string className)
    {
        // Arrange
        var factory = new CItemClassManagerFactory();

        // Act
        var result = factory.GetItemClassManager(itemClass);

        // Assert
        result.ClassName.Should().Be(className);
    }

    [Fact]
    public void GetItemManager_GivenInvalidClass_Throws()
    {
        // Arrange
        var factory = new CItemClassManagerFactory();

        // Act
        Action act = () => factory.GetItemClassManager((ItemClass)100000245);

        // Assert
        act.Should().Throw<Exception>().WithMessage("Wrong item class.");
    }
}