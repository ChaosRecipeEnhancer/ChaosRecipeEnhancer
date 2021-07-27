using EnhancePoE.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Windows;
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
        public List<Item> ItemListChaos { get; set; } = new List<Item>();
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

        private Thickness _tabHeaderWidth;
        public Thickness TabHeaderWidth
        {
            get { return _tabHeaderWidth; }
            set
            {
                if (value != _tabHeaderWidth)
                {
                    _tabHeaderWidth = value;
                    OnPropertyChanged("TabHeaderWidth");
                }
            }
        }




        public StashTab(string name, int index)
        {
            this.TabName = name;
            this.TabIndex = index;
            TabHeaderColor = Brushes.Transparent;
            TabHeaderWidth = new Thickness(Properties.Settings.Default.TabHeaderWidth, 2, Properties.Settings.Default.TabHeaderWidth, 2);
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
        //private static string GetItemClass(Item item, Dictionary<string, string> mappingContentDict)
        //{
        //    List<string> iconParts = new List<string>(item.icon.Split('/'));
        //    String lastPart = iconParts[iconParts.Count - 1];

        //    foreach (var itemMapping in mappingContentDict)
        //    {
        //        if (lastPart.ToLower().Contains(itemMapping.Key))
        //        {
        //            return itemMapping.Value;
        //        }
        //    }

        //    MainWindow.instance.addItemLog(lastPart, item.icon);
        //    return null;
        //}

        //private static string GetItemClass(Item item)
        //{
        //    //https://web.poecdn.com/gen/image/WzI1LDE0LHsiZiI6IjJESXRlbXMvQXJtb3Vycy9IZWxtZXRzL0hlbG1ldFN0ckRleDciLCJ3IjoyLCJoIjoyLCJzY2FsZSI6MX1d/0884b27765/HelmetStrDex7.png
        //    var urlParts = item.icon.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
        //    string encodedPart = urlParts[4];
        //    while (encodedPart.Length % 4 != 0)
        //    {
        //        encodedPart += "=";
        //    }
        //    string decodedItemData = Encoding.UTF8.GetString(Convert.FromBase64String(encodedPart));
        //    var iconParts = decodedItemData.Split('/');
        //    String itemClass = iconParts[1];
        //    switch (itemClass)
        //    {
        //        case "Weapons":
        //        case "Armours":
        //            itemClass = iconParts[2];
        //            break;
        //        case "Rings":
        //        case "Amulets":
        //        case "Belts":
        //            break;
        //        default:
        //            return null;
        //    }
        //    //Trace.WriteLine("item classe ", itemClass);
        //    return itemClass;
        //}

        public void CleanItemList()
        {
            if (Properties.Settings.Default.ExaltedRecipe)
            {
                ItemListShaper.Clear();
                ItemListElder.Clear();
                ItemListCrusader.Clear();
                ItemListWarlord.Clear();
                ItemListHunter.Clear();
                ItemListRedeemer.Clear();
            }

            // for loop backwards for deleting from list 
            for (int i = ItemList.Count - 1; i > -1; i--)
            {
                if (ItemList[i].identified == true && !Properties.Settings.Default.IncludeIdentified)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }
                ItemList[i].GetItemClass();
                ItemList[i].StashTabIndex = this.TabIndex;
                //exalted recipe every ilvl allowed, same bases, sort in itemlists
                if (Properties.Settings.Default.ExaltedRecipe)
                {
                    if (ItemList[i].influences != null)
                    {
                        if (ItemList[i].frameType == 2)
                        {
                            //string result = GetItemClass(ItemList[i], mappingContentDict);
                            //string result = GetItemClass(ItemList[i]);
                            if (ItemList[i].ItemType != null)
                            {
                                //ItemList[i].ItemType = result;
                                if (ItemList[i].influences.shaper) { ItemListShaper.Add(ItemList[i]); }
                                else if (ItemList[i].influences.elder) { ItemListElder.Add(ItemList[i]); }
                                else if (ItemList[i].influences.warlord) { ItemListWarlord.Add(ItemList[i]); }
                                else if (ItemList[i].influences.crusader) { ItemListCrusader.Add(ItemList[i]); }
                                else if (ItemList[i].influences.hunter) 
                                {
                                    //Trace.WriteLine("found item");
                                    ItemListHunter.Add(ItemList[i]); 
                                }
                                else if (ItemList[i].influences.redeemer) { ItemListRedeemer.Add(ItemList[i]); }
                            }
                            ItemList.RemoveAt(i);
                            continue;
                        }
                    }
                }
                if (!Properties.Settings.Default.ChaosRecipe && !Properties.Settings.Default.RegalRecipe)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }
                if (ItemList[i].ilvl < 60)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }
                if (Properties.Settings.Default.RegalRecipe && ItemList[i].ilvl < 75)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }
                if (ItemList[i].frameType == 2)
                {
                    if(ItemList[i].ItemType == null)
                    {
                        ItemList.RemoveAt(i);
                        continue;
                    }
                }
                if (ItemList[i].ilvl <= 74)
                {
                    ItemListChaos.Add(ItemList[i]);
                    ItemList.RemoveAt(i);
                }
            }
        }

        public void DeactivateItemCells()
        {
            foreach(Cell cell in OverlayCellsList)
            {
                cell.Active = false;
            }
        }

        public void DeactivateSingleItemCells(Item item)
        {
            List<List<int>> AllCoordinates = new List<List<int>>();

            for (int i = 0; i < item.w; i++)
            {
                for (int j = 0; j < item.h; j++)
                {
                    AllCoordinates.Add(new List<int> { item.x + i, item.y + j });
                }
            }

            foreach (Cell cell in OverlayCellsList)
            {
                foreach (List<int> coordinate in AllCoordinates)
                {
                    if (coordinate[0] == cell.XIndex && coordinate[1] == cell.YIndex)
                    {
                        cell.Active = false;
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
                foreach(List<int> coordinate in AllCoordinates)
                {
                    if(coordinate[0] == cell.XIndex && coordinate[1] == cell.YIndex)
                    {
                        cell.Active = true;
                        cell.CellItem = item;
                        cell.TabIndex = this.TabIndex;
                    }
                }
            }
        }

        public void MarkNextItem(Item item)
        {
            foreach(Cell cell in OverlayCellsList)
            {
                if(cell.CellItem == item)
                {
                    cell.ButtonName = "X";
                }
            }
        }

        public void ShowNumbersOnActiveCells(int index)
        {
            index++;
            foreach(Cell cell in OverlayCellsList)
            {
                if (cell.Active)
                {
                    cell.ButtonName = index.ToString();
                }
            }
        }
    }
}
