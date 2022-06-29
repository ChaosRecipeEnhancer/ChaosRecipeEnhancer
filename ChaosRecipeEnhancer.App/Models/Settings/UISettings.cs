using System.ComponentModel;

namespace ChaosRecipeEnhancer.App.Models.Settings
{
    public sealed partial class Settings
    {
        #region App UI Settings

        [DefaultValue(false)] public bool LockOverlayPosition { get; set; }

        [DefaultValue(0)] public double TopMain { get; set; }

        [DefaultValue(0)] public double LeftMain { get; set; }

        [DefaultValue(0)] public double TopOverlay { get; set; }

        [DefaultValue(0)] public double LeftOverlay { get; set; }

        [DefaultValue(1)] public float Opacity { get; set; }

        [DefaultValue(74)] public double TopStashTabOverlay { get; set; }

        [DefaultValue(12)] public double LeftStashTabOverlay { get; set; }

        [DefaultValue(641)] public double XStashTabOverlay { get; set; }

        [DefaultValue(690)] public double YStashTabOverlay { get; set; }

        [DefaultValue(1)] public float OpacityStashTab { get; set; }

        [DefaultValue(20.4)] public double TabHeaderWidth { get; set; }

        [DefaultValue(5)] public double TabHeaderGap { get; set; }

        [DefaultValue(23)] public double TabMargin { get; set; }

        [DefaultValue("#96F90000")] public string ColorStash { get; set; }

        #endregion
    }
}