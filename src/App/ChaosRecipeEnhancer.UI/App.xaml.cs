using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.View;

namespace ChaosRecipeEnhancer.UI;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
internal partial class App
{
    private readonly SingleInstance _singleInstance = new("EnhancePoE");

    public App()
    {
        if (!_singleInstance.Claim()) Shutdown();

        SetApplicationTheme(Settings.Default.AppTheme);
        SetupUnhandledExceptionHandling();
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

    private void OnStartup(object sender, StartupEventArgs e)
    {
        var settingsView = new SettingsView();
        settingsView.Show();
        _singleInstance.PingedByOtherProcess += (_, _) => Dispatcher.Invoke(settingsView.Show);
    }

    public void SetApplicationTheme(int appTheme)
    {
        var a = Current as App;

        var themePath = appTheme switch
        {
            0 => AppConstants.Themes.DeepDark,
            1 => AppConstants.Themes.DarkGrey,
            2 => AppConstants.Themes.SoftDark,
            3 => AppConstants.Themes.Grey,
            4 => AppConstants.Themes.Light,
            5 => AppConstants.Themes.RedBlack,
            _ => throw new ArgumentOutOfRangeException()
        };

        var dict = new ResourceDictionary() { Source = new Uri(themePath, UriKind.Relative) };

        foreach (var mergeDict in dict.MergedDictionaries)
        {
            a.Resources.MergedDictionaries.Add(mergeDict);
        }

        foreach (var key in dict.Keys)
        {
            a.Resources[key] = dict[key];
        }
    }
}