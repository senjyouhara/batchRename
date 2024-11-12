using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Senjyouhara.UI.Extensions;

public class PanelExtensions
{
    #region HorizontalContentAlignment

    public static readonly DependencyProperty HorizontalContentAlignmentProperty =
        DependencyProperty.RegisterAttached("HorizontalContentAlignment",
            typeof(HorizontalAlignment), typeof(PanelExtensions),
            new FrameworkPropertyMetadata(HorizontalAlignment.Left,
                OnHorizontalContentAlignmentChanged));

    public static void SetHorizontalContentAlignment(Panel d, HorizontalAlignment value)
    {
        d.SetValue(HorizontalContentAlignmentProperty, value);
    }

    public static HorizontalAlignment GetHorizontalContentAlignment(Panel d)
    {
        return (HorizontalAlignment)d.GetValue(HorizontalContentAlignmentProperty);
    }

    static void OnHorizontalContentAlignmentChanged
        (object sender, DependencyPropertyChangedEventArgs e)
    {
        var Panel = sender as Panel;

        Panel.SizeChanged -= OnHorizontalContentAlignmentUpdated;
        Panel.SizeChanged += OnHorizontalContentAlignmentUpdated;

        OnHorizontalContentAlignmentUpdated(Panel, null);
    }

    static void OnHorizontalContentAlignmentUpdated(object sender, SizeChangedEventArgs e)
    {
        var p = sender as Panel;
        var a = GetHorizontalContentAlignment(p);

        for (int i = 0, Count = p.Children.Count; i < Count; i++)
            (p.Children[i] as FrameworkElement).HorizontalAlignment = a;
    }

    #endregion

    #region Spacing

    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.RegisterAttached("Spacing", typeof(Thickness),
            typeof(PanelExtensions), new FrameworkPropertyMetadata(default(Thickness), OnSpacingChanged));

    public static void SetSpacing(Panel d, Thickness value)
    {
        d.SetValue(SpacingProperty, value);
    }

    public static Thickness GetSpacing(Panel d)
    {
        return (Thickness)d.GetValue(SpacingProperty);
    }

    static void OnSpacingChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        var Panel = sender as Panel;
        Panel.SizeChanged -= OnSpacingUpdated;
        Panel.SizeChanged += OnSpacingUpdated;

        // OnSpacingUpdated(Panel, null);
    }

    private static Thickness EmptyCompareObj = new();
    private static Dictionary<string, Thickness> OriginMarginDict = new();
    static void OnSpacingUpdated(object sender, SizeChangedEventArgs e)
    {
        var p = sender as Panel;
        var s = GetSpacing(p);

        var tf = GetTrimFirst(p);
        var tl = GetTrimLast(p);

        
        
        List<UIElement> filterChildren = new();
        foreach (UIElement pChild in p.Children)
        {
            if (pChild.Visibility == Visibility.Visible)
            {
                filterChildren.Add(pChild);
            }
        }
    
        for (int i = 0, Count = filterChildren.Count; i < Count; i++)
        {
            var Element = filterChildren[i] as FrameworkElement;
            if(Element == null) continue;
            
            if (string.IsNullOrEmpty(Element.Uid))
            {
                var uid = Guid.NewGuid().ToString();
                Element.Uid = uid;
                OriginMarginDict.Add(uid, Element.Margin);
            }
            
            if ((i == 0 && tf) || (i == (Count - 1) && tl))
            {
                if (!Element.Margin.Equals(EmptyCompareObj))
                {
                    var Margin = OriginMarginDict[Element.Uid];
                    Element.Margin = new Thickness()
                    {
                        Left =  i == 0 ? 0 : s.Left + Margin.Left, 
                        Top =  i == 0 ? 0 : s.Top + Margin.Top, 
                        Right = i == (Count - 1) ? 0 : s.Right + Margin.Right, 
                        Bottom = i == (Count - 1) ? 0 : s.Bottom + Margin.Bottom, 
                    };
                }
                else
                {
                    Element.Margin = new Thickness()
                    {
                        Left = i == 0 ? 0 : s.Left, 
                        Top = i == 0 ? 0 : s.Top,
                        Right = i == (Count - 1) ? 0 : s.Right, 
                        Bottom = i == (Count - 1) ? 0 : s.Bottom, 
                    };
                }
                
                // Element.Margin = new Thickness();
                continue;
            }

            if (!Element.Margin.Equals(EmptyCompareObj))
            {
                var Margin = OriginMarginDict[Element.Uid];
                Element.Margin = new Thickness()
                {
                    Left =  s.Left + Margin.Left, 
                    Top =  s.Top + Margin.Top, 
                    Right = s.Right + Margin.Right, 
                    Bottom = s.Bottom + Margin.Bottom, 
                };
            }
            else
            {
                Element.Margin = s;
            }
        }
    }

    #endregion

    #region TrimFirst

    public static readonly DependencyProperty TrimFirstProperty =
        DependencyProperty.RegisterAttached("TrimFirst", typeof(bool), typeof(PanelExtensions),
            new FrameworkPropertyMetadata(true, OnSpacingChanged));

    public static void SetTrimFirst(Panel d, bool value)
    {
        d.SetValue(TrimFirstProperty, value);
    }

    public static bool GetTrimFirst(Panel d)
    {
        return (bool)d.GetValue(TrimFirstProperty);
    }

    #endregion

    #region TrimLast

    public static readonly DependencyProperty TrimLastProperty =
        DependencyProperty.RegisterAttached("TrimLast", typeof(bool), typeof(PanelExtensions),
            new FrameworkPropertyMetadata(true, OnSpacingChanged));

    public static void SetTrimLast(Panel d, bool value)
    {
        d.SetValue(TrimLastProperty, value);
    }

    public static bool GetTrimLast(Panel d)
    {
        return (bool)d.GetValue(TrimLastProperty);
    }

    #endregion

    #region VerticalContentAlignment

    public static readonly DependencyProperty VerticalContentAlignmentProperty =
        DependencyProperty.RegisterAttached("VerticalContentAlignment", typeof(VerticalAlignment),
            typeof(PanelExtensions), new FrameworkPropertyMetadata(VerticalAlignment.Top,
                OnVerticalContentAlignmentChanged));

    public static void SetVerticalContentAlignment(Panel d, VerticalAlignment value)
    {
        d.SetValue(VerticalContentAlignmentProperty, value);
    }

    public static VerticalAlignment GetVerticalContentAlignment(Panel d)
    {
        return (VerticalAlignment)d.GetValue(VerticalContentAlignmentProperty);
    }

    static void OnVerticalContentAlignmentChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        var Panel = sender as Panel;

        Panel.SizeChanged -= OnVerticalContentAlignmentUpdated;
        Panel.SizeChanged += OnVerticalContentAlignmentUpdated;

        OnVerticalContentAlignmentUpdated(Panel, null);
    }

    static void OnVerticalContentAlignmentUpdated(object sender, SizeChangedEventArgs e)
    {
        var p = sender as Panel;
        var a = GetVerticalContentAlignment(p);

        for (int i = 0, Count = p.Children.Count; i < Count; i++)
            (p.Children[i] as FrameworkElement).VerticalAlignment = a;
    }

    #endregion
}