using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.DataFetching;
using ChaosRecipeEnhancer.UI.WPF.BusinessLogic.Hotkeys;
using ChaosRecipeEnhancer.UI.WPF.Extensions.Native;
using ChaosRecipeEnhancer.UI.WPF.Properties;
using ChaosRecipeEnhancer.UI.WPF.DynamicControls.StashTabs;
using ChaosRecipeEnhancer.UI.WPF.UserControls.SetTrackerOverlayDisplays;
using Serilog;
using ChaosRecipeEnhancer.UI.WPF.Api;
using ChaosRecipeEnhancer.UI.WPF.ViewModel;
using ChaosRecipeEnhancer.UI.WPF.Model;

namespace ChaosRecipeEnhancer.UI.WPF.View;

/// <summary>
///     Interaction logic for SettingsWindow.xaml
/// </summary>
public partial class SettingsView
{
	#region Fields

	private readonly SettingsViewModel _model;
	private readonly SetTrackerOverlayView _setTrackerOverlay;
	private readonly LeagueGetter _leagueGetter = new();
	private readonly StashTabGetter _stashTabGetter = new();
	private readonly System.Windows.Forms.NotifyIcon _trayIcon = new();

	private bool _closingFromTrayIcon;

	// This version # should match up with the format for Assembly version # (3 dots, 4 digits), or else you'll get spammed for AutoUpdates
	private const bool IsPreviewVersion = false;
	private const int PreviewPatchNumber = 0;
	private const string AppVersion = "1.7.2.0";

	#endregion

	#region Constructors

	public SettingsView()
	{
		var itemSetManager = new ItemSetManager();
		_setTrackerOverlay = new SetTrackerOverlayView(itemSetManager, _stashTabGetter);
		DataContext = _model = new SettingsViewModel(itemSetManager);

		InitializeComponent();
		InitializeTray();
		LoadLeagueList();
	}

	#endregion

	#region Properties

	public static bool SettingsComplete
	{
		get; set;
	}

	public static string AppVersionText
	{
		get; set;
	} = "v." + AppVersion +
														(IsPreviewVersion
															? $" (Preview {PreviewPatchNumber})"
															: String.Empty);

	#endregion

	#region Event Handlers

	// Minimize to system tray when application is closed.
	protected override void OnClosing(CancelEventArgs e)
	{
		// if hideOnClose
		// setting cancel to true will cancel the close request
		// so the application is not closed
		if (Settings.Default.CloseToTrayEnabled && !_trayClose)
		{
			e.Cancel = true;

			Hide();

			base.OnClosing(e);
		}

		if (Settings.Default.CloseToTrayEnabled && !_trayClose) return;

		_notifyIcon.Visible = false;

		NativeMouseExtensions.Stop();
		HotkeysManager.ShutdownSystemHook();
		Settings.Default.Save();

		if (LogWatcher.WorkerThread != null && LogWatcher.WorkerThread.IsAlive)
			LogWatcher.StopWatchingLogFile();
	}

	private static void CheckForClick(object sender, EventArgs e)
	{
		var releasesUrl = "https://github.com/ChaosRecipeEnhancer/EnhancePoEApp/releases";

		try
		{
			Process.Start(releasesUrl);
		}
		catch
		{
			// legacytodo because of this: https://github.com/dotnet/corefx/issues/10361
			releasesUrl = releasesUrl.Replace("&", "^&");
			Process.Start(new ProcessStartInfo(releasesUrl) { UseShellExecute = true });
		}
	}

	// Close the form, which closes the application.
	private void ExitMenuItemClick(object sender, EventArgs e)
	{
		_trayClose = true;
		Close();
	}

	private void RunButton_Click(object sender, RoutedEventArgs e)
	{
		RunOverlay();
	}

	private void ColorBootsPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
	{
		Settings.Default.LootFilterBootsColor = ColorBootsPicker.SelectedColor.ToString();
	}

	private void ColorGlovesPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
	{
		Settings.Default.LootFilterGlovesColor = ColorGlovesPicker.SelectedColor.ToString();
	}

	private void ColorHelmetPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
	{
		Settings.Default.LootFilterHelmetColor = ColorHelmetPicker.SelectedColor.ToString();
	}

	private void ColorChestPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
	{
		Settings.Default.LootFilterBodyArmourColor = ColorChestPicker.SelectedColor.ToString();
	}

	private void ColorWeaponsPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
	{
		Settings.Default.LootFilterWeaponColor = ColorWeaponsPicker.SelectedColor.ToString();
	}

