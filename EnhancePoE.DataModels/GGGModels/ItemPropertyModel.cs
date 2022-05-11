using System.Text.Json.Serialization;

namespace EnhancePoE.DataModels.GGGModels
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <seealso cref="https://www.pathofexile.com/developer/docs/reference#type-ItemProperty"/>
    public class ItemPropertyModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("displayMode")]
        public int DisplayMode { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }
    }
}
