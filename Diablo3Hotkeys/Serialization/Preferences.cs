using System.Text.Json.Serialization;

namespace DiabloIIIHotkeys.Serialization
{
    public class Preferences
    {
        [JsonPropertyName("alwaysAllowHotkeys")]
        public bool AlwaysAllowHotkeys { get; set; }

        [JsonPropertyName("writeLogToFile")]
        public bool WriteLogToFile { get; set; }

        [JsonPropertyName("toggleProfileMacro")]
        public ProfileMacro ToggleProfileMacro { get; set; }

        [JsonPropertyName("rightButtonDownMacro")]
        public ProfileMacro RightButtonDownMacro { get; set; }

        [JsonPropertyName("skillKeybindOverrides")]
        public KeybindOverrides SkillKeybindOverrides { get; set; }
    }
}
