using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using ChaosRecipeEnhancer.UI.State;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.Windows;

internal sealed class SetTrackerOverlayViewModel : ViewModelBase
{
    private readonly IFilterManipulationService _filterManipulationService = Ioc.Default.GetRequiredService<IFilterManipulationService>();
    private readonly IItemSetManagerService _itemSetManagerService = Ioc.Default.GetRequiredService<IItemSetManagerService>();
    private readonly IReloadFilterService _reloadFilterService = Ioc.Default.GetRequiredService<IReloadFilterService>();
    private readonly IApiService _apiService = Ioc.Default.GetRequiredService<IApiService>();

    private const string SetsFullText = "Sets full!";
    private const string NeedsLowerLevelText = "Need lower level items for recipe!";

    private const int FetchCooldown = 30;

    private bool _fetchButtonEnabled = true;
    private string _warningMessage;

    public bool FetchButtonEnabled
    {
        get => _fetchButtonEnabled;
        set => SetProperty(ref _fetchButtonEnabled, value);
    }

    public string WarningMessage
    {
        get => _warningMessage;
        set => SetProperty(ref _warningMessage, value);
    }

    public async Task<bool> FetchDataAsync()
    {
        WarningMessage = string.Empty;
        FetchButtonEnabled = false;

        try
        {
            // needed to update item set manager
            var setThreshold = Settings.FullSetThreshold;

            // have to do a bit of wizardry because we store the selected tab indices as a string in the user settings
            var selectedTabIndices = Settings.StashTabIndices.Split(',').ToList().Select(int.Parse).ToList();

            var filteredStashContents = new List<EnhancedItem>();
            var includeIdentified = Settings.IncludeIdentifiedItemsEnabled;
            var chaosRecipe = Settings.ChaosRecipeTrackingEnabled;

            // reset item amounts before fetching new data
            // invalidate some outdated state for our item manager
            _itemSetManagerService.ResetCompletedSets();
            _itemSetManagerService.ResetItemAmounts();

            // update the stash tab metadata based on your target stash
            var stashTabMetadataList = await _apiService.GetAllPersonalStashTabMetadataAsync();

            _itemSetManagerService.UpdateStashMetadata(stashTabMetadataList);

            // Create a new dictionary for stash index and ID pairs
            var selectedStashIndexIdPairs = new Dictionary<int, string>();

            // Map indices to stash IDs
            foreach (var index in selectedTabIndices)
            {
                var stashTab = stashTabMetadataList.StashTabs.FirstOrDefault(st => st.Index == index);
                if (stashTab != null)
                {
                    selectedStashIndexIdPairs.Add(index, stashTab.Id);
                }
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
                filteredStashContents.AddRange(
                    EnhancedItemHelper.FilterItemsForRecipe(enhancedItems, includeIdentified, chaosRecipe));

                _itemSetManagerService.UpdateData(
                    setThreshold,
                    selectedTabIndices,
                    filteredStashContents,
                    includeIdentified,
                    chaosRecipe
                );

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
            _itemSetManagerService.CalculateItemAmounts();
            _itemSetManagerService.GenerateItemSets(chaosRecipe);

            // update the UI accordingly
            UpdateDisplay();
            UpdateNotificationMessage();

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
        catch (FormatException)
        {
            FetchButtonEnabled = true;
            ErrorWindow.Spawn("It looks like you haven't selected any stash tab indices. Please navigate to the 'General > General > Select Stash Tabs' setting and select some tabs, and try again.", "Error: Set Tracker Overlay - Fetch Data");
            return false;
        }
        catch (NullReferenceException)
        {
            FetchButtonEnabled = true;
            GlobalAuthState.Instance.PurgeLocalAuthToken();
            ErrorWindow.Spawn("It looks like your credentials have expired. Please log back in to continue.", "Error: Set Tracker Overlay - Fetch Data");
            return false;
        }

        return true;
    }

    public void UpdateNotificationMessage()
    {
        // update the warning message (notifications based on status)
        // usually "Sets Full!"; sometimes rate limit warnings
        if (NeedsFetching)
            WarningMessage = string.Empty;
        else if (!NeedsFetching && FullSets >= Settings.FullSetThreshold)
            WarningMessage = SetsFullText;
        else if (!NeedsFetching && NeedsLowerLevel)
            WarningMessage = NeedsLowerLevelText;
        else if (WarningMessage == SetsFullText)
            WarningMessage = string.Empty;
    }

    public void RunReloadFilter()
    {
        // hash set of missing item classes (e.g. "ring", "amulet", etc.)
        var sets = _itemSetManagerService.RetrieveSetsInProgress();
        var needChaosItems = sets.Any(set => !set.HasRecipeQualifier);
        var missingItemClasses = new HashSet<string>();

        foreach (var set in sets)
        {
            foreach (var item in set.EmptyItemSlots)
            {
                missingItemClasses.Add(item);
            }
        }

        _filterManipulationService.GenerateSectionsAndUpdateFilterAsync(missingItemClasses, needChaosItems);
        _reloadFilterService.ReloadFilter();
    }

    private bool ShowAmountNeeded => Settings.SetTrackerOverlayItemCounterDisplayMode == 2;
    public bool NeedsFetching => _itemSetManagerService.RetrieveNeedsFetching();
    public bool NeedsLowerLevel => _itemSetManagerService.RetrieveNeedsLowerLevel();
    public int FullSets => _itemSetManagerService.RetrieveCompletedSetCount();

    #region Item Amount and Visibility Properties

    public int RingsAmount => ShowAmountNeeded ? Math.Max((Settings.FullSetThreshold * 2) - _itemSetManagerService.RetrieveRingsAmount(), 0) : _itemSetManagerService.RetrieveRingsAmount();
    public bool RingsActive => Settings.LootFilterRingsAlwaysActive || (NeedsFetching || (Properties.Settings.Default.FullSetThreshold * 2) - _itemSetManagerService.RetrieveRingsAmount() > 0);

    public int AmuletsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveAmuletsAmount(), 0) : _itemSetManagerService.RetrieveAmuletsAmount();
    public bool AmuletsActive => Settings.LootFilterAmuletsAlwaysActive || (NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveAmuletsAmount() > 0);

    public int BeltsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveBeltsAmount(), 0) : _itemSetManagerService.RetrieveBeltsAmount();
    public bool BeltsActive => Settings.LootFilterBeltsAlwaysActive || (NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveBeltsAmount() > 0);

