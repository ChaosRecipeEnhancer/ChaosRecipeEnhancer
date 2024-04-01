using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Models.Config;
using ChaosRecipeEnhancer.UI.UserControls;
using ChaosRecipeEnhancer.UI.Utilities;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.Windows;

internal class SettingsViewModel : ViewModelBase
{
    private bool _updateAvailable;

    public SettingsViewModel()
    {
        OpenLatestReleasePageCommand = new RelayCommand(OpenLatestReleasePage);
    }

    public static string Version => CreAppConfig.VersionText;
    public ICommand OpenLatestReleasePageCommand { get; }

    public bool UpdateAvailable
    {
        get => _updateAvailable;
        set => SetProperty(ref _updateAvailable, value);
    }

    private void OpenLatestReleasePage()
    {
        UrlUtilities.OpenUrl(SiteUrls.CreGithubReleasesUrl);
    }
}