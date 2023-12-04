using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;

public class BaseItemInfluences
{
    [JsonPropertyName("shaper")] public bool Shaper { get; set; } = false;
    [JsonPropertyName("elder")] public bool Elder { get; set; } = false;
    [JsonPropertyName("crusader")] public bool Crusader { get; set; } = false;
    [JsonPropertyName("redeemer")] public bool Redeemer { get; set; } = false;
    [JsonPropertyName("hunter")] public bool Hunter { get; set; } = false;
    [JsonPropertyName("warlord")] public bool Warlord { get; set; } = false;
}