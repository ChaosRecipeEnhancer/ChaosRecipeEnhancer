using System.Text.Json.Serialization;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses;

public class GetStashResponse
{
    [JsonPropertyName("stash")] public BaseStashTabMetadata Stash { get; set; }
}

