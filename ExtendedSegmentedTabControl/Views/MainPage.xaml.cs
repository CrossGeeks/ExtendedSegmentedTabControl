using System.ComponentModel;
using Xamarin.Forms;

namespace ExtendedSegmentedTabControl
{
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }
    }
}
