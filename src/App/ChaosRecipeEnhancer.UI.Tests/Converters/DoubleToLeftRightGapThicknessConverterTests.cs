using ChaosRecipeEnhancer.UI.Converters;
using System.Globalization;
using System.Windows;

namespace ChaosRecipeEnhancer.Tests.Converters;

public class DoubleToLeftRightGapThicknessConverterTests
{
    [Fact]
    public void Convert_DoubleValue_ReturnsThicknessWithLeftRightGap()
    {
        // Arrange
        var converter = new DoubleToLeftRightGapThicknessConverter();

        // Act
        var result = converter.Convert(10.5, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<Thickness>(result);
        Assert.Equal(new Thickness(10.5, 0, 10.5, 0), result);
    }

    [Fact]
    public void Convert_NonDoubleValue_ReturnsUnsetValue()
    {
        // Arrange
        var converter = new DoubleToLeftRightGapThicknessConverter();

        // Act
        var result = converter.Convert("InvalidValue", null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(DependencyProperty.UnsetValue, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange
        var converter = new DoubleToLeftRightGapThicknessConverter();

        // Act and Assert
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(new Thickness(10, 0, 10, 0), null, null, CultureInfo.InvariantCulture));
    }
}