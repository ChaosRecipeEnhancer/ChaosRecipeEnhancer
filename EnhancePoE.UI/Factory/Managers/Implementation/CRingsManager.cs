using EnhancePoE.UI.Properties;
using EnhancePoE.UI.Model;

namespace EnhancePoE.UI.Visitors
{
    internal class CRingsManager : CBaseItemClassManager
    {
        public CRingsManager()
        {
            ClassName = "Rings";
            ClassFilterName = "\"Rings\"";
            ClassColor = Settings.Default.ColorRing;
            AlwaysActive = Settings.Default.RingsAlwaysActive;
        }
        public override string ClassName { get; set; }
        public override string ClassColor { get; set; }
        public override string ClassFilterName { get; set; }
        public override bool AlwaysActive { get; set; }
        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.RingActive = newValue;
            return activeItems;
        }
    }
}