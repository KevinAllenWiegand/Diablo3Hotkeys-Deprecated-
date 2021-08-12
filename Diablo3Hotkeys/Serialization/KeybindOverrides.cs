using System.Text.Json.Serialization;

namespace DiabloIIIHotkeys.Serialization
{
    public class KeybindOverrides
    {
        [JsonPropertyName("1")]
        public KeybindOverride One { get; set; }

        [JsonPropertyName("2")]
        public KeybindOverride Two { get; set; }

        [JsonPropertyName("3")]
        public KeybindOverride Three { get; set; }

        [JsonPropertyName("4")]
        public KeybindOverride Four { get; set; }
    }
}
