using System;

namespace ChaosRecipeEnhancer.UI.Models.Constants;

public static class HotkeyDefaults
{
    public static readonly TimeSpan FetchStashDataHotkeyCooldown = TimeSpan.FromSeconds(10);
    public static readonly TimeSpan ToggleSetTrackerHotkeyCooldown = TimeSpan.FromMilliseconds(250);
    public static readonly TimeSpan ToggleStashTabHotkeyCooldown = TimeSpan.FromMilliseconds(250);
    public static readonly TimeSpan ReloadFilterHotkeyCooldown = TimeSpan.FromSeconds(10);
}