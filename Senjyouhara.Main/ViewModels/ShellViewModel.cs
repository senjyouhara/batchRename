using Caliburn.Micro;
using PropertyChanged;
using Senjyouhara.Common.Base;
using Senjyouhara.Common.Log;
using Senjyouhara.Main.Config;
using System.Threading.Tasks;

namespace Senjyouhara.Main.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ShellViewModel : Conductor<IScreen>
    {

        public string Title { get; set; }

        private IEventAggregator _eventAggregator;
        public ShellViewModel(IWindowManager windowManager)
        {
            Title = AppConfig.Title + " - v" + AppConfig.Version;
            _eventAggregator = new EventAggregator();
            _eventAggregator.SubscribeOnUIThread(this);
            Task.Run(async () =>
            {
                var model = IoC.Get<MainViewModel>();
                await ActivateItemAsync(model);
            });
        }

    }
}
