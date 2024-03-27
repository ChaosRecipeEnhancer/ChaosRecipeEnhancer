using ChaosRecipeEnhancer.UI.Converters;
using System.Globalization;
using System.Windows;

namespace ChaosRecipeEnhancer.UI.Tests.Converters;

public class BoolVisibilityConverterTests
{
    [Fact]
    public void Convert_True_ReturnsVisible()
    {
        // Arrange
        var converter = new BoolVisibilityConverter();

        // Act
        var result = converter.Convert(true, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_False_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BoolVisibilityConverter();

        // Act
        var result = converter.Convert(false, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_False_CollapseWhenNotVisibleFalse_ReturnsHidden()
    {
        // Arrange
        var converter = new BoolVisibilityConverter { CollapseWhenNotVisible = false };

        // Act
        var result = converter.Convert(false, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Hidden, result);
    }

    [Fact]
    public void Convert_True_InvertTrue_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BoolVisibilityConverter { Invert = true };

        // Act
        var result = converter.Convert(true, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Collapsed, result);
    }

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
        var converter = new BoolVisibilityConverter { Invert = true };

        // Act
        var result = converter.Convert(true, null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_GivenFalseAndInvert_ReturnsVisible()
    {
        // Arrange
        var converter = new BoolVisibilityConverter { Invert = true };

        // Act
        var result = converter.Convert(false, null, null, null);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Fact]
    public void Convert_GivenTrueAndCollapseWhenNotVisible_ReturnsVisible()
    {
        // Arrange
        var converter = new BoolVisibilityConverter { CollapseWhenNotVisible = true };

        // Act
        var result = converter.Convert(true, null, null, null);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Fact]
    public void Convert_GivenFalseAndCollapseWhenNotVisible_ReturnsCollapsed()
    {
        // Arrange
        var converter = new BoolVisibilityConverter { CollapseWhenNotVisible = true };

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
