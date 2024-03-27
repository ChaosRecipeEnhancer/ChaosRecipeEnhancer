using ChaosRecipeEnhancer.UI.Models;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.GeneralForms;

public class RecipesFormViewModel : CreViewModelBase
{
    private readonly IUserSettings _userSettings;

    public RecipesFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;
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

    public bool IncludeIdentifiedItemsEnabled
    {
        get => _userSettings.IncludeIdentifiedItemsEnabled;
        set
        {
            if (_userSettings.IncludeIdentifiedItemsEnabled != value)
            {
                _userSettings.IncludeIdentifiedItemsEnabled = value;
                OnPropertyChanged(nameof(IncludeIdentifiedItemsEnabled));
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
}