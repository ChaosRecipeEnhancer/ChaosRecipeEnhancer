using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.Shared;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.Exceptions;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using ChaosRecipeEnhancer.UI.UserControls;
using ChaosRecipeEnhancer.UI.Utilities;
using CommunityToolkit.Mvvm.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ChaosRecipeEnhancer.UI.Windows;

public sealed class SetTrackerOverlayViewModel : ViewModelBase
{
    #region Fields

    private readonly IFilterManipulationService _filterManipulationService = Ioc.Default.GetRequiredService<IFilterManipulationService>();
    private readonly IReloadFilterService _reloadFilterService = Ioc.Default.GetRequiredService<IReloadFilterService>();
    private readonly IPoeApiService _apiService = Ioc.Default.GetRequiredService<IPoeApiService>();
    private readonly IUserSettings _userSettings = Ioc.Default.GetRequiredService<IUserSettings>();
    private readonly IAuthStateManager _authStateManager = Ioc.Default.GetRequiredService<IAuthStateManager>();
    private readonly INotificationSoundService _notificationSoundService = Ioc.Default.GetRequiredService<INotificationSoundService>();

    private const string SetsFullText = "Sets full!";

    // this should match the cooldown we apply in the log watcher manager
    /// <see cref="LogWatcherManager.AutoFetchCooldownSeconds"/>
    private const int FetchCooldownSeconds = 15;

    private bool _fetchButtonEnabled = true;
    private bool _stashButtonTooltipEnabled = false;
    private bool _setsTooltipEnabled = false;
    private string _warningMessage;

    #endregion

    #region Properties

    private bool ShowAmountNeeded => _userSettings.SetTrackerOverlayItemCounterDisplayMode == SetTrackerOverlayItemCounterDisplayMode.ItemsMissing;
    public bool NeedsFetching => GlobalItemSetManagerState.NeedsFetching;
    public bool NeedsLowerLevel => GlobalItemSetManagerState.NeedsLowerLevel;
    public int FullSets => GlobalItemSetManagerState.CompletedSetCount;

    #region Item Amount and Visibility Properties

    #region Item Amount Properties

    public int RingsAmount => ShowAmountNeeded
        // case where we are showing missing items (calculate total needed and subtract from threshold, but don't show negatives)
        ? Math.Max((_userSettings.FullSetThreshold * 2) - GlobalItemSetManagerState.RingsAmount, 0)
        // case where we are showing total item sets (e.g. pair of rings as a single 'count')
        : GlobalItemSetManagerState.RingsAmount / 2;

    public int AmuletsAmount =>
        ShowAmountNeeded
            ? Math.Max(_userSettings.FullSetThreshold - GlobalItemSetManagerState.AmuletsAmount, 0)
            : GlobalItemSetManagerState.AmuletsAmount;

    public int BeltsAmount =>
        ShowAmountNeeded
            ? Math.Max(_userSettings.FullSetThreshold - GlobalItemSetManagerState.BeltsAmount, 0)
            : GlobalItemSetManagerState.BeltsAmount;

    public int ChestsAmount => ShowAmountNeeded
        ? Math.Max(_userSettings.FullSetThreshold - GlobalItemSetManagerState.ChestsAmount, 0)
        : GlobalItemSetManagerState.ChestsAmount;

    public int GlovesAmount =>
        ShowAmountNeeded
            ? Math.Max(_userSettings.FullSetThreshold - GlobalItemSetManagerState.GlovesAmount, 0)
            : GlobalItemSetManagerState.GlovesAmount;

    public int BootsAmount =>
    ShowAmountNeeded
        ? Math.Max(_userSettings.FullSetThreshold - GlobalItemSetManagerState.BootsAmount, 0)
        : GlobalItemSetManagerState.BootsAmount;

    public int HelmetsAmount =>
        ShowAmountNeeded
            ? Math.Max(_userSettings.FullSetThreshold - GlobalItemSetManagerState.HelmetsAmount, 0)
            : GlobalItemSetManagerState.HelmetsAmount;

    public int WeaponsAmount => ShowAmountNeeded
        // case where we are showing missing items (calculate total needed and subtract from threshold, but don't show negatives)
        ? Math.Max((_userSettings.FullSetThreshold * 2) - (GlobalItemSetManagerState.WeaponsSmallAmount + (GlobalItemSetManagerState.WeaponsBigAmount * 2)), 0)
        // case where we are showing total weapon sets (e.g. pair of one handed weapons plus two handed weapons as a 'count' each)
        : (GlobalItemSetManagerState.WeaponsSmallAmount / 2) + GlobalItemSetManagerState.WeaponsBigAmount;

