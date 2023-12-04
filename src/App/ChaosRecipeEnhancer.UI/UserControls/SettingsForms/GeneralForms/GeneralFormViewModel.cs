using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal class GeneralFormViewModel : ViewModelBase
{
    private readonly IApiService _apiService = Ioc.Default.GetService<IApiService>();
    private const int FetchCooldown = 10;

    private bool _fetchButtonEnabled = true;
    private bool _fetchingStashTabs;
    private bool _initialized;

    public GeneralFormViewModel()
    {
        Settings.PropertyChanged += OnSettingsChanged;
    }

    public ObservableCollection<BaseStashTabMetadata> StashTabIndexNameFullList { get; set; } = new();
    public ObservableCollection<BaseStashTabMetadata> SelectedStashTabs { get; set; } = new();
    public ObservableCollection<string> LeagueList { get; } = new();

    public bool FetchingStashTabs
    {
        get => _fetchingStashTabs;
        set => SetProperty(ref _fetchingStashTabs, value);
    }

    public bool FetchButtonEnabled
    {
        get => _fetchButtonEnabled;
        set => SetProperty(ref _fetchButtonEnabled, value);
    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.LeagueName) && _initialized)
        {
            SelectedStashTabs.Clear();
            StashTabIndexNameFullList.Clear();
        }
    }

    public void UpdateSelectedTabList(IList selectedStashTabProps)
    {
        // Handle collection changed event
        if (selectedStashTabProps is not null)
        {
            var selectedItems = new List<string>();

            foreach (var tab in (ObservableCollection<BaseStashTabMetadata>)selectedStashTabProps)
            {
                selectedItems!.Add(tab.Index.ToString());
            }

            Settings.StashTabIndices = string.Join(",", selectedItems!);
        }

        Settings.Save();
    }

    public async void LoadLeagueList()
    {
        FetchButtonEnabled = false;

        var leagues = await _apiService.GetLeaguesAsync();
        await UpdateLeagueList(leagues);
    }

    private async Task UpdateLeagueList(IEnumerable<BaseLeagueMetadata> leagueList)
    {
        // clearing observable (ui) collection for leagues
        LeagueList.Clear();

        // adding new items to observable (ui) collection for leagues
        if (leagueList is not null)
        {
            foreach (var league in leagueList)
            {
                LeagueList.Add(league.Name);
            }
        }

        // re-setting app setting for league name; if it was empty, set to first item in list
        if (string.IsNullOrEmpty(Settings.LeagueName))
        {
            Settings.LeagueName = LeagueList.FirstOrDefault();
        }

        _initialized = true;

        // enforce cooldown on fetch button to reduce chances of rate limiting
        try
        {
            await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
        }
        finally
        {
            FetchButtonEnabled = true;
        }
    }

    public async Task LoadStashTabNamesIndicesAsync()
    {
        var stashTabPropsList = await _apiService.GetAllPersonalStashTabMetadataAsync();
        if (stashTabPropsList is not null) UpdateStashTabNameIndexFullList(stashTabPropsList.StashTabs);
    }

    private void UpdateStashTabNameIndexFullList(List<BaseStashTabMetadata> stashTabProps)
    {
        // clearing observable (ui) collection for stash tabs
        StashTabIndexNameFullList.Clear();

        // adding new items to observable (ui) collection for stash tabs
        foreach (var tab in stashTabProps)
        {
            if (tab.Type == "Folder")
            {
                // adding folder tabs' children
                foreach (var nestedTab in tab.Children)
                {
                    StashTabIndexNameFullList.Add(nestedTab);
                }
            }
            // implicitly ignore "Folder" tabs
            else
            {
                StashTabIndexNameFullList.Add(tab);
            }
        }

        // re-setting app setting for stash tab indices
        if (Settings.StashTabIndices is not null)
        {
            // adding (pre) selected tabs to list
            var selectedStashTabs = Settings.StashTabIndices.Split(',').ToList();

            // re-selecting tabs from previous session on the ui side
            foreach (var tab in StashTabIndexNameFullList)
            {
                if (selectedStashTabs.Contains(tab.Index.ToString()))
                {
                    SelectedStashTabs.Add(tab);
                }
            }
        }
    }
}