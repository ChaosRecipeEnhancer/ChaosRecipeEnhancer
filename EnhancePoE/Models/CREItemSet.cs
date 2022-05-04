using System;
using System.Collections.Generic;
using System.Linq;

namespace EnhancePoE.Models
{
    /// <summary>
    /// Used to track complete item sets for currency turn-in
    /// </summary>
    public class CREItemSet
    {
        #region Properties

        public List<CREItemWrapper> ItemList { get; set; } = new List<CREItemWrapper>();

        // TODO: [Refactor] These should probably be enums
        public List<string> EmptyItemSlots { get; set; } = new List<string> { "BodyArmours", "TwoHandWeapons", "OneHandWeapons", "OneHandWeapons", "Helmets", "Gloves", "Boots", "Belts", "Rings", "Rings", "Amulets" };

        // TODO: [Refactor] This should (maybe?) be renamed to "CanProduceCurrency" or something
        public bool SetCanProduceChaos { get; set; }
        public List<int> CurrentPosition { get; set; } = new List<int> { 0, 0, 0 };
        public string InfluenceType { get; set; }

        #endregion

        #region Methods

        public bool AddItem(CREItemWrapper creItemWrapper)
        {
            if (EmptyItemSlots.Contains(creItemWrapper.ItemType))
            {
                if (creItemWrapper.ItemType == "OneHandWeapons")
                {
                    EmptyItemSlots.Remove("TwoHandWeapons");
                }
                else if (creItemWrapper.ItemType == "TwoHandWeapons")
                {
                    EmptyItemSlots.Remove("OneHandWeapons");
                    EmptyItemSlots.Remove("OneHandWeapons");
                }

                // TODO: [Refactor] Magic number for item level floor for Chaos Recipe
                if (creItemWrapper.Item.ItemLevel <= 74) SetCanProduceChaos = true;

                EmptyItemSlots.Remove(creItemWrapper.ItemType);
                ItemList.Add(creItemWrapper);

                CurrentPosition[0] = creItemWrapper.Item.X;
                CurrentPosition[1] = creItemWrapper.Item.Y;
                CurrentPosition[2] = creItemWrapper.StashTabIndex;

                return true;
            }

            return false;
        }

        public void OrderItems()
        {
            var orderedClasses = new List<string> { "BodyArmours", "TwoHandWeapons", "OneHandWeapons", "OneHandWeapons", "Helmets", "Gloves", "Boots", "Belts", "Rings", "Rings", "Amulets" };
            ItemList = ItemList.OrderBy(d => orderedClasses.IndexOf(d.ItemType)).ToList();
        }

        public double GetItemDistance(CREItemWrapper creItemWrapper)
        {
            if (creItemWrapper.StashTabIndex != CurrentPosition[2]) return 40;

            return Math.Sqrt(Math.Pow(creItemWrapper.Item.X - CurrentPosition[0], 2) + Math.Pow(creItemWrapper.Item.Y - CurrentPosition[1], 2));
        }

        public bool IsValidItem(CREItemWrapper creItemWrapper)
        {
            return EmptyItemSlots.Contains(creItemWrapper.ItemType);
        }

        public string GetNextItemClass()
        {
            return EmptyItemSlots[0];
        }

        #endregion
    }
}
