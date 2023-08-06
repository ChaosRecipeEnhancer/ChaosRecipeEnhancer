using ChaosRecipeEnhancer.Common.UI;
using ChaosRecipeEnhancer.UI.WPF.Model;

namespace ChaosRecipeEnhancer.UI.WPF.ViewModel;
internal class StashTabOverlayViewModel : ViewModelBase
{
	public StashTabOverlayViewModel(ISelectedStashTabHandler selectedStashTabHandler) => SelectedStashTabHandler = selectedStashTabHandler;

	private bool _isEditing;
	public bool IsEditing
	{
		get => _isEditing;
		set => SetProperty(ref _isEditing, value);
	}

	public ISelectedStashTabHandler SelectedStashTabHandler
	{
		get;
	}
}
