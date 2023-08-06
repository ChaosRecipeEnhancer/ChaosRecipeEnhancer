using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using ChaosRecipeEnhancer.Common.UI;
using ChaosRecipeEnhancer.UI.WPF.Api.Data;

namespace ChaosRecipeEnhancer.UI.WPF.Model;

internal sealed class StashTab : ViewModelBase
{
	public string TabName
	{
		get;
	}
	public int TabIndex
	{
		get;
	}
	public Uri StashTabUri
	{
		get;
	}

	public StashTab(string name, int index, Uri tabUri)
	{
		TabName = name;
		TabIndex = index;
		StashTabUri = tabUri;
	}

	public void FilterItemsForChaosRecipe(List<Item> itemList)
	{
		// (re)initialize the cell list
		OverlayCellsList.Clear();
		int size = Quad ? 24 : 12;
		for (int i = 0; i < size; i++)
		{
			for (int j = 0; j < size; j++)
			{
				OverlayCellsList.Add(new Cell(j, i));
			}
		}

		ItemsForChaosRecipe.Clear();
		foreach (var item in itemList)
		{
			if ((item.identified && !Properties.Settings.Default.IncludeIdentifiedItemsEnabled) || item.frameType != 2)
			{
				continue;
			}

			item.GetItemClass();
			if (item.ItemType == null)
			{
				continue;
			}

			if (item.ilvl >= 60 && item.ilvl <= 74)
			{
				item.StashTabIndex = TabIndex;
				ItemsForChaosRecipe.Add(item);
			}
		}
	}

	public void DeactivateItemCells()
	{
		foreach (var cell in OverlayCellsList)
		{
			cell.Deactivate();
		}
	}

	public void DeactivateItemCells(Item item)
	{
		var itemCoordinates = new List<List<int>>();

		for (int i = 0; i < item.w; i++)
		{
			for (int j = 0; j < item.h; j++)
			{
				itemCoordinates.Add(new List<int> { item.x + i, item.y + j });
			}
		}

		foreach (var cell in OverlayCellsList)
		{
			foreach (var coordinate in itemCoordinates)
			{
				if (coordinate[0] == cell.XIndex && coordinate[1] == cell.YIndex)
				{
					cell.Deactivate();
				}
			}
		}
	}

	public void ActivateItemCells(Item item)
	{
		var AllCoordinates = new List<List<int>>();

		for (int i = 0; i < item.w; i++)
		{
			for (int j = 0; j < item.h; j++)
			{
				AllCoordinates.Add(new List<int> { item.x + i, item.y + j });
			}
		}

		foreach (var cell in OverlayCellsList)
		{
			foreach (var coordinate in AllCoordinates)
			{
				if (coordinate[0] == cell.XIndex && coordinate[1] == cell.YIndex)
				{
					cell.Activate(ref item);
				}
			}
		}
	}

	// 0: rings
	// 1: amulets
	// 2: belts
	// 3: chests
	// 4: weaponsSmall
	// 5: weaponsBig
	// 6: gloves
	// 7: helmets
	// 8: boots
	public void UpdateAmounts(int[] amounts)
	{
		_ringsAmount = amounts[0];
		_amuletsAmount = amounts[1];
		_beltsAmount = amounts[2];
		_chestsAmount = amounts[3];
		_weaponsSmallAmount = amounts[4];
		_weaponsBigAmount = amounts[5];
		_glovesAmount = amounts[6];
		_helmetsAmount = amounts[7];
		_bootsAmount = amounts[8];
		NeedsItemFetch = false;
		UpdateDisplay();
	}

