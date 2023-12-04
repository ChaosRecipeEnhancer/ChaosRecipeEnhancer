using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ChaosRecipeEnhancer.UI;

internal partial class App
{
    private readonly SingleInstance _singleInstance = new("EnhancePoE");

    public App()
    {
        var args = Environment.GetCommandLineArgs();
        var isSecondaryInstance = !_singleInstance.Claim();

        if (isSecondaryInstance)
        {
            if (args.Length > 1 && args[1].StartsWith("chaosrecipe://"))
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
            // Setup for the main instance
            SetupUnhandledExceptionHandling();
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Other Service Registration
        services.AddSingleton<IApiService, ApiService>();
        services.AddSingleton<IReloadFilterService, ReloadFilterService>();
        services.AddSingleton<IItemSetManagerService, ItemSetManagerService>();
        services.AddSingleton<IFilterManipulationService, FilterManipulationService>();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        Trace.WriteLine("Starting app ChaosRecipeEnhancer");

        ValidateTokenOnAppLaunch();

        // Updates application settings to reflect a more recent installation of the application.
        if (Settings.Default.UpgradeSettingsAfterUpdate)
        {
            Settings.Default.Upgrade();
            Settings.Default.UpgradeSettingsAfterUpdate = false;
            Settings.Default.Save();
        }

        // Create the service collection and configure services
        var services = new ServiceCollection();
        ConfigureServices(services);

        // Build the service provider
        IServiceProvider serviceProvider = services.BuildServiceProvider();

        // Configure the MVVM Toolkit to use the DI provider
        Ioc.Default.ConfigureServices(serviceProvider);

        var settingsWindow = new SettingsWindow();
        settingsWindow.Show();

        _singleInstance.PingedByOtherProcess += (sender, _) =>
        {
            HandleAuthRedirection(sender);
            Dispatcher.Invoke(settingsWindow.Show);
        };
    }

    private void SetupUnhandledExceptionHandling()
    {
        // Catch exceptions from all threads in the AppDomain.
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");

        // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
        TaskScheduler.UnobservedTaskException += (_, args) =>
            ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException");

        // Catch exceptions from a single specific UI dispatcher thread.
        Dispatcher.UnhandledException += (_, args) =>
        {
            // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
            if (Debugger.IsAttached) return;

            args.Handled = true;
            ShowUnhandledException(args.Exception, "Dispatcher.UnhandledException");
        };
    }

    private static void ShowUnhandledException(Exception e, string unhandledExceptionType)
    {
        var limitedExceptionMessage = string.Join(
            Environment.NewLine,
            // split the exception message into lines and take the first 30 lines
            e.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None).Take(5)
        );

        var messageBoxTitle = $"Error: Unhandled Exception - {unhandledExceptionType}";
        var messageBoxMessage =
            $"The following exception occurred: {unhandledExceptionType}" +
            $"{limitedExceptionMessage}";

        var dialog = new CustomDialog(
            messageBoxTitle,
            messageBoxMessage
        );

        dialog.ShowDialog();
    }

    private static void ValidateTokenOnAppLaunch()
    {
        if (GlobalAuthState.Instance.ValidateLocalAuthToken())
        {
            Trace.WriteLine("Local auth token is valid");
        }
        else
        {
            Trace.WriteLine("Local auth token is invalid");
            GlobalAuthState.Instance.PurgeLocalAuthToken();
        }
    }

    private static void HandleAuthRedirection(object sender)
    {
        Trace.WriteLine("Pinged by other processes!");

        var data = sender as string; // Assuming sender is the data
        Trace.WriteLine($"Received data: {data}");

        // Process the data
        if (!string.IsNullOrEmpty(data) && data.StartsWith("chaosrecipe://"))
        {
            // we're getting a callback from the OAuth2 flow
            Trace.WriteLine("Local auth token is invalid");

            var uri = new Uri(data);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);

            var authCode = queryParams["code"];
            var state = queryParams["state"];

            Trace.WriteLine("Auth Code: " + authCode);
            Trace.WriteLine("State: " + state);

            _ = GlobalAuthState.Instance.GenerateAuthToken(authCode).Result;
        }

    }
}
