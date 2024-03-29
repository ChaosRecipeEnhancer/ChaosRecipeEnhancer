using ChaosRecipeEnhancer.UI.Properties;
using Serilog;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterStorage;

public interface IFilterStorage
{
    Task<string> ReadLootFilterAsync();
    Task WriteLootFilterAsync(string filter);
}

public class FileFilterStorage : IFilterStorage
{
    private readonly ILogger _log = Log.ForContext<FileFilterStorage>();
    private readonly IFileSystem _fileSystem;
    private readonly string _fileLocation;

    public FileFilterStorage(string fileLocation)
    {
        _fileSystem = new FileSystem();
        _fileLocation = fileLocation;
    }

    public async Task<string> ReadLootFilterAsync()
    {
        if (_fileLocation == "") return null;

        try
        {
            using var reader = new StreamReader(_fileLocation);

            return await reader.ReadToEndAsync();
        }
        catch (FileNotFoundException e)
        {
            _log.Error("IOException encountered: " + e.Message);

            GlobalErrorHandler.Spawn(
                e.ToString(),
                "Error: File Filter Storage - File Not Found",
                preamble: "Your selected filter file seems to have been moved, renamed, or deleted. Please re-select your loot filter for manipulation."
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
                using var fileStream = new FileStream(_fileLocation, fileMode, FileAccess.Write, FileShare.None);
                using var writer = new StreamWriter(fileStream);

                await writer.WriteAsync(filter);

                break;
            }
            catch (UnauthorizedAccessException e)
            {
                _log.Error("UnauthorizedAccessException encountered: " + e.Message);

                GlobalErrorHandler.Spawn(
                    e.ToString(),
                    "Error: File Filter Storage - Unauthorized Access",
                    preamble: "This application does not have the necessary permissions to write to the loot filter file. " +
                    "Please ensure that you have the required write permissions for the file.\n\n" +
                    "It is also possible that the file is currently in use by another application - " +
                    "if the filter is open in a text editor (or something similar), close it and try again."
                );

                break;
            }
            catch (IOException e) when (i < maxRetries - 1)
            {
                _log.Error("IOException encountered: " + e.Message);
                
                Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
                {
                    await Task.Delay(delayOnRetry);
                });
                
            }
        }
    }
}