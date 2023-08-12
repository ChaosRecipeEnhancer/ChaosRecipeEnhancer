using ChaosRecipeEnhancer.UI.BusinessLogic.Enums;
using ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterGeneration.Factory;

namespace ChaosRecipeEnhancer.UI.Tests.BusinessLogic.FilterManipulation.FilterGeneration.Factory;

public class CItemClassManagerFactoryTests
{
    [Theory]
    [InlineData(EnumItemClass.Amulets, "Amulets")]
    [InlineData(EnumItemClass.Belts, "Belts")]
    [InlineData(EnumItemClass.BodyArmours, "BodyArmours")]
    [InlineData(EnumItemClass.Boots, "Boots")]
    [InlineData(EnumItemClass.Gloves, "Gloves")]
    [InlineData(EnumItemClass.Helmets, "Helmets")]
    [InlineData(EnumItemClass.OneHandWeapons, "OneHandWeapons")]
    [InlineData(EnumItemClass.Rings, "Rings")]
    [InlineData(EnumItemClass.TwoHandWeapons, "TwoHandWeapons")]
    public void GetItemClassManager_GivenItemClass_ReturnsManagerWithClassName(EnumItemClass itemClass, string className)
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
        Action act = () => factory.GetItemClassManager((EnumItemClass) 100000245);

        // Assert
        act.Should().Throw<Exception>().WithMessage("Wrong item class.");
    }
}