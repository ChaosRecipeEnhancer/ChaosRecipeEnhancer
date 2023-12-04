using System.Windows;
using ChaosRecipeEnhancer.UI.Properties;
using ChaosRecipeEnhancer.UI.State;

namespace ChaosRecipeEnhancer.UI.Windows;

/// <summary>
///     Interaction logic for HotkeyView.xaml
/// </summary>
public partial class HotkeyWindow
{
    private readonly SettingsWindow _settingsView;
    private readonly string _type;

    public HotkeyWindow(SettingsWindow settingsView, string hotkeyType)
    {

        _settingsView = settingsView;
        _type = hotkeyType;
        InitializeComponent();
    }

    private void SaveHotkeyButton_Click(object sender, RoutedEventArgs e)
    {
        switch (_type)
        {
            case "refresh":
                {
                    GlobalHotkeyState.RemoveHotkey(GlobalHotkeyState.refreshModifier, GlobalHotkeyState.refreshKey);
                    if (CustomHotkeyToggle.Hotkey == null)
                    {
                        Settings.Default.FetchStashHotkey = "< not set >";
                    }
                    else
                    {
                        Settings.Default.FetchStashHotkey = CustomHotkeyToggle.Hotkey.ToString();
                        GlobalHotkeyState.GetRefreshHotkey();
                    }

                    ReApplyHotkeys();
                    break;
                }
            case "toggle":
                {
                    GlobalHotkeyState.RemoveHotkey(GlobalHotkeyState.toggleModifier, GlobalHotkeyState.toggleKey);
                    if (CustomHotkeyToggle.Hotkey == null)
                    {
                        Settings.Default.ToggleSetTrackerOverlayHotkey = "< not set >";
                    }
                    else
                    {
                        Settings.Default.ToggleSetTrackerOverlayHotkey = CustomHotkeyToggle.Hotkey.ToString();
                        GlobalHotkeyState.GetToggleHotkey();
                    }

                    ReApplyHotkeys();
                    break;
                }
            case "stashtab":
                {
                    GlobalHotkeyState.RemoveHotkey(GlobalHotkeyState.stashTabModifier, GlobalHotkeyState.stashTabKey);
                    if (CustomHotkeyToggle.Hotkey == null)
                    {
                        Settings.Default.ToggleStashTabOverlayHotkey = "< not set >";
                    }
                    else
                    {
                        Settings.Default.ToggleStashTabOverlayHotkey = CustomHotkeyToggle.Hotkey.ToString();
                        GlobalHotkeyState.GetStashTabHotkey();
                    }

                    ReApplyHotkeys();
                    break;
                }
        }

        Settings.Default.Save();
        Close();
    }

    private void ReApplyHotkeys()
    {
        _settingsView.RemoveAllHotkeys();
        _settingsView.AddAllHotkeys();
    }
}