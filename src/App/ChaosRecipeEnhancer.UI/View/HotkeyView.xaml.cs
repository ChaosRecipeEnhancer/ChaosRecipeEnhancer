using System.Windows;
using ChaosRecipeEnhancer.UI.BusinessLogic.Hotkeys;
using ChaosRecipeEnhancer.UI.Properties;

//legacytodo: Auto-focus on open
namespace ChaosRecipeEnhancer.UI.View;

/// <summary>
///     Interaction logic for HotkeyView.xaml
/// </summary>
public partial class HotkeyView
{
    public HotkeyView(SettingsView settingsView, string hotkeyType)
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
                    HotkeysManager.RemoveHotkey(HotkeysManager.refreshModifier, HotkeysManager.refreshKey);
                    if (CustomHotkeyToggle.Hotkey == null)
                    {
                        Settings.Default.FetchStashHotkey = "< not set >";
                    }
                    else
                    {
                        Settings.Default.FetchStashHotkey = CustomHotkeyToggle.Hotkey.ToString();
                        HotkeysManager.GetRefreshHotkey();
                    }

                    ReApplyHotkeys();
                    break;
                }
            case "toggle":
                {
                    HotkeysManager.RemoveHotkey(HotkeysManager.toggleModifier, HotkeysManager.toggleKey);
                    if (CustomHotkeyToggle.Hotkey == null)
                    {
                        Settings.Default.ToggleSetTrackerOverlayHotkey = "< not set >";
                    }
                    else
                    {
                        Settings.Default.ToggleSetTrackerOverlayHotkey = CustomHotkeyToggle.Hotkey.ToString();
                        HotkeysManager.GetToggleHotkey();
                    }

                    ReApplyHotkeys();
                    break;
                }
            case "stashtab":
                {
                    HotkeysManager.RemoveHotkey(HotkeysManager.stashTabModifier, HotkeysManager.stashTabKey);
                    if (CustomHotkeyToggle.Hotkey == null)
                    {
                        Settings.Default.ToggleStashTabOverlayHotkey = "< not set >";
                    }
                    else
                    {
                        Settings.Default.ToggleStashTabOverlayHotkey = CustomHotkeyToggle.Hotkey.ToString();
                        HotkeysManager.GetStashTabHotkey();
                    }

                    ReApplyHotkeys();
                    break;
                }
        }

        Close();
    }

    private void ReApplyHotkeys()
    {
        _settingsView.RemoveAllHotkeys();
        _settingsView.AddAllHotkeys();
    }
    
    private readonly SettingsView _settingsView;
    private readonly string _type;
}