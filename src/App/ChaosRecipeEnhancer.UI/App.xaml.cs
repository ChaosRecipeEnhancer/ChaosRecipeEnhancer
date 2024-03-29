using ChaosRecipeEnhancer.UI.Common;
using ChaosRecipeEnhancer.UI.Configuration;
using ChaosRecipeEnhancer.UI.Models.Constants;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Windows;

namespace ChaosRecipeEnhancer.UI;

public partial class App
{
    private readonly SingleInstance _singleInstance = new(CreAppConstants.InstanceName);

    public App()
    {
        var args = Environment.GetCommandLineArgs();
        var isSecondaryInstance = !_singleInstance.Claim();

        if (isSecondaryInstance)
        {
            Log.Information("Secondary instance detected");
            Log.Information("Arguments: {args}", args);
            if (args.Length > 1 && args[1].StartsWith(CreAppConstants.ProtocolPrefix))
            {
                // If it's a URI activation, send the URI to the main instance
                // This specific flow is for the OAuth2 callback
                _singleInstance.PingSingleInstance(args[1]);
                Shutdown();
            }
            else
            {
                // If it's a normal duplicate instance, just shut it down
                Shutdown();
            }
        }
        else
        {
            UpgradeConfiguration.UpgradeSettings();
            LoggingConfiguration.ConfigureSerilogLogging();
            ExceptionHandlingConfiguration.SetupUnhandledExceptionHandling();
        }
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        // Create the service collection and configure services
        var services = new ServiceCollection();
        ServiceConfiguration.ConfigureServices(services);
        IServiceProvider serviceProvider = services.BuildServiceProvider();

        Ioc.Default.ConfigureServices(serviceProvider);

        // Retrieve the AuthStateManager instance from the DI container
        var authStateManager = Ioc.Default.GetService<IAuthStateManager>();
        authStateManager?.ValidateAuthToken();

        // Show the main window
        var settingsWindow = new SettingsWindow();
        settingsWindow.Show();

        // Handle the OAuth2 callback
        _singleInstance.PingedByOtherProcess += (sender, e) =>
        {
            // Handle the OAuth2 callback from the main instance
            authStateManager?.HandleAuthRedirection(e.Data);
            // Show the main settings window
            Dispatcher.Invoke(settingsWindow.Show);
        };
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
