using System;
using System.Globalization;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

/// <summary>
/// Inverts a boolean value.
/// </summary>
public sealed class InvertBoolConverter : IValueConverter
{
    /// <summary>
    /// Inverts a boolean value.
    /// </summary>
    /// <param name="value">The boolean value to invert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>The inverted boolean value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : throw new ArgumentException();
    }

    /// <summary>
    /// Inverts a boolean value back.
    /// </summary>
    /// <param name="value">The boolean value to invert back.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>The inverted boolean value.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : throw new ArgumentException();
    }
}