using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suzumiya.Main.Core.Manager.Dialog
{
    public interface IDialogAware
    {
        bool CanCloseDialog();

        IDialogParameters OnDialogClosing(ButtonResult buttonResult);

        void OnDialogClosed();

        void OnDialogOpened(IDialogParameters parameters);

        string Title { get; }

        event Action<IDialogResult> RequestClose;
    }
}
