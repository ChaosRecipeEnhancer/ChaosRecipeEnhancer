namespace ChaosRecipeEnhancer.UI.BusinessLogic.Constants;

public static class SiteEndpoints
{
	public const string LeagueEndpoint = "https://api.pathofexile.com/leagues?type=main";

	public static string PersonalStashIndividualTabUri(string accountName, string league, string stashTab) =>
		$"https://www.pathofexile.com/character-window/get-stash-items?accountName={accountName}&realm=pc&league={league}&tabIndex={stashTab}";

	public static string GuildStashIndividualTabUri(string accountName, string league, string stashTab) =>
		$"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={accountName}&realm=pc&league={league}&tabIndex={stashTab}";
}