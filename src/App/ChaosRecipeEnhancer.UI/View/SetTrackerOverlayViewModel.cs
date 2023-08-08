using ChaosRecipeEnhancer.UI.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.View;

internal sealed class SetTrackerOverlayViewModel : ViewModelBase
{
    private bool _fetchButtonEnabled = true;

    private bool _showProgress;

    private string _warningMessage;

    public SetTrackerOverlayViewModel(ISelectedStashTabHandler selectedStashTabHandler)
    {
        SelectedStashTabHandler = selectedStashTabHandler;
    }

    public Settings Settings => Settings.Default;

    public ISelectedStashTabHandler SelectedStashTabHandler { get; }

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