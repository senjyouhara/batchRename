using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.UI.Controls.DialogControl
{
    public enum ControlAppearance
    {
        /// <summary>
        /// Control color according to the current theme accent.
        /// </summary>
        Primary,

        /// <summary>
        /// Control color according to the current theme element.
        /// </summary>
        Secondary,

        /// <summary>
        /// Blue color theme.
        /// </summary>
        Info,

        /// <summary>
        /// Dark color theme.
        /// </summary>
        Dark,

        /// <summary>
        /// Light color theme.
        /// </summary>
        Light,

        /// <summary>
        /// Red color theme.
        /// </summary>
        Danger,

        /// <summary>
        /// Green color theme.
        /// </summary>
        Success,

        /// <summary>
        /// Orange color theme.
        /// </summary>
        Caution,

        /// <summary>
        /// Transparent color theme.
        /// </summary>
        Transparent
    }
}
