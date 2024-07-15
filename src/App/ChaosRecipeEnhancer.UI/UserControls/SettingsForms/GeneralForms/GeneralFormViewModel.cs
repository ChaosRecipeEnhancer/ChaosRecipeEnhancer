using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Exceptions;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

public class GeneralFormViewModel : CreViewModelBase
{
    #region Fields

    // cooldown in seconds
    private const int FetchCooldownSeconds = 1;

    private readonly IPoeApiService _apiService;
    private readonly IUserSettings _userSettings;

    private ICommand _stashTabButtonCommand;
    private ICommand _leagueButtonCommand;

    // button states (enabled by default)
    private bool _leagueButtonEnabled = true;
    private bool _stashTabButtonEnabled = true;
    private bool _privateLeagueCheckboxEnabled = true;

    // dropdown states (disabled on first load)
    private bool _leagueDropDownEnabled = false;
    private bool _stashTabDropDownEnabled = false;

    // state flags (assume user has has settings = loaded true)
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
    public GeneralFormViewModel(IPoeApiService apiSevice, IUserSettings userSettings)
    {
        _apiService = apiSevice;
        _userSettings = userSettings;

        InitializeDataFromUserSettings();
    }

    #endregion

    #region Properties

    #region Commands

    public ICommand StashTabButtonCommand => _stashTabButtonCommand ??= new AsyncRelayCommand(ForceRefreshStashTabsAsync);
    public ICommand LeagueButtonCommand => _leagueButtonCommand ??= new AsyncRelayCommand(ForceRefreshLeaguesAsync);

    #endregion

    public ObservableCollection<string> LeagueList { get; set; } = [];
    public ObservableCollection<UnifiedStashTabMetadata> StashTabFullListForSelectionById { get; set; } = [];
    public ObservableCollection<UnifiedStashTabMetadata> SelectedStashTabsById { get; set; } = [];

    #region UI Enabled States

    public bool LeagueDropDownEnabled
    {
        get => _leagueDropDownEnabled;
        set => SetProperty(ref _leagueDropDownEnabled, value);
    }

    public bool LeagueButtonEnabled
    {
        get => _leagueButtonEnabled;
        set => SetProperty(ref _leagueButtonEnabled, value);
    }

    public bool PrivateLeagueCheckboxEnabled
    {
        get => _privateLeagueCheckboxEnabled;
        set => SetProperty(ref _privateLeagueCheckboxEnabled, value);
    }

    public bool StashTabButtonEnabled
    {
        get => _stashTabButtonEnabled;
        set => SetProperty(ref _stashTabButtonEnabled, value);
    }

    public bool StashTabDropDownEnabled
    {
        get => _stashTabDropDownEnabled;
        set => SetProperty(ref _stashTabDropDownEnabled, value);
    }

    #endregion

    #region User Settings Properties

    public string SelectedLeagueName
    {
        get => _userSettings.LeagueName;
        set => _ = UpdateLeagueNameAsync(value);
    }

    public bool CustomLeagueEnabled
    {
        get => _userSettings.CustomLeagueEnabled;
        set => _ = UpdateIsPrivateLeague(value);
    }

