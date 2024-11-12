using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Senjyouhara.Common.Base
{

    public abstract class DelegateCommandBase : ICommand
    {

        public event EventHandler CanExecuteChanged;
        private SynchronizationContext _synchronizationContext;

        protected abstract void Execute(object parameter);
        protected abstract bool CanExecute(object parameter);

        protected DelegateCommandBase()
        {
            _synchronizationContext = SynchronizationContext.Current;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            Execute(parameter);
        }

        protected virtual void OnCanExecuteChanged()
        {
            var handler = CanExecuteChanged;
            if (handler != null)
            {
                if (_synchronizationContext != null && _synchronizationContext != SynchronizationContext.Current)
                    _synchronizationContext.Post((o) => handler.Invoke(this, EventArgs.Empty), null);
                else
                    handler.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public class DelegateCommand : DelegateCommandBase
    {
        private Action execute;                     //定义成员
        private Func<bool> canExecute;//Predicate：述语//定义成员

        public DelegateCommand(Action execute)       //定义Action，CanExecute
           : this(execute, () => true)
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

        public bool CanExecute()            //CanExecute方法
        {
            return canExecute != null && canExecute();
        }

        public void Execute()              //Execute方法
        {
            execute();
        }

        public void Destroy()                          //销毁方法
        {
            canExecute = () => false;
            execute = () => { return; };
        }

        protected override void Execute(object parameter)
        {
            Execute();
        }

        protected override bool CanExecute(object parameter)
        {
            return CanExecute();
        }
    }

    public class DelegateCommand<T> : DelegateCommandBase
    {
        private Action<T> execute;                     //定义成员

        private Func<T, bool> canExecute;//Predicate：述语//定义成员

        public DelegateCommand(Action<T> execute)       //定义Action，CanExecute
           : this(execute, t => true)
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

        public void Destroy()                          //销毁方法
        {
            canExecute = _ => false;
            execute = _ => { return; };
        }

        public bool CanExecute(T parameter)
        {
            return canExecute != null && canExecute(parameter);
        }

        public void Execute(T parameter)
        {
            execute(parameter);
        }

        protected override void Execute(object parameter)
        {
            execute((T)parameter);
        }

        protected override bool CanExecute(object parameter)
        {
            return canExecute != null && canExecute((T)parameter);
        }
    }
}
