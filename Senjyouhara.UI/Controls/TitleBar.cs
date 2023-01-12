using Senjyouhara.UI.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Media;

namespace Senjyouhara.UI.Controls
{

        public class TitleBar : UserControl
    {

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title),
      typeof(string), typeof(TitleBar), new PropertyMetadata(null));

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon),
   typeof(ImageSource), typeof(TitleBar), new PropertyMetadata(null));

        public static readonly DependencyProperty ExtendButtonsProperty = DependencyProperty.Register(nameof(ExtendButtons),
typeof(object), typeof(TitleBar), new PropertyMetadata(null));

        public static readonly DependencyProperty OverrideTitleProperty = DependencyProperty.Register(nameof(OverrideTitle),
typeof(object), typeof(TitleBar), new PropertyMetadata(null));

        public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.Register(nameof(ButtonCommand),
typeof(ICommand), typeof(TitleBar), new PropertyMetadata(null));


        public static readonly DependencyProperty IsMaximizedProperty = DependencyProperty.Register(nameof(IsMaximized),
    typeof(bool), typeof(TitleBar), new PropertyMetadata(false));

        public static new readonly DependencyProperty HeightProperty = DependencyProperty.Register(nameof(Height),
typeof(int), typeof(TitleBar), new PropertyMetadata(null));

        public static readonly DependencyProperty IsOverrideTitleProperty = DependencyProperty.Register(nameof(IsOverrideTitle),
typeof(bool), typeof(TitleBar), new PropertyMetadata(false));


        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public object ExtendButtons
        {
            get => GetValue(ExtendButtonsProperty);
            set => SetValue(ExtendButtonsProperty, value);
        }
        public object OverrideTitle
        {
            get => GetValue(OverrideTitleProperty);
            set => SetValue(OverrideTitleProperty, value);
        }

        public bool IsOverrideTitle
        {
            get => (bool)GetValue(IsOverrideTitleProperty);
            set => SetValue(IsOverrideTitleProperty, value);
        }

        public new int Height
        {
            get => (int)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }


        public int ButtonHeight
        {
            get
            {
                var value = (int)GetValue(HeightProperty);
                return value;
            }
        }

        public ICommand ButtonCommand
        {
            get => (ICommand)GetValue(ButtonCommandProperty);
        }

        public bool IsMaximized
        {
            get => (bool)GetValue(IsMaximizedProperty);
            internal set => SetValue(IsMaximizedProperty, value);
        }

        private Window _parent;
        private Window ParentWindow => _parent = Window.GetWindow(this);

        private void TitleBar_loaded(object sender, RoutedEventArgs ev)
        {
            var minBtn = (Button)Template.FindName("ButtonMinimize", this);
            var maxBtn = (Button)Template.FindName("ButtonMaximize", this);
            var closeBtn = (Button)Template.FindName("ButtonClose", this);
            IsOverrideTitle = OverrideTitle != null;
            var RootGrid = (Grid)Template.FindName("RootGrid", this);

            if (RootGrid != null)
            {
                //RootGrid.MouseMove += (s, e) =>
                //{
                //    if (e.LeftButton == MouseButtonState.Pressed)
                //    {
                //        ParentWindow.DragMove();
                //    }
                //};

                //RootGrid.MouseDown += (s, e) =>
                //{
                //    if (e.ClickCount == 2)
                //    {
                //        if (ParentWindow.WindowState == WindowState.Normal)
                //            ParentWindow.WindowState = WindowState.Maximized;
                //        else
                //            ParentWindow.WindowState = WindowState.Normal;
                //    }
                //};
            }
        }

        public TitleBar()
        {
            SetValue(ButtonCommandProperty, new RelayCommand(o => TemplateButton_OnClick(this, o)));
            Loaded += TitleBar_loaded;
        }

        private void CloseWindow()
        {
            IsMaximized = false;
            ParentWindow.Close();
        }

        private void MaximizeWindow()
        {
            if (ParentWindow.WindowState == WindowState.Maximized)
            {
                IsMaximized = false;
                ParentWindow.WindowState = WindowState.Normal;
            }
            else
            {
                IsMaximized = true;
                ParentWindow.WindowState = WindowState.Maximized;
            }
        }

        private void MinimizeWindow()
        {
            IsMaximized = false;
            ParentWindow.WindowState = WindowState.Minimized;
        }

        private void TemplateButton_OnClick(TitleBar titleBar, object parameter)
        {
            string command = parameter as string;

            Console.WriteLine("params" + parameter);

            switch (command)
            {
                case "close":
                    CloseWindow();
                    break;

                case "minimize":
                    MinimizeWindow();
                    break;

                case "maximize":
                    MaximizeWindow();
                    break;
            }

        }
    }
}
