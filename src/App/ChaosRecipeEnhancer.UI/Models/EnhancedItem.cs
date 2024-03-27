using ChaosRecipeEnhancer.UI.Models.ApiResponses;
using ChaosRecipeEnhancer.UI.Models.Enums;
using System;
using System.Text;

namespace ChaosRecipeEnhancer.UI.Models;

/// <summary>
/// Represents an item with enhanced properties, derived from a base item,
/// with additional logic to determine eligibility for in-game vendor recipes
/// and to extract item classification from its icon URL.
/// </summary>
public class EnhancedItem : BaseItem
{
    /// <summary>
    /// Initializes a new instance of the EnhancedItem class using specified item properties.
    /// </summary>
    /// <param name="width">The width of the item in inventory slots.</param>
    /// <param name="height">The height of the item in inventory slots.</param>
    /// <param name="identified">Indicates whether the item has been identified.</param>
    /// <param name="itemLevel">The item level.</param>
    /// <param name="frameType">The frame type of the item, indicating its rarity.</param>
    /// <param name="x">The X position of the item in the stash.</param>
    /// <param name="y">The Y position of the item in the stash.</param>
    /// <param name="baseItemInfluences">The influences affecting the item.</param>
    /// <param name="icon">The URL of the item's icon.</param>
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
        Id = Guid.NewGuid().ToString();
        GetItemClass();
    }

    /// <summary>
    /// Initializes a new instance of the EnhancedItem class by copying properties from another base item.
    /// </summary>
    /// <param name="other">The base item to copy properties from.</param>
    public EnhancedItem(BaseItem other) : base(other)
    {
        Id = Guid.NewGuid().ToString();
        GetItemClass();
    }

    /// <summary>
    /// Initializes a new instance of the EnhancedItem class with default properties.
    /// </summary>
    public EnhancedItem()
    {
        Id = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// Gets or sets the unique identifier of the item.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets a value indicating whether the item is eligible for the Chaos Orb vendor recipe.
    /// Chaos Orb Vendor Recipe requires an item level of 60 to 74.
    /// See <a href="https://www.poewiki.net/wiki/Chaos_Orb#Vendor_recipes"></a>
    /// </summary>
    public bool IsChaosRecipeEligible => ItemLevel is >= 60 and <= 74;

    /// <summary>
    /// Gets a value indicating whether the item is eligible for the Regal Orb vendor recipe.
    /// Regal Orb Vendor Recipe requires an item level of 75 or higher.
    /// See <a href="https://www.poewiki.net/wiki/Regal_Orb#Vendor_recipe"></a>
    /// </summary>
    public bool IsRegalRecipeEligible => ItemLevel is >= 75;

    /// <summary>
    /// Gets or sets the derived classification of the item based on its icon URL.
    /// </summary>
    public string DerivedItemClass { get; set; }

    /// <summary>
    /// Gets or sets the index of the stash tab where the item is located.
    /// </summary>
    public int StashTabIndex { get; set; }

    /// <summary>
    /// Derives the item class based on the encoded information within the item's icon URL.
    /// This method is a workaround for the absence of a direct item class reference in the default response object.
    /// <br /> <br />
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
