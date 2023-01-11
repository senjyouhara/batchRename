using DryIoc;
using Prism.DryIoc;
using Prism.Ioc;
using Senjyouhara.Main.ViewModels;
using Senjyouhara.Main.Views;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Senjyouhara.Main
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {

            Debug.WriteLine("CreateShell");

            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
           
        }


        }
}
