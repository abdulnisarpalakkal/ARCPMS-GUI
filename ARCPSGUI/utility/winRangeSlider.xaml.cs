using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ARCPSGUI.utility
{
    /// <summary>
    /// Interaction logic for winRangeSlider.xaml
    /// </summary>
    public partial class winRangeSlider : Window
    {
        public winRangeSlider()
        {
            InitializeComponent();
            rangeSlider.RangeStart = 1*60;
            rangeSlider.RangeStop = 24*60;//Date Now
            rangeSlider.RangeStartSelected = 5*60; //6 hours ago
            rangeSlider.RangeStopSelected = 12*60; //9 hours ago
            rangeSlider.MinRange = 30;//1 minute
        }
        private void RangeSlider_RangeSelectionChanged(object sender, AC.AvalonControlsLibrary.Controls.RangeSelectionChangedEventArgs e)
        {
            Console.WriteLine("e.NewRangeStart: " + e.NewRangeStart);

            Console.WriteLine("e.NewRangeStop: " + e.NewRangeStop);
            long minute = 0;
            long hour = 0;
            hour = e.NewRangeStart / 60;
            minute = e.NewRangeStart % 60;
            min.Content = hour+":"+minute;

            hour = e.NewRangeStop / 60;
            minute = e.NewRangeStop % 60;
            max.Content = hour + ":" + minute;
           

        }
    }
}
