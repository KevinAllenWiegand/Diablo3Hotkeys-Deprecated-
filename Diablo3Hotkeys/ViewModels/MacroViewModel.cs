namespace DiabloIIIHotkeys.ViewModels
{
    internal class MacroViewModel : ViewModelBase
    {
        public Macro Macro { get; }

        public Skill Skill => Macro.Skill;
        public int Interval => Macro.Interval;

        public MacroViewModel(Macro macro)
        {
            Macro = macro;
        }
    }
}
