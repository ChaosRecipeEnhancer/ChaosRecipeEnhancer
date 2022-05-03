using System.IO;
using System.Threading.Tasks;

namespace EnhancePoE.UI.Model.Storage
{
    internal class FileFilterStorage : IFilterStorage
    {
        private readonly string _fileLocation;

        public FileFilterStorage(string fileLocation)
        {
            _fileLocation = fileLocation;
        }

        public async Task<string> ReadLootFilterAsync()
        {
            if (_fileLocation != "")
                using (var reader = new StreamReader(_fileLocation))
                {
                    return await reader.ReadToEndAsync();
                }

            return null;
        }

        public async Task WriteLootFilterAsync(string filter)
        {
            if (_fileLocation != "" && filter != "")
                using (var writer = new StreamWriter(_fileLocation))
                {
                    await writer.WriteAsync(filter);
                }
        }
    }
}