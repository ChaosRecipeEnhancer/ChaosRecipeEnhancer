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
        if (string.IsNullOrEmpty(_fileLocation) || string.IsNullOrEmpty(filter))
        {
            return;
        }

        int maxRetries = 3;
        int delayOnRetry = 3000; // milliseconds
        FileMode fileMode = FileMode.Create; // Overwrites the file if it exists

        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                // Using FileStream with explicit FileAccess and FileShare mode
                using (var fileStream = new FileStream(_fileLocation, fileMode, FileAccess.Write, FileShare.None))
                using (var writer = new StreamWriter(fileStream))
                {
                    await writer.WriteAsync(filter);
                }
                break; // success!
            }
            catch (IOException e) when (i < maxRetries - 1)
            {
                // Log the exception details for debugging
                // Example: Log("IOException encountered: " + e.Message);

                await Task.Delay(delayOnRetry);
            }
        }
    }

}