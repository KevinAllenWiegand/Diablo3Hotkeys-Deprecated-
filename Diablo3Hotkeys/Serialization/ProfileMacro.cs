using System;
using System.Text.Json.Serialization;

namespace DiabloIIIHotkeys.Serialization
{
    public class ProfileMacro
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("ctrlModifier")]
        public bool CtrlModifier { get; set; }

        [JsonPropertyName("altModifier")]
        public bool AltModifier { get; set; }

        [JsonPropertyName("shiftModifier")]
        public bool ShiftModifier { get; set; }

        public override string ToString()
        {
            var modifiers = String.Empty;

            if (CtrlModifier)
            {
                modifiers += "Ctrl+";
            }

            if (AltModifier)
            {
                modifiers += "Alt+";
            }

            if (ShiftModifier)
            {
                modifiers += "Shift+";
            }

            return $"{modifiers}{Key}";
        }
    }
}
