using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using HandyControl.Interactivity;

namespace Senjyouhara.UI.Controls;

/// <summary>
///     图片浏览器
/// </summary>
[TemplatePart(Name = ElementPanelTop, Type = typeof(Panel))]
[TemplatePart(Name = ElementImageViewer, Type = typeof(ImageViewer))]
public class ImageBrowser : HandyControl.Controls.Window
{
    #region Constants

    private const string ElementPanelTop = "PART_PanelTop";

    private const string ElementImageViewer = "PART_ImageViewer";

    #endregion Constants

    #region Data

    private Panel _panelTop;

    private ImageViewer _imageViewer;

    #endregion Data

    static ImageBrowser()
    {
        IsFullScreenProperty.AddOwner(typeof(ImageBrowser), new PropertyMetadata(false));
    }

    public ImageBrowser()
    {
        CommandBindings.Add(new CommandBinding(ControlCommands.Close, ButtonClose_OnClick));

        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        WindowStyle = WindowStyle.None;
        Topmost = true;
        AllowsTransparency = true;
    }

    /// <summary>
    ///     带一个图片Uri的构造函数
    /// </summary>
    /// <param name="uri"></param>
    public ImageBrowser(Uri uri) : this(uri, new List<Uri>())
    {
    }


    public ImageBrowser(Uri uri, List<Uri> list) : this()
    {
        Loaded += (s, e) =>
        {
            try
            {
                // _imageViewer = new(uri, list);
                // _imageViewer.ImageSource = BitmapFrame.Create(uri);
                // _imageViewer.ImgPath = uri.AbsolutePath;
                _imageViewer.IsShowPrevNext = true;
                _imageViewer.ImageSourceList = list;
                _imageViewer.Uri = uri;
                _imageViewer.ImgPath = uri.AbsolutePath;

                if (File.Exists(_imageViewer.ImgPath))
                {
                    var info = new FileInfo(_imageViewer.ImgPath);
                    _imageViewer.ImgSize = info.Length;
                }
            }
            catch
            {
                var result = MessageBox.Show("图片路径出错！");
                if (result == MessageBoxResult.OK) Close();
            }
        };
    }

    /// <summary>
    ///     带一个图片路径的构造函数
    /// </summary>
    /// <param name="path"></param>
    public ImageBrowser(string path) : this(new Uri(path), new List<Uri>())
    {
    }

    public ImageBrowser(string path, List<Uri> list) : this(new Uri(path), list)
    {
    }

    public override void OnApplyTemplate()
    {
        if (_panelTop != null) _panelTop.MouseLeftButtonDown -= PanelTopOnMouseLeftButtonDown;

        if (_imageViewer != null) _imageViewer.MouseLeftButtonDown -= ImageViewer_MouseLeftButtonDown;

        base.OnApplyTemplate();

        _panelTop = GetTemplateChild(ElementPanelTop) as Panel;
        _imageViewer = GetTemplateChild(ElementImageViewer) as ImageViewer;

        if (_panelTop != null) _panelTop.MouseLeftButtonDown += PanelTopOnMouseLeftButtonDown;

        if (_imageViewer != null) _imageViewer.MouseLeftButtonDown += ImageViewer_MouseLeftButtonDown;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        _imageViewer?.Dispose();
        base.OnClosing(e);
    }

    private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void PanelTopOnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed) DragMove();
    }

    private void ImageViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed &&
            !(_imageViewer.ImageWidth > ActualWidth || _imageViewer.ImageHeight > ActualHeight)) DragMove();
    }
}