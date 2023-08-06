using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows;

namespace ChaosRecipeEnhancer.UI.WPF.Utilities;
public sealed class BoolVisibilityConverter : IValueConverter
{
	public bool CollapseWhenNotVisible { get; set; } = true;
	public bool Invert
	{
		get; set;
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		bool visible = false;
		if (value is bool boolValue)
		{
			visible = boolValue;
		}

		if (Invert)
		{
			visible = !visible;
		}

		return visible
			  ? Visibility.Visible
			  : (CollapseWhenNotVisible ? Visibility.Collapsed : Visibility.Hidden);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public sealed class BoolToObjectConverter : IValueConverter
{
	public object TrueValue
	{
		get; set;
	}
	public object FalseValue
	{
		get; set;
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is bool boolValue ? boolValue ? TrueValue : FalseValue : FalseValue;
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value.Equals(TrueValue);
}

public sealed class EqualityConverter : IValueConverter
{
	public bool Invert
	{
		get; set;
	}
	public Type ComparisonType
	{
		get; set;
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		object castedParameter = parameter;
		if (ComparisonType is not null)
		{
			castedParameter = System.Convert.ChangeType(parameter, ComparisonType);
		}

		var equalityFunction = value == null ? new Func<object, bool>(x => x == null) : value.Equals;
		return Invert ? !equalityFunction(castedParameter) : equalityFunction(castedParameter);
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public sealed class EqualityToVisibilityConverter : IValueConverter
{
	public bool Invert
	{
		get; set;
	}
	public Type ComparisonType
	{
		get; set;
	}

	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		object castedParameter = parameter;
		if (ComparisonType is not null)
		{
			castedParameter = System.Convert.ChangeType(parameter, ComparisonType);
		}

		var equalityFunction = value == null ? new Func<object, bool>(x => x == null) : value.Equals;
		return Invert ? equalityFunction(castedParameter) ? Visibility.Collapsed : Visibility.Visible
					  : equalityFunction(castedParameter) ? Visibility.Visible : Visibility.Collapsed;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public sealed class MultiBoolToBoolAndConverter : IMultiValueConverter
{
	public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => values.OfType<bool>().Aggregate((current, value) => current && value);

	public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

public sealed class InvertBoolConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is bool boolValue ? !boolValue : throw new ArgumentException();

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value is bool boolValue ? !boolValue : throw new ArgumentException();
}

public sealed class NullVisibilityConverter : IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		bool nullOrEmpty = value is string stringValue ? string.IsNullOrEmpty(stringValue) : value is null;
		return nullOrEmpty ? Visibility.Collapsed : Visibility.Visible;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}