    #endregion

    #region Item Class Active (Visibility) Properties

    public bool RingsActive =>
        _userSettings.LootFilterRingsAlwaysActive ||
        (NeedsFetching || (_userSettings.FullSetThreshold * 2) - GlobalItemSetManagerState.RingsAmount > 0);

    public bool AmuletsActive =>
        _userSettings.LootFilterAmuletsAlwaysActive ||
        (NeedsFetching || _userSettings.FullSetThreshold - GlobalItemSetManagerState.AmuletsAmount > 0);

    public bool BeltsActive =>
        _userSettings.LootFilterBeltsAlwaysActive ||
        (NeedsFetching || _userSettings.FullSetThreshold - GlobalItemSetManagerState.BeltsAmount > 0);

    public bool ChestsActive =>
        _userSettings.LootFilterBodyArmourAlwaysActive ||
        (NeedsFetching || _userSettings.FullSetThreshold - GlobalItemSetManagerState.ChestsAmount > 0);

    public bool GlovesActive =>
        _userSettings.LootFilterGlovesAlwaysActive ||
        (NeedsFetching || _userSettings.FullSetThreshold - GlobalItemSetManagerState.GlovesAmount > 0);

    public bool HelmetsActive =>
        _userSettings.LootFilterHelmetsAlwaysActive ||
        (NeedsFetching || _userSettings.FullSetThreshold - GlobalItemSetManagerState.HelmetsAmount > 0);

    public bool BootsActive =>
        _userSettings.LootFilterBootsAlwaysActive ||
        (NeedsFetching || _userSettings.FullSetThreshold - GlobalItemSetManagerState.BootsAmount > 0);

    public bool WeaponsActive =>
        _userSettings.LootFilterWeaponsAlwaysActive ||
        (NeedsFetching || (_userSettings.FullSetThreshold * 2) - (GlobalItemSetManagerState.WeaponsSmallAmount + (GlobalItemSetManagerState.WeaponsBigAmount * 2)) > 0);

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
            var setThreshold = _userSettings.FullSetThreshold;

            // have to do a bit of wizardry because we store the selected tab indices as a string in the user settings
            var filteredStashContents = new List<EnhancedItem>();

            // reset item amounts before fetching new data
            // invalidate some outdated state for our item manager
            GlobalItemSetManagerState.ResetCompletedSetCount();
            GlobalItemSetManagerState.ResetItemAmounts();

            // update the stash tab metadata based on your target stash
            var stashTabMetadataList = !_userSettings.LegacyAuthMode
                ? await _apiService.GetAllPersonalStashTabMetadataWithOAuthAsync()
                : _userSettings.GuildStashMode
                    ? await _apiService.GetAllGuildStashTabMetadataWithSessionIdAsync(_userSettings.LegacyAuthSessionId)
                    : await _apiService.GetAllPersonalStashTabMetadataWithSessionIdAsync(_userSettings.LegacyAuthSessionId);

            // 'Flatten' the stash tab structure (unwrap children tabs from folders)
            var flattenedStashTabs = GlobalItemSetManagerState.FlattenStashTabs(stashTabMetadataList);

            List<string> selectedTabIds = [];

            if (_userSettings.StashTabQueryMode == (int)StashTabQueryMode.TabsById)
            {
                if (_userSettings.StashTabIds.Count == 0)
                {
                    FetchButtonEnabled = true;

                    GlobalErrorHandler.Spawn(
                        "It looks like you haven't selected any stash tab ids. Please navigate to the 'General > General > Select Stash Tabs' setting and select some tabs, and try again.",
                        "Error: Set Tracker Overlay - Fetch Data"
                    );

                    return false;
                }

                selectedTabIds = [.. _userSettings.StashTabIds];
            }
            else if (_userSettings.StashTabQueryMode == (int)StashTabQueryMode.TabsByNamePrefix)
            {

                if (string.IsNullOrWhiteSpace(_userSettings.StashTabPrefix))
                {
                    FetchButtonEnabled = true;

                    GlobalErrorHandler.Spawn(
                        "It looks like you haven't entered a stash tab prefix. Please navigate to the 'General > General > Stash Tab Prefix' setting and enter a valid value, and try again.",
                        "Error: Set Tracker Overlay - Fetch Data"
                    );

                    return false;
                }

                selectedTabIds = flattenedStashTabs
                    .Where(st => st.Name.StartsWith(_userSettings.StashTabPrefix))
                    .Select(st => st.Id)
                    .ToList();
            }

