using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

// GlovesFilterStyleSettings
public partial class UserSettings : IUserSettings
{
    // Old Settings
    public string LootFilterGlovesColor
    {
        get => Settings.Default.LootFilterGlovesColor;
        set
        {
            if (Settings.Default.LootFilterGlovesColor != value)
            {
                Settings.Default.LootFilterGlovesColor = value;
                Save();
            }
        }
    }

    public bool LootFilterGlovesAlwaysActive
    {
        get => Settings.Default.LootFilterGlovesAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterGlovesAlwaysActive != value)
            {
                Settings.Default.LootFilterGlovesAlwaysActive = value;
                Save();
            }
        }
    }

    // New Settings
    public int LootFilterStylesGlovesTextFontSize
    {
        get => Settings.Default.LootFilterStylesGlovesTextFontSize;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesTextFontSize != value)
            {
                Settings.Default.LootFilterStylesGlovesTextFontSize = value;
                Save();
            }
        }
    }

    public string LootFilterStylesGlovesTextColor
    {
        get => Settings.Default.LootFilterStylesGlovesTextColor;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesTextColor != value)
            {
                Settings.Default.LootFilterStylesGlovesTextColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesGlovesBorderColor
    {
        get => Settings.Default.LootFilterStylesGlovesBorderColor;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesBorderColor != value)
            {
                Settings.Default.LootFilterStylesGlovesBorderColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesGlovesBackgroundColor
    {
        get => Settings.Default.LootFilterStylesGlovesBackgroundColor;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesBackgroundColor != value)
            {
                Settings.Default.LootFilterStylesGlovesBackgroundColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesGlovesAlwaysActive
    {
        get => Settings.Default.LootFilterStylesGlovesAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesAlwaysActive != value)
            {
                Settings.Default.LootFilterStylesGlovesAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesGlovesAlwaysDisabled
    {
        get => Settings.Default.LootFilterStylesGlovesAlwaysDisabled;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesAlwaysDisabled != value)
            {
                Settings.Default.LootFilterStylesGlovesAlwaysDisabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesGlovesMapIconEnabled
    {
        get => Settings.Default.LootFilterStylesGlovesMapIconEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesMapIconEnabled != value)
            {
                Settings.Default.LootFilterStylesGlovesMapIconEnabled = value;
                Save();
            }
        }
    }

    public int LootFilterStylesGlovesMapIconColor
    {
        get => Settings.Default.LootFilterStylesGlovesMapIconColor;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesMapIconColor != value)
            {
                Settings.Default.LootFilterStylesGlovesMapIconColor = value;
                Save();
            }
        }
    }

    public int LootFilterStylesGlovesMapIconSize
    {
        get => Settings.Default.LootFilterStylesGlovesMapIconSize;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesMapIconSize != value)
            {
                Settings.Default.LootFilterStylesGlovesMapIconSize = value;
                Save();
            }
        }
    }

    public int LootFilterStylesGlovesMapIconShape
    {
        get => Settings.Default.LootFilterStylesGlovesMapIconShape;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesMapIconShape != value)
            {
                Settings.Default.LootFilterStylesGlovesMapIconShape = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesGlovesBeamEnabled
    {
        get => Settings.Default.LootFilterStylesGlovesBeamEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesBeamEnabled != value)
            {
                Settings.Default.LootFilterStylesGlovesBeamEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesGlovesBeamTemporary
    {
        get => Settings.Default.LootFilterStylesGlovesBeamTemporary;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesBeamTemporary != value)
            {
                Settings.Default.LootFilterStylesGlovesBeamTemporary = value;
                Save();
            }
        }
    }

    public int LootFilterStylesGlovesBeamColor
    {
        get => Settings.Default.LootFilterStylesGlovesBeamColor;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesBeamColor != value)
            {
                Settings.Default.LootFilterStylesGlovesBeamColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesGlovesTextColorEnabled
    {
        get => Settings.Default.LootFilterStylesGlovesTextColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesTextColorEnabled != value)
            {
                Settings.Default.LootFilterStylesGlovesTextColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesGlovesBorderColorEnabled
    {
        get => Settings.Default.LootFilterStylesGlovesBorderColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesBorderColorEnabled != value)
            {
                Settings.Default.LootFilterStylesGlovesBorderColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesGlovesBackgroundColorEnabled
    {
        get => Settings.Default.LootFilterStylesGlovesBackgroundColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesGlovesBackgroundColorEnabled != value)
            {
                Settings.Default.LootFilterStylesGlovesBackgroundColorEnabled = value;
                Save();
            }
        }
    }
}
