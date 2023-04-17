using Caliburn.Micro;
using PropertyChanged;
using System.Threading.Tasks;

namespace Senjyouhara.Main.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>
    {

        private IEventAggregator _eventAggregator;
        public ShellViewModel(IWindowManager windowManager)
        {
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
