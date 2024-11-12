using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Senjyouhara.UI.Extensions
{
    public class ListViewLayoutManager
    {
        public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached(
            "Enabled",
            typeof(bool),
            typeof(ListViewLayoutManager),
            new FrameworkPropertyMetadata(OnLayoutManagerEnabledChanged));

        private ListView listView;
        private bool loaded = false;
        private Cursor resizeCursor;
        private ScrollViewer scrollViewer;
        private ScrollBarVisibility verticalScrollBarVisibility = ScrollBarVisibility.Auto;


        public ListViewLayoutManager(ListView listView)
        {
            this.listView = listView;
            listView.Loaded += ListViewLoaded;
            listView.SizeChanged += ListViewSizeChanged;
        }


        public static void SetEnabled(DependencyObject dependencyObject, bool enabled)
        {
            dependencyObject.SetValue(EnabledProperty, enabled);
        }

        private void ListViewLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListView listView && !listView.IsLoaded) return;
            CalculateGridColumnWidths(sender);
            RegisterEvents(this.listView);
            ResizeColumns();
            loaded = true;
        }

        private void ListViewSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is ListView listView && !listView.IsLoaded) return;
            CalculateGridColumnWidths(sender);
        }

        private void RegisterEvents(DependencyObject start)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(start); i++)
            {
                Visual childVisual = VisualTreeHelper.GetChild(start, i) as Visual;
                if (childVisual is Thumb)
                {
                    GridViewColumn gridViewColumn = FindColumn(childVisual);
                    if (gridViewColumn == null)
                    {
                        continue;
                    }

                    Thumb thumb = childVisual as Thumb;
                    thumb.PreviewMouseMove += ThumbPreviewMouseMove;
                    // thumb.PreviewMouseLeftButtonDown += ThumbPreviewMouseLeftButtonDown;
                    DependencyPropertyDescriptor.FromProperty(
                        GridViewColumn.WidthProperty,
                        typeof(GridViewColumn)).AddValueChanged(gridViewColumn, GridColumnWidthChanged);
                }
                else if (scrollViewer == null && childVisual is ScrollViewer)
                {
                    scrollViewer = childVisual as ScrollViewer;
                    scrollViewer.ScrollChanged += ScrollViewerScrollChanged;
                    // assume we do the regulation of the horizontal scrollbar
                    scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    scrollViewer.VerticalScrollBarVisibility = verticalScrollBarVisibility;
                }

                RegisterEvents(childVisual);  
            }
        }


        private void GridColumnWidthChanged(object sender, EventArgs e)
        {
            if (loaded)
            {
                GridViewColumn gridViewColumn = sender as GridViewColumn;
                if (ListViewColumnWidthProperty.IsValidRangeColumn(gridViewColumn))
                {
                    if (ColumnWidthLimit(gridViewColumn) != 0)
                    {
                        return;
                    }
                }
                ResizeColumns();
            }
        }

        private void ResizeColumns()
        {
            GridView view = listView.View as GridView;
            if (view == null)
            {
                return;
            }

            double actualWidth = scrollViewer != null ? scrollViewer.ViewportWidth : listView.ActualWidth;
            if (actualWidth <= 0)
            {
                return;
            }

            double resizeableRegionCount = 0;
            double otherColumnsWidth = 0;
            
            // determine column sizes
            foreach (GridViewColumn gridViewColumn in view.Columns)
            {
                otherColumnsWidth += gridViewColumn.ActualWidth;
            }

            if (resizeableRegionCount <= 0)
            {
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                return;
            }

            double resizeableColumnsWidth = actualWidth - otherColumnsWidth;
            if (resizeableColumnsWidth <= 0)
            {
                return;
            }
        }

        private void CalculateGridColumnWidths(object sender)
        {
            if (sender is ListView listView && listView.View is GridView gridView)
            {
                if (listView.ActualWidth <= 0) return;

                // the extra offset may need to be altered per your application.
                var scrollOffset = SystemParameters.VerticalScrollBarWidth + 7;

                var remainingWidth = listView.ActualWidth - scrollOffset;
                var starTotal = 0.0;

                foreach (var column in gridView.Columns)
                {
                    var gridLength = ListViewColumnWidthProperty.GetWidth(column);
                    
                    if (gridLength.IsStar)
                    {
                        // Get the cumlative star value while passing over the columns
                        // but don't set their width until absolute and auto have been set.
                        starTotal += gridLength.Value;
                        continue;
                    }
                    
                    if (gridLength.IsAbsolute)
                    {
                        column.Width = gridLength.Value;
                    }
                    else
                    {
                        column.Width = double.NaN;
                    }

                    remainingWidth -= column.ActualWidth;
                }


                // now eval each star column
                foreach (var column in gridView.Columns)
                {
                    var gridLength = ListViewColumnWidthProperty.GetWidth(column);
                    if (starTotal == 0.0 || !gridLength.IsStar)
                    {
                        ColumnWidthLimit(column);
                        continue;
                    }


                    var starPercent = (gridLength.Value / starTotal);
                    column.Width = remainingWidth * starPercent;
                    ColumnWidthLimit(column);
                    // Debug.WriteLine($"column.Width :{column.Width}");
                }
            }
        }

        private double ColumnWidthLimit( GridViewColumn  column)
        {
            
            double startWidth = column.Width;

            double minWidth = ListViewColumnWidthProperty.GetMinWidth(column);
            double maxWidth = ListViewColumnWidthProperty.GetMaxWidth(column);
            
            if ((!double.IsNaN(minWidth) && !double.IsNaN(maxWidth)) && (minWidth > maxWidth))
            {
                return 0;
            }

            if (!double.IsNaN(minWidth) && column.Width < minWidth)
            {
                column.Width = minWidth;
            }
            else if (!double.IsNaN(maxWidth) && column.Width > maxWidth)
            {
                column.Width = maxWidth;
            }
         
            return column.Width - startWidth;
        }

        private GridViewColumn FindColumn(DependencyObject element)
        {
            if (element == null)
            {
                return null;
            }

            while (element != null)
            {
                if (element is GridViewColumnHeader)
                {
                    return ((GridViewColumnHeader)element).Column;
                }
                element = VisualTreeHelper.GetParent(element);
            }

            return null;
        }

        private void ScrollViewerScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (loaded && e.ViewportWidthChange != 0)
            {
                ResizeColumns();
            }
        }


        private void ThumbPreviewMouseMove(object sender, MouseEventArgs e)
        {
            Thumb thumb = sender as Thumb;
            GridViewColumn gridViewColumn = FindColumn(thumb);
            if (gridViewColumn == null)
            {
                return;
            }
        
        
            // check range column bounds
            if (thumb.IsMouseCaptured && ListViewColumnWidthProperty.IsValidRangeColumn(gridViewColumn))
            {
                double minWidth = ListViewColumnWidthProperty.GetMinWidth(gridViewColumn);
                double maxWidth = ListViewColumnWidthProperty.GetMaxWidth(gridViewColumn);
        
                if ((!double.IsNaN(minWidth) && !double.IsNaN(maxWidth)) && (minWidth > maxWidth))
                {
                    return; // invalid case
                }

                ColumnWidthLimit(gridViewColumn);
                
                if (resizeCursor == null)
                {
                    resizeCursor = thumb.Cursor; // save the resize cursor
                }
                
                if (!double.IsNaN(minWidth) && gridViewColumn.Width < minWidth)
                {
                    thumb.Cursor = Cursors.No;
                }
                else if (!double.IsNaN(maxWidth) && gridViewColumn.Width > maxWidth)
                {
                    thumb.Cursor = Cursors.No;
                }
                else
                {
                    thumb.Cursor = resizeCursor; // between valid min/max
                }
            }
        }


        private static void OnLayoutManagerEnabledChanged(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            ListView listView = dependencyObject as ListView;
            if (listView != null)
            {
                bool enabled = (bool)e.NewValue;
                if (enabled)
                {
                    new ListViewLayoutManager(listView);
                }
            }
        }
    }
}