using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using ChaosRecipeEnhancer.UI.Constants;
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
    public async Task<IEnumerable<string>> GetLeaguesAsync()
    {
        using (var client = new HttpClient())
        {
            Console.WriteLine($"Generated LeagueList Query URL: {SiteEndpoints.LeagueEndpoint}");
            var response = await client.GetAsync(SiteEndpoints.LeagueEndpoint);

            if (response.IsSuccessStatusCode)
            {
                Settings.Default.PoEAccountConnectionStatus = 1;
                Settings.Default.Save();

                var leagueJson = await response.Content.ReadAsStringAsync();
                var leagues = JsonSerializer.Deserialize<League[]>(leagueJson);

                Console.WriteLine("Successfully Retreived LeagueList data.");
                // Console.WriteLine(leagues);
                
                return leagues.Select(x => x.Name);
            }

            if (!response.IsSuccessStatusCode)
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
