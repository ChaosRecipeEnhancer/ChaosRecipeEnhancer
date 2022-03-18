using EnhancePoE.Properties;

namespace EnhancePoE.Visitors
{
    public class CAmuletsManager : CBaseItemClassManager
    {
        public CAmuletsManager()
        {
            ClassName = "Amulets";
            ClassFilterName = "\"Amulets\"";
            ClassColor = Settings.Default.ColorAmulet;
            AlwaysActive = Settings.Default.AmuletsAlwaysActive;
        }      
        public override string ClassName { get; set; }
        public override string ClassColor { get; set; }
        public override string ClassFilterName { get; set; }
        public override bool AlwaysActive { get; set; }        
        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.AmuletActive = newValue;
            return activeItems;
        }
    }
}