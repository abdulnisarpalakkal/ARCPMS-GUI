using ARCPSGUI.DB;
using ARCPSGUI.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ARCPSGUI.FloorUI.ucData
{
    /// <summary>
    /// Interaction logic for ucMachineJob.xaml
    /// </summary>
    public partial class ucMachineJob : UserControl
    {
        MachineJobDba objMachineJobDba = null;
        List<MachineJobData> MachineJobDataList = null;
        public int Floor { get; set; }
        System.Timers.Timer timerToUpdateGrid = null;
        public ucMachineJob()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            this.timerToUpdateGrid = new System.Timers.Timer();
            this.timerToUpdateGrid.Enabled = true;
            this.timerToUpdateGrid.Interval = 4000;
            this.timerToUpdateGrid.Start();
            this.timerToUpdateGrid.Elapsed += new System.Timers.ElapsedEventHandler(timerToUpdateGrid_Elapsed);
          
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            StopTimer(timerToUpdateGrid);
            timerToUpdateGrid.Dispose();
        }
        void timerToUpdateGrid_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (objMachineJobDba == null)
                objMachineJobDba = new MachineJobDba();
            machineJobGrid.Dispatcher.BeginInvoke(new Action(() =>
            {
                MachineJobDataList = objMachineJobDba.GetMachineJobsWrtFloor(Floor);
                machineJobGrid.ItemsSource = MachineJobDataList;
            }));
           
        }
        private void StartTimer(System.Timers.Timer timer)
        {
            try
            {
                timer.Start();
            }
            catch (Exception ex)
            {

            }
        }
        private void StopTimer(System.Timers.Timer timer)
        {
            try
            {
                timer.Stop();
            }
            catch (Exception ex)
            {

            }
        }
         
    }
}
