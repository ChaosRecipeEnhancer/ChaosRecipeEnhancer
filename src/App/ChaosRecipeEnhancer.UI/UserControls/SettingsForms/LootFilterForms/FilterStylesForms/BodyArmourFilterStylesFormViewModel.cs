using ChaosRecipeEnhancer.UI.Models.UserSettings;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public class BodyArmourFilterStylesFormViewModel : CreViewModelBase
{
    private readonly IUserSettings _userSettings;

    private ICommand _toggleAlwaysActiveCommand;
    private ICommand _toggleAlwaysDisabledCommand;
    private ICommand _resetBodyArmourStylesCommand;

    private readonly string _defaultBackgroundColor = "#000000"; // Black
    private readonly string _defaultBorderColor = "#FFFFFF"; // White
    private readonly string _defaultTextColor = "#FFFF77"; // Yellow

    private readonly string _bodyArmourDefaultBackgroundColor = "#FFDD00FF"; // Pink
    private readonly string _bodyArmourDefaultBorderColor = "#FFFFFFFF"; // White
    private readonly string _bodyArmourDefaultTextColor = "#FFFFFFFF"; // White

    private string _previousBackgroundColor;
    private string _previousBorderColor;
    private string _previousTextColor;
    private string _iconFilename;

    public BodyArmourFilterStylesFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;

        UpdateIconFilename();
    }

    #region Commands

    public ICommand ToggleAlwaysActiveCommand => _toggleAlwaysActiveCommand ??= new RelayCommand(ToggleAlwaysActive);
    public ICommand ToggleAlwaysDisabledCommand => _toggleAlwaysDisabledCommand ??= new RelayCommand(ToggleAlwaysDisabled);
    public ICommand ResetBodyArmourStylesCommand => _resetBodyArmourStylesCommand ??= new RelayCommand(ResetBodyArmourStyles);

    #endregion

    #region Properties

    public string LootFilterStylesBodyArmourTextColor
    {
        get => _userSettings.LootFilterStylesBodyArmourTextColor;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourTextColor != value)
            {
                _userSettings.LootFilterStylesBodyArmourTextColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesBodyArmourBorderColor
    {
        get => _userSettings.LootFilterStylesBodyArmourBorderColor;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourBorderColor != value)
            {
                _userSettings.LootFilterStylesBodyArmourBorderColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesBodyArmourBackgroundColor
    {
        get => _userSettings.LootFilterStylesBodyArmourBackgroundColor;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourBackgroundColor != value)
            {
                _userSettings.LootFilterStylesBodyArmourBackgroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesBodyArmourTextFontSize
    {
        get => _userSettings.LootFilterStylesBodyArmourTextFontSize;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourTextFontSize != value)
            {
                _userSettings.LootFilterStylesBodyArmourTextFontSize = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBodyArmourAlwaysActive
    {
        get => _userSettings.LootFilterStylesBodyArmourAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourAlwaysActive != value)
            {
                _userSettings.LootFilterStylesBodyArmourAlwaysActive = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBodyArmourAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesBodyArmourAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesBodyArmourAlwaysDisabled = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBodyArmourMapIconEnabled
    {
        get => _userSettings.LootFilterStylesBodyArmourMapIconEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourMapIconEnabled != value)
            {
                _userSettings.LootFilterStylesBodyArmourMapIconEnabled = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesBodyArmourMapIconSize
    {
        get => _userSettings.LootFilterStylesBodyArmourMapIconSize;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourMapIconSize != value)
            {
                _userSettings.LootFilterStylesBodyArmourMapIconSize = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesBodyArmourMapIconColor
    {
        get => _userSettings.LootFilterStylesBodyArmourMapIconColor;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourMapIconColor != value)
            {
                _userSettings.LootFilterStylesBodyArmourMapIconColor = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesBodyArmourMapIconShape
    {
        get => _userSettings.LootFilterStylesBodyArmourMapIconShape;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourMapIconShape != value)
            {
                _userSettings.LootFilterStylesBodyArmourMapIconShape = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public bool LootFilterStylesBodyArmourBeamEnabled
    {
        get => _userSettings.LootFilterStylesBodyArmourBeamEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourBeamEnabled != value)
            {
                _userSettings.LootFilterStylesBodyArmourBeamEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesBodyArmourBeamColor
    {
        get => _userSettings.LootFilterStylesBodyArmourBeamColor;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourBeamColor != value)
            {
                _userSettings.LootFilterStylesBodyArmourBeamColor = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBodyArmourBeamTemporary
    {
        get => _userSettings.LootFilterStylesBodyArmourBeamTemporary;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourBeamTemporary != value)
            {
                _userSettings.LootFilterStylesBodyArmourBeamTemporary = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBodyArmourTextColorEnabled
    {
        get => _userSettings.LootFilterStylesBodyArmourTextColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourTextColorEnabled != value)
            {
                _userSettings.LootFilterStylesBodyArmourTextColorEnabled = value;
                OnPropertyChanged();
                ApplyTextColorSetting();
            }
        }
    }

    public bool LootFilterStylesBodyArmourBorderColorEnabled
    {
        get => _userSettings.LootFilterStylesBodyArmourBorderColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourBorderColorEnabled != value)
            {
                _userSettings.LootFilterStylesBodyArmourBorderColorEnabled = value;
                OnPropertyChanged();
                ApplyBorderColorSetting();
            }
        }
    }

    public bool LootFilterStylesBodyArmourBackgroundColorEnabled
    {
        get => _userSettings.LootFilterStylesBodyArmourBackgroundColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBodyArmourBackgroundColorEnabled != value)
            {
                _userSettings.LootFilterStylesBodyArmourBackgroundColorEnabled = value;
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
        if (LootFilterStylesBodyArmourAlwaysActive)
        {
            LootFilterStylesBodyArmourAlwaysDisabled = false;
        }
    }

    private void ToggleAlwaysDisabled()
    {
        if (LootFilterStylesBodyArmourAlwaysDisabled)
        {
            LootFilterStylesBodyArmourAlwaysActive = false;
        }
    }

    private void ResetBodyArmourStyles()
    {
        // Reset all settings to their default values
        LootFilterStylesBodyArmourTextColor = _bodyArmourDefaultTextColor; // White
        LootFilterStylesBodyArmourBorderColor = _bodyArmourDefaultBorderColor; // White
        LootFilterStylesBodyArmourBackgroundColor = _bodyArmourDefaultBackgroundColor; // Red
        LootFilterStylesBodyArmourTextFontSize = 45; // Default Font Size
        LootFilterStylesBodyArmourAlwaysActive = false;
        LootFilterStylesBodyArmourAlwaysDisabled = false;
        LootFilterStylesBodyArmourMapIconEnabled = true;
        LootFilterStylesBodyArmourMapIconSize = 2; // Small size
        LootFilterStylesBodyArmourMapIconColor = 10; // Yellow
        LootFilterStylesBodyArmourMapIconShape = 0; // Circle
        LootFilterStylesBodyArmourBeamEnabled = false;
        LootFilterStylesBodyArmourBeamColor = 10; // Yellow
        LootFilterStylesBodyArmourBeamTemporary = false;
        LootFilterStylesBodyArmourTextColorEnabled = true;
        LootFilterStylesBodyArmourBorderColorEnabled = true;
        LootFilterStylesBodyArmourBackgroundColorEnabled = true;

        UpdateIconFilename();

        // Notify UI of all changes
        OnPropertyChanged(string.Empty);
    }

    private void ApplyTextColorSetting()
    {
        if (!LootFilterStylesBodyArmourTextColorEnabled && LootFilterStylesBodyArmourTextColor is not null)
        {
            _previousTextColor = LootFilterStylesBodyArmourTextColor;
            LootFilterStylesBodyArmourTextColor = _defaultTextColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousTextColor is not null)
            {
                LootFilterStylesBodyArmourTextColor = _previousTextColor;
            }
            else
            {
                LootFilterStylesBodyArmourTextColor = _bodyArmourDefaultTextColor;
            }
        }
    }

    private void ApplyBorderColorSetting()
    {
        if (!LootFilterStylesBodyArmourBorderColorEnabled && LootFilterStylesBodyArmourBorderColor is not null)
        {
            _previousBorderColor = LootFilterStylesBodyArmourBorderColor;
            LootFilterStylesBodyArmourBorderColor = _defaultBorderColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousBorderColor is not null)
            {
                LootFilterStylesBodyArmourBorderColor = _previousBorderColor;
            }
            else
            {
                LootFilterStylesBodyArmourBorderColor = _bodyArmourDefaultBorderColor;
            }
        }
    }

    private void ApplyBackgroundColorSetting()
    {
        if (!LootFilterStylesBodyArmourBackgroundColorEnabled && LootFilterStylesBodyArmourBackgroundColor is not null)
        {
            _previousBackgroundColor = LootFilterStylesBodyArmourBackgroundColor;
            LootFilterStylesBodyArmourBackgroundColor = _defaultBackgroundColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set

            if (_previousTextColor is not null)
            {
                LootFilterStylesBodyArmourBackgroundColor = _previousBackgroundColor;
            }
            else
            {
                LootFilterStylesBodyArmourBackgroundColor = _bodyArmourDefaultBackgroundColor;
            }
        }
    }

    private void UpdateIconFilename()
    {
        string size = LootFilterStylesBodyArmourMapIconSize switch
        {
            0 => "Large",
            1 => "Medium",
            2 => "Small",
            _ => "Large" // Default to Medium
        };

        string color = LootFilterStylesBodyArmourMapIconColor switch
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

        string shape = LootFilterStylesBodyArmourMapIconShape switch
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
