using Senjyouhara.Common.Utils;
using System.ComponentModel;
using System.Windows.Input;

namespace Senjyouhara.Main.ViewModels
{
    public class MainWindowViewModel: NotifycationObject
    {


        private string name;

        public string Name
        {
            get { return  name; }
            set {  name = value; RaisePropertyChanged(nameof(Name)); }
        }

    }
}
