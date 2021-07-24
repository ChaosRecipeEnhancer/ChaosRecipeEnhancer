using System.Threading.Tasks;

namespace EnhancePoE.Model.Storage
{
    public interface IFilterStorage
    {
        Task<string> ReadLootFilterAsync();
        Task WriteLootFilterAsync(string filter);
    }
}