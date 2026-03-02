using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Native;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.Windows;

public partial class SettingsWindow
{
    private readonly SettingsViewModel _model;
    private readonly SetTrackerOverlayWindow _recipeOverlay;

    public SettingsWindow()
    {
        InitializeComponent();
        DataContext = _model = Ioc.Default.GetService<SettingsViewModel>();

        _recipeOverlay = new SetTrackerOverlayWindow();

        InitializeTray();
        InitializeHotkeys();

        // Check for app updates on startup via Velopack
        _ = _model.CheckForUpdateAsync();

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

    protected override void OnClosing(CancelEventArgs e)
    {
        if (_model.CloseToTrayEnabled && !_closingFromTrayIcon)
        {
            e.Cancel = true;
            Hide();
            ShowInTaskbar = false;
            base.OnClosing(e);
        }
        else if (!_model.CloseToTrayEnabled || _closingFromTrayIcon)
        {
            _trayIcon.Visible = false;
            MouseHookForStashTabOverlay.Stop();
            KeyboardHookForGlobalHotkeys.ShutdownSystemHook();
            Settings.Default.Save();
            Application.Current.Shutdown();
        }
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

    private static bool CheckAllSettings(bool showError)
    {
        var missingSettings = new List<string>();
        var errorMessage = "Please add: \n";

        if (string.IsNullOrEmpty(Settings.Default.PathOfExileAccountName) && string.IsNullOrEmpty(Settings.Default.LegacyAuthAccountName)) missingSettings.Add("- Account Name \n");
        if (string.IsNullOrEmpty(Settings.Default.PathOfExileApiAuthToken) && string.IsNullOrEmpty(Settings.Default.LegacyAuthSessionId)) missingSettings.Add("- PoE Auth Token \n");
        if (string.IsNullOrEmpty(Settings.Default.LeagueName)) missingSettings.Add("- League \n");

        if (missingSettings.Count == 0) return true;

        foreach (var setting in missingSettings) errorMessage += setting;

        if (showError) _ = MessageBox.Show(errorMessage, "Missing GlobalUserSettings", MessageBoxButton.OK, MessageBoxImage.Error);

        return false;
    }

    #region Icon Tray Stuff (Move This... Later)

    // one of the big 'issues' (not really an issue) is that to support multi-platform
    // we'll have to change some of the way we do this for non-windows platforms
    // so we'll have to revisit this for the (re-write?) for linux / mac clients

    private readonly NotifyIcon _trayIcon = new();
    private bool _closingFromTrayIcon;

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

    private void OnExitTrayItemMenuClicked(object sender, EventArgs e)
    {
        _closingFromTrayIcon = true;
        Close();
    }

    private async void OnCheckForUpdatesItemMenuClicked(object sender, EventArgs e)
    {
        await _model.CheckForUpdateAsync();
    }

    #endregion


    #region Hotkey Stuff (Move This... Later)

    private void InitializeHotkeys()
    {
        KeyboardHookForGlobalHotkeys.SetupSystemHook();

        // sets the hotkeys from the saved values in settings
        // associates hotkeys with their respective actions
        SetAllHotkeysFromSettings();
    }

    private void SetHotkeyForToggleSetTrackerOverlay_Click(object sender, RoutedEventArgs e)
        => SetHotkey_Click(HotkeyTypes.ToggleSetTrackerOverlay);

    private void SetHotkeyForFetchStashData_Click(object sender, RoutedEventArgs e)
        => SetHotkey_Click(HotkeyTypes.FetchStashData);

    private void SetHotkeyForToggleStashTabOverlay_Click(object sender, RoutedEventArgs e)
        => SetHotkey_Click(HotkeyTypes.ToggleStashTabOverlay);

    private void SetHotkeyForReloadItemFilter_Click(object sender, RoutedEventArgs e)
        => SetHotkey_Click(HotkeyTypes.ReloadItemFilter);

    private void SetHotkey_Click(HotkeyTypes hotkeyType)
    {
        var isWindowOpen = Application.Current.Windows.OfType<HotkeyWindow>().Any();

        if (!isWindowOpen)
        {
            var hotkeyDialog = new HotkeyWindow(this, hotkeyType);
            hotkeyDialog.Show();
        }
    }

    public void SetAllHotkeysFromSettings()
    {
        GlobalHotkeyState.SetRefreshHotkeyFromSettings(_recipeOverlay.RunFetchingAsync);
        GlobalHotkeyState.SetToggleHotkeyFromSettings(_recipeOverlay.RunSetTrackerOverlay);
        GlobalHotkeyState.SetStashTabHotkeyFromSettings(_recipeOverlay.RunStashTabOverlay);
        GlobalHotkeyState.SetReloadFilterHotkeyFromSettings(_recipeOverlay.RunReloadFilter);
    }

    public void ResetGlobalHotkeyState()
    {
        GlobalHotkeyState.RemoveFetchStashDataHotkey();
        GlobalHotkeyState.RemoveToggleStashTabOverlayHotkey();
        GlobalHotkeyState.RemoveToggleSetTrackerOverlayHotkey();
        GlobalHotkeyState.RemoveReloadItemFilterHotkey();
    }

    #endregion
}