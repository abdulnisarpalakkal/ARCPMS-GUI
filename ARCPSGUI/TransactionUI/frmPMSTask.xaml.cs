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
using Oracle.DataAccess.Client;
using System.Data;
using ARCPSGUI.Popup;
using ARCPSGUI.Controls;
using ARCPSGUI.DB;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for frmPMSTask.xaml
    /// </summary>
    public partial class frmPMSTask : Window
    {
        ucWinGrid wgrid = new ucWinGrid();
        string selectedFilterEES = "";
        OracleDependency dep = null;
        OracleConnection con = null;
        frmHome g_homeUI = null;
        System.Timers.Timer timerToUpdateGrid = null;
        static bool isLoaded = false;
        public static frmPMSTask uiPmsTask = null;
        ERPDba objERPDba = null;
        private frmPMSTask()
        {
             InitializeComponent();
             if (objERPDba == null)
                 objERPDba = new ERPDba();
        }

        public static void ShowPMSTask(frmHome homeUI)
        {
            if (frmPMSTask.uiPmsTask == null)
            {
                frmPMSTask pmsTask = new frmPMSTask();
                frmPMSTask.uiPmsTask = pmsTask;
                pmsTask.Show();
                pmsTask.g_homeUI = homeUI;
                pmsTask.DoOnLoad();
            }
            else
            {
                frmPMSTask.uiPmsTask.BringIntoView();
                frmPMSTask.uiPmsTask.Activate();
                frmPMSTask.uiPmsTask.Topmost = true;
                frmPMSTask.uiPmsTask.WindowState = WindowState.Normal;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SetGridHeaderConfig();
        }

        public void DoOnLoad()
        {
           
            hostListView.Child = wgrid;
            wgrid.dataGridView1.DataSource = objERPDba.GetPMSTasks("").DefaultView;
            SetGridColumnSize();
            UpdateGrid();
          //  TriggerCommandNotification();
        }
      
       

        void SetGridColumnSize()
        {
            wgrid.dataGridView1.Columns[0].Width = 30;
            wgrid.dataGridView1.Columns[1].Width = 60;
            wgrid.dataGridView1.Columns[2].Width = 40;

            wgrid.dataGridView1.Columns[3].Width = 40;
            wgrid.dataGridView1.Columns[4].Width = 40;
            wgrid.dataGridView1.Columns[5].Width = 40;

            wgrid.dataGridView1.Columns[6].Width = 40;
            wgrid.dataGridView1.Columns[7].Width = 40;
            wgrid.dataGridView1.Columns[8].Width = 40;

            wgrid.dataGridView1.Columns[9].Width = 40;
            wgrid.dataGridView1.Columns[10].Width = 40;
            wgrid.dataGridView1.Columns[11].Width = 40;
            wgrid.dataGridView1.Columns[12].Width = 150;
            //wgrid.dataGridView1.Columns[13].Width = 65;
            //wgrid.dataGridView1.Columns[14].Width = 45;
            //wgrid.dataGridView1.Columns[15].Width = 45;

        }

        void dep_OnChange(object sender, OracleNotificationEventArgs eventArgs)
        {
            RefreshGrid();
        }
        void RefreshGrid()
        {
            try
            {
                if (wgrid.dataGridView1.IsHandleCreated == false) return;

                wgrid.BeginInvoke(new Action(() =>
                {
                    wgrid.dataGridView1.DataSource = objERPDba.GetPMSTasks("").DefaultView;
                    //SetGridColumnSize();
                }));
            }
            finally
            {

            }
        }
        public void Dispose()
        {
            try
            {
                // this.g_homeUI.OnTriggerSlotPathNotificaiton -= g_homeUI_OnTriggerSlotPathNotificaiton;
                if (timerToUpdateGrid != null) timerToUpdateGrid.Stop();
            }
            catch (Exception errMsg)
            {
                MessageBox.Show("Error occured on unregister oracle notificaiton", "ERP Task", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnERPTrans_Click(object sender, RoutedEventArgs e)
        {
            this.g_homeUI.homeui_triggerErpTask(sender, e);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.g_homeUI.homeui_triggerErpTask(sender, e);
        }

       

        void OnTriggerNotification(object sender, OracleNotificationEventArgs eventArgs)
        {
            RefreshGrid();
        }

        void UpdateGrid()
        {
            try
            {
                this.timerToUpdateGrid = new System.Timers.Timer();
                this.timerToUpdateGrid.Enabled = true;
                this.timerToUpdateGrid.Interval = 3000;
                timerToUpdateGrid.Start();
                timerToUpdateGrid.Elapsed += new System.Timers.ElapsedEventHandler(timerToUpdateGrid_Elapsed);
            }
            catch (Exception errMsg)
            { }
        }

        void timerToUpdateGrid_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RefreshGrid();
        }
        void SetGridHeaderConfig()
        {
            try
            {
                wgrid.dataGridView1.ColumnHeadersHeight = 30;
                wgrid.dataGridView1.AdvancedRowHeadersBorderStyle.All = System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.Outset;
                wgrid.dataGridView1.AllowUserToOrderColumns = true;
                wgrid.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            }
            catch (Exception errMsg)
            { }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (frmPMSTask.uiPmsTask != null)
            {
                frmPMSTask.uiPmsTask = null;
            }
        }

        private void complete_but_Click(object sender, RoutedEventArgs e)
        {
            PMSAbortPop abortPop = new PMSAbortPop();
            abortPop.Show();
        }

       
    }
}
