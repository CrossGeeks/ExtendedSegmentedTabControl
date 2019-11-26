using System.ComponentModel;

namespace ExtendedSegmentedTabControl.Models
{
    public class TabOption: INotifyPropertyChanged
    {
       public string Name { get; set; }
       public bool IsSelected { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
