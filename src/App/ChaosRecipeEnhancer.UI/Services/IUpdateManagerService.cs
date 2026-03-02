using System.Threading.Tasks;
using Velopack;

namespace ChaosRecipeEnhancer.UI.Services;

/// <summary>
/// Service for checking, downloading, and applying application updates via Velopack.
/// </summary>
public interface IUpdateManagerService
{
    /// <summary>
    /// Whether an update has been detected and is available.
    /// </summary>
    bool IsUpdateAvailable { get; }

    /// <summary>
    /// Whether the update has been downloaded and is ready to install.
    /// </summary>
    bool IsUpdateDownloaded { get; }

    /// <summary>
    /// The version string of the pending update, if any.
    /// </summary>
    string PendingVersion { get; }

    /// <summary>
    /// Checks GitHub releases for a newer version. Returns true if an update is available.
    /// </summary>
    Task<bool> CheckForUpdateAsync();

    /// <summary>
    /// Downloads the available update. Should only be called after <see cref="CheckForUpdateAsync"/> returns true.
    /// </summary>
    Task DownloadUpdateAsync();

    /// <summary>
    /// Applies the downloaded update and restarts the application.
    /// </summary>
    void ApplyUpdateAndRestart();
}
