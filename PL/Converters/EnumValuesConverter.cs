using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace PL.Converters;

public class EnumValuesConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Type t && t.IsEnum)
            return Enum.GetValues(t).Cast<object>().ToList();
        return Array.Empty<object>();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
