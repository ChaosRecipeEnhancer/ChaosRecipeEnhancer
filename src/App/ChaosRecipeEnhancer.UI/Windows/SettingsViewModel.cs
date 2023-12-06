using System.Diagnostics;
using System.Windows.Input;
using ChaosRecipeEnhancer.UI.Constants;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;
using CommunityToolkit.Mvvm.Input;

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
            FileName = AppInfo.GithubReleasesUrl,
            UseShellExecute = true
        };
        Process.Start(psi);
    }
}