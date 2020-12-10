using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using EnhancePoE.Model;


namespace EnhancePoE
{
    // TODO: rework to static class
    public class Data
    {
        //public List<List<Item>> Items { get; set; }

        public bool GlovesActive { get; set; } = true;
        public bool HelmetActive { get; set; } = true;
        public bool BootsActive { get; set; } = true;
        public bool ChestActive { get; set; } = true;
        public bool WeaponActive { get; set; } = true;

        public int OverallGlovesAmount { get; set; } = 0;
        public int OverallHelmetAmount { get; set; } = 0;
        public int OverallBootsAmount { get; set; } = 0;
        public int OverallChestAmount { get; set; } = 0;
        public int OverallWeaponAmount { get; set; } = 0;
        public int OverallRingAmount { get; set; } = 0;
        public int OverallAmuletAmount { get; set; } = 0;
        public int OverallBeltAmount { get; set; } = 0;

        public List<string> BootsBases { get; set; } = new List<string>();
        public List<string> GlovesBases { get; set; } = new List<string>();
        public List<string> HelmetBases { get; set; } = new List<string>();
        public List<string> ChestBases { get; set; } = new List<string>();
        public List<string> WeaponBases { get; set; } = new List<string>();
        public List<string> RingBases { get; set; } = new List<string>();
        public List<string> AmuletBases { get; set; } = new List<string>();
        public List<string> BeltBases { get; set; } = new List<string>();

        public int SetAmount { get; set; } = 0;
        public int SetTargetAmount { get; set; } = 0;


        public static List<ItemWithStash> GlobalItemOrderList { get; set; } = new List<ItemWithStash>();
        public static List<ItemWithStash> GlobalItemOrderListRest { get; set; } = new List<ItemWithStash>();

        private void InitializeBases()
        {




            string pathBoots = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\BootsBases.txt");
            string[] boots = File.ReadAllLines(pathBoots);
            foreach (string line in boots)
            {
                if (line == "") { continue; }
                this.BootsBases.Add(line.Trim());
            }

            string pathGloves = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\GlovesBases.txt");
            string[] gloves = File.ReadAllLines(pathGloves);
            foreach (string line in gloves)
            {
                if (line == "") { continue; }
                this.GlovesBases.Add(line.Trim());
            }

            string pathHelmet = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\HelmetBases.txt");
            string[] helmet = File.ReadAllLines(pathHelmet);
            foreach (string line in helmet)
            {
                if (line == "") { continue; }
                this.HelmetBases.Add(line.Trim());
            }

            string pathWeapon = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\WeaponSmallBases.txt");
            string[] weapon = File.ReadAllLines(pathWeapon);
            foreach (string line in weapon)
            {
                if (line == "") { continue; }
                this.WeaponBases.Add(line.Trim());
            }

            string pathChest = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\BodyArmourBases.txt");
            string[] chest = File.ReadAllLines(pathChest);
            foreach (string line in chest)
            {
                if (line == "") { continue; }
                this.ChestBases.Add(line.Trim());
            }

            string pathRing = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\RingBases.txt");
            string[] ring = File.ReadAllLines(pathRing);
            foreach (string line in ring)
            {
                if (line == "") { continue; }
                this.RingBases.Add(line.Trim());
            }

            string pathAmulet = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\AmuletBases.txt");
            string[] amulet = File.ReadAllLines(pathAmulet);
            foreach (string line in amulet)
            {
                if (line == "") { continue; }
                this.AmuletBases.Add(line.Trim());
            }

            string pathBelt = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"ChaosRecipeEnhancer\Bases\BeltBases.txt");
            string[] belt = File.ReadAllLines(pathBelt);
            foreach (string line in belt)
            {
                if (line == "") { continue; }
                this.BeltBases.Add(line.Trim());
            }
        }

        public Data()
        {
            InitializeBases();
        }

