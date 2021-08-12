using System;

namespace DiabloIIIHotkeys.ViewModels
{
    internal interface IMacroProfileHandler
    {
        public MacroProfile SelectedMacroProfile { get; }
        public void ToggleSelection(MacroProfile profile);
        public event EventHandler<MacroProfileEventArgs> SelectedMacroProfileChanged;
    }
}
