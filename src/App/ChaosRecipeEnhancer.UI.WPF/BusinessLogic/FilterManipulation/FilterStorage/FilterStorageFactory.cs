using ChaosRecipeEnhancer.UI.WPF.Properties;

namespace ChaosRecipeEnhancer.UI.WPF.BusinessLogic.FilterManipulation.FilterStorage;

public static class FilterStorageFactory
{
	private static IFilterStorage Create(string lootFilterFilePath)
	{
		return new FileFilterStorage(lootFilterFilePath);
	}

	internal static IFilterStorage Create(Settings settings)
	{
		return Create(settings.LootFilterFileLocation);
	}
}