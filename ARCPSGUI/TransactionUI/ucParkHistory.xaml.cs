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
using WindowsFormsApplication10;
using ARCPSGUI.Controls;
using System.Data;
using Oracle.DataAccess.Client;
using System.Xml;
using System.Xml.Xsl;
using System.IO;
using ExcelLibrary.SpreadSheet;
using System.IO;
using System.Xml.Linq;
using System.Threading.Tasks;
using ARCPSGUI.DB;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for ucHistory.xaml
    /// </summary>
    public partial class ucParkHistory : UserControl
    {
        ucWinGrid wgrid = new ucWinGrid();
        ucctrlTime ctrlDateTimeFrom = new ucctrlTime();
        ucctrlTime ctrlDateTimeTo = new ucctrlTime();
        event EventHandler AfterRecordFetch = null;

        public ucParkHistory()
        {
            InitializeComponent();
        
            wgrid.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView1_CellClick);

            AfterRecordFetch += new EventHandler(ucHistory_AfterRecordFetch);
           // wgrid.dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.Linen;
          
        }

        void ucHistory_AfterRecordFetch(object sender, EventArgs e)
        {
            try
            {
                DataTable dtResult = sender as DataTable;
               
                    if (wgrid.dataGridView1.IsHandleCreated == false) return;
                    wgrid.BeginInvoke(new Action(() =>
                    {
                        if (dtResult != null && dtResult.Rows.Count > 0)
                        {
                            try
                            {

                                wgrid.dataGridView1.SuspendLayout();
                                wgrid.dataGridView1.Columns.Clear();
                                wgrid.dataGridView1.DataSource = dtResult;

                                System.Windows.Forms.DataGridViewButtonColumn btnCol = new System.Windows.Forms.DataGridViewButtonColumn();
                                btnCol.Name = "Photo";
                                btnCol.Text = "Photo";
                                btnCol.Tag = "photo";
                                wgrid.dataGridView1.Columns.Insert(19, btnCol);

                                //For Note
                                System.Windows.Forms.DataGridViewButtonColumn btnNote = new System.Windows.Forms.DataGridViewButtonColumn();
                                btnNote.Name = "Note";
                                btnNote.Text = "Note";
                                btnNote.Tag = "Note";
                                wgrid.dataGridView1.Columns.Insert(20, btnNote);


                                wgrid.dataGridView1.Columns["ENTRY TIME"].DefaultCellStyle.Format = "dd/MMM/yyyy hh:mm:ss tt";
                                wgrid.dataGridView1.Columns["EXIT TIME"].DefaultCellStyle.Format = "dd/MMM/yyyy hh:mm:ss tt";

                            }
                            catch (Exception errMsg)
                            {
                                MessageBox.Show(errMsg.Message);
                            }
                            finally
                            {
                                wgrid.dataGridView1.ResumeLayout();
                                FormatGrid();
                            }
                        }
                        else
                        {
                          //  wgrid.dataGridView1.Rows.Clear();
                            wgrid.dataGridView1.DataSource = dtResult;
                        }
                    }));

                if (dtResult != null && dtResult.Rows.Count > 0)
                    lblTotalRecords.Dispatcher.BeginInvoke(new Action(() => lblTotalRecords.Content = "Total Records =" + dtResult.Rows.Count));
                else
                   lblTotalRecords.Dispatcher.BeginInvoke(new Action(() => lblTotalRecords.Content = "Total Records = 0"));
            }
            catch (Exception errMsg)
            {
            }
            finally
            { 
            
            }
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
                        string entryEESNorthImg = "";
                        string entryEESsouthImg = "";
                        string exitEESnorthImg = "";
                        string exitEESsouthImg = "";

                        int.TryParse(Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["PARK_PK_ID"].Value), out customerPrimaryKeyID);

                        GetPhotoPath(customerPrimaryKeyID, out entryEESNorthImg, out entryEESsouthImg, out exitEESnorthImg, out exitEESsouthImg);
                      
                        frmShowHistoryPhoto showPhoto = new frmShowHistoryPhoto();
                        showPhoto.EntryEESNorthPhotoPath = entryEESNorthImg;
                        showPhoto.EntryEESSouthPhotoPath = entryEESsouthImg;
                        showPhoto.ExitEESNorthPhotoPath = exitEESnorthImg;
                        showPhoto.ExitEESSouthPhotoPath = exitEESsouthImg;
                        showPhoto.lblCarRegNo.Content = wgrid.dataGridView1.SelectedRows[0].Cells["PLATE NO#"].Value.ToString();
                        showPhoto.lblEntryEES.Content = wgrid.dataGridView1.SelectedRows[0].Cells["Entry EES"].Value.ToString();
                        showPhoto.lblExitEES.Content = wgrid.dataGridView1.SelectedRows[0].Cells["Exit EES"].Value.ToString();
                        showPhoto.ShowDialog();                      
                    }
                }
                else if (Convert.ToString(wgrid.dataGridView1.Columns[e.ColumnIndex].Tag) == "Note")
                {
                    frmNote uiNote = new frmNote();
                    int parkPrimaryKeyID = 0;
                    int.TryParse(Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["PARK_PK_ID"].Value), out parkPrimaryKeyID);
                    uiNote.CustomerKey = parkPrimaryKeyID;
                    uiNote.GetNotes();
                    uiNote.ShowDialog();
                    //wgrid.dataGridView1.SelectedRows[0].Cells["Note"].Value = uiNote.Note;
                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
        }

        void GetPhotoPath(int customerPrimaryKeyID, out string entryEESNorthImg, out string entryEESsouthImg, out string exitEESnorthImg, out string exitEESsouthImg)
        {
            entryEESNorthImg = "";
            entryEESsouthImg = "";
            exitEESnorthImg = "";
            exitEESsouthImg = "";
            try
            {
                string query = "SELECT * FROM L2_PARK_HISTORY WHERE PARK_ID =" + customerPrimaryKeyID;
                DataTable dt = new DataTable();
                dt.TableName = "L2_PARK_HISTORY";
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
                                entryEESNorthImg = Convert.ToString(dreader["ENTRY_NORTH_IMG"]);
                                entryEESsouthImg = Convert.ToString(dreader["ENTRY_SOUTH_IMG"]);

                                exitEESnorthImg = Convert.ToString(dreader["EXIT_NORTH_IMG"]);
                                exitEESsouthImg = Convert.ToString(dreader["EXIT_SOUTH_IMG"]);
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
        }
        public void DoOnLoad()
        {
            hostListView.Child = wgrid;
            hostdatefrom.Child = ctrlDateTimeFrom;
            hostdateto.Child = ctrlDateTimeTo;
                 
            ctrlDateTimeFrom.Enabled = true;
            ctrlDateTimeTo.Enabled = true;

            ctrlDateTimeFrom.dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00");
            ctrlDateTimeTo.dateTimePicker1.Value = DateTime.Now;
           // GetCurrentParks();
            GetRecords(GetQuery());
        }

        void GetCurrentParks()
        {
            try
            {
                string query = "";
                query = GetQuery();

                Task.Factory.StartNew(() => Get(query));             
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }             
        }
        string GetQuery()
        {
            string query = "";
            try
            {
                //if (chkMember.IsChecked == true)
                //    query = "SELECT * FROM MEMBER_HISTORY_PARKS_VIEW WHERE S_NO > 0 ";
                //else
                   // query = "SELECT * FROM HISTORY_PARKS_VIEW WHERE S_NO > 0 ";
                query="SELECT "
 +" rownum S_NO  ,GATE "+@"""Entry EES"", EXIT_GATE  "+@"""Exit EES"",entry_TIME  "+@"""ENTRY TIME"",exit_TIME  "+@"""EXIT TIME"",ID,"
 + " CARID " + @"""PLATE NO#"", F_LEVEL " + @"""LEVEL"",AISLE,F_ROW " + @"""ROW"",CARWASH " + @"""CAR WASH"","
 +" ROTATION,CARTYPE "+@"""CAR TYPE"",DURATION,PARKING_TIME "+@"""PARKING TIME"",RETRIEVING_TIME "+@"""RETRIEVING TIME"",LOCATION,"
 +" PATRON_NAME "+@"""PATRON NAME"",park_id "+@"""PARK_PK_ID"",transfer_status "+@"""TRANSFER STATUS"""
 +" FROM ("
 
 +"  SELECT                                                                                "
 + "   ph.GATE gate,                                                                       "
 +"    ph.EXIT_GATE,                                                                       "
 +"    ph.ENTRY_TIME entry_TIME,                                                           "
 +"     ph.exit_time exit_TIME,                                                            "
 +"     ph.GET_FROM_SLOT_TIME GET_FROM_SLOT_TIME,                                          "
 + "    ph.CARD_ID ID,                                                                     "
 +"     ph.CAR_NUMBER CARID,                                                               "
 +"     ph.SLOT_FLOOR F_LEVEL,                                                             "
 +"     ph.SLOT_AISLE AISLE,                                                               "
 +"     ph.SLOT_ROW F_ROW,                                                                 "
 + "    CASE                                                                               "
 +"       WHEN ph.NEED_WASH = 0                                                            "
 +"       THEN 'FALSE'                                                                     "
 +"       WHEN ph.NEED_WASH = 1                                                            "
 +"        THEN 'TRUE'                                                                     "
 + "     END CARWASH,                                                                      "
 +"      CASE                                                                              "
 +"        WHEN ph.IS_ROTATE = 0                                                           "
 +"        THEN 'FALSE'                                                                    "
 +"      WHEN ph.IS_ROTATE = 1                                                             "
 + "       THEN 'TRUE'                                                                     "
 +"      END ROTATION,                                                                     "
 +"      CASE                                                                              "
 +"        WHEN ph.car_type = 1                                                            "
 +"        THEN 'LOW'                                                                      "
 + "  WHEN ph.car_type = 2                                                                 "
 +"        THEN 'HIGH'                                                                     "
 +"        WHEN ph.car_type = 3                                                            "
 +"        THEN 'Medium'                                                                   "
 +"    END CARTYPE,                                                                        "
 + "   abs(CONFIG_PACKAGE.datediff('MI',ph.ENTRY_TIME,ph.PUT_TO_SLOT_TIME))                "
 +"    ||':'||abs((CONFIG_PACKAGE.datediff('SS',ph.ENTRY_TIME,ph.PUT_TO_SLOT_TIME)-CONFIG_PACKAGE.datediff('MI',ph.ENTRY_TIME,ph.PUT_TO_SLOT_TIME)*60))     "
 +"    PARKING_TIME,                                                                                                                                        "
 +"    abs(CONFIG_PACKAGE.datediff('MI',ph.GET_FROM_SLOT_TIME,ph.EXIT_TIME))                                                                                "
 +"    ||':'||abs((CONFIG_PACKAGE.datediff('SS',ph.GET_FROM_SLOT_TIME,ph.EXIT_TIME)-CONFIG_PACKAGE.datediff('MI',ph.GET_FROM_SLOT_TIME,ph.EXIT_TIME)*60))   "
 + "   RETRIEVING_TIME                                                                     "
 + "    ,'Retrieved' LOCATION                                                               "
 + "    , ph.PATRON_NAME,                                                                   "
 + "    ph.get_from_slot_time - ph.ENTRY_TIME DURATION,                                     "
 + "  ph.park_id                                                                            "
 + "  ,CASE                                                                                 "
 + "   WHEN ph.is_transfer=1 THEN 'FROM '||ph.TRANSFER_FROM                                 "
 + "   ELSE ''                                                                              "
 + "    END transfer_status                                                                 "
 + "   FROM L2_PARK_HISTORY ph                                                             "
  +" )TBL"                                                                                  
  +" WHERE rownum > 0 ";                                                                    
 
  //where entry_TIME between '16/Sep/2015 12:00:00 AM' AND '16/Sep/2015 12:33:43 PM'
  //and exit_TIME between '16/Sep/2015 12:00:00 AM' AND '16/Sep/2015 12:33:43 PM';
                DateTime dtFrom;
                DateTime dtTo;

                string filter = "";
                if (txtEntryEES.Text != "All" && string.IsNullOrEmpty(txtEntryEES.Text) == false)
                {
                    filter += " AND gate = '" + txtEntryEES.Text + "'";
                }
                if (txtExitEES.Text != "All" && string.IsNullOrEmpty(txtExitEES.Text) == false)
                {
                    filter += " AND exit_gate = '" + txtExitEES.Text + "'";
                }

                if (string.IsNullOrEmpty(txtName.Text) == false)
                    filter += " AND UPPER(PATRON_NAME) LIKE UPPER('%" + txtName.Text.Trim() + "%')";

                if (string.IsNullOrEmpty(txtCarID.Text) == false)
                    filter += " AND UPPER(CARID) LIKE UPPER('%" + txtCarID.Text + "%')";

                if (string.IsNullOrEmpty(txtID.Text) == false)
                    filter += " AND UPPER(ID) LIKE UPPER('%" +txtID.Text + "%')"; 


                //if (string.IsNullOrEmpty(txtFloor.Text) == false)
                //    filter += " AND F_LEVEL = " + txtFloor.Text;


                //if (string.IsNullOrEmpty(txtRow.Text) == false)
                //    filter += " AND F_ROW=" + txtRow.Text;

                //if (string.IsNullOrEmpty(txtAisle.Text) == false)
                //    filter += " AND AISLE=" + txtAisle.Text;

                if (chkWash.IsChecked == true)
                    filter += " AND CARWASH='TRUE'";

                dtFrom = ctrlDateTimeFrom.dateTimePicker1.Value;
                dtTo = ctrlDateTimeTo.dateTimePicker1.Value;

                if (chkEntryDateEnable.IsChecked == true)
                    filter += " AND entry_TIME BETWEEN '" + dtFrom.ToString("dd/MMM/yyyy hh:mm:ss tt") + "' AND '" + dtTo.ToString("dd/MMM/yyyy hh:mm:ss tt") + "'";

                if (chkExitDateEnable.IsChecked == true)
                    filter += " AND exit_TIME BETWEEN '" + dtFrom.ToString("dd/MMM/yyyy hh:mm:ss tt") + "' AND '" + dtTo.ToString("dd/MMM/yyyy hh:mm:ss tt") + "'";
                if (chkMember.IsChecked == true)
                    filter += " and ID in (select card_id from rpmsadmin.l2_members)";
                int temp = 0;
                if (int.TryParse(retDuratnSearch.Text, out temp) && temp > 0)
                {
                    filter += "AND (abs(CONFIG_PACKAGE.datediff('MI',GET_FROM_SLOT_TIME,EXIT_TIME)))>=" + temp;
                }

                query += filter;
            }
            catch (Exception errMsg)
            {
            }
            finally
            { 
            
            }
            return query;
        }
        string GetQueryForDelayHist()
        {
            string query = "";
            try
            {

                query = "SELECT "

                + " CT.ENTRY_TIME entry_TIME,"
                + "CT.exit_time exit_TIME,"
                + "CT.CARD_ID CARD_ID,"
                + "CT.CAR_NUMBER Plate,"
                    //   + "abs(CONFIG_PACKAGE.datediff('MI',CT.ENTRY_TIME,CT.PUT_TO_SLOT_TIME))"
                    // + "||':'||abs((CONFIG_PACKAGE.datediff('SS',CT.ENTRY_TIME,CT.PUT_TO_SLOT_TIME)-CONFIG_PACKAGE.datediff('MI',CT.ENTRY_TIME,CT.PUT_TO_SLOT_TIME)*60))"
                    //   + " PARKING_TIME,"
                + "abs(CONFIG_PACKAGE.datediff('MI',CT.GET_FROM_SLOT_TIME,CT.EXIT_TIME))"
                + " ||':'||abs((CONFIG_PACKAGE.datediff('SS',CT.GET_FROM_SLOT_TIME,CT.EXIT_TIME)-CONFIG_PACKAGE.datediff('MI',CT.GET_FROM_SLOT_TIME,CT.EXIT_TIME)*60))"
                + " RETRIEVING_TIME"
                + ", CT.PATRON_NAME"
                + ", CT.NOTE"
                + " FROM "
                + " L2_PARK_HISTORY CT"
                + " WHERE "
                + " 1 = 1";


                DateTime dtFrom;
                DateTime dtTo;

                string filter = "";
                if (txtEntryEES.Text != "All" && string.IsNullOrEmpty(txtEntryEES.Text) == false)
                {
                    filter += " AND CT.gate = '" + txtEntryEES.Text + "'";
                }
                if (txtExitEES.Text != "All" && string.IsNullOrEmpty(txtExitEES.Text) == false)
                {
                    filter += " AND CT.exit_gate = '" + txtExitEES.Text + "'";
                }

                if (string.IsNullOrEmpty(txtName.Text) == false)
                    filter += " AND UPPER(CT.PATRON_NAME) LIKE UPPER('%" + txtName.Text.Trim() + "%')";

                if (string.IsNullOrEmpty(txtCarID.Text) == false)
                    filter += " AND UPPER(Plate) LIKE UPPER('%" + txtCarID.Text + "%')";




                dtFrom = ctrlDateTimeFrom.dateTimePicker1.Value;
                dtTo = ctrlDateTimeTo.dateTimePicker1.Value;

                if (chkEntryDateEnable.IsChecked == true)
                    filter += " AND entry_TIME BETWEEN '" + dtFrom.ToString("dd/MMM/yyyy hh:mm:ss tt") + "' AND '" + dtTo.ToString("dd/MMM/yyyy hh:mm:ss tt") + "'";

                if (chkExitDateEnable.IsChecked == true)
                    filter += " AND exit_TIME BETWEEN '" + dtFrom.ToString("dd/MMM/yyyy hh:mm:ss tt") + "' AND '" + dtTo.ToString("dd/MMM/yyyy hh:mm:ss tt") + "'";
                int temp=0;
                if (int.TryParse(retDuratnSearch.Text,out temp) && temp>0)
                {
                    filter += "AND (abs(CONFIG_PACKAGE.datediff('MI',CT.GET_FROM_SLOT_TIME,CT.EXIT_TIME)))>=" + temp;
                }
                query += filter;


                //query = "select ENTRY_TIME,EXIT_TIME,CARD_ID,PLATE,RETRIEVING_TIME,PATRON_NAME,NOTE"
                //        +" from DELAY_HISTORY_VIEW";

            }
            catch (Exception errMsg)
            {
            }
            finally
            {

            }
            return query;
        }
        DataTable Get(string query)
        {
            DataTable dtResult = new DataTable();
            try
            {
                dtResult.TableName = "CURRENTPARKS";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                        dadapter.Fill(dtResult);

                        if (AfterRecordFetch != null)
                            AfterRecordFetch(dtResult, new EventArgs());
                    }
                }
            }
            catch (Exception errMsg)
            {
            }
            finally { 
            
            }
            return dtResult;
        }
        DataTable GetDelayedDataTable(string query)
        {
            DataTable dtResult = new DataTable();
            try
            {
                dtResult.TableName = "DelayHistory";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                        dadapter.Fill(dtResult);

                       
                    }
                }
            }
            catch (Exception errMsg)
            {
            }
            finally
            {

            }
            return dtResult;
        }
        void  GetRecords(string query)
        {
            DataTable dtResult = new DataTable();
            try
            {
                dtResult.TableName = "CURRENTPARKS";
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                        dadapter.Fill(dtResult);
                    }
                }

                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                    if (wgrid.dataGridView1.IsHandleCreated == false) return;
                    wgrid.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                wgrid.dataGridView1.SuspendLayout();
                                wgrid.dataGridView1.Columns.Clear();
                                wgrid.dataGridView1.DataSource = dtResult;

                                //For photo
                                System.Windows.Forms.DataGridViewButtonColumn btnCol = new System.Windows.Forms.DataGridViewButtonColumn();
                                btnCol.Name = "Photo";
                                btnCol.Text = "Photo";
                                btnCol.Tag = "photo";
                                wgrid.dataGridView1.Columns.Insert(19, btnCol);

                                //For Note
                                System.Windows.Forms.DataGridViewButtonColumn btnNote = new System.Windows.Forms.DataGridViewButtonColumn();
                                btnNote.Name = "Note";
                                btnNote.Text = "Note";
                                btnNote.Tag = "Note";
                                wgrid.dataGridView1.Columns.Insert(20, btnNote);

                                wgrid.dataGridView1.Columns["ENTRY TIME"].DefaultCellStyle.Format = "dd/MMM/yyyy hh:mm:ss tt";
                                wgrid.dataGridView1.Columns["EXIT TIME"].DefaultCellStyle.Format = "dd/MMM/yyyy hh:mm:ss tt";

                            }
                            catch (Exception errMsg)
                            {
                                MessageBox.Show(errMsg.Message);
                            }
                            finally
                            {
                                wgrid.dataGridView1.ResumeLayout();
                                FormatGrid();
                            }
                        }));
                }
                else
                {
                    if(wgrid.dataGridView1.Columns.Count > 0)
                            wgrid.dataGridView1.Columns.Clear();
                }

                if (dtResult != null && dtResult.Rows.Count > 0)
                {
                   lblTotalRecords.Dispatcher.BeginInvoke(new Action (() => lblTotalRecords.Content = "Total Records =" + dtResult.Rows.Count));
                }
                else
                      lblTotalRecords.Dispatcher.BeginInvoke(new Action (() => lblTotalRecords.Content = "Total Records = 0"));
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
            finally { 
            
            }
            //return dtResult;
        }

        void FormatGrid()
        {

            HideColumn();
            ReArrangeColumn();
            SetGridColumnSize();
            SetGridHeaderConfig();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            GetCurrentParks();
        }

        private void txtName_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        void SetGridColumnSize()
        {
            wgrid.dataGridView1.Columns["S_NO"].Width = 43;
            wgrid.dataGridView1.Columns["PATRON NAME"].Width = 120;
            wgrid.dataGridView1.Columns["ID"].Width = 65;

            wgrid.dataGridView1.Columns["PLATE NO#"].Width = 70;
            wgrid.dataGridView1.Columns["CAR TYPE"].Width = 40;
            wgrid.dataGridView1.Columns["CAR WASH"].Width = 43;
            wgrid.dataGridView1.Columns["ENTRY TIME"].Width = 135;
            wgrid.dataGridView1.Columns["EXIT TIME"].Width = 135;
            
            wgrid.dataGridView1.Columns["DURATION"].Width = 65;
            wgrid.dataGridView1.Columns["PARKING TIME"].Width = 45;
            wgrid.dataGridView1.Columns["RETRIEVING TIME"].Width = 45;

            wgrid.dataGridView1.Columns["LEVEL"].Width = 35;
            wgrid.dataGridView1.Columns["AISLE"].Width = 35;
            wgrid.dataGridView1.Columns["ROW"].Width = 33;
            
            wgrid.dataGridView1.Columns["Entry EES"].Width = 43;
            wgrid.dataGridView1.Columns["Exit EES"].Width = 43;

            wgrid.dataGridView1.Columns["ROTATION"].Width = 63;
          

        }

        void ReArrangeColumn()
        {
            wgrid.dataGridView1.Columns["S_NO"].DisplayIndex = 0;
            wgrid.dataGridView1.Columns["PATRON NAME"].DisplayIndex = 1;
            wgrid.dataGridView1.Columns["ID"].DisplayIndex = 2;

            wgrid.dataGridView1.Columns["PLATE NO#"].DisplayIndex = 3;
            wgrid.dataGridView1.Columns["CAR TYPE"].DisplayIndex = 4;
            wgrid.dataGridView1.Columns["CAR WASH"].DisplayIndex = 5;

            wgrid.dataGridView1.Columns["ENTRY TIME"].DisplayIndex = 6;
            wgrid.dataGridView1.Columns["EXIT TIME"].DisplayIndex = 7;
            wgrid.dataGridView1.Columns["DURATION"].DisplayIndex = 8;

            wgrid.dataGridView1.Columns["PARKING TIME"].DisplayIndex = 9;
            wgrid.dataGridView1.Columns["RETRIEVING TIME"].DisplayIndex = 10;
            //wgrid.dataGridView1.Columns[""].DisplayIndex = 11;

            wgrid.dataGridView1.Columns["LEVEL"].DisplayIndex = 11;
            wgrid.dataGridView1.Columns["AISLE"].DisplayIndex = 12;
            wgrid.dataGridView1.Columns["ROW"].DisplayIndex = 13;

            wgrid.dataGridView1.Columns["Entry EES"].DisplayIndex = 14;
            wgrid.dataGridView1.Columns["Exit EES"].DisplayIndex = 15;
            wgrid.dataGridView1.Columns["ROTATION"].DisplayIndex = 16;
            wgrid.dataGridView1.Columns["TRANSFER STATUS"].DisplayIndex = 17;
         //   wgrid.dataGridView1.Columns["ROTATION"].DisplayIndex = 17;
        }

        void HideColumn()
        {
            wgrid.dataGridView1.Columns["PARK_PK_ID"].Visible = false;
            wgrid.dataGridView1.Columns["LOCATION"].Visible = false;
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

        private void txtCarID_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnFilter_Click(sender, e);
        }
     
        void ExportAsExcelReport()
        {
            DataTable dt = wgrid.dataGridView1.DataSource as DataTable;
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            ds.Namespace = "History";

            DataTable dtReport = new DataTable();
            dtReport.Columns.Add("S_NO");
            dtReport.Columns.Add("Entry EES");
            dtReport.Columns.Add("Exit EES");

            foreach (DataRow drow in dt.Rows)
            {
                DataRow drow1 = dtReport.NewRow();
                drow1[0] = drow[0].ToString();
                drow1[1] = drow[1].ToString();
                drow1[2] = drow[2].ToString();
                dtReport.Rows.Add(drow1);
            }

            DataSet dsRpt = new DataSet("dsReport");
            dsRpt.Tables.Add(dtReport);
            ExcelLibrary.DataSetHelper.CreateWorkbook(@"C:\MyExcelFile.xls", dsRpt);
            
   //          XmlDataDocument xmlDataDoc = new XmlDataDocument(ds);
   //XslTransform xt = new XslTransform();
   //StreamReader reader =new StreamReader(typeof(WorkbookEngine).Assembly.GetManifestResourceStream(typeof (WorkbookEngine), "Excel.xsl"));
   //XmlTextReader xRdr = new XmlTextReader(reader);
   //xt.Load(xRdr, null, null);

   //StringWriter sw = new StringWriter();
   //xt.Transform(xmlDataDoc, null, sw, null);

   //StreamWriter myWriter = new StreamWriter (path + "\\Report.xls");
   //myWriter.Write (sw.ToString());
   //myWriter.Close ();
        }

         void DisplayReport()
        {
            try
            {
                frmRptView rptView = new frmRptView();
                rptView.Show();
                DataTable dtRpt = wgrid.dataGridView1.DataSource as DataTable;
                DataSet dsRpt = null;
                if (dtRpt != null && dtRpt.Rows.Count > 0)
                {
                    dsRpt = new DataSet();
                    dsRpt.Tables.Add(dtRpt.Copy());
                }
                if (dsRpt != null)
                {
                    dsRpt.Tables[0].TableName = "dtHistory";
                    dsRpt.Namespace = "dsHistory";

                    dsRpt.Tables[0].Columns["PLATE NO#"].ColumnName = "CAR ID";
                   // dsRpt.WriteXmlSchema("C:\\RPTHistoryv1.xsd");

                    rptView.LoadHistoryReport(dsRpt);
                }
            }
            finally { }
        }
         void DisplayDelayReport()
         {
             try
             {
                 frmRptView rptView = new frmRptView();
                 rptView.Show();


                 DataTable dtRpt = GetDelayedDataTable(GetQueryForDelayHist());
                 DataSet dsRpt = null;
                 if (dtRpt != null && dtRpt.Rows.Count > 0)
                 {
                     dsRpt = new DataSet();
                     dsRpt.Tables.Add(dtRpt);
                 }
                 if (dsRpt != null)
                 {
                     dsRpt.Tables[0].TableName = "DELAY_HISTORY_VIEW";
                     dsRpt.Namespace = "DELAY_HISTORY_VIEW";

                    
                     rptView.LoadDelayedHistoryReport(dsRpt);
                 }
                
                 
             }
             finally { }
         }
        private void btnReport_Click(object sender, RoutedEventArgs e)
        {
            DisplayReport();

        }

        private void txtName_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            GetCurrentParks();
        }

        private void txtCarID_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetCurrentParks();
        }

        private void chkWash_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Security.Security.currentUserId == 1) // only admin is allowed to delete
                {
                    int delSelectedRecodCount = 0;
                    delSelectedRecodCount = wgrid.dataGridView1.SelectedRows.Count;

                    if (MessageBox.Show("You are selected to delete" + delSelectedRecodCount + " records, Do you want to continue?", 
                        "Information", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        int id = 0;
                        for (int i = 0; i <= wgrid.dataGridView1.SelectedRows.Count - 1; i++)
                        {
                            int.TryParse(Convert.ToString(wgrid.dataGridView1.Rows[i].Cells["PARK_PK_ID"].Value), out id);

                            using (OracleConnection con = new OracleConnection( Connection.connectionString))
                            {
                                if (con.State == System.Data.ConnectionState.Closed) con.Open();

                                using (OracleCommand command = con.CreateCommand())
                                {
                                    command.Connection = con;
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.CommandText = "DELETE_RECORDS_FROM_HISTORY";
                                    command.Parameters.Add("PARK_PK_ID", OracleDbType.Int32, id, ParameterDirection.Input);
                                    command.ExecuteNonQuery();
                                }
                            }

                            //image path
                            string entyNorthImg=null;
                            string entrySouthImg=null;
                            string exitNothImg=null;
                            string exitSouthImg=null;
                            GetPhotoPath(id, out entyNorthImg, out  entrySouthImg, out exitNothImg,out exitSouthImg);
                            if (File.Exists(entyNorthImg ))
                                File.Delete(entyNorthImg);
                            if (File.Exists(entrySouthImg))
                                File.Delete(entrySouthImg);
                            if (File.Exists(exitNothImg))
                                File.Delete(exitNothImg);
                            if (File.Exists(exitSouthImg))
                                File.Delete(exitSouthImg);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Access denied.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DoOnLoad();
            wgrid.dataGridView1.MultiSelect = true;
        }

        private void chkDateEnable_Checked(object sender, RoutedEventArgs e)
        {

        }


        void SetGridHeaderConfig()
        {
            wgrid.dataGridView1.ColumnHeadersHeight = 30;
            wgrid.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightSteelBlue;
            wgrid.dataGridView1.AdvancedRowHeadersBorderStyle.All = System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.Outset;
            wgrid.dataGridView1.AllowUserToOrderColumns = true;
            wgrid.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
            //wgrid.dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.Gainsboro;
        }

        private void btnDelayReport_Click(object sender, RoutedEventArgs e)
        {
            DisplayDelayReport();
        }
    }
}
