using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal class GeneralFormViewModel : ViewModelBase
{
    private bool _fetchingStashTabs;
    private bool _initialized;

    public GeneralFormViewModel()
    {
        // SelectedStashTabHandler = selectedStashTabHandler;
        Settings.PropertyChanged += OnSettingsChanged;
    }

    public ObservableCollection<BaseStashTabMetadata> StashTabIndexNameFullList { get; set; } = new();
    public ObservableCollection<BaseStashTabMetadata> SelectedStashTabs { get; set; } = new();
    public ObservableCollection<string> LeagueList { get; } = new();
    public ISelectedStashTabHandler SelectedStashTabHandler { get; }

    public bool FetchingStashTabs
    {
        get => _fetchingStashTabs;
        set => SetProperty(ref _fetchingStashTabs, value);
    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.LeagueName) && _initialized)
        {
            SelectedStashTabs.Clear();
            SelectedStashTabHandler.StashManagerControl.StashTabControls = null;
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

        _initialized = true;
    }

    public void UpdateStashTabNameIndexFullList(List<BaseStashTabMetadata> stashTabProps)
    {
        // clearing observable (ui) collection for stash tabs
        StashTabIndexNameFullList.Clear();

        // adding new items to observable (ui) collection for stash tabs
        foreach (var tab in stashTabProps) StashTabIndexNameFullList.Add(tab);

        if (Settings.StashTabIndices is not null)
        {
            var selectedStashTabs = Settings.StashTabIndices.Split(',').ToList();

            foreach (var tab in StashTabIndexNameFullList)
                if (selectedStashTabs.Contains(tab.Index.ToString()))
                    SelectedStashTabs.Add(tab);
        }
    }
}