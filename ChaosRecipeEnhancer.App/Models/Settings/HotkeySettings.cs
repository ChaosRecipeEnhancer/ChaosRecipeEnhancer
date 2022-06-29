using System.ComponentModel;

namespace ChaosRecipeEnhancer.App.Models.Settings
{
    public sealed partial class Settings
    {
        #region Hotkey Settings

        [DefaultValue("< not set >")] public string HotkeyToggle { get; set; }

        [DefaultValue("< not set >")] public string HotkeyRefresh { get; set; }

        [DefaultValue("< not set >")] public string HotkeyStashTab { get; set; }

        [DefaultValue("< not set >")] public string HotkeyReloadFilter { get; set; }

        #endregion
    }
}