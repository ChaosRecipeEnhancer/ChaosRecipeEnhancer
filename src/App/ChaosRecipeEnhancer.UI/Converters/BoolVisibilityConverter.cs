using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;


/// <summary>
/// Converts a boolean value to a Visibility value.
/// </summary>
public sealed class BoolVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets a value indicating whether to collapse the element when not visible.
    /// If true, the element will be collapsed when not visible; otherwise, it will be hidden.
    /// Default value is true.
    /// </summary>
    public bool CollapseWhenNotVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to invert the visibility logic.
    /// If true, the visibility will be inverted; otherwise, it will follow the normal logic.
    /// Default value is false.
    /// </summary>
    public bool Invert { get; set; }

    /// <summary>
    /// Converts a boolean value to a Visibility value.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>The converted Visibility value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var visible = false;
        if (value is bool boolValue) visible = boolValue;

        if (Invert) visible = !visible;

        return visible
            ? Visibility.Visible
            : CollapseWhenNotVisible
                ? Visibility.Collapsed
                : Visibility.Hidden;
    }

    /// <summary>
    /// Converts a Visibility value back to a boolean value (not implemented).
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
