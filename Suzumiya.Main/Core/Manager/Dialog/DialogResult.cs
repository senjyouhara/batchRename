using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suzumiya.Main.Core.Manager.Dialog
{
    public class DialogResult : IDialogResult
    {
        /// <summary>
        /// The parameters from the dialog.
        /// </summary>
        public IDialogParameters Parameters { get; set; } = new DialogParameters();

        /// <summary>
        /// The result of the dialog.
        /// </summary>
        public ButtonResult Result { get; private set; } = ButtonResult.None;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogResult"/> class.
        /// </summary>
        public DialogResult() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogResult"/> class.
        /// </summary>
        /// <param name="result">The result of the dialog.</param>
        public DialogResult(ButtonResult result)
        {
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogResult"/> class.
        /// </summary>
        /// <param name="result">The result of the dialog.</param>
        /// <param name="parameters">The parameters from the dialog.</param>
        public DialogResult(ButtonResult result, IDialogParameters parameters)
        {
            Result = result;
            Parameters = parameters;
        }
    }
}
