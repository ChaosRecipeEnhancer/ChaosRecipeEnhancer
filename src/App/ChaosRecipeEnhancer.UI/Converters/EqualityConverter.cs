using System;
using System.Globalization;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

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