using System;
using System.Windows;
using ChaosRecipeEnhancer.UI.Models.Enums;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.State;

namespace ChaosRecipeEnhancer.UI.Windows;

/// <summary>
///     Interaction logic for HotkeyView.xaml
/// </summary>
public partial class HotkeyWindow
{
    private readonly SettingsWindow _settingsView;
    private readonly HotkeyTypes _type;

    public HotkeyWindow(SettingsWindow settingsView, HotkeyTypes hotkeyType)
    {
        _settingsView = settingsView;
        _type = hotkeyType;
        InitializeComponent();
    }

    private void SaveHotkeyButton_Click(object sender, RoutedEventArgs e)
    {
        switch (_type)
        {
            case HotkeyTypes.FetchStashData:
                {
                    Settings.Default.FetchStashHotkey = CustomHotkeyToggle.Hotkey == null
                        ? "< not set >"
                        : CustomHotkeyToggle.Hotkey.ToString();

                    break;
                }
            case HotkeyTypes.ToggleSetTrackerOverlay:
                {
                    Settings.Default.ToggleSetTrackerOverlayHotkey = CustomHotkeyToggle.Hotkey == null
                        ? "< not set >"
                        : CustomHotkeyToggle.Hotkey.ToString();

                    break;
                }
            case HotkeyTypes.ToggleStashTabOverlay:
                {
                    Settings.Default.ToggleStashTabOverlayHotkey = CustomHotkeyToggle.Hotkey == null
                        ? "< not set >"
                        : CustomHotkeyToggle.Hotkey.ToString();

                    break;
                }
            case HotkeyTypes.ReloadItemFilter:
                {
                    Settings.Default.ReloadFilterHotkey = CustomHotkeyToggle.Hotkey == null
                        ? "< not set >"
                        : CustomHotkeyToggle.Hotkey.ToString();

                    break;
                }
            default:
                throw new ArgumentOutOfRangeException();
        }

        Settings.Default.Save();
        ReApplyHotkeys();
        Close();
    }

    private void ReApplyHotkeys()
    {
        _settingsView.ResetGlobalHotkeyState();
        _settingsView.SetAllHotkeysFromSettings();
    }
}