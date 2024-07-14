namespace ChaosRecipeEnhancer.UI.Models.Enums;

public enum ConnectionStatusTypes
{
    ConnectionNotValidated,
    ValidatedConnection,
    ConnectionError,
    AttemptingLogin
}

public enum TargetStash
{
    Personal,
    Guild
}

public enum StashTabOverlayHighlightMode
{
    SetBySet,
    ItemByItem
}

public enum StashTabQueryMode
{
    SelectTabsByIndex,
    TabNamePrefix,
    SelectTabsById
}

public enum SetTrackerOverlayMode
{
    Standard,
    VerticalStandard,
    Minified,
    VerticalMinified,
    OnlyButtons,
    OnlyButtonsVertical,
    OnlyMinifiedButtons,
    OnlyMinifiedButtonsVertical
}

public enum SetTrackerOverlayItemCounterDisplayMode
{
    None,
    TotalItems,
    ItemsMissing
}

public enum ClientLogFileLocationMode
{
    DefaultStandaloneLocation,
    DefaultSteamLocation,
    CustomLocation
}

public enum Language
{
    English,
    French,
    German,
    Portuguese,
    Russian,
    Spanish,
    Japanese,
    Korean
}