        public void CountTab()
        {
            SetTargetAmount = 0;
            OverallGlovesAmount = 0;
            OverallHelmetAmount = 0;
            OverallBootsAmount = 0;
            OverallChestAmount = 0;
            OverallWeaponAmount  = 0;
            OverallRingAmount = 0;
            OverallAmuletAmount = 0;
            OverallBeltAmount = 0;

            foreach(StashTab s in MainWindow.stashTabsModel.StashTabs)
            {
                GetSetTargetAmount(s);
                if(s.ItemList != null)
                {
                    foreach (Item item in s.ItemList)
                    {
                        if (item.ItemType == "ring")
                        {
                            s.RingAmount += 1;
                            this.OverallRingAmount += 1;
                        }
                        else if (item.ItemType == "amulet")
                        {
                            s.AmuletAmount += 1;
                            this.OverallAmuletAmount += 1;
                        }
                        else if (item.ItemType == "belt")
                        {
                            s.BeltAmount += 1;
                            this.OverallBeltAmount += 1;

                        }
                        else if (item.ItemType == "boots")
                        {
                            s.BootsAmount += 1;
                            this.OverallBootsAmount += 1;

                        }
                        else if (item.ItemType == "gloves")
                        {
                            s.GlovesAmount += 1;
                            this.OverallGlovesAmount += 1;

                        }
                        else if (item.ItemType == "chest")
                        {
                            s.ChestAmount += 1;
                            this.OverallChestAmount += 1;

                        }
                        else if (item.ItemType == "helmet")
                        {
                            s.HelmetAmount += 1;
                            this.OverallHelmetAmount += 1;

                        }
                        else if (item.ItemType == "weapon")
                        {
                            s.WeaponAmount += 1;
                            this.OverallWeaponAmount += 1;
                        }
                    } 
                }
                s.GetFullSets();
            }
        }

        public void GetSetTargetAmount(StashTab stash)
        {
            if(Properties.Settings.Default.Sets > 0)
            {
                this.SetTargetAmount = Properties.Settings.Default.Sets;
            }
            else
            {

                if (stash.Quad)
                {
                    this.SetTargetAmount += 16;
                }
                else
                {
                    this.SetTargetAmount += 4;
                }
            }
        }


