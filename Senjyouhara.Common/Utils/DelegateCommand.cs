using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Senjyouhara.Common.Utils
{

    public interface IDelegateCommand: ICommand
    {
        bool CanExecute();

        void Execute();

    }
    public interface IDelegateCommand<T> : ICommand
    {

    }

    public class DelegateCommand: IDelegateCommand
    {
        private Action execute;                     //定义成员

        private Func<bool> canExecute;//Predicate：述语//定义成员

        private event EventHandler CanExecuteChangedInternal;//事件

        public DelegateCommand(Action execute)       //定义Action，CanExecute
           : this(execute, DefaultCanExecute)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)//定义
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged        //CanExecuteChanged事件处理方法
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this.CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                this.CanExecuteChangedInternal -= value;
            }
        }

        public bool CanExecute()            //CanExecute方法
        {
            return this.canExecute != null && this.canExecute();
        }

        public void Execute()              //Execute方法
        {
            this.execute();
        }

        public void OnCanExecuteChanged()                //OnCanExecute方法
        {
            EventHandler handler = this.CanExecuteChangedInternal;
            if (handler != null)
            {
                //DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        public void Destroy()                          //销毁方法
        {
            this.canExecute = () => false;
            this.execute = () => { return; };
        }

        private static bool DefaultCanExecute()  //DefaultCanExecute方法
        {
            return true;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute != null && this.canExecute();

        }

        public void Execute(object parameter)
        {
            this.execute();
        }
    }

    public class DelegateCommand<T> : IDelegateCommand<T>
    {
        private Action<T> execute;                     //定义成员

        private Func<T, bool> canExecute;//Predicate：述语//定义成员

        private event EventHandler CanExecuteChangedInternal;//事件

        public DelegateCommand(Action<T> execute)       //定义Action，CanExecute
           : this(execute, DefaultCanExecute)
        {
        }

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute)//定义
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException("canExecute");
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged        //CanExecuteChanged事件处理方法
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this.CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                this.CanExecuteChangedInternal -= value;
            }
        }

        public void OnCanExecuteChanged()                //OnCanExecute方法
        {
            EventHandler handler = this.CanExecuteChangedInternal;
            if (handler != null)
            {
                //DispatcherHelper.BeginInvokeOnUIThread(() => handler.Invoke(this, EventArgs.Empty));
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        public void Destroy()                          //销毁方法
        {
            this.canExecute = _ => false;
            this.execute = _ => { return; };
        }

        private static bool DefaultCanExecute(T parameter)  //DefaultCanExecute方法
        {
            return true;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute != null && this.canExecute((T) parameter);
        }

        public void Execute(object parameter)
        {
            this.execute((T)parameter);
        }
    }
}
