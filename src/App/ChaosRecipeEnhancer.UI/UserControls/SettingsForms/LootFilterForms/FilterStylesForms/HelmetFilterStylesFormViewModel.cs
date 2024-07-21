using ChaosRecipeEnhancer.UI.Models.UserSettings;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public class HelmetFilterStylesFormViewModel : CreViewModelBase
{
    private readonly IUserSettings _userSettings;

    private ICommand _toggleAlwaysActiveCommand;
    private ICommand _toggleAlwaysDisabledCommand;
    private ICommand _resetHelmetStylesCommand;

    private readonly string _defaultBackgroundColor = "#000000"; // Black
    private readonly string _defaultBorderColor = "#FFFFFF"; // White
    private readonly string _defaultTextColor = "#FFFF77"; // Yellow

    private readonly string _helmetDefaultBackgroundColor = "#FFFFBC00"; // Yellow
    private readonly string _helmetDefaultBorderColor = "#FFFFFFFF"; // White
    private readonly string _helmetDefaultTextColor = "#FFFFFFFF"; // White

    private string _previousBackgroundColor;
    private string _previousBorderColor;
    private string _previousTextColor;
    private string _iconFilename;

    public HelmetFilterStylesFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;

        UpdateIconFilename();
    }

    #region Commands

    public ICommand ToggleAlwaysActiveCommand => _toggleAlwaysActiveCommand ??= new RelayCommand(ToggleAlwaysActive);
    public ICommand ToggleAlwaysDisabledCommand => _toggleAlwaysDisabledCommand ??= new RelayCommand(ToggleAlwaysDisabled);
    public ICommand ResetHelmetStylesCommand => _resetHelmetStylesCommand ??= new RelayCommand(ResetHelmetStyles);

    #endregion

    #region Properties

    public string LootFilterStylesHelmetTextColor
    {
        get => _userSettings.LootFilterStylesHelmetTextColor;
        set
        {
            if (_userSettings.LootFilterStylesHelmetTextColor != value)
            {
                _userSettings.LootFilterStylesHelmetTextColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesHelmetBorderColor
    {
        get => _userSettings.LootFilterStylesHelmetBorderColor;
        set
        {
            if (_userSettings.LootFilterStylesHelmetBorderColor != value)
            {
                _userSettings.LootFilterStylesHelmetBorderColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesHelmetBackgroundColor
    {
        get => _userSettings.LootFilterStylesHelmetBackgroundColor;
        set
        {
            if (_userSettings.LootFilterStylesHelmetBackgroundColor != value)
            {
                _userSettings.LootFilterStylesHelmetBackgroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesHelmetTextFontSize
    {
        get => _userSettings.LootFilterStylesHelmetTextFontSize;
        set
        {
            if (_userSettings.LootFilterStylesHelmetTextFontSize != value)
            {
                _userSettings.LootFilterStylesHelmetTextFontSize = value;
                OnPropertyChanged();
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
                OnPropertyChanged();
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
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesHelmetMapIconEnabled
    {
        get => _userSettings.LootFilterStylesHelmetMapIconEnabled;
        set
        {
            if (_userSettings.LootFilterStylesHelmetMapIconEnabled != value)
            {
                _userSettings.LootFilterStylesHelmetMapIconEnabled = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesHelmetMapIconSize
    {
        get => _userSettings.LootFilterStylesHelmetMapIconSize;
        set
        {
            if (_userSettings.LootFilterStylesHelmetMapIconSize != value)
            {
                _userSettings.LootFilterStylesHelmetMapIconSize = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesHelmetMapIconColor
    {
        get => _userSettings.LootFilterStylesHelmetMapIconColor;
        set
        {
            if (_userSettings.LootFilterStylesHelmetMapIconColor != value)
            {
                _userSettings.LootFilterStylesHelmetMapIconColor = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesHelmetMapIconShape
    {
        get => _userSettings.LootFilterStylesHelmetMapIconShape;
        set
        {
            if (_userSettings.LootFilterStylesHelmetMapIconShape != value)
            {
                _userSettings.LootFilterStylesHelmetMapIconShape = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public bool LootFilterStylesHelmetBeamEnabled
    {
        get => _userSettings.LootFilterStylesHelmetBeamEnabled;
        set
        {
            if (_userSettings.LootFilterStylesHelmetBeamEnabled != value)
            {
                _userSettings.LootFilterStylesHelmetBeamEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesHelmetBeamColor
    {
        get => _userSettings.LootFilterStylesHelmetBeamColor;
        set
        {
            if (_userSettings.LootFilterStylesHelmetBeamColor != value)
            {
                _userSettings.LootFilterStylesHelmetBeamColor = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesHelmetBeamTemporary
    {
        get => _userSettings.LootFilterStylesHelmetBeamTemporary;
        set
        {
            if (_userSettings.LootFilterStylesHelmetBeamTemporary != value)
            {
                _userSettings.LootFilterStylesHelmetBeamTemporary = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesHelmetTextColorEnabled
    {
        get => _userSettings.LootFilterStylesHelmetTextColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesHelmetTextColorEnabled != value)
            {
                _userSettings.LootFilterStylesHelmetTextColorEnabled = value;
                OnPropertyChanged();
                ApplyTextColorSetting();
            }
        }
    }

    public bool LootFilterStylesHelmetBorderColorEnabled
    {
        get => _userSettings.LootFilterStylesHelmetBorderColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesHelmetBorderColorEnabled != value)
            {
                _userSettings.LootFilterStylesHelmetBorderColorEnabled = value;
                OnPropertyChanged();
                ApplyBorderColorSetting();
            }
        }
    }

    public bool LootFilterStylesHelmetBackgroundColorEnabled
    {
        get => _userSettings.LootFilterStylesHelmetBackgroundColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesHelmetBackgroundColorEnabled != value)
            {
                _userSettings.LootFilterStylesHelmetBackgroundColorEnabled = value;
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
        if (LootFilterStylesHelmetAlwaysActive)
        {
            LootFilterStylesHelmetAlwaysDisabled = false;
        }
    }

    private void ToggleAlwaysDisabled()
    {
        if (LootFilterStylesHelmetAlwaysDisabled)
        {
            LootFilterStylesHelmetAlwaysActive = false;
        }
    }

    private void ResetHelmetStyles()
    {
        // Reset all settings to their default values
        LootFilterStylesHelmetTextColor = _helmetDefaultTextColor; // White
        LootFilterStylesHelmetBorderColor = _helmetDefaultBorderColor; // White
        LootFilterStylesHelmetBackgroundColor = _helmetDefaultBackgroundColor; // Red
        LootFilterStylesHelmetTextFontSize = 45; // Default Font Size
        LootFilterStylesHelmetAlwaysActive = false;
        LootFilterStylesHelmetAlwaysDisabled = false;
        LootFilterStylesHelmetMapIconEnabled = true;
        LootFilterStylesHelmetMapIconSize = 2; // Small size
        LootFilterStylesHelmetMapIconColor = 10; // Yellow
        LootFilterStylesHelmetMapIconShape = 0; // Circle
        LootFilterStylesHelmetBeamEnabled = false;
        LootFilterStylesHelmetBeamColor = 10; // Yellow
        LootFilterStylesHelmetBeamTemporary = false;
        LootFilterStylesHelmetTextColorEnabled = true;
        LootFilterStylesHelmetBorderColorEnabled = true;
        LootFilterStylesHelmetBackgroundColorEnabled = true;

        UpdateIconFilename();

        // Notify UI of all changes
        OnPropertyChanged(string.Empty);
    }

    private void ApplyTextColorSetting()
    {
        if (!LootFilterStylesHelmetTextColorEnabled && LootFilterStylesHelmetTextColor is not null)
        {
            _previousTextColor = LootFilterStylesHelmetTextColor;
            LootFilterStylesHelmetTextColor = _defaultTextColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousTextColor is not null)
            {
                LootFilterStylesHelmetTextColor = _previousTextColor;
            }
            else
            {
                LootFilterStylesHelmetTextColor = _helmetDefaultTextColor;
            }
        }
    }

    private void ApplyBorderColorSetting()
    {
        if (!LootFilterStylesHelmetBorderColorEnabled && LootFilterStylesHelmetBorderColor is not null)
        {
            _previousBorderColor = LootFilterStylesHelmetBorderColor;
            LootFilterStylesHelmetBorderColor = _defaultBorderColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousBorderColor is not null)
            {
                LootFilterStylesHelmetBorderColor = _previousBorderColor;
            }
            else
            {
                LootFilterStylesHelmetBorderColor = _helmetDefaultBorderColor;
            }
        }
    }

    private void ApplyBackgroundColorSetting()
    {
        if (!LootFilterStylesHelmetBackgroundColorEnabled && LootFilterStylesHelmetBackgroundColor is not null)
        {
            _previousBackgroundColor = LootFilterStylesHelmetBackgroundColor;
            LootFilterStylesHelmetBackgroundColor = _defaultBackgroundColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set

            if (_previousTextColor is not null)
            {
                LootFilterStylesHelmetBackgroundColor = _previousBackgroundColor;
            }
            else
            {
                LootFilterStylesHelmetBackgroundColor = _helmetDefaultBackgroundColor;
            }
        }
    }

    private void UpdateIconFilename()
    {
        string size = LootFilterStylesHelmetMapIconSize switch
        {
            0 => "Large",
            1 => "Medium",
            2 => "Small",
            _ => "Large" // Default to Medium
        };

        string color = LootFilterStylesHelmetMapIconColor switch
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

        string shape = LootFilterStylesHelmetMapIconShape switch
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
