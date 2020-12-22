using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhancePoE.Model
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
