using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.State;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using Timer = System.Timers.Timer;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

public class GeneralFormViewModel : CreViewModelBase
{
    #region Fields

    private readonly ILogger _log = Log.ForContext<GeneralFormViewModel>();
    private readonly IPoEApiService _apiService;
    private readonly IAuthStateManager _authStateManager;
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

    // Timer for debouncing
    private Timer _debounceTimer;
    private const double DebounceTime = 500; // Time in milliseconds
    private int _debounceAttempts = 0; // Track the number of debounce attempts

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneralFormViewModel"/> class.
    /// </summary>
    /// <param name="apiService">The service for API interactions.</param>
    /// <param name="authStateManager">Manages authentication state.</param>
    /// <param name="userSettings">Stores user settings.</param>
    public GeneralFormViewModel(IPoEApiService apiSevice, IAuthStateManager authStateManager, IUserSettings userSettings)
    {
        _apiService = apiSevice;
        _authStateManager = authStateManager;
        _userSettings = userSettings;

        // Initialize the debounce timer
        _debounceTimer = new Timer(DebounceTime);
        _debounceTimer.Elapsed += OnDebounceTimerElapsed;
        _debounceTimer.AutoReset = false; // Ensure the timer runs only once per interval
    }

    ~GeneralFormViewModel()
    {
        _debounceTimer?.Stop();
        _debounceTimer?.Dispose();
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

                // Increment debounce attempts
                _debounceAttempts++;

                // Calculate the delay with exponential backoff
                double delay = DebounceTime * Math.Pow(2, _debounceAttempts - 1);

                // Ensure the delay does not exceed a maximum value
                const double maxDelay = 5000; // Maximum delay in milliseconds
                delay = Math.Min(delay, maxDelay);

                // Reset and start the debounce timer with the new delay
                _debounceTimer.Stop();
                _debounceTimer.Interval = delay;
                _debounceTimer.Start();
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
                        PathOfExileClientLogLocation = PoEClient.DefaultStandaloneInstallLocationPath;
                        break;
                    case ClientLogFileLocationMode.DefaultSteamLocation:
                        PathOfExileClientLogLocation = PoEClient.DefaultSteamInstallLocationPath;
                        break;
                }
            }
        }
    }

    #endregion

    #region Events

    private void OnDebounceTimerElapsed(object sender, ElapsedEventArgs e)
    {
        Application.Current.Dispatcher.Invoke(async () =>
        {
            // Reset debounce attempts after successful API call delay
            _debounceAttempts = 0;

            // Existing method content remains unchanged
            // Ensure UI updates are performed on the main thread
            _stashTabsLoaded = false;

            // Call the method to load stash tabs for the new league
            await LoadStashTabNamesIndicesAsync();
        });
    }

    #endregion

    /// <summary>
    /// Loads the league list asynchronously. Ensures leagues are only loaded once unless explicitly refreshed.
    /// </summary>
    public async Task LoadLeagueListAsync()
    {
        if (_leaguesLoaded)
        {
            try
            {
                await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
            }
            finally
            {
                RefreshLeagueListButtonEnabled = true;
            }

            return;
        }

        RefreshLeagueListButtonEnabled = false;

        LeagueList.Clear();

        var leagueList = await _apiService.GetLeaguesAsync();
        if (leagueList != null)
        {
            foreach (var league in leagueList.Leagues)
            {
                LeagueList.Add(league.Id);
            }

            _leaguesLoaded = true;
        }

        _userSettings.LeagueName = LeagueList.FirstOrDefault();

        try
        {
            await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
        }
        finally
        {
            RefreshLeagueListButtonEnabled = true;
        }
    }

    /// <summary>
    /// Loads stash tab names and indices. Only loads once per session to reduce API calls, unless the league is changed or explicitly refreshed.
    /// </summary>
    public async Task LoadStashTabNamesIndicesAsync()
    {
        if (_stashTabsLoaded)
        {
            try
            {
                await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
            }
            finally
            {
                FetchStashTabsButtonEnabled = true;
            }

            return;
        }

        FetchStashTabsButtonEnabled = false;

        var stashTabPropsList = await _apiService.GetAllPersonalStashTabMetadataAsync();
        if (stashTabPropsList != null)
        {
            UpdateStashTabNameIndexFullList(stashTabPropsList.StashTabs);
            _stashTabsLoaded = true;
        }

        try
        {
            await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
        }
        finally
        {
            FetchStashTabsButtonEnabled = true;
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