using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

public sealed class BoolVisibilityConverter : IValueConverter
{
    public bool CollapseWhenNotVisible { get; set; } = true;
    public bool Invert { get; set; }

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

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public sealed class BoolToObjectConverter : IValueConverter
{
    public object TrueValue { get; set; }
    public object FalseValue { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue ? boolValue ? TrueValue : FalseValue : FalseValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value.Equals(TrueValue);
    }
}

public sealed class EqualityConverter : IValueConverter
{
    public bool Invert { get; set; }
    public Type ComparisonType { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var castedParameter = parameter;
        if (ComparisonType is not null) castedParameter = System.Convert.ChangeType(parameter, ComparisonType);

        var equalityFunction = value == null ? new Func<object, bool>(x => x == null) : value.Equals;
        return Invert ? !equalityFunction(castedParameter) : equalityFunction(castedParameter);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public sealed class EqualityToVisibilityConverter : IValueConverter
{
    public bool Invert { get; set; }
    public Type ComparisonType { get; set; }

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
        throw new NotImplementedException();
    }
}

public sealed class MultiBoolToBoolAndConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return values.OfType<bool>().Aggregate((current, value) => current && value);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public sealed class InvertBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : throw new ArgumentException();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is bool boolValue ? !boolValue : throw new ArgumentException();
    }
}

public sealed class NullOrEmptyVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var nullOrEmpty = value is string stringValue ? string.IsNullOrEmpty(stringValue) : value is null;
        return nullOrEmpty ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public sealed class ValueConverterGroup : List<IValueConverter>, IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return this.Aggregate(value, (current, converter) => converter.Convert(current, targetType, parameter, culture));
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return this.Reverse<IValueConverter>().Aggregate(value, (current, converter) => converter.ConvertBack(current, targetType, parameter, culture));
    }
}

[ValueConversion(typeof(string), typeof(Color))]
internal sealed class StringColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && !string.IsNullOrEmpty(stringValue))
        {

            return (Color)ColorConverter.ConvertFromString(stringValue);
        }

        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Color colorValue)
        {
            return colorValue.ToString();
        }

        return string.Empty;
    }
}


public sealed class DoubleToLeftMarginConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double marginValue)
        {
            return new Thickness(marginValue, 0, 0, 0);
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class DoubleToLeftRightGapThicknessConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double gapValue)
        {
            return new Thickness(gapValue, 0, gapValue, 0);
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class PaddingLeftRightConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double paddingValue)
        {
            return new Thickness(paddingValue, 2, paddingValue, 2);
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}