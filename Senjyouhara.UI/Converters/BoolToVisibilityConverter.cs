using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.UI.Converters
{
    internal class BoolToVisibilityConverter: IValueConverter
    {
        /// <summary>
        /// Converts <see cref="SolidColorBrush"/> to <see langword="Color"/>.
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is true ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Not Implemented.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
