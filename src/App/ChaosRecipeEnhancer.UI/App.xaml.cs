using ChaosRecipeEnhancer.UI.Common;
using ChaosRecipeEnhancer.UI.Configuration;
using ChaosRecipeEnhancer.UI.Models.Config;
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
    private readonly SingleInstance _singleInstance = new(CreAppConfig.InstanceName);

    public App()
    {
        var args = Environment.GetCommandLineArgs();
        var isSecondaryInstance = !_singleInstance.Claim();

        if (isSecondaryInstance)
        {
            if (args.Length > 1 && args[1].StartsWith(CreAppConfig.ProtocolPrefix))
            {
                _singleInstance.PingSingleInstance(args[1]);
                Shutdown();
            }
            else
            {
                Shutdown();
            }
        }
        else
        {
            SettingsConfiguration.UpgradeSettings();
            LoggingConfiguration.ConfigureSerilogLogging();
            ExceptionHandlingConfiguration.SetupUnhandledExceptionHandling();
        }
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var services = new ServiceCollection();
        ServiceConfiguration.ConfigureServices(services);
        IServiceProvider serviceProvider = services.BuildServiceProvider();

        Ioc.Default.ConfigureServices(serviceProvider);

        var authStateManager = Ioc.Default.GetService<IAuthStateManager>();
        authStateManager?.ValidateAuthToken();

        var settingsWindow = new SettingsWindow();
        settingsWindow.Show();

        _singleInstance.PingedByOtherProcess += (sender, e) =>
        {
            authStateManager?.HandleAuthRedirection(e.Data);
            Dispatcher.Invoke(settingsWindow.Show);
        };
    }

    protected override void OnExit(ExitEventArgs e)
    {
        SettingsConfiguration.OnClose();
        Log.CloseAndFlush();
        base.OnExit(e);
    }
}
