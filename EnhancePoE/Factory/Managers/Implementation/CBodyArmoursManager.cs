using EnhancePoE.UI.Properties;
using EnhancePoE.UI.Model;

namespace EnhancePoE.UI.Visitors
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