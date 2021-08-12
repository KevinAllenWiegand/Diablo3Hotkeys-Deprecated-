using System.Text.Json.Serialization;

namespace DiabloIIIHotkeys.Serialization
{
    public class Profile
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("default")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("allowRightMouseButtonDown")]
        public bool AllowRightMouseButtonDown { get; set; }

        [JsonPropertyName("skills")]
        public ProfileSkills Skills { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
