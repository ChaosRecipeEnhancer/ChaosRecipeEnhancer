using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Configuration;

public static class UpgradeConfiguration
{
    public static void UpgradeSettings()
    {
        if (Settings.Default.UpgradeSettingsAfterUpdate)
        {
            Settings.Default.Upgrade();
            Settings.Default.UpgradeSettingsAfterUpdate = false;
            Settings.Default.Save();
        }
    }
}
