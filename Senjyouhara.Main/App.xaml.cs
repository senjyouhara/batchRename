using Prism.Ioc;
using Prism.Unity;
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
            return Container.Resolve<MainWindow>();
        }

        protected override void OnInitialized()
        {

#if (!DEBUG)

#else
if (Container.Resolve<StartLoadingView>().ShowDialog() == false)
{
    Application.Current?.Shutdown();
    return;
}
else
#endif

            base.OnInitialized();
        }

        protected override void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
        }
    }
}
