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
using ARCPSGUI.DB;
using ARCPSGUI.Model;
using System.Collections.ObjectModel;

using System.Reflection;

namespace ARCPSGUI.UserControls
{
    /// <summary>
    /// Interaction logic for ucWaitHistView.xaml
    /// </summary>
    public partial class ucDBLogView : UserControl
    {
        GeneralDba objGeneralDba = null;
        System.Timers.Timer timerToUpdateGrid = null;
        decimal queueId = 0;
        public delegate void InvokeDelegate();
        public ObservableCollection<DBLogData> data { get; set; }
        public ucDBLogView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

           
           // LoadLogData(0);
            LoadLogData(queueId);
            this.timerToUpdateGrid = new System.Timers.Timer();
            this.timerToUpdateGrid.Enabled = true;
            this.timerToUpdateGrid.Interval = 10000;
            this.timerToUpdateGrid.Elapsed += new System.Timers.ElapsedEventHandler(timerToUpdateGrid_Elapsed);

            data = new ObservableCollection<DBLogData>();
            this.dbLogGrid.ItemsSource = data;

          
            
            this.timerToUpdateGrid.Start();
           
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            timerToUpdateGrid.Stop();
            timerToUpdateGrid.Dispose();
        }
        void timerToUpdateGrid_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {

            try
            {
                timerToUpdateGrid.Stop();
                LoadLogData(queueId);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                timerToUpdateGrid.Start();
            }
           
        }
        void LoadLogData(decimal queueId)
        {
            //DBLogData objDBLogData = new DBLogData();
            //objDBLogData.TrackId = 100;

            //ObservableCollection<DBLogData> newCol = new ObservableCollection<DBLogData>();
            //newCol.Add(objDBLogData);

            //this.Dispatcher.BeginInvoke(new InvokeDelegate(new Action(() =>
            //{

            //   // data=newCol;
            //     bjGeneralDba.GetDBLogData(queueId);
            //})));


            dbLogGrid.Dispatcher.BeginInvoke(new InvokeDelegate(new Action(() =>
            {
                dbLogGrid.ItemsSource = objGeneralDba.GetDBLogData(queueId);
            })));
        }

        private void searchBut_Click(object sender, RoutedEventArgs e)
        {
           
            Decimal.TryParse(queueIdText.Text, out queueId);

            LoadLogData(queueId);
        }

        //private void dbLogGrid_ScrollChanged(object sender, ScrollChangedEventArgs e)
        //{
        //    if (timerToUpdateGrid.Enabled)
        //        timerToUpdateGrid.Enabled = false;
        //}

        //private void dbLogGrid_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (!timerToUpdateGrid.Enabled)
        //        timerToUpdateGrid.Enabled = true;
        //}
        //public static void DoubleBuffered(this DataGridView dgv, bool setting)
        //{
        //    Type dgvType = dgv.GetType();
        //    PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
        //      BindingFlags.Instance | BindingFlags.NonPublic);
        //    pi.SetValue(dgv, setting, null);
        //}

       

       


        
    }
}
