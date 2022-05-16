using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using AutofacSerilogIntegration;
using EnhancePoE.UI.View;
using Serilog;
using Serilog.Exceptions;

namespace EnhancePoE.UI
{
    /// <summary>
    ///     Interaction logic for App.xaml
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
                "EnhancePoEApp Serilogger: [{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} | Level:{Level} | ThreadId:{ThreadId} | ContextValue:{ContextValue} | SourceContext:{SourceContext}] {Message}{NewLine}{Exception}";

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

            builder.RegisterType<MainWindow>()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<ChaosRecipeEnhancer>()
                .SingleInstance()
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

            builder.RegisterType<StashTabOverlayView>();

            builder.RegisterLogger();

            var container = builder.Build();

            using (var scope = container.BeginLifetimeScope())
            {
                var logger = scope.Resolve<ILogger>();

                logger.Debug("Initializing app views");

                var mainWindow = scope.Resolve<MainWindow>();
                var chaosRecipeEnhancer = scope.Resolve<ChaosRecipeEnhancer>();
                var stashTabOverlayView = scope.Resolve<StashTabOverlayView>();

                mainWindow.ShowDialog();
                chaosRecipeEnhancer.ShowDialog();
                stashTabOverlayView.ShowDialog();

                logger.Debug("App views successfully initialized");
            }
        }

        private void SetupUnhandledExceptionHandling()
        {
            // Catch exceptions from all threads in the AppDomain.
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
                ShowUnhandledException(args.ExceptionObject as Exception, "AppDomain.CurrentDomain.UnhandledException",
                    false);

            // Catch exceptions from each AppDomain that uses a task scheduler for async operations.
            TaskScheduler.UnobservedTaskException += (sender, args) =>
                ShowUnhandledException(args.Exception, "TaskScheduler.UnobservedTaskException", false);

            // Catch exceptions from a single specific UI dispatcher thread.
            Dispatcher.UnhandledException += (sender, args) =>
            {
                // If we are debugging, let Visual Studio handle the exception and take us to the code that threw it.
                if (!Debugger.IsAttached)
                {
                    args.Handled = true;
                    ShowUnhandledException(args.Exception, "Dispatcher.UnhandledException", true);
                }
            };
        }

        /// <summary>
        /// Showing an unhandled exception in a 'Warning' window.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="unhandledExceptionType"></param>
        /// <param name="promptUserForShutdown"></param>
        private void ShowUnhandledException(Exception e, string unhandledExceptionType, bool promptUserForShutdown)
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
            {
                Current.Shutdown();
            }
        }
    }
}