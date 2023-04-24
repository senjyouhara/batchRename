using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Senjyouhara.UI.Common
{
    public sealed class RelayCommand<T> : IRelayCommand
    {
        readonly Action<T> _execute;

        readonly Func<bool> _canExecute;

        /// <summary>
        /// Event occuring when encapsulated canExecute method is changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /// <summary>
        /// Creates new instance of <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="execute">Action to be executed.</param>
        public RelayCommand(Action execute) : this(execute, null)
        {
            // Delegated to RelayCommand(Action execute, Func<bool> canExecute)
        }

        /// <summary>
        /// Creates new instance of <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="execute">Action with <see cref="object"/> parameter to be executed.</param>
        public RelayCommand(Action<T> execute): this(execute, null)
        {
            // Delegated to RelayCommand(Action<object> execute, Func<bool> canExecute)
        }

        /// <summary>
        /// Creates new instance of <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="execute">Action to be executed.</param>
        /// <param name="canExecute">Encapsulated method determining whether to execute action.</param>
        /// <exception cref="ArgumentNullException">Exception occurring when no <see cref="Action"/> is defined.</exception>
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = p => execute();
            _canExecute = canExecute;
        }

        /// <summary>
        /// Creates new instance of <see cref="RelayCommand"/>.
        /// </summary>
        /// <param name="execute">Action with <see cref="object"/> parameter to be executed.</param>
        /// <param name="canExecute">Encapsulated method determining whether to execute action.</param>
        /// <exception cref="ArgumentNullException">Exception occurring when no <see cref="Action"/> is defined.</exception>
        public RelayCommand(Action<T> execute, Func<bool> canExecute)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        /// <inheritdoc cref="IRelayCommand.CanExecute" />
        public bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        /// <inheritdoc cref="IRelayCommand.Execute" />
        public void Execute(object parameter)
        {
            _execute((T) parameter);
        }
    }
}
