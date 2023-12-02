using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.Models;

public class GetStashResponse
{
    [JsonPropertyName("stash")] public BaseStashTabMetadata Stash { get; set; }
}

