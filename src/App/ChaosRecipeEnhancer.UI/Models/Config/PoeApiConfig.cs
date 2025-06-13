using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using System;
using System.Reflection;

namespace ChaosRecipeEnhancer.UI.Models.Config;

public static class PoeApiConfig
{
    public const string PoeApiHttpClientName = "PoEApiClient";
    public static string UserAgent
        => $"OAuth chaosrecipeenhancer/{Assembly.GetExecutingAssembly().GetName().Version} (contact: dev@chaos-recipe.com)";

    #region Public (no auth required) Endpoints

    public static readonly Uri PublicLeagueEndpoint = new("https://api.pathofexile.com/leagues?type=main&realm=pc");

    #endregion

    #region Session ID Endpoints

    private static string EscapedAccountName => Uri.EscapeDataString(Settings.Default.LegacyAuthAccountName);

    public static Uri HealthCheckSessionIdEndpoint() =>
        new("https://www.pathofexile.com/api/account-avatar?page=1&perPage=1&custom=false");

    public static Uri IndividualTabContentsSessionIdEndpoint(TargetStash targetStash, int stashTabIndex) =>
        targetStash == TargetStash.Personal
            ? PersonalStashIndividualTabSessionIdEndpoint(stashTabIndex)
            : GuildStashIndividualTabSessionIdEndpoint(stashTabIndex);

    private static Uri PersonalStashIndividualTabSessionIdEndpoint(int stashTabIndex) =>
        new($"https://www.pathofexile.com/character-window/get-stash-items?accountName={EscapedAccountName}&realm=pc&league={Settings.Default.LeagueName}&tabIndex={stashTabIndex}");

    private static Uri GuildStashIndividualTabSessionIdEndpoint(int stashTabIndex) =>
        new($"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={EscapedAccountName}&realm=pc&league={Settings.Default.LeagueName}&tabIndex={stashTabIndex}");

    public static Uri StashTabPropsSessionIdEndpoint(TargetStash targetStash) =>
        targetStash == TargetStash.Personal
            ? PersonalStashTabPropsSessionIdEndpoint()
            : GuildStashTabPropsSessionIdEndpoint();

    private static Uri PersonalStashTabPropsSessionIdEndpoint() =>
        new($"https://www.pathofexile.com/character-window/get-stash-items?accountName={EscapedAccountName}&league={Settings.Default.LeagueName}&tabs=1&tabIndex=");

    private static Uri GuildStashTabPropsSessionIdEndpoint() =>
        new($"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={EscapedAccountName}&league={Settings.Default.LeagueName}&tabs=1&tabIndex=");

    #endregion

    #region OAuth 2.0 Endpoints

    private const string PoeOAuthApiBaseUrl = "https://api.pathofexile.com";

    public static Uri PersonalLeaguesOAuthEndpoint()
        => new($"{PoeOAuthApiBaseUrl}/account/leagues");

    public static Uri PersonalStashTabPropsOAuthEndpoint()
        => new($"{PoeOAuthApiBaseUrl}/stash/{Settings.Default.LeagueName}");

    public static Uri PersonalIndividualTabContentsOAuthEndpoint(string stashTabId)
        => new($"{PoeOAuthApiBaseUrl}/stash/{Settings.Default.LeagueName}/{stashTabId}");

    public static Uri GuildStashTabPropsOAuthEndpoint()
        => new($"{PoeOAuthApiBaseUrl}/guild/stash/{Settings.Default.LeagueName}");

    public static Uri GuildIndividualTabContentsOAuthEndpoint(string stashTabId)
        => new($"{PoeOAuthApiBaseUrl}/guild/stash/{Settings.Default.LeagueName}/{stashTabId}");

    #endregion
}