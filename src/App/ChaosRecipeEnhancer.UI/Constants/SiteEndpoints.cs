using System;

namespace ChaosRecipeEnhancer.UI.Constants;

public static class SiteEndpoints
{
    public const string LeagueEndpoint = "https://api.pathofexile.com/leagues?type=main&realm=pc";

    public static Uri StashTabUrl(int targetStash, string accountName, string league, int stashTabIndex) => targetStash == 0
            ? PersonalStashIndividualTabUri(accountName, league, stashTabIndex)
            : GuildStashIndividualTabUri(accountName, league, stashTabIndex);

    private static Uri PersonalStashIndividualTabUri(string accountName, string league, int stashTabIndex) =>
        new($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accountName}&realm=pc&league={league}&tabIndex={stashTabIndex}");

    private static Uri GuildStashIndividualTabUri(string accountName, string league, int stashTabIndex) =>
        new($"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={accountName}&realm=pc&league={league}&tabIndex={stashTabIndex}");
}