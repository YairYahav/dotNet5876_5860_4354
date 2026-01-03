using System;
using System.Globalization;
using System.Windows.Data;
using BO;

namespace PL.Converters;

public class OrderStatusCanCancelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not OrderStatus status)
            return false;

        return status == OrderStatus.Open || status == OrderStatus.InProgress;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
