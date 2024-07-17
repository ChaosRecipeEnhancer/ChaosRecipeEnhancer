using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services.FilterManipulation;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms;

public class LootFilterManipulationFormViewModel : CreViewModelBase
{
    private readonly IFilterManipulationService _filterManipulationService;
    private readonly IReloadFilterService _reloadFilterService;
    private readonly IUserSettings _userSettings;

    private ICommand _cleanLootFilterCommand;

    public LootFilterManipulationFormViewModel(
        IFilterManipulationService filterManipulationService,
        IReloadFilterService reloadFilterService,
        IUserSettings userSettings
    )
    {
        _filterManipulationService = filterManipulationService;
        _reloadFilterService = reloadFilterService;
        _userSettings = userSettings;
    }

    public ICommand CleanLootFilterCommand => _cleanLootFilterCommand ??= new RelayCommand(CleanLootFilter);

    public bool LootFilterManipulationEnabled
    {
        get => _userSettings.LootFilterManipulationEnabled;
        set
        {
            _userSettings.LootFilterManipulationEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool LootFilterIconsEnabled
    {
        get => _userSettings.LootFilterIconsEnabled;
        set
        {
            _userSettings.LootFilterIconsEnabled = value;
            OnPropertyChanged();
        }
    }

    public string LootFilterFileLocation
    {
        get => _userSettings.LootFilterFileLocation;
        set
        {
            _userSettings.LootFilterFileLocation = value;
            OnPropertyChanged();
        }
    }

    public bool LootFilterSpaceSavingHideLargeWeapons
    {
        get => _userSettings.LootFilterSpaceSavingHideLargeWeapons;
        set
        {
            _userSettings.LootFilterSpaceSavingHideLargeWeapons = value;
            OnPropertyChanged();
        }
    }

    public bool LootFilterSpaceSavingHideOffHand
    {
        get => _userSettings.LootFilterSpaceSavingHideOffHand;
        set
        {
            _userSettings.LootFilterSpaceSavingHideOffHand = value;
            OnPropertyChanged();
        }
    }

    private void CleanLootFilter()
    {
        _filterManipulationService.RemoveChaosRecipeSectionAsync();
        _reloadFilterService.ReloadFilter();
    }
}