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
using WindowsFormsApplication10;
using System.Threading.Tasks;
using System.Configuration;
using System.Xml;
using ARCPSGUI.DB;
using System.IO;


namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for uiCurrentTransaction.xaml
    /// </summary>
    public partial class uiCurrentTransaction : UserControl
    {
        ucWinGrid wgrid = new ucWinGrid();
        ucctrlTime ctrlDateTimeFrom = new ucctrlTime();
        ucctrlTime ctrlDateTimeTo = new ucctrlTime();
        DataSet dsRpt = null;

         Connection dbpm = new  Connection();

        string exitFilePath = "";
        public uiCurrentTransaction()
        {
            InitializeComponent();
         
         wgrid.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView1_CellClick);
         
        }

        void dataGridView1_CellClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            try
            {
                if (Convert.ToString(wgrid.dataGridView1.Columns[e.ColumnIndex].Tag) == "photo")
                {
                    int customerPrimaryKeyID = 0;
                    if (wgrid.dataGridView1.SelectedRows.Count > 0)
                    {
                        string northImg = "";
                        string southImg = "";

                        int.TryParse(Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["CUSTOMER_PK_ID"].Value), out customerPrimaryKeyID);

                        GetPhotoPath(customerPrimaryKeyID, out northImg, out southImg);

                        frmShowPhoto showPhoto = new frmShowPhoto();

                        if (!string.IsNullOrEmpty(northImg))
                            showPhoto.NorthPhotoPath = northImg;
                        if (!string.IsNullOrEmpty(southImg) ) 
                            showPhoto.SouthPhotoPath = southImg;

                        showPhoto.lblCarRegNo.Content = wgrid.dataGridView1.SelectedRows[0].Cells["PLATE NO#"].Value.ToString();
                        showPhoto.lblNorthEES.Content = "North -" + wgrid.dataGridView1.SelectedRows[0].Cells["GATE"].Value.ToString();
                        showPhoto.lblSouthEES.Content = "South -" + wgrid.dataGridView1.SelectedRows[0].Cells["GATE"].Value.ToString();
                        showPhoto.BringIntoView();
                        showPhoto.Show();

                    }
                }
            }
            catch(FileNotFoundException ex)
            {
                Console.WriteLine("file not found");
            }
        }

        void dataGridView1_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            //int cardID = 0;
            //if (wgrid.dataGridView1.SelectedRows.Count > 0 && wgrid.dataGridView1.SelectedCells[0].ColumnIndex == 13 )
            //{
            //   int.TryParse( wgrid.dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString(),out cardID);
            //}
            //ucDisplayEESPhoto1.Margin =
            //vbViewImage.Visibility = System.Windows.Visibility.Visible;
            //ucDisplayEESPhoto1.Visibility = System.Windows.Visibility.Visible;
        }

  
        void dataGridView1_MouseHover(object sender, EventArgs e)
        {
            if (wgrid.dataGridView1.SelectedRows.Count > 0 && wgrid.dataGridView1.SelectedRows[0].Cells.Count > 0)
            {
                System.Windows.MessageBox.Show(wgrid.dataGridView1.SelectedRows[0].Cells["ID"].Value.ToString());
            }
        }


        
        public void DoOnLoad()
        {
            hostListView.Child = wgrid;
            hostdatefrom.Child = ctrlDateTimeFrom;
            hostdateto.Child = ctrlDateTimeTo;
            chkDateEnable.IsChecked = false;
            ctrlDateTimeFrom.Enabled = false;
            ctrlDateTimeTo.Enabled = false;

            chkDateEnable.IsChecked = true;
            ctrlDateTimeFrom.Enabled = true;
            ctrlDateTimeTo.Enabled = true;

            ctrlDateTimeFrom.dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00");
            ctrlDateTimeTo.dateTimePicker1.Value = DateTime.Now;

          
        }

        void GetCurrentParks()
        {
            try
            {
                string query = "SELECT * FROM CURRENT_PARKS_VIEW WHERE S_NO > 0 ";
                DateTime dtFrom;
                DateTime dtTo;

                string filter = "";
                if (txtGate.Text != "All" && string.IsNullOrEmpty(txtGate.Text) == false)
                {
                    filter += " AND GATE = '" + txtGate.Text + "'";
                }

                if (string.IsNullOrEmpty(txtName.Text) == false)
                    filter += " AND UPPER(" + @"""Name"" ) LIKE UPPER('%" + txtName.Text.Trim() + "%')";

                if (string.IsNullOrEmpty(txtCarID.Text) == false)
                    filter += " AND UPPER(" + @"""PLATE NO#"") LIKE UPPER('%" + txtCarID.Text + "%')";

                if (string.IsNullOrEmpty(txtFloor.Text) == false)
                    filter += " AND " + @"""LEVEL"" = " + txtFloor.Text;

                if (string.IsNullOrEmpty(txtAisle.Text) == false)
                    filter += " AND " + @"""AISLE""=" + txtAisle.Text;

                if (string.IsNullOrEmpty(txtRow.Text) == false)
                    filter += " AND " + @"""ROW""=" + txtRow.Text;

                if (string.IsNullOrEmpty(txtCardId.Text) == false)
                    filter += " AND " + @"""ID"" LIKE '%" + txtCardId.Text + "%'";

                if (chkDateEnable.IsChecked == true)
                {
                    dtFrom = ctrlDateTimeFrom.dateTimePicker1.Value;
                    dtTo = ctrlDateTimeTo.dateTimePicker1.Value;

                    filter += " AND TIME BETWEEN '" + dtFrom.ToString("dd/MMM/yyyy hh:mm:ss tt") + "' AND '" + dtTo.ToString("dd/MMM/yyyy hh:mm:ss tt") + "'";

                }
                       
                query += filter + " order by s_no";
                DataTable dt = new DataTable();
                dt.TableName = "CURRENTPARKS";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                        dadapter.Fill(dt);
                        this.dsRpt = new DataSet();
                        this.dsRpt.Tables.Add(dt);
                    }
                }
               

                wgrid.dataGridView1.SuspendLayout();
                wgrid.dataGridView1.Columns.Clear();
                wgrid.dataGridView1.DataSource = dt;
                System.Windows.Forms.DataGridViewButtonColumn btnCol = new System.Windows.Forms.DataGridViewButtonColumn();
                btnCol.Name = "Photo";
                btnCol.Text = "Photo";
                btnCol.Tag = "photo";
                wgrid.dataGridView1.Columns.Add(btnCol);
                
                SetGridColumnSize();
                ChangeDisableRecordBackColor();
                SetGridHeaderConfig();
                if (dt != null && dt.Rows.Count > 0)
                {
                    lblCurrentParks.Content = "Current Parks = " + dt.Rows.Count;
                }
                else
                    lblCurrentParks.Content = "Current Parks = 0";

                wgrid.dataGridView1.Columns[2].DefaultCellStyle.Format = "dd/MMM/yyyy hh:mm:ss tt";
            }
            catch (Exception errMsg)
            {

            }
            finally
            {
                wgrid.dataGridView1.ResumeLayout();
            }
           
        }

        void ChangeDisableRecordBackColor()
        {
            try
            {
                if (wgrid.dataGridView1.DataSource != null)
                {
                    for (int i = 0; i <= wgrid.dataGridView1.Rows.Count - 1; i++)
                    {
                        if (Convert.ToString(wgrid.dataGridView1.Rows[i].Cells["STATUS"].Value) == "DISABLED")
                            wgrid.dataGridView1.Rows[i].DefaultCellStyle.ForeColor = System.Drawing.Color.Red;
                        else
                            wgrid.dataGridView1.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.LightGray;
                    }
                }
            }
            finally { }
        }

        public void CreateOracleListner()
        {
            string count = "";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == System.Data.ConnectionState.Closed) con.Open();
                using (OracleCommand command = con.CreateCommand())
                {
                    string sql = "select count(*) FROM L2_PROC_SNAPSHOT";
                    command.CommandText = sql;

                    OracleDependency dep = new OracleDependency(command);
                    // command.AddRowid = true;
                    dep.QueryBasedNotification = true;
                    dep.OnChange += new OnChangeEventHandler(dep_OnChange);
                    command.Notification.IsNotifiedOnce = false;
                    count = Convert.ToString(command.ExecuteScalar());
                }
            }
        }

        void dep_OnChange(object sender, OracleNotificationEventArgs eventArgs)
        {
            this.Dispatcher.Invoke(new Action(() =>  GetCurrentParks()));
             

        }

        public void CreateOracleListnerForCustomerTable()
        {
            string count = "";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == System.Data.ConnectionState.Closed) con.Open();
                using (OracleCommand command = con.CreateCommand())
                {
                    string sql = "select count(*) FROM L2_CUSTOMERS";
                    command.CommandText = sql;

                    OracleDependency dep = new OracleDependency(command);
                    // command.AddRowid = true;
                    dep.QueryBasedNotification = true;
                    dep.OnChange += new OnChangeEventHandler(dep_OnChange);
                    command.Notification.IsNotifiedOnce = false;
                    count = Convert.ToString(command.ExecuteScalar());
                }
            }
        }

        private void btnRetrieveSelectedCar_Click(object sender, RoutedEventArgs e)
        {
            //RetreiveXMLGeneration();


            bool isDisabledSlot = false;
            string cardId = "";
            if (wgrid.dataGridView1.SelectedRows.Count > 0)
                {
                    cardId = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["ID"].Value);
                    isDisabledSlot = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["STATUS"].Value) == "DISABLED";
                }
                string message = "";

                if (isDisabledSlot)
                {
                    message = "If you want to retrieve the car, you should manually enable the slot.";
                    MessageBox.Show(message, "Confirmaton", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    message = "Do you want to retrieve the car?";

                if (!isDisabledSlot && MessageBox.Show(message, "Confirmaton", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    InsertQueue(cardId,0);
                }
               GetCurrentParks();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentParks();
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

        private void RetreiveXMLGeneration()
        {
          
            bool isDisabledSlot = false;
            string cardId = "";
            try
            {
                if (wgrid.dataGridView1.SelectedRows.Count > 0)
                {
                    cardId = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["ID"].Value);
                    isDisabledSlot = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["STATUS"].Value) == "DISABLED";
                }
                string message = "";

                if (isDisabledSlot)
                {
                    message = "If you want to retrieve the car, you should manually enable the slot.";
                    MessageBox.Show(message, "Confirmaton", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                    message = "Do you want to retrieve the car?";

                if (!isDisabledSlot && MessageBox.Show(message, "Confirmaton", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    DataTable dt = new DataTable();

                    dt.TableName = "OPCTagNode";
                    dt.Columns.Add("ExID", typeof(int));
                    dt.Columns.Add("Location", typeof(string));
                    dt.Columns.Add("RDT", typeof(string));
                    dt.Columns.Add("Card_ID", typeof(string));

                    DataRow drow = dt.NewRow();
                    drow["ExID"] = 1;
                    drow["Location"] = "RKiosk1";
                    drow["Card_ID"] = cardId;
                    drow["RDT"] = System.DateTime.Now.ToFileTime().ToString();

                    dt.Rows.Add(drow);

                    DataSet ds = new DataSet();
                    ds.DataSetName = "OPCXMLData";

                    ds.Tables.Add(dt);
                    string a = System.DateTime.Now.ToFileTime().ToString();

                    //string filePath = ConfigurationSettings.AppSettings["ExitKioskXmlPath"];
                    ds.WriteXml(exitFilePath + @"\" + a + cardId.ToString() + ".xml");
                }
            }
            finally {
                GetCurrentParks();
            }

        }
        public void InsertQueue(string cardId,int requestType)
        {
            var queueId = 0;
            try
            {
               
                    using (OracleConnection con = new OracleConnection( Connection.connectionString))
                    {
                        if (con.State == ConnectionState.Closed) con.Open();
                        using (OracleCommand command = con.CreateCommand())
                        {

                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "INSERT_QUEUE";
                            command.Parameters.Add("V_QUEUE_ID", OracleDbType.Int64, queueId, ParameterDirection.Output);
                            command.Parameters.Add("cust_id", OracleDbType.Varchar2, 50, cardId, ParameterDirection.Input);
                            command.Parameters.Add("ARG_EES_ID", OracleDbType.Int64, 0, ParameterDirection.Input);
                            command.Parameters.Add("KIOSK_ID", OracleDbType.Varchar2, "", ParameterDirection.Input);
                            command.Parameters.Add("CAR_ID", OracleDbType.Varchar2, "", ParameterDirection.Input);
                            command.Parameters.Add("ARG_REQUEST_TYPE", OracleDbType.Int64, requestType, ParameterDirection.Input);
                            command.Parameters.Add("PRIORITY", OracleDbType.Int64, 1, ParameterDirection.Input); //TODO: NOTIFY
                            command.Parameters.Add("high_car", OracleDbType.Int64, 0, ParameterDirection.Input);
                            command.Parameters.Add("STAT", OracleDbType.Int64, 0, ParameterDirection.Input);
                            command.Parameters.Add("PATRON_NAME", OracleDbType.NVarchar2, 500, "", ParameterDirection.Input);
                            command.Parameters.Add("arg_need_wash", OracleDbType.Int64, 0, ParameterDirection.Input);
                            command.Parameters.Add("RETRIEVAL_TYPE", OracleDbType.Int64, 0, ParameterDirection.Input);
                            command.Parameters.Add("arg_rot_staus", OracleDbType.Char, 0, ParameterDirection.Input);

                            command.ExecuteNonQuery();
                            Int32.TryParse(Convert.ToString(command.Parameters["V_QUEUE_ID"].Value), out queueId);
                        }
                    }
                
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);

            }

        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetCurrentParks();
        }

        void SetGridColumnSize()
        {
          
            wgrid.dataGridView1.Columns[0].Width = 40;
            wgrid.dataGridView1.Columns[1].Width = 60;
            wgrid.dataGridView1.Columns[2].Width = 145;

            wgrid.dataGridView1.Columns[3].Width = 60;
            wgrid.dataGridView1.Columns[4].Width = 90;
            wgrid.dataGridView1.Columns[5].Width = 50;

            wgrid.dataGridView1.Columns[6].Width = 45;
            wgrid.dataGridView1.Columns[7].Width = 40;
            wgrid.dataGridView1.Columns[8].Width = 50;

            wgrid.dataGridView1.Columns[9].Width = 70;
            wgrid.dataGridView1.Columns[10].Width = 60;
            wgrid.dataGridView1.Columns[11].Width = 65;

            wgrid.dataGridView1.Columns[12].Width = 90;
            wgrid.dataGridView1.Columns[13].Width = 65;

            wgrid.dataGridView1.Columns[15].Visible = false;

        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtAisle.Text = "";
            txtFloor.Text = "";
            txtCarID.Text = "";
            txtName.Text = "";
            txtGate.SelectedIndex = 0;
            GetCurrentParks();
        }

        void GetPhotoPath(int customerPrimaryKeyID, out string northImgPath, out string southImgPath)
        {
            northImgPath = "";
            southImgPath = "";
            string query = "SELECT * FROM L2_CUSTOMERS WHERE CUSTOMER_ID =" + customerPrimaryKeyID;
            DataTable dt = new DataTable();
            dt.TableName = "L2_CUSTOMERS";
            using (OracleConnection con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    command.Connection = con;
                    using (OracleDataReader dreader = command.ExecuteReader())
                    {
                        if (dreader.HasRows)
                        {
                            northImgPath = Convert.ToString(dreader["ENTRY_NORTH_IMG"]);
                            southImgPath = Convert.ToString(dreader["ENTRY_SOUTH_IMG"]);
                        }
                    }
                }
            }
        }

        private void txtCarID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnFilter_Click(sender, e);
        }

        private void txtFloor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnFilter_Click(sender, e);
        }

        private void txtAisle_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnFilter_Click(sender, e);
        }

        private void txtRow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnFilter_Click(sender, e);
        }

        void DisplayReport()
        {
            frmRptView rptView = new frmRptView();
            rptView.Show();
            rptView.LoadCurrentParksReport(this.dsRpt);
        }

        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            DisplayReport();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                GetCurrentParks();
                exitFilePath = dbpm.GetConfigValue("ExitXMLPath", "", "Path");
                SetGridHeaderConfig();
            }
            finally { }

        }

        private void txtCarID_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetCurrentParks();
        }

        private void btnDeleteSelectedCar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = 0;
                int floor = 0;
                int row = 0;
                int aisle = 0;

                if (MessageBox.Show("Do you want to delete", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (wgrid.dataGridView1.SelectedRows.Count > 0)
                    {
                        int selectedRowIndex = 0;
                        selectedRowIndex = wgrid.dataGridView1.SelectedRows[0].Index;

                        int.TryParse(Convert.ToString(wgrid.dataGridView1.Rows[selectedRowIndex].Cells["CUSTOMER_PK_ID"].Value), out id);
                        int.TryParse(Convert.ToString(wgrid.dataGridView1.Rows[selectedRowIndex].Cells["LEVEL"].Value), out floor);
                        int.TryParse(Convert.ToString(wgrid.dataGridView1.Rows[selectedRowIndex].Cells["ROW"].Value), out row);
                        int.TryParse(Convert.ToString(wgrid.dataGridView1.Rows[selectedRowIndex].Cells["AISLE"].Value), out aisle);

                        using (OracleConnection con = new OracleConnection( Connection.connectionString))
                        {
                            if (con.State == System.Data.ConnectionState.Closed) con.Open();

                            using (OracleCommand command = con.CreateCommand())
                            {
                                command.Connection = con;
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = "DELETE_FROM_CURRENT_PARKS";
                                command.Parameters.Add("par_floor", OracleDbType.Int32, floor, ParameterDirection.Input);
                                command.Parameters.Add("par_aisle", OracleDbType.Int32, aisle, ParameterDirection.Input);
                                command.Parameters.Add("par_row", OracleDbType.Int32, row, ParameterDirection.Input);
                                command.Parameters.Add("par_user_name", OracleDbType.Varchar2, Security.Security.currentUserId, ParameterDirection.Input);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show("Error occured while deleting records", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                GetCurrentParks();
            }
        }

        private void txtCardId_KeyUp(object sender, KeyEventArgs e)
        {
            GetCurrentParks();
        }

        void SetGridHeaderConfig()
        {
            wgrid.dataGridView1.ColumnHeadersHeight = 30;
            //wgrid.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightSteelBlue;
            wgrid.dataGridView1.AdvancedRowHeadersBorderStyle.All = System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.Outset;
            wgrid.dataGridView1.AllowUserToOrderColumns = true;
            wgrid.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            //wgrid.dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
        }
    }
}
