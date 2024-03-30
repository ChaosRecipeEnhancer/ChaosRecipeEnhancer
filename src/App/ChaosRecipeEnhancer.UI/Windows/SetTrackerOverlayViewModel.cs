using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Models.Exceptions;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using ChaosRecipeEnhancer.UI.UserControls;
using ChaosRecipeEnhancer.UI.Utilities;
using CommunityToolkit.Mvvm.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    private const int FetchCooldown = 30;

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
            var setThreshold = GlobalUserSettings.FullSetThreshold;

            // have to do a bit of wizardry because we store the selected tab indices as a string in the user settings
            var filteredStashContents = new List<EnhancedItem>();

            // reset item amounts before fetching new data
            // invalidate some outdated state for our item manager
            GlobalItemSetManagerState.ResetCompletedSetCount();
            GlobalItemSetManagerState.ResetItemAmounts();

            // update the stash tab metadata based on your target stash
            var stashTabMetadataList = GlobalItemSetManagerState.FlattenStashTabs(await _apiService.GetAllPersonalStashTabMetadataAsync());

            // the following regions will need to be cleaned up and refactored at some point becaues this method is too long

            #region OPERATIONS THAT REQUIRE STASH TAB INDEXES

            // if the user has selected stash tabs by index or by tab name prefix (which also uses indices)
            if (GlobalUserSettings.StashTabQueryMode == (int)StashTabQueryMode.SelectTabsByIndex ||
                GlobalUserSettings.StashTabQueryMode == (int)StashTabQueryMode.TabNamePrefix)
            {
                List<int> selectedTabIndices = [];
                if (GlobalUserSettings.StashTabQueryMode == (int)StashTabQueryMode.SelectTabsByIndex)
                {
                    if (string.IsNullOrWhiteSpace(GlobalUserSettings.StashTabIndices))
                    {
                        FetchButtonEnabled = true;

                        GlobalErrorHandler.Spawn(
                            "It looks like you haven't selected any stash tab indices. Please navigate to the 'General > General > Select Stash Tabs' setting and select some tabs, and try again.",
                            "Error: Set Tracker Overlay - Fetch Data"
                        );

                        return false;
                    }

                    selectedTabIndices = GlobalUserSettings.StashTabIndices.Split(',').ToList().Select(int.Parse).ToList();
                }
                else if (GlobalUserSettings.StashTabQueryMode == (int)StashTabQueryMode.TabNamePrefix)
                {
                    if (string.IsNullOrWhiteSpace(GlobalUserSettings.StashTabPrefix))
                    {
                        FetchButtonEnabled = true;

                        GlobalErrorHandler.Spawn(
                            "It looks like you haven't entered a stash tab prefix. Please navigate to the 'General > General > Stash Tab Prefix' setting and enter a valid value, and try again.",
                            "Error: Set Tracker Overlay - Fetch Data"
                        );

                        return false;
                    }

                    selectedTabIndices = stashTabMetadataList
                        .Where(st => st.Name.StartsWith(GlobalUserSettings.StashTabPrefix))
                        .Select(st => st.Index)
                        .ToList();

                    GlobalUserSettings.StashTabPrefixIndices = string.Join(',', selectedTabIndices);
                    GlobalUserSettings.Save();
                }

                if (stashTabMetadataList is not null)
                {
                    GlobalItemSetManagerState.UpdateStashMetadata(stashTabMetadataList);

                    // Create a new dictionary for stash index and ID pairs
                    var selectedStashIndexIdPairs = new Dictionary<int, string>();

                    try
                    {
                        // Map indices to stash IDs
                        foreach (var index in selectedTabIndices)
                        {
                            var stashTab = stashTabMetadataList.FirstOrDefault(st => st.Index == index);
                            if (stashTab != null)
                            {
                                selectedStashIndexIdPairs.Add(index, stashTab.Id);
                            }
                        }
                    }
                    // there are few reports of users attempting to add items with duplicate keys
                    // in this case it's attempting to add stash tabs with the same index
                    // this is not allowed, and is caused by stash metadata being out of sync
                    // therefore, rethrow the exception and let the user know to re-fetch their tabs
                    catch (ArgumentException)
                    {
                        throw new ArgumentNullException();
                    }

                    foreach (var (index, id) in selectedStashIndexIdPairs)
                    {
                        // first we retrieve the 'raw' results from the API
                        var rawResults = await _apiService.GetPersonalStashTabContentsByStashIdAsync(id);

                        // then we convert the raw results into a list of EnhancedItem objects
                        var enhancedItems = rawResults.Stash.Items.Select(item => new EnhancedItem(item)).ToList();

                        // Manually setting index because we need to know which tab the item came from
                        foreach (var enhancedItem in enhancedItems)
                        {
                            enhancedItem.StashTabId = id; // we will use the id later
                            enhancedItem.StashTabIndex = index; // Now 'index' refers to the correct stash tab index
                        }

                        // add the enhanced items to the filtered stash contents
                        filteredStashContents.AddRange(EnhancedItemUtilities.FilterItemsForRecipe(enhancedItems));

                        GlobalItemSetManagerState.UpdateStashContentsByIndex(setThreshold, selectedTabIndices, filteredStashContents);

                        if (GlobalRateLimitState.RateLimitExceeded)
                        {
                            WarningMessage = "Rate Limit Exceeded! Selecting less tabs may help. Waiting...";

                            await Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
                            {
                                await Task.Delay(GlobalRateLimitState.GetSecondsToWait() * 1000);

                                GlobalRateLimitState.RequestCounter = 0;
                                GlobalRateLimitState.RateLimitExceeded = false;
                            });
                        }
                        else if (GlobalRateLimitState.BanTime > 0)
                        {
                            WarningMessage = "Temporary Ban from API Requests! Waiting...";

                            await Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
                            {
                                await Task.Delay(GlobalRateLimitState.BanTime * 1000);

                                GlobalRateLimitState.BanTime = 0;
                            });
                        }
                    }

                    // recalculate item amounts and generate item sets after fetching from api
                    GlobalItemSetManagerState.CalculateItemAmounts();

                    // generate item sets for the chosen recipe (chaos or regal)
                    GlobalItemSetManagerState.GenerateItemSets(!GlobalUserSettings.ChaosRecipeTrackingEnabled);

                    // update the UI accordingly
                    UpdateDisplay();
                    UpdateStashButtonAndWarningMessage();

                    // enforce cooldown on fetch button to reduce chances of rate limiting
                    await Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
                    {
                        try
                        {
                            await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
                        }
                        finally
                        {
                            FetchButtonEnabled = true;
                        }
                    });
                }
                else
                {
                    FetchButtonEnabled = true;
                    return false;
                }

            }

            #endregion

            #region OPERATIONS THAT REQUIRE STASH TAB IDS

            // if the user has selected stash tabs by id we have to do things differently
            else if (GlobalUserSettings.StashTabQueryMode == (int)StashTabQueryMode.SelectTabsById)
            {
                List<string> selectedTabIds = [];
                if (GlobalUserSettings.StashTabQueryMode == (int)StashTabQueryMode.SelectTabsById)
                {
                    if (string.IsNullOrWhiteSpace(GlobalUserSettings.StashTabIdentifiers))
                    {
                        FetchButtonEnabled = true;

                        GlobalErrorHandler.Spawn(
                            "It looks like you haven't selected any stash tab ids. Please navigate to the 'General > General > Select Stash Tabs' setting and select some tabs, and try again.",
                            "Error: Set Tracker Overlay - Fetch Data"
                        );

                        return false;
                    }

                    selectedTabIds = GlobalUserSettings.StashTabIdentifiers.Split(',').ToList();
                }

                if (stashTabMetadataList is not null)
                {
                    GlobalItemSetManagerState.UpdateStashMetadata(stashTabMetadataList);

                    foreach (var id in selectedTabIds)
                    {
                        // first we retrieve the 'raw' results from the API using the stash id
                        var rawResults = await _apiService.GetPersonalStashTabContentsByStashIdAsync(id);

                        // then we convert the raw results into a list of EnhancedItem objects
                        var enhancedItems = rawResults.Stash.Items.Select(item => new EnhancedItem(item)).ToList();

                        // Manually setting id because we need to know which tab the item came from
                        foreach (var enhancedItem in enhancedItems)
                        {
                            enhancedItem.StashTabId = id;
                        }

                        // add the enhanced items to the filtered stash contents
                        filteredStashContents.AddRange(EnhancedItemUtilities.FilterItemsForRecipe(enhancedItems));

                        GlobalItemSetManagerState.UpdateStashContentsById(setThreshold, selectedTabIds, filteredStashContents);

                        if (GlobalRateLimitState.RateLimitExceeded)
                        {
                            WarningMessage = "Rate Limit Exceeded! Selecting less tabs may help. Waiting...";

                            await Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
                            {
                                await Task.Delay(GlobalRateLimitState.GetSecondsToWait() * 1000);

                                GlobalRateLimitState.RequestCounter = 0;
                                GlobalRateLimitState.RateLimitExceeded = false;
                            });
                        }
                        else if (GlobalRateLimitState.BanTime > 0)
                        {
                            WarningMessage = "Temporary Ban from API Requests! Waiting...";

                            await Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
                            {
                                await Task.Delay(GlobalRateLimitState.BanTime * 1000);

                                GlobalRateLimitState.BanTime = 0;
                            });
                        }
                    }

                    // recalculate item amounts and generate item sets after fetching from api
                    GlobalItemSetManagerState.CalculateItemAmounts();

                    // generate item sets for the chosen recipe (chaos or regal)
                    GlobalItemSetManagerState.GenerateItemSets(!GlobalUserSettings.ChaosRecipeTrackingEnabled);

                    // update the UI accordingly
                    UpdateDisplay();
                    UpdateStashButtonAndWarningMessage();

                    // enforce cooldown on fetch button to reduce chances of rate limiting
                    await Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
                    {
                        try
                        {
                            await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
                        }
                        finally
                        {
                            FetchButtonEnabled = true;
                        }
                    });

                }
                else
                {
                    FetchButtonEnabled = true;
                    return false;
                }
            }

            #endregion
        }
        catch (RateLimitException e)
        {
            FetchButtonEnabled = false;

            // Cooldown the refresh button until the rate limit is lifted
            await Dispatcher.CurrentDispatcher.InvokeAsync(async () =>
            {
                await Task.Factory.StartNew(() => Thread.Sleep(e.SecondsToWait * 1000));
                FetchButtonEnabled = true;
            });


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
        catch (ArgumentNullException e)
        {
            FetchButtonEnabled = true;

            if (string.IsNullOrWhiteSpace(GlobalUserSettings.StashTabIndices))
            {
                GlobalErrorHandler.Spawn(
                    e.ToString(),
                    "Error: Set Tracker Overlay - No Tabs Selected",
                    preamble: "It looks like you haven't selected any stash tab indices. Please navigate to" +
                    "the 'General > General > Select Stash Tabs' setting and select some tabs, and try again."
                );

                return false;
            }
            else
            {
                GlobalErrorHandler.Spawn(
                    e.ToString(),
                    "Error: Set Tracker Overlay - Selected Tabs Out Of Sync",
                    preamble: "It looks like your currently selected stash tabs are out of sync.\n\n" +
                    "You may have moved them or modified them in some way that made us unable " +
                    "to determine which stash tab you meant to select.\n\nPlease navigate to " +
                    "the 'General > Select Stash Tabs', re-fetch your tabs, and validate your selections."
                );
            }

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
            if (FullSets >= GlobalUserSettings.FullSetThreshold)
            {
                // if the user has vendor sets early enabled, we don't want to show the warning message
                if (!GlobalUserSettings.VendorSetsEarly || FullSets >= GlobalUserSettings.FullSetThreshold)
                {
                    WarningMessage = !GlobalUserSettings.SilenceSetsFullMessage
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
            else if ((FullSets < GlobalUserSettings.FullSetThreshold || GlobalUserSettings.VendorSetsEarly) && FullSets >= 1)
            {
                WarningMessage = string.Empty;

                // stash button is disabled with warning tooltip to change threshold
                if (GlobalUserSettings.VendorSetsEarly)
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
            else if (NeedsLowerLevel && GlobalUserSettings.ChaosRecipeTrackingEnabled)
            {
                WarningMessage = !GlobalUserSettings.SilenceNeedItemsMessage
                    ? NeedsLowerLevelText(FullSets - GlobalUserSettings.FullSetThreshold)
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

        if (oneHandedWeaponCount / 2 + twoHandedWeaponCount >= GlobalUserSettings.FullSetThreshold)
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
                if (itemClass.Value < GlobalUserSettings.FullSetThreshold)
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

    #endregion
}