using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.WPF.Api;

internal sealed class League
{
	[JsonPropertyName("id")]
	public string Name
	{
		get; set;
	}
}

internal sealed class LeagueGetter
{
	private const string LeaguesListEndpoint = "https://api.pathofexile.com/leagues?type=main&realm=pc";

	public async Task<IEnumerable<string>> GetLeaguesAsync()
	{
		using (var client = new HttpClient())
		{
			var response = await client.GetAsync(LeaguesListEndpoint);

			if (response.IsSuccessStatusCode)
			{
				var leagueJson = await response.Content.ReadAsStringAsync();
				var leagues = JsonSerializer.Deserialize<League[]>(leagueJson);

				return leagues.Select(x => x.Name);
			}
		}

		return new List<string>();
	}
}
