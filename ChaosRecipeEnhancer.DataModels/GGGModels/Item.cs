using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.DataModels.GGGModels
{
    /// <summary>
    ///     TODO
    /// </summary>
    /// <seealso cref="https://www.pathofexile.com/developer/docs/reference#type-Item" />
    public class Item
    {
        [JsonPropertyName("w")] public int Width { get; set; }

        [JsonPropertyName("h")] public int Height { get; set; }

        [JsonPropertyName("typeLine")] public string TypeLine { get; set; }

        [JsonPropertyName("baseType")] public string BaseType { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("identified")] public bool Identified { get; set; }

        [JsonPropertyName("ilvl")] public int ItemLevel { get; set; }

        [JsonPropertyName("frameType")] public int FrameType { get; set; }

        [JsonPropertyName("x")] public int X { get; set; }

        [JsonPropertyName("y")] public int Y { get; set; }

        [JsonPropertyName("influences")] public ItemInfluenceModel Influences { get; set; }

        [JsonPropertyName("icon")] public string Icon { get; set; }

        [JsonPropertyName("properties")] public List<ItemPropertyModel> Properties { get; set; }
    }
}