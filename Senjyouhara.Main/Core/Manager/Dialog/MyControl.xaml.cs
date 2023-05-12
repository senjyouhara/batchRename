using Senjyouhara.Common.Base;
using Senjyouhara.Common.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    /// <summary>
    /// MyControl.xaml 的交互逻辑
    /// </summary>
    public partial class MyControl : UserControl, IDialogControl
    {
        public MyControl()
        {
            InitializeComponent();
            closeBtn.Command = new DelegateCommand(() =>
            {
                Close();
                //((IDialogAware)DataContext).RequestClose?.Invock(new DialogResult(ButtonResult.Abort));
            });
        }
        public IDialogResult Result { get; set; }

        public void Close()
        {
            ((Grid)(Application.Current.MainWindow)?.FindName("dialog"))?.Children.Remove(this);
        }

        public void Show()
        {
            ((Grid)(Application.Current.MainWindow)?.FindName("dialog"))?.Children.Add(this);
        }
    }
}
