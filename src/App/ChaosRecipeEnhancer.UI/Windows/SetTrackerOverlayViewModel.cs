using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.Windows;

internal sealed class SetTrackerOverlayViewModel : ViewModelBase
{
    #region Fields

    private readonly IFilterManipulationService _filterManipulationService = Ioc.Default.GetRequiredService<IFilterManipulationService>();
    private readonly IReloadFilterService _reloadFilterService = Ioc.Default.GetRequiredService<IReloadFilterService>();
    private readonly IApiService _apiService = Ioc.Default.GetRequiredService<IApiService>();

    private const string SetsFullText = "Sets full!";
    private const int FetchCooldown = 30;

    private bool _fetchButtonEnabled = true;
    private bool _stashButtonEnabled = false;
    private bool _stashButtonTooltipEnabled = false;
    private bool _setsTooltipEnabled = false;
    private string _warningMessage;

    #endregion

    #region Properties

    public bool FetchButtonEnabled
    {
        get => _fetchButtonEnabled;
        set => SetProperty(ref _fetchButtonEnabled, value);
    }

    public bool StashButtonEnabled
    {
        get => _stashButtonEnabled;
        set => SetProperty(ref _stashButtonEnabled, value);
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

    public async Task<bool> FetchStashDataAsync()
    {
        WarningMessage = string.Empty;
        FetchButtonEnabled = false;
        StashButtonEnabled = false;

        try
        {
            // needed to update item set manager
            var setThreshold = Settings.FullSetThreshold;

            // have to do a bit of wizardry because we store the selected tab indices as a string in the user settings
            var filteredStashContents = new List<EnhancedItem>();

            // reset item amounts before fetching new data
            // invalidate some outdated state for our item manager
            GlobalItemSetManagerState.ResetCompletedSetCount();
            GlobalItemSetManagerState.ResetItemAmounts();

            // update the stash tab metadata based on your target stash
            var stashTabMetadataList = GlobalItemSetManagerState.FlattenStashTabs(await _apiService.GetAllPersonalStashTabMetadataAsync());

            List<int> selectedTabIndices = new();
            if (Settings.StashTabQueryMode == (int)StashTabQueryMode.SelectTabsFromList)
            {
                if (string.IsNullOrWhiteSpace(Settings.StashTabIndices))
                {
                    FetchButtonEnabled = true;

                    ErrorWindow.Spawn(
                        "It looks like you haven't selected any stash tab indices. Please navigate to the 'General > General > Select Stash Tabs' setting and select some tabs, and try again.",
                        "Error: Set Tracker Overlay - Fetch Data"
                    );

                    return false;
                }

                selectedTabIndices = Settings.StashTabIndices.Split(',').ToList().Select(int.Parse).ToList();
            }
            else if (Settings.StashTabQueryMode == (int)StashTabQueryMode.TabNamePrefix)
            {
                if (string.IsNullOrWhiteSpace(Settings.StashTabPrefix))
                {
                    FetchButtonEnabled = true;

                    ErrorWindow.Spawn(
                        "It looks like you haven't entered a stash tab prefix. Please navigate to the 'General > General > Stash Tab Prefix' setting and enter a valid value, and try again.",
                        "Error: Set Tracker Overlay - Fetch Data"
                    );

                    return false;
                }

                selectedTabIndices = stashTabMetadataList
                    .Where(st => st.Name.StartsWith(Settings.StashTabPrefix))
                    .Select(st => st.Index)
                    .ToList();

                Settings.StashTabPrefixIndices = string.Join(',', selectedTabIndices);
                Settings.Save();
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
                        enhancedItem.StashTabIndex = index; // Now 'index' refers to the correct stash tab index
                    }

                    // add the enhanced items to the filtered stash contents
                    filteredStashContents.AddRange(EnhancedItemHelper.FilterItemsForRecipe(enhancedItems));

                    GlobalItemSetManagerState.UpdateStashContents(setThreshold, selectedTabIndices, filteredStashContents);

                    if (GlobalRateLimitState.RateLimitExceeded)
                    {
                        WarningMessage = "Rate Limit Exceeded! Selecting less tabs may help. Waiting...";
                        await Task.Delay(GlobalRateLimitState.GetSecondsToWait() * 1000);
                        GlobalRateLimitState.RequestCounter = 0;
                        GlobalRateLimitState.RateLimitExceeded = false;
                    }
                    else if (GlobalRateLimitState.BanTime > 0)
                    {
                        WarningMessage = "Temporary Ban from API Requests! Waiting...";
                        await Task.Delay(GlobalRateLimitState.BanTime * 1000);
                        GlobalRateLimitState.BanTime = 0;
                    }
                }

                // recalculate item amounts and generate item sets after fetching from api
                GlobalItemSetManagerState.CalculateItemAmounts();
                GlobalItemSetManagerState.GenerateItemSets();

                // update the UI accordingly
                UpdateDisplay();
                UpdateStashButtonAndWarningMessage();

                // enforce cooldown on fetch button to reduce chances of rate limiting
                try
                {
                    await Task.Factory.StartNew(() => Thread.Sleep(FetchCooldown * 1000));
                }
                finally
                {
                    FetchButtonEnabled = true;
                }
            }
            else
            {
                FetchButtonEnabled = true;
                return false;
            }
        }
        catch (NullReferenceException)
        {
            FetchButtonEnabled = true;
            GlobalAuthState.Instance.PurgeLocalAuthToken();
            ErrorWindow.Spawn(
                "It looks like your credentials have expired. Please log back in to continue.",
                "Error: Set Tracker Overlay - Fetch Data"
            );
            return false;
        }
        catch (ArgumentNullException)
        {
            FetchButtonEnabled = true;

            if (string.IsNullOrWhiteSpace(Settings.StashTabIndices))
            {
                ErrorWindow.Spawn(
                    "It looks like you haven't selected any stash tab indices. Please navigate to the 'General > General > Select Stash Tabs' setting and select some tabs, and try again.",
                    "Error: Set Tracker Overlay - Fetch Data"
                );

                return false;
            }
            else
            {
                ErrorWindow.Spawn(
                    "It looks like your currently selected stash tabs are out of sync.\n\n" +
                    "You may have moved them or modified them in some way that made us unable " +
                    "to determine which stash tab you meant to select.\n\nPlease navigate to " +
                    "the 'General > Select Stash Tabs', re-fetch your tabs, and validate your selections.",
                    "Error: Set Tracker Overlay - Fetch Data"
                );
            }

            return false;
        }

