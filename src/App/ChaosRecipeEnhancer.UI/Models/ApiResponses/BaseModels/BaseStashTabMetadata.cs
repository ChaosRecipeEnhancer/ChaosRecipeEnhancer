using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;

/// <summary>
/// Represents a JSON response object nested within the previously mentioned response object for the `/api/stashes/{league}/{index}` endpoints.
///
/// The (nested) tab object structure is as follows:
///
/// {
///     "id": "85d828223b",
///     "name": "Chaos Recipe",
///     "type": "PremiumStash",
///     "index": 0,
///     "metadata": {
///         "colour": "ffd500"
///     }
/// }
///
/// Notice how there are more fields in the example than in our defined object.
/// </summary>
public class BaseStashTabMetadata
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("type")] public string Type { get; set; }

    [JsonPropertyName("index")] public int Index { get; set; }

    [JsonPropertyName("children")] public List<BaseStashTabMetadata> Children { get; set; }

    [JsonPropertyName("items")] public List<BaseItem> Items { get; set; }

    public override string ToString() => $"[Index {Index}] {Name}";
}