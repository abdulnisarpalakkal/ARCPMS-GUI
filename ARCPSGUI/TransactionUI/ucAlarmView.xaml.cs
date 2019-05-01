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
using WindowsFormsApplication10;
using System.Data;
using Oracle.DataAccess.Client;
using ARCPSGUI.DB;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for ucAlarmView.xaml
    /// </summary>
    public partial class ucAlarmView : UserControl
    {
        ucWinGrid wgrid = new ucWinGrid();
        ucctrlTime ctrlDateTimeFrom = new ucctrlTime();
        ucctrlTime ctrlDateTimeTo = new ucctrlTime();
        static DataTable dtMachines = new DataTable();
        static DataTable dtErrorMaster = new DataTable();

        public ucAlarmView()
        {
            InitializeComponent();
            DoOnLoad();
        }

        public void DoOnLoad()
        {
            WindowsFormsHost.Child = wgrid;
            hostdatefrom.Child = ctrlDateTimeFrom;
            hostdateto.Child = ctrlDateTimeTo;
            chkDateEnable.IsChecked = false;
            ctrlDateTimeFrom.Enabled = false;
            ctrlDateTimeTo.Enabled = false;
            
            if(dtMachines.Rows.Count < 1) LoadMachines();
            if (dtErrorMaster.Rows.Count < 1) LoadErrorCodes();

            LoadAlarmData(); 
            //GetCurrentParks();
        }

        void LoadMachines()
        {
            string query = "";

          
            query = "select * from vw_MachinesForAlarmView order by machine, floor ";
            //else
            //query = "select * from vw_MachinesForAlarmView where floor =" + level + " order by machine, floor ";
        
            dtMachines.TableName = "Machines";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    OracleDataAdapter dadapter = new OracleDataAdapter(command);
                    dadapter.Fill(dtMachines);
                }
            }
        }

        void LoadMachinesToMachineCombo(int floor)
        {
            try
            {
                cmbMachines.BeginInit();
                cmbMachines.Items.Clear();
                if (dtMachines != null && dtMachines.Rows.Count > 0)
                {
                    if (floor > 0)
                    {
                        DataRow[] drowlist = dtMachines.Select("floor =" + floor);
                        foreach (DataRow drow in drowlist)
                        {
                            cmbMachines.Items.Add(drow[0]);
                        }
                    }
                    else
                    {
                        foreach (DataRow drow in dtMachines.Rows)
                        {
                            cmbMachines.Items.Add(drow[0]);
                        }
                    }
                }

                cmbMachines.Items.Insert(0, "All");
            }
            finally
            {
                cmbMachines.EndInit();
            }
        }
      
        void LoadErrorToErrorCombo(string machinename)
        {
            try
            {
                cmbErrorCode.BeginInit();
                if (dtErrorMaster != null && dtErrorMaster.Rows.Count > 0)
                {
                    cmbErrorCode.Items.Clear();
                    DataRow[] drowlist = null;
                    if (string.IsNullOrEmpty(machinename) == false)
                    {
                        drowlist = dtErrorMaster.Select("machine_model in (" + MachineModel(machinename) + ")");
                        foreach (DataRow drow in drowlist)
                        {
                            cmbErrorCode.Items.Add(drow[2]);
                        }
                    }
                    else
                    {
                        foreach (DataRow drow in dtErrorMaster.Rows)
                        {
                            cmbErrorCode.Items.Add(drow[2]);
                        }
                    }

                    cmbErrorCode.Items.Insert(0, "All");
                }
            }
            finally
            {
                cmbErrorCode.EndInit();
            }
        }

        string MachineModel(string machineName)
        {
            string machinemodel = "";
            try
            {
                if (machineName.Contains("LCM"))
                    machinemodel = "'LCM'";
                else if (machineName.Contains("UCM"))
                    machinemodel = "'UCM'";
                else if (machineName.Contains("REM"))
                    machinemodel = "'REM'";
                else if (machineName.Contains("PV"))
                    machinemodel = "'PVLDeck','PVL'";
                else if (machineName.Contains("VLC"))
                    machinemodel = "'VLCDeck','VLC'";
                else if (machineName.Contains("PST"))
                    machinemodel = "'PST100','PST1000'";
                else if (machineName.Contains("PS"))
                    machinemodel = "'PS'";
                else if (machineName.Contains("EES"))
                    machinemodel = "'EES'";
            }
            finally
            { 
            
            }
            return machinemodel;
        }
        void LoadErrorCodes()
        {
            string query = "";

            query = " select * from L2_ERROR_MASTER order by machine_model ";

            dtErrorMaster.TableName = "ERROR_MASTER";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    OracleDataAdapter dadapter = new OracleDataAdapter(command);
                    dadapter.Fill(dtErrorMaster);
                }
            }

            //foreach (DataRow drow in dtErrorMaster.Rows)
            //{
            //    cmbErrorCode.BeginInit();
            //    cmbErrorCode.Items.Add(drow[0]);
            //    cmbErrorCode.EndInit();
            //}
        }

        void SetErrorDescription(int errorCode)
        {
           string machineName = "";
            DataRow[] drow = null;
           //if (cmbMachines.SelectedIndex > -1)
           //{
           //    machineName = MachineModel(cmbMachines.Text);
           //    drow = dtErrorMaster.Select("MACHINE_MODEL ='" + machineName + "' AND ERROR_CODE =" + errorCode);
           //}
           //else
             drow = dtErrorMaster.Select(" ERROR_CODE =" + errorCode);

            if (drow != null && drow.Length > 0)
            {
                txtErrorDesc.Text = Convert.ToString(drow[0].ItemArray.GetValue(3));
            }
           //txtErrorDesc.Text = 
        }

        void LoadAlarmData()
        {
            string query = "";

            int level = 0;
            string machine = "";
            int errorCode = 0;
            DataTable dtAlarm = new DataTable();

            DateTime dtFrom;
            DateTime dtTo;

            wgrid.SuspendLayout();
            try
            {
                if (cmbFloor.SelectedIndex == 0)
                    level = 0;
                else if (cmbFloor.SelectedIndex > 0)
                    level = cmbFloor.SelectedIndex;

                if (cmbMachines.Text.Contains("All"))
                    machine = "";
                else
                    machine = cmbMachines.Text.Trim();

                if (cmbErrorCode.Text.Contains("All"))
                    errorCode = 0;
                else
                {
                    Int32.TryParse(cmbErrorCode.Text, out errorCode);
                }

                query = "select distinct ALAM.INITDATETIME, ALAM.MACHINENAME,ALAM.FLOOR, ALAM.ERRORCODE, "
                  + " EM.ERROR_DESCRIPTION, ALAM.MESSAGE from L2_ALARM ALAM INNER JOIN L2_ERROR_MASTER EM ON ALAM.ERRORCODE = EM.ERROR_CODE "
                  + " AND ALAM.MACHINE_MODEL = EM.MACHINE_MODEL where ALAM.machinename is not null ";

                if (errorCode > 0)
                    query += " and ALAM.ERRORCODE = " + errorCode;

                if (string.IsNullOrEmpty(machine) == false)
                    query += " and ALAM.machinename ='" + machine + "'";

                if (level > 0)
                    query += " and ALAM.FLOOR = " + level;

                if (chkDateEnable.IsChecked == true)
                {
                    dtFrom = ctrlDateTimeFrom.dateTimePicker1.Value;
                    dtTo = ctrlDateTimeTo.dateTimePicker1.Value;

                    query += " AND INITDATETIME BETWEEN '" + dtFrom.ToString("dd/MMM/yyyy hh:mm:ss tt") + "' AND '" + dtTo.ToString("dd/MMM/yyyy hh:mm:ss tt") + "'";
                }

                query += " order by ALAM.machinename, ALAM.floor ";

                dtAlarm.TableName = "ALARM";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                        dadapter.Fill(dtAlarm);
                    }
                }

                wgrid.dataGridView1.DataSource = null;
                wgrid.dataGridView1.DataSource = dtAlarm.DefaultView;
                if (wgrid.dataGridView1.Columns.Count > 0)
                {
                    wgrid.dataGridView1.Columns[0].HeaderText = "Date";
                    wgrid.dataGridView1.Columns[1].HeaderText = "Machine";
                    wgrid.dataGridView1.Columns[2].HeaderText = "Floor";
                    wgrid.dataGridView1.Columns[3].HeaderText = "Error Code";
                    wgrid.dataGridView1.Columns[4].HeaderText = "Error Description";
                    wgrid.dataGridView1.Columns[5].HeaderText = "Message";
                    wgrid.dataGridView1.Columns[0].DefaultCellStyle.Format = "dd/MM/yyyy hh:mm:ss tt";
                    SetGridColumnSize();
                }

                
            }
            finally
            {
                wgrid.ResumeLayout();
            }
        }

        private void cmbFloor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbFloor.SelectedIndex == 0)
                LoadMachinesToMachineCombo(0);
            else if(cmbFloor.SelectedIndex > 0)
                LoadMachinesToMachineCombo(cmbFloor.SelectedIndex);
        }

        //private void txtExitEES_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    int errorCode = 0;
        //    if (cmbErrorCode.SelectedIndex > 0)
        //    {
        //        int.TryParse(Convert.ToString(cmbErrorCode.SelectedItem), out errorCode);
        //        SetErrorDescription(errorCode);
        //    }
        //}

        private void cmbMachines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string machineName = "";
            if(cmbMachines.Text.Contains("All"))
                machineName = "";
            else
                machineName = cmbMachines.Text;

            LoadErrorToErrorCombo(machineName);
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadAlarmData();
        }

        private void cmbErrorCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int errorCode = 0;
            if (cmbErrorCode.SelectedIndex > -1)
            {
                int.TryParse(Convert.ToString(cmbErrorCode.SelectedItem), out errorCode);
                SetErrorDescription(errorCode);
            }
        }

        private void chkDateEnable_Checked(object sender, RoutedEventArgs e)
        {
            if (chkDateEnable.IsChecked == false)
            {
                ctrlDateTimeFrom.Enabled = false;
                ctrlDateTimeTo.Enabled = false;
            }
            else
            {
                ctrlDateTimeFrom.Enabled = true;
                ctrlDateTimeTo.Enabled = true;
            }
        }

        private void chkDateEnable_Unchecked(object sender, RoutedEventArgs e)
        {
            if (chkDateEnable.IsChecked == false)
            {
                ctrlDateTimeFrom.Enabled = false;
                ctrlDateTimeTo.Enabled = false;
            }
            else
            {
                ctrlDateTimeFrom.Enabled = true;
                ctrlDateTimeTo.Enabled = true;
            }
        }

        void SetGridColumnSize()
        {
            wgrid.dataGridView1.Columns[0].Width = 120;
            wgrid.dataGridView1.Columns[1].Width = 60;
            wgrid.dataGridView1.Columns[2].Width = 45;
            wgrid.dataGridView1.Columns[3].Width = 45;
            wgrid.dataGridView1.Columns[4].Width = 450;
            wgrid.dataGridView1.Columns[5].Width = 220;

        }

        void DeleteSelectedRecords()
        {
            string qry = "";
            foreach (DataGridRow gridrow in wgrid.dataGridView1.SelectedRows)
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                       // qry = "DELETE FROM L2_ALARM  WHERE machine_code ='" + machineName + "'";

                        command.CommandText = qry;
                        command.ExecuteNonQuery();
                        
                    }
                }
            }

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (chkDateEnable.IsEnabled)
            {
                if (MessageBox.Show("Do you want to continue?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    new  Connection().DeleteAlam(ctrlDateTimeFrom.dateTimePicker1.Value, ctrlDateTimeTo.dateTimePicker1.Value);
                    LoadAlarmData();
                }
            }
        }
    }
}
