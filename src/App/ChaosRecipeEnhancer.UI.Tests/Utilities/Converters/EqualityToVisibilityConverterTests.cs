using System.Windows;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities.Converters;

public class EqualityToVisibilityConverterTests
{
    [Fact]
    public void Convert_WhenValueIsNullAndParameterIsNull_ReturnsTrue()
    {
        // Arrange
        var converter = new EqualityToVisibilityConverter();

        // Act
        var result = converter.Convert(null, null, null, null);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Fact]
    public void Convert_WhenValueIsNullAndParameterIsNotNull_ReturnsFalse()
    {
        // Arrange
        var converter = new EqualityToVisibilityConverter();

        // Act
        var result = converter.Convert(null, null, "test", null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_WhenValueIsNotNullAndParameterIsNull_ReturnsFalse()
    {
        // Arrange
        var converter = new EqualityToVisibilityConverter();

        // Act
        var result = converter.Convert("test", null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_WhenValueIsNotNullAndParameterIsNotNull_ReturnsTrue()
    {
        // Arrange
        var converter = new EqualityToVisibilityConverter();

        // Act
        var result = converter.Convert("test", null, "test", null);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Fact]
    public void Convert_WhenValueIsNotNullAndParameterIsNotEqual_ReturnsFalse()
    {
        // Arrange
        var converter = new EqualityToVisibilityConverter();

        // Act
        var result = converter.Convert("test", null, "test2", null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_WhenValueIsNotNullAndParameterIsNotEqualAndInvert_ReturnsTrue()
    {
        // Arrange
        var converter = new EqualityToVisibilityConverter { Invert = true };

        // Act
        var result = converter.Convert("test", null, "test2", null);

        // Assert
        result.Should().Be(Visibility.Visible);
    }

    [Fact]
    public void Convert_WhenValueIsNotNullAndParameterIsEqualAndInvert_ReturnsFalse()
    {
        // Arrange
        var converter = new EqualityToVisibilityConverter { Invert = true };

        // Act
        var result = converter.Convert("test", null, "test", null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_WhenComparisonTypeIsNotNullAndValueIsNotEqualAndInvert_ReturnsTrue()
    {
        // Arrange
        var converter = new EqualityToVisibilityConverter { Invert = true, ComparisonType = typeof(string) };

        // Act
        var result = converter.Convert("test", null, "TEST", null);

        // Assert
        result.Should().Be(Visibility.Visible);
    }
}