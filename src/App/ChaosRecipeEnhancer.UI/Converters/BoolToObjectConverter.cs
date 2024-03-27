using System;
using System.Globalization;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

/// <summary>
/// Converts a boolean value to an object value.
/// </summary>
public sealed class BoolToObjectConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets the value to return when the boolean value is true.
    /// </summary>
    public object TrueValue { get; set; }

    /// <summary>
    /// Gets or sets the value to return when the boolean value is false.
    /// </summary>
    public object FalseValue { get; set; }

    /// <summary>
    /// Converts a boolean value to an object value.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>The object value corresponding to the boolean value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue ? boolValue ? TrueValue : FalseValue : FalseValue;
    }

    /// <summary>
    /// Converts an object value back to a boolean value.
    /// </summary>
    /// <param name="value">The object value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>True if the object value equals the TrueValue; otherwise, false.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.Equals(TrueValue);
    }
}
