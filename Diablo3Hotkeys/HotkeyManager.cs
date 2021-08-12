using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using DiabloIIIHotkeys.Commands;
using DiabloIIIHotkeys.Serialization;
using DiabloIIIHotkeys.ViewModels;

namespace DiabloIIIHotkeys
{
    internal class HotkeyManager
    {
        private static Lazy<HotkeyManager> _Instance = new Lazy<HotkeyManager>(() => new HotkeyManager());

        public static HotkeyManager Instance
        {
            get { return _Instance.Value; }
        }

        private const int _TimerDueTime = 5000;
        private const string _DefaultKey1 = "1";
        private const string _DefaultKey2 = "2";
        private const string _DefaultKey3 = "3";
        private const string _DefaultKey4 = "4";
        private const string _DefaultToggleRunningMacroKey = "A";
        private const uint _DefaultToggleRunningMacroModifiers = NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT;
        private const string _DefaultRightButtonDownMacroKey = "W";
        private const uint _DefaultRightButtonDownMacroModifiers = NativeMethods.MOD_CONTROL | NativeMethods.MOD_SHIFT;

        private IntPtr _MainWindowHandle;
        private HwndSource _MainWindowHwndSource;
        private Timer _Timer;
        private bool _AlwaysAllowHotKeys = false;
        private string _RightButtonDownMacroKey = _DefaultRightButtonDownMacroKey;
        private uint _RightButtonDownMacroModifiers = _DefaultRightButtonDownMacroModifiers;

        private static readonly List<string> _ProcessNames = new List<string>() { "Diablo III64", "Diablo III" };
        private static readonly IDictionary<int, HotkeyDefinition> _HotkeyDefinitions = new Dictionary<int, HotkeyDefinition>();

        public event EventHandler<EventArgs> HotkeysRegisteredChanged;

        public bool AreHotkeysRegistered { get; set; }

