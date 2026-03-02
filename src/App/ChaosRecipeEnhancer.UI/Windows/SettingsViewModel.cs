using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Config;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.Services;
using ChaosRecipeEnhancer.UI.UserControls;
using ChaosRecipeEnhancer.UI.Utilities;
using CommunityToolkit.Mvvm.Input;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.Windows;

public class SettingsViewModel : CreViewModelBase
{
    #region Fields

    private readonly IUserSettings _userSettings;
    private readonly IUpdateManagerService _updateManagerService;

    private ICommand _openLatestReleasePageCommand;
    private ICommand _downloadAndApplyUpdateCommand;

    private bool _updateAvailable;
    private bool _isDownloadingUpdate;
    private bool _isUpdateReadyToInstall;
    private string _updateStatusText;

    #endregion

    #region Constructors

    public SettingsViewModel(IUserSettings userSettings, IUpdateManagerService updateManagerService)
    {
        _userSettings = userSettings;
        _updateManagerService = updateManagerService;
    }

    #endregion

    #region Properties

    public static string Version => CreAppConfig.VersionText;

    public ICommand OpenLatestReleasePageCommand => _openLatestReleasePageCommand ??= new RelayCommand(OpenLatestReleasePage);

    public ICommand DownloadAndApplyUpdateCommand => _downloadAndApplyUpdateCommand ??= new AsyncRelayCommand(DownloadAndApplyUpdateAsync);

    public bool UpdateAvailable
    {
        get => _updateAvailable;
        set => SetProperty(ref _updateAvailable, value);
    }

    public bool IsDownloadingUpdate
    {
        get => _isDownloadingUpdate;
        set => SetProperty(ref _isDownloadingUpdate, value);
    }

    public bool IsUpdateReadyToInstall
    {
        get => _isUpdateReadyToInstall;
        set => SetProperty(ref _isUpdateReadyToInstall, value);
    }

    public string UpdateStatusText
    {
        get => _updateStatusText;
        set => SetProperty(ref _updateStatusText, value);
    }

    public bool CloseToTrayEnabled
    {
        get => _userSettings.CloseToTrayEnabled;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Checks for updates on startup. If an update is available, sets the UI state accordingly.
    /// </summary>
    public async Task CheckForUpdateAsync()
    {
        try
        {
            var hasUpdate = await _updateManagerService.CheckForUpdateAsync();
            UpdateAvailable = hasUpdate;

            if (hasUpdate)
            {
                UpdateStatusText = $"v{_updateManagerService.PendingVersion} available — click to update";
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "SettingsViewModel - Error during update check");
        }
    }

    /// <summary>
    /// Downloads the pending update, then applies it and restarts the app.
    /// </summary>
    private async Task DownloadAndApplyUpdateAsync()
    {
        if (!_updateManagerService.IsUpdateAvailable) return;

        try
        {
            IsDownloadingUpdate = true;
            UpdateStatusText = "Downloading update...";

            await _updateManagerService.DownloadUpdateAsync();

            if (_updateManagerService.IsUpdateDownloaded)
            {
                UpdateStatusText = "Restarting...";
                IsUpdateReadyToInstall = true;
                _updateManagerService.ApplyUpdateAndRestart();
            }
            else
            {
                UpdateStatusText = "Download failed. Click to try again.";
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "SettingsViewModel - Error downloading/applying update");
            UpdateStatusText = "Update failed. Click to try again.";
        }
        finally
        {
            IsDownloadingUpdate = false;
        }
    }

    private void OpenLatestReleasePage()
    {
        UrlUtilities.OpenUrl(SiteUrls.CreGithubReleasesUrl);
    }

    #endregion
}
