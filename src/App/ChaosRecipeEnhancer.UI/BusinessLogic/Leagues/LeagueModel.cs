using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.Leagues;

/// <summary>
/// Represents a JSON response object nested within a response object for GGG's `/leagues` endpoint
///
///     {
///         "id": "Standard",
///         "realm": "pc",
///         "url": "https://www.pathofexile.com/ladders/league/Standard",
///          "startAt": "2013-01-23T21:00:00Z",
///          "endAt": null,
///          "description": "The default game mode.",
///          "registerAt": "2019-09-06T19:00:00Z",
///          "delveEvent": true,
///          "rules": []
///     }
/// </summary>
public class LeagueModel
{
	public LeagueModel(string id)
	{
		Id = id;
	}

	[JsonPropertyName("id")]
	public string Id
	{
		get; set;
	}
}
