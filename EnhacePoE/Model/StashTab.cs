using EnhancePoE.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
//using System.Windows.Input;

namespace EnhancePoE.Model
{
    public class StashTab : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public Uri StashTabUri { get; set; }
        public List<Item> ItemList { get; set; }
        public List<Item> ItemListShaper { get; set; } = new List<Item>();
        public List<Item> ItemListElder { get; set; } = new List<Item>();
        public List<Item> ItemListWarlord { get; set; } = new List<Item>();
        public List<Item> ItemListCrusader { get; set; } = new List<Item>();
        public List<Item> ItemListHunter { get; set; } = new List<Item>();
        public List<Item> ItemListRedeemer { get; set; } = new List<Item>();

        public ObservableCollection<Cell> OverlayCellsList { get; set; } = new ObservableCollection<Cell>();

        // used for registering clicks on tab headers
        public TextBlock TabHeader { get; set; }

        public string TabName { get; set; }
        public bool Quad { get; set; }
        public int TabIndex { get; set; }






        private SolidColorBrush _tabHeaderColor;
        public SolidColorBrush TabHeaderColor
        {
            get
            {
                return _tabHeaderColor;
            }
            set
            {
                _tabHeaderColor = value;
                OnPropertyChanged(nameof(TabHeaderColor));
            }
        }




        public StashTab(string name, int index)
        {
            this.TabName = name;
            this.TabIndex = index;
            TabHeaderColor = Brushes.Transparent;
        }

        private void Generate2dArr(int size)
        { 
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    OverlayCellsList.Add(new Cell
                    {
                        Active = false,
                        XIndex = j,
                        YIndex = i
                    });
                }
            }
        }

        public void PrepareOverlayList()
        {
            int size;
            if (this.Quad)
            {
                size = 24;
            }
            else
            {
                size = 12;
            }
            Generate2dArr(size);
        }
        public static string GetItemClass(Item item)
        {
            List<string> iconParts = new List<string>(item.icon.Split('/'));
            String itemClass = iconParts[6];
            switch (itemClass)
            {
                case "Weapons":
                case "Armours":
                    itemClass = iconParts[7];
                    break;
                case "Rings":
                case "Amulets":
                case "Belts":
                    break;
                default:
                    return null;
            }
            return itemClass;
        }

        public void CleanItemList()
        {

            // for loop backwards for deleting from list 
            for (int i = ItemList.Count - 1; i > -1; i--)
            {
                if (ItemList[i].identified == true)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }

                //exalted recipe every ilvl allowed, same bases, sort in itemlists
                if (Properties.Settings.Default.ExaltedRecipe)
                {
                    ItemListShaper.Clear();
                    ItemListElder.Clear();
                    ItemListCrusader.Clear();
                    ItemListWarlord.Clear();
                    ItemListHunter.Clear();
                    ItemListRedeemer.Clear();

                    if (ItemList[i].influences != null)
                    {
                        if (ItemList[i].frameType == 2)
                        {
                            string result = GetItemClass(ItemList[i]);
                            if (result != null)
                            {
                                ItemList[i].ItemType = result;
                                if (ItemList[i].influences.shaper) { ItemListShaper.Add(ItemList[i]); }
                                else if (ItemList[i].influences.elder) { ItemListElder.Add(ItemList[i]); }
                                else if (ItemList[i].influences.warlord) { ItemListWarlord.Add(ItemList[i]); }
                                else if (ItemList[i].influences.crusader) { ItemListCrusader.Add(ItemList[i]); }
                                else if (ItemList[i].influences.hunter) { ItemListHunter.Add(ItemList[i]); }
                                else if (ItemList[i].influences.redeemer) { ItemListRedeemer.Add(ItemList[i]); }
                            }
                            ItemList.RemoveAt(i);
                            continue;
                        }
                    }
                }

                if (ItemList[i].ilvl < 60)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }
                if (ItemList[i].ilvl > 74)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }
                if (ItemList[i].frameType == 2)
                {
                    string result = GetItemClass(ItemList[i]);
                    if (result != null)
                    {
                        ItemList[i].ItemType = result;
                    }
                    else
                    {
                        ItemList.RemoveAt(i);
                    }
                }
            }
        }

        public void ActivateItemCells(Item item)
        {
            List<List<int>> AllCoordinates = new List<List<int>>();

            for (int i = 0; i < item.w; i++)
            {
                for (int j = 0; j < item.h; j++)
                {
                    AllCoordinates.Add(new List<int> { item.x + i, item.y + j });
                }
            }

            foreach(Cell cell in OverlayCellsList)
            {
                cell.Active = false;
                foreach(List<int> coordinate in AllCoordinates)
                {
                    if(coordinate[0] == cell.XIndex && coordinate[1] == cell.YIndex)
                    {
                        cell.Active = true;
                    }
                }
            }
        }
    }
}
