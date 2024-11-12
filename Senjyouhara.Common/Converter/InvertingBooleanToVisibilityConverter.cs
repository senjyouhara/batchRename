using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Senjyouhara.Common.Converter;

public class InvertingBooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var vis = bool.Parse(value.ToString());
        return vis ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var vis = (Visibility)value;
        return vis == Visibility.Collapsed;
    }
}