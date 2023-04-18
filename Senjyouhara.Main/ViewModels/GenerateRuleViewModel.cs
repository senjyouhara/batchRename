using Caliburn.Micro;
using PropertyChanged;
using Senjyouhara.Main.models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Senjyouhara.Main.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AppendNumber
    {
        public string SerialNumber { get; set; }
        public string DecimalNumber { get; set; }
        public bool IsCanDelete { get; set; } = true;
    }

    [AddINotifyPropertyChangedInterface]
    public class GenerateRuleViewModel: Screen
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(Screen));

        public string FirstNumber { get; set; }
        public string Step { get; set; }
        public string OriginName { get; set; }
        public string Replace { get; set; }
        public string PreviewReplace { get; set; }
        public ObservableCollection<AppendNumber> AppendNumberList { get; set; } = new();

        public GenerateRuleViewModel()
        {
            AppendNumberList.Add(new AppendNumber { DecimalNumber = "", SerialNumber = "", IsCanDelete = false });
        }

        public void AddAppendNumberItem(AppendNumber appendNumber)
        {
            var index = AppendNumberList.IndexOf(appendNumber);
            if(index >= 0)
            {
                AppendNumberList.Insert(index+1, new AppendNumber { DecimalNumber = "", SerialNumber = "", IsCanDelete = false });

            }
            if(AppendNumberList.Count > 1) {
                foreach (var item in AppendNumberList)
                {
                    item.IsCanDelete = true;
                }
            } else
            {
                foreach (var item in AppendNumberList)
                {
                    item.IsCanDelete = false;
                }
            }
        }

        public void RemoveAppendNumberItem(AppendNumber appendNumber)
        {
            if (AppendNumberList.Count <= 1)
            {
                AppendNumberList.Clear();
                AppendNumberList.Add(new AppendNumber { DecimalNumber = "", SerialNumber = "", IsCanDelete = false });
            } else
            {
                AppendNumberList.Remove(appendNumber);
            }

            if (AppendNumberList.Count > 1)
            {
                foreach (var item in AppendNumberList)
                {
                    item.IsCanDelete = true;
                }
            }
            else
            {
                foreach (var item in AppendNumberList)
                {
                    item.IsCanDelete = false;
                }
            }
        }
    }
}
