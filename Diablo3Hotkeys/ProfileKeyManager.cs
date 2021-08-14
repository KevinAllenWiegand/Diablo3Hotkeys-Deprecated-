using System;
using DiabloIIIHotkeys.Commands;
using System.Collections.Generic;
using System.Threading;
using DiabloIIIHotkeys.ViewModels;
using System.Windows;
using System.Media;

namespace DiabloIIIHotkeys
{
    internal class ProfileKeyManager
    {
        private static Lazy<ProfileKeyManager> _Instance = new Lazy<ProfileKeyManager>(() => new ProfileKeyManager());
        private static SoundPlayer _SoundPlayer;

        public static ProfileKeyManager Instance
        {
            get { return _Instance.Value; }
        }

        private readonly object _SyncRoot = new object();

        private const int _TimerInterval = 500;

        private IDictionary<int, string> _Keybinds;
        private Timer _Timer = null;
        private MacroProfile _CurrentMacroProfile;
        private bool _CurrentProfileIsActive = false;
        private List<KeypressParameters> _KeysToPress = new List<KeypressParameters>();
        private bool _KeysChangedSinceLastExecute = false;

        public event EventHandler<ProfileRunningStateEventArgs> RunningStateChanged;

        private bool _IsRunning;
        public bool IsRunning
        {
            get { return _IsRunning; }
            private set
            {
                if (value == _IsRunning)
                {
                    return;
                }

                _IsRunning = value;
                OnRunningStateChanged();
            }
        }

        private ProfileKeyManager()
        {
            _SoundPlayer = new SoundPlayer("Goblin.wav");
            _Timer = new Timer(TimerCallback, null, _TimerInterval, _TimerInterval);
            MacroProfileManager.Instance.MacroProfileRemoved += MacroProfileRemoved;
            Application.Current.MainWindow.DataContextChanged += DataContextChanged;
        }

        private void DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Application.Current.MainWindow.DataContext == null)
            {
                return;
            }

            var profileHandler = (IMacroProfileHandler)Application.Current.MainWindow.DataContext;

