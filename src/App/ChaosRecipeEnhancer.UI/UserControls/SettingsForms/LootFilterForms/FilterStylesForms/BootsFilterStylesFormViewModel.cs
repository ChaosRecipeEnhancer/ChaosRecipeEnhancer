using ChaosRecipeEnhancer.UI.Models.UserSettings;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.UserControls.SettingsForms.LootFilterForms.FilterStylesForms;

public class BootsFilterStylesFormViewModel : CreViewModelBase
{
    private readonly IUserSettings _userSettings;

    private ICommand _toggleAlwaysActiveCommand;
    private ICommand _toggleAlwaysDisabledCommand;
    private ICommand _resetBootsStylesCommand;

    private readonly string _defaultBackgroundColor = "#000000"; // Black
    private readonly string _defaultBorderColor = "#FFFFFF"; // White
    private readonly string _defaultTextColor = "#FFFF77"; // Yellow

    private readonly string _bootsDefaultBackgroundColor = "#FF0018FF"; // Blue
    private readonly string _bootsDefaultBorderColor = "#FFFFFFFF"; // White
    private readonly string _bootsDefaultTextColor = "#FFFFFFFF"; // White

    private string _previousBackgroundColor;
    private string _previousBorderColor;
    private string _previousTextColor;
    private string _iconFilename;

    public BootsFilterStylesFormViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;

