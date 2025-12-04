using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters;

public class StatusToBooleanConverter : IValueConverter
{
    // Converts the ButtonText ("Add" or "Update") to a boolean for IsReadOnly property
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string text && text == "Update")
        {
            return true; // If Update mode, ReadOnly is true
        }
        return false; // If Add mode, ReadOnly is false
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}