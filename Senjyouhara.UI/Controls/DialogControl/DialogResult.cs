using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.UI.Controls.DialogControl
{
    public enum DialogResult
    {
        /// <summary>
        /// No button was tapped.
        /// </summary>
        None,
        /// <summary>
        /// The primary button was tapped by the user.
        /// </summary>
        Primary,
        /// <summary>
        /// The secondary button was tapped by the user.
        /// </summary>
        Secondary
    }
}
