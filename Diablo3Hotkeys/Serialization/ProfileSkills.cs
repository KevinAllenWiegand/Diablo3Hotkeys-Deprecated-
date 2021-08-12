using System.Text.Json.Serialization;

namespace DiabloIIIHotkeys.Serialization
{
    public class ProfileSkills
    {
        [JsonPropertyName("1")]
        public ProfileSkill One { get; set; }

        [JsonPropertyName("2")]
        public ProfileSkill Two { get; set; }

        [JsonPropertyName("3")]
        public ProfileSkill Three { get; set; }

        [JsonPropertyName("4")]
        public ProfileSkill Four { get; set; }
    }
}
