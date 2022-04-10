using EnhancePoE.Properties;
using EnhancePoE.Model;

namespace EnhancePoE.Visitors
{
    internal class CBootsManager : CBaseItemClassManager
    {
        public CBootsManager()
        {
            ClassName = "Boots";
            ClassFilterName = "\"Boots\"";
            ClassColor = Settings.Default.ColorBoots;
            AlwaysActive = Settings.Default.BootsAlwaysActive;            
        }
        public override string ClassName { get; set; }
        public override string ClassColor { get; set; }
        public override string ClassFilterName { get; set; }
        public override bool AlwaysActive { get; set; }
        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.BootsActive = newValue;
            return activeItems;
        }
    }
}