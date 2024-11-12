using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Senjyouhara.Common.Converter;

/// <summary>
/// 对象转布尔值
/// </summary>
public class ObjectToBooleanConverter : IValueConverter
{
    ///当界面的绑定到DataContext中的属性发生变化时，会调用该方法，将绑定的object值转换为界面需要的Boolean类型的值
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is bool val)
        {
            return val;
        }

        if (value is int valN)
        {
            return valN != 0;
        }

        if (value is IEnumerable enumerable)
        {
            var result = enumerable.GetEnumerator().MoveNext();
            return result;
        }

        if (value is not null)
        {
            return true;
        }
        
        return false;
    }
        
    ///当界面的Boolean值发生变化时，会调用该方法，将Boolean类型的值转换为Object值返回给绑定到DataContext中的属性
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}