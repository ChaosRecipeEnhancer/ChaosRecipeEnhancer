using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CBootsManager : ABaseItemClassManager
{
    public CBootsManager()
    {
        ClassName = "Boots";
        ClassFilterName = "\"Boots\"";

        FontSize = Settings.Default.LootFilterStylesBootsTextFontSize;
        FontColor = Settings.Default.LootFilterStylesBootsTextColor;
        BorderColor = Settings.Default.LootFilterStylesBootsBorderColor;
        ClassColor = Settings.Default.LootFilterStylesBootsBackgroundColor;
        AlwaysActive = Settings.Default.LootFilterStylesBootsAlwaysActive;
        AlwaysHidden = Settings.Default.LootFilterStylesBootsAlwaysDisabled;
        MapIconEnabled = Settings.Default.LootFilterStylesBootsMapIconEnabled;
        MapIconColor = Settings.Default.LootFilterStylesBootsMapIconColor;
        MapIconSize = Settings.Default.LootFilterStylesBootsMapIconSize;
        MapIconShape = Settings.Default.LootFilterStylesBootsMapIconShape;
        BeamEnabled = Settings.Default.LootFilterStylesBootsBeamEnabled;
        BeamTemporary = Settings.Default.LootFilterStylesBootsBeamTemporary;
        BeamColor = Settings.Default.LootFilterStylesBootsBeamColor;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.BootsActive = newValue;
        return activeItems;
    }
}