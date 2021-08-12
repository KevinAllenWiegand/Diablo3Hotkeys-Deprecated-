using System.Text.Json.Serialization;

namespace DiabloIIIHotkeys.Serialization
{
    public class ProfileSkill
    {
        [JsonPropertyName("interval")]
        public int Interval { get; set; }

        public override string ToString()
        {
            return Interval.ToString();
        }
    }
}
