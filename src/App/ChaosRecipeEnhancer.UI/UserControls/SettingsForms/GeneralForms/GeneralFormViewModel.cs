using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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
    public ObservableCollection<string> LeagueList { get; } = new();
    public ISelectedStashTabHandler SelectedStashTabHandler { get; }
    public Settings Settings { get; } = Settings.Default;

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
        var selectedLeague = Settings.LeagueName;
        LeagueList.Clear();
        foreach (var league in leagueList) LeagueList.Add(league);

        Settings.LeagueName = string.IsNullOrEmpty(selectedLeague)
            ? LeagueList.FirstOrDefault()
            : selectedLeague;
    }

    internal void LoadStashQueryModeVisibility()
    {
        switch (Settings.Default.StashTabQueryMode)
        {
            case 0:
                TabSelectVisible = Visibility.Visible;
                TabIndicesVisible = Visibility.Hidden;
                TabNamePrefixVisible = Visibility.Hidden;
                TabNameSuffixVisible = Visibility.Hidden;
                break;
            case 1:
                TabSelectVisible = Visibility.Hidden;
                TabIndicesVisible = Visibility.Visible;
                TabNamePrefixVisible = Visibility.Hidden;
                TabNameSuffixVisible = Visibility.Hidden;
                break;
            case 2:
                TabSelectVisible = Visibility.Hidden;
                TabIndicesVisible = Visibility.Hidden;
                TabNamePrefixVisible = Visibility.Visible;
                TabNameSuffixVisible = Visibility.Hidden;
                break;
            case 3:
                TabSelectVisible = Visibility.Hidden;
                TabIndicesVisible = Visibility.Hidden;
                TabNamePrefixVisible = Visibility.Hidden;
                TabNameSuffixVisible = Visibility.Visible;
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