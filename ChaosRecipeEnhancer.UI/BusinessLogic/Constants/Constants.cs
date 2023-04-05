using System;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.Constants
{
    public static class StringConstruction
    {
        public static string NewLineCharacter => Environment.NewLine;
        public static string DoubleNewLineCharacter => Environment.NewLine + Environment.NewLine;
        public const string TabCharacter = "\t";
    }

    public static class SoundAssets
    {
        public const string SoundFileExtensionFilter = "MP3|*.mp3";
            
        public const string DefaultFilterChangedSoundFilePath = @"Assets\Sounds\FilterChanged.mp3";
        public const string DefaultItemPickedUpSoundFilePath = @"Assets\Sounds\ItemPickedUp.mp3";
    }

    public static class FilterAssets
    {
        public const string DefaultNormalItemFilterStyleFilePath = @"Assets\FilterStyles\NormalItemsStyle.txt";
        public const string DefaultInfluencedItemFilterStyleFilePath = @"Assets\FilterStyles\InfluencedItemsStyle.txt";
    }

    public static class GameApi
    {
        public const string LeagueEndpoint = "https://api.pathofexile.com/leagues?type=main";

        public static string PersonalStashIndividualTabUri(string accountName, string league, string stashTab) =>
            $"https://www.pathofexile.com/character-window/get-stash-items?accountName={accountName}&realm=pc&league={league}&tabIndex={stashTab}";
        
        public static string GuildStashIndividualTabUri(string accountName, string league, string stashTab) =>
            $"https://www.pathofexile.com/character-window/get-guild-stash-items?accountName={accountName}&realm=pc&league={league}&tabIndex={stashTab}";
    }
}