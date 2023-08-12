namespace ChaosRecipeEnhancer.UI.Tests.BusinessLogic.FilterManipulation.FilterGeneration.Factory.Managers;

public class ABaseItemClassManagerTests
{
    [Fact]
    public void SetBaseType_GivenClassFilterNameAssigned_ReturnsClassAndFilterName()
    {
        // Arrange
        var manager = new ItemClassManager
        {
            ClassFilterName = "Test"
        };

        // Act
        var result = manager.SetBaseType();

        // Assert
        result.Should().Be("Class Test");
    }
    
    [Fact]
    public void CheckIfMissing_GivenClassNameAssigned_ReturnsTrue()
    {
        // Arrange
        var manager = new ItemClassManager
        {
            ClassName = "Test"
        };
        var missingItemClasses = new HashSet<string>
        {
            "Test"
        };

        // Act
        var result = manager.CheckIfMissing(missingItemClasses);

        // Assert
        result.Should().BeTrue();
    }
    
    [Fact]
    public void CheckIfMissing_GivenClassNameNotAssigned_ReturnsFalse()
    {
        // Arrange
        var manager = new ItemClassManager
        {
            ClassName = "Test"
        };
        var missingItemClasses = new HashSet<string>
        {
            "Test2"
        };

        // Act
        var result = manager.CheckIfMissing(missingItemClasses);

        // Assert
        result.Should().BeFalse();
    }
}