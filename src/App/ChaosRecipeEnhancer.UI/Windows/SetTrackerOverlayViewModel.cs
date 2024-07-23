using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.Exceptions;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using ChaosRecipeEnhancer.UI.UserControls;
using ChaosRecipeEnhancer.UI.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Enums = ChaosRecipeEnhancer.UI.Models.Enums;

namespace ChaosRecipeEnhancer.UI.Windows;

public sealed class SetTrackerOverlayViewModel : CreViewModelBase
{
    private readonly IFilterManipulationService _filterManipulationService;
    private readonly IReloadFilterService _reloadFilterService;
    private readonly IPoeApiService _apiService;
    private readonly IUserSettings _userSettings;
    private readonly IAuthStateManager _authStateManager;
    private readonly INotificationSoundService _notificationSoundService;

    private const string SetsFullText = "Sets full!";

    // this should match the cooldown we apply in the log watcher manager
    /// <see cref="LogWatcherManager.AutoFetchCooldownSeconds"/>
    private const int FetchCooldownSeconds = 15;

    private bool _fetchButtonEnabled = true;
    private bool _stashButtonTooltipEnabled = false;
    private bool _setsTooltipEnabled = false;
    private string _warningMessage;

    public SetTrackerOverlayViewModel(
        IFilterManipulationService filterManipulationService,
        IReloadFilterService reloadFilterService,
        IPoeApiService apiService,
        IUserSettings userSettings,
        IAuthStateManager authStateManager,
        INotificationSoundService notificationSoundService
    )
    {
        _filterManipulationService = filterManipulationService;
        _reloadFilterService = reloadFilterService;
        _apiService = apiService;
        _userSettings = userSettings;
        _authStateManager = authStateManager;
        _notificationSoundService = notificationSoundService;
    }

    #region Properties

    #region User Settings Accessor Properties

    public double SetTrackerOverlayWindowScale
    {
        get => _userSettings.SetTrackerOverlayWindowScale;
    }

    public Enums.SetTrackerOverlayItemCounterDisplayMode SetTrackerOverlayItemCounterDisplayMode
    {
        get => _userSettings.SetTrackerOverlayItemCounterDisplayMode;
        set
        {
            if (_userSettings.SetTrackerOverlayItemCounterDisplayMode != value)
            {
                _userSettings.SetTrackerOverlayItemCounterDisplayMode = value;
                OnPropertyChanged(nameof(SetTrackerOverlayItemCounterDisplayMode));
            }
        }
    }

    public int FullSetThreshold
    {
        get => _userSettings.FullSetThreshold;
        set
        {
            if (_userSettings.FullSetThreshold != value)
            {
                _userSettings.FullSetThreshold = value;
                OnPropertyChanged(nameof(FullSetThreshold));
            }
        }
    }

    public bool LegacyAuthMode
    {
        get => _userSettings.LegacyAuthMode;
        set
        {
            if (_userSettings.LegacyAuthMode != value)
            {
                _userSettings.LegacyAuthMode = value;
                OnPropertyChanged(nameof(LegacyAuthMode));
            }
        }
    }

    public string LegacyAuthSessionId
    {
        get => _userSettings.LegacyAuthSessionId;
        set
        {
            if (_userSettings.LegacyAuthSessionId != value)
            {
                _userSettings.LegacyAuthSessionId = value;
                OnPropertyChanged(nameof(LegacyAuthSessionId));
            }
        }
    }

