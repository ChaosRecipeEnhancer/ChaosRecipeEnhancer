using ChaosRecipeEnhancer.UI.Converters;
using System.Globalization;
using System.Windows;

namespace ChaosRecipeEnhancer.UI.Tests.Converters;

public class NullOrEmptyVisibilityConverterTests
{
    [Fact]
    public void Convert_NullValue_ReturnsCollapsed()
    {
        // Arrange
        var converter = new NullOrEmptyVisibilityConverter();

        // Act
        var result = converter.Convert(null, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsCollapsed()
    {
        // Arrange
        var converter = new NullOrEmptyVisibilityConverter();

        // Act
        var result = converter.Convert(string.Empty, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Collapsed, result);
    }

    [Fact]
    public void Convert_NonEmptyString_ReturnsVisible()
    {
        // Arrange
        var converter = new NullOrEmptyVisibilityConverter();

        // Act
        var result = converter.Convert("Hello", null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_GivenNull_ReturnsCollapsed()
    {
        // Arrange
        var converter = new NullOrEmptyVisibilityConverter();

        // Act
        var result = converter.Convert(null, null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_GivenEmptyString_ReturnsCollapsed()
    {
        // Arrange
        var converter = new NullOrEmptyVisibilityConverter();

        // Act
        var result = converter.Convert(string.Empty, null, null, null);

        // Assert
        result.Should().Be(Visibility.Collapsed);
    }

    [Fact]
    public void Convert_GivenNotNull_ReturnsVisible()
    {
        // Arrange
        var converter = new NullOrEmptyVisibilityConverter();

        // Act
        var result = converter.Convert("test", null, null, null);

        // Assert
        result.Should().Be(Visibility.Visible);
    }
}