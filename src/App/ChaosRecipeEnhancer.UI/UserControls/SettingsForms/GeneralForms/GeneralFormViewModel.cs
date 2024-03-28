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

    public ICommand FetchStashTabsCommand => _fetchStashTabsCommand ??= new AsyncRelayCommand(ForceRefreshStashTabsAsync);
    public ICommand RefreshLeaguesCommand => _refreshLeaguesCommand ??= new AsyncRelayCommand(LoadLeagueListAsync);
    public ICommand SelectLogFileCommand => _selectLogFileCommand ??= new RelayCommand(SelectLogFile);
    public ObservableCollection<string> LeagueList { get; } = [];

    // selection by index has its own set of properties
    // i realized it's a lot harder to combine the two selection modes into one set of properties
    public ObservableCollection<BaseStashTabMetadata> StashTabFullListForSelectionByIndex { get; set; } = [];
    public ObservableCollection<BaseStashTabMetadata> SelectedStashTabsByIndex { get; set; } = [];

    // selection by ID has its own set of properties
    public ObservableCollection<BaseStashTabMetadata> StashTabFullListForSelectionById { get; set; } = [];
    public ObservableCollection<BaseStashTabMetadata> SelectedStashTabsById { get; set; } = [];

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
        set => _ = UpdateStashTabQueryModeAsync(value);
    }

    public HashSet<string> StashTabIndices
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

    public HashSet<string> StashTabIds
    {
        get => _userSettings.StashTabIds;
        set
        {
            if (_userSettings.StashTabIds != value)
            {
                _userSettings.StashTabIds = value;
                OnPropertyChanged(nameof(StashTabIds));
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

    public ClientLogFileLocationMode ClientLogFileLocationMode
    {
        get => (ClientLogFileLocationMode)_userSettings.PathOfExileClientLogLocationMode;
        set => UpdateClientLogFileLocationMode(value);
    }

    #endregion

    #endregion

    /// <summary>
    /// Forces a refresh of the stash tabs asynchronously. Used by our button command.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task ForceRefreshStashTabsAsync()
    {
        _stashTabsLoaded = false;
        await LoadStashTabsAsync();
    }

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
    /// Determines if the user needs to fetch data from the API.
    /// <br /><br />
    /// The main purpose of this is so that we don't spam the API with requests when the user to fetch data,
    /// which includes instances of data already fetched and stored in the application.
    /// This can also include instances where the user is not yet authenticated to make API calls.
    /// </summary>
    /// <returns>True if the user needs to fetch some data, false otherwise.</returns>
    public bool NeedsToFetchData()
    {
        // TODO: should this instead be delegated through the auth state manager?
        // if the user is NOT connected to the PoE account, we can't fetch any data to begin with
        if (_userSettings.PoEAccountConnectionStatus != ConnectionStatusTypes.ValidatedConnection)
        {
            return false;
        }

        // we have 2 data dependencies: leagues and stash tabs
        // we will separate the logical checks for both, starting with leagues.

        // if the leagues are not loaded or the list is empty (first time loading)
        if (!_leaguesLoaded || LeagueList.Count == 0)
        {
            return true;
        }

        if (
            // if the query mode is by index and its associated list is empty
            (_userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsByIndex && StashTabFullListForSelectionByIndex.Count == 0) ||
            // if the query mode is by ID and its associated list is empty
            (_userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsById && StashTabFullListForSelectionById.Count == 0)
        )
        {
            if (!_stashTabsLoaded)
            {
                return true;
            }
            // if the stash tabs are already loaded for the current
            // query mode, we don't need to fetch them again
            else
            {
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

            // re-fetching stash tabs if league is changed
            _stashTabsLoaded = false;

            await LoadStashTabsAsync();

            OnPropertyChanged(nameof(LeagueName));
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

    #region Stash Tab Selection Methods

    /// <summary>
    /// Loads the stash tabs from the API asynchronously.
    /// Ensures stash tabs are only loaded once unless explicitly refreshed.
    /// </summary>
    /// <returns></returns>
    public async Task LoadStashTabsAsync()
    {
        Log.Information("Loading stash tabs for {LeagueName}...", _userSettings.LeagueName);

        // If the stash tabs are already loaded, wait for the cooldown and re-enable the button
        if (_stashTabsLoaded)
        {
            // TODO: maybe create a sort of "CooldownEnabledButton" control?
            // Cooldown for button to prevent spamming the API
            await Task.Delay(FetchCooldown * 1000);

            // Re-enable the button after the cooldown
            FetchStashTabsButtonEnabled = true;

            return;
        }

        // Disable the button to prevent multiple requests while waiting for the API response
        FetchStashTabsButtonEnabled = false;

        // Fetch the stash tabs - this is the biggest call in this component
        var stashTabPropsList = await _apiService.GetAllPersonalStashTabMetadataAsync();

        // If the response is valid and we have stash tabs
        if (stashTabPropsList != null && stashTabPropsList.StashTabs != null)
        {
            // update the full list of stash tabs
            UpdateStashTabListForSelection(stashTabPropsList.StashTabs);
        }

        // Indicate that the tabs have been and re-enable the fetch button
        _stashTabsLoaded = true;
        FetchStashTabsButtonEnabled = true;
    }

    /// <summary>
    /// Updates the stash tab query mode property.
    /// </summary>
    /// <param name="stashTabQueryMode">The new query mode.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task UpdateStashTabQueryModeAsync(int stashTabQueryMode)
    {
        if (_userSettings.StashTabQueryMode != stashTabQueryMode)
        {
            _userSettings.StashTabQueryMode = stashTabQueryMode;

            // reset the loaded state to force a reload of the stash tab data
            _stashTabsLoaded = false;

            // Load tabs for the new mode
            await LoadStashTabsAsync();

            // Notify the UI of the change
            OnPropertyChanged(nameof(StashTabQueryMode));
        }
    }

    /// <summary>
    /// Updates the full list of stash tabs for selection in the UI.
    /// </summary>
    /// <param name="stashTabProps">The list of stash tabs to update the UI with.</param>
    private void UpdateStashTabListForSelection(List<BaseStashTabMetadata> stashTabProps)
    {
        // Depending on the query mode, select the appropriate observable list to update
        var StashTabFullListForSelection = _userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsByIndex
            ? StashTabFullListForSelectionByIndex
            : StashTabFullListForSelectionById;

        // Clear the current list based on the query mode
        StashTabFullListForSelection.Clear();

        // Populate the list based on the stash tabs returned from the API
        foreach (var tab in stashTabProps)
        {
            // If the tab is a folder, add all children to the list
            if (tab.Type == "Folder" && tab.Children != null)
            {
                foreach (var nestedTab in tab.Children)
                {
                    StashTabFullListForSelection.Add(nestedTab);
                }
            }
            else
            {
                StashTabFullListForSelection.Add(tab);
            }
        }

        // Pre-select tabs based on the app settings if there are any
        if (StashTabFullListForSelection.Count > 0)
        {
            PreselectTabs();
        }
    }

    /// <summary>
    /// Pre-selects stash tabs based on the user settings.
    /// </summary>
    private void PreselectTabs()
    {
        // Depending on the query mode, select the appropriate observable list to update
        var StashTabFullListForSelection = _userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsByIndex
            ? StashTabFullListForSelectionByIndex
            : StashTabFullListForSelectionById;

        // Depending on the mode, select tabs by either index or ID
        // We'll store the set of selected stash tab in this local variable
        var selectedIdentifiers = _userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsByIndex
            ? _userSettings.StashTabIndices
            : _userSettings.StashTabIds;

        // If there are selected tabs from the settings, pre-select them
        if (selectedIdentifiers.Count > 0)
        {
            // for each tab in the full list, check if it's in the selected list
            foreach (var tab in StashTabFullListForSelection)
            {
                // if we are searching by index, check if the index is in the list
                if (_userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsByIndex)
                {
                    if (selectedIdentifiers.Contains(tab.Index.ToString()))
                    {
                        SelectedStashTabsByIndex.Add(tab);
                    }
                }
                // if we are searching by ID, check if the ID is in the list
                else if (_userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsById)
                {
                    if (selectedIdentifiers.Contains(tab.Id))
                    {
                        SelectedStashTabsById.Add(tab);
                    }
                }
            }
        }

        // if we are searching by index, check if the index is in the list
        if (_userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsByIndex)
        {
            var settingsLength = _userSettings.StashTabIndices.Count;
            var selectedLength = SelectedStashTabsByIndex.Count;

            Log.Information("(Should be the same) Settings Collection Length: {SettingsLength} - Selected Collection Length: {SelectedLength}", settingsLength, selectedLength);

            for (int i = 0; i < settingsLength; i++)
            {
                Log.Information("(Should be the same) Settings Value: {SettingsIndex} - Selected Value: {SelectedIndex}", _userSettings.StashTabIndices.ElementAt(i), SelectedStashTabsByIndex.ElementAt(i).Index);
            }
        }
        // if we are searching by ID, check if the ID is in the list
        else if (_userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsById)
        {
            var settingsLength = _userSettings.StashTabIds.Count;
            var selectedLength = SelectedStashTabsById.Count;

            Log.Information("(Should be the same) Settings Collection Length: {SettingsLength} - Selected Collection Length: {SelectedLength}", settingsLength, selectedLength);

            for (int i = 0; i < settingsLength; i++)
            {
                Log.Information("(Should be the same) Settings Value: {SettingsId} - Selected Value: {SelectedId}", _userSettings.StashTabIds.ElementAt(i), SelectedStashTabsById.ElementAt(i).Id);
            }
        }
    }

    /// <summary>
    /// Updates the user settings based on the selected stash tabs.
    /// </summary>
    /// <param name="selectedStashTabProps">The selected stash tabs.</param>
    public void UpdateUserSettingsForSelectedTabIdentifiers(IList selectedStashTabProps)
    {
        if (selectedStashTabProps == null) return;

        // Temporary collection to accumulate selected identifiers
        var tempSelectedItems = new HashSet<string>();

        foreach (var tab in selectedStashTabProps.Cast<BaseStashTabMetadata>())
        {
            var identifier = _userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsByIndex
                ? tab.Index.ToString()
                : tab.Id;

            tempSelectedItems.Add(identifier);
        }

        // Update the user settings based on the temporary collection
        if (_userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsByIndex)
        {
            // Ensure only to update if there's a change to minimize setter calls
            if (!_userSettings.StashTabIndices.SetEquals(tempSelectedItems))
            {
                _userSettings.StashTabIndices = tempSelectedItems;
            }
        }
        else if (_userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.SelectTabsById)
        {
            // Ensure only to update if there's a change to minimize setter calls
            if (!_userSettings.StashTabIds.SetEquals(tempSelectedItems))
            {
                _userSettings.StashTabIds = tempSelectedItems;
            }
        }
    }

    #endregion
}