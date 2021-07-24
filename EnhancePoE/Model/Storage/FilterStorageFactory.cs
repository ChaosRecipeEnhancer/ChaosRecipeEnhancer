namespace EnhancePoE.Model.Storage
{
    public static class FilterStorageFactory
    {
        public static IFilterStorage Create(
            bool settingsLootFilterOnline,
            string lootFilterFilePath,
            string filterName,
            string accName,
            string sessionId)
        {
            if (settingsLootFilterOnline)
            {
                return new OnlineFilterStorage(filterName, accName, sessionId);
            }
            else
            {
                return new FileFilterStorage(lootFilterFilePath);
            }
        }

        internal static IFilterStorage Create(Properties.Settings settings)
        {
            return Create(
                settings.LootfilterOnline,
                settings.LootfilterLocation,
                settings.LootfilterOnlineName,
                settings.accName,
                settings.SessionId);
        }
    }
}
