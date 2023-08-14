using System;
using ChaosRecipeEnhancer.UI.Models.Enums;

namespace ChaosRecipeEnhancer.UI.Constants;

public static class ApiEndpoints
{
    public static readonly Uri LeagueEndpoint = new("https://api.pathofexile.com/leagues?type=main&realm=pc");
    
    public static Uri IndividualTabContentsEndpoint(TargetStash targetStash, string accountName, string leagueName, int stashTabIndex) => targetStash == TargetStash.Personal
            ? PersonalStashIndividualTabEndpoint(accountName, leagueName, stashTabIndex)
            : GuildStashIndividualTabEndpoint(accountName, leagueName, stashTabIndex);

    private static Uri PersonalStashIndividualTabEndpoint(string accountName, string leagueName, int stashTabIndex) =>
        new($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accountName}&realm=pc&league={leagueName}&tabIndex={stashTabIndex}");
    
    private static Uri GuildStashIndividualTabEndpoint(string accountName, string leagueName, int stashTabIndex) =>
        new($"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={accountName}&realm=pc&league={leagueName}&tabIndex={stashTabIndex}");
    
    public static Uri StashTabPropsEndpoint(TargetStash targetStash, string accountName, string leagueName) => targetStash == TargetStash.Personal
        ? PersonalStashTabPropsEndpoint(accountName, leagueName)
        : GuildStashTabPropsEndpoint(accountName, leagueName);
    
    private static Uri PersonalStashTabPropsEndpoint(string accountName, string leagueName) =>
        new($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accountName}&league={leagueName}&tabs=1&tabIndex=");
    
    private static Uri GuildStashTabPropsEndpoint(string accountName, string leagueName) =>
        new($"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={accountName}&league={leagueName}&tabs=1&tabIndex=");

}