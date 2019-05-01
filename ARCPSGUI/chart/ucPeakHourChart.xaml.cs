using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ARCPSGUI.DB;

namespace ARCPSGUI.chart
{
    /// <summary>
    /// Interaction logic for ucPeakHourChart.xaml
    /// </summary>
    public partial class ucPeakHourChart : UserControl
    {
        ChartDba objChartDba = null;
        public ucPeakHourChart()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.peakStartDate.SelectedDate = DateTime.Now;
            string startDateString = null;
            DateTime? startDate = peakStartDate.SelectedDate;
            startDateString = startDate.Value.ToString("dd-MMM-yyyy");
            LoadLineChartData(startDateString,null);
            
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void peakTimeSubmit_Click(object sender, RoutedEventArgs e)
        {
            string startDateString = null;
            string endDateString = null;
            string chartType = null;

            DateTime? startDate = peakStartDate.SelectedDate;
           

            if (startDate == null)
            {
                // ... A null object.
               // this.Title = "No date";
            }
            else
            {
                startDateString = startDate.Value.ToString("dd-MMM-yyyy");
            }

            LoadLineChartData(startDateString, endDateString);

        }
        private void LoadLineChartData(string startDate,string endDate)
        {
            Hashtable entryHash = new Hashtable();
            Hashtable exitHash = new Hashtable();
            double avgEntryCars = 0;
            double avgEntryTime = 0;
            double avgExitCars = 0;
            double avgExitTime = 0;
            if (objChartDba == null)
                objChartDba = new ChartDba();

            //  ht = new DBConnection().getEntryParksData();
            entryHash = objChartDba.GetEntryPeakTimeFindView(startDate, endDate);
            exitHash = objChartDba.GetExitPeakTimeFindView(startDate, endDate);
            //new DBConnection().getAvgEntryCarAndTime(startDate, endDate,out avgEntryCars,out avgEntryTime);
            //new DBConnection().getAvgExitCarAndTime(startDate, endDate, out avgExitCars, out avgExitTime);

            //avg_entry_cars.Content = "Avg. entry cars/day: "+avgEntryCars;
            //avg_entry_duration.Content = "Avg. entry time: " + avgEntryTime;
            //avg_exit_cars.Content = "Avg. exit cars/day: " + avgExitCars;
            //avg_exit_duration.Content = "Avg. exit time: " + avgExitTime;
            //from_date.Content = startDate;
            //to_date.Content = endDate;

            ((LineSeries)mcChart.Series[0]).ItemsSource = entryHash;
            ((LineSeries)mcChart.Series[1]).ItemsSource = exitHash;

        


        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {

        }

      
    }
}