        private HotkeyManager()
        {
            Preferences preferences = null;
            var key1Value = String.Empty;
            var key2Value = String.Empty;
            var key3Value = String.Empty;
            var key4Value = String.Empty;

            if (File.Exists(Utils.Instance.PreferencesFilename))
            {
                try
                {
                    preferences = JsonSerializer.Deserialize<Preferences>(File.ReadAllText(Utils.Instance.PreferencesFilename));
                }
                catch
                {
                }
            }

            if (preferences != null)
            {
                _AlwaysAllowHotKeys = preferences.AlwaysAllowHotkeys;

                if (preferences.SkillKeybindOverrides != null)
                {
                    if (!String.IsNullOrEmpty(preferences.SkillKeybindOverrides.One?.Key))
                    {
                        if (preferences.SkillKeybindOverrides.One.UseNumPad && int.TryParse(preferences.SkillKeybindOverrides.One.Key, out int value))
                        {
                            key1Value = $"{{NumPad{preferences.SkillKeybindOverrides.One.Key}}}";
                        }
                        else
                        {
                            key1Value = preferences.SkillKeybindOverrides.One.Key;
                        }
                    }

                    if (!String.IsNullOrEmpty(preferences.SkillKeybindOverrides.Two?.Key))
                    {
                        if (preferences.SkillKeybindOverrides.Two.UseNumPad && int.TryParse(preferences.SkillKeybindOverrides.Two.Key, out int value))
                        {
                            key2Value = $"{{NumPad{preferences.SkillKeybindOverrides.Two.Key}}}";
                        }
                        else
                        {
                            key2Value = preferences.SkillKeybindOverrides.Two.Key;
                        }
                    }

                    if (!String.IsNullOrEmpty(preferences.SkillKeybindOverrides.Three?.Key))
                    {
                        if (preferences.SkillKeybindOverrides.Three.UseNumPad && int.TryParse(preferences.SkillKeybindOverrides.Three.Key, out int value))
                        {
                            key3Value = $"{{NumPad{preferences.SkillKeybindOverrides.Three.Key}}}";
                        }
                        else
                        {
                            key3Value = preferences.SkillKeybindOverrides.Three.Key;
                        }
                    }

                    if (!String.IsNullOrEmpty(preferences.SkillKeybindOverrides.Four?.Key))
                    {
                        if (preferences.SkillKeybindOverrides.Four.UseNumPad && int.TryParse(preferences.SkillKeybindOverrides.Four.Key, out int value))
                        {
                            key4Value = $"{{NumPad{preferences.SkillKeybindOverrides.Four.Key}}}";
                        }
                        else
                        {
                            key4Value = preferences.SkillKeybindOverrides.Four.Key;
                        }
                    }
                }
            }

            _HotkeyDefinitions.Add(0, new HotkeyDefinition(NativeMethods.MOD_CONTROL, NativeMethods.GetVirtualKeyForValue("0"), ProfileKeyActionParameters.StopActionParameters));
            _HotkeyDefinitions.Add(1, new HotkeyDefinition(NativeMethods.MOD_CONTROL, NativeMethods.GetVirtualKeyForValue("1"), new ProfileKeyActionParameters(ProfileKeyAction.Action, key1Value ?? _DefaultKey1)));
            _HotkeyDefinitions.Add(2, new HotkeyDefinition(NativeMethods.MOD_CONTROL, NativeMethods.GetVirtualKeyForValue("2"), new ProfileKeyActionParameters(ProfileKeyAction.Action, key2Value ?? _DefaultKey2)));
            _HotkeyDefinitions.Add(3, new HotkeyDefinition(NativeMethods.MOD_CONTROL, NativeMethods.GetVirtualKeyForValue("3"), new ProfileKeyActionParameters(ProfileKeyAction.Action, key3Value ?? _DefaultKey3)));
            _HotkeyDefinitions.Add(4, new HotkeyDefinition(NativeMethods.MOD_CONTROL, NativeMethods.GetVirtualKeyForValue("4"), new ProfileKeyActionParameters(ProfileKeyAction.Action, key4Value ?? _DefaultKey4)));
            // This is for the Right Mouse Button Down Macro
            _HotkeyDefinitions.Add(6, null);

            var toggleRunningMacroKey = _DefaultToggleRunningMacroKey;
            var toggleRunningMacroModifiers = _DefaultToggleRunningMacroModifiers;

            if (!String.IsNullOrEmpty(preferences?.ToggleProfileMacro?.Key))
            {
                toggleRunningMacroKey = preferences.ToggleProfileMacro.Key;
                toggleRunningMacroModifiers = NativeMethods.MOD_NONE;

                if (preferences.ToggleProfileMacro.CtrlModifier)
                {
                    toggleRunningMacroModifiers |= NativeMethods.MOD_CONTROL;
                }

                if (preferences.ToggleProfileMacro.AltModifier)
                {
                    toggleRunningMacroModifiers |= NativeMethods.MOD_ALT;
                }

                if (preferences.ToggleProfileMacro.ShiftModifier)
                {
                    toggleRunningMacroModifiers |= NativeMethods.MOD_SHIFT;
                }
            }

            _HotkeyDefinitions.Add(5, new HotkeyDefinition(toggleRunningMacroModifiers, NativeMethods.GetVirtualKeyForValue(toggleRunningMacroKey), ProfileKeyActionParameters.ToggleRunningMacroProfileParameters));

            if (!String.IsNullOrEmpty(preferences?.RightButtonDownMacro?.Key))
            {
                _RightButtonDownMacroKey = preferences.RightButtonDownMacro.Key;
                _RightButtonDownMacroModifiers = NativeMethods.MOD_NONE;

                if (preferences.RightButtonDownMacro.CtrlModifier)
                {
                    _RightButtonDownMacroModifiers |= NativeMethods.MOD_CONTROL;
                }

                if (preferences.RightButtonDownMacro.AltModifier)
                {
                    _RightButtonDownMacroModifiers |= NativeMethods.MOD_ALT;
                }

                if (preferences.RightButtonDownMacro.ShiftModifier)
                {
                    _RightButtonDownMacroModifiers |= NativeMethods.MOD_SHIFT;
                }
            }

            ProfileKeyManager.Instance.SetKeybinds(new Dictionary<int, string>()
            {
                { 1, key1Value ?? _DefaultKey1 },
                { 2, key2Value ?? _DefaultKey2 },
                { 3, key3Value ?? _DefaultKey3 },
                { 4, key4Value ?? _DefaultKey4 },
            });
        }

        public void Start()
        {
            _MainWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            _MainWindowHwndSource = HwndSource.FromHwnd(_MainWindowHandle);
            _MainWindowHwndSource.AddHook(HwndHook);

            _Timer = new Timer(TimerCallback, null, _TimerDueTime, Timeout.Infinite);
        }

        private void TimerCallback(object state)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var foundProcess = false;

