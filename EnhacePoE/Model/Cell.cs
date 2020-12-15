using System.ComponentModel;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public int XIndex { get; set; }
        public int YIndex { get; set; }
        public string ItemID { get; set; }
        //public ICommand ToggleCellCommand { get; set; }
        
        public string CellName { get; set; }

        public string ButtonName { get; set; }

        public Button CellButton { get; set; }

        public Cell()
        {
            CellButton = new Button
            {
                //Command = ToggleCellCommand,
                Content = ButtonName,
                CommandParameter = Active
            };
        }
    }
}