        UpdateIconFilename();
    }

    #region Commands

    public ICommand ToggleAlwaysActiveCommand => _toggleAlwaysActiveCommand ??= new RelayCommand(ToggleAlwaysActive);
    public ICommand ToggleAlwaysDisabledCommand => _toggleAlwaysDisabledCommand ??= new RelayCommand(ToggleAlwaysDisabled);
    public ICommand ResetBootsStylesCommand => _resetBootsStylesCommand ??= new RelayCommand(ResetBootsStyles);

    #endregion

    #region Properties

    public string LootFilterStylesBootsTextColor
    {
        get => _userSettings.LootFilterStylesBootsTextColor;
        set
        {
            if (_userSettings.LootFilterStylesBootsTextColor != value)
            {
                _userSettings.LootFilterStylesBootsTextColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesBootsBorderColor
    {
        get => _userSettings.LootFilterStylesBootsBorderColor;
        set
        {
            if (_userSettings.LootFilterStylesBootsBorderColor != value)
            {
                _userSettings.LootFilterStylesBootsBorderColor = value;
                OnPropertyChanged();
            }
        }
    }

    public string LootFilterStylesBootsBackgroundColor
    {
        get => _userSettings.LootFilterStylesBootsBackgroundColor;
        set
        {
            if (_userSettings.LootFilterStylesBootsBackgroundColor != value)
            {
                _userSettings.LootFilterStylesBootsBackgroundColor = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesBootsTextFontSize
    {
        get => _userSettings.LootFilterStylesBootsTextFontSize;
        set
        {
            if (_userSettings.LootFilterStylesBootsTextFontSize != value)
            {
                _userSettings.LootFilterStylesBootsTextFontSize = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBootsAlwaysActive
    {
        get => _userSettings.LootFilterStylesBootsAlwaysActive;
        set
        {
            if (_userSettings.LootFilterStylesBootsAlwaysActive != value)
            {
                _userSettings.LootFilterStylesBootsAlwaysActive = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBootsAlwaysDisabled
    {
        get => _userSettings.LootFilterStylesBootsAlwaysDisabled;
        set
        {
            if (_userSettings.LootFilterStylesBootsAlwaysDisabled != value)
            {
                _userSettings.LootFilterStylesBootsAlwaysDisabled = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBootsMapIconEnabled
    {
        get => _userSettings.LootFilterStylesBootsMapIconEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBootsMapIconEnabled != value)
            {
                _userSettings.LootFilterStylesBootsMapIconEnabled = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesBootsMapIconSize
    {
        get => _userSettings.LootFilterStylesBootsMapIconSize;
        set
        {
            if (_userSettings.LootFilterStylesBootsMapIconSize != value)
            {
                _userSettings.LootFilterStylesBootsMapIconSize = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesBootsMapIconColor
    {
        get => _userSettings.LootFilterStylesBootsMapIconColor;
        set
        {
            if (_userSettings.LootFilterStylesBootsMapIconColor != value)
            {
                _userSettings.LootFilterStylesBootsMapIconColor = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public int LootFilterStylesBootsMapIconShape
    {
        get => _userSettings.LootFilterStylesBootsMapIconShape;
        set
        {
            if (_userSettings.LootFilterStylesBootsMapIconShape != value)
            {
                _userSettings.LootFilterStylesBootsMapIconShape = value;
                OnPropertyChanged();
                UpdateIconFilename();
            }
        }
    }

    public bool LootFilterStylesBootsBeamEnabled
    {
        get => _userSettings.LootFilterStylesBootsBeamEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBootsBeamEnabled != value)
            {
                _userSettings.LootFilterStylesBootsBeamEnabled = value;
                OnPropertyChanged();
            }
        }
    }

    public int LootFilterStylesBootsBeamColor
    {
        get => _userSettings.LootFilterStylesBootsBeamColor;
        set
        {
            if (_userSettings.LootFilterStylesBootsBeamColor != value)
            {
                _userSettings.LootFilterStylesBootsBeamColor = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBootsBeamTemporary
    {
        get => _userSettings.LootFilterStylesBootsBeamTemporary;
        set
        {
            if (_userSettings.LootFilterStylesBootsBeamTemporary != value)
            {
                _userSettings.LootFilterStylesBootsBeamTemporary = value;
                OnPropertyChanged();
            }
        }
    }

    public bool LootFilterStylesBootsTextColorEnabled
    {
        get => _userSettings.LootFilterStylesBootsTextColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBootsTextColorEnabled != value)
            {
                _userSettings.LootFilterStylesBootsTextColorEnabled = value;
                OnPropertyChanged();
                ApplyTextColorSetting();
            }
        }
    }

    public bool LootFilterStylesBootsBorderColorEnabled
    {
        get => _userSettings.LootFilterStylesBootsBorderColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBootsBorderColorEnabled != value)
            {
                _userSettings.LootFilterStylesBootsBorderColorEnabled = value;
                OnPropertyChanged();
                ApplyBorderColorSetting();
            }
        }
    }

    public bool LootFilterStylesBootsBackgroundColorEnabled
    {
        get => _userSettings.LootFilterStylesBootsBackgroundColorEnabled;
        set
        {
            if (_userSettings.LootFilterStylesBootsBackgroundColorEnabled != value)
            {
                _userSettings.LootFilterStylesBootsBackgroundColorEnabled = value;
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
        if (LootFilterStylesBootsAlwaysActive)
        {
            LootFilterStylesBootsAlwaysDisabled = false;
        }
    }

    private void ToggleAlwaysDisabled()
    {
        if (LootFilterStylesBootsAlwaysDisabled)
        {
            LootFilterStylesBootsAlwaysActive = false;
        }
    }

    private void ResetBootsStyles()
    {
        // Reset all settings to their default values
        LootFilterStylesBootsTextColor = _bootsDefaultTextColor; // White
        LootFilterStylesBootsBorderColor = _bootsDefaultBorderColor; // White
        LootFilterStylesBootsBackgroundColor = _bootsDefaultBackgroundColor; // Red
        LootFilterStylesBootsTextFontSize = 30; // Default Font Size
        LootFilterStylesBootsAlwaysActive = false;
        LootFilterStylesBootsAlwaysDisabled = false;
        LootFilterStylesBootsMapIconEnabled = true;
        LootFilterStylesBootsMapIconSize = 0; // Large size
        LootFilterStylesBootsMapIconColor = 10; // Yellow
        LootFilterStylesBootsMapIconShape = 0; // Circle
        LootFilterStylesBootsBeamEnabled = false;
        LootFilterStylesBootsBeamColor = 10; // Yellow
        LootFilterStylesBootsBeamTemporary = false;
        LootFilterStylesBootsTextColorEnabled = true;
        LootFilterStylesBootsBorderColorEnabled = true;
        LootFilterStylesBootsBackgroundColorEnabled = true;

        UpdateIconFilename();

        // Notify UI of all changes
        OnPropertyChanged(string.Empty);
    }

    private void ApplyTextColorSetting()
    {
        if (!LootFilterStylesBootsTextColorEnabled && LootFilterStylesBootsTextColor is not null)
        {
            _previousTextColor = LootFilterStylesBootsTextColor;
            LootFilterStylesBootsTextColor = _defaultTextColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousTextColor is not null)
            {
                LootFilterStylesBootsTextColor = _previousTextColor;
            }
            else
            {
                LootFilterStylesBootsTextColor = _bootsDefaultTextColor;
            }
        }
    }

    private void ApplyBorderColorSetting()
    {
        if (!LootFilterStylesBootsBorderColorEnabled && LootFilterStylesBootsBorderColor is not null)
        {
            _previousBorderColor = LootFilterStylesBootsBorderColor;
            LootFilterStylesBootsBorderColor = _defaultBorderColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set
            if (_previousBorderColor is not null)
            {
                LootFilterStylesBootsBorderColor = _previousBorderColor;
            }
            else
            {
                LootFilterStylesBootsBorderColor = _bootsDefaultBorderColor;
            }
        }
    }

    private void ApplyBackgroundColorSetting()
    {
        if (!LootFilterStylesBootsBackgroundColorEnabled && LootFilterStylesBootsBackgroundColor is not null)
        {
            _previousBackgroundColor = LootFilterStylesBootsBackgroundColor;
            LootFilterStylesBootsBackgroundColor = _defaultBackgroundColor;
        }
        else
        {
            // Restore to custom color if previously disabled and custom color was set

            if (_previousTextColor is not null)
            {
                LootFilterStylesBootsBackgroundColor = _previousBackgroundColor;
            }
            else
            {
                LootFilterStylesBootsBackgroundColor = _bootsDefaultBackgroundColor;
            }
        }
    }

    private void UpdateIconFilename()
    {
        string size = LootFilterStylesBootsMapIconSize switch
        {
            0 => "Large",
            1 => "Medium",
            2 => "Small",
            _ => "Large" // Default to Medium
        };

        string color = LootFilterStylesBootsMapIconColor switch
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

        string shape = LootFilterStylesBootsMapIconShape switch
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
