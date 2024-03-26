using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using ChaosRecipeEnhancer.UI.Models.Constants;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Serilog;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

public class GeneralFormViewModel : CreViewModelBase
{
    #region Fields

    private readonly ILogger _log = Log.ForContext<GeneralFormViewModel>();
    private readonly IPoEApiService _apiService;
    private readonly IUserSettings _userSettings;

    private ICommand _fetchStashTabsCommand;
    private ICommand _refreshLeaguesCommand;
    private ICommand _selectLogFileCommand;

    private const int FetchCooldown = 5; // cooldown in seconds
    private bool _refreshLeagueListButtonEnabled = true;
    private bool _fetchStashTabButtonEnabled = true;

    // Indicates whether data has been loaded to prevent unnecessary API calls
    private bool _leaguesLoaded = false;
    private bool _stashTabsLoaded = false;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralFormViewModel"/> class.
    /// </summary>
    /// <param name="apiService">The service for API interactions.</param>
    /// <param name="authStateManager">Manages authentication state.</param>
    /// <param name="userSettings">Stores user settings.</param>
    public GeneralFormViewModel(IPoEApiService apiSevice, IUserSettings userSettings)
    {
        _apiService = apiSevice;
        _userSettings = userSettings;
    }

    #endregion

    #region Properties

    public ICommand FetchStashTabsCommand => _fetchStashTabsCommand ??= new AsyncRelayCommand(LoadStashTabNamesIndicesAsync);
    public ICommand RefreshLeaguesCommand => _refreshLeaguesCommand ??= new AsyncRelayCommand(LoadLeagueListAsync);
    public ICommand SelectLogFileCommand => _selectLogFileCommand ??= new RelayCommand(SelectLogFile);
    public ObservableCollection<BaseStashTabMetadata> StashTabIndexNameFullList { get; set; } = [];
    public ObservableCollection<BaseStashTabMetadata> SelectedStashTabs { get; set; } = [];
    public ObservableCollection<string> LeagueList { get; } = [];

    public bool RefreshLeagueListButtonEnabled
    {
        get => _refreshLeagueListButtonEnabled;
        set => SetProperty(ref _refreshLeagueListButtonEnabled, value);
    }

    public bool FetchStashTabsButtonEnabled
    {
        get => _fetchStashTabButtonEnabled;
        set => SetProperty(ref _fetchStashTabButtonEnabled, value);
    }

    #region User Settings Properties

    public string LeagueName
    {
        get => _userSettings.LeagueName;
        set
        {
            if (_userSettings.LeagueName != value)
            {
                _userSettings.LeagueName = value;
                OnPropertyChanged(nameof(LeagueName));
            }
        }
    }

    public int StashTabQueryMode
    {
        get => _userSettings.StashTabQueryMode;
        set
        {
            if (_userSettings.StashTabQueryMode != value)
            {
                _userSettings.StashTabQueryMode = value;
                OnPropertyChanged(nameof(StashTabQueryMode));
            }
        }
    }

    public string StashTabIndices
    {
        get => _userSettings.StashTabIndices;
        set
        {
            if (_userSettings.StashTabIndices != value)
            {
                _userSettings.StashTabIndices = value;
                OnPropertyChanged(nameof(StashTabIndices));
            }
        }
    }

