using ChaosRecipeEnhancer.UI.Models.UserSettings;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public class WeaponFilterStylesFormViewModel : CreViewModelBase
{
    private readonly IUserSettings _userSettings;

    private ICommand _toggleAlwaysActiveCommand;
    private ICommand _toggleAlwaysDisabledCommand;
    private ICommand _resetWeaponStylesCommand;

    private readonly string _defaultBackgroundColor = "#000000"; // Black
    private readonly string _defaultBorderColor = "#FFFFFF"; // White
    private readonly string _defaultTextColor = "#FFFF77"; // Yellow

    private readonly string _weaponDefaultBackgroundColor = "#FF00B8D5"; // Blue
    private readonly string _weaponDefaultBorderColor = "#FFFFFFFF"; // White
    private readonly string _weaponDefaultTextColor = "#FFFFFFFF"; // White

    private string _previousBackgroundColor;
    private string _previousBorderColor;
    private string _previousTextColor;
    private string _iconFilename;

    public WeaponFilterStylesFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;

        UpdateIconFilename();
    }

    #region Commands

    public ICommand ToggleAlwaysActiveCommand => _toggleAlwaysActiveCommand ??= new RelayCommand(ToggleAlwaysActive);
    public ICommand ToggleAlwaysDisabledCommand => _toggleAlwaysDisabledCommand ??= new RelayCommand(ToggleAlwaysDisabled);
    public ICommand ResetWeaponStylesCommand => _resetWeaponStylesCommand ??= new RelayCommand(ResetWeaponStyles);

    #endregion

    #region Properties

    public string LootFilterStylesWeaponTextColor
    {
        get => _userSettings.LootFilterStylesWeaponTextColor;
        set
        {
            if (_userSettings.LootFilterStylesWeaponTextColor != value)
            {
                _userSettings.LootFilterStylesWeaponTextColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesWeaponBorderColor
    {
        get => _userSettings.LootFilterStylesWeaponBorderColor;
        set
        {
            if (_userSettings.LootFilterStylesWeaponBorderColor != value)
            {
                _userSettings.LootFilterStylesWeaponBorderColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesWeaponBackgroundColor
    {
        get => _userSettings.LootFilterStylesWeaponBackgroundColor;
        set
        {
            if (_userSettings.LootFilterStylesWeaponBackgroundColor != value)
            {
                _userSettings.LootFilterStylesWeaponBackgroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesWeaponTextFontSize
    {
        get => _userSettings.LootFilterStylesWeaponTextFontSize;
        set
        {
            if (_userSettings.LootFilterStylesWeaponTextFontSize != value)
            {
                _userSettings.LootFilterStylesWeaponTextFontSize = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesWeaponMapIconEnabled
    {
        get => _userSettings.LootFilterStylesWeaponMapIconEnabled;
        set
        {
            if (_userSettings.LootFilterStylesWeaponMapIconEnabled != value)
            {
                _userSettings.LootFilterStylesWeaponMapIconEnabled = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesWeaponMapIconSize
    {
        get => _userSettings.LootFilterStylesWeaponMapIconSize;
        set
        {
            if (_userSettings.LootFilterStylesWeaponMapIconSize != value)
            {
                _userSettings.LootFilterStylesWeaponMapIconSize = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesWeaponMapIconColor
    {
        get => _userSettings.LootFilterStylesWeaponMapIconColor;
        set
        {
            if (_userSettings.LootFilterStylesWeaponMapIconColor != value)
            {
                _userSettings.LootFilterStylesWeaponMapIconColor = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesWeaponMapIconShape
    {
        get => _userSettings.LootFilterStylesWeaponMapIconShape;
        set
        {
            if (_userSettings.LootFilterStylesWeaponMapIconShape != value)
            {
                _userSettings.LootFilterStylesWeaponMapIconShape = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public bool LootFilterStylesWeaponBeamEnabled
    {
        get => _userSettings.LootFilterStylesWeaponBeamEnabled;
        set
        {
            if (_userSettings.LootFilterStylesWeaponBeamEnabled != value)
            {
                _userSettings.LootFilterStylesWeaponBeamEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesWeaponBeamColor
    {
        get => _userSettings.LootFilterStylesWeaponBeamColor;
        set
        {
            if (_userSettings.LootFilterStylesWeaponBeamColor != value)
            {
                _userSettings.LootFilterStylesWeaponBeamColor = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesWeaponBeamTemporary
    {
        get => _userSettings.LootFilterStylesWeaponBeamTemporary;
        set
        {
            if (_userSettings.LootFilterStylesWeaponBeamTemporary != value)
            {
                _userSettings.LootFilterStylesWeaponBeamTemporary = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesWeaponTextColorEnabled
    {
        get => _userSettings.LootFilterStylesWeaponTextColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesWeaponTextColorEnabled != value)
            {
                _userSettings.LootFilterStylesWeaponTextColorEnabled = value;
                OnPropertyChanged();
                ApplyTextColorSetting();
            }
        }
    }

    public bool LootFilterStylesWeaponBorderColorEnabled
    {
        get => _userSettings.LootFilterStylesWeaponBorderColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesWeaponBorderColorEnabled != value)
            {
                _userSettings.LootFilterStylesWeaponBorderColorEnabled = value;
                OnPropertyChanged();
                ApplyBorderColorSetting();
            }
        }
    }

    public bool LootFilterStylesWeaponBackgroundColorEnabled
    {
        get => _userSettings.LootFilterStylesWeaponBackgroundColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesWeaponBackgroundColorEnabled != value)
            {
                _userSettings.LootFilterStylesWeaponBackgroundColorEnabled = value;
                OnPropertyChanged();
                ApplyBackgroundColorSetting();
            }
        }
    }

    public string IconFilename
    {
        get => _iconFilename;
        set
        {
            if (_iconFilename != value)
            {
                _iconFilename = value;
                OnPropertyChanged();
            }
        }
    }

    #endregion

    #region Methods

    private void ToggleAlwaysActive()
    {
        if (LootFilterStylesWeaponAlwaysActive)
        {
            LootFilterStylesWeaponAlwaysDisabled = false;
        }
    }

    private void ToggleAlwaysDisabled()
    {
        if (LootFilterStylesWeaponAlwaysDisabled)
        {
            LootFilterStylesWeaponAlwaysActive = false;
        }
    }

    private void ResetWeaponStyles()
    {
        // Reset all settings to their default values
        LootFilterStylesWeaponTextColor = _weaponDefaultTextColor; // White
        LootFilterStylesWeaponBorderColor = _weaponDefaultBorderColor; // White
        LootFilterStylesWeaponBackgroundColor = _weaponDefaultBackgroundColor; // Red
        LootFilterStylesWeaponTextFontSize = 45; // Default Font Size
        LootFilterStylesWeaponAlwaysActive = false;
        LootFilterStylesWeaponAlwaysDisabled = false;
        LootFilterStylesWeaponMapIconEnabled = true;
        LootFilterStylesWeaponMapIconSize = 0; // Large size
        LootFilterStylesWeaponMapIconColor = 10; // Yellow
        LootFilterStylesWeaponMapIconShape = 0; // Circle
        LootFilterStylesWeaponBeamEnabled = false;
        LootFilterStylesWeaponBeamColor = 10; // Yellow
        LootFilterStylesWeaponBeamTemporary = false;
        LootFilterStylesWeaponTextColorEnabled = true;
        LootFilterStylesWeaponBorderColorEnabled = true;
        LootFilterStylesWeaponBackgroundColorEnabled = true;

        UpdateIconFilename();

        // Notify UI of all changes
        OnPropertyChanged(string.Empty);
    }

    private void ApplyTextColorSetting()
    {
        if (!LootFilterStylesWeaponTextColorEnabled && LootFilterStylesWeaponTextColor is not null)
        {
            _previousTextColor = LootFilterStylesWeaponTextColor;
            LootFilterStylesWeaponTextColor = _defaultTextColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousTextColor is not null)
            {
                LootFilterStylesWeaponTextColor = _previousTextColor;
            }
            else
            {
                LootFilterStylesWeaponTextColor = _weaponDefaultTextColor;
            }
        }
    }

    private void ApplyBorderColorSetting()
    {
        if (!LootFilterStylesWeaponBorderColorEnabled && LootFilterStylesWeaponBorderColor is not null)
        {
            _previousBorderColor = LootFilterStylesWeaponBorderColor;
            LootFilterStylesWeaponBorderColor = _defaultBorderColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousBorderColor is not null)
            {
                LootFilterStylesWeaponBorderColor = _previousBorderColor;
            }
            else
            {
                LootFilterStylesWeaponBorderColor = _weaponDefaultBorderColor;
            }
        }
    }

    private void ApplyBackgroundColorSetting()
    {
        if (!LootFilterStylesWeaponBackgroundColorEnabled && LootFilterStylesWeaponBackgroundColor is not null)
        {
            _previousBackgroundColor = LootFilterStylesWeaponBackgroundColor;
            LootFilterStylesWeaponBackgroundColor = _defaultBackgroundColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set

            if (_previousTextColor is not null)
            {
                LootFilterStylesWeaponBackgroundColor = _previousBackgroundColor;
            }
            else
            {
                LootFilterStylesWeaponBackgroundColor = _weaponDefaultBackgroundColor;
            }
        }
    }

    private void UpdateIconFilename()
    {
        string size = LootFilterStylesWeaponMapIconSize switch
        {
            0 => "Large",
            1 => "Medium",
            2 => "Small",
            _ => "Large" // Default to Medium
        };

        string color = LootFilterStylesWeaponMapIconColor switch
        {
            0 => "Blue",
            1 => "Brown",
            2 => "Cyan",
            3 => "Green",
            4 => "Grey",
            5 => "Orange",
            6 => "Pink",
            7 => "Purple",
            8 => "Red",
            9 => "White",
            10 => "Yellow",
            _ => "Yellow" // Default to Blue
        };

        string shape = LootFilterStylesWeaponMapIconShape switch
        {
            0 => "Circle",
            1 => "Cross",
            2 => "Diamond",
            3 => "Hexagon",
            4 => "Kite",
            5 => "Moon",
            6 => "Pentagon",
            7 => "Raindrop",
            8 => "Square",
            9 => "Star",
            10 => "Triangle",
            11 => "UpsideDownHouse",
            _ => "Circle" // Default to Circle
        };

        IconFilename = $"../../../../Assets/Images/LootFilter/LootFilter{size}{color}{shape}.png";
    }

    #endregion
}
