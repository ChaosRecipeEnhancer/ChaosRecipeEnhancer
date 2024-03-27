using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ChaosRecipeEnhancer.UI.Converters;

/// <summary>
/// Converts between a string value and a Color value.
/// </summary>
[ValueConversion(typeof(string), typeof(Color))]
public sealed class StringColorConverter : IValueConverter
{
    /// <summary>
    /// Converts a string value to a Color value.
    /// </summary>
    /// <param name="value">The string value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>The converted Color value if the input is a non-empty string; otherwise, null.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
            {
                return (Color)ColorConverter.ConvertFromString(stringValue);
            }
        }
        catch (FormatException)
        {
            // not a valid color, return empty
        }

        // return null if the conversion fails
        return null;

    }

    /// <summary>
    /// Converts a Color value back to a string value.
    /// </summary>
    /// <param name="value">The Color value to convert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>The string representation of the Color value if the input is a Color; otherwise, an empty string.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Color colorValue)
        {
            return colorValue.ToString();
        }

        return string.Empty;
    }
}