using ChaosRecipeEnhancer.Common.UI;
using ChaosRecipeEnhancer.UI.WPF.Api.Data;

namespace ChaosRecipeEnhancer.UI.WPF.Model;

internal sealed class Cell : ViewModelBase
{
	public int XIndex
	{
		get;
	}
	public int YIndex
	{
		get;
	}
	public Item Item
	{
		get; private set;
	}

	private bool _active;
	public bool Active
	{
		get => _active;
		private set => SetProperty(ref _active, value);
	}

	public Cell(int x, int y)
	{
		XIndex = x;
		YIndex = y;
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
