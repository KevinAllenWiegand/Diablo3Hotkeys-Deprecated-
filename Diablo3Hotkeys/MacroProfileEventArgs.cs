using System;

namespace DiabloIIIHotkeys
{
    internal class MacroProfileEventArgs : EventArgs
    {
        public MacroProfile Profile { get; }

        public MacroProfileEventArgs(MacroProfile profile)
        {
            Profile = profile;
        }
    }
}
