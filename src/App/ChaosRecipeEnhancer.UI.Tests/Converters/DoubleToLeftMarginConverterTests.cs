using ChaosRecipeEnhancer.UI.Converters;
using System.Globalization;
using System.Windows;

namespace ChaosRecipeEnhancer.Tests.Converters;

public class DoubleToLeftMarginConverterTests
{
    [Fact]
    public void Convert_DoubleValue_ReturnsThicknessWithLeftMargin()
    {
        // Arrange
        var converter = new DoubleToLeftMarginConverter();

        // Act
        var result = converter.Convert(10.5, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<Thickness>(result);
        Assert.Equal(new Thickness(10.5, 0, 0, 0), result);
    }

    [Fact]
    public void Convert_NonDoubleValue_ReturnsUnsetValue()
    {
        // Arrange
        var converter = new DoubleToLeftMarginConverter();

        // Act
        var result = converter.Convert("InvalidValue", null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(DependencyProperty.UnsetValue, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange
        var converter = new DoubleToLeftMarginConverter();

        // Act and Assert
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(new Thickness(10, 0, 0, 0), null, null, CultureInfo.InvariantCulture));
    }
}