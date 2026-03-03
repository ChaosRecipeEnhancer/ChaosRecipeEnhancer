using ChaosRecipeEnhancer.UI.Common;
using ChaosRecipeEnhancer.UI.Configuration;
using ChaosRecipeEnhancer.UI.Models.Config;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Windows;
using Velopack;

namespace ChaosRecipeEnhancer.UI;

public partial class App
{
    private readonly SingleInstance _singleInstance = new(CreAppConfig.InstanceName);

    [STAThread]
    private static void Main(string[] args)
    {
        // Velopack MUST run before any WPF initialization.
        // It handles install/uninstall/update hooks internally,
        // and will exit the process if it was invoked by the Velopack updater.
        var exePath = Environment.ProcessPath ?? string.Empty;
        VelopackApp.Build()
            .OnAfterInstallFastCallback((v) => ProtocolRegistration.Register(exePath))
            .OnAfterUpdateFastCallback((v) =>
            {
                ProtocolRegistration.Register(exePath);

                // After a Velopack update, reset the upgrade flag so that the next
                // normal launch triggers Settings.Default.Upgrade(). This is how we
                // carry forward user settings between Velopack versions — .NET's
                // Upgrade() copies the previous version's user.config into the new
                // version's settings folder, but only if this flag is true.
                UI.Properties.Settings.Default.UpgradeSettingsAfterUpdate = true;
                UI.Properties.Settings.Default.Save();
            })
            .OnBeforeUninstallFastCallback((v) => ProtocolRegistration.Unregister())
            .Run();

        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

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

        // Ensure chaosrecipe:// protocol is registered on every launch.
        // This covers existing users who upgraded from the MSI installer
        // or any case where the Velopack install hook didn't run.
        ProtocolRegistration.Register(Environment.ProcessPath ?? string.Empty);

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
