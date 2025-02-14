using Caliburn.Micro;
using PropertyChanged;
using Suzumiya.Common.Base;
using Suzumiya.Common.Log;
using Suzumiya.Main.Config;
using System.Threading.Tasks;

namespace Suzumiya.Main.ViewModels
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
