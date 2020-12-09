using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Linq;
using EnhancePoE.UserControls;
using System.Windows.Controls;
using System.Collections.Generic;
using EnhancePoE.Model;

namespace EnhancePoE
{
    public class TabItemViewModel : INotifyPropertyChanged
    {
        private int _currentTab;
        public int CurrentTab
        {
            get { return _currentTab; }
            set
            {
                _currentTab = value;
                OnPropertyChanged("CurrentTab");
            }
        }

        public TabItemViewModel()
        {
            StashTabs = new ObservableCollection<Model.StashTab>();
            StashTabItems = new ObservableCollection<TabItem>();
            CurrentTab = -1;

            if(Properties.Settings.Default.StashTabsString != null)
            {
                Trace.WriteLine("test not null");
                foreach(StashTab s in SettingsSerializer.DeserializeStashTab())
                {
                    StashTabs.Add(s);
                    StashTabItems.Add(s.StashTabItem);
                    CurrentTab++;
                    OnPropertyChanged("StashTabs");
                }
            }
        }

        public ObservableCollection<TabItem> StashTabItems
        {
            get { return _stashTabItems; }
            set
            {
                _stashTabItems = value;
                OnPropertyChanged("StashTabs");
            }
        }

        public ObservableCollection<Model.StashTab> StashTabs
        {
            get { return _stashTabs; }
            set
            {
                _stashTabs = value;
                OnPropertyChanged("StashTabs");
            }
        }


        private ICommand _addTab;
        private ICommand _removeTab;
        private ObservableCollection<Model.StashTab> _stashTabs;
        private ObservableCollection<TabItem> _stashTabItems;

        public ICommand AddTab
        {
            get
            {
                return _addTab ?? (_addTab = new Command(
                   x =>
                   {
                       AddTabItem();
                   }));
            }
        }

        public ICommand RemoveTab
        {
            get
            {
                return _removeTab ?? (_removeTab = new Command(
                   x =>
                   {
                       RemoveTabItem();
                   }));
            }
        }

        private void RemoveTabItem()
        {
            if (CurrentTab >= 0)
            {
                if (MainWindow.stashTabOverlay.IsOpen)
                {
                    MainWindow.stashTabOverlay.Hide();
                }
                StashTabs.Remove(StashTabs[CurrentTab]);
                StashTabItems.Remove(StashTabItems[CurrentTab]);
            }
        }

        private void AddTabItem()
        {
            if (MainWindow.stashTabOverlay.IsOpen)
            {
                MainWindow.stashTabOverlay.Hide();
            }
            Model.StashTab currTab = new Model.StashTab("New Tab");
            StashTabs.Add(currTab);
            StashTabItems.Add(currTab.StashTabItem);
            CurrentTab++;
            currTab.TabIndex = CurrentTab;
            OnPropertyChanged("StashTabs");
            //Trace.WriteLine(CurrentTab);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new ArgumentNullException(GetType().Name + " does not contain property: " + propertyName);
        }
    }
}