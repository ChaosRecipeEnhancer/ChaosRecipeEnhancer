using ChaosRecipeEnhancer.UI.Converters;
using System.Globalization;
using System.Windows;

namespace ChaosRecipeEnhancer.UI.Tests.Converters;

public class ThicknessConvertersTests
{
    [Fact]
    public void PaddingLeftRightConverter_Convert_ValidInput_ReturnsCorrectThickness()
    {
        var converter = new PaddingLeftRightConverter();
        var result = converter.Convert(5.0, typeof(Thickness), null, CultureInfo.InvariantCulture);

        Assert.IsType<Thickness>(result);
        var thickness = (Thickness)result;
        Assert.Equal(new Thickness(5, 2, 5, 2), thickness);
    }

    [Fact]
    public void PaddingLeftRightConverter_Convert_InvalidInput_ReturnsUnsetValue()
    {
        var converter = new PaddingLeftRightConverter();
        var result = converter.Convert("not a double", typeof(Thickness), null, CultureInfo.InvariantCulture);

        Assert.Equal(DependencyProperty.UnsetValue, result);
    }

    [Fact]
    public void PaddingLeftRightConverter_ConvertBack_ThrowsNotImplementedException()
    {
        var converter = new PaddingLeftRightConverter();
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(new Thickness(), typeof(double), null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void DoubleToLeftMarginConverter_Convert_ValidInput_ReturnsCorrectThickness()
    {
        var converter = new DoubleToLeftMarginConverter();
        var result = converter.Convert(10.0, typeof(Thickness), null, CultureInfo.InvariantCulture);

        Assert.IsType<Thickness>(result);
        var thickness = (Thickness)result;
        Assert.Equal(new Thickness(10, 0, 0, 0), thickness);
    }

    [Fact]
    public void DoubleToLeftMarginConverter_Convert_InvalidInput_ReturnsUnsetValue()
    {
        var converter = new DoubleToLeftMarginConverter();
        var result = converter.Convert("not a double", typeof(Thickness), null, CultureInfo.InvariantCulture);

        Assert.Equal(DependencyProperty.UnsetValue, result);
    }

    [Fact]
    public void DoubleToLeftMarginConverter_ConvertBack_ThrowsNotImplementedException()
    {
        var converter = new DoubleToLeftMarginConverter();
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(new Thickness(), typeof(double), null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void DoubleToLeftRightThicknessConverter_Convert_ValidInput_ReturnsCorrectThickness()
    {
        var converter = new DoubleToLeftRightThicknessConverter();
        var result = converter.Convert(7.5, typeof(Thickness), null, CultureInfo.InvariantCulture);

        Assert.IsType<Thickness>(result);
        var thickness = (Thickness)result;
        Assert.Equal(new Thickness(7.5, 5, 7.5, 0), thickness);
    }

    [Fact]
    public void DoubleToLeftRightThicknessConverter_Convert_InvalidInput_ReturnsUnsetValue()
    {
        var converter = new DoubleToLeftRightThicknessConverter();
        var result = converter.Convert("not a double", typeof(Thickness), null, CultureInfo.InvariantCulture);

        Assert.Equal(DependencyProperty.UnsetValue, result);
    }

    [Fact]
    public void DoubleToLeftRightThicknessConverter_ConvertBack_ThrowsNotImplementedException()
    {
        var converter = new DoubleToLeftRightThicknessConverter();
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(new Thickness(), typeof(double), null, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void DoubleToTopThicknessConverter_Convert_ValidInput_ReturnsCorrectThickness()
    {
        var converter = new DoubleToTopThicknessConverter();
        var result = converter.Convert(15.0, typeof(Thickness), null, CultureInfo.InvariantCulture);

        Assert.IsType<Thickness>(result);
        var thickness = (Thickness)result;
        Assert.Equal(new Thickness(0, 15, 0, 0), thickness);
    }

    [Fact]
    public void DoubleToTopThicknessConverter_Convert_InvalidInput_ReturnsUnsetValue()
    {
        var converter = new DoubleToTopThicknessConverter();
        var result = converter.Convert("not a double", typeof(Thickness), null, CultureInfo.InvariantCulture);

        Assert.Equal(DependencyProperty.UnsetValue, result);
    }

    [Fact]
    public void DoubleToTopThicknessConverter_ConvertBack_ThrowsNotImplementedException()
    {
        var converter = new DoubleToTopThicknessConverter();
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(new Thickness(), typeof(double), null, CultureInfo.InvariantCulture));
    }
}
