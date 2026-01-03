using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters;

public class ModeToPrimaryButtonTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool isAdd && isAdd) ? "הוסף" : "עדכן";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
