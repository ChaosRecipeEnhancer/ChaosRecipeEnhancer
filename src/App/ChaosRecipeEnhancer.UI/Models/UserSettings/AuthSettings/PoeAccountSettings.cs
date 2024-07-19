using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using System;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Path of Exile Account.
/// </summary>
public partial class UserSettings : EncryptedUserSettings, IUserSettings
{
    public string PathOfExileAccountName
    {
        get => Settings.Default.PathOfExileAccountName;
        set
        {
            if (Settings.Default.PathOfExileAccountName != value)
            {
                Settings.Default.PathOfExileAccountName = value;
                Save();
            }
        }
    }

    // Encrypted Data
    public string PathOfExileApiAuthToken
    {
        get => DecryptSetting(Settings.Default.PathOfExileApiAuthToken);
        set
        {
            string encryptedValue = EncryptSetting(value);
            if (Settings.Default.PathOfExileApiAuthToken != encryptedValue)
            {
                Settings.Default.PathOfExileApiAuthToken = encryptedValue;
                Save();
            }
        }
    }

    public DateTime PathOfExileApiAuthTokenExpiration
    {
        get => Settings.Default.PathOfExileApiAuthTokenExpiration;
        set
        {
            if (Settings.Default.PathOfExileApiAuthTokenExpiration != value)
            {
                Settings.Default.PathOfExileApiAuthTokenExpiration = value;
                Save();
            }
        }
    }

    public ConnectionStatusTypes PoEAccountConnectionStatus
    {
        get => (ConnectionStatusTypes)Settings.Default.PoEAccountConnectionStatus;
        set
        {
            if (Settings.Default.PoEAccountConnectionStatus != (int)value)
            {
                Settings.Default.PoEAccountConnectionStatus = (int)value;
                Save();
            }
        }
    }

    public string LegacyAuthAccountName
    {
        get => Settings.Default.LegacyAuthAccountName;
        set
        {
            if (Settings.Default.LegacyAuthAccountName != value)
            {
                Settings.Default.LegacyAuthAccountName = value;
                Save();
            }
        }
    }

    // Encrypted Data
    public string LegacyAuthSessionId
    {
        get => DecryptSetting(Settings.Default.LegacyAuthSessionId);
        set
        {
            string encryptedValue = EncryptSetting(value);
            if (Settings.Default.LegacyAuthSessionId != encryptedValue)
            {
                Settings.Default.LegacyAuthSessionId = encryptedValue;
                Save();
            }
        }
    }
}