        public void GetSetAmount()
        {
            int rings = OverallRingAmount / 2;
            int weapons = OverallWeaponAmount / 2;
            int sets = new[] { rings, weapons, OverallHelmetAmount, OverallBootsAmount, OverallGlovesAmount, OverallChestAmount, OverallAmuletAmount, OverallBeltAmount }.Min();
            SetAmount = sets;

            List<string> sectionList = new List<string>();
            bool filterActive = Properties.Settings.Default.LootfilterActive;


            if(weapons >= SetTargetAmount)
            {
                WeaponActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, WeaponBases, "weapon"));
                }
            }
            else
            {
                WeaponActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, WeaponBases, "weapon"));
                }
            }
            if(OverallChestAmount >= SetTargetAmount)
            {
                ChestActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, ChestBases, "chest"));

                }
            }
            else
            {
                ChestActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, ChestBases, "chest"));

                }
            }
            if (OverallBootsAmount >= SetTargetAmount)
            {
                BootsActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, BootsBases, "boots"));

                }
            }
            else
            {
                BootsActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, BootsBases, "boots"));

                }
            }
            if (OverallGlovesAmount >= SetTargetAmount)
            {
                GlovesActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, GlovesBases, "gloves"));

                }
            }
            else
            {
                GlovesActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, GlovesBases, "gloves"));

                }
            }
            if (OverallHelmetAmount >= SetTargetAmount)
            {
                HelmetActive = false;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(false, HelmetBases, "helmet"));

                }
            }
            else
            {
                HelmetActive = true;
                if (filterActive)
                {
                    sectionList.Add(FilterGeneration.GenerateSection(true, HelmetBases, "helmet"));

                }
            }

            MainWindow.overlay.Dispatcher.Invoke(() =>
            {
                MainWindow.overlay.FullSetsTextBlock.Text = sets.ToString();
            });

            if (filterActive)
            {
                sectionList.Add(FilterGeneration.GenerateSection(true, RingBases, "ring"));
                sectionList.Add(FilterGeneration.GenerateSection(true, AmuletBases, "amulet"));
                sectionList.Add(FilterGeneration.GenerateSection(true, BeltBases, "belt"));

                string oldFilter = FilterGeneration.OpenLootfilter();
                string newFilter = FilterGeneration.GenerateLootFilter(oldFilter, sectionList);
                FilterGeneration.WriteLootfilter(newFilter);
            }
            //Trace.WriteLine(sets, "set min amount");
        }

        public void CheckActives()
        {
            CountTab();
            GetSetAmount();
        }


        // TODO: reuse this code in count methods
        private bool HasFullSet(List<ItemWithStash> itemList)
        {
            int ringsAmount = 0;
            int weaponsAmount = 0;
            int helmetAmount = 0;
            int bootsAmount = 0;
            int glovesAmount = 0;
            int chestAmount = 0;
            int amuletAmount = 0;
            int beltAmount = 0;

            foreach (ItemWithStash item in itemList)
            {
                if (item.ItemOfStash.ItemType == "ring")
                {
                    ringsAmount++;
                }
                else if (item.ItemOfStash.ItemType == "amulet")
                {
                    amuletAmount++;
                }
                else if (item.ItemOfStash.ItemType == "belt")
                {
                    beltAmount++;
                }
                else if (item.ItemOfStash.ItemType == "boots")
                {
                    bootsAmount++;
                }
                else if (item.ItemOfStash.ItemType == "gloves")
                {
                    glovesAmount++;
                }
                else if (item.ItemOfStash.ItemType == "chest")
                {
                    chestAmount++;
                }
                else if (item.ItemOfStash.ItemType == "helmet")
                {
                    helmetAmount++;
                }
                else if (item.ItemOfStash.ItemType == "weapon")
                {
                    weaponsAmount++;
                }
            }



            int rings = ringsAmount / 2;
            int weapons = weaponsAmount / 2;
            int sets = new[] { rings, weapons, helmetAmount, bootsAmount, glovesAmount, chestAmount, amuletAmount, beltAmount }.Min();

            if(sets > 0)
            {
                return true;
            }
            return false;
        }

        private Dictionary<string, List<ItemWithStash>> GetItemOrderList(List<ItemWithStash> itemList)
        {
            //int breakvar = 0;
            //ItemOrderList.Clear();
            List<ItemWithStash> newItemOrderList = new List<ItemWithStash>();
            List<ItemWithStash> newItemOrderListRest = new List<ItemWithStash>();

            while (HasFullSet(itemList))
            {
                // 0: chest
                // 1: weapon 1
                // 2: weapon 2
                // 3: gloves
                // 4: helmet
                // 5: boots
                // 6: belt
                // 7: ring 1
                // 8: ring 2
                // 9: amulet
                int end = 0;
                while (end < 10)
                {
                    Trace.WriteLine(end, "end: ");
                    for(int i = 0; i < itemList.Count; i++)
                    //foreach(Item i in itemList)
                    {
                        ItemWithStash _i = itemList[i];
                        switch (end)
                        {
                            case 0:
                                if (itemList[i].ItemOfStash.ItemType == "chest" && !newItemOrderList.Contains(_i))
                                {
                                    newItemOrderList.Add(_i);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 1:
                                if (itemList[i].ItemOfStash.ItemType == "weapon" && !newItemOrderList.Contains(_i))
                                {
                                    newItemOrderList.Add(_i);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;

                            case 2:
                                if (itemList[i].ItemOfStash.ItemType == "weapon" && !newItemOrderList.Contains(_i))
                                {
                                    newItemOrderList.Add(_i);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;

                            case 3:
                                if (itemList[i].ItemOfStash.ItemType == "gloves" && !newItemOrderList.Contains(_i))
                                {
                                    newItemOrderList.Add(_i);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 4:
                                if (itemList[i].ItemOfStash.ItemType == "helmet" && !newItemOrderList.Contains(_i))
                                {
                                    newItemOrderList.Add(_i);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 5:
                                if (itemList[i].ItemOfStash.ItemType == "boots" && !newItemOrderList.Contains(_i))
                                {
                                    newItemOrderList.Add(_i);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 6:
                                if (itemList[i].ItemOfStash.ItemType == "belt" && !newItemOrderList.Contains(_i))
                                {
                                    newItemOrderList.Add(_i);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 7:
                                if (itemList[i].ItemOfStash.ItemType == "ring" && !newItemOrderList.Contains(_i))
                                {
                                    newItemOrderList.Add(_i);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 8:
                                if (itemList[i].ItemOfStash.ItemType == "ring" && !newItemOrderList.Contains(_i))
                                {
                                    newItemOrderList.Add(_i);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                            case 9:
                                if (itemList[i].ItemOfStash.ItemType == "amulet" && !newItemOrderList.Contains(_i))
                                {
                                    newItemOrderList.Add(_i);
                                    itemList.RemoveAt(i);
                                    end++;
                                }
                                break;
                        }
                    }
                }
            }

            foreach(ItemWithStash i in itemList)
            {
                newItemOrderListRest.Add(i);
            }

            Dictionary<string, List<ItemWithStash>> ret = new Dictionary<string, List<ItemWithStash>>();
            ret.Add("list", newItemOrderList);
            ret.Add("rest", newItemOrderListRest);

            return ret;
        }

        // 
        public void ActivateNextCell(bool active)
        {
            if (active)
            {
                if (GlobalItemOrderList.Count > 0)
                {
                    foreach (StashTab s in MainWindow.stashTabsModel.StashTabs)
                    {
                        s.TabHeaderColor = Brushes.Transparent;
                        foreach (Cell c in s.OverlayCellsList)
                        {
                            c.Active = false;
                        }
                    }
                    MainWindow.stashTabsModel.StashTabs[GlobalItemOrderList[0].IndexOfStash].ActivateItemCells(GlobalItemOrderList[0].ItemOfStash);
                    if (Properties.Settings.Default.ColorStash != "")
                    {
                        MainWindow.stashTabsModel.StashTabs[GlobalItemOrderList[0].IndexOfStash].TabHeaderColor = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Properties.Settings.Default.ColorStash));
                    }
                    else
                    {
                        MainWindow.stashTabsModel.StashTabs[GlobalItemOrderList[0].IndexOfStash].TabHeaderColor = Brushes.Red;
                    }
                    //ActivateItemCells(ItemOrderList[0]);
                    GlobalItemOrderList.RemoveAt(0);
                }
                else
                {
                    //MainWindow.stashTabsModel.StashTabs[GlobalItemOrderList[0].IndexOfStash].TabHeaderColor = Brushes.Transparent;
                    foreach(StashTab s in MainWindow.stashTabsModel.StashTabs)
                    {
                        s.TabHeaderColor = Brushes.Transparent;
                        foreach (Cell c in s.OverlayCellsList)
                        {
                            c.Active = false;
                        }
                    }
                }
            }
        }
        private void ReIndexAllStashTabs()
        {
            if(MainWindow.stashTabsModel != null)
            {
                for (int i = 0; i < MainWindow.stashTabsModel.StashTabs.Count; i++)
                {
                    MainWindow.stashTabsModel.StashTabs[i].TabIndex = i;
                }
            }
        }

        public void PrepareSelling()
        {
            GlobalItemOrderList.Clear();
            GlobalItemOrderListRest.Clear();
            ReIndexAllStashTabs();
            foreach(StashTab s in MainWindow.stashTabsModel.StashTabs)
            {
                s.PrepareOverlayList();
                if (s.ItemList != null)
                {
                    //Trace.WriteLine(s.ItemList.Count());
                    if (s.FullSets == 0)
                    {
                        foreach (Item i in s.ItemList)
                        {
                            GlobalItemOrderListRest.Add(new ItemWithStash { IndexOfStash = s.TabIndex, ItemOfStash = i });
                        }
                    }
                    else
                    {
                        List<ItemWithStash> copyItemList = new List<ItemWithStash>();
                        foreach(Item i in s.ItemList)
                        {
                            copyItemList.Add(new ItemWithStash { IndexOfStash = s.TabIndex, ItemOfStash = i });
                        }
                        Dictionary<string, List<ItemWithStash>> itemorder = GetItemOrderList(copyItemList);
                        GlobalItemOrderList.AddRange(itemorder["list"]);
                        GlobalItemOrderListRest.AddRange(itemorder["rest"]);
                    }
                }
            }
            Dictionary<string, List<ItemWithStash>> itemorderrest = GetItemOrderList(GlobalItemOrderListRest);
            GlobalItemOrderList.AddRange(itemorderrest["list"]);
        }
    }

    public class ItemWithStash
    {
        public int IndexOfStash { get; set; }
        public Item ItemOfStash { get; set; }
    }
}
