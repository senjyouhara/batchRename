using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Senjyouhara.UI.Common
{
    public interface IRelayCommand : ICommand
    {
        new bool CanExecute(object parameter);

        new void Execute(object parameter);
    }
}
