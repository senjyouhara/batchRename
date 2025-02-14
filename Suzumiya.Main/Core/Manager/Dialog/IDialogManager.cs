using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suzumiya.Main.Core.Manager.Dialog
{
    public interface IDialogManager:IWindowManager
    {

        void ShowMyDialogAsync(object rootModel);
        void ShowMyDialogAsync(object rootModel, IDialogParameters parameters = null);
        void ShowMyDialogAsync(object rootModel, IDialogParameters parameters, Action<IDialogResult> callback = null);
    }
}
