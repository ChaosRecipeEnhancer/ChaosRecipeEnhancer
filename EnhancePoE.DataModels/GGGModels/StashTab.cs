namespace EnhancePoE.DataModels.GGGModels
{
    public class StashTab
    {
        public string Parent { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
        public string Type { get; set; }
        public StashTabMetadata Metadata { get; set; }
    }

    public class StashTabMetadata
    {
        public bool? IsPublic { get; set; }
        public bool? IsFolder { get; set; }
    }
}