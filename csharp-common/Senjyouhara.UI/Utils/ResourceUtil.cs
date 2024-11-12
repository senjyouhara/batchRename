using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Senjyouhara.UI.Utils;

public class ResourceUtil
{

    /// <summary>
    /// 查找父元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindParent<T>(DependencyObject i_dp) where T : DependencyObject
    {
        DependencyObject dobj = (DependencyObject)VisualTreeHelper.GetParent(i_dp);
        if (dobj != null)
        {
            if (dobj is T)
            {
                return (T)dobj;
            }
            else
            {
                dobj = FindParent<T>(dobj);
                if (dobj != null && dobj is T)
                {
                    return (T)dobj;
                }
            }
        }
        return null;
    }
    
    /// <summary>
    /// 根据类型查找子元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="typename"></param>
    /// <returns></returns>
    public List<T> GetChildObjects<T>(DependencyObject obj, Type typename) where T : FrameworkElement
    {
        DependencyObject child = null;
        List<T> childList = new List<T>();

        for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
        {
            child = VisualTreeHelper.GetChild(obj, i);

            if (child is T && (((T)child).GetType() == typename))
            {
                childList.Add((T)child);
            }
            childList.AddRange(GetChildObjects<T>(child, typename));
        }
        return childList;
    }
}