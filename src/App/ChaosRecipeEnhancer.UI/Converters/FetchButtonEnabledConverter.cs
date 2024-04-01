using System;
using System.Globalization;
using System.Windows.Data;

namespace ChaosRecipeEnhancer.UI.Converters;

public class FetchButtonEnabledConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2) return false;

        bool isButtonEnabled = values[0] is bool boolVal && boolVal;
        bool hasValidLeagueName = values[1] is string strVal && !string.IsNullOrWhiteSpace(strVal);

        return isButtonEnabled && hasValidLeagueName;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}