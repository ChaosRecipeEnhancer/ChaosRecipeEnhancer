using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CAmuletsManager : ABaseItemClassManager
{
    public CAmuletsManager()
    {
        ClassName = "Amulets";
        ClassFilterName = "\"Amulets\"";

        FontSize = Settings.Default.LootFilterStylesAmuletTextFontSize;
        FontColor = Settings.Default.LootFilterStylesAmuletTextColor;
        BorderColor = Settings.Default.LootFilterStylesAmuletBorderColor;
        ClassColor = Settings.Default.LootFilterStylesAmuletBackgroundColor;
        AlwaysActive = Settings.Default.LootFilterStylesAmuletAlwaysActive;
        AlwaysHidden = Settings.Default.LootFilterStylesAmuletAlwaysDisabled;
        MapIconEnabled = Settings.Default.LootFilterStylesAmuletMapIconEnabled;
        MapIconColor = Settings.Default.LootFilterStylesAmuletMapIconColor;
        MapIconSize = Settings.Default.LootFilterStylesAmuletMapIconSize;
        MapIconShape = Settings.Default.LootFilterStylesAmuletMapIconShape;
        BeamEnabled = Settings.Default.LootFilterStylesAmuletBeamEnabled;
        BeamTemporary = Settings.Default.LootFilterStylesAmuletBeamTemporary;
        BeamColor = Settings.Default.LootFilterStylesAmuletBeamColor;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.AmuletActive = newValue;
        return activeItems;
    }
}