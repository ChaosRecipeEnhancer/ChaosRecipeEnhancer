using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ChaosRecipeEnhancer.UI.Api;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;
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
        // TODO: Check settings for real before fetching tabs
        // if (CheckAllSettings(showError: false)) 
        await LoadStashTabsAsync();
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
            if (previouslySelectedStashTab is not null) _model.SelectedStashTabHandler.SelectedStashTab = previouslySelectedStashTab;
        }

        _model.SelectedStashTabHandler.SelectedStashTab ??= _model.StashTabList[0];
    }

    private async void OnFetchStashTabsButtonClicked(object sender, RoutedEventArgs e)
    {
        await LoadStashTabsAsync();
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

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _model.LoadStashQueryModeVisibility();
    }

    private void AutoFetchCheckBox_Checked(object sender, RoutedEventArgs e)
    {
        _model.LoadFetchOnNewMapEnabled();
    }

    private void AutoFetchCheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        _model.LoadFetchOnNewMapEnabled();
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
}