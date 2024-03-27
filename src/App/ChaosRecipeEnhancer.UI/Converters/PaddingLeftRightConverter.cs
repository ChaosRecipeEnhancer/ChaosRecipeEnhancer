using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

/// <summary>
/// Converts a double value to a Thickness value with the double value as the left and right padding and a fixed top and bottom padding of 2.
/// </summary>
public class PaddingLeftRightConverter : IValueConverter
{
    /// <summary>
    /// Converts a double value to a Thickness value with the double value as the left and right padding and a fixed top and bottom padding of 2.
    /// </summary>
    /// <param name="value">The double value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>A Thickness value with the double value as the left and right padding and a fixed top and bottom padding of 2 if the input is a double; otherwise, DependencyProperty.UnsetValue.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double paddingValue)
        {
            return new Thickness(paddingValue, 2, paddingValue, 2);
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