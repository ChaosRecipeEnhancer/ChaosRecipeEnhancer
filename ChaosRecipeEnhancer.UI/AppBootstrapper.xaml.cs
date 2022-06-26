using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using AutofacSerilogIntegration;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.View;
using Serilog;
using Serilog.Context;
using Serilog.Exceptions;
using Application = System.Windows.Forms.Application;

namespace ChaosRecipeEnhancer.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class AppBootstrapper
    {
        // TODO: make app single instance
        public AppBootstrapper()
        {
            SetupUnhandledExceptionHandling();
        }

        protected void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            const string outputTemplate =
                "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} | Level:{Level} | ThreadId:{ThreadId} | ContextValue:{ContextValue} | SourceContext:{SourceContext}] {Message}{NewLine}{Exception}";
            const string debugOutputTemplate =
                "EnhancePoEApp Logger: [{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} | Level:{Level} | ThreadId:{ThreadId} | ContextValue:{ContextValue} | SourceContext:{SourceContext}] {Message}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.WithExceptionDetails()
                .Enrich.WithThreadId()
                .Enrich.FromLogContext()
                .WriteTo.Debug(outputTemplate: debugOutputTemplate)
                .WriteTo.File(
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty,
                        @"Logs/log_.txt"),
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: outputTemplate
                )
                .CreateLogger();

            Log.Debug("App is starting up");

            var builder = new ContainerBuilder();

            // Registering some of our main view components that depend on each-other
            builder.RegisterType<SettingsWindow>()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<SetTrackerOverlayWindow>()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<StashTabOverlayWindow>();
            builder.RegisterLogger();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var logger = scope.Resolve<ILogger>();

                // Resolving our previously defined dependency from our DI container
                var mainWindow = scope.Resolve<SettingsWindow>();

                // Opens our MainWindow and doesn't return until it has been closed
                logger.Debug("App initialized, starting up MainWindow");
                mainWindow.ShowDialog();

                // Anything after this should just be clean up if needed
            }
        }

        private void SetupUnhandledExceptionHandling()
        {
            // Catch exceptions from all threads in the AppDomain.
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException");

            // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
            TaskScheduler.UnobservedTaskException += (sender, args) =>
                ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException");

            // Catch exceptions from a single specific UI dispatcher thread.
            Dispatcher.UnhandledException += (sender, args) =>
            {
                // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
                if (!Debugger.IsAttached)
                {
                    args.Handled = true;
                    ShowUnhandledException(args.Exception, "Dispatcher.UnhandledException");
                }
            };
        }

        /// <summary>
        /// Showing an unhandled exception in a 'Warning' window.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="unhandledExceptionType"></param>
        /// <seealso href="https://www.codeproject.com/Articles/30216/Handling-Corrupt-user-config-Settings"/>
        private void ShowUnhandledException(Exception e, string unhandledExceptionType)
        {
            Log.Error(e.Demystify(), "Unexpected Error Occured");

            var messageBoxTitle = $"Unexpected Error Occurred: {unhandledExceptionType}";
            var messageBoxMessage = "The following exception occurred:\n\n" +
                                    $"{e.Demystify()}\n\n\n" +
                                    "Please report this issue on GitHub or in our Discord :^)\n\n";
            const MessageBoxButton messageBoxButtons = MessageBoxButton.YesNo;

            messageBoxMessage +=
                "Sometimes, NUKING 💣💥🧨 your settings can help resolve certain issues.\n\n" +
                "Press 'YES' if you want to try deleting your settings to fix your issue, and the app will restart automatically with the default settings loaded.";

            var result = MessageBox.Show(messageBoxMessage, messageBoxTitle, messageBoxButtons);

            switch (result)
            {
                case MessageBoxResult.Yes:

                    // REF: https://stackoverflow.com/a/64767842/10072406
                    var fullFilePath = ConfigurationManager
                        .OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
                    var versionDirectoryFilePath = Path.GetFullPath(Path.Combine(fullFilePath, @"..\..\"));
                    var fileEntries = Directory.GetDirectories(versionDirectoryFilePath);

                    foreach (var fileName in fileEntries)
                    {
                        if (fileName == versionDirectoryFilePath + Assembly.GetEntryAssembly()?.GetName().Version)
                        {
                            using (LogContext.PushProperty("SettingsFileLocation", fileName))
                            {
                                Log.Debug("Found user settings file, proceeding with a delete");
                            }

                            Directory.Delete(fileName, true);
                        }
                    }

                    Settings.Default.Reset();

                    Current.Shutdown();
                    Application.Restart();
                    break;

                case MessageBoxResult.Cancel:
                case MessageBoxResult.None:
                case MessageBoxResult.OK:
                case MessageBoxResult.No:
                default:
                    Current.Shutdown();
                    break;
            }
        }
    }
}