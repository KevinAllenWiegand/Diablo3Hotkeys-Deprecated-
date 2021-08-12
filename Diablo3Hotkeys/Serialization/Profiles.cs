using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DiabloIIIHotkeys.Serialization
{
    public class Profiles
    {
        [JsonPropertyName("profiles")]
        public IEnumerable<Profile> Items { get; set; }
    }
}
