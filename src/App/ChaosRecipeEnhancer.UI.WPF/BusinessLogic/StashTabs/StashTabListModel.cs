using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.WPF.BusinessLogic.StashTabs;

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
public class StashTabListModel
{
	[JsonPropertyName("tabs")]
	public List<StashTabModel> StashTabs
	{
		get; set;
	}
}