using Caliburn.Micro;
using PropertyChanged;
using Senjyouhara.Common.Base;
using Senjyouhara.Common.Log;
using Senjyouhara.Main.Core.Manager.Dialog;
using Senjyouhara.Main.models;
using Senjyouhara.Main.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Senjyouhara.Main.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class AppendNumber: IDataErrorInfo
    {
        [StringLength(10, ErrorMessage = "最多输入10位数")]
        [RegularExpression(@"^([1-9][0-9]*)$", ErrorMessage = "只能为整数")]
        public string SerialNumber { get; set; } = string.Empty;
        [StringLength(1, ErrorMessage = "最多输入1位数")]
        [RegularExpression(@"^[1-9]$", ErrorMessage = "只能为一位数整数")]
        public string DecimalNumber { get; set; } = string.Empty;

        public bool IsValid()
        {
            return Validator.TryValidateObject(
                this, new ValidationContext(this, null, null), new List<ValidationResult>(), true);
        }


        public string Error
        {
            get
            {
                Type FormDataType = GetType();
                PropertyInfo[] properties = FormDataType.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name.Equals(nameof(Error)) ||
                        properties[i].Name.Equals("Item")
                        )
                    {
                    }
                    else
                    {
                        var Name = properties[i].Name;
                        var vc = new ValidationContext(this, null, null);
                        vc.MemberName = Name;
                        var res = new List<ValidationResult>();
                        var result = Validator.TryValidateProperty(FormDataType.GetProperty(Name).GetValue(this, null), vc, res);
                        if (res.Count > 0)
                        {
                            var arr = res.Select(r => r.ErrorMessage).ToArray();
                            if (arr.Length > 0)
                            {
                                return arr[0];
                            }
                        }
                    }
                }
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                var vc = new ValidationContext(this, null, null);

                vc.MemberName = columnName;
                var res = new List<ValidationResult>();
                var result = Validator.TryValidateProperty(this.GetType().GetProperty(columnName).GetValue(this, null), vc, res);
                if (res.Count > 0)
                {
                    var arr = res.Select(r => r.ErrorMessage).ToArray();
                    if (arr.Length > 0)
                    {
                        return string.Join(Environment.NewLine, arr[0]);
                    }
                }
                return string.Empty;
            }
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class FormData : IDataErrorInfo
    {
        [StringLength(10, ErrorMessage = "最多输入10位数")]
        [RegularExpression(@"^([0-9][1-9]*)$", ErrorMessage = "只能为整数")]
        public string FirstNumber { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "最多输入10位数")]
        [RegularExpression(@"^([0-9]+\.[0-9]{1})|([0-9][1-9]*)$", ErrorMessage = "只能为整数或带一位小数")]
        public string Step { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "最多输入10位数")]
        [RegularExpression(@"^([0-9][1-9]*)$", ErrorMessage = "只能为整数")]
        public string DigitsNumber { get; set; } = string.Empty;

        public ObservableCollection<AppendNumber> AppendNumberList { get; set; } = new();

        public bool IsValid()
        {
            var list = AppendNumberList.Where(v => v.IsValid() == false).ToList();

            var result = Validator.TryValidateObject(
                this, new ValidationContext(this, null, null), new List<ValidationResult>(), true);

            if(result== false)
            {
                return result;
            }

            if (list.Count > 0)
            {
                return false;
            }

            return true;
        }


        public string Error {
            get 
            {
                Type FormDataType = GetType();
                PropertyInfo[] properties = FormDataType.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].Name.Equals(nameof(Error)) ||
                        properties[i].Name.Equals("Item")
                        )
                    {
                    } else if (properties[i].Name.Equals(nameof(AppendNumberList))) {
                        var list = this.AppendNumberList.Where(v => v.IsValid()).Select(v=> v.Error).ToList();
                        if(list.Count > 0)
                        {
                            return list[0];
                        }
                    } else
                    {
                        var Name = properties[i].Name;
                        Debug.WriteLine(Name);
                        var vc = new ValidationContext(this, null, null);
                        vc.MemberName = Name;
                        var res = new List<ValidationResult>();
                        var result = Validator.TryValidateProperty(FormDataType.GetProperty(Name).GetValue(this, null), vc, res);
                        if (res.Count > 0)
                        {
                            var arr = res.Select(r => r.ErrorMessage).ToArray();
                            Debug.WriteLine(arr.ToString());
                            if (arr.Length > 0)
                            {
                                return arr[0];
                            }
                        }
                    }
                }
                return string.Empty;
            }
        }

        public string this[string columnName]
        {
            get
            {
                var vc = new ValidationContext(this, null, null);
                
                vc.MemberName = columnName;
                var res = new List<ValidationResult>();
                var result = Validator.TryValidateProperty(this.GetType().GetProperty(columnName).GetValue(this, null), vc, res);
                if (res.Count > 0)
                {
                    var arr = (from r in res select r.ErrorMessage).ToArray();
                    if(arr.Length > 0)
                    {
                        return string.Join(Environment.NewLine, arr[0]);
                    }
                }
                return string.Empty;
            }
        }

    }

    [AddINotifyPropertyChangedInterface]
    public class GenerateRuleViewModel : Screen,IDialogAware
    {
        public DelegateCommand CloseCommand { get {
                return new DelegateCommand(() =>
                {
                    RequestClose(new DialogResult(ButtonResult.Abort));
                });
            } }
        public DelegateCommand<AppendNumber> AddAppendNumberItemCommand
        {
            get
            {
                return new DelegateCommand<AppendNumber>(AddAppendNumberItem);
            }
        }

        public DelegateCommand<AppendNumber> RemoveAppendNumberItemCommand
        {
            get
            {
                return new DelegateCommand<AppendNumber>(RemoveAppendNumberItem);
            }
        }

        public FormData FormData { get; set; }

        public string Title => "规则配置";

        private IEventAggregator _eventAggregator;

        public event Action<IDialogResult> RequestClose;

        public GenerateRuleViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            FormData = new();
            FormData.AppendNumberList.Add(new AppendNumber());
        }



        public void AddAppendNumberItem(AppendNumber appendNumber)
        {
            var index = FormData.AppendNumberList.IndexOf(appendNumber);
            if(index >= 0)
            {
                FormData.AppendNumberList.Insert(index+1, new ());

            }
        }

        public void RemoveAppendNumberItem(AppendNumber appendNumber)
        {
            if (FormData.AppendNumberList.Count <= 1)
            {
                FormData.AppendNumberList.Clear();
                FormData.AppendNumberList.Add(new ());
            } else
            {
                FormData.AppendNumberList.Remove(appendNumber);
            }
        }
        public void OnOk()
        {
            Log.Debug(FormData.Error);
            if(FormData.IsValid())
            {
                RequestClose(new DialogResult(ButtonResult.OK));
            }
        }
        
        public void OnCancel()
        {
            RequestClose(new DialogResult(ButtonResult.Cancel));
        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            var detail = parameters.GetValue<FormData>("detail");
            if(detail != null)
            {
                FormData = detail;
            }
        }

        public IDialogParameters OnDialogClosing(ButtonResult buttonResult)
        {
            var p = new DialogParameters();
            p.Add("detail", FormData);
            return buttonResult == ButtonResult.OK ? p : null;
        }

        public void OnDialogClosed()
        {
        }
    }
}
