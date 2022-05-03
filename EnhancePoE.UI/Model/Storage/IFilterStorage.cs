using System.Threading.Tasks;

namespace EnhancePoE.UI.Model.Storage
{
    public interface IFilterStorage
    {
        Task<string> ReadLootFilterAsync();
        Task WriteLootFilterAsync(string filter);
    }
}