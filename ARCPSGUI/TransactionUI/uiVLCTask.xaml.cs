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
using ARCPSGUI.Controls;
using Oracle.DataAccess.Client;
using System.Data;
using ARCPSGUI.DB;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for uiVLCTask.xaml
    /// </summary>
    public partial class uiVLCTask : UserControl, IDisposable
    {
        ucWinGrid wgrid = new ucWinGrid();
        string selectedFilterEES = "";
        OracleDependency dep = null;
        OracleConnection con = null;
        frmHome g_homeUI = null;
        System.Timers.Timer timerToUpdateGrid = null;

        public uiVLCTask(frmHome homeUI)
        {
            InitializeComponent();
         
            this.g_homeUI = homeUI;

            hostListView.Child = wgrid;
            wgrid.dataGridView1.DataSource = GetERPTasks("").DefaultView;
            SetGridColumnSize();
            UpdateGrid();

            //this.g_homeUI.OnTriggerEESQueueNotificaiton += new EventHandler(g_homeUI_OnTriggerEESQueueNotificaiton);
            //this.g_homeUI.OnTriggerSlotPathNotificaiton += new EventHandler(g_homeUI_OnTriggerSlotPathNotificaiton);
           // DoOnLoad();
        }

        void g_homeUI_OnTriggerSlotPathNotificaiton(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        void g_homeUI_OnTriggerEESQueueNotificaiton(object sender, EventArgs e)
        {
            RefreshGrid();
        }
        public void DoOnLoad()
        {
           
        }

        void RefreshGrid()
        {
           // wgrid.dataGridView1.BeginInvoke(new Action(() =>
             //  {
            try
            {

                if (wgrid.dataGridView1.IsHandleCreated == true)
                {
                    wgrid.dataGridView1.BeginInvoke(new Action(() =>
                    {
                        wgrid.dataGridView1.DataSource = GetERPTasks("").DefaultView;
                        SetGridColumnSize();
                        SetGridColumnName();
                    }));
                } 
            }
            catch (Exception errMsg)
            { 
              
            }
            finally
            { 
            
            }
               //}));
        }

        DataTable GetERPTasks(string eesName)
        {
            string query = "";

            query = " select rownum S_NO, TRANS_ID,MACHINE,FLOOR,AISLE," + @"""ROW"",COMMAND,STATUS," + @"""START TIME"""
                        + " from executing_commands_in_floor where machine like 'VLC%'";

            DataTable dt = new DataTable();
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

            //lblTotalEntryESSTrans.Content = "Current Parking : " + dt.Select("[PARK MODE] = 'ENTRY'").Count();
            //lblTotalExitESSTrans.Content = "Current Retrieval : " + dt.Select(@"[PARK MODE] = 'Exit'").Count();
            return dt;
        }

        void SetGridColumnSize()
        {
            try
            {
                wgrid.dataGridView1.Columns[5].Width = 50;
                wgrid.dataGridView1.Columns[6].Width = 90;
                wgrid.dataGridView1.Columns[7].Width = 75;
                wgrid.dataGridView1.Columns[8].Width = 150;
            }
            finally { }

        }
        void SetGridColumnName()
        {
             try
            {
            wgrid.dataGridView1.Columns[0].HeaderText = "NO#";
            wgrid.dataGridView1.Columns[1].HeaderText = "TRANS ID";
            }
             finally { }
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

        public void Dispose()
        {
            if (timerToUpdateGrid != null) timerToUpdateGrid.Stop();
            //this.g_homeUI.OnTriggerEESQueueNotificaiton -= g_homeUI_OnTriggerEESQueueNotificaiton;
            //this.g_homeUI.OnTriggerSlotPathNotificaiton -= g_homeUI_OnTriggerSlotPathNotificaiton;
        }
    }
}
