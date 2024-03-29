using ChaosRecipeEnhancer.UI.Properties;
using System;
using System.Reflection;

namespace ChaosRecipeEnhancer.UI.Models;

public static class ApiEndpoints
{
    public const string PoeApiHttpClientName = "PoEApiClient";

    private const string PoeApiBaseUrl = "https://api.pathofexile.com";

    public static readonly Uri LeagueEndpoint = new("https://api.pathofexile.com/leagues?type=main&realm=pc");

    public static Uri LeaguesEndpoint()
        => new($"{PoeApiBaseUrl}/account/leagues");

    public static Uri StashTabPropsEndpoint()
        => new($"{PoeApiBaseUrl}/stash/{Settings.Default.LeagueName}");

    public static Uri IndividualTabContentsEndpoint(string stashTabId)
        => new($"{PoeApiBaseUrl}/stash/{Settings.Default.LeagueName}/{stashTabId}");

    public static string UserAgent
        => $"OAuth chaosrecipeenhancer/{Assembly.GetExecutingAssembly().GetName().Version} (contact: dev@chaos-recipe.com)";
}