using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;
using CommunityToolkit.Mvvm.DependencyInjection;

namespace ChaosRecipeEnhancer.UI.Windows;

internal sealed class SetTrackerOverlayViewModel : ViewModelBase
{
    private readonly IItemSetManagerService _itemSetManagerService = Ioc.Default.GetRequiredService<IItemSetManagerService>();
    private readonly IReloadFilterService _reloadFilterService = Ioc.Default.GetRequiredService<IReloadFilterService>();
    private readonly IApiService _apiService = Ioc.Default.GetRequiredService<IApiService>();

    private const string SetsFullText = "Sets full!";
    private const int FetchCooldown = 30;

    private bool _fetchButtonEnabled = true;
    private bool _showProgress;
    private string _warningMessage;

    public bool ShowProgress
    {
        get => _showProgress;
        set => SetProperty(ref _showProgress, value);
    }

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

    public async void FetchDataAsync()
    {
        WarningMessage = string.Empty;
        ShowProgress = true;
        FetchButtonEnabled = false;

        // needed to craft api request
        var targetStash = (TargetStash)Settings.TargetStash;
        var accountName = Settings.PathOfExileAccountName;
        var leagueName = Settings.LeagueName;
        var secret = Settings.PathOfExileWebsiteSessionId;

        // needed to update item set manager
        var setThreshold = Settings.FullSetThreshold;

        // have to do a bit of wizardry because we store the selected tab indices as a string in the user settings
        var selectedTabIndices = Settings.StashTabIndices.Split(',').ToList().Select(int.Parse).ToList();

        var filteredStashContents = new List<EnhancedItem>();
        var includeIdentified = Settings.IncludeIdentifiedItemsEnabled;
        var chaosRecipe = Settings.ChaosRecipeTrackingEnabled;

        // reset item amounts before fetching new data
        ShowProgress = true;

        // invalidate some outdated state for our item manager
        _itemSetManagerService.ResetCompletedSets();
        _itemSetManagerService.ResetItemAmounts();

        // update the stash tab metadata based on your target stash
        var stashTabMetadataList = targetStash == TargetStash.Personal
            ? await _apiService.GetAllPersonalStashTabMetadataAsync(accountName, leagueName, secret)
            : await _apiService.GetAllGuildStashTabMetadataAsync(accountName, leagueName, secret);

        _itemSetManagerService.UpdateStashMetadata(stashTabMetadataList);

        foreach (var index in selectedTabIndices)
        {
            // first we retrieve the 'raw' results from the API
            var rawResults = targetStash == TargetStash.Personal
                ? await _apiService.GetPersonalStashTabContentsByIndexAsync(accountName, leagueName, index, secret)
                : await _apiService.GetGuildStashTabContentsByIndexAsync(accountName, leagueName, index, secret);

            // then we convert the raw results into a list of EnhancedItem objects
            var enhancedItems = rawResults.Items.Select(item => new EnhancedItem(item)).ToList();

            // add the enhanced items to the filtered stash contents
            filteredStashContents.AddRange(EnhancedItemHelper.FilterItemsForRecipe(enhancedItems, includeIdentified, chaosRecipe));

            _itemSetManagerService.UpdateData(
                setThreshold,
                selectedTabIndices,
                filteredStashContents,
                includeIdentified,
                chaosRecipe
            );

            if (RateLimitManager.RateLimitExceeded)
            {
                WarningMessage = "Rate Limit Exceeded! Selecting less tabs may help. Waiting...";
                await Task.Delay(RateLimitManager.GetSecondsToWait() * 1000);
                RateLimitManager.RequestCounter = 0;
                RateLimitManager.RateLimitExceeded = false;
            }
            else if (RateLimitManager.BanTime > 0)
            {
                WarningMessage = "Temporary Ban from API Requests! Waiting...";
                await Task.Delay(RateLimitManager.BanTime * 1000);
                RateLimitManager.BanTime = 0;
            }
        }

        // recalculate item amounts and generate item sets after fetching from api
        _itemSetManagerService.CalculateItemAmounts();
        _itemSetManagerService.GenerateItemSets(chaosRecipe);

        // update the UI accordingly
        ShowProgress = false;
        UpdateDisplay();
        UpdateNotificationMessage();

        // wait a bit before fetching the next tab
        await Task.Delay(FetchCooldown * 1000); // 30 seconds default fetch cooldown
        FetchButtonEnabled = true;
    }

