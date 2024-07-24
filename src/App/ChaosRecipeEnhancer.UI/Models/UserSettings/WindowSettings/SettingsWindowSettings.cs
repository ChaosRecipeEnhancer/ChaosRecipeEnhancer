using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Chaos Recipe Enhancer Settings Window Settings.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public int SettingsWindowNavIndex
    {
        get => Settings.Default.SettingsWindowNavIndex;
        set
        {
            if (Settings.Default.SettingsWindowNavIndex != value)
            {
                Settings.Default.SettingsWindowNavIndex = value;
                Save();
            }
        }
    }

    public bool CloseToTrayEnabled
    {
        get => Settings.Default.CloseToTrayEnabled;
        set
        {
            if (Settings.Default.CloseToTrayEnabled != value)
            {
                Settings.Default.CloseToTrayEnabled = value;
                Save();
            }
        }
    }

    public Language Language
    {
        get => (Language)Settings.Default.Language;
        set
        {
            if (Settings.Default.Language != (int)value)
            {
                Settings.Default.Language = (int)value;
                Save();
            }
        }
    }
}
