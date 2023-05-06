using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    public interface IDialogControl
    {
        public void Close();

        public void Show();
    }
}
