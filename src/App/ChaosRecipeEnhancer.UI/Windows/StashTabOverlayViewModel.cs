using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Windows;

internal sealed class StashTabOverlayViewModel : ViewModelBase
{
    private bool _isEditing;

    public bool IsEditing
    {
        get => _isEditing;
        set => SetProperty(ref _isEditing, value);
    }
}