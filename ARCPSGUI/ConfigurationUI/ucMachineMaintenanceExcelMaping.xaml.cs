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
using ARCPSGUI.DB;
using System.Data;
using Oracle.DataAccess.Client;
using System.IO;
using System.Xml;
using ARCPSGUI.Controls;

namespace ARCPSGUI.ConfigurationUI
{
    /// <summary>
    /// Interaction logic for ucMachineMaintenanceExcelMaping.xaml
    /// </summary>
    public partial class ucMachineMaintenanceExcelMaping : UserControl
    {
        ucWinGrid wgrid = new ucWinGrid();
        GeneralDba objGeneralDba = null;
        public ucMachineMaintenanceExcelMaping()
        {
            InitializeComponent();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            OnLoad();

            hostListView.Child = wgrid;
            wgrid.dataGridView1.ReadOnly = false;
            LoadCMData();

            wgrid.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
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

        private void btnExcelPathSave_Click(object sender, RoutedEventArgs e)
        {
            if (Security.Security.currentUserId == 1) // allow only for admin
            {
            try
            {
                string path = txtMacMaintExcelPath.Text.Trim();
                if (string.IsNullOrEmpty(path) == false)
                {
                    new Connection().SaveMachineMaintenancePath(path);
                    MessageBox.Show("Maintenance excel path have saved", "Information", MessageBoxButton.OK);
                }
            }
            finally
            { }
            }
            else
            {
                MessageBox.Show("Access Denied", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        void OnLoad()
        {
            try
            {
                Connection con = new Connection();
                txtMacMaintExcelPath.Text = con.GetMachineMaintenancePath();
                txtDisplayXML.Text = con.GetDisplayXMLPath();
                txtExitEstTime.Text = con.GetExitEstimateTime().ToString();
                txtEESImgPath.Text = con.GetEESPhotoPath();
                txtCWEstXMLPath.Text = con.GetEstCarWashTimeXMLPath();
               

               txtEntryXMLReqPath.Text = con.GetEntryXMlReqPath();
               txtExitXMLReqPath.Text = con.GetExitXMlReqPath();

               cmbEnableCarWash.Text  = con.GetCarWashEnableIndication();
               txtExitDisplayMessage.Text = objGeneralDba.GetExitDisplayMessage();
               LoadRampDisplayConfig();
            }
            finally
            {

            }
        }

        private void btnDisplayXML_Click(object sender, RoutedEventArgs e)
        {
            if (Security.Security.currentUserId == 1) // allow only for admin
            {
                try
                {
                    string path = txtDisplayXML.Text.Trim();
                    if (string.IsNullOrEmpty(path) == false)
                    {
                        new Connection().SaveDisplayXMLPath(path);
                        MessageBox.Show("Monitor Display file name have saved", "Information", MessageBoxButton.OK);
                    }
                }
                finally
                { }
            }
            else
            {
                MessageBox.Show("Access Denied", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

        }

        private void btnSaveExitEstTime_Click(object sender, RoutedEventArgs e)
        {
            if (Security.Security.currentUserId == 1) // allow only for admin
            {

                try
                {
                    int estimateTime = 0;
                    int.TryParse(txtExitEstTime.Text, out estimateTime);

                    if (estimateTime > 0)
                    {
                        new Connection().SaveExitEstmateTime(estimateTime);
                        MessageBox.Show("Exit estimate time have saved.", "Information", MessageBoxButton.OK);
                    }
                }
                finally
                { }
            }
            else
            {
                MessageBox.Show("Access Denied", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void btnSaveEESImagePath_Click(object sender, RoutedEventArgs e)
        {
            if (Security.Security.currentUserId == 1) // allow only for admin
            {

                try
                {
                    string path = txtEESImgPath.Text.Trim();
                    if (string.IsNullOrEmpty(path) == false)
                    {
                        new Connection().SaveEESPhotoPath(path);
                        MessageBox.Show("EES Image path have saved.", "Information", MessageBoxButton.OK);
                    }
                }
                finally
                { }
            }
            else
            {
                MessageBox.Show("Access Denied", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void btnSaveCWEstXMLPath_Click(object sender, RoutedEventArgs e)
        {
            if (Security.Security.currentUserId == 1) // allow only for admin
            {
                try
                {
                    string path = txtCWEstXMLPath.Text.Trim();
                    if (string.IsNullOrEmpty(path) == false)
                    {
                        new Connection().SaveEstCarWashTimeXMLPath(path);
                        MessageBox.Show("Car wash estimation xml path have saved.", "Information", MessageBoxButton.OK);
                    }
                }
                finally
                { }
            }
            else
            {
                MessageBox.Show("Access Denied", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

    
        private void btnSaveEntryXMLReqPath_Click(object sender, RoutedEventArgs e)
        {
            if (Security.Security.currentUserId == 1) // allow only for admin
            {
                try
                {
                    if (!string.IsNullOrEmpty(txtEntryXMLReqPath.Text.Trim()))
                    {
                        new Connection().SaveEntryXMlReqPath(txtEntryXMLReqPath.Text.Trim());
                        MessageBox.Show("Entry xml path has saved.", "Information", MessageBoxButton.OK);
                    }
                }
                finally
                { }
            }
            else
            {
                MessageBox.Show("Access Denied", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void btnSaveExitXMLReqPath_Click(object sender, RoutedEventArgs e)
        {
            if (Security.Security.currentUserId == 1) // allow only for admin
            {
                try
                {
                    if (!string.IsNullOrEmpty(txtExitXMLReqPath.Text.Trim()))
                    {
                        new Connection().SaveExitXMlReqPath(txtExitXMLReqPath.Text.Trim());
                        MessageBox.Show("Exit xml path has saved.", "Information", MessageBoxButton.OK);
                    }
                }
                finally
                { }
            }
            else
            {
                MessageBox.Show("Access Denied", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnSaveEnableCarWash_Click(object sender, RoutedEventArgs e)
        {
            if (Security.Security.currentUserId == 1) // allow only for admin
            {
                try
                {
                    Connection con = new Connection();
                    string xmlFilePath = "";
                    string isCarWashEnable = "";
                  
                    isCarWashEnable = cmbEnableCarWash.Text.Trim();
                    xmlFilePath = con.GetCarWashEnableIndicationXMLPath();

                    if (string.IsNullOrEmpty(isCarWashEnable) == false && string.IsNullOrEmpty(xmlFilePath) == false)
                    {
                        new Connection().SaveCarWashEnableIndication(isCarWashEnable);
                        UpdateCarWashEnableXML(xmlFilePath);
                        MessageBox.Show("Car wash enabled have saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Car wash enabled have saved has failed.", "Information", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                finally
                { }
            }
            else
            {
                MessageBox.Show("Access Denied", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        public void UpdateCarWashEnableXML(string xmlFilePath)
        {
            try
            {
                using (FileStream fs = new FileStream(xmlFilePath, FileMode.Create))
                {
                    using (XmlTextWriter w = new XmlTextWriter(fs, null))
                    {
                        w.Formatting = Formatting.Indented;
                        w.Indentation = 5;
                        w.WriteStartDocument();
                        w.WriteStartElement("KioskConfig");
                        w.WriteStartElement("CarwashEnable");
                        w.WriteValue(cmbEnableCarWash.Text.ToString());
                        w.WriteEndElement();
                        w.WriteEndElement();
                        w.Close();
                    }
                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message, "Information", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { }
        }

        #region CM Home Position Configuration
        void LoadCMData()
        {
            string query = "";
            try
            {
                query = "select Floor,machine_code, home_aisle,HOME_ROW from l2_lcm_ucm_master order by floor,lu_name ";
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
                        SetGridColumnSize();
                        SetReadOnlyColumn();
                        SetGridColumnDataAlignment();
                        SetGridEditColumnHeighLight();
                        SetGridHeaderConfig();
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
                    wgrid.dataGridView1.Columns["machine_code"].HeaderText = "Machine";
                    wgrid.dataGridView1.Columns["Floor"].HeaderText = "Floor";
                    wgrid.dataGridView1.Columns["home_aisle"].HeaderText = "Aisle";
                    wgrid.dataGridView1.Columns["HOME_ROW"].HeaderText = "Row";
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
                    wgrid.dataGridView1.Columns["machine_code"].Width = 500;
                    wgrid.dataGridView1.Columns["Floor"].Width = 100;
                    wgrid.dataGridView1.Columns["home_aisle"].Width = 100;
                    wgrid.dataGridView1.Columns["HOME_ROW"].Width = 100;
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
                wgrid.dataGridView1.Columns["machine_code"].ReadOnly = true;
                wgrid.dataGridView1.Columns["Floor"].ReadOnly = true;
            }
            catch (Exception errMsg)
            {

            }
            finally
            {
              
            }
        }
        void SetGridColumnDataAlignment()
        {
            try
            {
                wgrid.dataGridView1.Columns["Floor"].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
                wgrid.dataGridView1.Columns["machine_code"].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
                wgrid.dataGridView1.Columns["home_aisle"].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
                wgrid.dataGridView1.Columns["HOME_ROW"].DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
              
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
                wgrid.dataGridView1.Columns["home_aisle"].DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;
                wgrid.dataGridView1.Columns["HOME_ROW"].DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow;

            }
            catch (Exception errMsg)
            {

            }
            finally
            {

            }
        }
        void SetGridHeaderConfig()
        {
            wgrid.dataGridView1.ColumnHeadersHeight = 40;
            //wgrid.dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.LightSteelBlue;
            wgrid.dataGridView1.AdvancedRowHeadersBorderStyle.All = System.Windows.Forms.DataGridViewAdvancedCellBorderStyle.Outset;
            wgrid.dataGridView1.AllowUserToOrderColumns = true;
            wgrid.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Raised;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Security.Security.currentUserId == 1) // allow only for admin
                
                {
                    if (MessageBox.Show("Do you want to save records?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        int needToSave = 0;
                        string machineCode = "";
                        int homeAisle = 0;
                        int homeRow = 0;
                        for (int i = 0; i <= wgrid.dataGridView1.Rows.Count - 1; i++)
                        {
                            int.TryParse(Convert.ToString(wgrid.dataGridView1.Rows[i].Tag), out needToSave);
                            if (needToSave == 1)
                            {
                                machineCode = Convert.ToString(wgrid.dataGridView1.Rows[i].Cells["machine_code"].Value);
                                int.TryParse(Convert.ToString(wgrid.dataGridView1.Rows[i].Cells["home_aisle"].Value), out homeAisle);
                                int.TryParse(Convert.ToString(wgrid.dataGridView1.Rows[i].Cells["HOME_ROW"].Value), out homeRow);
                                if (string.IsNullOrEmpty(machineCode) == false && homeAisle > 0 && homeRow > 0)
                                {
                                    Save(machineCode, homeAisle,homeRow);
                                }
                                wgrid.dataGridView1.Rows[i].Tag = 0;
                            }
                            machineCode = "";
                            homeAisle = 0;
                            needToSave = 0;
                        }
                        MessageBox.Show("Successfuly saved", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Access Denied", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
        }
        void Save(string machineCode, int homeAisle, int homeRow)
        {
            try
            {
                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    OracleCommand command = con.CreateCommand();

                    string sql = "update l2_lcm_ucm_master set home_aisle = " + homeAisle + ", HOME_ROW = " + homeRow + " where machine_code = '" + machineCode + "'";

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

        private void btnSaveExitDisplayMessage_Click(object sender, RoutedEventArgs e)
        {
            objGeneralDba.SaveExitDisplayMessage(txtExitDisplayMessage.Text);
        }

        private void btnRampDisplayMessage_Click(object sender, RoutedEventArgs e)
        {
            SetRampDisplayConfig();
        }
       
        private void LoadRampDisplayConfig()
        {
            msg1Radio.Content = objGeneralDba.GetRampDisplayMesaage("MESSAGE1");
            msg2Radio.Content = objGeneralDba.GetRampDisplayMesaage("MESSAGE2");
            txtRampDisplayCustomMessage.Text = objGeneralDba.GetRampDisplayMesaage("CUSTOM_MESSAGE");
            txtRampDisplayMessage.Text = objGeneralDba.GetRampDisplayMesaage("MESSAGE");
            parkingStatusCheck.IsChecked = objGeneralDba.GetRoboticParkingStatus() == 1 ? true : false;
        }
        private void SetRampDisplayConfig()
        {
            try
            {
                //string msg = txtRampDisplayMessage.Text;
                //if (msg1Radio.IsChecked == true)
                //    msg = msg1Radio.Content.ToString();
                //else if (msg2Radio.IsChecked == true)
                //    msg = msg2Radio.Content.ToString();
                objGeneralDba.SetRampDisplayMesaage("CUSTOM_MESSAGE", txtRampDisplayCustomMessage.Text);
                objGeneralDba.SetRampDisplayMesaage("MESSAGE", txtRampDisplayMessage.Text);
                objGeneralDba.SetRoboticParkingStatus(parkingStatusCheck.IsChecked==true?1:0);
                MessageBox.Show("Saved");
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void msgRadio_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rb=(RadioButton)sender;
            if (rb == msg1Radio)
            {
                txtRampDisplayMessage.Text=msg1Radio.Content.ToString();
            }
            else  if(rb == msg2Radio)
            {
                txtRampDisplayMessage.Text = msg2Radio.Content.ToString();
            }
            else
            {
                txtRampDisplayMessage.Text = txtRampDisplayCustomMessage.Text;
            }
        }

        //void SetRowSetColor()
        //{
        //    try
        //    {
        //        string tmp = "";
                
        //        System.Drawing.Color color = System.Drawing.Color.LightGray;
        //        for (int i = 0; i <= wgrid.dataGridView1.Rows.Count - 1; i++)
        //        {
        //            if (tmp != Convert.ToString(wgrid.dataGridView1.Rows[i].Cells["Floor"].Value))
        //            {
        //                tmp = Convert.ToString(wgrid.dataGridView1.Rows[i].Cells["Floor"].Value);

        //                if (color == System.Drawing.Color.LightGray)
        //                    color = System.Drawing.Color.White;
        //                else
        //                    color = System.Drawing.Color.LightGray;

        //                wgrid.dataGridView1.Rows[i].DefaultCellStyle.BackColor = color;
        //            }
        //            else
        //            {
        //                wgrid.dataGridView1.Rows[i].DefaultCellStyle.BackColor = color;
        //            }
        //        }

        //    }
        //    catch (Exception errMsg)
        //    { 
            
        //    }
        //}
        #endregion

    }
}
