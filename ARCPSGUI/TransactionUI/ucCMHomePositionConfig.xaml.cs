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
using System.Windows.Forms.Integration;
using System.Data;
using Oracle.DataAccess.Client;
using ARCPSGUI.DB;


namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for ucCMHomePositionConfig.xaml
    /// </summary>
    public partial class ucCMHomePositionConfig : UserControl
    {
        ucWinGrid wgrid = new ucWinGrid();

        public ucCMHomePositionConfig()
        {
            InitializeComponent();

            hostListView.Child = wgrid;
            wgrid.dataGridView1.ReadOnly = false;
        }
        void LoadCMData()
        {
            string query = "";
            try
            {
                query = "select lu_name,Floor, home_aisle from l2_lcm_ucm_master order by floor,lu_name ";
                wgrid.dataGridView1.SuspendLayout();

                DataTable dtResult = new DataTable();

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                        dadapter.Fill(dtResult);

                        wgrid.dataGridView1.DataSource = null;
                        wgrid.dataGridView1.DataSource = dtResult.DefaultView;

                        SetGridColumnCaption();
                       // SetGridColumnSize();
                        SetReadOnlyColumn();
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
            finally
            {
                wgrid.dataGridView1.ResumeLayout();
            }
        }

        void SetGridColumnCaption()
        {
            try
            {
                if (wgrid.dataGridView1.Columns.Count > 0)
                {
                    wgrid.dataGridView1.Columns["lu_name"].HeaderText = "Machine";
                    wgrid.dataGridView1.Columns["Floor"].HeaderText = "Floor";
                    wgrid.dataGridView1.Columns["home_aisle"].HeaderText = "Aisle";
                }
            }
            finally
            {
            }
        }
        void SetGridColumnSize()
        {
            try
            {
                if (wgrid.dataGridView1.Columns.Count > 0)
                {
                    wgrid.dataGridView1.Columns["lu_name"].Width = 200;
                    wgrid.dataGridView1.Columns["Floor"].Width = 300;
                    wgrid.dataGridView1.Columns["home_aisle"].Width = 150;                   
                }
            }
            finally
            {
            }
        }
        void SetReadOnlyColumn()
        {
            try
            {
                wgrid.dataGridView1.Columns["lu_name"].ReadOnly = true;
                wgrid.dataGridView1.Columns["Floor"].ReadOnly = true;
            }
            catch (Exception errMsg)
            {

            }
            finally
            {
                wgrid.dataGridView1.ResumeLayout();
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
