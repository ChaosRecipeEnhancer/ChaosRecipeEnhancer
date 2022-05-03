using System.Collections.Generic;

namespace EnhancePoE.UI.Model
{
    // property names from api
    public class StashTabProps
    {
        public string n { get; set; }
        public int i { get; set; }
    }

    public class StashTabPropsList
    {
        public List<StashTabProps> tabs { get; set; }
    }
}