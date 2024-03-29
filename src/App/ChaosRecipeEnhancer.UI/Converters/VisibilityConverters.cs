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

/// <summary>
/// Converts an object value to a Visibility value based on equality comparison.
/// </summary>
public sealed class EqualityToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets a value indicating whether to invert the visibility logic.
    /// If true, the visibility will be inverted; otherwise, it will follow the normal logic.
    /// Default value is false.
    /// </summary>
    public bool Invert { get; set; }

    /// <summary>
    /// Gets or sets the type to be used for parameter comparison.
    /// If set, the parameter will be converted to this type before comparison.
    /// </summary>
    public Type ComparisonType { get; set; }

    /// <summary>
    /// Converts an object value to a Visibility value based on equality comparison.
    /// </summary>
    /// <param name="value">The object value to compare.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The parameter to compare against.</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>The converted Visibility value.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var castedParameter = parameter;
        if (ComparisonType is not null) castedParameter = System.Convert.ChangeType(parameter, ComparisonType);

        var equalityFunction = value == null ? new Func<object, bool>(x => x == null) : value.Equals;
        return Invert ? equalityFunction(castedParameter) ? Visibility.Collapsed : Visibility.Visible
            : equalityFunction(castedParameter) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            var castedParameter = parameter;
            if (ComparisonType is not null) castedParameter = System.Convert.ChangeType(parameter, ComparisonType);

            return Invert ? visibility == Visibility.Collapsed : visibility == Visibility.Visible;
        }

        return false;
    }
}

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