	public void UpdateDisplay()
	{
		OnPropertyChanged(nameof(RingsAmount));
		OnPropertyChanged(nameof(RingsActive));

		OnPropertyChanged(nameof(AmuletsAmount));
		OnPropertyChanged(nameof(AmuletsActive));

		OnPropertyChanged(nameof(BeltsAmount));
		OnPropertyChanged(nameof(BeltsActive));

		OnPropertyChanged(nameof(ChestsAmount));
		OnPropertyChanged(nameof(ChestsActive));

		OnPropertyChanged(nameof(WeaponsAmount));
		OnPropertyChanged(nameof(WeaponsActive));

		OnPropertyChanged(nameof(GlovesAmount));
		OnPropertyChanged(nameof(GlovesActive));

		OnPropertyChanged(nameof(HelmetsAmount));
		OnPropertyChanged(nameof(HelmetsActive));

		OnPropertyChanged(nameof(BootsAmount));
		OnPropertyChanged(nameof(BootsActive));
	}

	public List<Item> ItemsForChaosRecipe { get; } = new List<Item>();

	public ObservableCollection<Cell> OverlayCellsList { get; } = new();

	private bool _quad;
	public bool Quad
	{
		get => _quad;
		set => SetProperty(ref _quad, value);
	}

	private bool _needsItemFetch = true;
	public bool NeedsItemFetch
	{
		get => _needsItemFetch;
		set => SetProperty(ref _needsItemFetch, value);
	}

	private int _fullSets;
	public int FullSets
	{
		get => _fullSets;
		set => SetProperty(ref _fullSets, value);
	}

	private bool ShowAmountNeeded => Properties.Settings.Default.SetTrackerOverlayItemCounterDisplayMode == 2;

	private int _ringsAmount;
	public int RingsAmount => ShowAmountNeeded ? Math.Max((Properties.Settings.Default.FullSetThreshold * 2) - _ringsAmount, 0) : _ringsAmount;
	public bool RingsActive => NeedsItemFetch || (Properties.Settings.Default.FullSetThreshold * 2) - _ringsAmount > 0;

	private int _amuletsAmount;
	public int AmuletsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _amuletsAmount, 0) : _amuletsAmount;
	public bool AmuletsActive => NeedsItemFetch || Properties.Settings.Default.FullSetThreshold - _amuletsAmount > 0;

	private int _beltsAmount;
	public int BeltsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _beltsAmount, 0) : _beltsAmount;
	public bool BeltsActive => NeedsItemFetch || Properties.Settings.Default.FullSetThreshold - _beltsAmount > 0;

	private int _chestsAmount;
	public int ChestsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _chestsAmount, 0) : _chestsAmount;
	public bool ChestsActive => NeedsItemFetch || Properties.Settings.Default.FullSetThreshold - _chestsAmount > 0;

	private int _weaponsSmallAmount;
	private int _weaponsBigAmount;
	public int WeaponsAmount => ShowAmountNeeded ? Math.Max((Properties.Settings.Default.FullSetThreshold * 2) - (_weaponsSmallAmount + (_weaponsBigAmount * 2)), 0) : _weaponsSmallAmount + (_weaponsBigAmount * 2);
	public bool WeaponsActive => NeedsItemFetch || (Properties.Settings.Default.FullSetThreshold * 2) - (_weaponsSmallAmount + (_weaponsBigAmount * 2)) > 0;

	private int _glovesAmount;
	public int GlovesAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _glovesAmount, 0) : _glovesAmount;
	public bool GlovesActive => NeedsItemFetch || Properties.Settings.Default.FullSetThreshold - _glovesAmount > 0;

	private int _helmetsAmount;
	public int HelmetsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _helmetsAmount, 0) : _helmetsAmount;
	public bool HelmetsActive => NeedsItemFetch || Properties.Settings.Default.FullSetThreshold - _helmetsAmount > 0;

	private int _bootsAmount;
	public int BootsAmount => ShowAmountNeeded ? Math.Max(Properties.Settings.Default.FullSetThreshold - _bootsAmount, 0) : _bootsAmount;
	public bool BootsActive => NeedsItemFetch || Properties.Settings.Default.FullSetThreshold - _bootsAmount > 0;
}
