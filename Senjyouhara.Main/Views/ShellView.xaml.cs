using HandyControl.Controls;
using System;
using Senjyouhara.Main.ViewModels;

namespace Senjyouhara.Main.Views
{
    /// <summary>
    /// ShellView.xaml 的交互逻辑
    /// </summary>
    public partial class ShellView
    {
        public ShellView()
        {
            InitializeComponent();
            this.DataContext = new ShellViewModel(null);
        }
    }
}
