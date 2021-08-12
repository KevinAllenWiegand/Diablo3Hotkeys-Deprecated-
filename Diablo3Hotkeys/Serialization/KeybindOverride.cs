using System.Text.Json.Serialization;

namespace DiabloIIIHotkeys.Serialization
{
    public class KeybindOverride
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("useNumPad")]
        public bool UseNumPad { get; set; }

        public override string ToString()
        {
            var numPad = UseNumPad ? "NumPad" : "";

            return $"{numPad}{Key}";
        }
    }
}
