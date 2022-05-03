namespace EnhancePoE.Model
{
    public class ActiveItemTypes
    {
        public ActiveItemTypes()
        {

        }
        public ActiveItemTypes(ActiveItemTypes activeItems)
        {
            this.BootsActive = activeItems.BootsActive;
            this.GlovesActive = activeItems.GlovesActive;
            this.HelmetActive = activeItems.HelmetActive;
            this.WeaponActive = activeItems.WeaponActive;
            this.ChestActive = activeItems.ChestActive;
            this.RingActive = activeItems.RingActive;
            this.AmuletActive = activeItems.AmuletActive;
            this.BeltActive = activeItems.BeltActive;
        }
        public bool GlovesActive { get; set; } = true;
        public bool HelmetActive { get; set; } = true;
        public bool BootsActive { get; set; } = true;
        public bool ChestActive { get; set; } = true;
        public bool WeaponActive { get; set; } = true;
        public bool RingActive { get; set; } = true;
        public bool AmuletActive { get; set; } = true;
        public bool BeltActive { get; set; } = true;
        public bool ChaosMissing { get; set; } = true;
    }
}
