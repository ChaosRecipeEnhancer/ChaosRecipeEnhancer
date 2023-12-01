using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using ChaosRecipeEnhancer.UI.ServiceClients;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

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
            // ... rest of your setup code
        }
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Api Client Configuration
        services
            .AddHttpClient<IPathOfExileApiServiceClient, PathOfExileApiServiceClient>(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(10);
            })
            .AddPolicyHandler(GetRetryPolicy())
            .AddPolicyHandler(GetRateLimitPolicy())
            .AddHttpMessageHandler(GetUserAgentHandler);

        // Other Service Registration
        services.AddSingleton<IApiService, ApiService>();
        services.AddSingleton<IReloadFilterService, ReloadFilterService>();
        services.AddSingleton<IItemSetManagerService, ItemSetManagerService>();
        services.AddSingleton<IFilterManipulationService, FilterManipulationService>();
    }

    private void OnStartup(object sender, StartupEventArgs e)
    {
        Trace.WriteLine("Starting app ChaosRecipeEnhancer");
        Task.Run(ValidateAndRefreshTokenAsync);

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

    private static async Task ValidateAndRefreshTokenAsync()
    {
        if (GlobalAuthState.Instance.ValidateLocalAuthToken())
        {
            Trace.WriteLine("Local auth token is valid");
            if (GlobalAuthState.Instance.TokenExpiration - DateTime.UtcNow <= TimeSpan.FromHours(3))
            {
                Trace.WriteLine("Local auth token is about to expire; refreshing");
                await GlobalAuthState.Instance.RefreshAuthToken();
            }
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

            // first we'll check if there's a valid auth token saved in local settings
            if (GlobalAuthState.Instance.ValidateLocalAuthToken())
            {
                // if there is, we'll check if it's about to expire
                Trace.WriteLine("Local auth token is valid");

                // refresh the token if it's about to expire
                if (GlobalAuthState.Instance.TokenExpiration - DateTime.UtcNow <= TimeSpan.FromHours(3))
                {
                    Trace.WriteLine("Local auth token is about to expire; refreshing");
                    var unused = GlobalAuthState.Instance.RefreshAuthToken().Result;
                }
            }
            else
            {
                // if there isn't, we'll generate a new one
                Trace.WriteLine("Local auth token is invalid");

                var uri = new Uri(data);
                var queryParams = HttpUtility.ParseQueryString(uri.Query);

                var authCode = queryParams["code"];
                var state = queryParams["state"];

                Trace.WriteLine("Auth Code: " + authCode);
                Trace.WriteLine("State: " + state);

                var unused = GlobalAuthState.Instance.GenerateAuthToken(authCode).Result;
            }
        }
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.TooManyRequests)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRateLimitPolicy()
    {
        // Define the maximum number of requests and time period for the rate limit
        int maxRequests = 5;
        TimeSpan timePeriod = TimeSpan.FromSeconds(1);

        // Create and return the rate limit policy
        return Policy.RateLimitAsync<HttpResponseMessage>(maxRequests, timePeriod);
    }

    private static DelegatingHandler GetUserAgentHandler()
    {
        const string userAgent = "OAuth chaosrecipeenhancer/3.23.0001 (contact: dev@chaos-recipe.com) StrictMode";
        return new UserAgentHandler(userAgent);
    }
}
