using System.Threading.Tasks;

namespace ChaosRecipeEnhancer.UI.Model.Storage
{
    public interface IFilterStorage
    {
        Task<string> ReadLootFilterAsync();
        Task WriteLootFilterAsync(string filter);
    }
}