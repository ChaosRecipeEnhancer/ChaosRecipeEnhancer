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
        set => _ = UpdateLeagueNameAsync(value);
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
        set => UpdateClientLogFileLocationMode(value);
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
        }

        _userSettings.LeagueName = LeagueList.FirstOrDefault();
        try
        {
            if (_leaguesLoaded)
            {
                await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
            }
        }
        finally
        {
            _leaguesLoaded = true;
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
        }
        try
        {
            if (_stashTabsLoaded)
            {
                await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
            }
        }
        finally
        {
            _stashTabsLoaded = true;
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

    private async Task UpdateLeagueNameAsync(string leagueName)
    {
        if (_userSettings.LeagueName != leagueName)
        {
            _userSettings.LeagueName = leagueName;
            OnPropertyChanged(nameof(LeagueName));

            // re-fetching stash tabs if league is changed
            _stashTabsLoaded = false;
            await LoadStashTabNamesIndicesAsync();
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
                    PathOfExileClientLogLocation = PoEClientDefaults.DefaultStandaloneInstallLocationPath;
                    break;
                case ClientLogFileLocationMode.DefaultSteamLocation:
                    PathOfExileClientLogLocation = PoEClientDefaults.DefaultSteamInstallLocationPath;
                    break;
            }
        }
    }
}