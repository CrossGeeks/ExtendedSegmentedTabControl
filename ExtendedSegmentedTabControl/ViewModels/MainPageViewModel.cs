using System.Collections.Generic;
using ExtendedSegmentedTabControl.Models;

namespace ExtendedSegmentedTabControl
{
    public class MainPageViewModel
    {
        public List<TabOption> TabOptions { get; set; }
        public MainPageViewModel()
        {
            TabOptions=new List<TabOption>()
            {
                {new TabOption(){  Name="Invertebrates"} },
                {new TabOption(){  Name="Fish"} },
                {new TabOption(){  Name="Amphibians"} },
                {new TabOption(){  Name="Reptiles"} },
                {new TabOption(){  Name="Birds"} },
                {new TabOption(){  Name="Mammals"} }
            };
        }
    }
}
