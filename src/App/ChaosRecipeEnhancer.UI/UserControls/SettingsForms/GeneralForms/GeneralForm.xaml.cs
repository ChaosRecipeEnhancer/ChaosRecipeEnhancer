using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Api;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Windows;
using Xceed.Wpf.Toolkit;
using Xceed.Wpf.Toolkit.Primitives;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal partial class GeneralForm
{
    private readonly IApiService ApiService;
    private readonly GeneralFormViewModel _model;
    
    public GeneralForm(IApiService apiService)
    {
        ApiService = apiService;
        
        DataContext = _model = new GeneralFormViewModel();
        InitializeComponent();
        LoadLeagueList();
    }

    private async void OnFormLoaded(object sender, RoutedEventArgs e)
    {
        if (CheckAccountSettings(false))
        {
            if (_model.StashTabIndexNameFullList.Count == 0) await LoadStashTabNamesIndicesAsync();
            if (_model.SelectedStashTabHandler.StashManagerControl is null) await LoadStashTabContentAsync();
        }
    }

    private async void OnFetchStashTabsButtonClicked(object sender, RoutedEventArgs e)
    {
        await LoadStashTabNamesIndicesAsync();
    }

    private async Task LoadStashTabNamesIndicesAsync()
    {
        var secret = _model.Settings.PathOfExileWebsiteSessionId;
        var accountName = _model.Settings.PathOfExileAccountName;
        var leagueName = _model.Settings.LeagueName;

        var stashTabPropsList = _model.Settings.TargetStash == (int)TargetStash.Personal
            ? await ApiService.GetAllPersonalStashTabMetadataAsync(accountName, leagueName, secret)
            : await ApiService.GetAllGuildStashTabMetadataAsync(accountName, leagueName, secret);

        if (stashTabPropsList is not null) _model.UpdateStashTabNameIndexFullList(stashTabPropsList.StashTabs);
    }

    private async Task LoadStashTabContentAsync()
    {
        // visual (and programmatic) indication that we are currently fetching
        // i.e. disable future calls until this fetch has concluded
        _model.FetchingStashTabs = true;
        using var __ = new ScopeGuard(() => _model.FetchingStashTabs = false);

        // invalidate stuff for some reason
        _model.SelectedStashTabHandler.StashManagerControl = null;
        
        var secret = _model.Settings.PathOfExileWebsiteSessionId;
        var accountName = _model.Settings.PathOfExileAccountName;
        var leagueName = _model.Settings.LeagueName;
        var selectedTabIndices = _model.Settings.StashTabIndices;

        // why am i making a form responsible for fetching stash data
        // don't like this at all
        // var stashTabContent = _model.Settings.TargetStash == (int)TargetStash.Personal
        //     ? await ApiService.GetPersonalStashTabContentsByIndexAsync(accountName, leagueName, )
        //     : await ApiService.FetchStashTabsAsync();
        
        if (stashTabContent is null)
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