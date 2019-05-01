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
using System.Data;
using Oracle.DataAccess.Client;

using ARCPSGUI.DB;
using ARCPSGUI.Controls;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for ucWashingTrans.xaml
    /// </summary>
    public partial class ucWashingTrans : UserControl
    {
        ucWinGrid wgrid = new ucWinGrid();
        string selectedFilterEES = "";
        OracleDependency dep = null;
        OracleConnection con = null;
        frmHome g_homeUI = null;
         Connection dbpm = new  Connection();

        public ucWashingTrans()
        {
            InitializeComponent();
            hostListView.Child = wgrid;
            wgrid.dataGridView1.DataSource = GetERPTasks().DefaultView;
            SetGridColumnSize();
        }

        void RefreshGrid()
        {
            if (wgrid.dataGridView1.IsHandleCreated == true)
            {
                wgrid.dataGridView1.BeginInvoke(new Action(() =>
                {
                    //wgrid.dataGridView1.Rows.Clear();
                    wgrid.dataGridView1.DataSource = GetERPTasks().DefaultView;
                    SetGridColumnName();
                    SetGridColumnSize();
                }));
            }
        }


        DataTable GetERPTasks()
        {
            string query = "";

            query = "select * from CAR_WASH_QUEUE_VIEW ORDER BY PRIORITY"; //WHERE status_flag != -1

            DataTable dt = new DataTable();
            try
            {

                dt.TableName = "ERPTasks";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                        dadapter.Fill(dt);
                    }
                }
            }
            finally
            { }

            //lblTotalEntryESSTrans.Content = "Current Parking : " + dt.Select("[PARK MODE] = 'ENTRY'").Count();
            //lblTotalExitESSTrans.Content = "Current Retrieval : " + dt.Select(@"[PARK MODE] = 'Exit'").Count();
            return dt;
        }

        void SetGridColumnSize()
        {
            wgrid.dataGridView1.Columns[1].Width = 100;
            wgrid.dataGridView1.Columns[5].Width = 50;
            wgrid.dataGridView1.Columns[6].Width = 90;
            wgrid.dataGridView1.Columns[7].Width = 100;
            wgrid.dataGridView1.Columns[8].Visible = false;
            wgrid.dataGridView1.Columns[10].Visible = false;
        }

        void SetGridColumnName()
        {
            wgrid.dataGridView1.Columns[0].HeaderText = "NO#";
            wgrid.dataGridView1.Columns[1].HeaderText = "TRANS ID";

            //wgrid.dataGridView1.Columns[2].HeaderText = "MACHINE";
            //wgrid.dataGridView1.Columns[3].HeaderText = "";
        }
        public void Dispose()
        {
            //this.g_homeUI.OnTriggerEESQueueNotificaiton -= g_homeUI_OnTriggerEESQueueNotificaiton;
            //this.g_homeUI.OnTriggerSlotPathNotificaiton -= g_homeUI_OnTriggerSlotPathNotificaiton;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "";
                Int32 washingQueueId = 0;
                string washingStatus = "";
                string customerId = "";
                int priority = 0;
                if (wgrid.dataGridView1.SelectedRows.Count > 0)
                    customerId = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["CUSTOMER ID"].Value);

                if (string.IsNullOrEmpty(customerId) == true) return;

                 washingStatus = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells[9].Value);
                 int.TryParse(Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["PRIORITY"].Value), out priority);
                 if (washingStatus == "PROCESSING")
                   MessageBox.Show("Car wash is in progress.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                  
                 if (washingStatus == "WAITING" &&
                      MessageBox.Show("Do you want to cancel the car wash for the car " + wgrid.dataGridView1.SelectedRows[0].Cells[2].Value.ToString()
                      , "Information", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                 {
                     if (wgrid.dataGridView1.SelectedRows.Count > 0)
                         washingQueueId = Convert.ToInt32(wgrid.dataGridView1.SelectedRows[0].Cells[8].Value.ToString());

                     query = "UPDATE L2_CAR_WASH_QUEUE SET STATUS = -1 WHERE WASH_Q_ID = " + washingQueueId +
                            " AND STATUS = 0";
                     using (OracleConnection con = new OracleConnection( Connection.connectionString))
                     {
                         if (con.State == ConnectionState.Closed) con.Open();

                         using (OracleCommand command = new OracleCommand(query))
                         {
                             command.CommandText = query;
                             command.Connection = con;
                             command.ExecuteNonQuery();
                         }
                     }

                     //priority re-order
                     query = "UPDATE L2_CAR_WASH_QUEUE SET PRIORITY = (PRIORITY - 1) WHERE PRIORITY > " + priority +
                            " AND STATUS = 0";
                     using (OracleConnection con = new OracleConnection( Connection.connectionString))
                     {
                         if (con.State == ConnectionState.Closed) con.Open();

                         using (OracleCommand command = new OracleCommand(query))
                         {
                             command.CommandText = query;
                             command.Connection = con;
                             command.ExecuteNonQuery();
                         }
                     }
                     GetERPTasks();
                 }
            }
            finally
            { }

        }

        private void btnCarWashFinish_Click(object sender, RoutedEventArgs e)
        {
            //if (MessageBox.Show("Confirm door is opend at car wash slot. Do you want to continue?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question)
            //     == MessageBoxResult.Yes)
            //{
            //    if (dbpm.GetCarPresentInCarWash() == true)
            //        dbpm.UpdateCarWashFinishTrigger(2);
            //    else
            //        MessageBox.Show("L2 logic cannot get car from the car wash slot. It seems that L2 logic may not process this car wash.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            //}
        }

        private void btnCarWashFinish_Loaded(object sender, RoutedEventArgs e)
        {
            //btnCarWashFinish.IsEnabled = dbpm.IsCarWashFinished();
        }
    }
}
