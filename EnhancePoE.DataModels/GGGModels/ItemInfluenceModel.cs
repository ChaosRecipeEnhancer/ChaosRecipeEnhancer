namespace EnhancePoE.DataModels.GGGModels
{
    /// <summary>
    /// TODO
    /// </summary>
    /// <seealso cref="https://www.pathofexile.com/developer/docs/reference#type-Item"/>
    public class ItemInfluenceModel
    {
        public bool Shaper { get; set; } = false;
        public bool Elder { get; set; } = false;
        public bool Crusader { get; set; } = false;
        public bool Redeemer { get; set; } = false;
        public bool Hunter { get; set; } = false;
        public bool Warlord { get; set; } = false;
    }
}
