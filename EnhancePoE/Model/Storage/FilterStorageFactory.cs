namespace EnhancePoE.Model.Storage
{
    public static class FilterStorageFactory
    {
        public static IFilterStorage Create(string lootFilterFilePath)
        {
            return new FileFilterStorage(lootFilterFilePath);
        }

        internal static IFilterStorage Create(Properties.Settings settings)
        {
            return Create(settings.LootfilterLocation);
        }
    }
}
