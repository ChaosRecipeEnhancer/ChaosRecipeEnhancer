using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using EnhancePoE.Model;


namespace EnhancePoE
{
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

        private void InitializeBases()
        {
            string pathBoots = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Bases\BootsBases.txt");
            string[] boots = File.ReadAllLines(pathBoots);
            foreach (string line in boots)
            {
                if (line == "") { continue; }
                this.BootsBases.Add(line.Trim());
            }

            string pathGloves = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Bases\GlovesBases.txt");
            string[] gloves = File.ReadAllLines(pathGloves);
            foreach (string line in gloves)
            {
                if (line == "") { continue; }
                this.GlovesBases.Add(line.Trim());
            }

            string pathHelmet = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Bases\HelmetBases.txt");
            string[] helmet = File.ReadAllLines(pathHelmet);
            foreach (string line in helmet)
            {
                if (line == "") { continue; }
                this.HelmetBases.Add(line.Trim());
            }

            string pathWeapon = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Bases\WeaponSmallBases.txt");
            string[] weapon = File.ReadAllLines(pathWeapon);
            foreach (string line in weapon)
            {
                if (line == "") { continue; }
                this.WeaponBases.Add(line.Trim());
            }

            string pathChest = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Bases\BodyArmourBases.txt");
            string[] chest = File.ReadAllLines(pathChest);
            foreach (string line in chest)
            {
                if (line == "") { continue; }
                this.ChestBases.Add(line.Trim());
            }

            string pathRing = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Bases\RingBases.txt");
            string[] ring = File.ReadAllLines(pathRing);
            foreach (string line in ring)
            {
                if (line == "") { continue; }
                this.RingBases.Add(line.Trim());
            }

            string pathAmulet = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Bases\AmuletBases.txt");
            string[] amulet = File.ReadAllLines(pathAmulet);
            foreach (string line in amulet)
            {
                if (line == "") { continue; }
                this.AmuletBases.Add(line.Trim());
            }

            string pathBelt = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Bases\BeltBases.txt");
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
                else
                {
                    Trace.WriteLine("ItemList empty...");
                }

            }
        }

        public void GetSetTargetAmount()
        {
            if(Properties.Settings.Default.Sets > 0)
            {
                this.SetTargetAmount = Properties.Settings.Default.Sets;
            }
            else
            {
                if (Properties.Settings.Default.quad)
                {
                    this.SetTargetAmount = 16;
                }
                else
                {
                    this.SetTargetAmount = 4;
                }
            }
        }

        public void GetOverallItemsAmount()
        {

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
            GetSetTargetAmount();
            GetSetAmount();
        }

    }
}
