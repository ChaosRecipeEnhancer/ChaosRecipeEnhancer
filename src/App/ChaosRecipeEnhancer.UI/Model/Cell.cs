using ChaosRecipeEnhancer.UI.Api.Data;
using ChaosRecipeEnhancer.UI.Utilities;

namespace ChaosRecipeEnhancer.UI.Model;

public sealed class Cell : ViewModelBase
{
    private bool _active;

    public Cell(int x, int y)
    {
        XIndex = x;
        YIndex = y;
    }

    public int XIndex { get; }

    public int YIndex { get; }

    public Item Item { get; private set; }

    public bool Active
    {
        get => _active;
        private set => SetProperty(ref _active, value);
    }

    public void Activate(ref Item item)
    {
        Active = true;
        Item = item;
    }

    public void Deactivate()
    {
        Active = false;
        Item = null;
    }
}