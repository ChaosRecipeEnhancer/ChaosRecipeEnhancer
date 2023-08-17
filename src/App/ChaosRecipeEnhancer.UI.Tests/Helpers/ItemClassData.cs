using System.Collections;
using ChaosRecipeEnhancer.UI.Constants;

namespace ChaosRecipeEnhancer.UI.Tests.Helpers
{
    public class ItemClassData : IEnumerable<object[]>
    {
        private static readonly (string, string)[] ItemData =
        {
            (ItemIconConstants.HelmetUrl, GameTerminology.Helmets),
            (ItemIconConstants.BowUrl, GameTerminology.TwoHandWeapons),
            (ItemIconConstants.ClawUrl, GameTerminology.OneHandWeapons),
            (ItemIconConstants.DaggerUrl, GameTerminology.OneHandWeapons),
            (ItemIconConstants.RuneDaggerUrl, GameTerminology.OneHandWeapons),
            (ItemIconConstants.OneHandedAxeUrl, GameTerminology.OneHandWeapons),
            (ItemIconConstants.OneHandedSwordUrl, GameTerminology.OneHandWeapons),
            (ItemIconConstants.OneHandedMaceUrl, GameTerminology.OneHandWeapons),
            (ItemIconConstants.ScepterUrl, GameTerminology.OneHandWeapons),
            (ItemIconConstants.StaffUrl, GameTerminology.TwoHandWeapons),
            (ItemIconConstants.WarstaveUrl, GameTerminology.TwoHandWeapons),
            (ItemIconConstants.TwoHandedAxeUrl, GameTerminology.TwoHandWeapons),
            (ItemIconConstants.TwoHandedMaceUrl, GameTerminology.TwoHandWeapons),
            (ItemIconConstants.TwoHandedSwordUrl, GameTerminology.TwoHandWeapons),
            (ItemIconConstants.WandUrl, GameTerminology.OneHandWeapons),
            (ItemIconConstants.FishingRodUrl, GameTerminology.TwoHandWeapons),
            (ItemIconConstants.BodyArmourUrl, GameTerminology.BodyArmors),
            (ItemIconConstants.BootsUrl, GameTerminology.Boots),
            (ItemIconConstants.GlovesUrl, GameTerminology.Gloves),
            (ItemIconConstants.AmuletUrl, GameTerminology.Amulets),
            (ItemIconConstants.BeltUrl, GameTerminology.Belts),
            (ItemIconConstants.RingUrl, GameTerminology.Rings)
        };

        public IEnumerator<object[]> GetEnumerator()
        {
            foreach (var (itemIcon, terminology) in ItemData)
            {
                yield return new object[] { itemIcon, terminology };
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}