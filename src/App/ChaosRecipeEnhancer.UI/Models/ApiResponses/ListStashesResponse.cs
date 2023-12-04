using System.Collections.Generic;
using System.Text.Json.Serialization;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses;

/// <summary>
/// Represents JSON response objects from `/api/stashes/{league}` endpoint.
///
/// The full response object is structured as follows:
///
///     {
///         "stashes": [ ... ],
///     }
///
/// The only thing we're interested in is the `tabs` array. Nothing else.
/// </summary>
public class ListStashesResponse
{
    [JsonPropertyName("stashes")] public List<BaseStashTabMetadata> StashTabs { get; set; }
}