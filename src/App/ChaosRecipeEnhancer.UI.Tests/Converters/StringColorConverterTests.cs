using ChaosRecipeEnhancer.UI.Converters;
using System.Globalization;
using System.Windows.Media;

namespace ChaosRecipeEnhancer.UI.Tests.Converters;

public class StringColorConverterTests
{
    [Fact]
    public void Convert_ValidColorString_ReturnsColor()
    {
        // Arrange
        var converter = new StringColorConverter();

        // Act
        var result = converter.Convert("#FF0000", null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Colors.Red, result);
    }

    [Fact]
    public void Convert_EmptyString_ReturnsNull()
    {
        // Arrange
        var converter = new StringColorConverter();

        // Act
        var result = converter.Convert(string.Empty, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_InvalidColorString_ReturnsNull()
    {
        // Arrange
        var converter = new StringColorConverter();

        // Act
        var result = converter.Convert("InvalidColor", null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_Color_ReturnsString()
    {
        // Arrange
        var converter = new StringColorConverter();

        // Act
        var result = converter.ConvertBack(Colors.Blue, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("#FF0000FF", result);
    }

    [Fact]
    public void ConvertBack_NonColor_ReturnsEmptyString()
    {
        // Arrange
        var converter = new StringColorConverter();

        // Act
        var result = converter.ConvertBack(123, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}