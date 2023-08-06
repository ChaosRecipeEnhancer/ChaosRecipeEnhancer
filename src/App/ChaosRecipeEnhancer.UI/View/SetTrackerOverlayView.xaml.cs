using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ChaosRecipeEnhancer.UI.Api;
using ChaosRecipeEnhancer.UI.Model;
using ChaosRecipeEnhancer.UI.UserControls.SetTrackerOverlayDisplays;
using ChaosRecipeEnhancer.UI.Utilities;
using ChaosRecipeEnhancer.UI.ViewModel;

namespace ChaosRecipeEnhancer.UI.View;

/// <summary>
///     Interaction logic for SetTrackerOverlayView.xaml
/// </summary>
internal partial class SetTrackerOverlayView
{
	private const string _setsFullText = "Sets full!";
	private const int fetchCooldown = 30;

	public bool IsOpen
	{
		get; private set;
	}

	private readonly StashTabOverlayView _stashTabOverlay;
	private readonly ItemSetManager _itemSetManager;
	private readonly StashTabGetter _stashTabGetter;

	private readonly SetTrackerOverlayViewModel _model;

	public SetTrackerOverlayView(ItemSetManager itemSetManager, StashTabGetter stashTabGetter)
	{
		_itemSetManager = itemSetManager;
		_stashTabGetter = stashTabGetter;
		DataContext = _model = new SetTrackerOverlayViewModel(_itemSetManager);
		_stashTabOverlay = new StashTabOverlayView(_itemSetManager);

		InitializeComponent();

		UpdateOverlayType();
		Properties.Settings.Default.PropertyChanged += OnSettingsChanged;
	}

	private void OnLoaded(object sender, RoutedEventArgs e) => Win32.MakeToolWindow(this);

	private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
	{
		if (e.PropertyName == nameof(Properties.Settings.SetTrackerOverlayDisplayMode))
		{
			UpdateOverlayType();
		}
		else if (e.PropertyName == nameof(Properties.Settings.FullSetThreshold) || e.PropertyName == nameof(Properties.Settings.SelectedStashTabName))
		{
			CheckForFullSets();
		}
	}

	private void UpdateOverlayType()
	{
		if (Properties.Settings.Default.SetTrackerOverlayDisplayMode == 0)
		{
			MainOverlayContentControl.Content = new StandardDisplay(this);
		}
		else if (Properties.Settings.Default.SetTrackerOverlayDisplayMode == 1)
		{
			MainOverlayContentControl.Content = new MinifiedDisplay(this);
		}
		// TODO: Add more overlay types!
	}

	public void RunFetching()
	{
		if (!IsOpen)
		{
			return;
		}

		if (_itemSetManager.SelectedStashTab is null)
		{
			_ = MessageBox.Show("Missing Settings!" + Environment.NewLine + "Please select a Stash Tab.");
			return;
		}

		FetchDataAsync(); // Fire and forget async
	}

	private async void FetchDataAsync()
	{
		_model.WarningMessage = string.Empty;

		_model.ShowProgress = true;
		_model.FetchButtonEnabled = false;

		if (await _stashTabGetter.GetItemsAsync(_itemSetManager.SelectedStashTab))
		{
			await Task.Run(_itemSetManager.UpdateData);
			_model.ShowProgress = false;
			await Task.Delay(fetchCooldown * 1000);
		}
		else if (RateLimit.RateLimitExceeded)
		{
			_model.WarningMessage = "Rate Limit Exceeded! Waiting...";
			await Task.Delay(RateLimit.GetSecondsToWait() * 1000);
			RateLimit.RequestCounter = 0;
			RateLimit.RateLimitExceeded = false;
		}
		else if (RateLimit.BanTime > 0)
		{
			_model.WarningMessage = "Temporary Ban! Waiting...";
			await Task.Delay(RateLimit.BanTime * 1000);
			RateLimit.BanTime = 0;
		}

		_model.ShowProgress = false;
		_model.FetchButtonEnabled = true;

		CheckForFullSets();
	}

	private void CheckForFullSets()
	{
		if (_itemSetManager.SelectedStashTab?.NeedsItemFetch != false)
		{
			_model.WarningMessage = string.Empty;
		}
		else if (!_itemSetManager.SelectedStashTab.NeedsItemFetch && _itemSetManager.SelectedStashTab.FullSets >= Properties.Settings.Default.FullSetThreshold)
		{
			_model.WarningMessage = _setsFullText;
		}
		else if (_model.WarningMessage == _setsFullText)
		{
			_model.WarningMessage = string.Empty;
		}
	}

	private void Window_MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Left && !Properties.Settings.Default.SetTrackerOverlayOverlayLockPositionEnabled && Mouse.LeftButton == MouseButtonState.Pressed)
		{
			DragMove();
		}
	}

	public new virtual void Hide()
	{
		IsOpen = false;
		_stashTabOverlay.Hide();
		base.Hide();
	}

	public new virtual void Show()
	{
		IsOpen = true;
		base.Show();
	}

	public void RunStashTabOverlay()
	{
		if (_stashTabOverlay.IsOpen)
		{
			_stashTabOverlay.Hide();
		}
		else
		{
			if (_itemSetManager.SelectedStashTab.NeedsItemFetch)
			{
				RunFetching();
			}

			_stashTabOverlay.Show();
		}
	}
}