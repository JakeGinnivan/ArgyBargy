using System;
using System.Windows.Input;

namespace ArgyBargy
{
    class DelegateCommand<T> : ICommand, IRaiseCanExecuteChanged
    {
        private readonly Func<T, bool> canExecuteMethod;
        private readonly Action<T> executeMethod;
        private bool isExecuting;

        public DelegateCommand(Action<T> executeMethod)
            : this(executeMethod, null)
        {
        }

        public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
        {
            if ((executeMethod == null) && (canExecuteMethod == null))
            {
                throw new ArgumentNullException("executeMethod", @"Execute Method cannot be null");
            }
            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return !isExecuting && CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            isExecuting = true;
            try
            {
                RaiseCanExecuteChanged();
                Execute((T)parameter);
            }
            finally
            {
                isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public bool CanExecute(T parameter)
        {
            if (canExecuteMethod == null)
                return true;

            return canExecuteMethod(parameter);
        }

        public void Execute(T parameter)
        {
            executeMethod(parameter);
        }
    }
}