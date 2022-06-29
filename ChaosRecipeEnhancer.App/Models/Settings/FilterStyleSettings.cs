using System.ComponentModel;

namespace ChaosRecipeEnhancer.App.Models.Settings
{
    public sealed partial class Settings
    {
        #region Filter Style Settings

        [DefaultValue("#FFF8FF04")] public string ColorHelmet { get; set; }

        [DefaultValue("#FFDD00FF")] public string ColorChest { get; set; }

        [DefaultValue("#FF04FF00")] public string ColorGloves { get; set; }

        [DefaultValue("#FF0018FF")] public string ColorBoots { get; set; }

        [DefaultValue("#FF00DCFF")] public string ColorWeapon { get; set; }

        [DefaultValue("#FFFF0303")] public string ColorAmulet { get; set; }

        [DefaultValue("#FFFF0303")] public string ColorRing { get; set; }

        [DefaultValue("#FFFF0303")] public string ColorBelt { get; set; }

        #endregion
    }
}