using System;
using System.Windows;
using System.Windows.Input;
using DiabloIIIHotkeys.ViewModels;

namespace DiabloIIIHotkeys.Commands
{
    internal class ToggleProfileSelectionCommand : ICommand
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
            ((IMacroProfileHandler)Application.Current.MainWindow.DataContext)?.ToggleSelection(((MacroProfileViewModel)parameter)?.Profile);
        }
    }
}
