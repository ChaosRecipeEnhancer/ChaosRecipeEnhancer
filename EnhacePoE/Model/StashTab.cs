using EnhancePoE.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
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
        public int GlovesAmount { get; set; } = 0;
        public int HelmetAmount { get; set; } = 0;
        public int BootsAmount { get; set; } = 0;
        public int ChestAmount { get; set; } = 0;
        public int WeaponAmount { get; set; } = 0;
        public int RingAmount { get; set; } = 0;
        public int AmuletAmount { get; set; } = 0;
        public int BeltAmount { get; set; } = 0;
        public int FullSets { get; set; } = 0;
        public ObservableCollection<Cell> OverlayCellsList { get; set; } = new ObservableCollection<Cell>();
        public List<Item> ItemOrderList { get; set; } = new List<Item>();

        // used for registering clicks on tab headers
        public TextBlock TabHeader { get; set; }



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

        public StashTab(string name, bool quad, int number, int index)
        {
            this.TabName = name;
            this.Quad = quad;
            this.TabIndex = index;
            this.TabNumber = number;
            this.StashTabItem = new TabItem();
            this._stashTabItem.Header = this._tabName;
            this._stashTabItem.Content = this._stashControl;
            this._stashTabItem.DataContext = this;
            //this.TabHeader = new TextBlock() { Text = name, Padding = new Thickness(5, 2, 5, 2) };
            TabHeaderColor = Brushes.Transparent;

        }

        public void GetFullSets()
        {
            int rings = this.RingAmount / 2;
            int weapons = this.WeaponAmount / 2;
            int sets = new[] { rings, weapons, this.HelmetAmount, this.BootsAmount, this.GlovesAmount, this.ChestAmount, this.AmuletAmount, this.BeltAmount }.Min();
            this.FullSets = sets;
            //if (FullSets > 0)
            //{
            //    if (Properties.Settings.Default.ColorStash != "")
            //    {
            //        this.TabHeader.Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorStash));
            //    }
            //    else
            //    {
            //        this.TabHeader.Background = Brushes.Red;
            //    }
            //}
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
                        ToggleCellCommand = new RelayCommand<bool>(ChaosRecipeEnhancer.currentData.ActivateNextCell),
                        //ButtonName = "Btn " + j + " " + i,
                        //CellName = "Cell " + j + " " + i
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
            //GenerateItemOrderList();
            //th = new Thread(GenerateItemOrderList);
            //th.Start();
            //GenerateItemOrderList();
            //Trace.WriteLine(OverlayCellsList.Count, "overlaycelllist count");
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

            return null;
        }

        public void RemoveQualityFromItems()
        {
            foreach(Item i in this.ItemList)
            {
                //Trace.WriteLine(i.typeLine);
                //Trace.WriteLine(i.typeLine);
                if (i.typeLine.StartsWith("Superior"))
                {
                    //Trace.WriteLine(i.typeLine);
                    i.typeLine = i.typeLine.Replace("Superior ", "");
                    //i.typeLine.Trim();
                }
                //Trace.WriteLine(i.typeLine);
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
