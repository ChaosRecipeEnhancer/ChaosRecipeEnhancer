using ChaosRecipeEnhancer.UI.Models.UserSettings;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public class GlovesFilterStylesFormViewModel : CreViewModelBase
{
    private readonly IUserSettings _userSettings;

    private ICommand _toggleAlwaysActiveCommand;
    private ICommand _toggleAlwaysDisabledCommand;
    private ICommand _resetGlovesStylesCommand;

    private readonly string _defaultBackgroundColor = "#000000"; // Black
    private readonly string _defaultBorderColor = "#FFFFFF"; // White
    private readonly string _defaultTextColor = "#FFFF77"; // Yellow

    private readonly string _glovesDefaultBackgroundColor = "#FF1ABD17"; // Green
    private readonly string _glovesDefaultBorderColor = "#FFFFFFFF"; // White
    private readonly string _glovesDefaultTextColor = "#FFFFFFFF"; // White

    private string _previousBackgroundColor;
    private string _previousBorderColor;
    private string _previousTextColor;
    private string _iconFilename;

    public GlovesFilterStylesFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;

        UpdateIconFilename();
    }

    #region Commands

    public ICommand ToggleAlwaysActiveCommand => _toggleAlwaysActiveCommand ??= new RelayCommand(ToggleAlwaysActive);
    public ICommand ToggleAlwaysDisabledCommand => _toggleAlwaysDisabledCommand ??= new RelayCommand(ToggleAlwaysDisabled);
    public ICommand ResetGlovesStylesCommand => _resetGlovesStylesCommand ??= new RelayCommand(ResetGlovesStyles);

    #endregion

    #region Properties

    public string LootFilterStylesGlovesTextColor
    {
        get => _userSettings.LootFilterStylesGlovesTextColor;
        set
        {
            if (_userSettings.LootFilterStylesGlovesTextColor != value)
            {
                _userSettings.LootFilterStylesGlovesTextColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesGlovesBorderColor
    {
        get => _userSettings.LootFilterStylesGlovesBorderColor;
        set
        {
            if (_userSettings.LootFilterStylesGlovesBorderColor != value)
            {
                _userSettings.LootFilterStylesGlovesBorderColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesGlovesBackgroundColor
    {
        get => _userSettings.LootFilterStylesGlovesBackgroundColor;
        set
        {
            if (_userSettings.LootFilterStylesGlovesBackgroundColor != value)
            {
                _userSettings.LootFilterStylesGlovesBackgroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesGlovesTextFontSize
    {
        get => _userSettings.LootFilterStylesGlovesTextFontSize;
        set
        {
            if (_userSettings.LootFilterStylesGlovesTextFontSize != value)
            {
                _userSettings.LootFilterStylesGlovesTextFontSize = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesGlovesAlwaysActive
    {
        get => _userSettings.LootFilterStylesGlovesAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesGlovesAlwaysActive != value)
            {
                _userSettings.LootFilterStylesGlovesAlwaysActive = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesGlovesAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesGlovesAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesGlovesAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesGlovesAlwaysDisabled = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesGlovesMapIconEnabled
    {
        get => _userSettings.LootFilterStylesGlovesMapIconEnabled;
        set
        {
            if (_userSettings.LootFilterStylesGlovesMapIconEnabled != value)
            {
                _userSettings.LootFilterStylesGlovesMapIconEnabled = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesGlovesMapIconSize
    {
        get => _userSettings.LootFilterStylesGlovesMapIconSize;
        set
        {
            if (_userSettings.LootFilterStylesGlovesMapIconSize != value)
            {
                _userSettings.LootFilterStylesGlovesMapIconSize = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesGlovesMapIconColor
    {
        get => _userSettings.LootFilterStylesGlovesMapIconColor;
        set
        {
            if (_userSettings.LootFilterStylesGlovesMapIconColor != value)
            {
                _userSettings.LootFilterStylesGlovesMapIconColor = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesGlovesMapIconShape
    {
        get => _userSettings.LootFilterStylesGlovesMapIconShape;
        set
        {
            if (_userSettings.LootFilterStylesGlovesMapIconShape != value)
            {
                _userSettings.LootFilterStylesGlovesMapIconShape = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public bool LootFilterStylesGlovesBeamEnabled
    {
        get => _userSettings.LootFilterStylesGlovesBeamEnabled;
        set
        {
            if (_userSettings.LootFilterStylesGlovesBeamEnabled != value)
            {
                _userSettings.LootFilterStylesGlovesBeamEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesGlovesBeamColor
    {
        get => _userSettings.LootFilterStylesGlovesBeamColor;
        set
        {
            if (_userSettings.LootFilterStylesGlovesBeamColor != value)
            {
                _userSettings.LootFilterStylesGlovesBeamColor = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesGlovesBeamTemporary
    {
        get => _userSettings.LootFilterStylesGlovesBeamTemporary;
        set
        {
            if (_userSettings.LootFilterStylesGlovesBeamTemporary != value)
            {
                _userSettings.LootFilterStylesGlovesBeamTemporary = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesGlovesTextColorEnabled
    {
        get => _userSettings.LootFilterStylesGlovesTextColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesGlovesTextColorEnabled != value)
            {
                _userSettings.LootFilterStylesGlovesTextColorEnabled = value;
                OnPropertyChanged();
                ApplyTextColorSetting();
            }
        }
    }

    public bool LootFilterStylesGlovesBorderColorEnabled
    {
        get => _userSettings.LootFilterStylesGlovesBorderColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesGlovesBorderColorEnabled != value)
            {
                _userSettings.LootFilterStylesGlovesBorderColorEnabled = value;
                OnPropertyChanged();
                ApplyBorderColorSetting();
            }
        }
    }

    public bool LootFilterStylesGlovesBackgroundColorEnabled
    {
        get => _userSettings.LootFilterStylesGlovesBackgroundColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesGlovesBackgroundColorEnabled != value)
            {
                _userSettings.LootFilterStylesGlovesBackgroundColorEnabled = value;
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
        if (LootFilterStylesGlovesAlwaysActive)
        {
            LootFilterStylesGlovesAlwaysDisabled = false;
        }
    }

    private void ToggleAlwaysDisabled()
    {
        if (LootFilterStylesGlovesAlwaysDisabled)
        {
            LootFilterStylesGlovesAlwaysActive = false;
        }
    }

    private void ResetGlovesStyles()
    {
        // Reset all settings to their default values
        LootFilterStylesGlovesTextColor = _glovesDefaultTextColor; // White
        LootFilterStylesGlovesBorderColor = _glovesDefaultBorderColor; // White
        LootFilterStylesGlovesBackgroundColor = _glovesDefaultBackgroundColor; // Red
        LootFilterStylesGlovesTextFontSize = 45; // Default Font Size
        LootFilterStylesGlovesAlwaysActive = false;
        LootFilterStylesGlovesAlwaysDisabled = false;
        LootFilterStylesGlovesMapIconEnabled = true;
        LootFilterStylesGlovesMapIconSize = 0; // Large size
        LootFilterStylesGlovesMapIconColor = 10; // Yellow
        LootFilterStylesGlovesMapIconShape = 0; // Circle
        LootFilterStylesGlovesBeamEnabled = false;
        LootFilterStylesGlovesBeamColor = 10; // Yellow
        LootFilterStylesGlovesBeamTemporary = false;
        LootFilterStylesGlovesTextColorEnabled = true;
        LootFilterStylesGlovesBorderColorEnabled = true;
        LootFilterStylesGlovesBackgroundColorEnabled = true;

        UpdateIconFilename();

        // Notify UI of all changes
        OnPropertyChanged(string.Empty);
    }

    private void ApplyTextColorSetting()
    {
        if (!LootFilterStylesGlovesTextColorEnabled && LootFilterStylesGlovesTextColor is not null)
        {
            _previousTextColor = LootFilterStylesGlovesTextColor;
            LootFilterStylesGlovesTextColor = _defaultTextColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousTextColor is not null)
            {
                LootFilterStylesGlovesTextColor = _previousTextColor;
            }
            else
            {
                LootFilterStylesGlovesTextColor = _glovesDefaultTextColor;
            }
        }
    }

    private void ApplyBorderColorSetting()
    {
        if (!LootFilterStylesGlovesBorderColorEnabled && LootFilterStylesGlovesBorderColor is not null)
        {
            _previousBorderColor = LootFilterStylesGlovesBorderColor;
            LootFilterStylesGlovesBorderColor = _defaultBorderColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousBorderColor is not null)
            {
                LootFilterStylesGlovesBorderColor = _previousBorderColor;
            }
            else
            {
                LootFilterStylesGlovesBorderColor = _glovesDefaultBorderColor;
            }
        }
    }

    private void ApplyBackgroundColorSetting()
    {
        if (!LootFilterStylesGlovesBackgroundColorEnabled && LootFilterStylesGlovesBackgroundColor is not null)
        {
            _previousBackgroundColor = LootFilterStylesGlovesBackgroundColor;
            LootFilterStylesGlovesBackgroundColor = _defaultBackgroundColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set

            if (_previousTextColor is not null)
            {
                LootFilterStylesGlovesBackgroundColor = _previousBackgroundColor;
            }
            else
            {
                LootFilterStylesGlovesBackgroundColor = _glovesDefaultBackgroundColor;
            }
        }
    }

    private void UpdateIconFilename()
    {
        string size = LootFilterStylesGlovesMapIconSize switch
        {
            0 => "Large",
            1 => "Medium",
            2 => "Small",
            _ => "Large" // Default to Medium
        };

        string color = LootFilterStylesGlovesMapIconColor switch
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

        string shape = LootFilterStylesGlovesMapIconShape switch
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
