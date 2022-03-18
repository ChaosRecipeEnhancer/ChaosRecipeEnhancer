using EnhancePoE.Properties;

namespace EnhancePoE.Visitors
{
    internal class CBeltsManager : CBaseItemClassManager
    {
        public CBeltsManager()
        {
            ClassName = "Belts";
            ClassFilterName = "\"Belts\"";
            ClassColor = Settings.Default.ColorBelt;
            AlwaysActive = Settings.Default.BeltsAlwaysActive;
            
        }
        public override string ClassName { get; set; }
        public override string ClassColor { get; set; }
        public override string ClassFilterName { get; set; }
        public override bool AlwaysActive { get; set; }
        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.BeltActive = newValue;
            return activeItems;
        }
    }
}