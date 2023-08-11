using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

internal class GeneralFormViewModel : ViewModelBase
{
    private Visibility _customLeagueVisible = Visibility.Hidden;
    private bool _fetchingStashTabs;
    private Visibility _fetchOnNewMapEnabled = Visibility.Collapsed;
    private Visibility _tabIndicesVisible = Visibility.Hidden;
    private Visibility _tabNamePrefixVisible = Visibility.Hidden;
    private Visibility _tabNameSuffixVisible = Visibility.Hidden;
    private Visibility _tabSelectVisible = Visibility.Visible;

    public GeneralFormViewModel(ISelectedStashTabHandler selectedStashTabHandler)
    {
        Settings.PropertyChanged += OnSettingsChanged;
        SelectedStashTabHandler = selectedStashTabHandler;
        LoadStashQueryModeVisibility();
        LoadFetchOnNewMapEnabled();
    }

    public ObservableCollection<StashTab> StashTabList { get; } = new();
    public ObservableCollection<string> StashTabNameIndexList { get; } = new();
    public ObservableCollection<string> LeagueList { get; } = new();
    public ISelectedStashTabHandler SelectedStashTabHandler { get; }

    public bool FetchingStashTabs
    {
        get => _fetchingStashTabs;
        set => SetProperty(ref _fetchingStashTabs, value);
    }

    public Visibility CustomLeagueVisible
    {
        get => _customLeagueVisible;
        set => SetProperty(ref _customLeagueVisible, value);
    }

    public Visibility TabSelectVisible
    {
        get => _tabSelectVisible;
        set => SetProperty(ref _tabSelectVisible, value);
    }

    public Visibility TabIndicesVisible
    {
        get => _tabIndicesVisible;
        set => SetProperty(ref _tabIndicesVisible, value);
    }

    public Visibility TabNamePrefixVisible
    {
        get => _tabNamePrefixVisible;
        set => SetProperty(ref _tabNamePrefixVisible, value);
    }

    public Visibility TabNameSuffixVisible
    {
        get => _tabNameSuffixVisible;
        set => SetProperty(ref _tabNameSuffixVisible, value);
    }

    public Visibility FetchOnNewMapEnabled
    {
        get => _fetchOnNewMapEnabled;
        set => SetProperty(ref _fetchOnNewMapEnabled, value);
    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.LeagueName))
        {
            StashTabList.Clear();
            SelectedStashTabHandler.SelectedStashTab = null;
        }
        else if (e.PropertyName == nameof(Settings.CustomLeagueEnabled))
        {
            CustomLeagueVisible = !Settings.CustomLeagueEnabled
                ? Visibility.Hidden
                : Visibility.Visible;
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
        {
            foreach (var league in leagueList) LeagueList.Add(league);
        }

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
        foreach (var tab in stashTabProps) StashTabNameIndexList.Add($"[{tab.i}] {tab.n}");

        // re-setting app setting for selected stash tabs; if it was empty, set to empty collection
        // if (selectedTabs.Count == 0)
        // {
        //     Settings.SelectedStashTabs = new StringCollection();
        // }
        // else
        // {
        //     Settings.SelectedStashTabs = selectedTabs;
        // }
    }

    internal void LoadStashQueryModeVisibility()
    {
        switch (Settings.Default.StashTabQueryMode)
        {
            case 0:
                TabSelectVisible = Visibility.Visible;
                TabNamePrefixVisible = Visibility.Hidden;
                TabNameSuffixVisible = Visibility.Hidden;
                TabIndicesVisible = Visibility.Hidden;
                break;
            case 1:
                TabSelectVisible = Visibility.Hidden;
                TabNamePrefixVisible = Visibility.Visible;
                TabNameSuffixVisible = Visibility.Hidden;
                TabIndicesVisible = Visibility.Hidden;
                break;
            case 2:
                TabSelectVisible = Visibility.Hidden;
                TabNamePrefixVisible = Visibility.Hidden;
                TabNameSuffixVisible = Visibility.Visible;
                TabIndicesVisible = Visibility.Hidden;
                break;
            case 3:
                TabSelectVisible = Visibility.Hidden;
                TabNamePrefixVisible = Visibility.Hidden;
                TabNameSuffixVisible = Visibility.Hidden;
                TabIndicesVisible = Visibility.Visible;
                break;
        }
    }

    internal void LoadFetchOnNewMapEnabled()
    {
        FetchOnNewMapEnabled = Settings.Default.AutoFetchOnRezoneEnabled
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}