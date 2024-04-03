using ChaosRecipeEnhancer.UI.Models.Config;
using ChaosRecipeEnhancer.UI.Properties;
using Serilog;
using System;
using System.IO;

namespace ChaosRecipeEnhancer.UI.Configuration;

public static class LoggingConfiguration
{
    public static void ConfigureSerilogLogging()
    {
        // Getting the path to the Local AppData folder
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var logDirectory = Path.Combine(appDataPath, "ChaosRecipeEnhancer", "Logs");

        // Ensure the directory exists
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        var logFilePath = Path.Combine(logDirectory, "log.txt");

        var logConfiguration = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.WithProperty("CreAppVersion", CreAppConfig.VersionText)
            .WriteTo.Debug(outputTemplate: "[Serilog 📃] - {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");

        if (Settings.Default.DebugMode)
        {
            logConfiguration.WriteTo.File(logFilePath, rollingInterval: RollingInterval.Hour,
                outputTemplate: "[CRE {CreAppVersion}] {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");
        }

        Log.Logger = logConfiguration.CreateLogger();
    }
}
