using System.Text.Json.Serialization;
using ChaosRecipeEnhancer.UI.Models.Enums;

namespace ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;

/// <summary>
/// Represents a JSON response object nested within the previously mentioned response object for `get-stash-items`
/// and `get-guild-stash-items` endpoints.
///
/// The (nested) item object structure is as follows:
///
/// {
///     "verified": false,
///     "w": 1,
///     "h": 1,
///     "icon": "https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvSmV3ZWxzL1NlYXJjaGluZ0V5ZSIsInciOjEsImgiOjEsInNjYWxlIjoxfV0/ff2df16522/SearchingEye.png",
///     "league": "Standard",
///     "id": "7aa804a9cb366b682d0af99ad929603bebec56f38c3e0eb2848961f45f289e1e",
///     "abyssJewel": true,
///     "name": "Whispering Horn",
///     "typeLine": "Searching Eye Jewel",
///     "baseType": "Searching Eye Jewel",
///     "identified": true,
///     "ilvl": 84,
///     "properties": [{ "name": "Abyss", "values": [], "displayMode": 0 }],
///     "requirements": [
///         { "name": "Level", "values": [["51", 0]], "displayMode": 0, "type": 62 }
///     ],
///     "explicitMods": [
///         "Adds 7 to 11 Cold Damage to Attacks",
///         "Adds 1 to 36 Lightning Damage to Attacks",
///         "4 to 9 Added Cold Damage with Bow Attacks"
///     ],
///     "descrText": "Place into an Abyssal Socket on an Item or into an allocated Jewel Socket on the Passive Skill Tree. Right click to remove from the Socket.",
///     "frameType": 2,
///     "x": 11,
///     "y": 22,
///     "inventoryId": "Stash218"
/// },
///
/// Notice how there are more fields in the example than in our defined object.
/// </summary>
// This class is instantiated when serialized from JSON API response.
public class BaseItem
{
    [JsonConstructor]
    public BaseItem(
        uint width,
        uint height,
        bool identified,
        int? itemLevel,
        ItemFrameType frameType,
        int x,
        int y,
        BaseItemInfluences baseItemInfluences,
        string icon)
    {
        Width = width;
        Height = height;
        Identified = identified;
        ItemLevel = itemLevel;
        FrameType = frameType;
        X = x;
        Y = y;
        BaseItemInfluences = baseItemInfluences;
        Icon = icon;
    }

    public BaseItem(BaseItem other)
    {
        Width = other.Width;
        Height = other.Height;
        Identified = other.Identified;
        ItemLevel = other.ItemLevel;
        FrameType = other.FrameType;
        X = other.X;
        Y = other.Y;
        BaseItemInfluences = other.BaseItemInfluences;
        Icon = other.Icon;
    }

    public BaseItem() { }

    // poe json props
    [JsonPropertyName("w")] public uint Width { get; set; }

    [JsonPropertyName("h")] public uint Height { get; set; }

    [JsonPropertyName("identified")] public bool Identified { get; set; }

    [JsonPropertyName("ilvl")] public int? ItemLevel { get; set; }

    [JsonPropertyName("frameType")] public ItemFrameType FrameType { get; set; }

    [JsonPropertyName("x")] public int X { get; set; }

    [JsonPropertyName("y")] public int Y { get; set; }

    [JsonPropertyName("influences")] public BaseItemInfluences BaseItemInfluences { get; set; }

    // Property must be set to `public` so that it can be [de]serialized
    [JsonPropertyName("icon")] public string Icon { get; set; }
}
