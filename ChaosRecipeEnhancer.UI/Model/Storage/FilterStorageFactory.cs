using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Model.Storage
{
    public static class FilterStorageFactory
    {
        public static IFilterStorage Create(
            string lootFilterFilePath)
        {
            return new FileFilterStorage(lootFilterFilePath);
        }

        internal static IFilterStorage Create(Settings settings)
        {
            return Create(settings.LootFilterLocation);
        }
    }
}