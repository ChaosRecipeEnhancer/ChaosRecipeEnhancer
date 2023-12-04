using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.Utilities;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.Windows;

public partial class SettingsWindow
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
        InitializeHotkeys();

        // will force the window to resize to the size of its content.
        // we do this in lieu of the `SizeToContent` property due to an issue
        // with our custom styles leaving some weird black padding on the
        // right and bottom sides of the window.
        Loaded += (sender, e) =>
        {
            // This will force the window to resize to the size of its content.
            SizeToContent = SizeToContent.WidthAndHeight;
        };
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

    #region Hotkey Stuff (Move This... Later)

    private void InitializeHotkeys()
    {
        GlobalHotkeyState.SetupSystemHook();
        GlobalHotkeyState.GetRefreshHotkey();
        GlobalHotkeyState.GetToggleHotkey();
        GlobalHotkeyState.GetStashTabHotkey();
        AddAllHotkeys();
    }

    private void CustomHotkeyToggle_Click(object sender, RoutedEventArgs e)
    {
        var isWindowOpen = false;
        foreach (Window w in Application.Current.Windows)
            if (w is HotkeyWindow)
                isWindowOpen = true;

        if (isWindowOpen) return;
        var hotkeyDialog = new HotkeyWindow(this, "toggle");
        hotkeyDialog.Show();
    }

    private void RefreshHotkey_Click(object sender, RoutedEventArgs e)
    {
        var isWindowOpen = false;
        foreach (Window w in Application.Current.Windows)
            if (w is HotkeyWindow)
                isWindowOpen = true;

        if (isWindowOpen) return;
        var hotkeyDialog = new HotkeyWindow(this, "refresh");
        hotkeyDialog.Show();
    }

    private void StashTabHotkey_Click(object sender, RoutedEventArgs e)
    {
        var isWindowOpen = false;
        foreach (Window w in Application.Current.Windows)
            if (w is HotkeyWindow)
                isWindowOpen = true;

        if (!isWindowOpen)
        {
            var hotkeyDialog = new HotkeyWindow(this, "stashtab");
            hotkeyDialog.Show();
        }
    }

    public void AddAllHotkeys()
    {
        if (Settings.Default.FetchStashHotkey != "< not set >")
            GlobalHotkeyState.AddHotkey(GlobalHotkeyState.refreshModifier, GlobalHotkeyState.refreshKey, _recipeOverlay.RunFetchingAsync);
        if (Settings.Default.ToggleSetTrackerOverlayHotkey != "< not set >")
            GlobalHotkeyState.AddHotkey(GlobalHotkeyState.toggleModifier, GlobalHotkeyState.toggleKey, _recipeOverlay.RunSetTrackerOverlay);
        if (Settings.Default.ToggleStashTabOverlayHotkey != "< not set >")
            GlobalHotkeyState.AddHotkey(GlobalHotkeyState.stashTabModifier, GlobalHotkeyState.stashTabKey, _recipeOverlay.RunStashTabOverlay);
    }

    public void RemoveAllHotkeys()
    {
        GlobalHotkeyState.RemoveRefreshHotkey();
        GlobalHotkeyState.RemoveStashTabHotkey();
        GlobalHotkeyState.RemoveToggleHotkey();
    }

    #endregion
}