    public int ChestsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveChestsAmount(), 0) : _itemSetManagerService.RetrieveChestsAmount();
    public bool ChestsActive => Settings.LootFilterBodyArmourAlwaysActive || (NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveChestsAmount() > 0);

    public int WeaponsAmount => ShowAmountNeeded ? Math.Max((Properties.Settings.Default.FullSetThreshold * 2) - (_itemSetManagerService.RetrieveWeaponsSmallAmount() + (_itemSetManagerService.RetrieveWeaponsBigAmount() * 2)), 0) : _itemSetManagerService.RetrieveWeaponsSmallAmount() + (_itemSetManagerService.RetrieveWeaponsBigAmount() * 2);
    public bool WeaponsActive => Settings.LootFilterWeaponsAlwaysActive || (NeedsFetching || (Properties.Settings.Default.FullSetThreshold * 2) - (_itemSetManagerService.RetrieveWeaponsSmallAmount() + (_itemSetManagerService.RetrieveWeaponsBigAmount() * 2)) > 0);

    public int GlovesAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveGlovesAmount(), 0) : _itemSetManagerService.RetrieveGlovesAmount();
    public bool GlovesActive => Settings.LootFilterGlovesAlwaysActive || (NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveGlovesAmount() > 0);

    public int HelmetsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveHelmetsAmount(), 0) : _itemSetManagerService.RetrieveHelmetsAmount();
    public bool HelmetsActive => Settings.LootFilterHelmetsAlwaysActive || (NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveHelmetsAmount() > 0);

    public int BootsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveBootsAmount(), 0) : _itemSetManagerService.RetrieveBootsAmount();
    public bool BootsActive => Settings.LootFilterBootsAlwaysActive || (NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveBootsAmount() > 0);

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