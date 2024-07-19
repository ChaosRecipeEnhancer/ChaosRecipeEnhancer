using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

// BeltFilterStyleSettings
public partial class UserSettings : IUserSettings
{
    // Old Settings
    public string LootFilterBeltColor
    {
        get => Settings.Default.LootFilterBeltColor;
        set
        {
            if (Settings.Default.LootFilterBeltColor != value)
            {
                Settings.Default.LootFilterBeltColor = value;
                Save();
            }
        }
    }

    public bool LootFilterBeltsAlwaysActive
    {
        get => Settings.Default.LootFilterBeltsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterBeltsAlwaysActive != value)
            {
                Settings.Default.LootFilterBeltsAlwaysActive = value;
                Save();
            }
        }
    }

    // New Settings
    public int LootFilterStylesBeltTextFontSize
    {
        get => Settings.Default.LootFilterStylesBeltTextFontSize;
        set
        {
            if (Settings.Default.LootFilterStylesBeltTextFontSize != value)
            {
                Settings.Default.LootFilterStylesBeltTextFontSize = value;
                Save();
            }
        }
    }

    public string LootFilterStylesBeltTextColor
    {
        get => Settings.Default.LootFilterStylesBeltTextColor;
        set
        {
            if (Settings.Default.LootFilterStylesBeltTextColor != value)
            {
                Settings.Default.LootFilterStylesBeltTextColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesBeltBorderColor
    {
        get => Settings.Default.LootFilterStylesBeltBorderColor;
        set
        {
            if (Settings.Default.LootFilterStylesBeltBorderColor != value)
            {
                Settings.Default.LootFilterStylesBeltBorderColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesBeltBackgroundColor
    {
        get => Settings.Default.LootFilterStylesBeltBackgroundColor;
        set
        {
            if (Settings.Default.LootFilterStylesBeltBackgroundColor != value)
            {
                Settings.Default.LootFilterStylesBeltBackgroundColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBeltAlwaysActive
    {
        get => Settings.Default.LootFilterStylesBeltAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterStylesBeltAlwaysActive != value)
            {
                Settings.Default.LootFilterStylesBeltAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBeltAlwaysDisabled
    {
        get => Settings.Default.LootFilterStylesBeltAlwaysDisabled;
        set
        {
            if (Settings.Default.LootFilterStylesBeltAlwaysDisabled != value)
            {
                Settings.Default.LootFilterStylesBeltAlwaysDisabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBeltMapIconEnabled
    {
        get => Settings.Default.LootFilterStylesBeltMapIconEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBeltMapIconEnabled != value)
            {
                Settings.Default.LootFilterStylesBeltMapIconEnabled = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBeltMapIconColor
    {
        get => Settings.Default.LootFilterStylesBeltMapIconColor;
        set
        {
            if (Settings.Default.LootFilterStylesBeltMapIconColor != value)
            {
                Settings.Default.LootFilterStylesBeltMapIconColor = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBeltMapIconSize
    {
        get => Settings.Default.LootFilterStylesBeltMapIconSize;
        set
        {
            if (Settings.Default.LootFilterStylesBeltMapIconSize != value)
            {
                Settings.Default.LootFilterStylesBeltMapIconSize = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBeltMapIconShape
    {
        get => Settings.Default.LootFilterStylesBeltMapIconShape;
        set
        {
            if (Settings.Default.LootFilterStylesBeltMapIconShape != value)
            {
                Settings.Default.LootFilterStylesBeltMapIconShape = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBeltBeamEnabled
    {
        get => Settings.Default.LootFilterStylesBeltBeamEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBeltBeamEnabled != value)
            {
                Settings.Default.LootFilterStylesBeltBeamEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBeltBeamTemporary
    {
        get => Settings.Default.LootFilterStylesBeltBeamTemporary;
        set
        {
            if (Settings.Default.LootFilterStylesBeltBeamTemporary != value)
            {
                Settings.Default.LootFilterStylesBeltBeamTemporary = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBeltBeamColor
    {
        get => Settings.Default.LootFilterStylesBeltBeamColor;
        set
        {
            if (Settings.Default.LootFilterStylesBeltBeamColor != value)
            {
                Settings.Default.LootFilterStylesBeltBeamColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBeltTextColorEnabled
    {
        get => Settings.Default.LootFilterStylesBeltTextColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBeltTextColorEnabled != value)
            {
                Settings.Default.LootFilterStylesBeltTextColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBeltBorderColorEnabled
    {
        get => Settings.Default.LootFilterStylesBeltBorderColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBeltBorderColorEnabled != value)
            {
                Settings.Default.LootFilterStylesBeltBorderColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBeltBackgroundColorEnabled
    {
        get => Settings.Default.LootFilterStylesBeltBackgroundColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBeltBackgroundColorEnabled != value)
            {
                Settings.Default.LootFilterStylesBeltBackgroundColorEnabled = value;
                Save();
            }
        }
    }
}

