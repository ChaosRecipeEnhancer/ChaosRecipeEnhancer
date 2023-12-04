using System;
using System.Text;
using ChaosRecipeEnhancer.UI.Models.ApiResponses.BaseModels;
using ChaosRecipeEnhancer.UI.Models.Enums;

namespace ChaosRecipeEnhancer.UI.Models;

// This class is instantiated when serialized from JSON API response.
public class EnhancedItem : BaseItem
{
    public EnhancedItem(
        uint width,
        uint height,
        bool identified,
        int? itemLevel,
        ItemFrameType frameType,
        int x,
        int y,
        BaseItemInfluences baseItemInfluences,
        string icon
    ) : base(width, height, identified, itemLevel, frameType, x, y, baseItemInfluences, icon)
    {
        GetItemClass();
    }

    public EnhancedItem(BaseItem other) : base(other)
    {
        GetItemClass();
    }

    public EnhancedItem() { }

    public bool IsChaosRecipeEligible => ItemLevel is >= 60 and <= 74;
    public string DerivedItemClass { get; set; }
    public int StashTabIndex { get; set; }

    /// <summary>
    /// So the default Response Object does not contain a direct reference to an items base type.
    /// (i.e. Helmet, Gloves, Body Armour, Ring, etc.)
    /// Because of this, we have a clever workaround to derive a item class using the included IconUrl.
    /// </summary>
    public void GetItemClass()
    {
        // Example IconUrl: https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQXJtb3Vycy9IZWxtZXRzL0hlbG1ldFN0ckRleDciLCJ3IjoyLCJoIjoyLCJzY2FsZSI6MX1d/0884b27765/HelmetStrDex7.png
        //
        // Distinct URL parts split into array (after removing empty entries):
        //
        //  [
        //      "https:",
        //      "web.poecdn.com",
        //      "gen",
        //      "image",
        //      "WzI1LDE0LHsiZiI6IjJESXRlbXMvUmluZ3MvVG9wYXpTYXBwaGlyZSIsInciOjEsImgiOjEsInNjYWxlIjoxfV0",
        //      "0c211c12b4",
        //      "TopazSapphire.png"
        //  ]
        var urlParts = Icon.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

        // We're interested in the 5th element of our array, a Base64 encoded string representation that contains
        // some metadata about the item's icon.
        //
        // Original Base64 String: WzI1LDE0LHsiZiI6IjJESXRlbXMvUmluZ3MvVG9wYXpTYXBwaGlyZSIsInciOjEsImgiOjEsInNjYWxlIjoxfV0
        var encodedPart = urlParts[4];

        // Padding our Base64 string as needed to not lose our original contents
        // See: https://stackoverflow.com/a/26632221
        while (encodedPart.Length % 4 != 0) encodedPart += "=";

        // Decoding our item data from our (newly padded) Base64 string
        //
        // Decoded String: [25,14,{"f":"2DItems/Rings/TopazSapphire","w":1,"h":1,"scale":1}]
        //
        // Decoded String (JSON) Expanded out for readability:
        //
        //  [
        //     25,
        //     14,
        //     {
        //         "f": "2DItems/Rings/TopazSapphire",
        //         "w": 1,
        //         "h": 1,
        //         "scale": 1
        //     }
        //  ]
        var decodedItemData = Encoding.UTF8.GetString(Convert.FromBase64String(encodedPart));

        // Decoded String: [25,14,{"f":"2DItems/Rings/TopazSapphire","w":1,"h":1,"scale":1}]
        //
        // Icon metadata split by '/' delimiter (after removing empty entries):

        //  [
        //     "[25,14,{"f":"2DItems",
        //     "Rings",
        //     "TopazSapphire","w":1,"h":1,"scale":1}]"
        //  ]
        var iconParts = decodedItemData.Split('/');

        // We're interested in the 2nd element of our new icon metadata string array, that spells out the item
        // class. In our example, this class as "Rings" or a ring item class.
        var itemClass = iconParts[1];

        // In the case of item classes under the 'Armour' or 'Weapon' types (i.e. Boots, Gloves, Daggers, Wands,
        // etc.) there are extra forward slashes in the icon metadata, as show here:
        //
        //  [
        //     "[25,14,{"f":"2DItems",
        //     "Armours",
        //     "Boots",
        //     "BootsStr3","w":2,"h":2,"scale":1}]"
        //  ]
        //
        // So we have to modify which index we use to identify the true item class.
        switch (itemClass)
        {
            case "Weapons":
            case "Armours":
                itemClass = iconParts[2];
                break;
            case "Rings":
            case "Amulets":
            case "Belts":
                break;
        }

        DerivedItemClass = itemClass;
    }
}
