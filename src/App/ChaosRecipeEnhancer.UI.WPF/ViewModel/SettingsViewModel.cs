using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using ChaosRecipeEnhancer.Common.UI;
using ChaosRecipeEnhancer.UI.WPF.Model;

namespace ChaosRecipeEnhancer.UI.WPF.ViewModel;
internal sealed class SettingsViewModel : ViewModelBase
{

	private bool _initialized;

	public SettingsViewModel(ISelectedStashTabHandler selectedStashTabHandler)
	{
		SelectedStashTabHandler = selectedStashTabHandler;
		Settings.PropertyChanged += OnSettingsChanged;
	}

	private void OnSettingsChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Properties.Settings.Default.LeagueName) && _initialized)
		{
			StashTabList.Clear();
			SelectedStashTabHandler.SelectedStashTab = null;
		}
	}

	public void UpdateLeagueList(IEnumerable<string> leagueList)
	{
		var selectedLeague = Properties.Settings.Default.LeagueName;
		LeagueList.Clear();
		foreach (var league in leagueList)
		{
			LeagueList.Add(league);
		}

		if (string.IsNullOrEmpty(selectedLeague))
		{
			Properties.Settings.Default.LeagueName = LeagueList.FirstOrDefault();
		}
		else
		{
			Properties.Settings.Default.LeagueName = selectedLeague;
		}

		_initialized = true;
	}

	public Properties.Settings Settings { get; } = Properties.Settings.Default;
	public ISelectedStashTabHandler SelectedStashTabHandler
	{
		get;
	}

	public ObservableCollection<string> LeagueList { get; } = new();
	public ObservableCollection<StashTab> StashTabList { get; } = new();


	private bool _fetchingStashTabs;
	public bool FetchingStashTabs
	{
		get => _fetchingStashTabs;
		set => SetProperty(ref _fetchingStashTabs, value);
	}
}
