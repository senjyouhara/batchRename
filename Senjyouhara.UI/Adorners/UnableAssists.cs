using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Senjyouhara.UI.Adorners;



// 附加组件用于禁用状态无法修改鼠标手势, 通过附加组件方式来实现禁用手势
// 使用方式：   <Setter Property="Adorners:UnableAssists.IsDisabled" Value="False"></Setter>
// 或者    <Button Adorners:UnableAssists.IsDisabled="{Binding IsDisabled, RelativeSource={RelativeSource Self}}" ></Button>
public class UnableAssists
{
    public static bool GetIsDisabled(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsDisabledProperty);
    }
 
    public static void SetIsDisabled(DependencyObject obj, bool value)
    {
        obj.SetValue(IsDisabledProperty, value);
    }
 
    // Using a DependencyProperty as the backing store for IsDisabled.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsDisabledProperty =
        DependencyProperty.RegisterAttached("IsDisabled", typeof(bool), typeof(UnableAssists), new PropertyMetadata(true, IsDisabledPropertyChanged));
 
    private static void IsDisabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d == null)
        {
            return;
        }

        // var IsDisabled = (bool)d.GetValue(IsDisabledProperty);
        
        if (d is UIElement uIElement)
        {
            if (!(uIElement as FrameworkElement).IsLoaded)
            {
                if (IsDisabledProperty != null)
                {
                    RoutedEventHandler l = null;
                    l = (s, e) =>
                    {
                        var value = GetIsDisabled(uIElement);
                        if (value != null)
                        {
                            var layer = AdornerLayer.GetAdornerLayer(uIElement);
                            if (layer == null)
                            {
                                // uIElement.UpdateLayout();
                                Debug.WriteLine("获取控件装饰层失败，控件可能没有装饰层！ loaded");
                                return;
                            }

                            if (!value)
                            {
                                layer.Add(new UnableAdorner(uIElement));
                            }
                        }
                        (uIElement as FrameworkElement).Loaded -= l;
                    };
                    (uIElement as FrameworkElement).Loaded += l;
                }
                return;
            }
            
            var layer = AdornerLayer.GetAdornerLayer(uIElement);
            if (layer == null)
            {
                Debug.WriteLine("获取控件装饰层失败，控件可能没有装饰层！");
                return;
                // throw new Exception("获取控件装饰层失败，控件可能没有装饰层！");
            }
            
            var adorners = layer.GetAdorners(uIElement);

            if (adorners != null)
            {
                foreach (var i in adorners)
                {
                    if (i is UnableAdorner unableAdorner)
                    {
                        if ((bool)e.NewValue)
                        {
                            layer.Remove(unableAdorner);
                        }
                    }
                }
            }
            else
            {
                if (IsDisabledProperty != null)
                {
                    layer.Add(new UnableAdorner(uIElement));
                }
            }

        }
    }
 
 
}