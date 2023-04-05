using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ChaosRecipeEnhancer.UI.BusinessLogic.Items;
using ChaosRecipeEnhancer.UI.Properties;

namespace ChaosRecipeEnhancer.UI.DynamicControls.StashTabs
{
    /// <summary>
    ///     CRE-specific model of a stash tab within our app (NOT the GGG StashTab object model).
    /// </summary>
    public class StashTabControl : INotifyPropertyChanged
    {
        private SolidColorBrush _tabHeaderColor;
        private Thickness _tabHeaderWidth;

        public StashTabControl(string name, int index)
        {
            TabName = name;
            TabIndex = index;
            TabHeaderColor = Brushes.Transparent;
            TabHeaderWidth = new Thickness(Settings.Default.StashTabOverlayIndividualTabHeaderWidth, 2, Settings.Default.StashTabOverlayIndividualTabHeaderWidth, 2);
        }

        public int TabIndex { get; }
        public Uri StashTabApiRequestUrl { get; set; }
        public List<Item> ItemList { get; set; }
        public List<Item> ItemListChaos { get; } = new List<Item>();
        public List<Item> ItemListShaper { get; } = new List<Item>();
        public List<Item> ItemListElder { get; } = new List<Item>();
        public List<Item> ItemListWarlord { get; } = new List<Item>();
        public List<Item> ItemListCrusader { get; } = new List<Item>();
        public List<Item> ItemListHunter { get; } = new List<Item>();
        public List<Item> ItemListRedeemer { get; } = new List<Item>();

        public ObservableCollection<InteractiveStashCell> OverlayCellsList { get; } = new ObservableCollection<InteractiveStashCell>();

        // used for registering clicks on tab headers
        public TextBlock TabNameContainer { get; set; }
        public string TabName { get; set; }
        public bool Quad { get; set; }

        public SolidColorBrush TabHeaderColor
        {
            get => _tabHeaderColor;
            set
            {
                _tabHeaderColor = value;
                OnPropertyChanged("TabHeaderColor");
            }
        }

        public Thickness TabHeaderWidth
        {
            get => _tabHeaderWidth;
            set
            {
                if (value != _tabHeaderWidth)
                {
                    _tabHeaderWidth = value;
                    OnPropertyChanged("TabHeaderWidth");
                }
            }
        }

        /// <summary>
        /// Creates an N x N grid of interactable <see cref="InteractiveStashCell"/> objects. All objects are initialized to inactive.
        /// </summary>
        /// <param name="size">Represent the dimensions of our Cell object grid (Size = N)</param>
        private void GenerateInteractiveStashCellGrid(int size)
        {
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    OverlayCellsList.Add(new InteractiveStashCell
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
            // If quad tab, set grid to 24 x 24, else set to 12 x 12 grid
            var size = Quad ? 24 : 12;
            GenerateInteractiveStashCellGrid(size);
        }

        public void CleanItemList()
        {
            if (Settings.Default.ExaltedShardRecipeTrackingEnabled)
            {
                ItemListShaper.Clear();
                ItemListElder.Clear();
                ItemListCrusader.Clear();
                ItemListWarlord.Clear();
                ItemListHunter.Clear();
                ItemListRedeemer.Clear();
            }

            // for loop backwards for deleting from list 
            for (var i = ItemList.Count - 1; i > -1; i--)
            {
                if (ItemList[i].identified && !Settings.Default.IncludeIdentifiedItemsEnabled)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }

                if (ItemList[i].frameType != 2)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }

                ItemList[i].GetItemClass();
                if (ItemList[i].ItemType == null)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }

                ItemList[i].StashTabIndex = TabIndex;
                
                // Exalted recipe every ilvl allowed, same bases, sort in itemlists
                if (Settings.Default.ExaltedShardRecipeTrackingEnabled)
                {
                    if (ItemList[i].influences != null)
                    {
                        if (ItemList[i].influences.shaper)
                            ItemListShaper.Add(ItemList[i]);
                        else if (ItemList[i].influences.elder)
                            ItemListElder.Add(ItemList[i]);
                        else if (ItemList[i].influences.warlord)
                            ItemListWarlord.Add(ItemList[i]);
                        else if (ItemList[i].influences.crusader)
                            ItemListCrusader.Add(ItemList[i]);
                        else if (ItemList[i].influences.hunter)
                            ItemListHunter.Add(ItemList[i]);
                        else if (ItemList[i].influences.redeemer)
                            ItemListRedeemer.Add(ItemList[i]);

                        ItemList.RemoveAt(i);
                        continue;
                    }
                }
                    

                if (!Settings.Default.ChaosRecipeTrackingEnabled && !Settings.Default.RegalRecipeTrackingEnabled)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }

                if (ItemList[i].ilvl < 60)
                {
                    ItemList.RemoveAt(i);
                    continue;
                }

                if (Settings.Default.RegalRecipeTrackingEnabled && ItemList[i].ilvl < 75)
                {
                    ItemList.RemoveAt(i);
                    continue;
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
            foreach (var cell in OverlayCellsList) cell.Active = false;
        }

        public void DeactivateSingleItemCells(Item item)
        {
            // Initializing a list of tuples that represent our X,Y coordinates
            var allCoordinates = new List<(int X, int Y)>();

            // For a given in-game Item, populate a list of tuples that represent
            // their X,Y coordinates on our stash grid
            for (var i = 0; i < item.w; i++)
            {
                for (var j = 0; j < item.h; j++)
                {
                    allCoordinates.Add((item.x + i, item.y + j));
                }
            }

            foreach (var cell in OverlayCellsList)
            {
                foreach (var coordinate in allCoordinates)
                {
                    if (coordinate.X == cell.XIndex && coordinate.Y == cell.YIndex)
                        cell.Active = false;
                }
            }
        }

        public void ActivateItemCells(Item item)
        {
            // Initializing a list of tuples that represent our X,Y coordinates
            var allCoordinates = new List<(int X, int Y)>();

            // For a given in-game Item, populate a list of tuples that represent
            // their X,Y coordinates on our stash grid
            for (var i = 0; i < item.w; i++)
            {
                for (var j = 0; j < item.h; j++)
                {
                    allCoordinates.Add((item.x + i, item.y + j));
                }
            }

            foreach (var cell in OverlayCellsList)
            {
                foreach (var coordinate in allCoordinates)
                {
                    if (coordinate.X == cell.XIndex && coordinate.Y == cell.YIndex)
                    {
                        cell.Active = true;
                        cell.PathOfExileItemData = item;
                    }
                }
            }
        }

        public void MarkNextItem(Item item)
        {
            foreach (var cell in OverlayCellsList)
            {
                if (cell.PathOfExileItemData == item)
                {
                    cell.ButtonText = "X";
                }
            }
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}