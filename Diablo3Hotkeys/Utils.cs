using System;
using System.IO;
using System.Reflection;

namespace DiabloIIIHotkeys
{
    internal class Utils
    {
        private string _ProfilesFilename;
        public string ProfilesFilename
        {
            get
            {
                if (_ProfilesFilename == null)
                {
                    _ProfilesFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Profiles.json");
                }

                return _ProfilesFilename;
            }
        }

        private string _PreferenesFilename;
        public string PreferencesFilename
        {
            get
            {
                if (_PreferenesFilename == null)
                {
                    _PreferenesFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Preferences.json");
                }

                return _PreferenesFilename;
            }
        }

        private static Lazy<Utils> _Instance = new Lazy<Utils>(() => new Utils());
        public static Utils Instance
        {
            get { return _Instance.Value; }
        }
    }
}
