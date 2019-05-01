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
using System.Data;
using Oracle.DataAccess.Client;
using ARCPSGUI.DB;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for ucErrorMaster.xaml
    /// </summary>
    public partial class ucErrorMaster : UserControl
    {
        ucWinGrid wgrid = new ucWinGrid();
        static DataTable dtMachines = new DataTable();
        static DataTable dtErrorMaster = new DataTable();

        public ucErrorMaster()
        {
            InitializeComponent();
            DoOnLoad();
        }

        public void DoOnLoad()
        {
            WindowsFormsHost.Child = wgrid;
         
            if (dtMachines.Rows.Count < 1) LoadMachines();
            if (dtErrorMaster.Rows.Count < 1) LoadErrorCodes();
            LoadMachinesToMachineCombo();
            LoadErrorData();
            cmbMachines.SelectedIndex = 0;
            cmbErrorCode.SelectedIndex = 0;
        }

        void LoadMachines()
        {
            string query = "";

            query = "select distinct Machine_model from L2_ERROR_MASTER order by MACHINE_MODEL";

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

        void LoadMachinesToMachineCombo()
        {
            try
            {
                cmbMachines.BeginInit();
                cmbMachines.Items.Clear();
                if (dtMachines != null && dtMachines.Rows.Count > 0)
                {
                        foreach (DataRow drow in dtMachines.Rows)
                        {
                            cmbMachines.Items.Add(drow[0]);
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
                        drowlist = dtErrorMaster.Select("machine_model ='" + machinename + "'","ERROR_CODE");
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

            //  DataRow drow = this.dtErrorMaster.Select("ERROR_CODE =" + errorCode).First();
            DataRow[] drow = dtErrorMaster.Select("ERROR_CODE =" + errorCode);
            if (drow != null && drow.Length > 0)
            {
                txtErrorDesc.Text = Convert.ToString(drow[0].ItemArray.GetValue(3));
            }
            //txtErrorDesc.Text = 
        }

        void LoadErrorData()
        {
            string query = "";
            
            string machine = "";
            int errorCode = 0;
          
            try
            {
                query = "SELECT * FROM L2_ERROR_MASTER WHERE MACHINE_MODEL IS NOT NULL";
                wgrid.dataGridView1.SuspendLayout();

                if (cmbMachines.SelectedIndex > -1 && cmbMachines.SelectedItem.ToString().Contains("All"))
                    machine = "";
                else if (cmbMachines.SelectedIndex > -1 && string.IsNullOrEmpty(cmbMachines.SelectedItem.ToString()) == false)
                {
                    machine = cmbMachines.SelectedItem.ToString().Trim();
                    query += " AND MACHINE_MODEL = '" + machine + "'";
                }
                if (cmbErrorCode.SelectedIndex > -1 && cmbErrorCode.SelectedItem.ToString().Contains("All"))
                    errorCode = 0;
                else if (cmbErrorCode.SelectedIndex > -1 && string.IsNullOrEmpty(cmbErrorCode.SelectedItem.ToString()) == false)
                {
                    Int32.TryParse(cmbErrorCode.SelectedItem.ToString(), out errorCode);
                    query += " AND ERROR_CODE = " + errorCode;
                }
                DataTable dtErrorMaster = new DataTable();
            
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                        dadapter.Fill(dtErrorMaster);

                        wgrid.dataGridView1.DataSource = null;
                        wgrid.dataGridView1.DataSource = dtErrorMaster.DefaultView;
                    }
                }

                if (wgrid.dataGridView1.Columns.Count > 0)
                {
                    wgrid.dataGridView1.Columns[0].HeaderText = "Item";
                    wgrid.dataGridView1.Columns[1].HeaderText = "Machine Model";
                    wgrid.dataGridView1.Columns[2].HeaderText = "Error Code";
                    wgrid.dataGridView1.Columns[3].HeaderText = "Error Description";
                    SetGridColumnSize();
                }

            }
            finally
            { 
              wgrid.dataGridView1.ResumeLayout();
            }
        }    

        private void cmbMachines_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string machineName = "";
            if (cmbMachines.SelectedItem.ToString().Contains("All"))
                machineName = "";
            else
                machineName = cmbMachines.SelectedItem.ToString();
         
            LoadErrorToErrorCombo(machineName);
            cmbErrorCode.SelectedIndex = 0;
            LoadErrorData();
            
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            LoadErrorData();
        }

        private void cmbErrorCode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int errorCode = 0;
            if (cmbErrorCode.SelectedIndex > -1)
            {
                int.TryParse(Convert.ToString(cmbErrorCode.SelectedItem), out errorCode);
                SetErrorDescription(errorCode);
                LoadErrorData();
            }
        }

     
        void SetGridColumnSize()
        {
            wgrid.dataGridView1.Columns[0].Width = 65;
            wgrid.dataGridView1.Columns[1].Width = 85;
            wgrid.dataGridView1.Columns[2].Width = 65;
            wgrid.dataGridView1.Columns[3].Width = 650;
          //  wgrid.dataGridView1.Columns[4].Width = 450;
           // wgrid.dataGridView1.Columns[5].Width = 220;

        }

    }
}
