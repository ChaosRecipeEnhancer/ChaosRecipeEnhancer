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

    private readonly string _defaultBackgroundColor = "#000000"; // Black
    private readonly string _defaultBorderColor = "#FFFFFF"; // White
    private readonly string _defaultTextColor = "#FFFF77"; // Yellow

    private readonly string _ringDefaultBackgroundColor = "#FFFF0000"; // Red
    private readonly string _ringDefaultBorderColor = "#FFFFFFFF"; // White
    private readonly string _ringDefaultTextColor = "#FFFFFFFF"; // White

    private string _previousBackgroundColor;
    private string _previousBorderColor;
    private string _previousTextColor;
    private string _iconFilename;

    public RingFilterStylesFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;

        UpdateIconFilename();
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
                UpdateIconFilename();
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
                UpdateIconFilename();
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
                UpdateIconFilename();
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
                UpdateIconFilename();
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

    public bool LootFilterStylesRingTextColorEnabled
    {
        get => _userSettings.LootFilterStylesRingTextColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesRingTextColorEnabled != value)
            {
                _userSettings.LootFilterStylesRingTextColorEnabled = value;
                OnPropertyChanged();
                ApplyTextColorSetting();
            }
        }
    }

    public bool LootFilterStylesRingBorderColorEnabled
    {
        get => _userSettings.LootFilterStylesRingBorderColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesRingBorderColorEnabled != value)
            {
                _userSettings.LootFilterStylesRingBorderColorEnabled = value;
                OnPropertyChanged();
                ApplyBorderColorSetting();
            }
        }
    }

    public bool LootFilterStylesRingBackgroundColorEnabled
    {
        get => _userSettings.LootFilterStylesRingBackgroundColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesRingBackgroundColorEnabled != value)
            {
                _userSettings.LootFilterStylesRingBackgroundColorEnabled = value;
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
        LootFilterStylesRingTextColor = _ringDefaultTextColor; // White
        LootFilterStylesRingBorderColor = _ringDefaultBorderColor; // White
        LootFilterStylesRingBackgroundColor = _ringDefaultBackgroundColor; // Red
        LootFilterStylesRingTextFontSize = 30; // Default Font Size
        LootFilterStylesRingAlwaysActive = false;
        LootFilterStylesRingAlwaysDisabled = false;
        LootFilterStylesRingMapIconEnabled = true;
        LootFilterStylesRingMapIconSize = 0; // Large size
        LootFilterStylesRingMapIconColor = 10; // Yellow
        LootFilterStylesRingMapIconShape = 0; // Circle
        LootFilterStylesRingBeamEnabled = false;
        LootFilterStylesRingBeamColor = 10; // Yellow
        LootFilterStylesRingBeamTemporary = false;
        LootFilterStylesRingTextColorEnabled = true;
        LootFilterStylesRingBorderColorEnabled = true;
        LootFilterStylesRingBackgroundColorEnabled = true;

        UpdateIconFilename();

        // Notify UI of all changes
        OnPropertyChanged(string.Empty);
    }

    private void ApplyTextColorSetting()
    {
        if (!LootFilterStylesRingTextColorEnabled && LootFilterStylesRingTextColor is not null)
        {
            _previousTextColor = LootFilterStylesRingTextColor;
            LootFilterStylesRingTextColor = _defaultTextColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousTextColor is not null)
            {
                LootFilterStylesRingTextColor = _previousTextColor;
            }
            else
            {
                LootFilterStylesRingTextColor = _ringDefaultTextColor;
            }
        }
    }

    private void ApplyBorderColorSetting()
    {
        if (!LootFilterStylesRingBorderColorEnabled && LootFilterStylesRingBorderColor is not null)
        {
            _previousBorderColor = LootFilterStylesRingBorderColor;
            LootFilterStylesRingBorderColor = _defaultBorderColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousBorderColor is not null)
            {
                LootFilterStylesRingBorderColor = _previousBorderColor;
            }
            else
            {
                LootFilterStylesRingBorderColor = _ringDefaultBorderColor;
            }
        }
    }

    private void ApplyBackgroundColorSetting()
    {
        if (!LootFilterStylesRingBackgroundColorEnabled && LootFilterStylesRingBackgroundColor is not null)
        {
            _previousBackgroundColor = LootFilterStylesRingBackgroundColor;
            LootFilterStylesRingBackgroundColor = _defaultBackgroundColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set

            if (_previousTextColor is not null)
            {
                LootFilterStylesRingBackgroundColor = _previousBackgroundColor;
            }
            else
            {
                LootFilterStylesRingBackgroundColor = _ringDefaultBackgroundColor;
            }
        }
    }

    private void UpdateIconFilename()
    {
        string size = LootFilterStylesRingMapIconSize switch
        {
            0 => "Large",
            1 => "Medium",
            2 => "Small",
            _ => "Large" // Default to Medium
        };

        string color = LootFilterStylesRingMapIconColor switch
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

        string shape = LootFilterStylesRingMapIconShape switch
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
