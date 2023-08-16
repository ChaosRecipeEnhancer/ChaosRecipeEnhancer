using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.FilterManipulation.FilterStorage;

public interface IFilterStorage
{
	Task<string> ReadLootFilterAsync();
	Task WriteLootFilterAsync(string filter);
}