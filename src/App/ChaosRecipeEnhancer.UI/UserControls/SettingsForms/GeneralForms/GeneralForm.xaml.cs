using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Api;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Windows;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal partial class GeneralForm
{
    private readonly LeagueGetter _leagueGetter = new();
    private readonly GeneralFormViewModel _model;
    private readonly StashTabGetter _stashTabGetter = new();

    public GeneralForm(SettingsWindow parent)
    {
        DataContext = _model = new GeneralFormViewModel(parent);
        InitializeComponent();
        LoadLeagueList();
    }

    private async void OnFormLoaded(object sender, RoutedEventArgs e)
    {
        if (CheckAccountSettings(false))
        {
            if (_model.StashTabIndexNameFullList.Count == 0) await LoadStashTabNamesIndicesAsync();
            if (_model.SelectedStashTabHandler.StashManagerControl is null) await LoadStashTabsAsync();
        }
    }

    private async void OnFetchStashTabsButtonClicked(object sender, RoutedEventArgs e)
    {
        await LoadStashTabNamesIndicesAsync();
    }

    private async Task LoadStashTabNamesIndicesAsync()
    {
        var accName = Settings.Default.PathOfExileAccountName.Trim();
        var league = Settings.Default.LeagueName.Trim();

        var stashTabPropsList = await _stashTabGetter.GetStashPropsAsync(accName, league);

        if (stashTabPropsList is not null) _model.UpdateStashTabNameIndexFullList(stashTabPropsList.tabs);
    }

    private async Task LoadStashTabsAsync()
    {
        _model.FetchingStashTabs = true;
        using var __ = new ScopeGuard(() => _model.FetchingStashTabs = false);

        _model.SelectedStashTabHandler.StashManagerControl = null;
        var stashTabs = await _stashTabGetter.FetchStashTabsAsync();
        if (stashTabs is null)
        {
            _ = MessageBox.Show("Failed to fetch stash tabs", "Request Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnRefreshLeaguesButtonClicked(object sender, RoutedEventArgs e)
    {
        LoadLeagueList();
    }

    private async void LoadLeagueList()
    {
        var leagues = await _leagueGetter.GetLeaguesAsync();
        _model.UpdateLeagueList(leagues);
    }

    private void OnStashTabSelectionChanged(object sender, ItemSelectionChangedEventArgs itemSelectionChangedEventArgs)
    {
        var checkComboBox = (CheckComboBox)sender;
        _model.UpdateSelectedTabList(checkComboBox.SelectedItems);
    }

    private void LogLocationDialog_Click(object sender, RoutedEventArgs e)
    {
        var open = new OpenFileDialog();
        open.Filter = "Text|Client.txt";
        var res = open.ShowDialog();

        if (res != DialogResult.OK) return;

        var filename = open.FileName;

        if (filename.EndsWith("Client.txt"))
        {
            Settings.Default.PathOfExileClientLogLocation = filename;
            LogLocationDialog.Content = filename;
        }
        else
        {
            MessageBox.Show(
                "Invalid file selected. Make sure you're selecting the \"Client.txt\" file located in your main Path of Exile installation folder.",
                "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static bool CheckAccountSettings(bool showError)
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
}