using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

/// <summary>
/// Converts an Enum value to a Visibility state. Supports direct and inverted conversion.
/// </summary>
public class EnumToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Converts an Enum value to a Visibility state.
    /// </summary>
    /// <param name="value">The Enum value to convert.</param>
    /// <param name="targetType">The target type of the conversion (ignored).</param>
    /// <param name="parameter">The Enum value to compare against. If equal, the visibility is Visible (or Collapsed if Invert is true).</param>
    /// <param name="culture">The culture to use in the converter (ignored).</param>
    /// <returns>Visibility.Visible if the value equals the parameter (and not inverted); otherwise, Visibility.Collapsed.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return Visibility.Collapsed;

        var currentState = value.ToString();
        var targetState = parameter.ToString();
        bool isVisible = currentState.Equals(targetState, StringComparison.InvariantCultureIgnoreCase);

        if (Invert)
            isVisible = !isVisible;

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Back conversion is not supported and will throw an exception if used.
    /// </summary>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("Back conversion is not supported.");
    }

    /// <summary>
    /// Gets or sets a value indicating whether the conversion logic should be inverted.
    /// </summary>
    public bool Invert { get; set; }
}