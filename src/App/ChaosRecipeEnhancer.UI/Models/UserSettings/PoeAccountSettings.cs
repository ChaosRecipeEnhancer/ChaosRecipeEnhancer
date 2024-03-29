using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using System;

namespace ChaosRecipeEnhancer.UI.Models.UserSettings;

/// <summary>
/// User Settings related to the Path of Exile Account.
/// </summary>
public partial class UserSettings : IUserSettings
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

    public string PathOfExileApiAuthToken
    {
        get => Settings.Default.PathOfExileApiAuthToken;
        set
        {
            if (Settings.Default.PathOfExileApiAuthToken != value)
            {
                Settings.Default.PathOfExileApiAuthToken = value;
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
}
