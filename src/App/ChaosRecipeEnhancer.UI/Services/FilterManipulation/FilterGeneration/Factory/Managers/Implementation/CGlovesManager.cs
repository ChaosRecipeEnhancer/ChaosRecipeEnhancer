using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CGlovesManager : ABaseItemClassManager
{
    public CGlovesManager()
    {
        ClassName = "Gloves";
        ClassFilterName = "\"Gloves\"";

        FontSize = Settings.Default.LootFilterStylesGlovesTextFontSize;
        FontColor = Settings.Default.LootFilterStylesGlovesTextColor;
        BorderColor = Settings.Default.LootFilterStylesGlovesBorderColor;
        ClassColor = Settings.Default.LootFilterStylesGlovesBackgroundColor;
        AlwaysActive = Settings.Default.LootFilterStylesGlovesAlwaysActive;
        AlwaysHidden = Settings.Default.LootFilterStylesGlovesAlwaysDisabled;
        MapIconEnabled = Settings.Default.LootFilterStylesGlovesMapIconEnabled;
        MapIconColor = Settings.Default.LootFilterStylesGlovesMapIconColor;
        MapIconSize = Settings.Default.LootFilterStylesGlovesMapIconSize;
        MapIconShape = Settings.Default.LootFilterStylesGlovesMapIconShape;
        BeamEnabled = Settings.Default.LootFilterStylesGlovesBeamEnabled;
        BeamTemporary = Settings.Default.LootFilterStylesGlovesBeamTemporary;
        BeamColor = Settings.Default.LootFilterStylesGlovesBeamColor;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.GlovesActive = newValue;
        return activeItems;
    }
}