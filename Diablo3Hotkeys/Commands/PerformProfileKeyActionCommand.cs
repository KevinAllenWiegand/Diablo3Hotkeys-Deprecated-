using System;
using System.Windows.Input;

namespace DiabloIIIHotkeys.Commands
{
    internal class PerformProfileKeyActionCommand : ICommand
    {
        event EventHandler ICommand.CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return true;
        }

        void ICommand.Execute(object parameter)
        {
            if (!(parameter is ProfileKeyActionParameters actionParameters))
            {
                return;
            }

            ProfileKeyManager.Instance.Execute(actionParameters);
        }
    }
}
