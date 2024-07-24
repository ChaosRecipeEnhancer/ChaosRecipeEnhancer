using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

public partial class UserSettings : IUserSettings
{
    public int LootFilterStylesBootsTextFontSize
    {
        get => Settings.Default.LootFilterStylesBootsTextFontSize;
        set
        {
            if (Settings.Default.LootFilterStylesBootsTextFontSize != value)
            {
                Settings.Default.LootFilterStylesBootsTextFontSize = value;
                Save();
            }
        }
    }

    public string LootFilterStylesBootsTextColor
    {
        get => Settings.Default.LootFilterStylesBootsTextColor;
        set
        {
            if (Settings.Default.LootFilterStylesBootsTextColor != value)
            {
                Settings.Default.LootFilterStylesBootsTextColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesBootsBorderColor
    {
        get => Settings.Default.LootFilterStylesBootsBorderColor;
        set
        {
            if (Settings.Default.LootFilterStylesBootsBorderColor != value)
            {
                Settings.Default.LootFilterStylesBootsBorderColor = value;
                Save();
            }
        }
    }

    public string LootFilterStylesBootsBackgroundColor
    {
        get => Settings.Default.LootFilterStylesBootsBackgroundColor;
        set
        {
            if (Settings.Default.LootFilterStylesBootsBackgroundColor != value)
            {
                Settings.Default.LootFilterStylesBootsBackgroundColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBootsAlwaysActive
    {
        get => Settings.Default.LootFilterStylesBootsAlwaysActive;
        set
        {
            if (Settings.Default.LootFilterStylesBootsAlwaysActive != value)
            {
                Settings.Default.LootFilterStylesBootsAlwaysActive = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBootsAlwaysDisabled
    {
        get => Settings.Default.LootFilterStylesBootsAlwaysDisabled;
        set
        {
            if (Settings.Default.LootFilterStylesBootsAlwaysDisabled != value)
            {
                Settings.Default.LootFilterStylesBootsAlwaysDisabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBootsMapIconEnabled
    {
        get => Settings.Default.LootFilterStylesBootsMapIconEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBootsMapIconEnabled != value)
            {
                Settings.Default.LootFilterStylesBootsMapIconEnabled = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBootsMapIconColor
    {
        get => Settings.Default.LootFilterStylesBootsMapIconColor;
        set
        {
            if (Settings.Default.LootFilterStylesBootsMapIconColor != value)
            {
                Settings.Default.LootFilterStylesBootsMapIconColor = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBootsMapIconSize
    {
        get => Settings.Default.LootFilterStylesBootsMapIconSize;
        set
        {
            if (Settings.Default.LootFilterStylesBootsMapIconSize != value)
            {
                Settings.Default.LootFilterStylesBootsMapIconSize = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBootsMapIconShape
    {
        get => Settings.Default.LootFilterStylesBootsMapIconShape;
        set
        {
            if (Settings.Default.LootFilterStylesBootsMapIconShape != value)
            {
                Settings.Default.LootFilterStylesBootsMapIconShape = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBootsBeamEnabled
    {
        get => Settings.Default.LootFilterStylesBootsBeamEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBootsBeamEnabled != value)
            {
                Settings.Default.LootFilterStylesBootsBeamEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBootsBeamTemporary
    {
        get => Settings.Default.LootFilterStylesBootsBeamTemporary;
        set
        {
            if (Settings.Default.LootFilterStylesBootsBeamTemporary != value)
            {
                Settings.Default.LootFilterStylesBootsBeamTemporary = value;
                Save();
            }
        }
    }

    public int LootFilterStylesBootsBeamColor
    {
        get => Settings.Default.LootFilterStylesBootsBeamColor;
        set
        {
            if (Settings.Default.LootFilterStylesBootsBeamColor != value)
            {
                Settings.Default.LootFilterStylesBootsBeamColor = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBootsTextColorEnabled
    {
        get => Settings.Default.LootFilterStylesBootsTextColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBootsTextColorEnabled != value)
            {
                Settings.Default.LootFilterStylesBootsTextColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBootsBorderColorEnabled
    {
        get => Settings.Default.LootFilterStylesBootsBorderColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBootsBorderColorEnabled != value)
            {
                Settings.Default.LootFilterStylesBootsBorderColorEnabled = value;
                Save();
            }
        }
    }

    public bool LootFilterStylesBootsBackgroundColorEnabled
    {
        get => Settings.Default.LootFilterStylesBootsBackgroundColorEnabled;
        set
        {
            if (Settings.Default.LootFilterStylesBootsBackgroundColorEnabled != value)
            {
                Settings.Default.LootFilterStylesBootsBackgroundColorEnabled = value;
                Save();
            }
        }
    }
}
