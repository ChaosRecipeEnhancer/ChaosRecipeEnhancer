using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Api;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.View;

/// <summary>
///     Interaction logic for SettingsWindow.xaml
/// </summary>
internal partial class SettingsView
{
    private readonly SetTrackerOverlayView _recipeOverlay;
    private readonly StashTabGetter _stashTabGetter = new();
    private readonly NotifyIcon _trayIcon = new();

    private bool _closingFromTrayIcon;

    public SettingsView()
    {
        DataContext = new SettingsViewModel();

        var itemSetManager = new ItemSetManager();
        _recipeOverlay = new SetTrackerOverlayView(itemSetManager, _stashTabGetter);

        InitializeComponent();
        InitializeTray();
    }

    private void InitializeTray()
    {
        _trayIcon.Icon = Properties.Resources.CREIcon;
        _trayIcon.Visible = true;
        _trayIcon.ContextMenuStrip = new ContextMenuStrip();
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

    public static bool CheckAllSettings(bool showError)
    {
        var missingSettings = new List<string>();
        var errorMessage = "Please add: \n";

        if (string.IsNullOrEmpty(Settings.Default.PathOfExileAccountName)) missingSettings.Add("- Account Name \n");
        if (string.IsNullOrEmpty(Settings.Default.PathOfExileWebsiteSessionId)) missingSettings.Add("- PoE Session ID \n");
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

    private void OnSaveButtonClicked(object sender, RoutedEventArgs e)
    {
        Settings.Default.Save();
    }

    private void OnResetButtonClicked(object sender, RoutedEventArgs e)
    {
        switch (MessageBox.Show("This will reset all of your settings!", "Reset Settings", MessageBoxButton.YesNo))
        {
            case MessageBoxResult.Yes:
                Settings.Default.Reset();
                break;
            case MessageBoxResult.No:
                break;
        }
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