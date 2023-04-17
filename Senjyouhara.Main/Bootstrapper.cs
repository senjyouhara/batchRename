using Caliburn.Micro;
using Senjyouhara.Main.ViewModels;
using System.Collections.Generic;
using System;
using System.Windows;

namespace Senjyouhara.Main
{
    public class Bootstrapper : BootstrapperBase
    {

        private SimpleContainer container;

        public Bootstrapper()
        {
            Initialize();
        }


        protected override void Configure()
        {
            container = new SimpleContainer();

            container.Singleton<IWindowManager, WindowManager>();
            
            container.PerRequest<ShellViewModel>();
            container.PerRequest<StartLoadingViewModel>();
            container.PerRequest<MainViewModel>();
            container.PerRequest<GenerateRuleViewModel>();
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
#if (DEBUG)
            await DisplayRootViewForAsync<ShellViewModel>();
#else
            await DisplayRootViewForAsync<StartLoadingViewModel>();
#endif

        }

        protected override object GetInstance(Type service, string key)
        {
            return container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            container.BuildUp(instance);
        }
    }
}
