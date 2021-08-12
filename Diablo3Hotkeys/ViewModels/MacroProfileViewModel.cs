using System.Collections.ObjectModel;
using System.Windows.Input;
using DiabloIIIHotkeys.Commands;

namespace DiabloIIIHotkeys.ViewModels
{
    internal class MacroProfileViewModel : ViewModelBase
    {
        public MacroProfile Profile { get; }

        public string Name => Profile.Name;

        public ObservableCollection<MacroViewModel> Macros { get; } = new ObservableCollection<MacroViewModel>();

        public string Tooltip
        {
            get
            {
                return $"{Macros[0].Interval}/{Macros[1].Interval}/{Macros[2].Interval}/{Macros[3].Interval}";
            }
        }

        private bool _IsSelected;

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                NotifyPropertyChanged(nameof(IsSelected));
            }
        }

        public ICommand ToggleSelectionCommand { get; }

        public MacroProfileViewModel(MacroProfile profile)
        {
            Profile = profile;

            foreach (var macro in profile.Macros)
            {
                Macros.Add(new MacroViewModel(macro));
            }

            ToggleSelectionCommand = new ToggleProfileSelectionCommand();
        }
    }
}
