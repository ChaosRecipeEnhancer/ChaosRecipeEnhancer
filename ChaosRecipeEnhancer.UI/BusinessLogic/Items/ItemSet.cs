using System;
using System.Collections.Generic;
using System.Linq;

namespace ChaosRecipeEnhancer.UI.BusinessLogic.Items
{
    public class ItemSet
    {
        public List<ItemModel> ItemList { get; set; } = new List<ItemModel>();

        // We'll use the list to check which types we still needs to add to the set.
        // We'll need some logic to extract that from the item icon path but that should be possible
        // TODO These should probably be enums.
        public List<string> EmptyItemSlots { get; set; } = new List<string>
        {
            "BodyArmours", "TwoHandWeapons", "OneHandWeapons", "OneHandWeapons", "Helmets", "Gloves", "Boots", "Belts",
            "Rings", "Rings", "Amulets"
        };

        public bool SetCanProduceChaos { get; set; }
        public List<int> CurrentPosition { get; set; } = new List<int> { 0, 0, 0 };
        public string InfluenceType { get; set; }

        public bool AddItem(ItemModel itemModel)
        {
            if (EmptyItemSlots.Contains(itemModel.DerivedItemClass))
            {
                if (itemModel.DerivedItemClass == "OneHandWeapons")
                {
                    EmptyItemSlots.Remove("TwoHandWeapons");
                }
                else if (itemModel.DerivedItemClass == "TwoHandWeapons")
                {
                    EmptyItemSlots.Remove("OneHandWeapons");
                    EmptyItemSlots.Remove("OneHandWeapons");
                }

                if (itemModel.ItemLevel <= 74) SetCanProduceChaos = true;

                EmptyItemSlots.Remove(itemModel.DerivedItemClass);
                ItemList.Add(itemModel);
                CurrentPosition[0] = itemModel.X;
                CurrentPosition[1] = itemModel.Y;
                CurrentPosition[2] = itemModel.StashTabIndex;
                return true;
            }

            return false;
        }

        public void OrderItems()
        {
            var orderedClasses = new List<string>
            {
                "BodyArmours", "TwoHandWeapons", "OneHandWeapons", "OneHandWeapons", "Helmets", "Gloves", "Boots",
                "Belts", "Rings", "Rings", "Amulets"
            };
            ItemList = ItemList.OrderBy(d => orderedClasses.IndexOf(d.DerivedItemClass)).ToList();
        }

        public double GetItemDistance(ItemModel itemModel)
        {
            if (itemModel.StashTabIndex != CurrentPosition[2]) return 40;

            return Math.Sqrt(Math.Pow(itemModel.X - CurrentPosition[0], 2) + Math.Pow(itemModel.Y - CurrentPosition[1], 2));
        }

        public bool IsValidItem(ItemModel itemModel)
        {
            if (EmptyItemSlots.Contains(itemModel.DerivedItemClass)) return true;
            return false;
        }

        public string GetNextItemClass()
        {
            return EmptyItemSlots[0];
        }
    }
}