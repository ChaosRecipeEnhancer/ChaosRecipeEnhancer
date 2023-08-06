using ChaosRecipeEnhancer.Common.UI;
using ChaosRecipeEnhancer.UI.Model;

namespace ChaosRecipeEnhancer.UI.ViewModel;
internal class SetTrackerOverlayViewModel : ViewModelBase
{
	public SetTrackerOverlayViewModel(ISelectedStashTabHandler selectedStashTabHandler) => SelectedStashTabHandler = selectedStashTabHandler;

	public Properties.Settings Settings => Properties.Settings.Default;

	public ISelectedStashTabHandler SelectedStashTabHandler
	{
		get;
	}

	private bool _showProgress;
	public bool ShowProgress
	{
		get => _showProgress;
		set => SetProperty(ref _showProgress, value);
	}

	private bool _fetchButtonEnabled = true;
	public bool FetchButtonEnabled
	{
		get => _fetchButtonEnabled;
		set => SetProperty(ref _fetchButtonEnabled, value);
	}

	private string _warningMessage;
	public string WarningMessage
	{
		get => _warningMessage;
		set => SetProperty(ref _warningMessage, value);
	}
}
