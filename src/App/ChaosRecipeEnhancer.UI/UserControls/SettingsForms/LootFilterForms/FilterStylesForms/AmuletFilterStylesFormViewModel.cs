using ChaosRecipeEnhancer.UI.Models.UserSettings;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public class AmuletFilterStylesFormViewModel : CreViewModelBase
{
    private readonly IUserSettings _userSettings;

    private ICommand _toggleAlwaysActiveCommand;
    private ICommand _toggleAlwaysDisabledCommand;
    private ICommand _resetAmuletStylesCommand;

    private readonly string _defaultBackgroundColor = "#000000"; // Black
    private readonly string _defaultBorderColor = "#FFFFFF"; // White
    private readonly string _defaultTextColor = "#FFFF77"; // Yellow

    private readonly string _amuletDefaultBackgroundColor = "#FFFF0000"; // Red
    private readonly string _amuletDefaultBorderColor = "#FFFFFFFF"; // White
    private readonly string _amuletDefaultTextColor = "#FFFFFFFF"; // White

    private string _previousBackgroundColor;
    private string _previousBorderColor;
    private string _previousTextColor;
    private string _iconFilename;

    public AmuletFilterStylesFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;

        UpdateIconFilename();
    }

    #region Commands

    public ICommand ToggleAlwaysActiveCommand => _toggleAlwaysActiveCommand ??= new RelayCommand(ToggleAlwaysActive);
    public ICommand ToggleAlwaysDisabledCommand => _toggleAlwaysDisabledCommand ??= new RelayCommand(ToggleAlwaysDisabled);
    public ICommand ResetAmuletStylesCommand => _resetAmuletStylesCommand ??= new RelayCommand(ResetAmuletStyles);

    #endregion

    #region Properties

    public string LootFilterStylesAmuletTextColor
    {
        get => _userSettings.LootFilterStylesAmuletTextColor;
        set
        {
            if (_userSettings.LootFilterStylesAmuletTextColor != value)
            {
                _userSettings.LootFilterStylesAmuletTextColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesAmuletBorderColor
    {
        get => _userSettings.LootFilterStylesAmuletBorderColor;
        set
        {
            if (_userSettings.LootFilterStylesAmuletBorderColor != value)
            {
                _userSettings.LootFilterStylesAmuletBorderColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesAmuletBackgroundColor
    {
        get => _userSettings.LootFilterStylesAmuletBackgroundColor;
        set
        {
            if (_userSettings.LootFilterStylesAmuletBackgroundColor != value)
            {
                _userSettings.LootFilterStylesAmuletBackgroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesAmuletTextFontSize
    {
        get => _userSettings.LootFilterStylesAmuletTextFontSize;
        set
        {
            if (_userSettings.LootFilterStylesAmuletTextFontSize != value)
            {
                _userSettings.LootFilterStylesAmuletTextFontSize = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesAmuletAlwaysActive
    {
        get => _userSettings.LootFilterStylesAmuletAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesAmuletAlwaysActive != value)
            {
                _userSettings.LootFilterStylesAmuletAlwaysActive = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesAmuletAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesAmuletAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesAmuletAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesAmuletAlwaysDisabled = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesAmuletMapIconEnabled
    {
        get => _userSettings.LootFilterStylesAmuletMapIconEnabled;
        set
        {
            if (_userSettings.LootFilterStylesAmuletMapIconEnabled != value)
            {
                _userSettings.LootFilterStylesAmuletMapIconEnabled = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesAmuletMapIconSize
    {
        get => _userSettings.LootFilterStylesAmuletMapIconSize;
        set
        {
            if (_userSettings.LootFilterStylesAmuletMapIconSize != value)
            {
                _userSettings.LootFilterStylesAmuletMapIconSize = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesAmuletMapIconColor
    {
        get => _userSettings.LootFilterStylesAmuletMapIconColor;
        set
        {
            if (_userSettings.LootFilterStylesAmuletMapIconColor != value)
            {
                _userSettings.LootFilterStylesAmuletMapIconColor = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesAmuletMapIconShape
    {
        get => _userSettings.LootFilterStylesAmuletMapIconShape;
        set
        {
            if (_userSettings.LootFilterStylesAmuletMapIconShape != value)
            {
                _userSettings.LootFilterStylesAmuletMapIconShape = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public bool LootFilterStylesAmuletBeamEnabled
    {
        get => _userSettings.LootFilterStylesAmuletBeamEnabled;
        set
        {
            if (_userSettings.LootFilterStylesAmuletBeamEnabled != value)
            {
                _userSettings.LootFilterStylesAmuletBeamEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesAmuletBeamColor
    {
        get => _userSettings.LootFilterStylesAmuletBeamColor;
        set
        {
            if (_userSettings.LootFilterStylesAmuletBeamColor != value)
            {
                _userSettings.LootFilterStylesAmuletBeamColor = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesAmuletBeamTemporary
    {
        get => _userSettings.LootFilterStylesAmuletBeamTemporary;
        set
        {
            if (_userSettings.LootFilterStylesAmuletBeamTemporary != value)
            {
                _userSettings.LootFilterStylesAmuletBeamTemporary = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesAmuletTextColorEnabled
    {
        get => _userSettings.LootFilterStylesAmuletTextColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesAmuletTextColorEnabled != value)
            {
                _userSettings.LootFilterStylesAmuletTextColorEnabled = value;
                OnPropertyChanged();
                ApplyTextColorSetting();
            }
        }
    }

    public bool LootFilterStylesAmuletBorderColorEnabled
    {
        get => _userSettings.LootFilterStylesAmuletBorderColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesAmuletBorderColorEnabled != value)
            {
                _userSettings.LootFilterStylesAmuletBorderColorEnabled = value;
                OnPropertyChanged();
                ApplyBorderColorSetting();
            }
        }
    }

    public bool LootFilterStylesAmuletBackgroundColorEnabled
    {
        get => _userSettings.LootFilterStylesAmuletBackgroundColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesAmuletBackgroundColorEnabled != value)
            {
                _userSettings.LootFilterStylesAmuletBackgroundColorEnabled = value;
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
        if (LootFilterStylesAmuletAlwaysActive)
        {
            LootFilterStylesAmuletAlwaysDisabled = false;
        }
    }

    private void ToggleAlwaysDisabled()
    {
        if (LootFilterStylesAmuletAlwaysDisabled)
        {
            LootFilterStylesAmuletAlwaysActive = false;
        }
    }

    private void ResetAmuletStyles()
    {
        // Reset all settings to their default values
        LootFilterStylesAmuletTextColor = _amuletDefaultTextColor; // White
        LootFilterStylesAmuletBorderColor = _amuletDefaultBorderColor; // White
        LootFilterStylesAmuletBackgroundColor = _amuletDefaultBackgroundColor; // Red
        LootFilterStylesAmuletTextFontSize = 30; // Default Font Size
        LootFilterStylesAmuletAlwaysActive = false;
        LootFilterStylesAmuletAlwaysDisabled = false;
        LootFilterStylesAmuletMapIconEnabled = true;
        LootFilterStylesAmuletMapIconSize = 0; // Large size
        LootFilterStylesAmuletMapIconColor = 10; // Yellow
        LootFilterStylesAmuletMapIconShape = 0; // Circle
        LootFilterStylesAmuletBeamEnabled = false;
        LootFilterStylesAmuletBeamColor = 10; // Yellow
        LootFilterStylesAmuletBeamTemporary = false;
        LootFilterStylesAmuletTextColorEnabled = true;
        LootFilterStylesAmuletBorderColorEnabled = true;
        LootFilterStylesAmuletBackgroundColorEnabled = true;

        UpdateIconFilename();

        // Notify UI of all changes
        OnPropertyChanged(string.Empty);
    }

    private void ApplyTextColorSetting()
    {
        if (!LootFilterStylesAmuletTextColorEnabled && LootFilterStylesAmuletTextColor is not null)
        {
            _previousTextColor = LootFilterStylesAmuletTextColor;
            LootFilterStylesAmuletTextColor = _defaultTextColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousTextColor is not null)
            {
                LootFilterStylesAmuletTextColor = _previousTextColor;
            }
            else
            {
                LootFilterStylesAmuletTextColor = _amuletDefaultTextColor;
            }
        }
    }

    private void ApplyBorderColorSetting()
    {
        if (!LootFilterStylesAmuletBorderColorEnabled && LootFilterStylesAmuletBorderColor is not null)
        {
            _previousBorderColor = LootFilterStylesAmuletBorderColor;
            LootFilterStylesAmuletBorderColor = _defaultBorderColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousBorderColor is not null)
            {
                LootFilterStylesAmuletBorderColor = _previousBorderColor;
            }
            else
            {
                LootFilterStylesAmuletBorderColor = _amuletDefaultBorderColor;
            }
        }
    }

    private void ApplyBackgroundColorSetting()
    {
        if (!LootFilterStylesAmuletBackgroundColorEnabled && LootFilterStylesAmuletBackgroundColor is not null)
        {
            _previousBackgroundColor = LootFilterStylesAmuletBackgroundColor;
            LootFilterStylesAmuletBackgroundColor = _defaultBackgroundColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set

            if (_previousTextColor is not null)
            {
                LootFilterStylesAmuletBackgroundColor = _previousBackgroundColor;
            }
            else
            {
                LootFilterStylesAmuletBackgroundColor = _amuletDefaultBackgroundColor;
            }
        }
    }

    private void UpdateIconFilename()
    {
        string size = LootFilterStylesAmuletMapIconSize switch
        {
            0 => "Large",
            1 => "Medium",
            2 => "Small",
            _ => "Large" // Default to Large
        };

        string color = LootFilterStylesAmuletMapIconColor switch
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
            _ => "Yellow" // Default to Yellow
        };

        string shape = LootFilterStylesAmuletMapIconShape switch
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
