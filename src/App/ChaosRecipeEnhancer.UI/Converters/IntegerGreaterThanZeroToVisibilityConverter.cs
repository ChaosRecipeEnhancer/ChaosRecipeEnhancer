using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

/// <summary>
/// Converts an integer value to a Visibility value based on whether the integer is greater than zero.
/// Allows inverting the behavior using the Invert property.
/// </summary>
public class IntegerGreaterThanZeroToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets a value indicating whether to invert the conversion behavior.
    /// If true, the converter will return Visibility.Visible when the integer is zero and Visibility.Collapsed otherwise.
    /// If false (default), the converter will return Visibility.Visible when the integer is greater than zero and Visibility.Collapsed otherwise.
    /// </summary>
    public bool Invert { get; set; }

    /// <summary>
    /// Converts an integer value to a Visibility value.
    /// </summary>
    /// <param name="value">The integer value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>
    /// If Invert is false (default), returns Visibility.Visible if the integer value is greater than zero; otherwise, Visibility.Collapsed.
    /// If Invert is true, returns Visibility.Visible if the integer value is zero; otherwise, Visibility.Collapsed.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int count)
        {
            if (Invert)
            {
                return count == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        return Visibility.Collapsed;
    }

    /// <summary>
    /// Converts a Visibility value back to an integer value (not implemented).
    /// </summary>
    /// <param name="value">The Visibility value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>Throws a NotImplementedException.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}