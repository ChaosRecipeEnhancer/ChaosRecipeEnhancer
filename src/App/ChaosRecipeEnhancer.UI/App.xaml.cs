using ChaosRecipeEnhancer.UI.Common;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;
using ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;
using ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;
using ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OverlayForms;
using ChaosRecipeEnhancer.UI.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Serilog;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;

namespace ChaosRecipeEnhancer.UI;

public partial class App
{
    private readonly SingleInstance _singleInstance = new("ChaosRecipeEnhancer");

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
            ConfigureSerilogLogging();
            SetupUnhandledExceptionHandling();
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Order matters here; be mindful of dependencies between services

        // Core Services
        services.AddSingleton<IUserSettings, UserSettings>();
        services.AddSingleton<IReloadFilterService, ReloadFilterService>();
        services.AddSingleton<IFilterManipulationService, FilterManipulationService>();
        services.AddSingleton<INotificationSoundService, NotificationSoundService>();

        // HttpClient Registration
        services.AddHttpClient<IAuthStateManager, AuthStateManager>();
        services.AddHttpClient("PoEApiClient")
            // Standard retry policy for transient errors
            .AddTransientHttpErrorPolicy(builder =>
                builder.WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    onRetry: (exception, retryCount, context) =>
                    {
                        // Log the retry attempts
                        Log.Information(
                            "Retrying request {RequestUri} - {ExceptionMessage} - {RetryCount}",
                            context,
                            exception.Exception.Message,
                            retryCount
                        );
                    }
                )
            );

        // Services dependant on UserSettings
        services.AddSingleton<IAuthStateManager, AuthStateManager>();
        services.AddSingleton<IPoEApiService, PoEApiService>();

        // ViewModel Registration
        services.AddTransient<GeneralFormViewModel>();
        services.AddTransient<SetTrackerOverlayFormViewModel>();
        services.AddTransient<StashTabOverlayViewModel>();
        services.AddTransient<PathOfExileAccountOAuthFormViewModel>();
        services.AddTransient<RecipesFormViewModel>();
        services.AddTransient<AdvancedFormViewModel>();
        services.AddTransient<SystemFormViewModel>();

        // Eventually we will want to hook up more ViewModels here...
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        Log.Information("Starting app ChaosRecipeEnhancer");

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

        ValidateTokenOnAppLaunch();

        var settingsWindow = new SettingsWindow();
        settingsWindow.Show();

        _singleInstance.PingedByOtherProcess += (sender, _) =>
        {
            HandleAuthRedirection(sender);
            Dispatcher.Invoke(settingsWindow.Show);
        };
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Log.CloseAndFlush();
        base.OnExit(e);
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
        var currentCulture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            // Set the current thread's UI culture to English
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var limitedExceptionMessage = string.Join(
                Environment.NewLine,
                // split the exception message into lines and take the first 30 lines
                e.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None).Take(5)
            );

            var messageBoxTitle = $"Error: Unhandled Exception - {unhandledExceptionType}";
            var messageBoxMessage =
                $"The following exception occurred: {unhandledExceptionType}" +
                $"{limitedExceptionMessage}";

            var dialog = new ErrorWindow(
                messageBoxTitle,
                messageBoxMessage
            );
            dialog.ShowDialog();
        }
        finally
        {
            // Restore the original UI culture
            Thread.CurrentThread.CurrentUICulture = currentCulture;
        }
    }

    private static void ValidateTokenOnAppLaunch()
    {
        var authStateManager = Ioc.Default.GetService<IAuthStateManager>();
        if (authStateManager != null)
        {
            try
            {
                if (authStateManager.ValidateAuthToken())
                {
                    Log.Information("Local auth token is valid - not doing anything");
                }
                else
                {
                    Log.Information("Local auth token is invalid - logging out now");
                    authStateManager.Logout();
                }
            }
            catch (Exception ex)
            {
                Log.Information($"Exception in GenerateAuthToken: {ex.Message}");
            }
        }
    }

    private static async void HandleAuthRedirection(object sender)
    {
        Log.Information("Pinged by other processes!");

        var data = sender as string;
        Log.Information($"Received data: {data}");

        if (!string.IsNullOrEmpty(data) && data.StartsWith("chaosrecipe://"))
        {
            var uri = new Uri(data);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);

            var authCode = queryParams["code"];
            var state = queryParams["state"];

            Log.Information("Auth Code: " + authCode);
            Log.Information("State: " + state);

            var authStateManager = Ioc.Default.GetService<IAuthStateManager>();
            if (authStateManager != null)
            {
                try
                {
                    await authStateManager.GenerateAuthToken(authCode);
                }
                catch (Exception ex)
                {
                    Log.Information($"Exception in GenerateAuthToken: {ex.Message}");
                }
            }
        }
    }

    private void ConfigureSerilogLogging()
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