    public string StashTabPrefix
    {
        get => _userSettings.StashTabPrefix;
        set
        {
            if (_userSettings.StashTabPrefix != value)
            {
                _userSettings.StashTabPrefix = value;
                OnPropertyChanged(nameof(StashTabPrefix));
            }
        }
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
            _log.Information("Setting Path of Exile client log location to: {Path}", value);
            if (_userSettings.PathOfExileClientLogLocation != value)
            {
                _userSettings.PathOfExileClientLogLocation = value;
                OnPropertyChanged(nameof(PathOfExileClientLogLocation));
            }
        }
    }

    #endregion

    public ClientLogFileLocationMode ClientLogFileLocationMode
    {
        get => (ClientLogFileLocationMode)_userSettings.PathOfExileClientLogLocationMode;
        set
        {
            _log.Information("Setting client log file location mode to: {Mode}", value);
            if (_userSettings.PathOfExileClientLogLocationMode != (int)value)
            {
                _userSettings.PathOfExileClientLogLocationMode = (int)value;
                OnPropertyChanged(nameof(ClientLogFileLocationMode));

                _userSettings.PathOfExileClientLogLocationMode = (int)value;

                switch (value)
                {
                    case ClientLogFileLocationMode.DefaultStandaloneLocation:
                        PathOfExileClientLogLocation = PoEClientDefaults.DefaultStandaloneInstallLocationPath;
                        break;
                    case ClientLogFileLocationMode.DefaultSteamLocation:
                        PathOfExileClientLogLocation = PoEClientDefaults.DefaultSteamInstallLocationPath;
                        break;
                }
            }
        }
    }

    #endregion

    /// <summary>
    /// Loads the league list asynchronously. Ensures leagues are only loaded once unless explicitly refreshed.
    /// </summary>
    public async Task LoadLeagueListAsync()
    {
        _log.Information("Starting LoadLeagueListAsync");

        if (_leaguesLoaded)
        {
            _log.Information("Leagues already loaded. Entering cooldown.");
            try
            {
                await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
            }
            finally
            {
                RefreshLeagueListButtonEnabled = true;
                _log.Information("Cooldown complete. RefreshLeagueListButtonEnabled set to true.");
            }

            return;
        }

        _log.Information("Loading league list for the first time.");
        RefreshLeagueListButtonEnabled = false;

        LeagueList.Clear();

        _log.Information("Calling API to fetch league list.");
        var leagueList = await _apiService.GetLeaguesAsync();
        if (leagueList != null)
        {
            _log.Information("League list fetched successfully. Total leagues: {LeagueCount}", leagueList.Leagues.Count);

            foreach (var league in leagueList.Leagues)
            {
                LeagueList.Add(league.Id);
            }
        }
        else
        {
            _log.Warning("Failed to fetch league list from API.");
        }

        _userSettings.LeagueName = LeagueList.FirstOrDefault();
        _log.Information("Default league name set to: {LeagueName}", _userSettings.LeagueName);

        try
        {
            if (_leaguesLoaded)
            {
                _log.Information("Entering cooldown after league list load.");
                await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
            }
        }
        finally
        {
            _leaguesLoaded = true;
            _log.Information("Leagues loaded and marked as loaded.");

            RefreshLeagueListButtonEnabled = true;
            _log.Information("Cooldown complete. RefreshLeagueListButtonEnabled set to true.");
        }
    }


    /// <summary>
    /// Loads stash tab names and indices. Only loads once per session to reduce API calls, unless the league is changed or explicitly refreshed.
    /// </summary>
    public async Task LoadStashTabNamesIndicesAsync()
    {
        _log.Information("Starting LoadStashTabNamesIndicesAsync");

        if (_stashTabsLoaded)
        {
            _log.Information("Stash tabs already loaded. Entering cooldown.");
            try
            {
                await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
            }
            finally
            {
                FetchStashTabsButtonEnabled = true;
                _log.Information("Cooldown complete. FetchStashTabsButtonEnabled set to true.");
            }

            return;
        }

        _log.Information("Loading stash tab names and indices for the first time.");
        FetchStashTabsButtonEnabled = false;

        _log.Information("Calling API to fetch all personal stash tab metadata.");
        var stashTabPropsList = await _apiService.GetAllPersonalStashTabMetadataAsync();
        if (stashTabPropsList != null)
        {
            _log.Information("Stash tab metadata fetched successfully. Total tabs: {TabCount}", stashTabPropsList.StashTabs.Count);

            UpdateStashTabNameIndexFullList(stashTabPropsList.StashTabs);

        }
        else
        {
            _log.Warning("Failed to fetch stash tab metadata from API.");
        }

        try
        {
            if (_stashTabsLoaded)
            {
                _log.Information("Entering cooldown after loading stash tabs.");
                await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
            }
        }
        finally
        {
            _stashTabsLoaded = true;
            _log.Information("Stash tabs loaded and marked as loaded.");

            FetchStashTabsButtonEnabled = true;
            _log.Information("Cooldown complete. FetchStashTabsButtonEnabled set to true.");
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

            _userSettings.StashTabIndices = string.Join(",", selectedItems!);
        }
    }

    public void UpdateStashTabNameIndexFullList(List<BaseStashTabMetadata> stashTabProps)
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
        if (_userSettings.StashTabIndices is not null)
        {
            // adding (pre) selected tabs to list
            var selectedStashTabs = _userSettings.StashTabIndices.Split(',').ToList();

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

    public bool ReadyToFetchData()
    {
        if (
            _userSettings.PoEAccountConnectionStatus == ConnectionStatusTypes.ValidatedConnection &&
            StashTabIndexNameFullList.Count == 0
        )
        {
            if (!_stashTabsLoaded)
            {
                return true;
            }
            else
            {
                // if the stash tabs are already loaded, we don't need to fetch them again
                return false;
            }
        }

        return false;
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
}