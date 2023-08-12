using System.Collections;

namespace ChaosRecipeEnhancer.UI.Tests.Helpers;

public class ItemClassData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[]
        {
            ItemIconConstants.HelmetUrl,
            "Helmets"
        };
        yield return new object[]
        {
            ItemIconConstants.BowUrl,
            "TwoHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.ClawUrl,
            "OneHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.DaggerUrl,
            "OneHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.RuneDaggerUrl,
            "OneHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.OneHandedAxeUrl,
            "OneHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.OneHandedSwordUrl,
            "OneHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.OneHandedMaceUrl,
            "OneHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.ScepterUrl,
            "OneHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.StaffUrl,
            "TwoHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.WarstaveUrl,
            "TwoHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.TwoHandedAxeUrl,
            "TwoHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.TwoHandedMaceUrl,
            "TwoHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.TwoHandedSwordUrl,
            "TwoHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.WandUrl,
            "OneHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.FishingRodUrl,
            "TwoHandWeapons"
        };
        yield return new object[]
        {
            ItemIconConstants.BodyArmourUrl,
            "BodyArmours"
        };
        yield return new object[]
        {
            ItemIconConstants.BootsUrl,
            "Boots"
        };
        yield return new object[]
        {
            ItemIconConstants.GlovesUrl,
            "Gloves"
        };
        yield return new object[]
        {
            ItemIconConstants.QuiverUrl,
            "Quivers"
        };
        yield return new object[]
        {
            ItemIconConstants.AmuletUrl,
            "Amulets"
        };
        yield return new object[]
        {
            ItemIconConstants.BeltUrl,
            "Belts"
        };
        yield return new object[]
        {
            ItemIconConstants.RingUrl,
            "Rings"
        };
    } 

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}