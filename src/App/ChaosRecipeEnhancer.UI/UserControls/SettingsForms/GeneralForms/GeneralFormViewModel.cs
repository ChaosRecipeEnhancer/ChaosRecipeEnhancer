using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal class GeneralFormViewModel : ViewModelBase
{
    private bool _fetchingStashTabs;

    public GeneralFormViewModel(ISelectedStashTabHandler selectedStashTabHandler)
    {
        Settings.PropertyChanged += OnSettingsChanged;
        // SelectedStashTabHandler = selectedStashTabHandler;
    }

    // public ObservableCollection<StashTab> StashTabList { get; } = new();
    public ObservableCollection<string> StashTabNameIndexList { get; } = new();

    public ObservableCollection<string> LeagueList { get; } = new();
    // public ISelectedStashTabHandler SelectedStashTabHandler { get; }

    public bool FetchingStashTabs
    {
        get => _fetchingStashTabs;
        set => SetProperty(ref _fetchingStashTabs, value);
    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.LeagueName))
        {
            // StashTabList.Clear();
            // SelectedStashTabHandler.SelectedStashTab = null;
        }
    }

    public void UpdateLeagueList(IEnumerable<string> leagueList)
    {
        // 'backing up' app setting for league name
        var selectedLeague = Settings.LeagueName;

        // clearing observable (ui) collection for leagues
        LeagueList.Clear();

        // adding new items to observable (ui) collection for leagues
        if (leagueList is not null)
            foreach (var league in leagueList)
                LeagueList.Add(league);

        // re-setting app setting for league name; if it was empty, set to first item in list
        Settings.LeagueName = string.IsNullOrEmpty(selectedLeague)
            ? LeagueList.FirstOrDefault()
            : selectedLeague;
    }

    public void UpdateStashTabNameIndexList(List<StashTabProps> stashTabProps)
    {
        // backing up app setting for selected stash tabs
        // var selectedTabs = Settings.SelectedStashTabs;

        // clearing observable (ui) collection for stash tabs
        StashTabNameIndexList.Clear();

        // adding new items to observable (ui) collection for stash tabs
        foreach (var tab in stashTabProps) StashTabNameIndexList.Add($"[Index: {tab.i}] {tab.n}");
    }
}