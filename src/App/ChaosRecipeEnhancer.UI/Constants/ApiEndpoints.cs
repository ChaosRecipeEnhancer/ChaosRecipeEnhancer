using System;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Constants;

public static class ApiEndpoints
{
    private const string PoeApiBaseUrl = "https://api.pathofexile.com";
    public static readonly Uri LeagueEndpoint = new("https://api.pathofexile.com/leagues?type=main&realm=pc");

    public static Uri StashTabPropsEndpoint() =>
        new($"{PoeApiBaseUrl}/stash/{Settings.Default.LeagueName}");

    public static Uri IndividualTabContentsEndpoint(string stashTabId) =>
        new($"{PoeApiBaseUrl}/stash/{Settings.Default.LeagueName}/{stashTabId}");
}