using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses.SessionIdEndpointResponses;

/// <summary>
/// Represents JSON response objects from `get-stash-items` and `get-guild-stash-items` endpoints.
/// <br /><br />
/// The full response object is structured as follows:
/// <br /><br />
///     {
///         "numTabs" : number,
///         "tabs": [ ... ],
///         "items": [ ... ],
///         "quadLayout": bool
///     }
/// <br /><br />
/// The only thing we're interested in is the `tabs` array. Nothing else.
/// </summary>
public class BaseStashTabMetadataList
{
    [JsonPropertyName("tabs")] public List<BaseStashTabMetadata> StashTabs { get; set; }
}