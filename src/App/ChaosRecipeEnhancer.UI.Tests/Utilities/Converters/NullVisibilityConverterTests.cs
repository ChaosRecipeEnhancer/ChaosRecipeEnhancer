using System.Windows;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

namespace ChaosRecipeEnhancer.UI.Tests.Utilities.Converters;

public class NullVisibilityConverterTests
{
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