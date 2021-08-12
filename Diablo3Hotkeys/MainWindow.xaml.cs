using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DiabloIIIHotkeys.Commands;
using DiabloIIIHotkeys.ViewModels;

namespace DiabloIIIHotkeys
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;
                Hide();
            }));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HotkeyManager.Instance.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            HotkeyManager.Instance.Stop();
            ((ICommand)new PerformProfileKeyActionCommand()).Execute(ProfileKeyActionParameters.ResetActionParameters);

            base.OnClosed(e);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ShowInTaskbar = false;
                Hide();
            }
        }
    }
}
