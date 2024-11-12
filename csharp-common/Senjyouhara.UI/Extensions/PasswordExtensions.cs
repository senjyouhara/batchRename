using System.Windows;
using System.Windows.Controls;

namespace Senjyouhara.UI.Extensions
{
    public class PasswordExtensions
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",
                typeof(string),
                typeof(PasswordExtensions),
                new PropertyMetadata(new PropertyChangedCallback(OnPropertyChanged))
            );

        public static string GetPassword(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(PasswordProperty, value);
        }

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",
                typeof(string),
                typeof(PasswordExtensions),
                new PropertyMetadata(new PropertyChangedCallback(OnAttachChanged))
            );

        public static string GetAttach(DependencyObject dependencyObject)
        {
            return (string)dependencyObject.GetValue(AttachProperty);
        }

        public static void SetAttach(DependencyObject dependencyObject, string value)
        {
            dependencyObject.SetValue(AttachProperty, value);
        }

        static bool _isUpdating = false;

        private static void OnPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            PasswordBox pb = (d as PasswordBox);
            pb.PasswordChanged -= Pb_PasswordChanged;
            if (!_isUpdating)
            {
                (d as PasswordBox).Password = e.NewValue.ToString();
            }
            pb.PasswordChanged += Pb_PasswordChanged;
        }

        private static void OnAttachChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e
        )
        {
            PasswordBox pb = (d as PasswordBox);
            pb.PasswordChanged += Pb_PasswordChanged;
        }

        private static void Pb_PasswordChanged(object sender, RoutedEventArgs s)
        {
            PasswordBox pb = (sender as PasswordBox);
            _isUpdating = true;
            SetPassword(pb, pb.Password);
            _isUpdating = false;
        }
    }
}
