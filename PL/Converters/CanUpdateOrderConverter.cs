using System;
using System.Globalization;
using System.Windows.Data;
using BO;

namespace PL.Converters;

/// <summary>
/// Converter that determines if an order can be updated based on its status.
/// An order can be updated only if it's in Open status.
/// </summary>
public class CanUpdateOrderConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not OrderStatus status)
            return false;

        // Order can be updated only when it's Open
        return status == OrderStatus.Open;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