        return true;
    }

    public void UpdateStashButtonAndWarningMessage()
    {
        // case 1: user just opened the app, hasn't hit fetch yet
        if (NeedsFetching)
        {
            WarningMessage = string.Empty;
        }
        else if (!NeedsFetching)
        {
            // case 2: user fetched data and has enough sets to turn in based on their threshold
            if (FullSets >= Settings.FullSetThreshold)
            {
                // if the user has vendor sets early enabled, we don't want to show the warning message
                if (!Settings.VendorSetsEarly || FullSets >= Settings.FullSetThreshold)
                {
                    if (!Settings.SilenceSetsFullMessage)
                    {
                    WarningMessage = SetsFullText;
                }
                }

                // stash button is enabled with no warning tooltip
                StashButtonEnabled = true;
                StashButtonTooltipEnabled = false;
                SetsTooltipEnabled = false;
            }

            // case 3: user fetched data and has at least 1 set, but not to their full threshold
            else if ((FullSets < Settings.FullSetThreshold || Settings.VendorSetsEarly) && FullSets >= 1)
            {
                WarningMessage = string.Empty;

                // stash button is disabled with warning tooltip to change threshold
                if (Settings.VendorSetsEarly)
                {
                    StashButtonEnabled = true;
                    StashButtonTooltipEnabled = false;
                    SetsTooltipEnabled = false;
                }
                else
                {
                    StashButtonEnabled = false;
                    StashButtonTooltipEnabled = true;
                    SetsTooltipEnabled = true;
                }
            }

            // case 3: user has fetched and needs items for chaos recipe (needs more lower level items)
            // this one doesn't work as expected
            else if (NeedsLowerLevel)
            {
                if (!Settings.SilenceSetsFullMessage)
                {
                WarningMessage = NeedsLowerLevelText(FullSets - Settings.FullSetThreshold);
                }

                // stash button is disabled with conditional tooltip enabled
                // based on whether or not the user has at least 1 set
                StashButtonEnabled = false;
                StashButtonTooltipEnabled = FullSets >= 1;
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

        if (oneHandedWeaponCount / 2 + twoHandedWeaponCount >= Settings.FullSetThreshold)
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
                if (itemClass.Value < Settings.FullSetThreshold)
                {
                    missingItemClasses.Add(itemClass.Key.ToString());
                }
            }
        }

        await _filterManipulationService.GenerateSectionsAndUpdateFilterAsync(missingItemClasses);
        _reloadFilterService.ReloadFilter();
    }

    private string NeedsLowerLevelText(int diff) => $"Need {Math.Abs(diff)} items with iLvl 60-74!";
    private bool ShowAmountNeeded => Settings.SetTrackerOverlayItemCounterDisplayMode == 2;
    public bool NeedsFetching => GlobalItemSetManagerState.NeedsFetching;
    public bool NeedsLowerLevel => GlobalItemSetManagerState.NeedsLowerLevel;
    public int FullSets => GlobalItemSetManagerState.CompletedSetCount;

    #region Item Amount and Visibility Properties

    #region Item Amount Properties

    public int RingsAmount => ShowAmountNeeded
        // case where we are showing missing items (calculate total needed and subtract from threshold, but don't show negatives)
        ? Math.Max((Settings.FullSetThreshold * 2) - GlobalItemSetManagerState.RingsAmount, 0)
        // case where we are showing total item sets (e.g. pair of rings as a single 'count')
        : GlobalItemSetManagerState.RingsAmount / 2;

    public int AmuletsAmount =>
        ShowAmountNeeded
            ? Math.Max(Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.AmuletsAmount, 0)
            : GlobalItemSetManagerState.AmuletsAmount;

    public int BeltsAmount =>
        ShowAmountNeeded
            ? Math.Max(Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.BeltsAmount, 0)
            : GlobalItemSetManagerState.BeltsAmount;

    public int ChestsAmount => ShowAmountNeeded
        ? Math.Max(Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.ChestsAmount, 0)
        : GlobalItemSetManagerState.ChestsAmount;

    public int GlovesAmount =>
        ShowAmountNeeded
            ? Math.Max(Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.GlovesAmount, 0)
            : GlobalItemSetManagerState.GlovesAmount;

    public int HelmetsAmount =>
        ShowAmountNeeded
            ? Math.Max(Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.HelmetsAmount, 0)
            : GlobalItemSetManagerState.HelmetsAmount;

    public int WeaponsAmount => ShowAmountNeeded
        // case where we are showing missing items (calculate total needed and subtract from threshold, but don't show negatives)
        ? Math.Max((Properties.Settings.Default.FullSetThreshold * 2) - (GlobalItemSetManagerState.WeaponsSmallAmount + (GlobalItemSetManagerState.WeaponsBigAmount * 2)), 0)
        // case where we are showing total weapon sets (e.g. pair of one handed weapons plus two handed weapons as a 'count' each)
        : (GlobalItemSetManagerState.WeaponsSmallAmount / 2) + GlobalItemSetManagerState.WeaponsBigAmount;

    #endregion

    #region Item Class Active (Visibility) Properties

    public bool RingsActive =>
        Settings.LootFilterRingsAlwaysActive ||
        (NeedsFetching || (Properties.Settings.Default.FullSetThreshold * 2) - GlobalItemSetManagerState.RingsAmount > 0);

    public bool AmuletsActive =>
        Settings.LootFilterAmuletsAlwaysActive ||
        (NeedsFetching || Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.AmuletsAmount > 0);

    public bool BeltsActive =>
        Settings.LootFilterBeltsAlwaysActive ||
        (NeedsFetching || Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.BeltsAmount > 0);

    public bool ChestsActive =>
        Settings.LootFilterBodyArmourAlwaysActive ||
        (NeedsFetching || Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.ChestsAmount > 0);

    public bool GlovesActive =>
        Settings.LootFilterGlovesAlwaysActive ||
        (NeedsFetching || Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.GlovesAmount > 0);

    public bool HelmetsActive =>
        Settings.LootFilterHelmetsAlwaysActive ||
        (NeedsFetching || Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.HelmetsAmount > 0);

    public int BootsAmount =>
        ShowAmountNeeded
            ? Math.Max(Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.BootsAmount, 0)
            : GlobalItemSetManagerState.BootsAmount;

    public bool BootsActive =>
        Settings.LootFilterBootsAlwaysActive ||
        (NeedsFetching || Properties.Settings.Default.FullSetThreshold - GlobalItemSetManagerState.BootsAmount > 0);

    public bool WeaponsActive =>
        Settings.LootFilterWeaponsAlwaysActive ||
        (NeedsFetching || (Properties.Settings.Default.FullSetThreshold * 2) - (GlobalItemSetManagerState.WeaponsSmallAmount + (GlobalItemSetManagerState.WeaponsBigAmount * 2)) > 0);

    #endregion

    #endregion

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
}