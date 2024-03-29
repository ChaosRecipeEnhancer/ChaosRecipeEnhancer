using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

public partial class UserSettings : IUserSettings
{
    public bool UpgradeSettingsAfterUpdate
    {
        get => Settings.Default.UpgradeSettingsAfterUpdate;
        set
        {
            if (Settings.Default.UpgradeSettingsAfterUpdate != value)
            {
                Settings.Default.UpgradeSettingsAfterUpdate = value;
                Save();
            }
        }
    }

    public bool DebugMode
    {
        get => Settings.Default.DebugMode;
        set
        {
            if (Settings.Default.DebugMode != value)
            {
                Settings.Default.DebugMode = value;
                Save();
            }
        }
    }

    public void Reset()
    {
        Settings.Default.Reset();
        Settings.Default.Save();
    }

    private static void Save() => Settings.Default.Save();
}
