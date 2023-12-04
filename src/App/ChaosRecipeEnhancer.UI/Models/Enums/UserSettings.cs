namespace ChaosRecipeEnhancer.UI.Models.Enums;

public enum ConnectionStatusTypes
{
    ConnectionNotValidated,
    ValidatedConnection,
    ConnectionError,
    AttemptingLogin
}

public enum StashTabOverlayHighlightMode
{
    SetBySet
}

public enum StashTabQueryMode
{
    SelectTabsFromList,
    TabNamePrefix
}

public enum TargetStash
{
    Personal,
    Guild
}

public enum SetTrackerOverlayMode
{
    Standard,
    VerticalStandard,
    Minified,
    VerticalMinified
}