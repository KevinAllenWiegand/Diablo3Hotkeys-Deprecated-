using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DiabloIIIHotkeys.Commands;

namespace DiabloIIIHotkeys.ViewModels
{
    internal class MainViewModel : ViewModelBase, IToggleVisibility, ILogHandler, IMacroProfileHandler
    {
        private readonly object _SyncRoot = new object();

        private bool _AllowAutoScroll = true;

        public ICommand OnToggleApplicationVisibilityCommand { get; }
        public ICommand OnExitCommand { get; }
        public ICommand OnCopyLogToClipboardCommand { get; }
        public ICommand OnClearLogCommand { get; }
        public ICommand OnToggleAutoScrollStateCommand { get; }
        public ICommand OnStopMacrosCommand { get; }
        public ICommand OnEditPreferencesCommand { get; }
        public ICommand OnEditProfilesCommand { get; }

        public string TrayIconSource => ProfileKeyManager.Instance.IsRunning ? "Resources/D3IconRunning.ico" : "Resources/D3Icon.ico";

        public string ToggleStateMenuDisplayString
        {
            get { return Application.Current.MainWindow.Visibility == Visibility.Visible ? "Hide" : "Show"; }
        }

        private bool _AreHotkeysEnabled;
        public bool AreHotkeysEnabled
        {
            get { return _AreHotkeysEnabled; }
            set
            {
                _AreHotkeysEnabled = value;
                NotifyPropertyChanged(nameof(AreHotkeysEnabled));
            }
        }

        private bool _ScrollToBottom;
        public bool ScrollToBottom
        {
            get { return _ScrollToBottom; }
            set
            {
                if (!_AllowAutoScroll)
                {
                    return;
                }

                _ScrollToBottom = value;
                NotifyPropertyChanged(nameof(ScrollToBottom));
            }
        }

        public string LogMessages { get; private set; } = String.Empty;

        public ObservableCollection<MacroProfileViewModel> MacroProfiles { get; } = new ObservableCollection<MacroProfileViewModel>();

        public bool IsProfileRunning => ProfileKeyManager.Instance.IsRunning;

        private string _CurrentProfileName = "None";
        public string CurrentProfileName
        {
            get { return _CurrentProfileName; }
            set
            {
                _CurrentProfileName = value;
                NotifyPropertyChanged(nameof(CurrentProfileName));
            }
        }

        private string _CurrentProfileMacroSummary = "0/0/0/0";
        public string CurrentProfileMacroSummary
        {
            get { return _CurrentProfileMacroSummary; }
            set
            {
                _CurrentProfileMacroSummary = value;
                NotifyPropertyChanged(nameof(CurrentProfileMacroSummary));
            }
        }

        private string _AutoScrollStateString = "Pause Auto Scroll";
        public string AutoScrollStateString
        {
            get { return _AutoScrollStateString; }
            set
            {
                _AutoScrollStateString = value;
                NotifyPropertyChanged(nameof(AutoScrollStateString));
            }
        }

        private MacroProfile _SelectedMacroProfile;
        public MacroProfile SelectedMacroProfile
        {
            get { return _SelectedMacroProfile; }
            set
            {
                _SelectedMacroProfile = value;
                CurrentProfileName = _SelectedMacroProfile != null ? _SelectedMacroProfile.Name : "None";
                CurrentProfileMacroSummary = _SelectedMacroProfile != null ? $"{_SelectedMacroProfile.Macros[0].Interval}/{_SelectedMacroProfile.Macros[1].Interval}/{_SelectedMacroProfile.Macros[2].Interval}/{_SelectedMacroProfile.Macros[3].Interval}" : "0/0/0/0";
                NotifyPropertyChanged(nameof(SelectedMacroProfile));
                OnSelectedMacroProfileChanged();
            }
        }

        public event EventHandler<MacroProfileEventArgs> SelectedMacroProfileChanged;

        public MainViewModel()
        {
            Logger.Instance.LogMessage += LogMessage;
            HotkeyManager.Instance.HotkeysRegisteredChanged += HotkeysRegisteredChanged;
            MacroProfileManager.Instance.MacroProfileAdded += MacroProfileAdded;
            MacroProfileManager.Instance.MacroProfileRemoved += MacroProfileRemoved;
            MacroProfileManager.Instance.LoadProfiles();
            ProfileKeyManager.Instance.RunningStateChanged += RunningStateChanged;

            OnToggleApplicationVisibilityCommand = new ToggleApplicationVisibilityCommand();
            OnExitCommand = new ExitApplicationCommand();
            OnCopyLogToClipboardCommand = new CopyLogToClipboardCommand();
            OnClearLogCommand = new ClearLogCommand();
            OnToggleAutoScrollStateCommand = new ToggleAutoScrollStateCommand();
            OnStopMacrosCommand = new RelayCommand(OnStopMacrosCommandImpl, parameter => IsProfileRunning);
            OnEditPreferencesCommand = new RelayCommand(OnEditPreferencesCommandImpl, parameter => !IsProfileRunning);
            OnEditProfilesCommand = new RelayCommand(OnEditProfilesCommandImpl, parameter => !IsProfileRunning);
        }

