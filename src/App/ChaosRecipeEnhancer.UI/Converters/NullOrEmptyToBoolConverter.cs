using System;
using System.Globalization;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

/// <summary>
/// Converts a string or object value to a boolean value based on null or empty check.
/// </summary>
public sealed class NullOrEmptyToBoolConverter : IValueConverter
{
    /// <summary>
    /// Converts a string or object value to a boolean value based on null or empty check.
    /// </summary>
    /// <param name="value">The string or object value to check.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>False if the value is null or empty; otherwise, true.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var nullOrEmpty = value is string stringValue ? string.IsNullOrEmpty(stringValue) : value is null;
        return nullOrEmpty ? false : true;
    }

    /// <summary>
    /// Converts a boolean value back to a string or object value (not implemented).
    /// </summary>
    /// <param name="value">The boolean value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>Throws a NotImplementedException.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}