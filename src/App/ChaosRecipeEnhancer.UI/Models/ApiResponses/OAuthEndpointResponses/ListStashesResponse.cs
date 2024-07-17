using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses.OAuthEndpointResponses;

/// <summary>
/// Represents JSON response objects from `/api/stashes/{league}` endpoint.
/// <br /><br />
/// The full response object is structured as follows:
/// <br /><br />
///     {
///         "stashes": [ ... ],
///     }
/// <br /><br />
/// The only thing we're interested in is the `tabs` array. Nothing else.
/// </summary>
public class ListStashesResponse
{
    [JsonPropertyName("stashes")] public List<BaseStashTabMetadata> StashTabs { get; set; }
}