                    if (!_AlwaysAllowHotKeys)
                    {
                        foreach (var processName in _ProcessNames)
                        {
                            try
                            {
                                foundProcess = Process.GetProcessesByName(processName).FirstOrDefault() != null;

                                if (foundProcess)
                                {
                                    break;
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    else
                    {
                        foundProcess = true;
                    }

                    if (foundProcess)
                    {
                        if (!AreHotkeysRegistered)
                        {
                            Logger.Instance.Log("Registering hotkeys.");
                            AreHotkeysRegistered = true;
                            OnHotkeysRegisteredChanged();

                            foreach (var kvp in _HotkeyDefinitions)
                            {
                                if (kvp.Value == null)
                                {
                                    continue;
                                }

                                NativeMethods.RegisterHotKey(_MainWindowHandle, kvp.Key, kvp.Value.HotkeyModifiers, kvp.Value.HotkeyKey);
                            }

                            Logger.Instance.Log($"Skill 1 is bound to {_HotkeyDefinitions[1].ActionParameters.Parameter}");
                            Logger.Instance.Log($"Skill 2 is bound to {_HotkeyDefinitions[2].ActionParameters.Parameter}");
                            Logger.Instance.Log($"Skill 3 is bound to {_HotkeyDefinitions[3].ActionParameters.Parameter}");
                            Logger.Instance.Log($"Skill 4 is bound to {_HotkeyDefinitions[4].ActionParameters.Parameter}");
                        }
                    }
                    else
                    {
                        UnregisterHotkeysIfNecessary();
                        ((ILogHandler)Application.Current.MainWindow.DataContext)?.ClearLog();
                    }

                    if (!_AlwaysAllowHotKeys)
                    {
                        _Timer.Change(_TimerDueTime, Timeout.Infinite);
                    }
                });
            }
            catch
            {
            }
        }

        public void Stop()
        {
            _Timer.Change(Timeout.Infinite, Timeout.Infinite);
            _MainWindowHwndSource.RemoveHook(HwndHook);
            UnregisterHotkeysIfNecessary();
        }

        public void RegisterRightButtonDownHotkeyIfNecessary()
        {
            if (_HotkeyDefinitions[6] != null)
            {
                return;
            }

            Logger.Instance.Log("Registering hotkey for right mouse button down.");

            var hotkeyDefinition = new HotkeyDefinition(_RightButtonDownMacroModifiers, NativeMethods.GetVirtualKeyForValue(_RightButtonDownMacroKey), ProfileKeyActionParameters.SendRightMouseDownParameters);

            _HotkeyDefinitions[6] = hotkeyDefinition;
            NativeMethods.RegisterHotKey(_MainWindowHandle, 6, hotkeyDefinition.HotkeyModifiers, hotkeyDefinition.HotkeyKey);
        }

        public void UnRegisterRightButtonDownHotkeyIfNecessary()
        {
            if (_HotkeyDefinitions[6] == null)
            {
                return;
            }

            Logger.Instance.Log("Unregistering hotkey for right mouse button down.");

            NativeMethods.UnregisterHotKey(_MainWindowHandle, 6);
            _HotkeyDefinitions[6] = null;
        }

        private void UnregisterHotkeysIfNecessary()
        {
            if (!AreHotkeysRegistered)
            {
                return;
            }

            Logger.Instance.Log("Unregistering hotkeys.");

            foreach (var kvp in _HotkeyDefinitions)
            {
                if (kvp.Value == null)
                {
                    continue;
                }

                NativeMethods.UnregisterHotKey(_MainWindowHandle, kvp.Key);
            }

            AreHotkeysRegistered = false;
            OnHotkeysRegisteredChanged();
            ((ICommand)new PerformProfileKeyActionCommand()).Execute(ProfileKeyActionParameters.ResetActionParameters);
        }

        private void OnHotkeysRegisteredChanged()
        {
            var handler = HotkeysRegisteredChanged;

            handler?.Invoke(this, EventArgs.Empty);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeMethods.WM_HOTKEY:
                    var hotkeyId = wParam.ToInt32();

                    Logger.Instance.Log($"Received Hotkey ID {hotkeyId}.");

                    if (_HotkeyDefinitions.TryGetValue(hotkeyId, out HotkeyDefinition hotkeyDefinition))
                    {
                        handled = true;

                        Task.Run(() =>
                        {
                            Logger.Instance.Log($"Executing handler for Hotkey ID {hotkeyId}.");

                            var start = DateTime.Now;

                            ((ICommand)new PerformProfileKeyActionCommand()).Execute(hotkeyDefinition.ActionParameters);

                            var milliseconds = DateTime.Now.Subtract(start).TotalMilliseconds;

                            Logger.Instance.Log($"Handler for Hotkey ID {hotkeyId} completed in {milliseconds}ms.");
                        });
                    }

                    break;
            }

            return IntPtr.Zero;
        }
    }
}
