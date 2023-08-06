namespace ChaosRecipeEnhancer.UI.WPF.BusinessLogic.Items;

public class ActiveItemTypes
{
	public ActiveItemTypes()
	{
	}

	public ActiveItemTypes(ActiveItemTypes activeItems)
	{
		BootsActive = activeItems.BootsActive;
		GlovesActive = activeItems.GlovesActive;
		HelmetActive = activeItems.HelmetActive;
		WeaponActive = activeItems.WeaponActive;
		ChestActive = activeItems.ChestActive;
		RingActive = activeItems.RingActive;
		AmuletActive = activeItems.AmuletActive;
		BeltActive = activeItems.BeltActive;
	}

	public bool GlovesActive { get; set; } = true;
	public bool HelmetActive { get; set; } = true;
	public bool BootsActive { get; set; } = true;
	public bool ChestActive { get; set; } = true;
	public bool WeaponActive { get; set; } = true;
	public bool RingActive { get; set; } = true;
	public bool AmuletActive { get; set; } = true;
	public bool BeltActive { get; set; } = true;
}