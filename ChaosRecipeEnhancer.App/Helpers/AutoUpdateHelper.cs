using System;
using System.Net;
using System.Windows.Forms;
using AutoUpdaterDotNET;

namespace ChaosRecipeEnhancer.App.Helpers
{
    public static class AutoUpdateHelper
    {
        private static bool _firstTry = true;

        public static void InitializeAutoUpdater(string appVersion)
        {
            AutoUpdater.ShowSkipButton = false;
            AutoUpdater.InstalledVersion = new Version(appVersion);
            AutoUpdater.RunUpdateAsAdmin = true;

            CheckForUpdates();
        }

        public static void CheckForUpdates()
        {
            AutoUpdater.Start(
                "https://raw.githubusercontent.com/ChaosRecipeEnhancer/EnhancePoEApp/master/ChaosRecipeEnhancer.Installer/autoupdate.xml");
        }

        /// <summary>
        ///     TODO [Remove] [Refactor] Do we even need this? Maybe this is something we could refactor and use for better update
        ///     UI?
        /// </summary>
        /// <param name="args"></param>
        private static void AutoUpdaterOnCheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args.Error == null)
            {
                if (args.IsUpdateAvailable)
                {
                    _firstTry = false;
                    DialogResult dialogResult;
                    if (args.Mandatory.Value)
                        dialogResult =
                            MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. This is required update. Press Ok to begin updating the application.",
                                @"Update Available",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                    else
                        dialogResult =
                            MessageBox.Show(
                                $@"There is new version {args.CurrentVersion} available. You are using version {args.InstalledVersion}. Do you want to update the application now?",
                                @"Update Available",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Information);

                    // Uncomment the following line if you want to show standard update dialog instead.
                    // AutoUpdater.ShowUpdateForm(args);

                    if (dialogResult.Equals(DialogResult.Yes) || dialogResult.Equals(DialogResult.OK))
                        try
                        {
                            if (AutoUpdater.DownloadUpdate(args))
                                // TODO: [Move] Feels weird to call UI stuff here, idk
                                Application.Exit();
                        }
                        catch (Exception exception)
                        {
                            MessageBox.Show(exception.Message, exception.GetType().ToString(), MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                }
                else
                {
                    if (!_firstTry)
                        MessageBox.Show(@"There is no update available please try again later.", @"No update available",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _firstTry = false;
                }
            }
            else
            {
                if (args.Error is WebException)
                    MessageBox.Show(
                        @"There is a problem reaching update server. Please check your internet connection and try again later.",
                        @"Update Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(args.Error.Message,
                        args.Error.GetType().ToString(), MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
            }
        }
    }
}