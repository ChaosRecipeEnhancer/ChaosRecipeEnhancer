using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Sound Settings.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public bool SoundEnabled
    {
        get => Settings.Default.SoundEnabled;
        set
        {
            if (Settings.Default.SoundEnabled != value)
            {
                Settings.Default.SoundEnabled = value;
                Save();
            }
        }
    }

    public double SoundLevel
    {
        get => Settings.Default.SoundLevel;
        set
        {
            if (Settings.Default.SoundLevel != value)
            {
                Settings.Default.SoundLevel = value;
                Save();
            }
        }
    }
}