    public void UpdateNotificationMessage()
    {
        // update the warning message (notifications based on status)
        // usually "Sets Full!"; sometimes rate limit warnings
        if (NeedsFetching)
            WarningMessage = string.Empty;
        else if (!NeedsFetching && FullSets >= Settings.FullSetThreshold)
            WarningMessage = SetsFullText;
        else if (WarningMessage == SetsFullText)
            WarningMessage = string.Empty;
    }

    public void RunReloadFilter()
    {
        _reloadFilterService.ReloadFilter();
    }

    private bool ShowAmountNeeded => Settings.SetTrackerOverlayItemCounterDisplayMode == 2;

    public bool NeedsFetching => _itemSetManagerService.RetrieveNeedsFetching();
    public int FullSets => _itemSetManagerService.RetrieveCompletedSetCount();

    public int RingsAmount => ShowAmountNeeded ? Math.Max((Settings.FullSetThreshold * 2) - _itemSetManagerService.RetrieveRingsAmount(), 0) : _itemSetManagerService.RetrieveRingsAmount();
    public bool RingsActive => NeedsFetching || (Properties.Settings.Default.FullSetThreshold * 2) - _itemSetManagerService.RetrieveRingsAmount() > 0;

    public int AmuletsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveAmuletsAmount(), 0) : _itemSetManagerService.RetrieveAmuletsAmount();
    public bool AmuletsActive => NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveAmuletsAmount() > 0;

    public int BeltsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveBeltsAmount(), 0) : _itemSetManagerService.RetrieveBeltsAmount();
    public bool BeltsActive => NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveBeltsAmount() > 0;

    public int ChestsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveChestsAmount(), 0) : _itemSetManagerService.RetrieveChestsAmount();
    public bool ChestsActive => NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveChestsAmount() > 0;


    public int WeaponsAmount => ShowAmountNeeded ? Math.Max((Properties.Settings.Default.FullSetThreshold * 2) - (_itemSetManagerService.RetrieveWeaponsSmallAmount() + (_itemSetManagerService.RetrieveWeaponsBigAmount() * 2)), 0) : _itemSetManagerService.RetrieveWeaponsSmallAmount() + (_itemSetManagerService.RetrieveWeaponsBigAmount() * 2);
    public bool WeaponsActive => NeedsFetching || (Properties.Settings.Default.FullSetThreshold * 2) - (_itemSetManagerService.RetrieveWeaponsSmallAmount() + (_itemSetManagerService.RetrieveWeaponsBigAmount() * 2)) > 0;

    public int GlovesAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveGlovesAmount(), 0) : _itemSetManagerService.RetrieveGlovesAmount();
    public bool GlovesActive => NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveGlovesAmount() > 0;

    public int HelmetsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveHelmetsAmount(), 0) : _itemSetManagerService.RetrieveHelmetsAmount();
    public bool HelmetsActive => NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveHelmetsAmount() > 0;

    public int BootsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveBootsAmount(), 0) : _itemSetManagerService.RetrieveBootsAmount();
    public bool BootsActive => NeedsFetching || Properties.Settings.Default.FullSetThreshold - _itemSetManagerService.RetrieveBootsAmount() > 0;

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
        OnPropertyChanged(nameof(FullSets));
        OnPropertyChanged(nameof(WarningMessage));
        OnPropertyChanged(nameof(ShowProgress));
        OnPropertyChanged(nameof(FetchButtonEnabled));
        OnPropertyChanged(nameof(ShowAmountNeeded));
    }
}