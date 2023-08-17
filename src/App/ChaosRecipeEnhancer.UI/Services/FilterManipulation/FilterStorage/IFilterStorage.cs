using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterStorage;

public interface IFilterStorage
{
    Task<string> ReadLootFilterAsync();
    Task WriteLootFilterAsync(string filter);
}