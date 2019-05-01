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
    /// Interaction logic for ucAbortedTransactionView.xaml
    /// </summary>
    public partial class ucAbortedTransactionView : UserControl
    {
        ucWinGrid wgrid = new ucWinGrid();
        ucctrlTime ctrlDateTimeFrom = new ucctrlTime();
        ucctrlTime ctrlDateTimeTo = new ucctrlTime();
        ERPDba objERPDba = null;
        CustomerDba objCustomerDba = null;
      

        public ucAbortedTransactionView()
        {
            InitializeComponent();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {

        }

        public void DoOnLoad()
        {
            if (objERPDba == null)
                objERPDba = new ERPDba();
            if (objCustomerDba == null)
                objCustomerDba = new CustomerDba();
            

            hostListView.Child = wgrid;
            hostdatefrom.Child = ctrlDateTimeFrom;
            hostdateto.Child = ctrlDateTimeTo;
            wgrid.dataGridView1.MultiSelect = true;
            //chkDateEnable.IsChecked = true;
            ctrlDateTimeFrom.Enabled = true;
            ctrlDateTimeTo.Enabled = true;

            ctrlDateTimeFrom.dateTimePicker1.Value = Convert.ToDateTime(DateTime.Now.ToShortDateString() + " 00:00:00");
            ctrlDateTimeTo.dateTimePicker1.Value = DateTime.Now;

            GetAbortedRecords();
            wgrid.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView1_CellClick);
            wgrid.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridView1_DoubleClick);
        }

        void GetAbortedRecords()
        {
            try
            {
                DataTable dt = new DataTable();
                dt.TableName = "AbortedTrans";
                dt = objERPDba.GetAbortedRecords(ctrlDateTimeFrom.dateTimePicker1.Value, ctrlDateTimeTo.dateTimePicker1.Value);


                wgrid.dataGridView1.SuspendLayout();
                wgrid.dataGridView1.Columns.Clear();
                wgrid.dataGridView1.DataSource = dt;

                System.Windows.Forms.DataGridViewButtonColumn btnCol = new System.Windows.Forms.DataGridViewButtonColumn();
                btnCol.Name = "Photo";
                btnCol.Text = "Photo";
                btnCol.Tag = "photo";
                wgrid.dataGridView1.Columns.Add(btnCol);

                SetGridColumnSize();
                wgrid.dataGridView1.Columns[19].DefaultCellStyle.Format = "dd/MMM/yyyy hh:mm:ss tt";

            }
            finally
            {
                wgrid.dataGridView1.ResumeLayout();
            }
            
        }

        void SetGridColumnSize()
        {
            wgrid.dataGridView1.Columns[0].Width = 40;
            wgrid.dataGridView1.Columns[1].Width = 55;
            wgrid.dataGridView1.Columns[2].Width = 100;

            wgrid.dataGridView1.Columns[3].Width = 75;
            wgrid.dataGridView1.Columns[4].Width = 50;
            wgrid.dataGridView1.Columns[5].Width = 35;
            wgrid.dataGridView1.Columns[6].Width = 35;
            wgrid.dataGridView1.Columns[7].Width = 70;
            wgrid.dataGridView1.Columns[7].Width = 40;
            wgrid.dataGridView1.Columns[8].Width = 40;

            wgrid.dataGridView1.Columns[9].Width = 35;
            wgrid.dataGridView1.Columns[10].Width = 50;
            wgrid.dataGridView1.Columns[11].Width = 45;
            wgrid.dataGridView1.Columns[12].Width = 45;
            wgrid.dataGridView1.Columns[13].Width = 65;
            wgrid.dataGridView1.Columns[14].Width = 80;
            wgrid.dataGridView1.Columns[15].Width = 45;
            wgrid.dataGridView1.Columns[19].Width = 125;
            wgrid.dataGridView1.Columns[22].Visible = false;

        }

        void dataGridView1_CellClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (Convert.ToString(wgrid.dataGridView1.Columns[e.ColumnIndex].Tag) == "photo")
            {
                int customerPrimaryKeyID = 0;
                if (wgrid.dataGridView1.SelectedRows.Count > 0)
                {
                    string northImg = "";
                    string southImg = "";

                    int.TryParse(Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["CUSTOMER_PK_ID"].Value), out customerPrimaryKeyID);

                    objCustomerDba.GetPhotoPath(customerPrimaryKeyID, out northImg, out southImg);

                    frmShowPhoto showPhoto = new frmShowPhoto();
                    showPhoto.NorthPhotoPath = northImg;
                    showPhoto.SouthPhotoPath = southImg;
                    showPhoto.lblCarRegNo.Content = wgrid.dataGridView1.SelectedRows[0].Cells["PLATE NO#"].Value.ToString();
                    showPhoto.BringIntoView();

                    showPhoto.Show();

                }
            }
            else // this else for showing dialog with full details
            {
               
            }
        }
        void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                frmAbortDetail frm = new frmAbortDetail();

                if (wgrid.dataGridView1.DataSource != null && wgrid.dataGridView1.SelectedRows.Count > 0)
                {
                    string mode = "";
                    Int32 queueId = 0;

                    Int32.TryParse(wgrid.dataGridView1.SelectedRows[0].Cells["TRANS ID"].Value.ToString(), out queueId);
                    mode = wgrid.dataGridView1.SelectedRows[0].Cells["MODE"].Value.ToString();

                    frm.QueueId = queueId;
                    frm.Mode = mode;

                    frm.Name = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["NAME"].Value);
                    frm.CustomerId = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["CUSTOMER ID"].Value);
                    frm.Plate = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["PLATE NO#"].Value);

                    frm.Type = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["TYPE"].Value);

                    
                    frm.StartTime = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["START TIME"].Value);
                    frm.CarWash = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["CAR WASH"].Value);

                    frm.WashStatus = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["WASH STATUS"].Value);
                    frm.Rotation = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["ROTATION"].Value);
                    frm.Gate = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["GATE"].Value);
                    frm.CustomerPkId = Convert.ToInt32((wgrid.dataGridView1.SelectedRows[0].Cells["CUSTOMER_PK_ID"].Value));


                }

                frm.ShowDialog();

            }
            catch (Exception errMsg)
            {

            }
            finally
            {

            }

        }

       

        private void btnFilter_Click_1(object sender, RoutedEventArgs e)
        {
            GetAbortedRecords();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DoOnLoad();
        }

      
       
    }
}
