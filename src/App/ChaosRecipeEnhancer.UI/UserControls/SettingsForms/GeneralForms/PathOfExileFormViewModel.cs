using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

public class PathOfExileFormViewModel : ViewModelBase
{
    private Visibility _customLeagueVisible = Visibility.Hidden;

    public PathOfExileFormViewModel()
    {
        Settings.PropertyChanged += OnLeagueTypeSelectionChange;
    }

    public Settings Settings { get; } = Settings.Default;
    public ObservableCollection<string> LeagueList { get; } = new();

    public Visibility CustomLeagueVisible
    {
        get => _customLeagueVisible;
        set => SetProperty(ref _customLeagueVisible, value);
    }

    private void OnLeagueTypeSelectionChange(object sender, PropertyChangedEventArgs e)
    {
        CustomLeagueVisible = !Settings.Default.CustomLeagueEnabled
            ? Visibility.Hidden
            : Visibility.Visible;
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
}