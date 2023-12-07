using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.Utilities.Native;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.Windows;

public partial class SettingsWindow
{
    private readonly SettingsViewModel _model;
    private readonly SetTrackerOverlayWindow _recipeOverlay;
    private readonly NotifyIcon _trayIcon = new();
    private bool _closingFromTrayIcon;

    public SettingsWindow()
    {
        DataContext = _model = new SettingsViewModel();
        _recipeOverlay = new SetTrackerOverlayWindow();
        InitializeComponent();
        InitializeTray();
        InitializeHotkeys();

        // Check for app updates on startup
        CheckForAppUpdate();

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
            MouseHookForStashTabOverlay.Stop();
            KeyboardHookForHotkeys.ShutdownSystemHook();
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

    #region Check For Updates Stuff

    private async Task CheckForAppUpdate()
    {
        try
        {
            var currentVersion = AppInfo.VersionTextNoPrefix;
            var latestVersion = await GetLatestReleaseVersion(AppInfo.GitHubOrgName, AppInfo.GithubRepoName);

            _model.UpdateAvailable = IsUpdateAvailable(currentVersion, latestVersion);

            // Notify user about the update
            Trace.WriteLine($"A new version {latestVersion} is available!");

        }
        catch (Exception ex)
        {
            // Handle exceptions, like network issues
            Trace.WriteLine("Error checking for updates: " + ex.Message);
        }
    }

    private static async Task<string> GetLatestReleaseVersion(string user, string repo)
    {
        using var client = new HttpClient();

        // GitHub API requires a user-agent
        client.DefaultRequestHeaders.Add("User-Agent", "request");

        var url = $"https://api.github.com/repos/{user}/{repo}/releases/latest";
        var response = await client.GetStringAsync(url);

        using var jsonDoc = JsonDocument.Parse(response);

        var root = jsonDoc.RootElement;
        var tagName = root.GetProperty("tag_name").GetString();
        return tagName; // 'tag_name' usually contains the version
    }

    private static bool IsUpdateAvailable(string currentVersion, string latestVersion)
    {
        var currentParts = currentVersion.Split('.');
        var latestParts = latestVersion.Split('.');

        // Compare each part of the version numbers
        for (var i = 0; i < Math.Max(currentParts.Length, latestParts.Length); i++)
        {
            var currentPart = i < currentParts.Length ? int.Parse(currentParts[i]) : 0;
            var latestPart = i < latestParts.Length ? int.Parse(latestParts[i]) : 0;

            if (currentPart < latestPart) return true; // An update is available
            if (currentPart > latestPart) return false; // Current version is newer (unlikely, but possible)
        }

        // Versions are equal, so no update is needed
        return false;
    }

    #endregion

    #region Hotkey Stuff (Move This... Later)

    private void InitializeHotkeys()
    {
        KeyboardHookForHotkeys.SetupSystemHook();

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