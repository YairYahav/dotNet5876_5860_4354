namespace Helpers;
using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Text;


internal static class Tools
{

    public static string ToStringProperty<T>(this T t)
    {
        if (t == null) return "null";

        var sb = new StringBuilder();
        Type type = t.GetType();

        sb.AppendLine($"--- {type.Name} ---");

        foreach (PropertyInfo prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            try
            {
                object? value = prop.GetValue(t);
                string valueString;

                if (value is null)
                {
                    valueString = "null";
                }
                else if (value is IEnumerable enumerable and not string)
                {
                    // For collections (except strings), list the count or items
                    int count = enumerable.Cast<object>().Count();
                    valueString = count > 0 ? $"Collection with {count} items" : "Empty Collection";
                }
                else
                {
                    valueString = value.ToString() ?? "null";
                }

                sb.AppendLine($"  {prop.Name}: {valueString}");
            }
            catch (Exception ex)
            {
                sb.AppendLine($"  {prop.Name}: [Error retrieving value: {ex.Message}]");
            }
        }
        return sb.ToString();
    }
}