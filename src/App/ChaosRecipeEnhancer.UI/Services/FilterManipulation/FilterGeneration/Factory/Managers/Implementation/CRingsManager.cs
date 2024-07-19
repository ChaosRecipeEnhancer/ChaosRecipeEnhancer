using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CRingsManager : ABaseItemClassManager
{
    public CRingsManager()
    {
        ClassName = "Rings";
        ClassFilterName = "\"Rings\"";

        FontSize = Settings.Default.LootFilterStylesRingTextFontSize;
        FontColor = Settings.Default.LootFilterStylesRingTextColor;
        BorderColor = Settings.Default.LootFilterStylesRingBorderColor;
        ClassColor = Settings.Default.LootFilterStylesRingBackgroundColor;
        AlwaysActive = Settings.Default.LootFilterStylesRingAlwaysActive;
        AlwaysHidden = Settings.Default.LootFilterStylesRingAlwaysDisabled;
        MapIconEnabled = Settings.Default.LootFilterStylesRingMapIconEnabled;
        MapIconColor = Settings.Default.LootFilterStylesRingMapIconColor;
        MapIconSize = Settings.Default.LootFilterStylesRingMapIconSize;
        MapIconShape = Settings.Default.LootFilterStylesRingMapIconShape;
        BeamEnabled = Settings.Default.LootFilterStylesRingBeamEnabled;
        BeamTemporary = Settings.Default.LootFilterStylesRingBeamTemporary;
        BeamColor = Settings.Default.LootFilterStylesRingBeamColor;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.RingActive = newValue;
        return activeItems;
    }
}