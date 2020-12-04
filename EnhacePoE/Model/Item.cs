using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancePoE
{

    // names are from api
    public class Item
    {

        // poe json props
        public int w { get; set; }
        public int h { get; set; }
        public string league { get; set; }
        public string typeLine { get; set; }
        public string name { get; set; }
        public bool identified { get; set; }
        public int ilvl { get; set; }
        public int frameType { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public string inventoryId { get; set; }
        public string id { get; set; }

        // own prop
        public string ItemType { get; set; }
    }

    public class ItemList
    {
        public List<Item> items { get; set; }
    }
}
