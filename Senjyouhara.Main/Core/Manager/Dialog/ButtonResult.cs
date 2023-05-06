﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    public enum ButtonResult
    {
        /// <summary>
        /// Abort.
        /// </summary>
        Abort = 3,
        /// <summary>
        /// Cancel.
        /// </summary>
        Cancel = 2,
        /// <summary>
        /// Ignore.
        /// </summary>
        Ignore = 5,
        /// <summary>
        /// No.
        /// </summary>
        No = 7,
        /// <summary>
        /// No result returned.
        /// </summary>
        None = 0,
        /// <summary>
        /// OK.
        /// </summary>
        OK = 1,
        /// <summary>
        /// Retry.
        /// </summary>
        Retry = 4,
        /// <summary>
        /// Yes.
        /// </summary>
        Yes = 6
    }
}
