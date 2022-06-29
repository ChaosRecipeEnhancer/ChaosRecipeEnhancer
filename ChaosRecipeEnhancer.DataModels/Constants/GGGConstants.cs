using System;

namespace ChaosRecipeEnhancer.DataModels.Constants
{
    public static class GGGConstants
    {
        public static class GameClient
        {
            private static readonly string[] ClientProcessNames = { "PathOfExile", "PathOfExile_x64", "PathOfExileSteam", "PathOfExile_x64Steam", "PathOfExile_x64_KG.exe", "PathOfExile_KG.exe" };
            public static readonly string[] LogFileNames = { "Client.txt", "KakaoClient.txt" };
        }

        public static class ApiQuery
        {
            public const string LeagueList = "https://api.pathofexile.com/leagues?type=main";

            public static Uri InitialTabUri(string accountName, string league)
                => new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accountName}&tabs=1&league={league}");

            public static Uri StashTabUri(string accountName, string stashTabIndex, string league)
                => new Uri($"https://www.pathofexile.com/character-window/get-stash-items?accountName={accountName}&tabIndex={stashTabIndex}&league={league}");
        }

        public static class ApiResponses
        {
            public static readonly string[] ValidStashTabTypes = { "PremiumStash", "QuadStash", "NormalStash" };
        }
    }
}