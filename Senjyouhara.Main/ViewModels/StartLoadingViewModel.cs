using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Animation;
using System.Threading;

namespace Senjyouhara.Main.ViewModels
{
    public class StartLoadingViewModel : Conductor<IScreen>
    {
        private IWindowManager _WindowManager;
        public StartLoadingViewModel(IWindowManager manager)
        {
            _WindowManager = manager;
        }
        
        public async void CloseHandle()
        {
            var model = IoC.Get<ShellViewModel>();
            await _WindowManager.ShowWindowAsync(model);
            await CloseForm();
        }

        public Task CloseForm()
        {
            return TryCloseAsync();
        }
    }
}
