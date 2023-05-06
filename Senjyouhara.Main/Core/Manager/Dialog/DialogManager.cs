using Caliburn.Micro;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;

namespace Senjyouhara.Main.Core.Manager.Dialog
{
    public class DialogManager : WindowManager, IDialogManager
    {
        public void ShowMyDialogAsync(object rootModel)
        {
            ShowMyDialogAsync(rootModel, null, null);
        }

        
        public void ShowMyDialogAsync(object rootModel, IDialogParameters parameters = null)
        {
            ShowMyDialogAsync(rootModel, parameters, null);
        }

        public void ShowMyDialogAsync(object rootModel, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            object context = null;
            if (parameters == null) parameters = new DialogParameters();

            var view = ViewLocator.LocateForModel(rootModel, null, context);
            ViewModelBinder.Bind(rootModel, view, context);
            MyControl DialogWarp = new MyControl();

            if (!(rootModel is IDialogAware dialogView))
            {
                throw new Exception("页面必须实现IDialogAware 接口");
            }


            Action<IDialogResult> requestCloseHandler = null;
            requestCloseHandler = delegate (IDialogResult o)
            {
                if (!((IDialogAware)DialogWarp.DataContext).CanCloseDialog())
                {
                    return;
                }
                DialogWarp.Result = o;
                DialogWarp.Close();
            };
            RoutedEventHandler loadedHandler = null;
            loadedHandler = delegate
            {
                DialogWarp.Loaded -= loadedHandler;
                ((IDialogAware)DialogWarp.DataContext).RequestClose += requestCloseHandler;
            };
            DialogWarp.Loaded += loadedHandler;


            RoutedEventHandler closedHandler = null;
            closedHandler = delegate
            {
                DialogWarp.Unloaded -= closedHandler;
                ((IDialogAware)DialogWarp.DataContext).RequestClose -= requestCloseHandler;
                if (DialogWarp.Result == null)
                {
                    DialogWarp.Result = new DialogResult();
                }
                var dialogParams = ((IDialogAware)DialogWarp.DataContext).OnDialogClosed(DialogWarp.Result.Result);
                DialogWarp.Result.Parameters = dialogParams ?? parameters;
                callback?.Invoke(DialogWarp.Result);
                DialogWarp.DataContext = null;
                DialogWarp.Content = null;
            };
            DialogWarp.Unloaded += closedHandler;

            var dialogContent = (FrameworkElement)view;

            DialogWarp.dialogContent.Children.Add(dialogContent);
            DialogWarp.DataContext = rootModel;

            DialogWarp.dialogWarp.Width = dialogContent.Width;
            // 37 为 dialog 标题栏高度
            DialogWarp.dialogWarp.Height = dialogContent.Height + 37;
            if (dialogContent.Style != null)
                DialogWarp.dialogWarp.Style = dialogContent.Style;

            var dialogModel = (IDialogAware) rootModel;
            dialogModel.OnDialogOpened(parameters);
            DialogWarp.Show();
        }

      
    }
}
