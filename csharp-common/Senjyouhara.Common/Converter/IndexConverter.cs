using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace Senjyouhara.Common.Converter;

// listview 序号
public class IndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        ListViewItem item = (ListViewItem)value;
        ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
        return listView.ItemContainerGenerator.IndexFromContainer(item) + 1;

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}