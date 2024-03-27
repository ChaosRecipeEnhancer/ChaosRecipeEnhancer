using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

/// <summary>
/// Converts a string or object value to a Visibility value based on null or empty check.
/// </summary>
public sealed class NullOrEmptyVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts a string or object value to a Visibility value based on null or empty check.
    /// </summary>
    /// <param name="value">The string or object value to check.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>The converted Visibility value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var nullOrEmpty = value is string stringValue ? string.IsNullOrEmpty(stringValue) : value is null;
        return nullOrEmpty ? Visibility.Collapsed : Visibility.Visible;
    }

    /// <summary>
    /// Converts a Visibility value back to a string or object value (not implemented).
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