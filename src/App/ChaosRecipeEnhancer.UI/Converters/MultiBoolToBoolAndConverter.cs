using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

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
