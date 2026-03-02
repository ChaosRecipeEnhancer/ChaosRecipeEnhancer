using ChaosRecipeEnhancer.UI.Models.Config;
using Serilog;
using System;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace ChaosRecipeEnhancer.UI.Services;

/// <summary>
/// Velopack-based implementation of <see cref="IUpdateManagerService"/>.
/// Checks for updates from the ChaosRecipeEnhancer GitHub releases.
/// </summary>
public class UpdateManagerService : IUpdateManagerService
{
    private readonly UpdateManager _updateManager;
    private UpdateInfo _pendingUpdate;

    public UpdateManagerService()
    {
        var repoUrl = $"https://github.com/{CreAppConfig.GitHubOrgName}/{CreAppConfig.GitHubRepoName}";
        _updateManager = new UpdateManager(new GithubSource(repoUrl, null, false));
    }

    public bool IsUpdateAvailable => _pendingUpdate != null;

    public bool IsUpdateDownloaded { get; private set; }

    public string PendingVersion => _pendingUpdate?.TargetFullRelease?.Version?.ToString();

    public async Task<bool> CheckForUpdateAsync()
    {
        if (!_updateManager.IsInstalled)
        {
            Log.Information("UpdateManagerService - App is not installed via Velopack (dev mode), skipping update check");
            return false;
        }

        try
        {
            _pendingUpdate = await _updateManager.CheckForUpdatesAsync();

            if (_pendingUpdate != null)
            {
                Log.Information("UpdateManagerService - New version {Version} available", PendingVersion);
            }
            else
            {
                Log.Information("UpdateManagerService - Application is up to date");
            }

            return IsUpdateAvailable;
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "UpdateManagerService - Error checking for updates");
            _pendingUpdate = null;
            return false;
        }
    }

    public async Task DownloadUpdateAsync()
    {
        if (_pendingUpdate == null)
        {
            Log.Warning("UpdateManagerService - DownloadUpdateAsync called with no pending update");
            return;
        }

        try
        {
            Log.Information("UpdateManagerService - Downloading update {Version}...", PendingVersion);
            await _updateManager.DownloadUpdatesAsync(_pendingUpdate);
            IsUpdateDownloaded = true;
            Log.Information("UpdateManagerService - Update {Version} downloaded successfully", PendingVersion);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "UpdateManagerService - Error downloading update");
            IsUpdateDownloaded = false;
        }
    }

    public void ApplyUpdateAndRestart()
    {
        if (!IsUpdateDownloaded || _pendingUpdate == null)
        {
            Log.Warning("UpdateManagerService - ApplyUpdateAndRestart called but no update is downloaded");
            return;
        }

        Log.Information("UpdateManagerService - Applying update {Version} and restarting...", PendingVersion);
        _updateManager.ApplyUpdatesAndRestart(_pendingUpdate);
    }
}
