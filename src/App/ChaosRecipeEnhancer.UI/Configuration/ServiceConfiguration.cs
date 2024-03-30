using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using ChaosRecipeEnhancer.UI.UserControls.SettingsForms.AccountForms;
using ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;
using ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OtherForms;
using ChaosRecipeEnhancer.UI.UserControls.SettingsForms.OverlayForms;
using ChaosRecipeEnhancer.UI.Windows;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Serilog;
using System;

namespace ChaosRecipeEnhancer.UI.Configuration;

public static class ServiceConfiguration
{
    public static void ConfigureServices(IServiceCollection services)
    {
        ConfigureCoreServices(services);
        ConfigureHttpClients(services);
        ConfigureViewModels(services);

        // Order matters a lot here - these have to be registered
        // last since they depend on other services

        services.AddSingleton<IAuthStateManager, AuthStateManager>();
        services.AddSingleton<IPoeApiService, PoeApiService>();
    }

    private static void ConfigureCoreServices(IServiceCollection services)
    {
        services.AddSingleton<IUserSettings, UserSettings>();
        services.AddSingleton<IReloadFilterService, ReloadFilterService>();
        services.AddSingleton<IFilterManipulationService, FilterManipulationService>();
        services.AddSingleton<INotificationSoundService, NotificationSoundService>();
    }

    private static void ConfigureHttpClients(IServiceCollection services)
    {
        services.AddHttpClient<IAuthStateManager, AuthStateManager>();
        services.AddHttpClient(ApiEndpoints.PoeApiHttpClientName)
            // Custom retry policy for transient errors, excluding 429 status code
            .AddTransientHttpErrorPolicy(builder =>
                builder
                    .OrResult(response =>
                    {
                        // Log stuff
                        Log.Information(
                            "Request {RequestUri} with status code {StatusCode}",
                            response.RequestMessage.RequestUri,
                            response.StatusCode
                        );

                        // Retry on 5xx status codes only
                        // The reason we don't retry 4XX status
                        // codes is because they are client errors
                        // i.e. the request was malformed or missing
                        // so retrying the same request will not help

                        int statusCode = (int)response.StatusCode;
                        return statusCode >= 500 && statusCode <= 599;
                    })
                .WaitAndRetryAsync(
                    retryCount: 1,
                    sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(3), // retry after 3 seconds
                    onRetry: (response, retryCount, context) =>
                    {
                        // Log the retry attempts
                        Log.Information(
                            "Retrying request {RequestUri} - {StatusCode} - {RetryCount}",
                            response.Result.RequestMessage.RequestUri,
                            response.Result.StatusCode,
                            retryCount
                        );
                    }
                )
            );
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        services.AddTransient<GeneralFormViewModel>();
        services.AddTransient<SetTrackerOverlayFormViewModel>();
        services.AddTransient<StashTabOverlayViewModel>();
        services.AddTransient<PathOfExileAccountOAuthFormViewModel>();
        services.AddTransient<RecipesFormViewModel>();
        services.AddTransient<AdvancedFormViewModel>();
        services.AddTransient<SystemFormViewModel>();
    }
}
