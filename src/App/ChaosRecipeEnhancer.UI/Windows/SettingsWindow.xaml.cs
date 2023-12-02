using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.Windows;

internal partial class SettingsWindow
{
    private readonly SetTrackerOverlayWindow _recipeOverlay;
    private readonly NotifyIcon _trayIcon = new();

    private bool _closingFromTrayIcon;

    public SettingsWindow()
    {
        DataContext = new SettingsViewModel();
        _recipeOverlay = new SetTrackerOverlayWindow();
        InitializeComponent();
        InitializeTray();
    }

    private void InitializeTray()
    {
        _trayIcon.Icon = Properties.Resources.CREIcon;
        _trayIcon.Visible = true;
        _trayIcon.ContextMenuStrip = new ContextMenuStrip();

        _ = _trayIcon.ContextMenuStrip.Items.Add("Check for Update", null, OnCheckForUpdatesItemMenuClicked);
        _ = _trayIcon.ContextMenuStrip.Items.Add("Close", null, OnExitTrayItemMenuClicked);

        _trayIcon.DoubleClick += (s, a) =>
        {
            Show();
            _ = Activate();
            Topmost = true;
            Topmost = false;
            ShowInTaskbar = true;
            WindowState = WindowState.Normal;
        };
    }

    private static bool CheckAllSettings(bool showError)
    {
        var missingSettings = new List<string>();
        var errorMessage = "Please add: \n";

        if (string.IsNullOrEmpty(Settings.Default.PathOfExileAccountName)) missingSettings.Add("- Account Name \n");
        if (string.IsNullOrEmpty(Settings.Default.PathOfExileApiAuthToken)) missingSettings.Add("- PoE Auth Token \n");
        if (string.IsNullOrEmpty(Settings.Default.LeagueName)) missingSettings.Add("- League \n");

        if (missingSettings.Count == 0) return true;

        foreach (var setting in missingSettings) errorMessage += setting;

        if (showError) _ = MessageBox.Show(errorMessage, "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);

        return false;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (Settings.Default.CloseToTrayEnabled && !_closingFromTrayIcon)
        {
            e.Cancel = true;
            Hide();
            ShowInTaskbar = false;
            base.OnClosing(e);
        }
        else if (!Settings.Default.CloseToTrayEnabled || _closingFromTrayIcon)
        {
            _trayIcon.Visible = false;
            MouseHook.Stop();
            Settings.Default.Save();
            Application.Current.Shutdown();
        }
    }

    private void OnExitTrayItemMenuClicked(object sender, EventArgs e)
    {
        _closingFromTrayIcon = true;
        Close();
    }

    private void OnCheckForUpdatesItemMenuClicked(object sender, EventArgs e)
    {
        Process.Start(new ProcessStartInfo(AppInfo.GithubReleasesUrl) { UseShellExecute = true });
    }

    private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
    {
        Settings.Default.Save();
    }

    private void OnRunOverlayButtonClicked(object sender, RoutedEventArgs e)
    {
        if (_recipeOverlay.IsOpen)
        {
            _recipeOverlay.Hide();
            RunOverlayButton.Content = "Run Overlay";
        }
        else
        {
            if (CheckAllSettings(true))
            {
                _recipeOverlay.Show();
                RunOverlayButton.Content = "Stop Overlay";
            }
        }
    }
}