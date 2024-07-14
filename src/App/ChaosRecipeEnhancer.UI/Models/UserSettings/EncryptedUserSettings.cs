using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

public abstract class EncryptedUserSettings
{
    public static string EncryptSetting(string value)
    {
        return string.IsNullOrEmpty(value) ? string.Empty : EncryptionUtilities.EncryptString(value);
    }

    public static string DecryptSetting(string value)
    {
        return string.IsNullOrEmpty(value) ? string.Empty : EncryptionUtilities.DecryptString(value);
    }
}
