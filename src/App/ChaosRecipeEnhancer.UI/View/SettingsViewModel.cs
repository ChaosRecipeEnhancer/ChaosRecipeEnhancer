using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.View;

internal class SettingsViewModel : ViewModelBase
{
    private bool _fetchingStashTabs;
    private bool _initialized;

    public SettingsViewModel(ISelectedStashTabHandler selectedStashTabHandler)
    {
        SelectedStashTabHandler = selectedStashTabHandler;
        Settings.PropertyChanged += OnSettingsChanged;
    }

    public Settings Settings { get; } = Settings.Default;
    public ISelectedStashTabHandler SelectedStashTabHandler { get; }

    public ObservableCollection<string> LeagueList { get; } = new();
    public ObservableCollection<StashTab> StashTabList { get; } = new();

    public bool FetchingStashTabs
    {
        get => _fetchingStashTabs;
        set => SetProperty(ref _fetchingStashTabs, value);
    }

    private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Settings.LeagueName) && _initialized)
        {
            StashTabList.Clear();
            SelectedStashTabHandler.SelectedStashTab = null;
        }
    }

    public void UpdateLeagueList(IEnumerable<string> leagueList)
    {
        var selectedLeague = Settings.LeagueName;
        LeagueList.Clear();
        foreach (var league in leagueList) LeagueList.Add(league);

        if (string.IsNullOrEmpty(selectedLeague))
            Settings.LeagueName = LeagueList.FirstOrDefault();
        else
            Settings.LeagueName = selectedLeague;

        _initialized = true;
    }
}