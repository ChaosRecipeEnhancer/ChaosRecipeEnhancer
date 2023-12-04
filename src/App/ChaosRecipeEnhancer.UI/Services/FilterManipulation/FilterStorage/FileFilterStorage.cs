using System.IO;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Windows;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterStorage;

internal class FileFilterStorage : IFilterStorage
{
    private readonly string _fileLocation;

    public FileFilterStorage(string fileLocation)
    {
        _fileLocation = fileLocation;
    }

    public async Task<string> ReadLootFilterAsync()
    {
        if (_fileLocation == "") return null;

        try
        {
            using (var reader = new StreamReader(_fileLocation))
            {
                return await reader.ReadToEndAsync();
            }
        }
        catch (FileNotFoundException)
        {
            ErrorWindow.Spawn(
                "Your selected filter file seems to have been moved, renamed, or deleted. Please re-select your loot filter for manipulation.",
                "Error: Reload Filter - File Not Found"
            );

            Settings.Default.LootFilterFileLocation = "";
            Settings.Default.Save();

            return null;
        }
    }

    public async Task WriteLootFilterAsync(string filter)
    {
        if (_fileLocation != "" && filter != "")
        {
            await using var writer = new StreamWriter(_fileLocation);
            await writer.WriteAsync(filter);
        }
    }
}