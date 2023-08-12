using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Api;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Properties;
using MessageBox = System.Windows.MessageBox;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal partial class GeneralForm
{
    private readonly LeagueGetter _leagueGetter = new();
    private readonly GeneralFormViewModel _model;
    private readonly StashTabGetter _stashTabGetter = new();

    public GeneralForm()
    {
        var itemSetManager = new ItemSetManager();
        DataContext = _model = new GeneralFormViewModel(itemSetManager);
        InitializeComponent();
        LoadLeagueList();
    }

    private async void OnFormLoaded(object sender, RoutedEventArgs e)
    {
        if (CheckAccountSettings(false))
            await LoadStashTabNamesIndicesAsync();
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

        if (stashTabPropsList is not null) _model.UpdateStashTabNameIndexList(stashTabPropsList.tabs);
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