    public bool GuildStashMode
    {
        get => _userSettings.GuildStashMode;
        set
        {
            if (_userSettings.GuildStashMode != value)
            {
                _userSettings.GuildStashMode = value;
                OnPropertyChanged(nameof(GuildStashMode));
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

    public bool VendorSetsEarly
    {
        get => _userSettings.VendorSetsEarly;
        set
        {
            if (_userSettings.VendorSetsEarly != value)
            {
                _userSettings.VendorSetsEarly = value;
                OnPropertyChanged(nameof(VendorSetsEarly));
            }
        }
    }

    public bool SilenceSetsFullMessage
    {
        get => _userSettings.SilenceSetsFullMessage;
        set
        {
            if (_userSettings.SilenceSetsFullMessage != value)
            {
                _userSettings.SilenceSetsFullMessage = value;
                OnPropertyChanged(nameof(SilenceSetsFullMessage));
            }
        }
    }

    public bool SilenceNeedItemsMessage
    {
        get => _userSettings.SilenceNeedItemsMessage;
        set
        {
            if (_userSettings.SilenceNeedItemsMessage != value)
            {
                _userSettings.SilenceNeedItemsMessage = value;
                OnPropertyChanged(nameof(SilenceNeedItemsMessage));
            }
        }
    }

    public bool ChaosRecipeTrackingEnabled
    {
        get => _userSettings.ChaosRecipeTrackingEnabled;
        set
        {
            if (_userSettings.ChaosRecipeTrackingEnabled != value)
            {
                _userSettings.ChaosRecipeTrackingEnabled = value;
                OnPropertyChanged(nameof(ChaosRecipeTrackingEnabled));
            }
        }
    }

    #region Always Active Settings

    public bool LootFilterStylesRingAlwaysActive
    {
        get => _userSettings.LootFilterStylesRingAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesRingAlwaysActive != value)
            {
                _userSettings.LootFilterStylesRingAlwaysActive = value;
                OnPropertyChanged(nameof(LootFilterStylesRingAlwaysActive));
            }
        }
    }

    public bool LootFilterStylesAmuletAlwaysActive
    {
        get => _userSettings.LootFilterStylesAmuletAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesAmuletAlwaysActive != value)
            {
                _userSettings.LootFilterStylesAmuletAlwaysActive = value;
                OnPropertyChanged(nameof(LootFilterStylesAmuletAlwaysActive));
            }
        }
    }

    public bool LootFilterStylesBeltAlwaysActive
    {
        get => _userSettings.LootFilterStylesBeltAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesBeltAlwaysActive != value)
            {
                _userSettings.LootFilterStylesBeltAlwaysActive = value;
                OnPropertyChanged(nameof(LootFilterStylesBeltAlwaysActive));
            }
        }
    }

    public bool LootFilterStylesBodyArmourAlwaysActive
    {
        get => _userSettings.LootFilterStylesBodyArmourAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourAlwaysActive != value)
            {
                _userSettings.LootFilterStylesBodyArmourAlwaysActive = value;
                OnPropertyChanged(nameof(LootFilterStylesBodyArmourAlwaysActive));
            }
        }
    }

    public bool LootFilterStylesGlovesAlwaysActive
    {
        get => _userSettings.LootFilterStylesGlovesAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesGlovesAlwaysActive != value)
            {
                _userSettings.LootFilterStylesGlovesAlwaysActive = value;
                OnPropertyChanged(nameof(LootFilterStylesGlovesAlwaysActive));
            }
        }
    }

    public bool LootFilterStylesBootsAlwaysActive
    {
        get => _userSettings.LootFilterStylesBootsAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesBootsAlwaysActive != value)
            {
                _userSettings.LootFilterStylesBootsAlwaysActive = value;
                OnPropertyChanged(nameof(LootFilterStylesBootsAlwaysActive));
            }
        }
    }

    public bool LootFilterStylesHelmetAlwaysActive
    {
        get => _userSettings.LootFilterStylesHelmetAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesHelmetAlwaysActive != value)
            {
                _userSettings.LootFilterStylesHelmetAlwaysActive = value;
                OnPropertyChanged(nameof(LootFilterStylesHelmetAlwaysActive));
            }
        }
    }

    public bool LootFilterStylesWeaponAlwaysActive
    {
        get => _userSettings.LootFilterStylesWeaponAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesWeaponAlwaysActive != value)
            {
                _userSettings.LootFilterStylesWeaponAlwaysActive = value;
                OnPropertyChanged(nameof(LootFilterStylesWeaponAlwaysActive));
            }
        }
    }

    #endregion

    #region Always Disabled Settings

    public bool LootFilterStylesRingAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesRingAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesRingAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesRingAlwaysDisabled = value;
                OnPropertyChanged(nameof(LootFilterStylesRingAlwaysDisabled));
            }
        }
    }

    public bool LootFilterStylesAmuletAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesAmuletAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesAmuletAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesAmuletAlwaysDisabled = value;
                OnPropertyChanged(nameof(LootFilterStylesAmuletAlwaysDisabled));
            }
        }
    }

    public bool LootFilterStylesBeltAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesBeltAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesBeltAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesBeltAlwaysDisabled = value;
                OnPropertyChanged(nameof(LootFilterStylesBeltAlwaysDisabled));
            }
        }
    }

    public bool LootFilterStylesBodyArmourAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesBodyArmourAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesBodyArmourAlwaysDisabled = value;
                OnPropertyChanged(nameof(LootFilterStylesBodyArmourAlwaysDisabled));
            }
        }
    }

    public bool LootFilterStylesGlovesAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesGlovesAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesGlovesAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesGlovesAlwaysDisabled = value;
                OnPropertyChanged(nameof(LootFilterStylesGlovesAlwaysDisabled));
            }
        }
    }

    public bool LootFilterStylesBootsAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesBootsAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesBootsAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesBootsAlwaysDisabled = value;
                OnPropertyChanged(nameof(LootFilterStylesBootsAlwaysDisabled));
            }
        }
    }

    public bool LootFilterStylesHelmetAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesHelmetAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesHelmetAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesHelmetAlwaysDisabled = value;
                OnPropertyChanged(nameof(LootFilterStylesHelmetAlwaysDisabled));
            }
        }
    }

    public bool LootFilterStylesWeaponAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesWeaponAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesWeaponAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesWeaponAlwaysDisabled = value;
                OnPropertyChanged(nameof(LootFilterStylesWeaponAlwaysDisabled));
            }
        }
    }

    #endregion

    #endregion

    private bool ShowAmountNeeded => SetTrackerOverlayItemCounterDisplayMode == Enums.SetTrackerOverlayItemCounterDisplayMode.ItemsMissing;
    public bool NeedsFetching => GlobalItemSetManagerState.NeedsFetching;
    public bool NeedsLowerLevel => GlobalItemSetManagerState.NeedsLowerLevel;
    public int FullSets => GlobalItemSetManagerState.CompletedSetCount;

    #region Item Amount and Visibility Properties

    #region Item Amount Properties

    public int RingsAmount => ShowAmountNeeded
        // case where we are showing missing items (calculate total needed and subtract from threshold, but don't show negatives)
        ? Math.Max((FullSetThreshold * 2) - GlobalItemSetManagerState.RingsAmount, 0)
        // case where we are showing total item sets (e.g. pair of rings as a single 'count')
        : GlobalItemSetManagerState.RingsAmount / 2;

    public int AmuletsAmount =>
        ShowAmountNeeded
            ? Math.Max(FullSetThreshold - GlobalItemSetManagerState.AmuletsAmount, 0)
            : GlobalItemSetManagerState.AmuletsAmount;

    public int BeltsAmount =>
        ShowAmountNeeded
            ? Math.Max(FullSetThreshold - GlobalItemSetManagerState.BeltsAmount, 0)
            : GlobalItemSetManagerState.BeltsAmount;

    public int ChestsAmount => ShowAmountNeeded
        ? Math.Max(FullSetThreshold - GlobalItemSetManagerState.ChestsAmount, 0)
        : GlobalItemSetManagerState.ChestsAmount;

    public int GlovesAmount =>
        ShowAmountNeeded
            ? Math.Max(FullSetThreshold - GlobalItemSetManagerState.GlovesAmount, 0)
            : GlobalItemSetManagerState.GlovesAmount;

    public int BootsAmount =>
    ShowAmountNeeded
        ? Math.Max(FullSetThreshold - GlobalItemSetManagerState.BootsAmount, 0)
        : GlobalItemSetManagerState.BootsAmount;

    public int HelmetsAmount =>
        ShowAmountNeeded
            ? Math.Max(FullSetThreshold - GlobalItemSetManagerState.HelmetsAmount, 0)
            : GlobalItemSetManagerState.HelmetsAmount;

    public int WeaponsAmount => ShowAmountNeeded
        // case where we are showing missing items (calculate total needed and subtract from threshold, but don't show negatives)
        ? Math.Max((FullSetThreshold * 2) - (GlobalItemSetManagerState.WeaponsSmallAmount + (GlobalItemSetManagerState.WeaponsBigAmount * 2)), 0)
        // case where we are showing total weapon sets (e.g. pair of one handed weapons plus two handed weapons as a 'count' each)
        : (GlobalItemSetManagerState.WeaponsSmallAmount / 2) + GlobalItemSetManagerState.WeaponsBigAmount;

    #endregion

    #region Item Class Active (Visibility) Properties

    public bool RingsActive =>
        (LootFilterStylesRingAlwaysActive ||
        (NeedsFetching || (FullSetThreshold * 2) - GlobalItemSetManagerState.RingsAmount > 0)) &&
        !LootFilterStylesRingAlwaysDisabled;

    public bool AmuletsActive =>
        (LootFilterStylesAmuletAlwaysActive ||
        (NeedsFetching || FullSetThreshold - GlobalItemSetManagerState.AmuletsAmount > 0)) &&
        !LootFilterStylesAmuletAlwaysDisabled;

    public bool BeltsActive =>
        (LootFilterStylesBeltAlwaysActive ||
        (NeedsFetching || FullSetThreshold - GlobalItemSetManagerState.BeltsAmount > 0)) &&
        !LootFilterStylesBeltAlwaysDisabled;

    public bool ChestsActive =>
        (LootFilterStylesBodyArmourAlwaysActive ||
        (NeedsFetching || FullSetThreshold - GlobalItemSetManagerState.ChestsAmount > 0)) &&
        !LootFilterStylesBodyArmourAlwaysDisabled;

    public bool GlovesActive =>
        (LootFilterStylesGlovesAlwaysActive ||
        (NeedsFetching || FullSetThreshold - GlobalItemSetManagerState.GlovesAmount > 0)) &&
        !LootFilterStylesGlovesAlwaysDisabled;

    public bool HelmetsActive =>
        (LootFilterStylesHelmetAlwaysActive ||
        (NeedsFetching || FullSetThreshold - GlobalItemSetManagerState.HelmetsAmount > 0)) &&
        !LootFilterStylesHelmetAlwaysDisabled;

    public bool BootsActive =>
        (LootFilterStylesBootsAlwaysActive ||
        (NeedsFetching || FullSetThreshold - GlobalItemSetManagerState.BootsAmount > 0)) &&
        !LootFilterStylesBootsAlwaysDisabled;

    public bool WeaponsActive =>
        (LootFilterStylesWeaponAlwaysActive ||
        (NeedsFetching || (FullSetThreshold * 2) - (GlobalItemSetManagerState.WeaponsSmallAmount + (GlobalItemSetManagerState.WeaponsBigAmount * 2)) > 0)) &&
        !LootFilterStylesWeaponAlwaysDisabled;

    #endregion

    #endregion

    public bool FetchButtonEnabled
    {
        get => _fetchButtonEnabled;
        set => SetProperty(ref _fetchButtonEnabled, value);
    }

    public bool StashButtonTooltipEnabled
    {
        get => _stashButtonTooltipEnabled;
        set => SetProperty(ref _stashButtonTooltipEnabled, value);
    }

    public bool SetsTooltipEnabled
    {
        get => _setsTooltipEnabled;
        set => SetProperty(ref _setsTooltipEnabled, value);
    }

    public string WarningMessage
    {
        get => _warningMessage;
        set => SetProperty(ref _warningMessage, value);
    }

    #endregion

    #region Domain Methods

    public async Task<bool> FetchStashDataAsync()
    {
        WarningMessage = string.Empty;
        FetchButtonEnabled = false;

        try
        {
            // needed to update item set manager
            var setThreshold = FullSetThreshold;

            // have to do a bit of wizardry because we store the selected tab indices as a string in the user settings
            var filteredStashContents = new List<EnhancedItem>();

            // reset item amounts before fetching new data
            // invalidate some outdated state for our item manager
            GlobalItemSetManagerState.ResetCompletedSetCountAndItemAmounts();

            // update the stash tab metadata based on your target stash
            var stashTabMetadataList = LegacyAuthMode
                ? GuildStashMode
                    ? await _apiService.GetAllGuildStashTabMetadataWithSessionIdAsync(LegacyAuthSessionId)
                    : await _apiService.GetAllPersonalStashTabMetadataWithSessionIdAsync(LegacyAuthSessionId)
                : GuildStashMode
                    ? await _apiService.GetAllGuildStashTabMetadataWithOAuthAsync()
                    : await _apiService.GetAllPersonalStashTabMetadataWithOAuthAsync();

            // 'Flatten' the stash tab structure (unwrap children tabs from folders)
            var flattenedStashTabs = GlobalItemSetManagerState.FlattenStashTabs(stashTabMetadataList);

            List<string> selectedTabIds = [];

            if (StashTabQueryMode == (int)Enums.StashTabQueryMode.TabsById)
            {
                if (StashTabIds.Count == 0)
                {
                    FetchButtonEnabled = true;

                    GlobalErrorHandler.Spawn(
                        "It looks like you haven't selected any stash tab ids. Please navigate to the 'General > General > Select Stash Tabs' setting and select some tabs, and try again.",
                        "Error: Set Tracker Overlay - Fetch Data"
                    );

                    return false;
                }

                selectedTabIds = [.. StashTabIds];
            }
            else if (StashTabQueryMode == (int)Enums.StashTabQueryMode.TabsByNamePrefix)
            {

                if (string.IsNullOrWhiteSpace(StashTabPrefix))
                {
                    FetchButtonEnabled = true;

                    GlobalErrorHandler.Spawn(
                        "It looks like you haven't entered a stash tab prefix. Please navigate to the 'General > General > Stash Tab Prefix' setting and enter a valid value, and try again.",
                        "Error: Set Tracker Overlay - Fetch Data"
                    );

                    return false;
                }

                selectedTabIds = flattenedStashTabs
                    .Where(st => st.Name.StartsWith(StashTabPrefix))
                    .Select(st => st.Id)
                    .ToList();
            }

            if (flattenedStashTabs is not null)
            {
                GlobalItemSetManagerState.StashTabMetadataListStashesResponse = flattenedStashTabs;

                foreach (var id in selectedTabIds)
                {
                    UnifiedStashTabContents rawResults;

                    // Session ID endpoint uses tab index for lookup - so we 'extract' the index from the tab collection constructed using id's
                    if (LegacyAuthMode)
                    {
                        // For SessionId auth, we need to find the index corresponding to this id
                        var stashTab = flattenedStashTabs.FirstOrDefault(st => st.Id == id);

                        if (stashTab == null)
                        {
                            continue; // Skip this tab if we can't find its metadata
                        }

                        if (GuildStashMode)
                        {
                            rawResults = await _apiService.GetGuildStashTabContentsByIndexWithSessionIdAsync(
                                LegacyAuthSessionId,
                                stashTab.Id,
                                stashTab.Name,
                                stashTab.Index,
                                stashTab.Type
                            );
                        }
                        else
                        {
                            rawResults = await _apiService.GetPersonalStashTabContentsByIndexWithSessionIdAsync(
                                LegacyAuthSessionId,
                                stashTab.Id,
                                stashTab.Name,
                                stashTab.Index,
                                stashTab.Type
                            );
                        }

                    }
                    // OAuth endpoint uses tab ID for lookup
                    else
                    {
                        if (GuildStashMode)
                        {
                            rawResults = await _apiService.GetGuildStashTabContentsByStashIdWithOAuthAsync(id);
                        }
                        else
                        {
                            rawResults = await _apiService.GetPersonalStashTabContentsByStashIdWithOAuthAsync(id);
                        }
                    }

                    // then we convert the raw results into a list of EnhancedItem objects
                    var enhancedItems = rawResults.Items.Select(item => new EnhancedItem(item)).ToList();

                    // Manually setting id because we need to know which tab the item came from
                    foreach (var enhancedItem in enhancedItems)
                    {
                        enhancedItem.StashTabId = rawResults.Id;
                        enhancedItem.StashTabIndex = rawResults.Index;
                    }

                    // add the enhanced items to the filtered stash contents
                    filteredStashContents.AddRange(EnhancedItemUtilities.FilterItemsForRecipe(enhancedItems));

                    GlobalItemSetManagerState.UpdateStashContents(setThreshold, selectedTabIds, filteredStashContents);
                }

                // recalculate item amounts and generate item sets after fetching from api
                GlobalItemSetManagerState.CalculateItemAmounts();

                // generate item sets for the chosen recipe (chaos or regal)
                GlobalItemSetManagerState.GenerateItemSets(!ChaosRecipeTrackingEnabled);

                // update the UI accordingly
                UpdateDisplay();
                UpdateStashButtonAndWarningMessage();

                // enforce cooldown on fetch button to reduce chances of rate limiting
                TriggerSetTrackerFetchCooldown(FetchCooldownSeconds);
            }
            else
            {
                FetchButtonEnabled = true;
                return false;
            }
        }
        catch (RateLimitException e)
        {
            FetchButtonEnabled = false;
            WarningMessage = $"Rate Limit Exceeded! Waiting {e.SecondsToWait} seconds...";

            // Cooldown the refresh button until the rate limit is lifted
            TriggerSetTrackerFetchCooldown(e.SecondsToWait);

            WarningMessage = string.Empty;

            return true;
        }
        catch (NullReferenceException e)
        {
            FetchButtonEnabled = true;
            _authStateManager.Logout();
            GlobalErrorHandler.Spawn(
                e.ToString(),
                "Error: Set Tracker Overlay - Invalid Credentials",
                preamble: "It looks like your credentials have expired. Please log back in to continue."
            );
            return false;
        }

        return true;
    }

    public void UpdateStashButtonAndWarningMessage(bool playNotificationSound = true)
    {
        // case 1: user just opened the app, hasn't hit fetch yet
        if (NeedsFetching)
        {
            WarningMessage = string.Empty;
        }
        else if (!NeedsFetching)
        {
            // case 2: user fetched data and has enough sets to turn in based on their threshold
            if (FullSets >= FullSetThreshold)
            {
                // if the user has vendor sets early enabled, we don't want to show the warning message
                if (!VendorSetsEarly || FullSets >= FullSetThreshold)
                {
                    WarningMessage = !SilenceSetsFullMessage
                        ? SetsFullText
                        : string.Empty;
                }

                // stash button is enabled with no warning tooltip
                StashButtonTooltipEnabled = false;
                SetsTooltipEnabled = false;

                if (playNotificationSound)
                {
                    PlayItemSetStateChangedNotificationSound();
                }
            }

            // case 3: user fetched data and has at least 1 set, but not to their full threshold
            else if ((FullSets < FullSetThreshold || VendorSetsEarly) && FullSets >= 1)
            {
                if (NeedsLowerLevel && ChaosRecipeTrackingEnabled)
                {
                    WarningMessage = !SilenceNeedItemsMessage
                    ? NeedsLowerLevelText(FullSets - FullSetThreshold)
                    : string.Empty;

                    // stash button is disabled with conditional tooltip enabled
                    // based on whether or not the user has at least 1 set
                    StashButtonTooltipEnabled = FullSets >= 1;
                    SetsTooltipEnabled = true;
                }
                else
                {
                    WarningMessage = string.Empty;

                    // stash button is disabled with warning tooltip to change threshold
                    if (VendorSetsEarly)
                    {
                        StashButtonTooltipEnabled = false;
                        SetsTooltipEnabled = false;
                    }
                    else
                    {
                        StashButtonTooltipEnabled = true;
                        SetsTooltipEnabled = true;
                    }
                }
            }

            // case 4: user has fetched and needs items for chaos recipe (needs more lower level items)
            else if (NeedsLowerLevel && ChaosRecipeTrackingEnabled)
            {
                WarningMessage = !SilenceNeedItemsMessage
                    ? NeedsLowerLevelText(FullSets - FullSetThreshold)
                    : string.Empty;

                // stash button is disabled with conditional tooltip enabled
                // based on whether or not the user has at least 1 set
                StashButtonTooltipEnabled = FullSets >= 1;
                SetsTooltipEnabled = true;
            }

            // case 5: user has fetched and has no sets
            else if (FullSets == 0)
            {
                WarningMessage = string.Empty;

                // stash button is disabled with warning tooltip to change threshold
                StashButtonTooltipEnabled = true;
                SetsTooltipEnabled = true;
            }
        }
    }

    public async Task RunReloadFilter()
    {
        // Retrieve the current item counts for different item classes
        var itemClassAmounts = GlobalItemSetManagerState.RetrieveCurrentItemCountsForFilterManipulation();

        // Initialize a set to keep track of missing item classes
        var missingItemClasses = new HashSet<string>();

        // Aggregate counts for all item classes from the provided data
        var totalItemCounts = AggregateItemCounts(itemClassAmounts);

        // Handle special cases for weapons and rings
        HandleSpecialCases(totalItemCounts);

        // Identify item classes that are missing based on the aggregated counts
        IdentifyMissingItemClasses(totalItemCounts, missingItemClasses);

        // Generate the necessary filter sections and update the filter with the missing item classes
        await _filterManipulationService.GenerateSectionsAndUpdateFilterAsync(missingItemClasses, NeedsLowerLevel);

        // Reload the filter to apply the updates
        _reloadFilterService.ReloadFilter();
    }

    private Dictionary<ItemClass, int> AggregateItemCounts(List<Dictionary<ItemClass, int>> itemClassAmounts)
    {
        // Create a dictionary to store the total counts of each item class
        var totalItemCounts = new Dictionary<ItemClass, int>();

        // Iterate over each dictionary in the list
        foreach (var dict in itemClassAmounts)
        {
            // Iterate over each item class count pair in the dictionary
            foreach (var kvp in dict)
            {
                // If the item class is already in the total counts dictionary, add the current count
                if (totalItemCounts.ContainsKey(kvp.Key))
                    totalItemCounts[kvp.Key] += kvp.Value;
                // Otherwise, add the item class to the total counts dictionary with the current count
                else
                    totalItemCounts[kvp.Key] = kvp.Value;
            }
        }

        // Return the aggregated counts of item classes
        return totalItemCounts;
    }

    private void HandleSpecialCases(Dictionary<ItemClass, int> totalItemCounts)
    {
        // Get the total count of one-handed weapons, defaulting to 0 if not found
        var oneHandedWeaponCount = totalItemCounts.GetValueOrDefault(ItemClass.OneHandWeapons, 0);

        // Get the total count of two-handed weapons, defaulting to 0 if not found
        var twoHandedWeaponCount = totalItemCounts.GetValueOrDefault(ItemClass.TwoHandWeapons, 0);

        // Get the total count of rings, defaulting to 0 if not found
        var ringCount = totalItemCounts.GetValueOrDefault(ItemClass.Rings, 0);

        // If the combined count of one-handed and two-handed weapons reaches the full set threshold,
        // remove these item classes from the total counts
        if (oneHandedWeaponCount / 2 + twoHandedWeaponCount >= FullSetThreshold)
        {
            totalItemCounts.Remove(ItemClass.OneHandWeapons);
            totalItemCounts.Remove(ItemClass.TwoHandWeapons);
        }

        // If the count of rings reaches the full set threshold, remove the rings item class from the total counts
        if (ringCount / 2 >= FullSetThreshold)
        {
            totalItemCounts.Remove(ItemClass.Rings);
        }
    }

    private void IdentifyMissingItemClasses(Dictionary<ItemClass, int> totalItemCounts, HashSet<string> missingItemClasses)
    {
        // Iterate over each item class count pair in the total counts dictionary
        foreach (var kvp in totalItemCounts)
        {
            // Special treatment for rings
            if (kvp.Key == ItemClass.Rings)
            {
                // If the count of rings does not reach the full set threshold, add it to the missing item classes
                if (kvp.Value / 2 < FullSetThreshold)
                {
                    missingItemClasses.Add(kvp.Key.ToString());
                }
            }
            // For other item classes, if the count does not reach the full set threshold, add it to the missing item classes
            else if (kvp.Value < FullSetThreshold)
            {
                missingItemClasses.Add(kvp.Key.ToString());
            }
        }
    }

    #endregion

    #region Utility Methods

    private static string NeedsLowerLevelText(int diff) => $"Need {Math.Abs(diff)} items with iLvl 60-74!";

    public void UpdateWindowScale()
    {
        OnPropertyChanged(nameof(SetTrackerOverlayWindowScale));
    }

    public void UpdateDisplay()
    {
        OnPropertyChanged(nameof(RingsAmount));
        OnPropertyChanged(nameof(RingsActive));

        OnPropertyChanged(nameof(AmuletsAmount));
        OnPropertyChanged(nameof(AmuletsActive));

        OnPropertyChanged(nameof(BeltsAmount));
        OnPropertyChanged(nameof(BeltsActive));

        OnPropertyChanged(nameof(ChestsAmount));
        OnPropertyChanged(nameof(ChestsActive));

        OnPropertyChanged(nameof(WeaponsAmount));
        OnPropertyChanged(nameof(WeaponsActive));

        OnPropertyChanged(nameof(GlovesAmount));
        OnPropertyChanged(nameof(GlovesActive));

        OnPropertyChanged(nameof(HelmetsAmount));
        OnPropertyChanged(nameof(HelmetsActive));

        OnPropertyChanged(nameof(BootsAmount));
        OnPropertyChanged(nameof(BootsActive));

        OnPropertyChanged(nameof(NeedsFetching));
        OnPropertyChanged(nameof(NeedsLowerLevel));
        OnPropertyChanged(nameof(FullSets));
        OnPropertyChanged(nameof(WarningMessage));
        OnPropertyChanged(nameof(FetchButtonEnabled));
        OnPropertyChanged(nameof(ShowAmountNeeded));
    }

    public void PlayItemSetStateChangedNotificationSound()
    {
        _notificationSoundService.PlayNotificationSound(Enums.NotificationSoundType.ItemSetStateChanged);
    }

    public void TriggerSetTrackerFetchCooldown(int secondsToWait)
    {
        // Ensure operation on the UI thread if called from another thread
        Application.Current.Dispatcher.Invoke(() =>
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(secondsToWait)
            };

            // define the action to take when the timer ticks
            timer.Tick += (sender, args) =>
            {
                FetchButtonEnabled = true; // Re-enable the fetch button
                timer.Stop(); // Stop the timer to avoid it triggering again
                Log.Information("SetTrackerOverlayViewModel - Fetch cooldown timer has ended");
            };

            Log.Information("SetTrackerOverlayViewModel - Starting fetch cooldown timer for {SecondsToWait} seconds", secondsToWait);
            FetchButtonEnabled = false; // Disable the fetch button before starting the timer
            timer.Start(); // Start the cooldown timer
        });
    }

    #endregion
}