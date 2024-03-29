using ChaosRecipeEnhancer.UI.Common;
using ChaosRecipeEnhancer.UI.Models.Constants;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using System.Windows.Input;

namespace ChaosRecipeEnhancer.UI.Windows;

internal class SettingsViewModel : ViewModelBase
{
    private bool _updateAvailable;

    public SettingsViewModel()
    {
        OpenLatestReleasePageCommand = new RelayCommand(OpenLatestReleasePage);
    }

    public string Version => AppInfo.VersionText;
    public ICommand OpenLatestReleasePageCommand { get; }

    public bool UpdateAvailable
    {
        get => _updateAvailable;
        set => SetProperty(ref _updateAvailable, value);
    }

    private void OpenLatestReleasePage()
    {
        var psi = new ProcessStartInfo
        {
            FileName = SiteUrls.CreGithubReleasesUrl,
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}