            profileHandler.SelectedMacroProfileChanged += SelectedMacroProfileChanged;
            _CurrentMacroProfile = profileHandler.SelectedMacroProfile;
        }

        private void SelectedMacroProfileChanged(object sender, MacroProfileEventArgs e)
        {
            lock (_SyncRoot)
            {
                RemoveCurrentProfileKeysInLock();
                _CurrentMacroProfile = e.Profile;

                if (_CurrentProfileIsActive)
                {
                    AddCurrentProfileKeysInLock();
                }

                if (e.Profile.AllowRightMouseButtonDown)
                {
                    HotkeyManager.Instance.RegisterRightButtonDownHotkeyIfNecessary();
                }
                else
                {
                    HotkeyManager.Instance.UnRegisterRightButtonDownHotkeyIfNecessary();
                }
            }
        }

        public void SetKeybinds(Dictionary<int, string> keybinds)
        {
            _Keybinds = keybinds;
        }

        public void Execute(ProfileKeyActionParameters actionParameters)
        {
            lock (_SyncRoot)
            {
                switch (actionParameters.Action)
                {
                    case ProfileKeyAction.Reset:
                        Logger.Instance.Log("Resetting profile.");
                        _KeysToPress.Clear();
                        _KeysChangedSinceLastExecute = true;
                        IsRunning = false;

                        break;
                    case ProfileKeyAction.ToggleProfileRunning:
                        try
                        {
                            _SoundPlayer.Play();
                        }
                        catch (Exception ex)
                        {
                            Logger.Instance.Log($"Error playing sound: {ex.Message}.");
                        }

                        if (_CurrentMacroProfile != null)
                        {
                            _CurrentProfileIsActive = !_CurrentProfileIsActive;

                            if (_CurrentProfileIsActive)
                            {
                                Logger.Instance.Log($"Starting Profile {_CurrentMacroProfile.Name}.");
                                AddCurrentProfileKeysInLock();
                                _KeysChangedSinceLastExecute = true;
                            }
                            else
                            {
                                Logger.Instance.Log($"Stopping Profile {_CurrentMacroProfile.Name}.");
                                RemoveCurrentProfileKeysInLock();
                            }
                        }

                        IsRunning = _KeysToPress.Count > 0;

                        break;
                    case ProfileKeyAction.RightMouseDown:
                        Logger.Instance.Log("Sending right mouse down.");

                        var inputs = new NativeMethods.INPUT[]
                        {
                            new NativeMethods.INPUT
                            {
                                type = (uint)NativeMethods.INPUT_TYPES.INPUT_MOUSE,
                                U = new NativeMethods.InputUnion() { mi = new NativeMethods.MOUSEINPUT { dwFlags = NativeMethods.MOUSEEVENTF.RIGHTDOWN } }
                            }
                        };

                        NativeMethods.SendInput(1, inputs, NativeMethods.INPUT.Size);

                        break;
                }
            }
        }

        private void TimerCallback(object state)
        {
            lock (_SyncRoot)
            {
                if (_Timer == null || _KeysToPress.Count == 0)
                {
                    return;
                }

                var keys = new List<string>();
                var keysString = String.Empty;
                var currentTime = DateTime.Now;

                foreach (var parameter in _KeysToPress)
                {
                    if (parameter.NextRunTime == DateTime.MinValue || currentTime >= parameter.NextRunTime)
                    {
                        keys.Add(parameter.Key);
                        keysString += parameter.Key;
                        parameter.UpdateNextRunTime();
                    }
                }

                if (!String.IsNullOrEmpty(keysString))
                {
                    if (_KeysChangedSinceLastExecute)
                    {
                        Logger.Instance.Log($"Sending \"{keysString}\".");
                        _KeysChangedSinceLastExecute = false;
                    }

                    SendKeys(keys);
                }
            }
        }

        private void SendKeys(IEnumerable<string> keys)
        {
            var inputs = new List<NativeMethods.INPUT>();

            foreach (var key in keys)
            {
                var vkey = (short)NativeMethods.GetVirtualKeyForValue(key);
                var input = new NativeMethods.INPUT
                {
                    type = (uint)NativeMethods.INPUT_TYPES.INPUT_KEYBOARD,
                    U = new NativeMethods.InputUnion() { ki = new NativeMethods.KEYBDINPUT() { wVk = vkey } }
                };

                inputs.Add(input);

                input = new NativeMethods.INPUT
                {
                    type = (uint)NativeMethods.INPUT_TYPES.INPUT_KEYBOARD,
                    U = new NativeMethods.InputUnion() { ki = new NativeMethods.KEYBDINPUT() { wVk = vkey, dwFlags = NativeMethods.KEYEVENTF.KEYUP } }
                };

                inputs.Add(input);
            }

            var inputsArray = inputs.ToArray();

            NativeMethods.SendInput((uint)inputsArray.Length, inputsArray, NativeMethods.INPUT.Size);
        }

        private void MacroProfileRemoved(object sender, MacroProfileEventArgs e)
        {
            lock (_SyncRoot)
            {
                if (_CurrentMacroProfile == e.Profile)
                {
                    _CurrentProfileIsActive = false;
                    RemoveCurrentProfileKeysInLock();
                    _CurrentMacroProfile = null;
                }
            }
        }

        private void RemoveCurrentProfileKeysInLock()
        {
            if (_CurrentMacroProfile != null)
            {
                foreach (var macro in _CurrentMacroProfile.Macros)
                {
                    _KeysToPress.Remove(new KeypressParameters(GetKeyFromSkill(macro.Skill), macro.Interval));
                }
            }
        }

        private void AddCurrentProfileKeysInLock()
        {
            if (_CurrentMacroProfile != null)
            {
                foreach (var macro in _CurrentMacroProfile.Macros)
                {
                    if (macro.Interval < 1)
                    {
                        continue;
                    }

                    _KeysToPress.Add(new KeypressParameters(GetKeyFromSkill(macro.Skill), macro.Interval));
                }
            }
        }

        private string GetKeyFromSkill(Skill skill)
        {
            switch (skill)
            {
                case Skill.Skill1:
                    return _Keybinds[1];
                case Skill.Skill2:
                    return _Keybinds[2];
                case Skill.Skill3:
                    return _Keybinds[3];
                case Skill.Skill4:
                    return _Keybinds[4];
                default:
                    return String.Empty;
            }
        }

        private void OnRunningStateChanged()
        {
            var handler = RunningStateChanged;

            handler.Invoke(this, new ProfileRunningStateEventArgs(_IsRunning));
        }
    }
}
