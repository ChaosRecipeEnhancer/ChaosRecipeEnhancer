using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Config;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Services;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

class LegacyGeneralFormViewModel : CreViewModelBase
{
    private readonly IPoeApiService _apiService = Ioc.Default.GetService<IPoeApiService>();
    private readonly IUserSettings _userSettings;

    private bool _fetchingStashTabs;
    private bool _initialized;
    private ICommand _selectLogFileCommand;

    public LegacyGeneralFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;

        GlobalUserSettings.PropertyChanged += OnSettingsChanged;
    }

    public ICommand SelectLogFileCommand => _selectLogFileCommand ??= new RelayCommand(SelectLogFile);

    public ObservableCollection<UnifiedStashTabMetadata> StashTabIndexNameFullList { get; set; } = [];
    public ObservableCollection<UnifiedStashTabMetadata> SelectedStashTabs { get; set; } = [];
    public ObservableCollection<string> LeagueList { get; } = [];

    public bool FetchingStashTabs
    {
        get => _fetchingStashTabs;
        set => SetProperty(ref _fetchingStashTabs, value);
    }

    public bool AutoFetchOnRezoneEnabled
    {
        get => _userSettings.AutoFetchOnRezoneEnabled;
        set
        {
            if (_userSettings.AutoFetchOnRezoneEnabled != value)
            {
                _userSettings.AutoFetchOnRezoneEnabled = value;
                OnPropertyChanged(nameof(AutoFetchOnRezoneEnabled));
            }
        }
    }

    public string PathOfExileClientLogLocation
    {
        get => _userSettings.PathOfExileClientLogLocation;
        set
        {
            if (_userSettings.PathOfExileClientLogLocation != value)
            {
                _userSettings.PathOfExileClientLogLocation = value;
                OnPropertyChanged(nameof(PathOfExileClientLogLocation));
            }
        }
    }

    public ClientLogFileLocationMode ClientLogFileLocationMode
    {
        get => (ClientLogFileLocationMode)_userSettings.PathOfExileClientLogLocationMode;
        set => UpdateClientLogFileLocationMode(value);
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

            foreach (var tab in (ObservableCollection<UnifiedStashTabMetadata>)selectedStashTabProps)
            {
                selectedItems!.Add(tab.Index.ToString());
            }

            GlobalUserSettings.StashTabIndices = string.Join(",", selectedItems!);
        }

        GlobalUserSettings.Save();
    }

    public async void LoadLeagueList()
    {
        var leagues = await _apiService.GetLeaguesAsync();
        UpdateLeagueList(leagues);
    }

    private void UpdateLeagueList(IEnumerable<string> leagueList)
    {
        // 'backing up' app setting for league name
        var selectedLeague = GlobalUserSettings.LeagueName;

        // clearing observable (ui) collection for leagues
        LeagueList.Clear();

        // adding new items to observable (ui) collection for leagues
        if (leagueList is not null)
            foreach (var league in leagueList)
                LeagueList.Add(league);

        // re-setting app setting for league name; if it was empty, set to first item in list
        GlobalUserSettings.LeagueName = string.IsNullOrEmpty(selectedLeague)
            ? LeagueList.FirstOrDefault()
            : selectedLeague;

        _initialized = true;
    }

    public async Task LoadStashTabNamesIndicesAsync()
    {
        var secret = GlobalUserSettings.LegacyAuthSessionId;
        var accountName = GlobalUserSettings.LegacyAuthAccountName;
        var leagueName = GlobalUserSettings.LeagueName;

        var stashTabPropsList = !GlobalUserSettings.GuildStashMode
            ? await _apiService.GetAllPersonalStashTabMetadataWithSessionIdAsync()
            : await _apiService.GetAllGuildStashTabMetadataWithSessionIdAsync();

        if (stashTabPropsList is not null) UpdateStashTabNameIndexFullList(stashTabPropsList);
    }

    private void UpdateStashTabNameIndexFullList(List<UnifiedStashTabMetadata> stashTabProps)
    {
        // clearing observable (ui) collection for stash tabs
        StashTabIndexNameFullList.Clear();

        // adding new items to observable (ui) collection for stash tabs
        foreach (var tab in stashTabProps) StashTabIndexNameFullList.Add(tab);

        if (GlobalUserSettings.StashTabIndices is not null)
        {
            var selectedStashTabs = GlobalUserSettings.StashTabIndices.Split(',').ToList();

            foreach (var tab in StashTabIndexNameFullList)
                if (selectedStashTabs.Contains(tab.Index.ToString()))
                    SelectedStashTabs.Add(tab);
        }
    }

    public void SelectLogFile()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt",
            FilterIndex = 1,
            FileName = "Client.txt"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            var filename = openFileDialog.FileName;

            if (filename.EndsWith("Client.txt"))
            {
                PathOfExileClientLogLocation = filename;
            }
            else
            {
                MessageBox.Show(
                    "Invalid file selected. Make sure you're selecting the \"Client.txt\" file located in your main Path of Exile installation folder.",
                    "Missing Settings",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }

    private void UpdateClientLogFileLocationMode(ClientLogFileLocationMode mode)
    {
        if (_userSettings.PathOfExileClientLogLocationMode != (int)mode)
        {
            _userSettings.PathOfExileClientLogLocationMode = (int)mode;
            OnPropertyChanged(nameof(ClientLogFileLocationMode));

            switch (mode)
            {
                case ClientLogFileLocationMode.DefaultStandaloneLocation:
                    PathOfExileClientLogLocation = PoeClientConfigs.DefaultStandaloneInstallLocationPath;
                    break;
                case ClientLogFileLocationMode.DefaultSteamLocation:
                    PathOfExileClientLogLocation = PoeClientConfigs.DefaultSteamInstallLocationPath;
                    break;
            }
        }
    }
}
