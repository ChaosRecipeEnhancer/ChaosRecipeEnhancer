using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.Models;

/// <summary>
/// Represents JSON response objects from `get-stash-items` and `get-guild-stash-items` endpoints.
///
/// The full response object is structured as follows:
///
///     {
///         "numTabs" : number,
///         "tabs": [ ... ],
///         "items": [ ... ],
///         "quadLayout": bool
///     }
///
/// The only thing we're interested in is the `tabs` array. Nothing else.
/// </summary>
public class BaseStashTabMetadataList
{
    [JsonPropertyName("tabs")] public List<BaseStashTabMetadata> StashTabs { get; set; }
}