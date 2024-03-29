using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

/// <summary>
/// Converts a double value to a Thickness value with the double value as the left and right gap.
/// </summary>
public class DoubleToLeftRightThicknessConverter : IValueConverter
{
    /// <summary>
    /// Converts a double value to a Thickness value with the double value as the left and right gap.
    /// </summary>
    /// <param name="value">The double value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>A Thickness value with the double value as the left and right gap if the input is a double; otherwise, DependencyProperty.UnsetValue.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double gapValue)
        {
            // HACK: Hardcoded top and bottom values for stashtabs
            return new Thickness(gapValue, 5, gapValue, 0);
        }

        return DependencyProperty.UnsetValue;
    }

    /// <summary>
    /// Converts a Thickness value back to a double value (not implemented).
    /// </summary>
    /// <param name="value">The Thickness value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>Throws a NotImplementedException.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts a double value to a Thickness value with the double value as the top gap.
/// </summary>
public class DoubleToTopThicknessConverter : IValueConverter
{
    /// <summary>
    /// Converts a double value to a Thickness value with the double value as the top gap.
    /// </summary>
    /// <param name="value">The double value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>A Thickness value with the double value as the top gap if the input is a double; otherwise, DependencyProperty.UnsetValue.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double gapValue)
        {
            return new Thickness(0, gapValue, 0, 0);
        }

        return DependencyProperty.UnsetValue;
    }

    /// <summary>
    /// Converts a Thickness value back to a double value (not implemented).
    /// </summary>
    /// <param name="value">The Thickness value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>Throws a NotImplementedException.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}