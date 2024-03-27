using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses;

public class GetStashResponse
{
    [JsonPropertyName("stash")] public BaseStashTabMetadata Stash { get; set; }
}

