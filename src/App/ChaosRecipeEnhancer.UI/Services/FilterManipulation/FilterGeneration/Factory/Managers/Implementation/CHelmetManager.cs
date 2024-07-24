using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CHelmetManager : ABaseItemClassManager
{
    public CHelmetManager()
    {
        ClassName = "Helmets";
        ClassFilterName = "\"Helmets\"";

        FontSize = Settings.Default.LootFilterStylesHelmetTextFontSize;
        FontColor = Settings.Default.LootFilterStylesHelmetTextColor;
        BorderColor = Settings.Default.LootFilterStylesHelmetBorderColor;
        ClassColor = Settings.Default.LootFilterStylesHelmetBackgroundColor;
        AlwaysActive = Settings.Default.LootFilterStylesHelmetAlwaysActive;
        AlwaysHidden = Settings.Default.LootFilterStylesHelmetAlwaysDisabled;
        MapIconEnabled = Settings.Default.LootFilterStylesHelmetMapIconEnabled;
        MapIconColor = Settings.Default.LootFilterStylesHelmetMapIconColor;
        MapIconSize = Settings.Default.LootFilterStylesHelmetMapIconSize;
        MapIconShape = Settings.Default.LootFilterStylesHelmetMapIconShape;
        BeamEnabled = Settings.Default.LootFilterStylesHelmetBeamEnabled;
        BeamTemporary = Settings.Default.LootFilterStylesHelmetBeamTemporary;
        BeamColor = Settings.Default.LootFilterStylesHelmetBeamColor;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.HelmetActive = newValue;
        return activeItems;
    }
}