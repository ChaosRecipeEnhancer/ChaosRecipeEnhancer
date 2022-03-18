using EnhancePoE.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EnhancePoE.Visitors
{
    //add interfaces for everything
    public abstract class CBaseItemClassManager
    {
        //make a class for these properties
        public abstract string ClassName { get; set; }
        public abstract bool AlwaysActive { get; set; }
        public abstract string ClassColor { get; set; }
        public abstract string ClassFilterName { get; set; }

        public virtual string SetBaseType()
        {
            var baseType = "Class " + ClassFilterName;
            return baseType;
        }
        public virtual string SetSocketRules(string result)
        {
            return result;
        }
        public abstract ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue);
    }
}
