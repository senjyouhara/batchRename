//using Prism.Common;
//using Prism.Ioc;
//using Prism.Mvvm;
//using Prism.Services.Dialogs;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;

//namespace Senjyouhara.Main.Core.Manager.Dialog
//{
//    public class DialogHostService : DialogService, IDialogHostService
//    {
//        private readonly IContainerExtension containerExtension;

//        public DialogHostService(IContainerExtension containerExtension) : base(containerExtension)
//        {
//            this.containerExtension = containerExtension;
//            containerExtension.Register<MyControl>();
//        }

//        public void ShowDialogAsync(string name)
//        {
//            ShowDialogAsync(name, null, null);
//        }
//        public void ShowDialogAsync(string name, IDialogParameters parameters)
//        {
//            ShowDialogAsync(name, parameters, null);
//        }
//        public void ShowDialogAsync(string name, Action<IDialogResult> callback)
//        {
//            ShowDialogAsync(name, null, callback);
//        }

//        public void ShowDialogAsync(string name, IDialogParameters parameters, Action<IDialogResult> callback)
//        {

//            if (parameters == null)
//                parameters = new DialogParameters();

//            MyControl DialogWarp = containerExtension.Resolve<MyControl>();
//            Action<IDialogResult> requestCloseHandler = null;

//            requestCloseHandler = delegate (IDialogResult o)
//            {
//                if (!((IDialogAware)DialogWarp.DataContext).CanCloseDialog())
//                {
//                    return;
//                }
//                DialogWarp.Result = o;
//                DialogWarp.Close();
//            };
//            RoutedEventHandler loadedHandler = null;
//            loadedHandler = delegate
//            {
//                DialogWarp.Loaded -= loadedHandler;
//                ((IDialogAware)DialogWarp.DataContext).RequestClose += requestCloseHandler;
//            };
//            DialogWarp.Loaded += loadedHandler;


//            RoutedEventHandler closedHandler = null;
//            closedHandler = delegate
//            {
//                DialogWarp.Unloaded -= closedHandler;
//                ((IDialogAware)DialogWarp.DataContext).RequestClose -= requestCloseHandler;
//                ((IDialogAware)DialogWarp.DataContext).OnDialogClosed();
//                if (DialogWarp.Result == null)
//                {
//                    DialogWarp.Result = new DialogResult();
//                }

//                callback?.Invoke(DialogWarp.Result);
//                DialogWarp.DataContext = null;
//                DialogWarp.Content = null;
//            };
//            DialogWarp.Unloaded += closedHandler;


//            //从容器中取出弹出窗口实例
//            var content = containerExtension.Resolve<object>(name);

//            //验证实例的有效性
//            if (!(content is FrameworkElement dialogContent))
//                throw new NullReferenceException("A dialog's content must be a FrameworkElement");

//            if (dialogContent is FrameworkElement view && view.DataContext is null && ViewModelLocator.GetAutoWireViewModel(view) is null)
//            {
//                ViewModelLocator.SetAutoWireViewModel(view, true);
//            }

//            if (!(dialogContent.DataContext is IDialogAware viewModel))
//                throw new NullReferenceException("A dialog's ViewModel must implement the IDialogAware interface");

//            DialogWarp.dialogContent.Children.Add(dialogContent);
//            DialogWarp.DataContext = viewModel;

//            DialogWarp.dialogWarp.Width = dialogContent.Width;
//            DialogWarp.dialogWarp.Height = dialogContent.Height;
//            if (dialogContent.Style != null)
//                DialogWarp.Style = dialogContent.Style;

//            MvvmHelpers.ViewAndViewModelAction(viewModel, delegate (IDialogAware d)
//            {
//                d.OnDialogOpened(parameters);
//            });

//            DialogWarp.Show();

//        }
//    }
//}
