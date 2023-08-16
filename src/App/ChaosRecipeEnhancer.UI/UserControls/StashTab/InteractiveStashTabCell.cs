using ChaosRecipeEnhancer.UI.Models;
using ChaosRecipeEnhancer.UI.Utilities.ZemotoCommon;

namespace ChaosRecipeEnhancer.UI.UserControls.StashTab;

public sealed class InteractiveStashTabCell : ViewModelBase
{
    private bool _active;
    private string _buttonText;

    public EnhancedItem ItemModel { get; set; }

    public int XIndex { get; init; }
    public int YIndex { get; init; }

    public bool Active
    {
        get => _active;
        set => SetProperty(ref _active, value);
    }

    public string ButtonText
    {
        get => _buttonText;
        set => SetProperty(ref _buttonText, value);
    }
}