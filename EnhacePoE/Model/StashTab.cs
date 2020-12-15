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
        //public List<Item> ItemOrderList { get; set; } = new List<Item>();

        // used for registering clicks on tab headers
        public TextBlock TabHeader { get; set; }
        public double TabHeaderWidth { get; set; } = 22;

        private string _tabName;
        public string TabName { 
            get
            {
                return _tabName;
            }
            set
            {
                if (value == _tabName)
                    return;

                _tabName = value;
                OnPropertyChanged("TabName");
            }
        }

        private bool _quad;
        public bool Quad
        {
            get
            {
                return _quad;
            }
            set
            {
                _quad = value;
                OnPropertyChanged(nameof(Quad));
            }
        }

        private int _tabNumber;
        public int TabNumber
        {
            get
            {
                return _tabNumber;
            }
            set
            {
                _tabNumber = value;
                OnPropertyChanged(nameof(TabNumber));
            }
        }

        private int _tabIndex;
        public int TabIndex
        {
            get
            {
                return _tabIndex;
            }
            set
            {
                _tabIndex = value;
                OnPropertyChanged(nameof(TabIndex));
            }
        }

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

        public StashTabControl _stashControl { get; set; } = new StashTabControl();

        private TabItem _stashTabItem;
        public TabItem StashTabItem
        {
            get { return _stashTabItem; }
            set
            {
                _stashTabItem = value;
                OnPropertyChanged(nameof(StashTabItem));
            }
        }


        public StashTab(string name)
        {
            this._tabName = name;
            this._stashTabItem = new TabItem();
            this._stashTabItem.Header = this._tabName;
            this._stashTabItem.Content = this._stashControl;
            this._stashTabItem.DataContext = this;
            //this.TabHeader = new TextBlock() { Text = this.TabName, Padding = new Thickness(5, 2, 5, 2) };
            TabHeaderColor = Brushes.Transparent;
        }

        public StashTab(string name, bool quad, int number, int index, double width)
        {
            this.TabName = name;
            this.Quad = quad;
            this.TabIndex = index;
            this.TabNumber = number;
            this.StashTabItem = new TabItem();
            this._stashTabItem.Header = this._tabName;
            this._stashTabItem.Content = this._stashControl;
            this._stashTabItem.DataContext = this;
            this.TabHeaderWidth = width;
            //this.TabHeader = new TextBlock() { Text = name, Padding = new Thickness(5, 2, 5, 2) };
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
                        YIndex = i,
                        //ToggleCellCommand = new RelayCommand<bool>(ChaosRecipeEnhancer.currentData.ActivateNextCell),
                    });
                }
            }
        }

        public void PrepareOverlayList()
        {
            //GetFullSets();
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

        private string CheckBase(Item item)
        {
            foreach (string ringBase in ChaosRecipeEnhancer.currentData.RingBases)
            {
                if (item.typeLine == ringBase)
                {
                    return "ring";
                }
            }

            foreach (string amuletBase in ChaosRecipeEnhancer.currentData.AmuletBases)
            {
                if (item.typeLine == amuletBase)
                {
                    return "amulet";
                }
            }

            foreach (string beltBase in ChaosRecipeEnhancer.currentData.BeltBases)
            {
                if (item.typeLine == beltBase)
                {
                    return "belt";
                }
            }

            foreach (string bootBase in ChaosRecipeEnhancer.currentData.BootsBases)
            {
                if (item.typeLine == bootBase)
                {
                    return "boots";
                }
            }

            foreach (string gloveBase in ChaosRecipeEnhancer.currentData.GlovesBases)
            {
                if (item.typeLine == gloveBase)
                {
                    return "gloves";
                }
            }

            foreach (string helmetBase in ChaosRecipeEnhancer.currentData.HelmetBases)
            {
                if (item.typeLine == helmetBase)
                {
                    return "helmet";
                }
            }

            foreach (string chestBase in ChaosRecipeEnhancer.currentData.ChestBases)
            {
                if (item.typeLine == chestBase)
                {
                    return "chest";
                }
            }

            foreach (string weaponBase in ChaosRecipeEnhancer.currentData.WeaponBases)
            {
                if (item.typeLine == weaponBase)
                {
                    return "weapon";
                }
            }

            if (Properties.Settings.Default.TwoHand)
            {
                foreach (string twohandBase in ChaosRecipeEnhancer.currentData.TwoHandBases)
                {
                    if (item.typeLine == twohandBase)
                    {
                        return "twohand";
                    }
                }
            }

            return null;
        }

        public void RemoveQualityFromItems()
        {
            foreach(Item i in this.ItemList)
            {
                if (i.typeLine.StartsWith("Superior"))
                {
                    i.typeLine = i.typeLine.Replace("Superior ", "");
                }
            }
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
                            string result = CheckBase(ItemList[i]);
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
                    string result = CheckBase(ItemList[i]);
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
