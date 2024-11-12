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
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Senjyouhara.Common.Base;

namespace Senjyouhara.UI.Controls
{

    [TemplatePart(Name = "PART_ButtonClose", Type = typeof(ButtonBase))]
    public class TitleBar : UserControl
    {

        private ButtonBase ButtonCloseHost;
        
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
        
        public static readonly DependencyProperty CloseCommandProperty =
            DependencyProperty.Register(
                nameof(CloseCommand),
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
        
        // [Category("Behavior")]
        // public static readonly RoutedEvent OnCloseEvent =
        //     EventManager.RegisterRoutedEvent(
        //         "OnClose",
        //         RoutingStrategy.Bubble,
        //         typeof(RoutedEventHandler),
        //         typeof(TitleBar));
        
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

        // Using a DependencyProperty as the backing store for ShowMinBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowMinBtnProperty =
            DependencyProperty.Register(nameof(ShowMinBtn), typeof(bool), typeof(TitleBar), new PropertyMetadata(true));

        // Using a DependencyProperty as the backing store for ShowMaxBtn.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowMaxBtnProperty =
            DependencyProperty.Register(nameof(ShowMaxBtn), typeof(bool), typeof(TitleBar), new PropertyMetadata(true));

        private Window _parent;

        public TitleBar()
        {
            var command = GetValue(ButtonCommandProperty);
            if (command == null) SetValue(ButtonCommandProperty, new DelegateCommand<string>(TemplateButton_OnClick));
            Loaded += TitleBar_loaded;
        }


        public bool ShowMinBtn
        {
            get { return (bool)GetValue(ShowMinBtnProperty); }
            set { SetValue(ShowMinBtnProperty, value); }
        }


        public bool ShowMaxBtn
        {
            get { return (bool)GetValue(ShowMaxBtnProperty); }
            set { SetValue(ShowMaxBtnProperty, value); }
        }


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

        public ICommand CloseCommand
        {
            get => (ICommand)GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
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
        }
        
        
        private void HandleOnCloseClick(object sender, RoutedEventArgs e)
        {

            if (CloseCommand != null)
            {
                CloseCommand.Execute(null);
            }
            else
            {
                ParentWindow.Close();
            }
            // RaiseEvent(new RoutedEventArgs(OnCloseEvent, this));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            
            if (ButtonCloseHost != null)
            {
                ButtonCloseHost.Click -= HandleOnCloseClick;
            }
            
            ButtonCloseHost = GetTemplateChild("PART_ButtonClose") as ButtonBase;
            if (ButtonCloseHost != null)
            {
                ButtonCloseHost.Click += HandleOnCloseClick;
            }
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

            switch (command)
            {
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
