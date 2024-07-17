using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Chaos Recipe Enhancer Auto-Fetch Settings.
/// </summary>
public partial class UserSettings : IUserSettings
{
    public bool AutoFetchOnRezoneEnabled
    {
        get => Settings.Default.AutoFetchOnRezoneEnabled;
        set
        {
            if (Settings.Default.AutoFetchOnRezoneEnabled != value)
            {
                Settings.Default.AutoFetchOnRezoneEnabled = value;
                Save();
            }
        }
    }

    public int PathOfExileClientLogLocationMode
    {
        get => Settings.Default.PathOfExileClientLogLocationMode;
        set
        {
            if (Settings.Default.PathOfExileClientLogLocationMode != value)
            {
                Settings.Default.PathOfExileClientLogLocationMode = value;
                Save();
            }
        }
    }

    public string PathOfExileClientLogLocation
    {
        get => Settings.Default.PathOfExileClientLogLocation;
        set
        {
            if (Settings.Default.PathOfExileClientLogLocation != value)
            {
                Settings.Default.PathOfExileClientLogLocation = value;
                Save();
            }
        }
    }
}
