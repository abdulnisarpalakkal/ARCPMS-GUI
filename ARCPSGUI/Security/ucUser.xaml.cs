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
using Oracle.DataAccess.Client;
using ARCPSGUI.Controls;
using System.Data;
using ARCPSGUI.DB;

namespace ARCPSGUI.Security
{
    /// <summary>
    /// Interaction logic for ucUser.xaml
    /// </summary>
    public partial class ucUser : UserControl
    {
        ucWinGrid wgrid = new ucWinGrid();
        DataTable dtGuiList = new DataTable();
       
        public ucUser()
        {
            InitializeComponent();
            WindowsFormsHost.Child = wgrid;
            wgrid.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView1_CellClick);
            
        }

        void dataGridView1_CellClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (Convert.ToString(wgrid.dataGridView1.Columns[e.ColumnIndex].Tag) == "Read")
            {
                wgrid.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !Convert.ToBoolean(wgrid.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            }
            else if (Convert.ToString(wgrid.dataGridView1.Columns[e.ColumnIndex].Tag) == "Write")
            {
                wgrid.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = !Convert.ToBoolean(wgrid.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
            }
        }


        bool Save()
        {
            int userId = 0;
            bool bOk = false;
            if (txtEmployeeName.Visibility == System.Windows.Visibility.Visible && IsValidNewUpdateEntry())
                userId = SaveNewUser();
            else if(IsValidNewUpdateEntry())
            {
                int.TryParse(cmbEmployeeName.SelectedValue.ToString(), out userId);
                UpdateUser(userId);
            }

            if (userId > 0) bOk = SaveAuthenticatedGUI(userId);
            if (userId > 0)
            {
                if (Security.currentUserId == 1)
                    LoadEmployees(Security.currentUserId);
                else
                    LoadEmployees(userId);

                cmbEmployeeName.SelectedValue = userId;
                LoadUserInformation(userId);
                AddGuiListTemplate(userId);

                txtEmployeeName.Visibility = System.Windows.Visibility.Hidden;
                cmbEmployeeName.Visibility = System.Windows.Visibility.Visible;
            }
            return bOk;
        }

        int SaveNewUser()
        {
            int nextUserId = 0;
            try
            {
                string qry = "";

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "SELECT MAX(V_USERID) FROM L2_AUTHENTICATION";
                        command.CommandText = qry;
                        int.TryParse(command.ExecuteScalar().ToString(), out nextUserId);
                        nextUserId += 1;
                    }
                }

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "INSERT INTO L2_AUTHENTICATION (V_USERID,V_USERNAME,V_PASSWORD,V_EMPLOYEE_NAME) VALUES (" +
                            nextUserId + ", '" + txtUserName.Text.Trim() + "','" + txtPwd.Password.Trim() + "','" + txtEmployeeName.Text + "')";
                        command.CommandText = qry;
                        command.ExecuteNonQuery();
                    }
                }
            }
            finally
            {

            }
            return nextUserId;
        }

        bool IsValidNewUpdateEntry()
        {
            bool isValid = false;
            try
            {
                isValid = !string.IsNullOrEmpty(txtUserName.Text);
                isValid &= !string.IsNullOrEmpty(txtPwd.Password);
                isValid &= !string.IsNullOrEmpty(txtRePwd.Password);

                if (!isValid)
                    MessageBox.Show("Invalid user data", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            finally
            { 
            
            }
            return isValid;
        }

        void UpdateUser(int userId)
        {
            try
            {
                string qry = "";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "UPDATE L2_AUTHENTICATION SET V_USERNAME ='" + txtUserName.Text.Trim() + "',V_PASSWORD ='" +
                            txtPwd.Password.Trim() + "',V_EMPLOYEE_NAME ='" + txtEmployeeName.Text.Trim() + "' WHERE V_USERID =" + userId;

                        command.CommandText = qry;
                        command.ExecuteNonQuery();
                    }
                }
            }
            finally
            {

            }

        }

        bool SaveAuthenticatedGUI(int userId)
        {
            bool bOk = false;
            try
            {
                string qry = "";

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "DELETE FROM L2_ACCESSRIGHT_GUI WHERE USER_ID =" + userId;
                        command.CommandText = qry;
                        command.ExecuteNonQuery();
                    }
                }

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        foreach (System.Windows.Forms.DataGridViewRow drow in wgrid.dataGridView1.Rows)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(drow.Cells["AccessKey"].Value)))
                            {
                                if (Convert.ToBoolean(drow.Cells["Read"].Value) || Convert.ToBoolean(drow.Cells["Write"].Value))
                                {
                                    qry = "INSERT INTO L2_ACCESSRIGHT_GUI (ACCESS_KEY,USER_ID,V_READ,V_WRITE) VALUES ('" +
                                          Convert.ToString(drow.Cells["AccessKey"].Value) + "', " + userId + ","
                                          + (Convert.ToBoolean(drow.Cells["Read"].Value) ? 1 : 0)
                                          + "," + (Convert.ToBoolean(drow.Cells["Write"].Value) ? 1 : 0) + ")";

                                    command.CommandText = qry;
                                    command.ExecuteNonQuery();
                                }
                                //else
                                //{
                                //    qry = "DELETE FROM L2_ACCESSRIGHT_GUI  WHERE ACCESS_KEY ='" + Convert.ToString(drow.Cells["AccessKey"].Value) +
                                //          "' AND USER_ID =" + userId;
                                //}
                              
                            }
                        }
                    }
                }
                bOk = true;
            }
            catch (Exception errMsg)
            {
                bOk = false;
            }
            finally
            {

            }
            return bOk;
        }

        void AddGuiListTemplate(int userId)
        {
            try
            {
                DataTable dtUserGuiList = null;


                dtGuiList.Rows.Clear();
                dtGuiList.Columns.Clear();
                if (wgrid.dataGridView1.DataSource != null)
                {
                    wgrid.dataGridView1.Columns.Clear();
                    wgrid.dataGridView1.DataSource = null;
                }

                dtGuiList.Columns.Add("AccessKey");
                dtGuiList.Columns.Add("GUI");

                DataRow drowCustDetail = dtGuiList.NewRow();
                drowCustDetail["AccessKey"] = "guiCustDetail";
                drowCustDetail["GUI"] = "Customer Details";
                dtGuiList.Rows.Add(drowCustDetail);

                DataRow drowDemoMode = dtGuiList.NewRow();
                drowDemoMode["AccessKey"] = "guiDemoMode";
                drowDemoMode["GUI"] = "Demo Mode";
                dtGuiList.Rows.Add(drowDemoMode);

                DataRow drowAllowSlotTypeChange = dtGuiList.NewRow();
                drowAllowSlotTypeChange["AccessKey"] = "guiAllowSlotTypeChange";
                drowAllowSlotTypeChange["GUI"] = "Allow Slot Type Change";
                dtGuiList.Rows.Add(drowAllowSlotTypeChange);

                //DataRow drowAllowConfigAccess = dtGuiList.NewRow();
                //drowAllowConfigAccess["AccessKey"] = "guiAllowConfigAccess";
                //drowAllowConfigAccess["GUI"] = "Allow Config Access";
                //dtGuiList.Rows.Add(drowAllowConfigAccess);


                wgrid.dataGridView1.DataSource = dtGuiList;


                System.Windows.Forms.DataGridViewCheckBoxColumn chkRead = new System.Windows.Forms.DataGridViewCheckBoxColumn();
                chkRead.Name = "Read";
                chkRead.HeaderText = "Read";
                chkRead.Tag = "Read";
                wgrid.dataGridView1.Columns.Add(chkRead);

                System.Windows.Forms.DataGridViewCheckBoxColumn chkWrite = new System.Windows.Forms.DataGridViewCheckBoxColumn();
                chkWrite.Name = "Write";
                chkWrite.HeaderText = "Write";
                chkWrite.Tag = "Write";
                wgrid.dataGridView1.Columns.Add(chkWrite);

                wgrid.dataGridView1.Columns[0].Visible = false;

                if (userId > 0)
                    dtUserGuiList = GetAuthenticatedGUI(userId);

                if (dtUserGuiList != null && dtUserGuiList.Rows.Count > 0)
                {
                    DataRow[] drowlst = null;
                    foreach (System.Windows.Forms.DataGridViewRow dgvrow in wgrid.dataGridView1.Rows)
                    {
                        drowlst = dtUserGuiList.Select("Access_Key = '" + Convert.ToString(dgvrow.Cells["AccessKey"].Value) + "'");
                        if (drowlst != null && drowlst.Length > 0)
                        {
                            dgvrow.Cells["Read"].Value = Convert.ToInt32(drowlst[0].ItemArray.GetValue(2));
                            dgvrow.Cells["Write"].Value = Convert.ToInt32(drowlst[0].ItemArray.GetValue(3));
                        }

                    }

                }
            }
            finally { }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            bool isPwdMatch = txtPwd.Password.Trim() == txtRePwd.Password.Trim();
            if (isPwdMatch)
            {
                bool bOk = Save();
                if (bOk) MessageBox.Show("Saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show("Password is mis match", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtEmployeeName.Visibility = System.Windows.Visibility.Hidden;
            cmbEmployeeName.Visibility = System.Windows.Visibility.Visible;

            cmbEmployeeName.SelectedValue = Security.currentUserId;
            LoadUserInformation(Security.currentUserId);
            AddGuiListTemplate(Security.currentUserId);
        }

        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            txtEmployeeName.Visibility = System.Windows.Visibility.Visible;
            cmbEmployeeName.Visibility = System.Windows.Visibility.Hidden;
            Refresh();
        }

        private void cmbEmployeeName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Int32 userId = 0;

                if (Security.currentUserId == 1)
                    Int32.TryParse(Convert.ToString(cmbEmployeeName.SelectedValue), out userId);
                else
                    userId = Security.currentUserId;
                if (userId > 0)
                {
                    EmployeeSelectionChanged(userId);
                }

            }
            finally { }
        }

        void EmployeeSelectionChanged(int userId)
        {
            try
            {
                if (Security.currentUserId == 1)
                    Int32.TryParse(Convert.ToString(cmbEmployeeName.SelectedValue), out userId);
                else
                    userId = Security.currentUserId;
                if (userId > 0)
                {
                    LoadUserInformation(userId);
                    AddGuiListTemplate(userId);
                }

            }
            finally { }
        }

        DataTable GetAuthenticatedGUI(int userId)
        {
            DataTable dtResult = new DataTable();
            try
            {
                string qry = "";

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "SELECT * FROM L2_ACCESSRIGHT_GUI WHERE USER_ID = " + userId;
                        command.CommandText = qry;
                        command.Connection = con;
                        OracleDataAdapter dAdap = new OracleDataAdapter(command);
                        dAdap.Fill(dtResult);
                    }
                }
            }
            finally
            {

            }
            return dtResult;
        }
        void LoadEmployees(int userId)
        {
            try
            {
                KeyValuePair<Int32, string> kval; // KeyValuePair<int, string>(1, "Admin");

                string qry = "";
                cmbEmployeeName.Items.Clear();

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        if (userId == 1)
                            qry = "SELECT V_USERID,V_EMPLOYEE_NAME FROM L2_AUTHENTICATION ORDER BY V_USERID";
                        else
                            qry = "SELECT V_USERID,V_EMPLOYEE_NAME FROM L2_AUTHENTICATION WHERE V_USERID = " + userId;
                        command.CommandText = qry;
                        using (OracleDataReader dreader = command.ExecuteReader())
                        {
                            while (dreader.Read())
                            {
                                kval = new KeyValuePair<int, string>(Convert.ToInt32(dreader["V_USERID"]), Convert.ToString(dreader["V_EMPLOYEE_NAME"]));
                                cmbEmployeeName.Items.Add(kval);
                            }
                            cmbEmployeeName.DisplayMemberPath = "Value";
                            cmbEmployeeName.SelectedValuePath = "Key";
                        }
                    }
                }
            }
            finally
            {

            }

        }

       

        void LoadUserInformation(int userId)
        {
            try
            {

                string qry = "";

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == System.Data.ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        qry = "SELECT * FROM L2_AUTHENTICATION WHERE V_USERID = " + userId;
                        command.CommandText = qry;
                        using (OracleDataReader dreader = command.ExecuteReader())
                        {
                            while (dreader.Read())
                            {
                                txtEmployeeName.Text = Convert.ToString(dreader["V_EMPLOYEE_NAME"]);
                                txtUserName.Text = Convert.ToString(dreader["V_USERNAME"]);
                                txtPwd.Password = Convert.ToString(dreader["V_PASSWORD"]);
                                txtRePwd.Password = Convert.ToString(dreader["V_PASSWORD"]);
                            }
                        }
                    }
                }
            }
            finally
            {

            }

        }

        void Refresh()
        {
            txtEmployeeName.Text = "";
            txtPwd.Password = "";
            txtRePwd.Password = "";
            txtUserName.Text = "";
            AddGuiListTemplate(0);
        }

        private void txtPwd_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
         
        private void WindowsFormsHost_Loaded(object sender, RoutedEventArgs e)
        {
            DoOnLoad();

        }

        public void DoOnLoad()
        {
            try
            {
                if (Security.currentUserId != 1)
                {
                    btnNew.IsEnabled = false;
                    wgrid.dataGridView1.Enabled = false;
                    btnDelete.IsEnabled = false;
                }

                // AddGuiListTemplate(Security.currentUserId);
                // cmbEmployeeName.SelectedValue = Security.currentUserId;
                //LoadUserInformation(Security.currentUserId);

                cmbEmployeeName.SelectionChanged -= cmbEmployeeName_SelectionChanged;
                LoadEmployees(Security.currentUserId);
                cmbEmployeeName.Tag = Security.currentUserId;
                cmbEmployeeName.SelectedValue = Security.currentUserId;

                LoadUserInformation(Security.currentUserId);
                AddGuiListTemplate(Security.currentUserId);


            }
            finally
            {
                cmbEmployeeName.SelectionChanged += cmbEmployeeName_SelectionChanged;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                int userId = 0;
                bool bOk = false;
                if (cmbEmployeeName.Visibility == System.Windows.Visibility.Visible)
                {
                    int.TryParse(cmbEmployeeName.SelectedValue.ToString(), out userId);
                    if (userId > 0)
                    {
                        if (MessageBox.Show("Do you want to delete the user " + cmbEmployeeName.Text + "?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            string qry = "";
                            using (OracleConnection con = new OracleConnection( Connection.connectionString))
                            {
                                if (con.State == System.Data.ConnectionState.Closed) con.Open();
                                using (OracleCommand command = con.CreateCommand())
                                {
                                    qry = "DELETE FROM L2_AUTHENTICATION WHERE V_USERID =" + userId;
                                    command.CommandText = qry;
                                    command.ExecuteNonQuery();
                                }
                            }

                            using (OracleConnection con = new OracleConnection( Connection.connectionString))
                            {
                                if (con.State == System.Data.ConnectionState.Closed) con.Open();
                                using (OracleCommand command = con.CreateCommand())
                                {
                                    qry = "DELETE FROM L2_ACCESSRIGHT_GUI WHERE USER_ID =" + userId;
                                    command.CommandText = qry;
                                    command.ExecuteNonQuery();
                                }
                            }
                            LoadEmployees(Security.currentUserId);
                            cmbEmployeeName.SelectedValue = userId;
                            LoadUserInformation(userId);
                            AddGuiListTemplate(userId);

                            txtEmployeeName.Visibility = System.Windows.Visibility.Hidden;
                            cmbEmployeeName.Visibility = System.Windows.Visibility.Visible;
                        }
                    }
                }
            }
            finally
            {

            }
        }
    }


}
