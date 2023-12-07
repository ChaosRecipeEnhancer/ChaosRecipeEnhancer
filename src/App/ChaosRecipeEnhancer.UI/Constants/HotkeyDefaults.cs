using System;

namespace ChaosRecipeEnhancer.UI.Constants;

public static class HotkeyDefaults
{
    public static readonly TimeSpan FetchHotkeyCooldown = TimeSpan.FromMilliseconds(3000);
    public static readonly TimeSpan ToggleSetTrackerHotkeyCooldown = TimeSpan.FromMilliseconds(50);
    public static readonly TimeSpan ToggleStashTabHotkeyCooldown = TimeSpan.FromMilliseconds(50);
    public static readonly TimeSpan ReloadFilterHotkeyCooldown = TimeSpan.FromMilliseconds(3000);
}