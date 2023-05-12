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
using Senjyouhara.Common.Base;

namespace Senjyouhara.UI.Controls
{

    public class TitleBar : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(TitleBar),
            new PropertyMetadata(null)
        );

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            nameof(Icon),
            typeof(ImageSource),
            typeof(TitleBar),
            new PropertyMetadata(null)
        );

        public static readonly DependencyProperty ExtendButtonsProperty =
            DependencyProperty.Register(
                nameof(ExtendButtons),
                typeof(object),
                typeof(TitleBar),
                new PropertyMetadata(null)
            );

        public static readonly DependencyProperty OverrideTitleProperty =
            DependencyProperty.Register(
                nameof(OverrideTitle),
                typeof(object),
                typeof(TitleBar),
                new PropertyMetadata(null)
            );

        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register(
                nameof(ButtonCommand),
                typeof(ICommand),
                typeof(TitleBar),
                new PropertyMetadata(null)
            );

        public static readonly DependencyProperty IsMaximizedProperty = DependencyProperty.Register(
            nameof(IsMaximized),
            typeof(bool),
            typeof(TitleBar),
            new PropertyMetadata(false)
        );


        public static readonly DependencyProperty IsCanMoveProperty = DependencyProperty.Register(
            nameof(IsCanMove),
            typeof(bool),
            typeof(TitleBar),
            new PropertyMetadata(true)
        );

        public static readonly DependencyProperty IsOverrideTitleProperty =
            DependencyProperty.Register(
                nameof(IsOverrideTitle),
                typeof(bool),
                typeof(TitleBar),
                new PropertyMetadata(false)
            );



        public bool ShowMinBtn
        {
            get { return (bool)GetValue(ShowMinBtnProperty); }
            set { SetValue(ShowMinBtnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowMinBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowMinBtnProperty =
            DependencyProperty.Register(nameof(ShowMinBtn), typeof(bool), typeof(TitleBar), new PropertyMetadata(true));



        public bool ShowMaxBtn
        {
            get { return (bool)GetValue(ShowMaxBtnProperty); }
            set { SetValue(ShowMaxBtnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowMaxBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowMaxBtnProperty =
            DependencyProperty.Register(nameof(ShowMaxBtn), typeof(bool), typeof(TitleBar), new PropertyMetadata(true));




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



        public ICommand ButtonCommand
        {
            get => (ICommand)GetValue(ButtonCommandProperty);
        }
        public bool IsCanMove
        {
            get => (bool)GetValue(IsCanMoveProperty);
            set => SetValue(IsCanMoveProperty, value);
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
                RootGrid.MouseMove += (s, e) =>
                {

                    if (IsCanMove)
                    {
                        if (e.LeftButton == MouseButtonState.Pressed)
                        {
                            ParentWindow.DragMove();
                        }
                    }
                 
                };

                RootGrid.MouseDown += (s, e) =>
                {
                    if (e.ClickCount == 2)
                    {
                        if (ShowMaxBtn)
                        {
                            MaximizeWindow();
                        }
                    }
                };
            }

            if (ParentWindow != null)
            {
                ParentWindow.MaxHeight = SystemParameters.WorkArea.Height;
            }

            Console.WriteLine(ShowMinBtn);
            Console.WriteLine(ShowMaxBtn);
        }

        public TitleBar()
        {           
            SetValue(ButtonCommandProperty, new DelegateCommand<string>(TemplateButton_OnClick));
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

        private void TemplateButton_OnClick(string parameter)
        {
            string command = parameter;

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
