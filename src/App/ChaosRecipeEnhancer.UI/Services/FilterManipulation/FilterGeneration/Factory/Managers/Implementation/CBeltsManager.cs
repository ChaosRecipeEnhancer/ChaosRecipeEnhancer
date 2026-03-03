using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.Services.FilterManipulation.FilterGeneration.Factory.Managers.Implementation;

public class CBeltsManager : ABaseItemClassManager
{
    public CBeltsManager()
    {
        ClassName = "Belts";
        ClassFilterName = "\"Belts\"";

        FontSize = Settings.Default.LootFilterStylesBeltTextFontSize;
        FontColor = Settings.Default.LootFilterStylesBeltTextColor;
        BorderColor = Settings.Default.LootFilterStylesBeltBorderColor;
        ClassColor = Settings.Default.LootFilterStylesBeltBackgroundColor;
        AlwaysActive = Settings.Default.LootFilterStylesBeltAlwaysActive;
        AlwaysHidden = Settings.Default.LootFilterStylesBeltAlwaysDisabled;
        MapIconEnabled = Settings.Default.LootFilterStylesBeltMapIconEnabled;
        MapIconColor = Settings.Default.LootFilterStylesBeltMapIconColor;
        MapIconSize = Settings.Default.LootFilterStylesBeltMapIconSize;
        MapIconShape = Settings.Default.LootFilterStylesBeltMapIconShape;
        BeamEnabled = Settings.Default.LootFilterStylesBeltBeamEnabled;
        BeamTemporary = Settings.Default.LootFilterStylesBeltBeamTemporary;
        BeamColor = Settings.Default.LootFilterStylesBeltBeamColor;
        SoundMode = Settings.Default.LootFilterStylesBeltSoundMode;
        SoundId = Settings.Default.LootFilterStylesBeltSoundId;
        SoundVolume = Settings.Default.LootFilterStylesBeltSoundVolume;
        CustomSoundPath = Settings.Default.LootFilterStylesBeltCustomSoundPath;
    }

    public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
    {
        activeItems.BeltActive = newValue;
        return activeItems;
    }
}
