using ChaosRecipeEnhancer.UI.Properties;
using System;
using System.Reflection;

namespace ChaosRecipeEnhancer.UI.Models.Config;

public static class PoeApiConfig
{
    public const string PoeApiHttpClientName = "PoEApiClient";

    private const string PoeApiBaseUrl = "https://api.pathofexile.com";

    public static readonly Uri PublicLeagueEndpoint = new("https://api.pathofexile.com/leagues?type=main&realm=pc");

    public static Uri PersonalLeaguesEndpoint()
        => new($"{PoeApiBaseUrl}/account/leagues");

    public static Uri PersonalStashTabPropsEndpoint()
        => new($"{PoeApiBaseUrl}/stash/{Settings.Default.LeagueName}");

    public static Uri PersonalIndividualTabContentsEndpoint(string stashTabId)
        => new($"{PoeApiBaseUrl}/stash/{Settings.Default.LeagueName}/{stashTabId}");

    public static string UserAgent
        => $"OAuth chaosrecipeenhancer/{Assembly.GetExecutingAssembly().GetName().Version} (contact: dev@chaos-recipe.com)";
}