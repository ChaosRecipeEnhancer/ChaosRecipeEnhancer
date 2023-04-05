using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.DynamicControls.StashTabs
{
    /// <summary>
    /// Represents JSON response objects from `get-stash-items` and `get-guild-stash-items` endpoints.
    ///
    /// The full response object is structured as follows:
    ///
    ///     {
    ///         "numTabs" : number,
    ///         "tabs": [ ... ],
    ///         "items": [ ... ]
    ///     }
    ///
    /// The only thing we're interested in is the `tabs` array.
    /// </summary>
    public class StashTabResponseObject
    {
        [JsonPropertyName("tabs")] public List<StashTabProperties> StashTabs { get; set; }
    }

    /// <summary>
    /// Represents a JSON response object nested within the previously mentioned response object for `get-stash-items`
    /// and `get-guild-stash-items` endpoints.
    ///
    /// The (nested) tab object structure is as follows:
    ///
    ///     {
    ///         "n": "Expedition (Remove-only)",
    ///         "i": 0,
    ///         "id": "1a487045221d745b934eae3d1e05c7513392a40947c436b3f02766e3d5a519d0",
    ///         "type": "NormalStash",
    ///         "selected": true,
    ///         "colour": { "r": 124, "g": 84, "b": 54 },
    ///         "srcL": "https://web.poecdn.com/gen/image/WzIzLDEseyJ0IjoibCIsImMiOjQyODYzMzgxMDJ9XQ/e4f4722145/Stash_TabL.png",
    ///         "srcC": "https://web.poecdn.com/gen/image/WzIzLDEseyJ0IjoibSIsImMiOjQyODYzMzgxMDJ9XQ/0beaf4cf09/Stash_TabL.png",
    ///         "srcR": "https://web.poecdn.com/gen/image/WzIzLDEseyJ0IjoiciIsImMiOjQyODYzMzgxMDJ9XQ/078f335f09/Stash_TabL.png"
    ///     }
    ///
    /// Notice how there are more fields in the example than in our defined object.
    /// </summary>
    public class StashTabProperties
    {
        [JsonPropertyName("n")] public string Name { get; set; }

        [JsonPropertyName("i")] public int Index { get; set; }

        [JsonPropertyName("type")] public string Type { get; set; }
    }
}