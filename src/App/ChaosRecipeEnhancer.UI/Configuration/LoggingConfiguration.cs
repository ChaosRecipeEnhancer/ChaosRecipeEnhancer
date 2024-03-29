using ChaosRecipeEnhancer.UI.Properties;
using Serilog;

namespace ChaosRecipeEnhancer.UI.Configuration;

public static class LoggingConfiguration
{
    public static void ConfigureSerilogLogging()
    {
        var logConfiguration = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Debug(outputTemplate: "[Serilog 📃] - {Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");

        if (Settings.Default.DebugMode)
        {
            logConfiguration.WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Hour,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}");
        }

        Log.Logger = logConfiguration.CreateLogger();
    }
}
