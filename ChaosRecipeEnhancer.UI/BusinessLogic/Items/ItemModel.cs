using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.Items
{
    /// <summary>
    /// Represents JSON response objects from `get-stash-items` and `get-guild-stash-items` endpoints.
    ///
    /// The full response object is structured as follows:
    ///
    ///     {
    ///         "numTabs" : number,
    ///         "quadLayout": bool
    ///         "items": [ ... ]
    ///     }
    ///
    /// The only thing we're interested in is the `items` array and the `quadLayout` field.
    /// </summary>
    public class ItemList
    {
        [JsonPropertyName("items")] public List<ItemModel> Items { get; set; }
        [JsonPropertyName("quadLayout")] public bool IsQuadLayout { get; set; }
    }
    
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
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemModel
    {
        public ItemModel(
            int width,
            int height,
            bool identified,
            int? itemLevel,
            int frameType,
            int x,
            int y,
            ItemInfluences itemInfluences,
            string icon)
        {
            Width = width;
            Height = height;
            Identified = identified;
            ItemLevel = itemLevel;
            FrameType = frameType;
            X = x;
            Y = y;
            ItemInfluences = itemInfluences;
            Icon = icon;
        }

        // poe json props
        [JsonPropertyName("w")] public int Width { get; }

        [JsonPropertyName("h")] public int Height { get; }

        [JsonPropertyName("identified")] public bool Identified { get; }

        [JsonPropertyName("ilvl")] public int? ItemLevel { get; }

        [JsonPropertyName("frameType")] public int FrameType { get; }

        [JsonPropertyName("x")] public int X { get; }

        [JsonPropertyName("y")] public int Y { get; }

        [JsonPropertyName("influences")] public ItemInfluences ItemInfluences { get; }

        // Property must be set to `public` so that it can be [de]serialized
        // ReSharper disable once MemberCanBePrivate.Global
        [JsonPropertyName("icon")] public string Icon { get; }

        // own prop - refactor this to DTO
        public string DerivedItemClass { get; set; }
        public int StashTabIndex { get; set; }

        /// <summary>
        /// So the default Response Object does not contain a direct reference to an items base type.
        /// (i.e. Helmet, Gloves, Body Armour, Ring, etc.)
        /// Because of this, we have a clever workaround to derive a item class using the included IconUrl.
        /// </summary>
        // TODO: [Refactor] Move this method into DTO utility object (?) extension object maybe
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

    // This class is instantiated when serialized from JSON API response.
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ItemInfluences
    {
        [JsonPropertyName("shaper")] public bool Shaper { get; set; } = false;
        [JsonPropertyName("elder")] public bool Elder { get; set; } = false;
        [JsonPropertyName("crusader")] public bool Crusader { get; set; } = false;
        [JsonPropertyName("redeemer")] public bool Redeemer { get; set; } = false;
        [JsonPropertyName("hunter")] public bool Hunter { get; set; } = false;
        [JsonPropertyName("warlord")] public bool Warlord { get; set; } = false;
    }
}