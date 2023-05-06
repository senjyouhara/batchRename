using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    public interface IDialogAware
    {
        bool CanCloseDialog();

        IDialogParameters OnDialogClosed(ButtonResult buttonResult);

        void OnDialogOpened(IDialogParameters parameters);

        string Title { get; }

        event Action<IDialogResult> RequestClose;
    }
}
