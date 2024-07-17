using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Config;
using ChaosRecipeEnhancer.UI.Models.UserSettings;
using ChaosRecipeEnhancer.UI.UserControls;
using ChaosRecipeEnhancer.UI.Utilities;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.Windows;

public class SettingsViewModel : CreViewModelBase
{
    #region Fields

    private readonly IUserSettings _userSettings;
    private ICommand _openLatestReleasePageCommand;
    private bool _updateAvailable;

    #endregion

    #region Constructors

    public SettingsViewModel(IUserSettings userSettings)
    {
        _userSettings = userSettings;
    }

    #endregion

    #region Properties

    public static string Version => CreAppConfig.VersionText;
    public ICommand OpenLatestReleasePageCommand => _openLatestReleasePageCommand ??= new RelayCommand(OpenLatestReleasePage);

    public bool UpdateAvailable
    {
        get => _updateAvailable;
        set => SetProperty(ref _updateAvailable, value);
    }

    public bool CloseToTrayEnabled
    {
        get => _userSettings.CloseToTrayEnabled;
    }

    #endregion

    #region Methods

    private void OpenLatestReleasePage()
    {
        UrlUtilities.OpenUrl(SiteUrls.CreGithubReleasesUrl);
    }

    #endregion
}