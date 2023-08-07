using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Api;
using ChaosRecipeEnhancer.UI.Model;
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
    private readonly SettingsViewModel _model;
    private readonly SetTrackerOverlayView _recipeOverlay;
    private readonly StashTabGetter _stashTabGetter = new();
    private readonly NotifyIcon _trayIcon = new();

    private bool _closingFromTrayIcon;

    public SettingsView()
    {
        var itemSetManager = new ItemSetManager();
        _recipeOverlay = new SetTrackerOverlayView(itemSetManager, _stashTabGetter);
        DataContext = _model = new SettingsViewModel(itemSetManager);

        InitializeComponent();

        InitializeTray();
    }

    private async void OnWindowLoaded(object sender, RoutedEventArgs e)
    {
        if (CheckAllSettings(false)) await LoadStashTabsAsync();
    }

    private void InitializeTray()
    {
        _trayIcon.Icon = Properties.Resources.CREIcon;
        _trayIcon.Visible = true;
        _trayIcon.ContextMenuStrip = new ContextMenuStrip();
        _ = _trayIcon.ContextMenuStrip.Items.Add("Close", null, OnTrayItemMenuClicked);
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

    private void OnTrayItemMenuClicked(object Sender, EventArgs e)
    {
        _closingFromTrayIcon = true;
        Close();
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

    private void LootFilterFileDialog_Click(object sender, RoutedEventArgs e)
    {
        var open = new OpenFileDialog();
        open.Filter = "LootFilter|*.filter";
        var res = open.ShowDialog();

        if (res != System.Windows.Forms.DialogResult.OK) return;

        var filename = open.FileName;
        Settings.Default.LootFilterFileLocation = filename;
        LootFilterFileDialog.Content = filename;
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

        if (showError)
            _ = MessageBox.Show(errorMessage, "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);

        return false;
    }

    private async Task LoadStashTabsAsync()
    {
        _model.FetchingStashTabs = true;
        using var __ = new ScopeGuard(() => _model.FetchingStashTabs = false);

        _model.SelectedStashTabHandler.SelectedStashTab = null;
        var stashTabs = await _stashTabGetter.FetchStashTabsAsync();
        if (stashTabs is null)
        {
            _ = MessageBox.Show("Failed to fetch stash tabs", "Request Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        if (stashTabs.Count == 0) return;

        _model.StashTabList.Clear();
        foreach (var tab in stashTabs) _model.StashTabList.Add(tab);

        var selectedStashTabName = Settings.Default.SelectedStashTabName;
        if (!string.IsNullOrEmpty(selectedStashTabName))
        {
            var previouslySelectedStashTab = _model.StashTabList.FirstOrDefault(x => x.TabName == selectedStashTabName);
            if (previouslySelectedStashTab is not null)
                _model.SelectedStashTabHandler.SelectedStashTab = previouslySelectedStashTab;
        }

        _model.SelectedStashTabHandler.SelectedStashTab ??= _model.StashTabList[0];
    }

    private async void OnFetchStashTabsButtonClicked(object sender, RoutedEventArgs e)
    {
        if (CheckAllSettings(true)) await LoadStashTabsAsync();
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