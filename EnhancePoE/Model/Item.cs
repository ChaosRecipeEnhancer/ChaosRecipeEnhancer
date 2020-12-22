
using System.Collections.Generic;


namespace EnhancePoE
{

    // names are from api
    public class Item
    {

        // poe json props
        public int w { get; set; }
        public int h { get; set; }
        public string typeLine { get; set; }
        public string name { get; set; }
        public bool identified { get; set; }
        public int ilvl { get; set; }
        public int frameType { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public Influence influences { get; set; }
        public string icon { get; set; }

        // own prop
        public string ItemType { get; set; }
        public int StashTabIndex { get; set; }
    }

    public class ItemList
    {
        public List<Item> items { get; set; }
        public bool quadLayout { get; set; } = false;
    }

    public class Influence
    {
        public bool shaper { get; set; } = false;
        public bool elder { get; set; } = false;
        public bool crusader { get; set; } = false;
        public bool redeemer { get; set; } = false;
        public bool hunter { get; set; } = false;
        public bool warlord { get; set; } = false;
    }
}