            if (flattenedStashTabs is not null)
            {
                GlobalItemSetManagerState.UpdateStashMetadata(flattenedStashTabs);

                foreach (var id in selectedTabIds)
                {
                    UnifiedStashTabContents rawResults;

                    // OAuth endpoint uses tab ID for lookup
                    if (!_userSettings.LegacyAuthMode)
                    {
                        rawResults = await _apiService.GetPersonalStashTabContentsByStashIdWithOAuthAsync(id);
                    }
                    // Session ID endpoint uses tab index for lookup - so we 'extract' the index from the tab collection constructed using id's
                    else
                    {
                        // For SessionId auth, we need to find the index corresponding to this id
                        var stashTab = flattenedStashTabs.FirstOrDefault(st => st.Id == id);

                        if (stashTab == null)
                        {
                            continue; // Skip this tab if we can't find its metadata
                        }

                        if (_userSettings.GuildStashMode)
                        {
                            rawResults = await _apiService.GetGuildStashTabContentsByIndexWithSessionIdAsync(
                                _userSettings.LegacyAuthSessionId,
                                stashTab.Id,
                                stashTab.Name,
                                stashTab.Index,
                                stashTab.Type
                            );
                        }
                        else
                        {
                            rawResults = await _apiService.GetPersonalStashTabContentsByIndexWithSessionIdAsync(
                                _userSettings.LegacyAuthSessionId,
                                stashTab.Id,
                                stashTab.Name,
                                stashTab.Index,
                                stashTab.Type
                            );
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
                GlobalItemSetManagerState.GenerateItemSets(!_userSettings.ChaosRecipeTrackingEnabled);

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
            if (FullSets >= _userSettings.FullSetThreshold)
            {
                // if the user has vendor sets early enabled, we don't want to show the warning message
                if (!_userSettings.VendorSetsEarly || FullSets >= _userSettings.FullSetThreshold)
                {
                    WarningMessage = !_userSettings.SilenceSetsFullMessage
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
            else if ((FullSets < _userSettings.FullSetThreshold || _userSettings.VendorSetsEarly) && FullSets >= 1)
            {
                WarningMessage = string.Empty;

                // stash button is disabled with warning tooltip to change threshold
                if (_userSettings.VendorSetsEarly)
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

            // case 3: user has fetched and needs items for chaos recipe (needs more lower level items)
            else if (NeedsLowerLevel && _userSettings.ChaosRecipeTrackingEnabled)
            {
                WarningMessage = !_userSettings.SilenceNeedItemsMessage
                    ? NeedsLowerLevelText(FullSets - _userSettings.FullSetThreshold)
                    : string.Empty;

                // stash button is disabled with conditional tooltip enabled
                // based on whether or not the user has at least 1 set
                StashButtonTooltipEnabled = FullSets >= 1;
                SetsTooltipEnabled = true;
            }

            // case 4: user has fetched and has no sets
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
        var itemClassAmounts = GlobalItemSetManagerState.RetrieveCurrentItemCountsForFilterManipulation();

        // hash set of missing item classes (e.g. "ring", "amulet", etc.)
        var missingItemClasses = new HashSet<string>();

        // first we check weapons since they're special and 2 item classes count for one
        var oneHandedWeaponCount = itemClassAmounts
            .Where(dict => dict.ContainsKey(ItemClass.OneHandWeapons))
            .Select(dict => dict[ItemClass.OneHandWeapons])
            .FirstOrDefault();

        var twoHandedWeaponCount = itemClassAmounts
            .Where(dict => dict.ContainsKey(ItemClass.TwoHandWeapons))
            .Select(dict => dict[ItemClass.TwoHandWeapons])
            .FirstOrDefault();

        if (oneHandedWeaponCount / 2 + twoHandedWeaponCount >= _userSettings.FullSetThreshold)
        {
            foreach (var dict in itemClassAmounts)
            {
                dict.Remove(ItemClass.OneHandWeapons);
                dict.Remove(ItemClass.TwoHandWeapons);
            }
        }

        foreach (var itemCountByClass in itemClassAmounts)
        {
            foreach (var itemClass in itemCountByClass)
            {
                if (itemClass.Value < _userSettings.FullSetThreshold)
                {
                    missingItemClasses.Add(itemClass.Key.ToString());
                }
            }
        }

        await _filterManipulationService.GenerateSectionsAndUpdateFilterAsync(missingItemClasses);
        _reloadFilterService.ReloadFilter();
    }

    #endregion

    #region Utility Methods

    private static string NeedsLowerLevelText(int diff) => $"Need {Math.Abs(diff)} items with iLvl 60-74!";

    private void UpdateDisplay()
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
        _notificationSoundService.PlayNotificationSound(NotificationSoundType.ItemSetStateChanged);
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