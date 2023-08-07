using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.View;

internal sealed class StashTabOverlayViewModel : ViewModelBase
{
    private bool _isEditing;

    public StashTabOverlayViewModel(ISelectedStashTabHandler selectedStashTabHandler)
    {
        SelectedStashTabHandler = selectedStashTabHandler;
    }

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }

    public ISelectedStashTabHandler SelectedStashTabHandler { get; }
}