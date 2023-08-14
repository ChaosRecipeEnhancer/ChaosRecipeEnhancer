using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

namespace ChaosRecipeEnhancer.UI.Windows;

internal sealed class SetTrackerOverlayViewModel : ViewModelBase
{
    private bool _fetchButtonEnabled = true;
    private bool _showProgress;
    private string _warningMessage;

    public bool ShowProgress
    {
        get => _showProgress;
        set => SetProperty(ref _showProgress, value);
    }

    public bool FetchButtonEnabled
    {
        get => _fetchButtonEnabled;
        set => SetProperty(ref _fetchButtonEnabled, value);
    }

    public string WarningMessage
    {
        get => _warningMessage;
        set => SetProperty(ref _warningMessage, value);
    }
}