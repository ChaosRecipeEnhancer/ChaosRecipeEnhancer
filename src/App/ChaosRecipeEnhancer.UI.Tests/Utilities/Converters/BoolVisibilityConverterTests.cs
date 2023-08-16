using System.Windows;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities.Converters;

public class BoolVisibilityConverterTests
{
    [Fact]
    public void Convert_GivenTrue_ReturnsVisible()
    {
        // Arrange
        var converter = new BoolVisibilityConverter();

        // Act
        var result = converter.Convert(true, null, null, null);

        // Assert
        result.Should().Be(Visibility.Visible);
    }
    
    [Fact]
    public void Convert_GivenFalse_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BoolVisibilityConverter();

        // Act
        var result = converter.Convert(false, null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }
    
    [Fact]
    public void Convert_GivenTrueAndInvert_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BoolVisibilityConverter {Invert = true};

        // Act
        var result = converter.Convert(true, null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }
    
    [Fact]
    public void Convert_GivenFalseAndInvert_ReturnsVisible()
    {
        // Arrange
        var converter = new BoolVisibilityConverter {Invert = true};

        // Act
        var result = converter.Convert(false, null, null, null);

        // Assert
        result.Should().Be(Visibility.Visible);
    }
    
    [Fact]
    public void Convert_GivenTrueAndCollapseWhenNotVisible_ReturnsVisible()
    {
        // Arrange
        var converter = new BoolVisibilityConverter {CollapseWhenNotVisible = true};

        // Act
        var result = converter.Convert(true, null, null, null);

        // Assert
        result.Should().Be(Visibility.Visible);
    }
    
    [Fact]
    public void Convert_GivenFalseAndCollapseWhenNotVisible_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BoolVisibilityConverter {CollapseWhenNotVisible = true};

        // Act
        var result = converter.Convert(false, null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_GivenNonBoolValue_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BoolVisibilityConverter();

        // Act
        var result = converter.Convert("I'm a string!", null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }
}