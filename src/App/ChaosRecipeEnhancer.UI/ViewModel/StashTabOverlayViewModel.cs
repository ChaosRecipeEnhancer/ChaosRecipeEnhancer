using ChaosRecipeEnhancer.Common.UI;
using ChaosRecipeEnhancer.UI.Model;

namespace ChaosRecipeEnhancer.UI.ViewModel;
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
