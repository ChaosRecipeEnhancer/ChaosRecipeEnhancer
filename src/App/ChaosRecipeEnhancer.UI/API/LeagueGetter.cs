using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Api;

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
                Settings.Default.PoEAccountConnectionStatus = 1;
                Settings.Default.Save();

                var leagueJson = await response.Content.ReadAsStringAsync();
                var leagues = JsonSerializer.Deserialize<League[]>(leagueJson);

                return leagues.Select(x => x.Name);
            }
            else if (!response.IsSuccessStatusCode)
            {
                Settings.Default.PoEAccountConnectionStatus = 2;
                Settings.Default.Save();

                _ = MessageBox.Show(response.StatusCode == HttpStatusCode.Forbidden
                        ? "Connection forbidden. Please check your Account Name and Session ID."
                        : response.ReasonPhrase,
                    "Error fetching data", MessageBoxButton.OK, MessageBoxImage.Error);

                return null;

            }
        }

        return new List<string>();
    }
}
