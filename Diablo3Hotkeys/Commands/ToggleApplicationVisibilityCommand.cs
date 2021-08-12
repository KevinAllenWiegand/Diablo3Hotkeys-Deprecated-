using System;
using System.Windows;
using System.Windows.Input;
using DiabloIIIHotkeys.ViewModels;

namespace DiabloIIIHotkeys.Commands
{
    internal class ToggleApplicationVisibilityCommand : ICommand
    {
        private WindowState _LastState = WindowState.Normal;

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
            var window = Application.Current.MainWindow;

            if (window.WindowState != WindowState.Minimized)
            {
                _LastState = window.WindowState;
                window.WindowState = WindowState.Minimized;
                window.ShowInTaskbar = false;
                window.Hide();
            }
            else
            {
                window.Show();
                window.ShowInTaskbar = true;
                window.WindowState = _LastState;
            }

            ((IToggleVisibility)window.DataContext).OnVisibilityChanged();
        }
    }
}
