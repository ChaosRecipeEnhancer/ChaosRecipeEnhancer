using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace EnhancePoE.Model
{
    public class Cell : INotifyPropertyChanged
    {

        private bool _active;
        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = value;
                OnPropertyChanged("active");
            }
        }

        //private SolidBrush _buttonColor;
        //public SolidBrush ButtonColor
        //{
        //    get
        //    {
        //        return _buttonColor;
        //    }
        //    set
        //    {
        //        if(!this.Active)
        //        {
        //            _buttonColor = new SolidBrush(Color.Blue);
        //        }
        //        else
        //        {
        //            _buttonColor = new SolidBrush(Color.Red);
        //        }
        //        OnPropertyChanged("color");
        //    }
        //}

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int XIndex { get; set; }
        public int YIndex { get; set; }
        public string ItemID { get; set; }
        public ICommand ToggleCellCommand { get; set; }
        
        public string CellName { get; set; }

        public string ButtonName { get; set; }

        public Button CellButton { get; set; }

        public Cell()
        {
            CellButton = new Button
            {
                Command = ToggleCellCommand,
                Content = ButtonName,
                CommandParameter = Active
            };
        }

        //private void ToggleCell(Cell cell)
        //{
        //    this.Active = !this.Active;
        //    Trace.WriteLine(this.Active);
        //}

        //public Cell()
        //{
        //    //this.ToggleCellCommand = new RelayCommand<Cell>(StashTab.ActivateNextCell);

        //    ButtonName = "Button " + this.XIndex + " " + this.YIndex;
        //}
    }
}
