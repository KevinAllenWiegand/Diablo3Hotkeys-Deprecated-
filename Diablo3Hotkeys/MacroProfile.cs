using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DiabloIIIHotkeys
{
    internal class MacroProfile : IEquatable<MacroProfile>
    {
        private List<Macro> _Macros;

        public string Name { get; set; }
        
        public ReadOnlyCollection<Macro> Macros
        {
            get { return new ReadOnlyCollection<Macro>(_Macros); }
        }

        public bool IsDefault { get; set; }

        public bool AllowRightMouseButtonDown { get; set; }

        public MacroProfile(string name, List<Macro> macros, bool isDefault, bool allowRightMouseButtonDown)
        {
            Name = name;
            _Macros = new List<Macro>(macros);
            IsDefault = isDefault;
            AllowRightMouseButtonDown = allowRightMouseButtonDown;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ GetMacrosHashcode() ^ IsDefault.GetHashCode();
        }

        private int GetMacrosHashcode()
        {
            var retval = 0;

            foreach (var macro in _Macros)
            {
                retval ^= macro.GetHashCode();
            }

            return retval;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (!(obj is MacroProfile macroProfile))
            {
                return false;
            }

            return Equals(macroProfile);
        }

        public bool Equals(MacroProfile other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name && MacrosAreEqual(other.Macros);
        }

        private bool MacrosAreEqual(ICollection<Macro> macros)
        {
            if (_Macros.Count != macros.Count)
            {
                return false;
            }

            var validated = new List<Macro>();

            foreach (var macro in macros)
            {
                if (!_Macros.Contains(macro))
                {
                    break;
                }

                validated.Add(macro);
            }

            return validated.Count == _Macros.Count;
        }
    }
}
