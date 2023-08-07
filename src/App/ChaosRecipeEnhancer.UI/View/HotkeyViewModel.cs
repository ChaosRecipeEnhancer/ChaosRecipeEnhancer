using ChaosRecipeEnhancer.UI.Utilities.UI;

namespace ChaosRecipeEnhancer.UI.View;

public class HotkeyViewModel : ViewModelBase
{
    public void RemoveAllHotkeys()
    {
        HotkeysManager.RemoveRefreshHotkey();
        HotkeysManager.RemoveStashTabHotkey();
        HotkeysManager.RemoveToggleHotkey();
    }
    
    private void CustomHotkeyToggle_Click(object sender, RoutedEventArgs e)
    {
        var isWindowOpen = false;
        foreach (Window w in Application.Current.Windows)
            if (w is HotkeyView)
                isWindowOpen = true;

        if (isWindowOpen) return;
        var hotkeyDialog = new HotkeyView(this, "toggle");
        hotkeyDialog.Show();
    }

    private void RefreshHotkey_Click(object sender, RoutedEventArgs e)
    {
        var isWindowOpen = false;
        foreach (Window w in Application.Current.Windows)
            if (w is HotkeyView)
                isWindowOpen = true;

        if (isWindowOpen) return;
        var hotkeyDialog = new HotkeyView(this, "refresh");
        hotkeyDialog.Show();
    }

    private void StashTabHotkey_Click(object sender, RoutedEventArgs e)
    {
        var isWindowOpen = false;
        foreach (Window w in Application.Current.Windows)
            if (w is HotkeyView)
                isWindowOpen = true;

        if (!isWindowOpen)
        {
            var hotkeyDialog = new HotkeyView(this, "stashtab");
            hotkeyDialog.Show();
        }
    }

}