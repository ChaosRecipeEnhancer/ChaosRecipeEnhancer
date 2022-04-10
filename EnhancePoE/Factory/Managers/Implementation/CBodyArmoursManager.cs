using EnhancePoE.Properties;
using EnhancePoE.Model;

namespace EnhancePoE.Visitors
{
    internal class CBodyArmoursManager : CBaseItemClassManager
    {
        public CBodyArmoursManager()
        {
            ClassName = "BodyArmours";
            ClassFilterName = "\"Body Armours\"";
            ClassColor = Settings.Default.ColorChest;
            AlwaysActive = Settings.Default.ChestsAlwaysActive;            
        }
        public override string ClassName { get; set; }
        public override string ClassColor { get; set; }
        public override string ClassFilterName { get; set; }
        public override bool AlwaysActive { get; set; }
        public override ActiveItemTypes SetActiveTypes(ActiveItemTypes activeItems, bool newValue)
        {
            activeItems.ChestActive = newValue;
            return activeItems;
        }
    }
}