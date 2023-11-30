using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
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
        string[] args = Environment.GetCommandLineArgs();
        bool isSecondaryInstance = !_singleInstance.Claim();

        if (isSecondaryInstance)
        {
            if (args.Length > 1 && args[1].StartsWith("chaosrecipe://"))
            {
                // If it's a URI activation, send the URI to the main instance
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
            // ... rest of your setup code
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // services-as-services registration
        services.AddSingleton<IApiService, ApiService>();
        services.AddSingleton<IReloadFilterService, ReloadFilterService>();
        services.AddSingleton<IItemSetManagerService, ItemSetManagerService>();
        services.AddSingleton<IFilterManipulationService, FilterManipulationService>();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        Trace.WriteLine("Starting app up!");

        // print out all startup arguments
        foreach (var arg in e.Args)
        {
            Trace.WriteLine("Startup Argument: " + arg);
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

        _singleInstance.PingedByOtherProcess += (sender, e) =>
        {
            Trace.WriteLine("Pinged by other processes!");

            var data = sender as string; // Assuming sender is the data
            Trace.WriteLine($"Received data: {data}");

            // Process the data
            if (!string.IsNullOrEmpty(data) && data.StartsWith("chaosrecipe://"))
            {
                var uri = new Uri(data);
                var queryParams = HttpUtility.ParseQueryString(uri.Query);

                var authCode = queryParams["code"];
                var state = queryParams["state"];

                Trace.WriteLine("Auth Code: " + authCode);
                Trace.WriteLine("State: " + state);

                // TODO: Add your logic to handle the auth code and state
                AuthHelper.RetrieveAuthToken(authCode, state);
            }

            Dispatcher.Invoke(settingsWindow.Show);
        };
    }

    private void SetupUnhandledExceptionHandling()
    {
        // Catch exceptions from all threads in the AppDomain.
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException",
                false);

        // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
        TaskScheduler.UnobservedTaskException += (_, args) =>
            ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException", false);

        // Catch exceptions from a single specific UI dispatcher thread.
        Dispatcher.UnhandledException += (_, args) =>
        {
            // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
            if (Debugger.IsAttached) return;

            args.Handled = true;
            ShowUnhandledException(args.Exception, "Dispatcher.UnhandledException", true);
        };
    }

    private static void ShowUnhandledException(Exception e, string unhandledExceptionType, bool promptUserForShutdown)
    {
        var messageBoxTitle = $"Unexpected Error Occurred: {unhandledExceptionType}";
        var messageBoxMessage = $"The following exception occurred:\n\n{e}";
        var messageBoxButtons = MessageBoxButton.OK;

        if (promptUserForShutdown)
        {
            messageBoxMessage += "\n\n\nPlease report this issue on github or discord :)";
            messageBoxButtons = MessageBoxButton.OK;
        }

        // Let the user decide if the app should die or not (if applicable).
        if (MessageBox.Show(messageBoxMessage, messageBoxTitle, messageBoxButtons) == MessageBoxResult.Yes)
            Current.Shutdown();
    }
}