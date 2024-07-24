using ChaosRecipeEnhancer.UI.Models.UserSettings;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public class BeltFilterStylesFormViewModel : CreViewModelBase
{
    private readonly IUserSettings _userSettings;

    private ICommand _toggleAlwaysActiveCommand;
    private ICommand _toggleAlwaysDisabledCommand;
    private ICommand _resetBeltStylesCommand;

    private readonly string _defaultBackgroundColor = "#000000"; // Black
    private readonly string _defaultBorderColor = "#FFFFFF"; // White
    private readonly string _defaultTextColor = "#FFFF77"; // Yellow

    private readonly string _beltDefaultBackgroundColor = "#FFFF0000"; // Red
    private readonly string _beltDefaultBorderColor = "#FFFFFFFF"; // White
    private readonly string _beltDefaultTextColor = "#FFFFFFFF"; // White

    private string _previousBackgroundColor;
    private string _previousBorderColor;
    private string _previousTextColor;
    private string _iconFilename;

    public BeltFilterStylesFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;

        UpdateIconFilename();
    }

    #region Commands

    public ICommand ToggleAlwaysActiveCommand => _toggleAlwaysActiveCommand ??= new RelayCommand(ToggleAlwaysActive);
    public ICommand ToggleAlwaysDisabledCommand => _toggleAlwaysDisabledCommand ??= new RelayCommand(ToggleAlwaysDisabled);
    public ICommand ResetBeltStylesCommand => _resetBeltStylesCommand ??= new RelayCommand(ResetBeltStyles);

    #endregion

    #region Properties

    public string LootFilterStylesBeltTextColor
    {
        get => _userSettings.LootFilterStylesBeltTextColor;
        set
        {
            if (_userSettings.LootFilterStylesBeltTextColor != value)
            {
                _userSettings.LootFilterStylesBeltTextColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesBeltBorderColor
    {
        get => _userSettings.LootFilterStylesBeltBorderColor;
        set
        {
            if (_userSettings.LootFilterStylesBeltBorderColor != value)
            {
                _userSettings.LootFilterStylesBeltBorderColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesBeltBackgroundColor
    {
        get => _userSettings.LootFilterStylesBeltBackgroundColor;
        set
        {
            if (_userSettings.LootFilterStylesBeltBackgroundColor != value)
            {
                _userSettings.LootFilterStylesBeltBackgroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesBeltTextFontSize
    {
        get => _userSettings.LootFilterStylesBeltTextFontSize;
        set
        {
            if (_userSettings.LootFilterStylesBeltTextFontSize != value)
            {
                _userSettings.LootFilterStylesBeltTextFontSize = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBeltAlwaysActive
    {
        get => _userSettings.LootFilterStylesBeltAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesBeltAlwaysActive != value)
            {
                _userSettings.LootFilterStylesBeltAlwaysActive = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBeltAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesBeltAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesBeltAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesBeltAlwaysDisabled = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBeltMapIconEnabled
    {
        get => _userSettings.LootFilterStylesBeltMapIconEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBeltMapIconEnabled != value)
            {
                _userSettings.LootFilterStylesBeltMapIconEnabled = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesBeltMapIconSize
    {
        get => _userSettings.LootFilterStylesBeltMapIconSize;
        set
        {
            if (_userSettings.LootFilterStylesBeltMapIconSize != value)
            {
                _userSettings.LootFilterStylesBeltMapIconSize = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesBeltMapIconColor
    {
        get => _userSettings.LootFilterStylesBeltMapIconColor;
        set
        {
            if (_userSettings.LootFilterStylesBeltMapIconColor != value)
            {
                _userSettings.LootFilterStylesBeltMapIconColor = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesBeltMapIconShape
    {
        get => _userSettings.LootFilterStylesBeltMapIconShape;
        set
        {
            if (_userSettings.LootFilterStylesBeltMapIconShape != value)
            {
                _userSettings.LootFilterStylesBeltMapIconShape = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public bool LootFilterStylesBeltBeamEnabled
    {
        get => _userSettings.LootFilterStylesBeltBeamEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBeltBeamEnabled != value)
            {
                _userSettings.LootFilterStylesBeltBeamEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesBeltBeamColor
    {
        get => _userSettings.LootFilterStylesBeltBeamColor;
        set
        {
            if (_userSettings.LootFilterStylesBeltBeamColor != value)
            {
                _userSettings.LootFilterStylesBeltBeamColor = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBeltBeamTemporary
    {
        get => _userSettings.LootFilterStylesBeltBeamTemporary;
        set
        {
            if (_userSettings.LootFilterStylesBeltBeamTemporary != value)
            {
                _userSettings.LootFilterStylesBeltBeamTemporary = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBeltTextColorEnabled
    {
        get => _userSettings.LootFilterStylesBeltTextColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBeltTextColorEnabled != value)
            {
                _userSettings.LootFilterStylesBeltTextColorEnabled = value;
                OnPropertyChanged();
                ApplyTextColorSetting();
            }
        }
    }

    public bool LootFilterStylesBeltBorderColorEnabled
    {
        get => _userSettings.LootFilterStylesBeltBorderColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBeltBorderColorEnabled != value)
            {
                _userSettings.LootFilterStylesBeltBorderColorEnabled = value;
                OnPropertyChanged();
                ApplyBorderColorSetting();
            }
        }
    }

    public bool LootFilterStylesBeltBackgroundColorEnabled
    {
        get => _userSettings.LootFilterStylesBeltBackgroundColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBeltBackgroundColorEnabled != value)
            {
                _userSettings.LootFilterStylesBeltBackgroundColorEnabled = value;
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
        if (LootFilterStylesBeltAlwaysActive)
        {
            LootFilterStylesBeltAlwaysDisabled = false;
        }
    }

    private void ToggleAlwaysDisabled()
    {
        if (LootFilterStylesBeltAlwaysDisabled)
        {
            LootFilterStylesBeltAlwaysActive = false;
        }
    }

    private void ResetBeltStyles()
    {
        // Reset all settings to their default values
        LootFilterStylesBeltTextColor = _beltDefaultTextColor; // White
        LootFilterStylesBeltBorderColor = _beltDefaultBorderColor; // White
        LootFilterStylesBeltBackgroundColor = _beltDefaultBackgroundColor; // Red
        LootFilterStylesBeltTextFontSize = 45; // Default Font Size
        LootFilterStylesBeltAlwaysActive = true;
        LootFilterStylesBeltAlwaysDisabled = false;
        LootFilterStylesBeltMapIconEnabled = true;
        LootFilterStylesBeltMapIconSize = 2; // Small size
        LootFilterStylesBeltMapIconColor = 10; // Yellow
        LootFilterStylesBeltMapIconShape = 0; // Circle
        LootFilterStylesBeltBeamEnabled = false;
        LootFilterStylesBeltBeamColor = 10; // Yellow
        LootFilterStylesBeltBeamTemporary = false;
        LootFilterStylesBeltTextColorEnabled = true;
        LootFilterStylesBeltBorderColorEnabled = true;
        LootFilterStylesBeltBackgroundColorEnabled = true;

        UpdateIconFilename();

        // Notify UI of all changes
        OnPropertyChanged(string.Empty);
    }

    private void ApplyTextColorSetting()
    {
        if (!LootFilterStylesBeltTextColorEnabled && LootFilterStylesBeltTextColor is not null)
        {
            _previousTextColor = LootFilterStylesBeltTextColor;
            LootFilterStylesBeltTextColor = _defaultTextColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousTextColor is not null)
            {
                LootFilterStylesBeltTextColor = _previousTextColor;
            }
            else
            {
                LootFilterStylesBeltTextColor = _beltDefaultTextColor;
            }
        }
    }

    private void ApplyBorderColorSetting()
    {
        if (!LootFilterStylesBeltBorderColorEnabled && LootFilterStylesBeltBorderColor is not null)
        {
            _previousBorderColor = LootFilterStylesBeltBorderColor;
            LootFilterStylesBeltBorderColor = _defaultBorderColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousBorderColor is not null)
            {
                LootFilterStylesBeltBorderColor = _previousBorderColor;
            }
            else
            {
                LootFilterStylesBeltBorderColor = _beltDefaultBorderColor;
            }
        }
    }

    private void ApplyBackgroundColorSetting()
    {
        if (!LootFilterStylesBeltBackgroundColorEnabled && LootFilterStylesBeltBackgroundColor is not null)
        {
            _previousBackgroundColor = LootFilterStylesBeltBackgroundColor;
            LootFilterStylesBeltBackgroundColor = _defaultBackgroundColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set

            if (_previousTextColor is not null)
            {
                LootFilterStylesBeltBackgroundColor = _previousBackgroundColor;
            }
            else
            {
                LootFilterStylesBeltBackgroundColor = _beltDefaultBackgroundColor;
            }
        }
    }

    private void UpdateIconFilename()
    {
        string size = LootFilterStylesBeltMapIconSize switch
        {
            0 => "Large",
            1 => "Medium",
            2 => "Small",
            _ => "Large" // Default to Medium
        };

        string color = LootFilterStylesBeltMapIconColor switch
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

        string shape = LootFilterStylesBeltMapIconShape switch
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