        private void OnStopMacrosCommandImpl(object parameter)
        {
            ((ICommand)new PerformProfileKeyActionCommand()).Execute(ProfileKeyActionParameters.StopActionParameters);
        }

        private void OnEditPreferencesCommandImpl(object parameter)
        {
            OpenFileWithDefaultProgram(Utils.Instance.PreferencesFilename);
        }

        private void OnEditProfilesCommandImpl(object parameter)
        {
            OpenFileWithDefaultProgram(Utils.Instance.ProfilesFilename);
        }

        private void OpenFileWithDefaultProgram(string filename)
        {
            try
            {
                Process process = new Process();

                process.StartInfo.FileName = "explorer";
                process.StartInfo.Arguments = $"\"{filename}\"";

                process.Start();
            }
            catch
            {
            }
        }

        private void MacroProfileAdded(object sender, MacroProfileEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MacroProfiles.Add(new MacroProfileViewModel(e.Profile));

                if (e.Profile.IsDefault)
                {
                    ((IMacroProfileHandler)this).ToggleSelection(e.Profile);
                }
            });
        }

        private void MacroProfileRemoved(object sender, MacroProfileEventArgs e)
        {
            var item = MacroProfiles.FirstOrDefault(i => i.Profile == e.Profile);

            if (item == null)
            {
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                MacroProfiles.Remove(item);

                if (item.IsSelected)
                {
                    CurrentProfileName = "None";
                    CurrentProfileMacroSummary = "0/0/0/0";
                }
            });
        }

        private void LogMessage(object sender, LogMessageEventArgs e)
        {
            var dateTimeString = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");

            lock (_SyncRoot)
            {
                if (!String.IsNullOrEmpty(LogMessages))
                {
                    LogMessages += Environment.NewLine;
                }

                LogMessages += $"[{dateTimeString}] {e.Message}";
            }

            NotifyPropertyChanged(nameof(LogMessages));
            ScrollToBottom = true;
        }

        private void HotkeysRegisteredChanged(object sender, System.EventArgs e)
        {
            AreHotkeysEnabled = HotkeyManager.Instance.AreHotkeysRegistered;
        }

        private void RunningStateChanged(object sender, ProfileRunningStateEventArgs e)
        {
            NotifyPropertyChanged(nameof(TrayIconSource));
            NotifyPropertyChanged(nameof(IsProfileRunning));
        }

        private void OnSelectedMacroProfileChanged()
        {
            var handler = SelectedMacroProfileChanged;

            handler?.Invoke(this, new MacroProfileEventArgs(_SelectedMacroProfile));
        }

        void IToggleVisibility.OnVisibilityChanged()
        {
            NotifyPropertyChanged(nameof(ToggleStateMenuDisplayString));
        }

        void ILogHandler.CopyLogToClipboard()
        {
            lock (_SyncRoot)
            {
                Clipboard.SetText(LogMessages);
            }
        }

        void ILogHandler.ClearLog()
        {
            lock (_SyncRoot)
            {
                LogMessages = String.Empty;
            }

            NotifyPropertyChanged(nameof(LogMessages));
        }

        void ILogHandler.ToggleAutoScrollState()
        {
            _AllowAutoScroll = !_AllowAutoScroll;
            AutoScrollStateString = _AllowAutoScroll ? "Pause Auto Scroll" : "Resume Auto Scroll";

            if (_AllowAutoScroll)
            {
                ScrollToBottom = true;
            }
        }

        void IMacroProfileHandler.ToggleSelection(MacroProfile profile)
        {
            MacroProfileViewModel toggle = null;
            var toggleWasSelected = false;

            foreach (var viewModel in MacroProfiles)
            {
                if (viewModel.Profile == profile)
                {
                    toggle = viewModel;
                    toggleWasSelected = toggle.IsSelected;
                }

                viewModel.IsSelected = false;
            }

            if (toggle != null)
            {
                toggle.IsSelected = !toggleWasSelected;
            }

            var selectedProfile = toggle != null && toggle.IsSelected ? toggle.Profile : null;
            var profileName = selectedProfile != null ? selectedProfile.Name : "None";

            Logger.Instance.Log($"Profile switched to {profileName}");

            CurrentProfileName = profileName;
            SelectedMacroProfile = selectedProfile;
            OnSelectedMacroProfileChanged();
        }
    }
}
