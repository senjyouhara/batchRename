using Senjyouhara.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Senjyouhara.UI.Styles
{
        /// <summary>
        /// MessageBoxWindow.xaml 的交互逻辑
        /// </summary>
        public partial class MessageBoxWindow : Window
        {
            public enum MessageBoxType
            {
                Info,
                Success,
                Warning,
                Error,
                Ask
            }

            public enum ButtonType
            {
                OK = 0,
                OKCancel = 1,
                YesNoCancel = 3,
                YesNo = 4,
                None = 5,
                Custom
            }

            private ButtonType currentButtonStyle;

            private Action<MessageBoxResult> resultAction;

            public MessageBoxWindow()
            {
                InitializeComponent();
                foreach (Window item in Application.Current.Windows)
                {
                    if (item.IsActive)
                    {
                        Owner = item;
                        break;
                    }
                }
                CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, CloseEvent));
                Loaded += MessageBoxWindow_Loaded;
        }

        protected bool m_IsDraging = false;
        protected Point m_DragStartPoint;
        private void MessageBoxWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var header = (Grid)Template.FindName("header", this);
            if (header != null)
            {
                header.MouseMove += (s, e) =>
                {
                    if (e.LeftButton == MouseButtonState.Pressed)
                    {
                        Window.GetWindow(this).DragMove();
                    }
                };
            }
        }

        private void CloseEvent(object sender, ExecutedRoutedEventArgs e)
            {
                this.Close();
                resultAction?.Invoke(MessageBoxResult.None);
            }

            private void Confirm_Click(object sender, RoutedEventArgs e)
            {
                this.Close();
                if (currentButtonStyle == ButtonType.OKCancel || currentButtonStyle == ButtonType.Custom)
                {
                    resultAction?.Invoke(MessageBoxResult.OK);
                }
                else if (currentButtonStyle == ButtonType.YesNoCancel || currentButtonStyle == ButtonType.YesNo)
                {
                    resultAction?.Invoke(MessageBoxResult.Yes);
                };
            }

            private void Cannel_Click(object sender, RoutedEventArgs e)
            {
                this.Close();
                if (currentButtonStyle == ButtonType.OKCancel || currentButtonStyle == ButtonType.Custom)
                {
                    resultAction?.Invoke(MessageBoxResult.Cancel);
                }
                else if (currentButtonStyle == ButtonType.YesNoCancel || currentButtonStyle == ButtonType.YesNo)
                {
                    resultAction?.Invoke(MessageBoxResult.No);
                };
            }

            public void Show(object messageBoxContent, string caption, MessageBoxType type = MessageBoxType.Info, ButtonType buttonType = ButtonType.OKCancel, Action<MessageBoxResult> callback = null, string confirmName = "", string cannelName = "")
            {

                var content = messageBoxContent as UIElement;
                
                if(messageBoxContent.GetType() == typeof(string))
                {
                    var block = new TextBlock();
                    block.FontSize = 16;
                    block.TextWrapping = TextWrapping.Wrap;
                    block.VerticalAlignment = VerticalAlignment.Center;
                    block.HorizontalAlignment = HorizontalAlignment.Center;
                    block.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                    block.Text = messageBoxContent.ToString();
                    content = block;
                }

                if(content is IMessageBoxAware mba)
                {
                    mba.RequestClose += (r) =>
                    {
                        this.Close();
                        resultAction?.Invoke(r);
                    };
                }

                Content.Children.Add(content);
                this.Title = caption;
                this.currentButtonStyle = buttonType;
                switch (buttonType)
                {
                    case ButtonType.OK:
                        Cannel.Visibility = Visibility.Collapsed;
                        break;
                    case ButtonType.OKCancel:
                        Cannel.Visibility = Visibility.Visible;
                        Confirm.Visibility = Visibility.Visible;
                        Confirm.Content = "确定";
                        Cannel.Content = "取消";
                        break;
                    case ButtonType.YesNoCancel:
                    case ButtonType.YesNo:
                        Cannel.Visibility = Visibility.Visible;
                        Confirm.Visibility = Visibility.Visible;
                        Confirm.Content = "是";
                        Cannel.Content = "否";
                        break;
                    case ButtonType.None:
                        Cannel.Visibility = Visibility.Collapsed;
                        Confirm.Visibility = Visibility.Collapsed;
                        break;
                    case ButtonType.Custom:
                        Cannel.Visibility = Visibility.Visible;
                        Confirm.Visibility = Visibility.Visible;
                        Cannel.Content = cannelName;
                        Confirm.Content = confirmName;
                        break;
                    default:
                        break;
                }
                //SetIcon(type);
                this.resultAction = callback;
                this.ShowDialog();
            }

        private void Mba_RequestClose(MessageBoxResult obj)
        {
            throw new NotImplementedException();
        }

        private void SetIcon(MessageBoxType type)
            {
            //string iconText = "";
            //switch (type)
            //{
            //    case MessageBoxType.Info:
            //        icon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#00bcd4"));
            //        iconText = "&#xe77f;";
            //        break;
            //    case MessageBoxType.Success:
            //        icon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2db84d"));
            //        iconText = "&#xe77d;";
            //        break;
            //    case MessageBoxType.Warning:
            //        icon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e9af20"));
            //        iconText = "&#xe785;";
            //        break;
            //    case MessageBoxType.Error:
            //        icon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#db3340"));
            //        iconText = "&#xe77e;";
            //        break;
            //    case MessageBoxType.Ask:
            //        icon.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8a2be2"));
            //        iconText = "&#xe60c;";
            //        break;
            //    default:
            //        break;
            //}
            //icon.Text = iconText;
        }
    }
}
