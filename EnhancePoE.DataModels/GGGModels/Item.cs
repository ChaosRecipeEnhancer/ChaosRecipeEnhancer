using System.Collections.Generic;

namespace EnhancePoE.DataModels.GGGModels
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <seealso cref="https://www.pathofexile.com/developer/docs/reference#type-Item"/>
    public partial class Item
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string TypeLine { get; set; }
        public string BaseType { get; set; }
        public string Name { get; set; }
        public bool Identified { get; set; }
        public int ItemLevel { get; set; }
        public int FrameType { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public ItemInfluenceModel Influences { get; set; }
        public string Icon { get; set; }
        public List<ItemPropertyModel> Properties { get; set; }
    }
}