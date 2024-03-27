using ChaosRecipeEnhancer.UI.Converters;
using System.Globalization;
using System.Windows;

namespace ChaosRecipeEnhancer.UI.Tests.Converters;

public class PaddingLeftRightConverterTests
{
    [Fact]
    public void Convert_DoubleValue_ReturnsThicknessWithLeftRightPadding()
    {
        // Arrange
        var converter = new PaddingLeftRightConverter();

        // Act
        var result = converter.Convert(10.5, null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<Thickness>(result);
        Assert.Equal(new Thickness(10.5, 2, 10.5, 2), result);
    }

    [Fact]
    public void Convert_NonDoubleValue_ReturnsUnsetValue()
    {
        // Arrange
        var converter = new PaddingLeftRightConverter();

        // Act
        var result = converter.Convert("InvalidValue", null, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(DependencyProperty.UnsetValue, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        // Arrange
        var converter = new PaddingLeftRightConverter();

        // Act and Assert
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(new Thickness(10, 2, 10, 2), null, null, CultureInfo.InvariantCulture));
    }
}