using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.App.Storage
{
    public interface IFilterStorage
    {
        Task<string> ReadLootFilterAsync();
        Task WriteLootFilterAsync(string filter);
    }
}