	private void ColorStashPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
	{
		Settings.Default.StashTabOverlayHighlightColor = ColorStashPicker.SelectedColor.ToString();
	}

	private void ColorRingPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
	{
		Settings.Default.LootFilterRingColor = ColorRingPicker.SelectedColor.ToString();
	}

	private void ColorAmuletPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
	{
		Settings.Default.LootFilterAmuletColor = ColorAmuletPicker.SelectedColor.ToString();
	}

	private void ColorBeltPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
	{
		Settings.Default.LootFilterBeltColor = ColorBeltPicker.SelectedColor.ToString();
	}

	private void Window_MouseDown(object sender, MouseButtonEventArgs e)
	{
		MainGrid.Focus();
	}


	private void ColorStashBackgroundPicker_SelectedColorChanged(object sender,
		RoutedPropertyChangedEventArgs<Color?> e)
	{
		Settings.Default.StashTabOverlayBackgroundColor = ColorStashBackgroundPicker.SelectedColor.ToString();
	}

	private void CustomHotkeyToggle_Click(object sender, RoutedEventArgs e)
	{
		var isWindowOpen = false;
		foreach (Window w in System.Windows.Application.Current.Windows)
			if (w is HotkeyView)
				isWindowOpen = true;

		if (isWindowOpen) return;
		var hotkeyDialog = new HotkeyView(this, "toggle");
		hotkeyDialog.Show();
	}

	private void RefreshHotkey_Click(object sender, RoutedEventArgs e)
	{
		var isWindowOpen = false;
		foreach (Window w in System.Windows.Application.Current.Windows)
			if (w is HotkeyView)
				isWindowOpen = true;

		if (isWindowOpen) return;
		var hotkeyDialog = new HotkeyView(this, "refresh");
		hotkeyDialog.Show();
	}

	private void StashTabHotkey_Click(object sender, RoutedEventArgs e)
	{
		var isWindowOpen = false;
		foreach (Window w in System.Windows.Application.Current.Windows)
			if (w is HotkeyView)
				isWindowOpen = true;

		if (!isWindowOpen)
		{
			var hotkeyDialog = new HotkeyView(this, "stashtab");
			hotkeyDialog.Show();
		}
	}

	private void LootFilterFileDialog_Click(object sender, RoutedEventArgs e)
	{
		var open = new OpenFileDialog();
		open.Filter = "LootFilter|*.filter";
		var res = open.ShowDialog();

		if (res != System.Windows.Forms.DialogResult.OK) return;

		var filename = open.FileName;
		Settings.Default.LootFilterFileLocation = filename;
		LootFilterFileDialog.Content = filename;
	}

