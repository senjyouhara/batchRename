using Caliburn.Micro;
using PropertyChanged;
using System.Threading.Tasks;

namespace Senjyouhara.Main.ViewModels
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {

        private IEventAggregator _eventAggregator;
        public ShellViewModel()
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
