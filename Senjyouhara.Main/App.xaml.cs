using Senjyouhara.Main.ViewModels;
using Senjyouhara.Main.Views;
using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Senjyouhara.Main
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {



        public App()
        {
            InitializeComponent();
        }


        private void OpenMainWindow()
        {
            var mwin = new MainWindow();
            mwin.Show();

        }

        protected override void OnStartup(StartupEventArgs e)
        {
#if (DEBUG)
            base.OnStartup(e);
            OpenMainWindow();
#else

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            StartLoadingView window = new StartLoadingView();
            bool? dialogResult = window.ShowDialog();
            if (dialogResult == true)
            {
                base.OnStartup(e);
                OpenMainWindow();
                Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            }
            else
            {
                Shutdown();
            }

#endif

        }
    }
}