	private void TabHeaderGapSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		_stashTabOverlayView.StashTabOverlayIndividualTabHeaderGap =
			new Thickness(Settings.Default.StashTabOverlayIndividualTabHeaderGap, 0,
				Settings.Default.StashTabOverlayIndividualTabHeaderGap, 0);
	}

	private void TabHeaderWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		if (StashTabControlManager.StashTabControls.Count <= 0) return;

		foreach (var s in StashTabControlManager.StashTabControls)
			s.TabHeaderWidth =
				new Thickness(Settings.Default.StashTabOverlayIndividualTabHeaderWidth, 2,
					Settings.Default.StashTabOverlayIndividualTabHeaderWidth, 2);
	}

	private void TabHeaderMarginSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
	{
		_stashTabOverlayView.StashTabOverlayIndividualTabMargin =
			new Thickness(Settings.Default.StashTabOverlayIndividualTabMargin, 0, 0, 0);
	}

	private void SaveButton_Click_1(object sender, RoutedEventArgs e)
	{
		Settings.Default.Save();
	}

	private void SetTrackerOverlayDisplayModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		switch (Settings.Default.SetTrackerOverlayDisplayMode)
		{
			case 0:
				Trace.WriteLine(
					$"OverlayModeComboBox_SelectionChanged: Case 0 {Settings.Default.SetTrackerOverlayDisplayMode}");
				_setTrackerOverlayView.MainOverlayContentControl.Content =
					new StandardDisplay(this, _setTrackerOverlayView);
				break;
			case 1:
				Trace.WriteLine(
					$"OverlayModeComboBox_SelectionChanged: Case 1 {Settings.Default.SetTrackerOverlayDisplayMode}");
				_setTrackerOverlayView.MainOverlayContentControl.Content =
					new MinifiedDisplay(this, _setTrackerOverlayView);
				break;
			case 2:
				Trace.WriteLine(
					$"OverlayModeComboBox_SelectionChanged: Case 2 {Settings.Default.SetTrackerOverlayDisplayMode}");
				_setTrackerOverlayView.MainOverlayContentControl.Content =
					new OnlyButtonsDisplay(this, _setTrackerOverlayView);
				break;
			case 3:
				Trace.WriteLine(
					$"OverlayModeComboBox_SelectionChanged: Case 3 {Settings.Default.SetTrackerOverlayDisplayMode}");
				_setTrackerOverlayView.MainOverlayContentControl.Content =
					new VerticalStandardDisplay(this, _setTrackerOverlayView);
				break;
			case 4:
				Trace.WriteLine(
					$"OverlayModeComboBox_SelectionChanged: Case 4 {Settings.Default.SetTrackerOverlayDisplayMode}");
				_setTrackerOverlayView.MainOverlayContentControl.Content =
					new VerticalMinifiedDisplay(this, _setTrackerOverlayView);
				break;
			case 5:
				Trace.WriteLine(
					$"OverlayModeComboBox_SelectionChanged: Case 5 {Settings.Default.SetTrackerOverlayDisplayMode}");
				_setTrackerOverlayView.MainOverlayContentControl.Content =
					new OnlyMinifiedButtonsDisplay(this, _setTrackerOverlayView);
				break;
		}
	}

	private void ResetButton_Click(object sender, RoutedEventArgs e)
	{
		var result = System.Windows.MessageBox.Show("This will reset all of your settings!", "Reset Settings",
			MessageBoxButton.YesNo);
		switch (result)
		{
			case MessageBoxResult.Yes:
				Settings.Default.Reset();
				break;
			case MessageBoxResult.No:
				break;
			case MessageBoxResult.None:
				break;
			case MessageBoxResult.OK:
				break;
			case MessageBoxResult.Cancel:
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void ShowNumbersComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		_setTrackerOverlayView.AmountsVisibility = Settings.Default.SetTrackerOverlayItemCounterDisplayMode != 0
			? Visibility.Visible
			: Visibility.Hidden;
	}

	#endregion

	#region Methods

	private void InitializeHotkeys()
	{
		HotkeysManager.SetupSystemHook();
		HotkeysManager.GetRefreshHotkey();
		HotkeysManager.GetToggleHotkey();
		HotkeysManager.GetStashTabHotkey();
		AddAllHotkeys();
	}

	private void InitializeColors()
	{
		if (Settings.Default.LootFilterBootsColor != "")
			ColorBootsPicker.SelectedColor =
				(Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterBootsColor);
		if (Settings.Default.LootFilterBodyArmourColor != "")
			ColorChestPicker.SelectedColor =
				(Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterBodyArmourColor);
		if (Settings.Default.LootFilterWeaponColor != "")
			ColorWeaponsPicker.SelectedColor =
				(Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterWeaponColor);
		if (Settings.Default.LootFilterGlovesColor != "")
			ColorGlovesPicker.SelectedColor =
				(Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterGlovesColor);
		if (Settings.Default.LootFilterHelmetColor != "")
			ColorHelmetPicker.SelectedColor =
				(Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterHelmetColor);
		if (Settings.Default.StashTabOverlayHighlightColor != "")
			ColorStashPicker.SelectedColor =
				(Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayHighlightColor);
		if (Settings.Default.StashTabOverlayBackgroundColor != "")
			ColorStashBackgroundPicker.SelectedColor =
				(Color)ColorConverter.ConvertFromString(Settings.Default.StashTabOverlayBackgroundColor);
		if (Settings.Default.LootFilterRingColor != "")
			ColorRingPicker.SelectedColor =
				(Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterRingColor);
		if (Settings.Default.LootFilterAmuletColor != "")
			ColorAmuletPicker.SelectedColor =
				(Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterAmuletColor);
		if (Settings.Default.LootFilterBeltColor != "")
			ColorBeltPicker.SelectedColor =
				(Color)ColorConverter.ConvertFromString(Settings.Default.LootFilterBeltColor);
	}

	// creates tray icon with menu
	private void InitializeTray()
	{
		_notifyIcon.Icon = Properties.Resources.CREIcon;
		_notifyIcon.Visible = true;
		_notifyIcon.DoubleClick +=
			delegate
			{
				Show();
				WindowState = WindowState.Normal;
			};

		new Container();
		_contextMenuStrip = new ContextMenuStrip();
		_exitMenuItem = new ToolStripMenuItem();
		_checkForUpdatesMenuItem = new ToolStripMenuItem();

		// Initialize contextMenu1
		_contextMenuStrip.Items.AddRange(new[] { _exitMenuItem, _checkForUpdatesMenuItem });

		// Initialize menuItem1
		_exitMenuItem.MergeIndex = 1;
		_exitMenuItem.Text = "E&xit";
		_exitMenuItem.Click += ExitMenuItemClick;

		// Initialize menuItemUpdate
		_checkForUpdatesMenuItem.MergeIndex = 0;
		_checkForUpdatesMenuItem.Text = "C&heck for Updates";
		_checkForUpdatesMenuItem.Click += CheckForClick;

		_notifyIcon.ContextMenuStrip = _contextMenuStrip;
	}

	public void RunOverlay()
	{
		if (_setTrackerOverlayView.IsOpen)
		{
			_setTrackerOverlayView.Hide();
			if (_stashTabOverlayView.IsOpen) _stashTabOverlayView.Hide();
			RunButton.Content = "Run Overlay";
		}
		else
		{
			if (CheckAllSettings())
			{
				_setTrackerOverlayView.Show();
				RunButton.Content = "Stop Overlay";
			}
		}
	}

	public void RunStashTabOverlay()
	{
		var ready = CheckAllSettings();
		if (ready)
		{
			if (_stashTabOverlayView.IsOpen)
				_stashTabOverlayView.Hide();
			else
				_stashTabOverlayView.Show();
		}
	}

	public void AddAllHotkeys()
	{
		if (Settings.Default.FetchStashHotkey != "< not set >")
			HotkeysManager.AddHotkey(HotkeysManager.refreshModifier, HotkeysManager.refreshKey,
				_setTrackerOverlayView.RunFetching);
		if (Settings.Default.ToggleSetTrackerOverlayHotkey != "< not set >")
			HotkeysManager.AddHotkey(HotkeysManager.toggleModifier, HotkeysManager.toggleKey, RunOverlay);
		if (Settings.Default.ToggleStashTabOverlayHotkey != "< not set >")
			HotkeysManager.AddHotkey(HotkeysManager.stashTabModifier, HotkeysManager.stashTabKey,
				RunStashTabOverlay);
	}

	public void RemoveAllHotkeys()
	{
		HotkeysManager.RemoveRefreshHotkey();
		HotkeysManager.RemoveStashTabHotkey();
		HotkeysManager.RemoveToggleHotkey();
	}

	private string GetSoundFilePath()
	{
		var open = new OpenFileDialog();
		open.Filter = "MP3|*.mp3";
		var res = open.ShowDialog();

		if (res == System.Windows.Forms.DialogResult.OK) return open.FileName;

		return null;
	}

	public static bool CheckAllSettings()
	{
		var accName = Settings.Default.PathOfExileAccountName;
		var sessId = Settings.Default.PathOfExileWebsiteSessionId;
		var league = Settings.Default.LeagueName;
		var lootFilterLocation = Settings.Default.LootFilterFileLocation;
		var lootFilterActive = Settings.Default.LootFilterManipulationEnabled;
		var logLocation = Settings.Default.PathOfExileClientLogLocation;
		var autoFetch = Settings.Default.AutoFetchOnRezoneEnabled;

		var missingSettings = new List<string>();

		if (accName == "") missingSettings.Add("- Account Name \n");
		if (sessId == "") missingSettings.Add("- PoE Session ID \n");
		if (league == "") missingSettings.Add("- League \n");
		if (lootFilterActive)
			if (lootFilterLocation == "")
				missingSettings.Add("- Loot Filter Location \n");

		if (autoFetch)
			if (logLocation == "")
				missingSettings.Add("- Log File Location \n");

		switch (Settings.Default.StashTabQueryMode)
		{
			case 0:
				{
					if (Settings.Default.StashTabIndices == "") missingSettings.Add("- StashTab Index");
					break;
				}
			case 1:
				{
					if (Settings.Default.StashTabPrefix == "") missingSettings.Add("- StashTab Prefix");
					break;
				}
			case 2:
				{
					if (Settings.Default.StashTabSuffix == "") missingSettings.Add("- StashTab Suffix");
					break;
				}
		}

		if (missingSettings.Count > 0)
		{
			SettingsComplete = false;
		}
		else
		{
			SettingsComplete = true;
			return true;
		}

		var errorMessage = missingSettings.Aggregate("Please add: \n", (current, setting) => current + setting);

		System.Windows.MessageBox.Show(errorMessage, "Missing Settings", MessageBoxButton.OK, MessageBoxImage.Error);
		return false;
	}

	#endregion
}