using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace NoteSHR.Components.List.Converters;

public class PrefixTypeToStringConverter : IMultiValueConverter
{
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        var prefixType = (PrefixType)values[0];
        var index = (int)values[1];

        return prefixType switch
        {
            PrefixType.Bullet => "•",
            PrefixType.Numbered => $"{index + 1}.",
            _ => throw new ArgumentOutOfRangeException(nameof(values), values, null)
        };
    }
}