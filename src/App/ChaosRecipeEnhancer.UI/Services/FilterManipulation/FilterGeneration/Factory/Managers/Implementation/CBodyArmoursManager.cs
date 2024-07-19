using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CBodyArmoursManager : ABaseItemClassManager
{
    public CBodyArmoursManager()
    {
        ClassName = "BodyArmours";
        ClassFilterName = "\"Body Armours\"";

        FontSize = Settings.Default.LootFilterStylesBodyArmourTextFontSize;
        FontColor = Settings.Default.LootFilterStylesBodyArmourTextColor;
        BorderColor = Settings.Default.LootFilterStylesBodyArmourBorderColor;
        ClassColor = Settings.Default.LootFilterStylesBodyArmourBackgroundColor;
        AlwaysActive = Settings.Default.LootFilterStylesBodyArmourAlwaysActive;
        AlwaysHidden = Settings.Default.LootFilterStylesBodyArmourAlwaysDisabled;
        MapIconEnabled = Settings.Default.LootFilterStylesBodyArmourMapIconEnabled;
        MapIconColor = Settings.Default.LootFilterStylesBodyArmourMapIconColor;
        MapIconSize = Settings.Default.LootFilterStylesBodyArmourMapIconSize;
        MapIconShape = Settings.Default.LootFilterStylesBodyArmourMapIconShape;
        BeamEnabled = Settings.Default.LootFilterStylesBodyArmourBeamEnabled;
        BeamTemporary = Settings.Default.LootFilterStylesBodyArmourBeamTemporary;
        BeamColor = Settings.Default.LootFilterStylesBodyArmourBeamColor;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.BootsActive = newValue;
        return activeItems;
    }
}