    public int StashTabQueryMode
    {
        get => _userSettings.StashTabQueryMode;
        set => _ = UpdateStashTabQueryModeAsync(value);
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

    public string StashTabIdsToString
    {
        get => string.Join(",", StashTabIds);
        set { return; }
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

    #endregion

    #endregion

    #region Methods

    private void InitializeDataFromUserSettings()
    {
        Log.Information("GeneralFormViewModel - Initializing data from user settings...");
        Log.Information("Current LeagueName Property: {LeagueName}", SelectedLeagueName);

        // Set LeagueName from user settings if it's valid
        if (!string.IsNullOrWhiteSpace(_userSettings.LeagueName))
        {
            SelectedLeagueName = _userSettings.LeagueName;

            // Force a refresh of the leagues list property for UI updates
            OnPropertyChanged(nameof(SelectedLeagueName));

            Log.Information("Post Change LeagueName Property: {LeagueName}", SelectedLeagueName);
        }
    }

    public async Task ForceRefreshLeaguesAsync()
    {
        _leaguesLoaded = false;
        await LoadLeagueListAsync();
    }

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
        Log.Information("GeneralFormViewModel - Loading {LeagueType} league names...", CustomLeagueEnabled ? "private" : "public");

        if (_leaguesLoaded)
        {
            Log.Information("GeneralFormViewModel - Leagues already loaded. Skipping fetch.");
            return;
        }

        if (CustomLeagueEnabled && _userSettings.LegacyAuthMode)
        {
            Log.Information("GeneralFormViewModel - Legacy Auth Mode with Custom Leagues is enabled, manually input league name.");
            return;
        }

        LeagueList.Clear();
        List<string> leagueList;

        try
        {
            leagueList = await _apiService.GetLeaguesAsync();
            Log.Information("GeneralFormViewModel - Leagues fetched successfully.");
        }
        catch (RateLimitException e)
        {
            Log.Error(e, "GeneralFormViewModel - Rate limit exceeded while fetching stash tabs. Waiting {SecondsToWait} seconds...", e.SecondsToWait);
            _leaguesLoaded = false;
            TriggerGeneralFormFetchCooldown();
            return;
        }

        // If the response is valid and we have leagues
        if (leagueList != null)
        {
            foreach (var league in leagueList)
            {
                LeagueList.Add(league);
            }
        }

        // Indicate that the leagues have been loaded
        _leaguesLoaded = true;
        TriggerGeneralFormFetchCooldown();
    }

    #region Property Setters

    private async Task UpdateLeagueNameAsync(string leagueName)
    {
        if (_userSettings.LeagueName != leagueName)
        {
            _userSettings.LeagueName = leagueName;
            StashTabIds = [];

            SetUIEnabledState(false);

            // re-fetching stash tabs if league is changed
            _stashTabsLoaded = false;

            // only update the stash tab list if the dropdown is enabled
            // if the user changes this setting while the dropdown is disabled,
            // we don't need to update the list automatically
            if (StashTabDropDownEnabled && !string.IsNullOrWhiteSpace(leagueName))
            {
                await LoadStashTabsAsync();
            }

            OnPropertyChanged(nameof(SelectedLeagueName));

            if (StashTabDropDownEnabled)
            {
                TriggerGeneralFormFetchCooldown();
            }
            else
            {
                SetUIEnabledState(true);
            }
        }
    }

    private async Task UpdateIsPrivateLeague(bool isPrivateLeague)
    {
        if (_userSettings.CustomLeagueEnabled != isPrivateLeague)
        {
            _userSettings.CustomLeagueEnabled = isPrivateLeague;

            // reset the selected league name
            SelectedLeagueName = string.Empty;

            SetUIEnabledState(false);

            // reset the loaded state to force a reload of the league data
            _leaguesLoaded = false;

            // only update the league list if the dropdown is enabled
            // if the user changes this setting while the dropdown is disabled,
            // we don't need to update the list automatically
            if (LeagueDropDownEnabled)
            {
                await LoadLeagueListAsync();
            }

            OnPropertyChanged(nameof(CustomLeagueEnabled));

            TriggerGeneralFormFetchCooldown();
        }
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

            StashTabButtonEnabled = false;

            // reset the loaded state to force a reload of the stash tab data
            _stashTabsLoaded = false;

            // Load tabs for the new mode
            await LoadStashTabsAsync();

            // Notify the UI of the change
            OnPropertyChanged(nameof(StashTabQueryMode));

            TriggerGeneralFormFetchCooldown();
        }
    }

    #endregion

    #region Stash Tab Selection Methods

    /// <summary>
    /// Loads the stash tabs from the API asynchronously.
    /// Ensures stash tabs are only loaded once unless explicitly refreshed.
    /// </summary>
    /// <returns></returns>
    public async Task LoadStashTabsAsync(bool isFirstLoad = false)
    {

        if (string.IsNullOrWhiteSpace(SelectedLeagueName))
        {
            Log.Information("GeneralFormViewModel - League name not defined; cannot fetch Stash Tabs. Skipping fetch.");
            return;
        }

        Log.Information("GeneralFormViewModel - Loading stash tabs for {LeagueName}...", _userSettings.LeagueName);

        // If the stash tabs are already loaded, wait for the cooldown and re-enable the button
        if (_stashTabsLoaded)
        {
            Log.Information("GeneralFormViewModel - StashTabs already loaded. Skipping fetch.");
            return;
        }

        // Disable the button to prevent multiple requests while waiting for the API response
        SetUIEnabledState(false);

        // Fetch the stash tabs - this is the biggest call in this component
        List<UnifiedStashTabMetadata> stashTabPropsList;

        try
        {
            stashTabPropsList = await _apiService.GetAllPersonalStashTabMetadataWithOAuthAsync();

            Log.Information("GeneralFormViewModel - StashTabs fetched successfully.");
        }
        catch (RateLimitException e)
        {
            Log.Error(e, "GeneralFormViewModel - Rate limit exceeded while fetching stash tabs. Waiting {SecondsToWait} seconds...", e.SecondsToWait);
            _stashTabsLoaded = false;
            TriggerGeneralFormFetchCooldown();
            return;
        }

        // If the response is valid and we have stash tabs
        if (stashTabPropsList != null)
        {
            // update the full list of stash tabs
            UpdateStashTabListForSelection(stashTabPropsList);
        }

        // Indicate that the tabs have been and re-enable the fetch button
        _stashTabsLoaded = true;

        // If this is the first load, we don't need to wait for the cooldown
        TriggerGeneralFormFetchCooldown();
    }

