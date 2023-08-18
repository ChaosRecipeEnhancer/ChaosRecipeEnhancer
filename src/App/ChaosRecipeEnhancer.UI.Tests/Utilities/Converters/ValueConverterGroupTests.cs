using System.Windows;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities.Converters;

public class ValueConverterGroupTests
{
    [Fact]
    public void Convert_GivenTrueAndTrue_ReturnsTrue()
    {
        // Arrange
        var converter = new ValueConverterGroup
        {
                new BoolVisibilityConverter(),
                new BoolVisibilityConverter()
        };

        // Act
        var result = converter.Convert(new object[] { true, true }, null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_GivenTrueAndFalse_ReturnsFalse()
    {
        // Arrange
        var converter = new ValueConverterGroup
        {
                new BoolVisibilityConverter(),
                new BoolVisibilityConverter()
        };

        // Act
        var result = converter.Convert(new object[] { true, false }, null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_GivenFalseAndTrue_ReturnsFalse()
    {
        // Arrange
        var converter = new ValueConverterGroup
        {
                new BoolVisibilityConverter(),
                new BoolVisibilityConverter()
        };

        // Act
        var result = converter.Convert(new object[] { false, true }, null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_GivenFalseAndFalse_ReturnsFalse()
    {
        // Arrange
        var converter = new ValueConverterGroup
        {
                new BoolVisibilityConverter(),
                new BoolVisibilityConverter()
        };

        // Act
        var result = converter.Convert(new object[] { false, false }, null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }
}