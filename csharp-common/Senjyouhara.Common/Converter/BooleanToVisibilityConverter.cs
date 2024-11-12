﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Senjyouhara.Common.Converter;

/// <summary>
/// 布尔值转换为可见性
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
    ///当界面的绑定到DataContext中的属性发生变化时，会调用该方法，将绑定的bool值转换为界面需要的Visibility类型的值
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Visibility.Visible : Visibility.Collapsed;
    }
        
    ///当界面的Visibility值发生变化时，会调用该方法，将Visibility类型的值转换为bool值返回给绑定到DataContext中的属性
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            Visibility v = (Visibility)value;
            if (value == null)
            {
                return false;
            }

            if (v == Visibility.Visible)
            {
                return true;
            }
            else
                return false;
        }
        catch (System.NullReferenceException e)
        {
            Console.WriteLine(e);
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}