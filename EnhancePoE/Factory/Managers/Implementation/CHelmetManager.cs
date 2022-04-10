using EnhancePoE.Properties;
using EnhancePoE.Model;

namespace EnhancePoE.Visitors
{
    internal class CHelmetManager : CBaseItemClassManager
    {
        public CHelmetManager()
        {
            ClassName = "Helmets";
            ClassFilterName = "\"Helmets\"";
            ClassColor = Settings.Default.ColorHelmet;
            AlwaysActive = Settings.Default.HelmetsAlwaysActive;            
        }
        public override string ClassName { get; set; }
        public override string ClassColor { get; set; }
        public override string ClassFilterName { get; set; }
        public override bool AlwaysActive { get; set; }
        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.HelmetActive = newValue;
            return activeItems;
        }
    }
}