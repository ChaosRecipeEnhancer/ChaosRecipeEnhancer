using ChaosRecipeEnhancer.UI.Windows;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ChaosRecipeEnhancer.UI.Configuration;

public static class ExceptionHandlingConfiguration
{
    public static void SetupUnhandledExceptionHandling()
    {
        // Catch exceptions from all threads in the AppDomain.
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");

        // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
        TaskScheduler.UnobservedTaskException += (_, args) =>
            ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException");

        // Subscribe to Dispatcher.UnhandledException for the primary UI thread
        Application.Current.Dispatcher.UnhandledException += (_, args) =>
        {
            // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
            if (Debugger.IsAttached) return;

            args.Handled = true;
            ShowUnhandledException(args.Exception, "Dispatcher.UnhandledException");
        };
    }

    private static void ShowUnhandledException(Exception e, string unhandledExceptionType)
    {
        Application.Current.Dispatcher.Invoke(() =>
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
        });
    }
}
