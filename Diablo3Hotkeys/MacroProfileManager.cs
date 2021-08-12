using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using DiabloIIIHotkeys.Serialization;

namespace DiabloIIIHotkeys
{
    internal class MacroProfileManager
    {
        private static Lazy<MacroProfileManager> _Instance = new Lazy<MacroProfileManager>(() => new MacroProfileManager());
        public static MacroProfileManager Instance
        {
            get { return _Instance.Value; }
        }

        private List<MacroProfile> _Profiles = new List<MacroProfile>();
        public IReadOnlyCollection<MacroProfile> Profiles
        {
            get
            {
                return new ReadOnlyCollection<MacroProfile>(_Profiles);
            }
        }

        public event EventHandler<MacroProfileEventArgs> MacroProfileAdded;
        public event EventHandler<MacroProfileEventArgs> MacroProfileRemoved;

        private MacroProfileManager()
        {
        }

        internal void LoadProfiles()
        {
            if (!File.Exists(Utils.Instance.ProfilesFilename))
            {
                return;
            }

            Profiles profiles = null;

            try
            {
                profiles = JsonSerializer.Deserialize<Profiles>(File.ReadAllText(Utils.Instance.ProfilesFilename));
            }
            catch
            {
            }

            if (profiles == null)
            {
                return;
            }

            foreach (var profile in profiles.Items)
            {
                if (String.IsNullOrEmpty(profile.Name) || profile.Skills == null || (profile.Skills.One == null && profile.Skills.Two == null && profile.Skills.Three == null && profile.Skills.Four == null))
                {
                    continue;
                }

                Add(new MacroProfile(profile.Name, new List<Macro>
                {
                    new Macro(Skill.Skill1, profile.Skills.One != null ? profile.Skills.One.Interval : 0),
                    new Macro(Skill.Skill2, profile.Skills.Two != null ? profile.Skills.Two.Interval : 0),
                    new Macro(Skill.Skill3, profile.Skills.Three != null ? profile.Skills.Three.Interval : 0),
                    new Macro(Skill.Skill4, profile.Skills.Four != null ? profile.Skills.Four.Interval : 0)
                }, profile.IsDefault, profile.AllowRightMouseButtonDown));
            }
        }

        public void Add(MacroProfile profile)
        {
            if (_Profiles.Contains(profile))
            {
                return;
            }

            _Profiles.Add(profile);
            OnMacroProfileAdded(profile);
        }

        public void Remove(MacroProfile profile)
        {
            if (!_Profiles.Contains(profile))
            {
                return;
            }

            _Profiles.Remove(profile);
            OnMacroProfileRemoved(profile);
        }

        private void OnMacroProfileAdded(MacroProfile profile)
        {
            var handler = MacroProfileAdded;

            handler?.Invoke(this, new MacroProfileEventArgs(profile));
        }

        private void OnMacroProfileRemoved(MacroProfile profile)
        {
            var handler = MacroProfileRemoved;

            handler?.Invoke(this, new MacroProfileEventArgs(profile));
        }
    }
}
