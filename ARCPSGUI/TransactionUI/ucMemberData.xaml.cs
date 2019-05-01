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
    /// Interaction logic for ucMemberData.xaml
    /// </summary>
    public partial class ucMemberData : UserControl
    {
        ucWinGrid wgrid = new ucWinGrid();
        public ucMemberData()
        {
            InitializeComponent();
            WindowsFormsHost.Child = wgrid;
            wgrid.dataGridView1.ReadOnly = false;
            wgrid.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
        }

        void dataGridView1_CellValueChanged(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            try
            {
                wgrid.dataGridView1.Rows[e.RowIndex].Tag = 1;
            }
            catch (Exception errMsg)
            { 
            
            }
        }

        void LoadMemberDataData()
        {
            string query = "";
            try
            {
                query = "SELECT CARD_ID,MEMBER_NAME,PLATE_NO, MOBILE_NO,MEMBERSHIP_REG_DATE,MEMBERSHIP_START_DATE,MEMBERSHIP_END_DATE "
                          + " FROM rpmsadmin.L2_MEMBERS WHERE  CARD_ID > 0 ";

                string filter = "";

                if (!string.IsNullOrEmpty(txtName.Text.Trim()))
                    filter += " AND LTRIM(RTRIM(UPPER(MEMBER_NAME))) LIKE UPPER('%" + txtName.Text.Trim() + "%')";
                else if (!string.IsNullOrEmpty(txtCardId.Text.Trim()))
                    filter += " AND LTRIM(RTRIM(UPPER(CARD_ID))) LIKE UPPER('%" + txtCardId.Text.Trim() + "%')";
                else if (!string.IsNullOrEmpty(txtMobile.Text.Trim()))
                    filter += " AND LTRIM(RTRIM(UPPER(MOBILE_NO))) LIKE UPPER('%" + txtMobile.Text.Trim() + "%')";
                else if (!string.IsNullOrEmpty(txtPlateNo.Text.Trim()))
                    filter += " AND LTRIM(RTRIM(UPPER(PLATE_NO))) LIKE UPPER('%" + txtPlateNo.Text.Trim() + "%')";

                query += filter + " ORDER BY MEMBER_NAME ";
                wgrid.dataGridView1.SuspendLayout();

                DataTable dtMember = new DataTable();

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                        dadapter.Fill(dtMember);

                        wgrid.dataGridView1.DataSource = null;
                        wgrid.dataGridView1.DataSource = dtMember.DefaultView;

                        SetGridColumnCaption();
                        SetGridColumnSize();
                        SetReadOnlyColumn();
                        SetGridEditColumnHeighLight();
                        if (dtMember != null && dtMember.Rows.Count > 0)
                        {
                            lblTotalRecords.Content = "Total Records = " + dtMember.Rows.Count;
                        }
                        else
                            lblTotalRecords.Content = "Total Records = 0";
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
                    wgrid.dataGridView1.Columns["CARD_ID"].HeaderText = "Card ID";
                    wgrid.dataGridView1.Columns["MEMBER_NAME"].HeaderText = "Name";
                    wgrid.dataGridView1.Columns["PLATE_NO"].HeaderText = "Plate#";
                    wgrid.dataGridView1.Columns["MOBILE_NO"].HeaderText = "Mobile";
                    wgrid.dataGridView1.Columns["MEMBERSHIP_REG_DATE"].HeaderText = "Registered Date";
                    wgrid.dataGridView1.Columns["MEMBERSHIP_START_DATE"].HeaderText = "Membership Start";
                    wgrid.dataGridView1.Columns["MEMBERSHIP_END_DATE"].HeaderText = "Membership End";

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
                    wgrid.dataGridView1.Columns["CARD_ID"].Width = 200;
                    wgrid.dataGridView1.Columns["MEMBER_NAME"].Width = 300;
                    wgrid.dataGridView1.Columns["PLATE_NO"].Width = 150;
                    wgrid.dataGridView1.Columns["MOBILE_NO"].Width = 150;
                    wgrid.dataGridView1.Columns["MEMBERSHIP_REG_DATE"].Width = 125;
                    wgrid.dataGridView1.Columns["MEMBERSHIP_START_DATE"].Width = 125;
                    wgrid.dataGridView1.Columns["MEMBERSHIP_END_DATE"].Width = 125;

                }
            }
            finally
            {
            }
        }

        void SetReadOnlyColumn()
        { 
        try{
            wgrid.dataGridView1.Columns["CARD_ID"].ReadOnly = true;
            wgrid.dataGridView1.Columns["MEMBER_NAME"].ReadOnly = true;
            wgrid.dataGridView1.Columns["MOBILE_NO"].ReadOnly = true;

            wgrid.dataGridView1.Columns["MEMBERSHIP_REG_DATE"].ReadOnly = true;
            wgrid.dataGridView1.Columns["MEMBERSHIP_START_DATE"].ReadOnly = true;
            wgrid.dataGridView1.Columns["MEMBERSHIP_END_DATE"].ReadOnly = true;
            
        }
        catch (Exception errMsg)
        {

        }
        finally
        {
           
        }
        }

        void SetGridEditColumnHeighLight()
        {
            try
            {
                wgrid.dataGridView1.Columns["PLATE_NO"].DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;

            }
            catch (Exception errMsg)
            {

            }
            finally
            {

            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LoadMemberDataData();
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtMobile.Text = "";
            txtCardId.Text = "";
            txtPlateNo.Text = "";
            LoadMemberDataData();
        }

        private void txtMobile_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtName.Text = "";
            txtCardId.Text = "";
            txtPlateNo.Text = "";
            LoadMemberDataData();
        }

        private void txtCardId_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtName.Text = "";
            txtMobile.Text = "";
            txtPlateNo.Text = "";
            LoadMemberDataData();
        }

        private void txtPlateNo_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtName.Text = "";
            txtMobile.Text = "";
            txtCardId.Text = "";
            LoadMemberDataData();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you want to save records?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    int needToSave = 0;
                    string plateNumber = "";
                    string cardId = "";
                    for (int i = 0; i <= wgrid.dataGridView1.Rows.Count - 1; i++)
                    {
                        int.TryParse(Convert.ToString(wgrid.dataGridView1.Rows[i].Tag), out needToSave);
                        if (needToSave == 1)
                        {
                            plateNumber = Convert.ToString(wgrid.dataGridView1.Rows[i].Cells["PLATE_NO"].Value);
                            cardId = Convert.ToString(wgrid.dataGridView1.Rows[i].Cells["CARD_ID"].Value);
                            if (string.IsNullOrEmpty(cardId) == false)
                            {
                                Save(cardId, plateNumber);
                            }
                            wgrid.dataGridView1.Rows[i].Tag = 0;
                        }
                        plateNumber = "";
                        needToSave = 0;
                    }
                    MessageBox.Show("Successfuly saved","Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
        }

        void Save(string cardId, string plateNumber)
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();
                 
                    string sql = "update rpmsadmin.L2_MEMBERS  set PLATE_NO = '" + plateNumber
                        + "' where CARD_ID = '" + cardId + "'";
                 
                    command.CommandText = sql;
                    command.CommandType = CommandType.Text;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }

        }
    }
}
