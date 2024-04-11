using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace NoteSHR.Components.List.Converters;

public class PrefixTypeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var item = (ListItem)value;

        return item?.Prefix switch
        {
            PrefixType.Bullet => "•",
            PrefixType.Number => $"{item.Index + 1}.",
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}