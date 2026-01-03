using System;
using System.Globalization;
using System.Windows.Data;
using BO;

namespace PL.Converters;

public class OrderStatusToBoolConverter : IValueConverter
{
    // ConverterParameter: "Cancel" או "Update"
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not OrderStatus s) return false;
        string mode = parameter as string ?? "";

        return mode switch
        {
            "Cancel" => s == OrderStatus.Open || s == OrderStatus.InProgress,
            "Update" => s == OrderStatus.Open,
            _ => false
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
