using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Senjyouhara.UI.Common
{
    public interface IMessageBoxAware
    {
        event Action<MessageBoxResult>  RequestClose;
    }
}