    /// <summary>
    /// Updates the full list of stash tabs for selection in the UI.
    /// </summary>
    /// <param name="stashTabProps">The list of stash tabs to update the UI with.</param>
    private void UpdateStashTabListForSelection(List<UnifiedStashTabMetadata> stashTabProps)
    {
        // Depending on the query mode, select the appropriate observable list to update
        var StashTabFullListForSelection = StashTabFullListForSelectionById;

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
        var StashTabFullListForSelection = StashTabFullListForSelectionById;

        // Depending on the mode, select tabs by either index or ID
        // We'll store the set of selected stash tab in this local variable
        var selectedIdentifiers = _userSettings.StashTabIds;

        // If there are selected tabs from the settings, pre-select them
        if (selectedIdentifiers.Count > 0)
        {
            // for each tab in the full list, check if it's in the selected list
            foreach (var tab in StashTabFullListForSelection)
            {
                if (selectedIdentifiers.Contains(tab.Id))
                {
                    SelectedStashTabsById.Add(tab);
                }
            }
        }

        // if we are searching by ID, check if the ID is in the list
        if (_userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.TabsById)
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

    #endregion

    private void TriggerGeneralFormFetchCooldown()
    {
        // Ensure operation on the UI thread if called from another thread
        Application.Current.Dispatcher.Invoke(() =>
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(FetchCooldownSeconds * 1000)
            };

            // Set the action to be executed after the cooldown
            timer.Tick += (sender, args) =>
            {
                SetUIEnabledState(true); // Re-enable the UI elements
                timer.Stop(); // Stop the timer to avoid it triggering again
                Log.Information("GeneralFormViewModel - Fetch cooldown timer has ended");
            };

            Log.Information("GeneralFormViewModel - Starting fetch cooldown timer for {SecondsToWait} seconds", FetchCooldownSeconds);
            SetUIEnabledState(false); // Disable the UI elements before starting the timer
            timer.Start(); // Start the cooldown timer
        });
    }

    private void SetUIEnabledState(bool isEnabled)
    {
        // Set the enabled state of all UI elements involved in fetching operations
        LeagueButtonEnabled = isEnabled;
        StashTabButtonEnabled = isEnabled;
        PrivateLeagueCheckboxEnabled = isEnabled;
        LeagueDropDownEnabled = isEnabled && _leaguesLoaded; // Only enable if leagues have been successfully loaded
        StashTabDropDownEnabled = isEnabled && _stashTabsLoaded; // Only enable if stash tabs have been successfully loaded
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

        foreach (var tab in selectedStashTabProps.Cast<UnifiedStashTabMetadata>())
        {
            var identifier = tab.Id;

            tempSelectedItems.Add(identifier);
        }

        // Update the user settings based on the temporary collection
        if (_userSettings.StashTabQueryMode == (int)Models.Enums.StashTabQueryMode.TabsById)
        {
            // Ensure only to update if there's a change to minimize setter calls
            if (!_userSettings.StashTabIds.SetEquals(tempSelectedItems))
            {
                _userSettings.StashTabIds = tempSelectedItems;
            }
        }
    }

    public void UpdateUserSettingsForSelectedLeague(object selectedItem)
    {

        if (selectedItem == null)
        {
            SelectedLeagueName = _userSettings.LeagueName;
            return;
        }

        if (string.IsNullOrWhiteSpace(selectedItem.ToString())) return;

        SelectedLeagueName = selectedItem.ToString();
        LeagueDropDownEnabled = false;
    }

    #endregion
}