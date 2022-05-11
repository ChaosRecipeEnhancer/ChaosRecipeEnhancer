using System;
using System.Text;
using EnhancePoE.DataModels.GGGModels;

namespace EnhancePoE.App.Models
{
    /// <summary>
    /// TODO
    /// </summary>
    public class CREItemWrapper
    {
        #region Properties

        public Item Item { get; set; }
        public string ItemType { get; set; }
        public int StashTabIndex { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// For some reason, we're using the Icon supplied by GGG's API response to figure out what ItemClass our item is.
        /// </summary>
        public void GetItemClass()
        {
            // Sample URL: https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQXJtb3Vycy9IZWxtZXRzL0hlbG1ldFN0ckRleDciLCJ3IjoyLCJoIjoyLCJzY2FsZSI6MX1d/0884b27765/HelmetStrDex7.png

            // Splitting our URL into 7 different parts, separating by the '/' character
            var urlParts = Item.Icon.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            // Saving the encoded bit we actually care about (more on why we care about this bit after we decode it...)
            var encodedPart = urlParts[4];

            // We have to pad the end of the string to decode properly further down
            // See: https://stackoverflow.com/a/18518605/10072406
            while (encodedPart.Length % 4 != 0)
            {
                encodedPart += "=";
            }

            // Sample decoded data below
            // 
            //  Armour and Weapons:
            //  
            //      [25,14,{"f":"2DItems/Armours/BodyArmours/BodyStrInt1B","w":2,"h":3,"scale":1}]
            //  
            //  Rings, Amulets, and Belts:
            //  
            //      [25,14,{"f":"2DItems/Amulets/Amulet5","w":1,"h":1,"scale":1}]
            //  
            //  There's a difference in length in the data. Because of this, we check 
            //
            var decodedItemData = Encoding.UTF8.GetString(Convert.FromBase64String(encodedPart));

            // Further splitting to isolate the single word in between the two '/' characters
            var iconParts = decodedItemData.Split('/');

            // In the example above for amulet, iconParts[1] will be equal to the string "Amulets"
            var itemClass = iconParts[1];

            // In the case of Armours, we go one index up and assign the type "BodyArmours" instead
            // To do that, we use this nasty switch statement which can probably be made into a simple if
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

            ItemType = itemClass;

            // Does this method need to exist? I'm not 100% sure. It can definitely be refactored, at least I hope.
            //System.Diagnostics.Trace.WriteLine("item class ", itemClass);
        }

        #endregion
    }
}
