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
    TabsById,
    TabsByNamePrefix
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

// Loot Filter Styles Stuff

public enum FilterStyleMapIconSize
{
    Large,
    Medium,
    Small
}

public enum FilterStyleMapIconColor
{
    Blue,
    Brown,
    Cyan,
    Green,
    Grey,
    Orange,
    Pink,
    Purple,
    Red,
    White,
    Yellow
}

public enum FilterStyleMapIconShape
{
    Circle,
    Cross,
    Diamond,
    Hexagon,
    Kite,
    Moon,
    Pentagon,
    Raindrop,
    Square,
    Star,
    Triangle,
    UpsideDownHouse
}

public enum FilterStyleBeamColor
{
    Blue,
    Brown,
    Cyan,
    Green,
    Grey,
    Orange,
    Pink,
    Purple,
    Red,
    White,
    Yellow
}