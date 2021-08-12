using System;
using System.Windows.Input;

namespace DiabloIIIHotkeys.Commands
{
    internal class RelayCommand : ICommand
    {
        private Action<object> _Execute;
        private Func<object, bool> _CanExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _Execute = execute;
            _CanExecute = canExecute;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return _CanExecute != null ? _CanExecute(parameter) : true;
        }

        void ICommand.Execute(object parameter)
        {
            if (_Execute == null)
            {
                return;
            }

            _Execute(parameter);
        }
    }
}
