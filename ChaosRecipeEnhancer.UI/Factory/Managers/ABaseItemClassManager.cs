using System.Collections.Generic;
using ChaosRecipeEnhancer.UI.Model;

namespace ChaosRecipeEnhancer.UI.Factory.Managers
{
    public abstract class ABaseItemClassManager
    {
        #region Properties

        public string ClassName { get; set; }
        public string ClassFilterName { get; set; }
        public string ClassColor { get; set; }
        public bool AlwaysActive { get; set; }

        #endregion

        #region Methods

        public virtual string SetBaseType()
        {
            var baseType = "Class " + ClassFilterName;
            return baseType;
        }

        // TODO: [Remove] I don't think we need this
        public string SetSocketRules(string result)
        {
            return result;
        }

        public virtual bool CheckIfMissing(HashSet<string> missingItemClasses)
        {
            return missingItemClasses.Contains(ClassName);
        }

        public abstract ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue);

        #endregion
    }
}