using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace EnhancePoE.DataModels.GGGModels
{
    // TODO: [Feature] We don't use all the props, but we could for detection of tab types etc.
    public class StashTabProps
    {
        [JsonPropertyName("n")]
        public string Name { get; set; }

        [JsonPropertyName("i")]
        public int Index { get; set; }

        [JsonPropertyName("parent")]
        public string Parent { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        public StashTabMetadata Metadata { get; set; }
    }

    public class StashTabMetadata
    {
        [JsonPropertyName("public")]
        public bool? IsPublic { get; set; }

        [JsonPropertyName("folder")]
        public bool? IsFolder { get; set; }
    }

    // TODO: [Remove] Why not just initialize a list of StashTabProps in the associated code?
    public class StashTabPropsList
    {
        public List<StashTabProps> tabs { get; set; }
    }
}