using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

// BodyArmourFilterStyleSettings
public partial class UserSettings : IUserSettings
{
    // Old Settings
    public string LootFilterBodyArmourColor
    {
        get => Settings.Default.LootFilterBodyArmourColor;
        set
        {
            if (Settings.Default.LootFilterBodyArmourColor != value)
            {
                Settings.Default.LootFilterBodyArmourColor = value;
                Save();
            }
        }
    }

    public bool LootFilterBodyArmourAlwaysActive
    {
        get => Settings.Default.LootFilterBodyArmourAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterBodyArmourAlwaysActive != value)
            {
                Settings.Default.LootFilterBodyArmourAlwaysActive = value;
                Save();
            }
        }
    }


    // New Settings
    public int LootFilterStylesBodyArmourTextFontSize
    {
        get => Settings.Default.LootFilterStylesBodyArmourTextFontSize;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourTextFontSize != value)
            {
                Settings.Default.LootFilterStylesBodyArmourTextFontSize = value;
                Save();
            }
        }
    }

    public string LootFilterStylesBodyArmourTextColor
    {
        get => Settings.Default.LootFilterStylesBodyArmourTextColor;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourTextColor != value)
            {
                Settings.Default.LootFilterStylesBodyArmourTextColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesBodyArmourBorderColor
    {
        get => Settings.Default.LootFilterStylesBodyArmourBorderColor;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourBorderColor != value)
            {
                Settings.Default.LootFilterStylesBodyArmourBorderColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesBodyArmourBackgroundColor
    {
        get => Settings.Default.LootFilterStylesBodyArmourBackgroundColor;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourBackgroundColor != value)
            {
                Settings.Default.LootFilterStylesBodyArmourBackgroundColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBodyArmourAlwaysActive
    {
        get => Settings.Default.LootFilterStylesBodyArmourAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourAlwaysActive != value)
            {
                Settings.Default.LootFilterStylesBodyArmourAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBodyArmourAlwaysDisabled
    {
        get => Settings.Default.LootFilterStylesBodyArmourAlwaysDisabled;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourAlwaysDisabled != value)
            {
                Settings.Default.LootFilterStylesBodyArmourAlwaysDisabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBodyArmourMapIconEnabled
    {
        get => Settings.Default.LootFilterStylesBodyArmourMapIconEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourMapIconEnabled != value)
            {
                Settings.Default.LootFilterStylesBodyArmourMapIconEnabled = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBodyArmourMapIconColor
    {
        get => Settings.Default.LootFilterStylesBodyArmourMapIconColor;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourMapIconColor != value)
            {
                Settings.Default.LootFilterStylesBodyArmourMapIconColor = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBodyArmourMapIconSize
    {
        get => Settings.Default.LootFilterStylesBodyArmourMapIconSize;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourMapIconSize != value)
            {
                Settings.Default.LootFilterStylesBodyArmourMapIconSize = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBodyArmourMapIconShape
    {
        get => Settings.Default.LootFilterStylesBodyArmourMapIconShape;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourMapIconShape != value)
            {
                Settings.Default.LootFilterStylesBodyArmourMapIconShape = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBodyArmourBeamEnabled
    {
        get => Settings.Default.LootFilterStylesBodyArmourBeamEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourBeamEnabled != value)
            {
                Settings.Default.LootFilterStylesBodyArmourBeamEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBodyArmourBeamTemporary
    {
        get => Settings.Default.LootFilterStylesBodyArmourBeamTemporary;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourBeamTemporary != value)
            {
                Settings.Default.LootFilterStylesBodyArmourBeamTemporary = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBodyArmourBeamColor
    {
        get => Settings.Default.LootFilterStylesBodyArmourBeamColor;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourBeamColor != value)
            {
                Settings.Default.LootFilterStylesBodyArmourBeamColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBodyArmourTextColorEnabled
    {
        get => Settings.Default.LootFilterStylesBodyArmourTextColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourTextColorEnabled != value)
            {
                Settings.Default.LootFilterStylesBodyArmourTextColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBodyArmourBorderColorEnabled
    {
        get => Settings.Default.LootFilterStylesBodyArmourBorderColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourBorderColorEnabled != value)
            {
                Settings.Default.LootFilterStylesBodyArmourBorderColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBodyArmourBackgroundColorEnabled
    {
        get => Settings.Default.LootFilterStylesBodyArmourBackgroundColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBodyArmourBackgroundColorEnabled != value)
            {
                Settings.Default.LootFilterStylesBodyArmourBackgroundColorEnabled = value;
                Save();
            }
        }
    }
}
