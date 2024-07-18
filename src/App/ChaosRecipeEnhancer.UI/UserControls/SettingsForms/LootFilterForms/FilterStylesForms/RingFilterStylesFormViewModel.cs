using ChaosRecipeEnhancer.UI.Models.UserSettings;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public class RingFilterStylesFormViewModel : CreViewModelBase
{
    private readonly IUserSettings _userSettings;

    private ICommand _toggleAlwaysActiveCommand;
    private ICommand _toggleAlwaysDisabledCommand;
    private ICommand _resetRingStylesCommand;

    public RingFilterStylesFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;
    }

    #region Commands

    public ICommand ToggleAlwaysActiveCommand => _toggleAlwaysActiveCommand ??= new RelayCommand(ToggleAlwaysActive);
    public ICommand ToggleAlwaysDisabledCommand => _toggleAlwaysDisabledCommand ??= new RelayCommand(ToggleAlwaysDisabled);
    public ICommand ResetRingStylesCommand => _resetRingStylesCommand ??= new RelayCommand(ResetRingStyles);

    #endregion

    #region Properties

    public string LootFilterStylesRingTextColor
    {
        get => _userSettings.LootFilterStylesRingTextColor;
        set
        {
            if (_userSettings.LootFilterStylesRingTextColor != value)
            {
                _userSettings.LootFilterStylesRingTextColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesRingBorderColor
    {
        get => _userSettings.LootFilterStylesRingBorderColor;
        set
        {
            if (_userSettings.LootFilterStylesRingBorderColor != value)
            {
                _userSettings.LootFilterStylesRingBorderColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesRingBackgroundColor
    {
        get => _userSettings.LootFilterStylesRingBackgroundColor;
        set
        {
            if (_userSettings.LootFilterStylesRingBackgroundColor != value)
            {
                _userSettings.LootFilterStylesRingBackgroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesRingTextFontSize
    {
        get => _userSettings.LootFilterStylesRingTextFontSize;
        set
        {
            if (_userSettings.LootFilterStylesRingTextFontSize != value)
            {
                _userSettings.LootFilterStylesRingTextFontSize = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesRingAlwaysActive
    {
        get => _userSettings.LootFilterStylesRingAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesRingAlwaysActive != value)
            {
                _userSettings.LootFilterStylesRingAlwaysActive = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesRingAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesRingAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesRingAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesRingAlwaysDisabled = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesRingMapIconEnabled
    {
        get => _userSettings.LootFilterStylesRingMapIconEnabled;
        set
        {
            if (_userSettings.LootFilterStylesRingMapIconEnabled != value)
            {
                _userSettings.LootFilterStylesRingMapIconEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesRingMapIconSize
    {
        get => _userSettings.LootFilterStylesRingMapIconSize;
        set
        {
            if (_userSettings.LootFilterStylesRingMapIconSize != value)
            {
                _userSettings.LootFilterStylesRingMapIconSize = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesRingMapIconColor
    {
        get => _userSettings.LootFilterStylesRingMapIconColor;
        set
        {
            if (_userSettings.LootFilterStylesRingMapIconColor != value)
            {
                _userSettings.LootFilterStylesRingMapIconColor = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesRingMapIconShape
    {
        get => _userSettings.LootFilterStylesRingMapIconShape;
        set
        {
            if (_userSettings.LootFilterStylesRingMapIconShape != value)
            {
                _userSettings.LootFilterStylesRingMapIconShape = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesRingBeamEnabled
    {
        get => _userSettings.LootFilterStylesRingBeamEnabled;
        set
        {
            if (_userSettings.LootFilterStylesRingBeamEnabled != value)
            {
                _userSettings.LootFilterStylesRingBeamEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesRingBeamColor
    {
        get => _userSettings.LootFilterStylesRingBeamColor;
        set
        {
            if (_userSettings.LootFilterStylesRingBeamColor != value)
            {
                _userSettings.LootFilterStylesRingBeamColor = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesRingBeamTemporary
    {
        get => _userSettings.LootFilterStylesRingBeamTemporary;
        set
        {
            if (_userSettings.LootFilterStylesRingBeamTemporary != value)
            {
                _userSettings.LootFilterStylesRingBeamTemporary = value;
                OnPropertyChanged();
            }
        }
    }

    #endregion

    #region Methods

    private void ToggleAlwaysActive()
    {
        if (LootFilterStylesRingAlwaysActive)
        {
            LootFilterStylesRingAlwaysDisabled = false;
        }
    }

    private void ToggleAlwaysDisabled()
    {
        if (LootFilterStylesRingAlwaysDisabled)
        {
            LootFilterStylesRingAlwaysActive = false;
        }
    }

    private void ResetRingStyles()
    {
        // Reset all settings to their default values
        LootFilterStylesRingTextColor = "#FFFFB0B0"; // Light Red
        LootFilterStylesRingBorderColor = "#FFFFB0B0"; // Light Red
        LootFilterStylesRingBackgroundColor = "#FFFF0303"; // Red
        LootFilterStylesRingTextFontSize = 45; // Default font size
        LootFilterStylesRingAlwaysActive = false;
        LootFilterStylesRingAlwaysDisabled = false;
        LootFilterStylesRingMapIconEnabled = false;
        LootFilterStylesRingMapIconSize = 1; // Medium size
        LootFilterStylesRingMapIconColor = 0; // Blue
        LootFilterStylesRingMapIconShape = 0; // Circle
        LootFilterStylesRingBeamEnabled = false;
        LootFilterStylesRingBeamColor = 0; // Blue
        LootFilterStylesRingBeamTemporary = false;

        // Notify UI of all changes
        OnPropertyChanged(string.Empty);
    }

    #endregion
}
