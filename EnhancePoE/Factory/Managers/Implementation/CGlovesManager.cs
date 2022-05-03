using EnhancePoE.UI.Properties;
using EnhancePoE.UI.Model;

namespace EnhancePoE.UI.Visitors
{
    internal class CGlovesManager : CBaseItemClassManager
    {
        public CGlovesManager()
        {
            ClassName = "Gloves";
            ClassFilterName = "\"Gloves\"";
            ClassColor = Settings.Default.ColorGloves;
            AlwaysActive = Settings.Default.GlovesAlwaysActive;
        }
        public override string ClassName { get; set; }
        public override string ClassColor { get; set; }
        public override string ClassFilterName { get; set; }
        public override bool AlwaysActive { get; set; }
        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.GlovesActive = newValue;
            return activeItems;
        }
    }
}