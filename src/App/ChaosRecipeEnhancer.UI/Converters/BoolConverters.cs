using System;
using System.Globalization;
using System.Linq;
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

/// <summary>
/// Converts an object value to a boolean value based on equality comparison.
/// </summary>
public sealed class EqualityConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets a value indicating whether to invert the equality comparison result.
    /// If true, the result will be inverted; otherwise, it will follow the normal logic.
    /// Default value is false.
    /// </summary>
    public bool Invert { get; set; }

    /// <summary>
    /// Gets or sets the type to be used for parameter comparison.
    /// If set, the parameter will be converted to this type before comparison.
    /// </summary>
    public Type ComparisonType { get; set; }

    /// <summary>
    /// Converts an object value to a boolean value based on equality comparison.
    /// </summary>
    /// <param name="value">The object value to compare.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The parameter to compare against.</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>The boolean result of the equality comparison.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var castedParameter = parameter;
        if (ComparisonType is not null) castedParameter = System.Convert.ChangeType(parameter, ComparisonType);

        var equalityFunction = value == null ? new Func<object, bool>(x => x == null) : value.Equals;
        return Invert ? !equalityFunction(castedParameter) : equalityFunction(castedParameter);
    }

    /// <summary>
    /// Converts a boolean value back to an object value (not implemented).
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

/// <summary>
/// Converts multiple boolean values to a single boolean value using the AND operation.
/// </summary>
public sealed class MultiBoolToBoolAndConverter : IMultiValueConverter
{
    /// <summary>
    /// Converts multiple boolean values to a single boolean value using the AND operation.
    /// </summary>
    /// <param name="values">The array of boolean values to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>True if all boolean values are true; otherwise, false.</returns>
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.OfType<bool>().Aggregate((current, value) => current && value);
    }

    /// <summary>
    /// Converts a single boolean value back to multiple boolean values (not implemented).
    /// </summary>
    /// <param name="value">The single boolean value to convert back.</param>
    /// <param name="targetTypes">The target types of the conversion.</param>
    /// <param name="parameter">Additional parameter for the conversion (not used).</param>
    /// <param name="culture">The culture information for the conversion.</param>
    /// <returns>Throws a NotImplementedException.</returns>
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Converts an enum value to a boolean value based on a specified parameter.
/// </summary>
[ValueConversion(typeof(Enum), typeof(bool))]
public class EnumBooleanConverter : IValueConverter
{
    /// <summary>
    /// Converts an enum value to a boolean value.
    /// </summary>
    /// <param name="value">The enum value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The parameter to compare the enum value against.</param>
    /// <param name="culture">The culture information.</param>
    /// <returns>True if the enum value equals the specified parameter; otherwise, false.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Check if the enum value equals the specified parameter
        return value.Equals(parameter);
    }

    /// <summary>
    /// Converts a boolean value back to an enum value.
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The target type of the conversion.</param>
    /// <param name="parameter">The parameter to return if the boolean value is true.</param>
    /// <param name="culture">The culture information.</param>
    /// <returns>The specified parameter if the boolean value is true; otherwise, <see cref="Binding.DoNothing"/>.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // Check if the boolean value is true
        // If true, return the specified parameter
        // If false, return Binding.DoNothing to indicate that no conversion should occur
        return value.Equals(true) ? parameter : Binding.DoNothing;
    }
}

public class BooleanToDoubleConverter : IValueConverter
{
    public double TrueValue { get; set; }
    public double FalseValue { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolean)
        {
            return boolean ? TrueValue : FalseValue;
        }
        return FalseValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}