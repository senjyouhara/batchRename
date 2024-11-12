

using System;
using System.Windows;
using System.Windows.Controls;

namespace Senjyouhara.UI.Extensions
{
    public class ListViewColumnWidthProperty
    {

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.RegisterAttached(
                "MinWidth",
                typeof(double),
                typeof(ListViewColumnWidthProperty), new FrameworkPropertyMetadata(double.NaN));

        public static readonly DependencyProperty MaxWidthProperty =
            DependencyProperty.RegisterAttached(
                "MaxWidth",
                typeof(double),
                typeof(ListViewColumnWidthProperty), new FrameworkPropertyMetadata(double.NaN));

        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached(
            name: "Width",
            propertyType: typeof(GridLength),
            ownerType: typeof(ListViewColumnWidthProperty),
            defaultMetadata: new FrameworkPropertyMetadata(GridLength.Auto));

        public static GridLength GetWidth(DependencyObject dependencyObject) 
            => (GridLength)dependencyObject.GetValue(WidthProperty);

        public static void SetWidth(DependencyObject dependencyObject, string value) 
            => dependencyObject.SetValue(WidthProperty, value);
        
        public static double GetMinWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(MinWidthProperty);
        } 

        public static void SetMinWidth(DependencyObject obj, double minWidth)
        {
            obj.SetValue(MinWidthProperty, minWidth);
        } 

        public static double GetMaxWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(MaxWidthProperty);
        } 

        public static void SetMaxWidth(DependencyObject obj, double maxWidth)
        {
            obj.SetValue(MaxWidthProperty, maxWidth);
        } 
        
        public static bool IsValidRangeColumn(GridViewColumn column)
        {
            if (column == null)
            {
                return false;
            }
            return !double.IsNaN(GetMinWidth(column)) || !double.IsNaN(GetMaxWidth(column));
        } 
        
    } 
}
