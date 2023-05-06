using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    public interface IDialogResult
    {
        /// <summary>
        /// The parameters from the dialog.
        /// </summary>
        IDialogParameters Parameters { get; set; }

        /// <summary>
        /// The result of the dialog.
        /// </summary>
        ButtonResult Result { get; }
    }
}
