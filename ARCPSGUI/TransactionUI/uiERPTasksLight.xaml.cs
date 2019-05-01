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
using System.Data;
using Oracle.DataAccess.Client;
//using ARCPSGUI.Controls;
using OPC;
using System.Xml.Linq;
using System.Collections;
using ARCPSGUI.Controls;
using System.Threading.Tasks;

using System.Drawing;
using ARCPSGUI.Model;
using System.Threading;
using ARCPSGUI.OPC;
using System.Windows.Threading;
using System.Globalization;
using ARCPSGUI.DB;
using ARCPSGUI.DB;
using ARCPSGUI.ConfigurationUI;
using ARCPSGUI.Popup;
using ARCPSGUI.StaticGlobalClass;



namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for uiERPTasks.xaml
    /// </summary>
    public partial class uiERPTasksLight : UserControl, IDisposable
    {
        ucWinGrid wgrid = new ucWinGrid();
       
        frmHome g_homeUI = null;
        System.Windows.Controls.Button prevSelectedButton = null;
        System.Timers.Timer timerToUpdateGrid = null;
        System.Timers.Timer timerToUpdateCount = null;
        System.Timers.Timer timerToUpdateGridData = null;
        bool isRefreshChange = false;
        List<EESData> eesList = null;
         Connection objConnection = null;
        public delegate void InvokeDelegate();
        public delegate void InvokeDelegateForEESBlink();
        public delegate void InvokeDelegateForCount();
        LinearGradientBrush statusGradient = null;
      
        OPCServerDirector objOPCServerDirector = null;
        Connection objNewConnection = null;
        GeneralDba objGeneralDba = null;
        DataTable objDataTable = null;

        string todayDate = null;
        EESDba objEESDba = null;
        ERPDba objERPDba = null;
        VLCDba objVLCDba = null;

     

        string eesChannelForGrid = null;
        string eesCodeForGrid = null;
        string gateForGrid = null;

        
        //Dictionary<string, bool> needBlinkForEES = new Dictionary<string, bool>() 
        //{
        //    {"EES1",false},
        //     {"EES2",false},
        //      {"EES3",false},
        //       {"EES4",false},
        //       {"EES5",false},
        //       {"EES6",false},
        //       {"EES7",false},
        //       {"EES8",false},
        //       {"EES9",false},
        //};

        //Dictionary<string, bool> toggleBlinkForEES = new Dictionary<string, bool>() 
        //{
        //    {"EES1",false},
        //     {"EES2",false},
        //      {"EES3",false},
        //       {"EES4",false},
        //       {"EES5",false},
        //       {"EES6",false},
        //       {"EES7",false},
        //       {"EES8",false},
        //       {"EES9",false},
        //};
        SolidColorBrush refreshBrush1 = new SolidColorBrush(Colors.Gray);
        SolidColorBrush refreshBrush2 = new SolidColorBrush(Colors.Honeydew);
        System.Windows.Forms.ContextMenu m = new System.Windows.Forms.ContextMenu();
        public uiERPTasksLight()
        {
            InitializeComponent();
            // DoOnLoad();
        }
        public uiERPTasksLight(frmHome homeUI)
        {
            InitializeComponent();
            this.g_homeUI = homeUI;
           // DoOnLoad();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            if (objERPDba == null)
                objERPDba = new ERPDba();
            if (objVLCDba == null)
                objVLCDba = new VLCDba();
            try
            {


                hostListView.Child = wgrid;
               // wgrid.dataGridView1.DataSource = GetERPTasks("").DefaultView;
                objDataTable = GetERPTasks();
                if (objDataTable != null && objDataTable.Columns.Count != 0)
                {
                    wgrid.dataGridView1.DataSource = objDataTable.DefaultView;
                }
                //RefreshGrid();
                SetGridColumnSize();
                ColumnStatus();
                ChangeProcessingRecordBackColor();
                DoOnLoad();
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
            finally
            {
            }
           
            
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DoOnUnload();
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
        }
         void DoOnLoad()
        {
            if (objDataTable == null)
                objDataTable = new DataTable();
          
            wgrid.dataGridView1.DoubleClick += new EventHandler(dataGridView1_DoubleClick);
            wgrid.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(dataGridView1_Scroll);
            wgrid.dataGridView1.Click += new EventHandler(dataGridView1_Click);
            wgrid.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);

          


            SetVLCDyanmicEnableStatus();
            SetVLCDyanmicExitEnableStatus();
            initializeVLCMode();
           
            SetPathPriorityCheckOnScreen();
            SetPathLockCheckOnScreen();
            SetEESDynamicExitCheckOnScreen();
            
            SetPeakHourCheckOnScreen();
            SetEntryRotaionCheckOnScreen();
            InitializeTimer();
        }
         
         void DoOnUnload()
         {
            

        
             wgrid.dataGridView1.DoubleClick -= new EventHandler(dataGridView1_DoubleClick);
             wgrid.dataGridView1.Scroll -= new System.Windows.Forms.ScrollEventHandler(dataGridView1_Scroll);
             wgrid.dataGridView1.Click -= new EventHandler(dataGridView1_Click);
             wgrid.dataGridView1.MouseDown -= new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
             timerToUpdateGrid.Stop();
             timerToUpdateGrid.Dispose();

          

             timerToUpdateGridData.Stop();
             timerToUpdateGridData.Dispose();
            
         }


        public void InitializeTimer()
         {
             try
             {
                 
                 this.timerToUpdateGrid = new System.Timers.Timer();
                 this.timerToUpdateGrid.Enabled = true;
                 this.timerToUpdateGrid.Interval = 4000;
                 this.timerToUpdateGrid.Start();
                 this.timerToUpdateGrid.Elapsed += new System.Timers.ElapsedEventHandler(timerToUpdateGrid_Elapsed);


               

                 this.timerToUpdateGridData = new System.Timers.Timer();
                 this.timerToUpdateGridData.Enabled = true;
                 this.timerToUpdateGridData.Interval = 5000;
                 this.timerToUpdateGridData.Start();
                 this.timerToUpdateGridData.Elapsed += new System.Timers.ElapsedEventHandler(RefreshGridData);
               
                 
             }
             catch (Exception errMsg)
             {
                 MessageBox.Show(errMsg.Message);
             }
         }
        void timerToUpdateGrid_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                StopTimer(timerToUpdateGrid);

                RefreshGrid();
                
                RefreshIndication();

                StartTimer(timerToUpdateGrid);
            }
            catch (Exception ex)
            {
                StartTimer(timerToUpdateGrid);
            }
            finally
            {

            }


        }
        void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                frmErpDetail frm = new frmErpDetail();

                frm.OnDelete += (snd, evt) =>
                {
                    frm.Close();
                    string[] result = Convert.ToString(snd).Split(',');
                    if (result != null && result.Length > 0)
                    {
                        Int32 queueId = 0;
                        string mode = "";

                        Int32.TryParse(Convert.ToString(result[0]), out queueId);
                        mode = result[1];

                        DeleteTransaction(queueId, mode);
                    }
                };

                frm.OnComplete += (snd, evt) => {
                    frm.Close();
                    string[] result = Convert.ToString(snd).Split(',');
                    if (result != null && result.Length > 0)
                    {
                        Int32 queueId = 0;
                        string mode = "";

                        Int32.TryParse(Convert.ToString(result[0]), out queueId);
                        mode = result[1];
                        CompleteTransaction(queueId, mode);
                    }                
                };

            

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
                   // frm.Timer = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["TIMER"].Value);
                    frm.Rotation = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["ROTATION"].Value);

                    frm.Gate = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["GATE"].Value);
                    frm.Cmd = Convert.ToString(wgrid.dataGridView1.SelectedRows[0].Cells["L2 CMD"].Value);
                    frm.setTriggerStatus();
                    frm.SetHoldStatus();
                }

                frm.Show();

            }
            catch (Exception errMsg)
            {

            }
            finally { 
            
            }
            
        }

      

         DataTable GetERPTasks()
        {

            return objERPDba.GetERPTasks(GetQueryForERP());
            
        }
        private string GetQueryForERP()
        {
            string query = "select * from CURRENT_REQUEST_VIEW where 1=1";
           
           
            return query;
        }

        void ChangeProcessingRecordBackColor()
        {
            if (objNewConnection == null) objNewConnection = new Connection();
            if (objEESDba == null)
                objEESDba = new EESDba();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            
            int queueId = 0;
            try
            {
                if (wgrid.dataGridView1.DataSource != null)
                {
                    //for (int i = 0; i <= wgrid.dataGridView1.Rows.Count - 1; i++)
                    //{
                    //    if (Convert.ToString(wgrid.dataGridView1.Rows[i].Cells["PROCESS"].Value) == "REHNDLING")
                    //    {
                    //        wgrid.dataGridView1.Rows[i].Cells["PROCESS"].Style.ForeColor = System.Drawing.Color.White;
                    //        wgrid.dataGridView1.Rows[i].Cells["PROCESS"].Style.BackColor = System.Drawing.Color.DarkRed;
                    //    }
                    //}

                    int pathLockId=objGeneralDba.GetPathlockId();

                    for (int i = 0; i <= wgrid.dataGridView1.Rows.Count - 1; i++)
                    {
                        var grid = wgrid.dataGridView1;

                        var processCell = grid.Rows[i].Cells["PROCESS"];
                        if (!Convert.ToString(grid.Rows[i].Cells["PROCESS"].Value).Equals("PROCESSING"))
                        {
                            processCell.Style.ForeColor = System.Drawing.Color.White;
                            processCell.Style.SelectionForeColor = System.Drawing.Color.White;
                            processCell.Style.BackColor = System.Drawing.Color.DarkCyan;
                            processCell.Style.SelectionBackColor = System.Drawing.Color.DarkCyan;
                        }
                        
                        var modeCell = grid.Rows[i].Cells["MODE"];
                        if (Convert.ToString(modeCell.Value) == "ENTRY")
                        {
                           
                            modeCell.Style.ForeColor = System.Drawing.Color.DarkGreen;
                        }
                        else if (Convert.ToString(modeCell.Value) == "EXIT")
                        {
                           
                            modeCell.Style.ForeColor = System.Drawing.Color.DarkRed;
                            
                        }

                        var qCell=grid.Rows[i].Cells["TRANS ID"];
                        var snoCell = grid.Rows[i].Cells["S_NO"];
                        queueId = int.Parse(Convert.ToString(qCell.Value));
                        if (objNewConnection.IsTriggerEnabledUsingQueueId(queueId))
                        {
                            snoCell.Style.BackColor = System.Drawing.Color.Red;
                            snoCell.Style.SelectionBackColor = System.Drawing.Color.Red;

                        }
                        if (Convert.ToString(grid.Rows[i].Cells["HOLD_FLAG"].Value).Equals("1"))
                        {
                            snoCell.Style.BackColor = System.Drawing.Color.Yellow;
                            snoCell.Style.SelectionBackColor = System.Drawing.Color.Yellow;
                        }



                        var washCell = grid.Rows[i].Cells["CAR WASH"];
                        if (Convert.ToString(washCell.Value) == "YES")
                        {

                            washCell.Style.BackColor = System.Drawing.Color.BlueViolet;
                        }

                        var timerCell = grid.Rows[i].Cells["TIMER"];
                        Int32 tempVal=0;
                        Int32.TryParse(timerCell.Value.ToString().Split(':')[0], out tempVal);
                        if (tempVal >= 9)
                        {

                            timerCell.Style.ForeColor = System.Drawing.Color.Red;
                            timerCell.Style.SelectionForeColor = System.Drawing.Color.Red;
                        }

                         var cmdCell = grid.Rows[i].Cells["L2 CMD"];

                         if (Convert.ToString(cmdCell.Value).Equals("Payment"))
                         {

                             gateForGrid = Convert.ToString(grid.Rows[i].Cells["GATE"].Value);
                             char c = gateForGrid[gateForGrid.Length - 1];
                             wgrid.BeginInvoke(new InvokeDelegate(new Action(() =>
                                   {
                                       objEESDba.getEESParameters(c - '0', out eesChannelForGrid, out eesCodeForGrid);
                                       if (GetInnerDoorBlockedStatus(eesChannelForGrid, eesCodeForGrid))
                                       {
                                           cmdCell.Style.BackColor = System.Drawing.Color.Red;
                                           cmdCell.Style.SelectionBackColor = System.Drawing.Color.Red;
                                       }
                                       //else
                                       //{
                                       //    cmdCell.Style.BackColor = System.Drawing.Color.Green;
                                       //    cmdCell.Style.SelectionBackColor = System.Drawing.Color.Green;
                                       //}
                                   })));
                         }
                        
                        

                         var transIdCell = grid.Rows[i].Cells["TRANS ID"];
                         if (pathLockId !=0 && Convert.ToString(transIdCell.Value).Equals(pathLockId.ToString()))
                         {

                             transIdCell.Style.BackColor = System.Drawing.Color.LightCyan;
                             transIdCell.Style.SelectionBackColor = System.Drawing.Color.LightCyan;
                             
                         }
                        
                        
                    }
                   
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine();
            }
            finally { }
        }
       
        void SetGridColumnSize()
        {
            try
            {
                
               // var font=wgrid.dataGridView1.DefaultCellStyle["RETRIEVAL TYPE"].
                wgrid.dataGridView1.Columns["S_NO"].Width = 35;
                wgrid.dataGridView1.Columns["TIMER"].Width = 50;
                wgrid.dataGridView1.Columns["NAME"].Width = 90;

                wgrid.dataGridView1.Columns["PLATE NO#"].Width = 50;
                wgrid.dataGridView1.Columns["MODE"].Width = 40;

                wgrid.dataGridView1.Columns["TYPE"].Width = 37;

                wgrid.dataGridView1.Columns["GATE"].Width = 38;
                wgrid.dataGridView1.Columns["LCM"].Width = 38;
                wgrid.dataGridView1.Columns["VLC"].Width = 38;

                wgrid.dataGridView1.Columns["UCM"].Width = 38;
                wgrid.dataGridView1.Columns["FLOOR"].Width = 42;
                wgrid.dataGridView1.Columns["AISLE"].Width = 40;
                wgrid.dataGridView1.Columns["ROW"].Width = 35;
                wgrid.dataGridView1.Columns["ROTATION"].Width = 65;
                wgrid.dataGridView1.Columns["L2 CMD"].Width = 100;
                wgrid.dataGridView1.Columns["CAR AT"].Width = 42;
                wgrid.dataGridView1.Columns["PROCESS"].Width = 130;

                wgrid.dataGridView1.Columns["CAR WASH"].Width = 42;
                //wgrid.dataGridView1.Columns["CUSTOMER ID"].Width = 45;
                wgrid.dataGridView1.Columns["START TIME"].Width = 130;
                wgrid.dataGridView1.Columns["WASH STATUS"].Width = 45;
                
              
                //wgrid.dataGridView1.Columns["RETRIEVAL TYPE"].Width = 130;

                //wgrid.dataGridView1.Columns["RETRIEVAL_TYPE"].Visible = false;
            }
            catch (Exception errMsg)
            {
                Console.Write("" + errMsg);
            
            }
            finally { }

        }

        void ColumnStatus()
        {
            try
            {
                wgrid.dataGridView1.Columns["S_NO"].DisplayIndex = 0;
                wgrid.dataGridView1.Columns["TIMER"].DisplayIndex = 1;

                wgrid.dataGridView1.Columns["NAME"].DisplayIndex = 2;

                wgrid.dataGridView1.Columns["PLATE NO#"].DisplayIndex = 3;
                wgrid.dataGridView1.Columns["MODE"].DisplayIndex = 4;
                wgrid.dataGridView1.Columns["TYPE"].DisplayIndex = 5;

                wgrid.dataGridView1.Columns["GATE"].DisplayIndex = 6;
                wgrid.dataGridView1.Columns["LCM"].DisplayIndex = 7;
                wgrid.dataGridView1.Columns["VLC"].DisplayIndex = 8;

                wgrid.dataGridView1.Columns["UCM"].DisplayIndex = 9;
                wgrid.dataGridView1.Columns["FLOOR"].DisplayIndex = 10;
                wgrid.dataGridView1.Columns["AISLE"].DisplayIndex = 11;
                wgrid.dataGridView1.Columns["ROW"].DisplayIndex = 12;
                wgrid.dataGridView1.Columns["ROTATION"].DisplayIndex = 13;
                wgrid.dataGridView1.Columns["L2 CMD"].DisplayIndex = 14;
                wgrid.dataGridView1.Columns["CAR AT"].DisplayIndex = 15;
                wgrid.dataGridView1.Columns["PROCESS"].DisplayIndex = 16;

                wgrid.dataGridView1.Columns["CAR WASH"].DisplayIndex = 17;
                wgrid.dataGridView1.Columns["CUSTOMER ID"].DisplayIndex = 18;
                wgrid.dataGridView1.Columns["START TIME"].DisplayIndex = 19;
                wgrid.dataGridView1.Columns["WASH STATUS"].DisplayIndex = 20;
                wgrid.dataGridView1.Columns["TRANS ID"].DisplayIndex = 21;
               // wgrid.dataGridView1.Columns["TRANS ID"].Visible = false;
                wgrid.dataGridView1.Columns["HOLD_FLAG"].Visible = false;
                //wgrid.dataGridView1.Columns["RETRIEVAL_TYPE"].Visible = false;
            }
            catch (Exception errMsg)
            { }
            finally { 
            
            }
        }

        void RefreshGrid()
        {
            try
            {
                if (wgrid.dataGridView1.IsHandleCreated == false) return;

                wgrid.BeginInvoke(new InvokeDelegate(new Action(() =>
                {
                    try
                    {
                      
                        //  wgrid.dataGridView1.SuspendLayout();
                        if (objDataTable != null && objDataTable.Columns.Count!=0)
                        {
                            

                            

                            wgrid.dataGridView1.DataSource = objDataTable;

                          

                            ChangeProcessingRecordBackColor();
                             
                           
                            //ColumnStatus();
                        }
                       // SetGridColumnSize();
                        //ColumnStatus();
                    }
                    catch (Exception errMsg)
                    {
                       // lblErrorMsg.Text = errMsg.Message;
                    }
                    finally
                    {
                        //  wgrid.dataGridView1.ResumeLayout();
                       // wgrid.dataGridView1.Refresh();
                        //this.timerToUpdateGrid.Enabled = true;
                       // if (this.timerToUpdateGrid == null) InitializeTimer();
                    }
                })));
            }
            catch (Exception errMsg)
            {
                //lblErrorMsg.Text = errMsg.Message;
            }
            finally {
                //if (this.timerToUpdateGrid == null) InitializeTimer();
            }

        }

        public void Dispose()
        {
            try
            {
               // this.g_homeUI.OnErpRefresh -= new EventHandler(g_homeUI_OnErpRefresh);
                //Commented on : 20-May-2013
                //this.g_homeUI.OnTriggerEESQueueNotificaiton -= g_homeUI_OnTriggerEESQueueNotificaiton;
                //this.g_homeUI.OnTriggerSlotPathNotificaiton -= g_homeUI_OnTriggerSlotPathNotificaiton;
                //this.g_homeUI.OnTriggerCustomerNotification -= g_homeUI_OnTriggerCustomerNotification;

                //if (timerToUpdateGrid != null)
                //{
                //    timerToUpdateGrid.Stop();
                //    timerToUpdateGrid.Dispose();
                //    timerToUpdateGrid = null;
                //}
            }
            catch (Exception errMsg)
            {
                MessageBox.Show("Error occured on unregister oracle notificaiton", "ERP Task", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDeleteTrans_Click(object sender, RoutedEventArgs e)
        {
            int queueId = 0;
            string mode = "";
            try
            {
                if (wgrid.dataGridView1.DataSource != null && wgrid.dataGridView1.SelectedRows.Count > 0)
                {
                    Int32.TryParse(wgrid.dataGridView1.SelectedRows[0].Cells["TRANS ID"].Value.ToString(), out queueId);
                    mode = wgrid.dataGridView1.SelectedRows[0].Cells["MODE"].Value.ToString();
                }

                DeleteTransaction(queueId, mode);

             
            }
            catch (Exception errMsg)
            { }
        }

        private void btnCompleteTrans_Click(object sender, RoutedEventArgs e)
        {
            int queueId = 0;
            string mode = "";
            try
            {
                if (wgrid.dataGridView1.DataSource != null && wgrid.dataGridView1.SelectedRows.Count > 0)
                {
                    Int32.TryParse(wgrid.dataGridView1.SelectedRows[0].Cells["TRANS ID"].Value.ToString(), out queueId);
                    mode = wgrid.dataGridView1.SelectedRows[0].Cells["MODE"].Value.ToString();
                }

                CompleteTransaction(queueId, mode);

               
            }
            catch (Exception errMsg)
            { }
        }

        private void btnERPTrans_Click(object sender, RoutedEventArgs e)
        {
            this.g_homeUI.homeui_triggerPMSTransTask(sender, e);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            this.g_homeUI.homeui_triggerPMSTransTask(sender, e);
        }

        private void rbtnAll_Checked(object sender, RoutedEventArgs e)
        {
            objDataTable = GetERPTasks();
            RefreshGrid();
        }

        private void rbtnEntry_Checked(object sender, RoutedEventArgs e)
        {
            objDataTable = GetERPTasks();
            RefreshGrid();
        }

        private void rbtnExit_Checked(object sender, RoutedEventArgs e)
        {
            objDataTable = GetERPTasks();
            RefreshGrid();
        }

        private void rbtnWash_Checked(object sender, RoutedEventArgs e)
        {
            objDataTable = GetERPTasks();
            RefreshGrid();
        }

        private void hostListView_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

       
      

        void RefreshIndication()
        {
            try
            {
                refreshRect.Dispatcher.BeginInvoke(new Action(() =>
                    {
                       // refreshRect.Visibility = isRefreshChange ? Visibility.Visible : Visibility.Hidden;
                        refreshRect.Fill = isRefreshChange ? refreshBrush1 : refreshBrush2;
                        isRefreshChange = !isRefreshChange;
                    }));
            }
            catch (Exception errMsg)
            {
               // MessageBox.Show(errMsg.Message);
            }

        }

        void DeleteTransaction(int queueid, string mode)
        {
            try
            {
                if (mode.Contains("ENTRY") || mode.Contains("EXIT")
                    || mode.Contains("TRANSFER") || mode.Contains("REHANDLE") || mode.Contains("WASH"))
                {
                    if (MessageBox.Show("Do you want to delete Transaction : " + queueid + "?", "ERP Task", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        if (queueid > 0)
                        {
                            objERPDba.DeleteTransaction(queueid);
                            RefreshGrid();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("This operation is not valid for this mode", "ERP Task", MessageBoxButton.YesNo, MessageBoxImage.Information);
                }

            }
            catch (Exception errMsg)
            {

            }
        }

        void CompleteTransaction(int queueid, string mode)
        {
            try
            {
                if (mode.Contains("ENTRY") || mode.Contains("EXIT")
                    || mode.Contains("TRANSFER") || mode.Contains("REHANDLE") || mode.Contains("WASH"))
                {
                    if (MessageBox.Show("Do you want to complete Transaction : " + queueid + "?", "ERP Task", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        objERPDba.CompleteTransaction(queueid);
                        RefreshGrid();
                       
                    }
                }
                else
                {
                    MessageBox.Show("Complete operation needs a valid transaction", "ERP Task", MessageBoxButton.YesNo, MessageBoxImage.Information);
                }
            }
            catch (Exception errMsg)
            {


            }
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            
            objERPDba.UpdateVLCDynamicEntryStatus(vlcDynamicCheck.IsChecked == true);
            SetVLCDyanmicEnableStatus();
        }
        void SetVLCDyanmicEnableStatus()
        {
            try
            {
                if (IsVLCDynamicEnabled())
                {
                    vlcDynamicCheck.IsChecked = true;
                    vlcDynamicCheck.Foreground = new SolidColorBrush(Colors.Green);

                }
                else
                {
                    vlcDynamicCheck.IsChecked = false;
                    vlcDynamicCheck.Foreground = new SolidColorBrush(Colors.Blue);
                }
            }
            catch { }
        }
        bool IsVLCDynamicEnabled()
        {
           return  objERPDba.GetVLCDynamicEntryStatus();
        }

        /// <summary>
        /// vlc dynamic exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void vlcDynamicExitCheck_Checked(object sender, RoutedEventArgs e)
        {
          
            objERPDba.UpdateVLCDynamicExitStatus(vlcDynamicExitCheck.IsChecked == true);
            SetVLCDyanmicExitEnableStatus();
        }
        void SetVLCDyanmicExitEnableStatus()
        {
            if (IsVLCDynamicExitEnabled())
            {
                vlcDynamicExitCheck.IsChecked = true;
                vlcDynamicExitCheck.Foreground = new SolidColorBrush(Colors.Green);

            }
            else
            {
                vlcDynamicExitCheck.IsChecked = false;
                vlcDynamicExitCheck.Foreground = new SolidColorBrush(Colors.Blue);
            }
        }
        bool IsVLCDynamicExitEnabled()
        {
            return objERPDba.GetVLCDynamicExitStatus();
        }

       

        public string GetUpdatedGradiantBrush(EESData objEESData)
        {
            bool isPutting = false;
            bool isGetting = false;
            
            string returnBrush = "redGradiantBrush";
            isPutting = objConnection.IsPSPuttingEES(objEESData.machineCode);
            isGetting=objConnection.IsLCMGettingEES(objEESData.machineCode);
            //if (!isPutting)
            //    needBlinkForEES[objEESData.eesName] = false;
            if (isPutting)
            {

                returnBrush = "yellowGradiantBrush";
            }
            else if(isGetting)
            {
                returnBrush = "orangeGradiantBrush";
               // needBlinkForEES[objEESData.eesName] = true;

            }
            else
            {
              

                if (IsEESReadyForCarEntry(objEESData))
                    returnBrush = "greenGradiantBrush";
                else
                    returnBrush = "redGradiantBrush";


            }
            return returnBrush;

        }
        
       
        public bool IsPalletPresentOnEES(Model.EESData objEESData)
        {
            bool isPresent = false;
            if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();

            isPresent = objOPCServerDirector.ReadTag<bool>(objEESData.machineChannel + "." + objEESData.machineCode + ".Pallet_Present_Prox_NE");
            isPresent = isPresent || objOPCServerDirector.ReadTag<bool>(objEESData.machineChannel + "." + objEESData.machineCode + ".Pallet_Present_Prox_SW");


            return isPresent;

        }
        public bool IsEESEntryInOPC(EESData objEESData)
        {

            bool isEntry = false;
            int ees_mode = 0;
            if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
            ees_mode = objOPCServerDirector.ReadTag<UInt16>(objEESData.machineChannel + "." + objEESData.machineCode + ".EES_Mode");
            isEntry = (ees_mode == 1);
            return isEntry;
        }
        public bool IsEESReadyForCarEntry(EESData objEESData)
        {

            bool isReady = false;
            int ees_state = 0;
            if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
            ees_state = objOPCServerDirector.ReadTag<Int16>(objEESData.machineChannel + "." + objEESData.machineCode + ".State_EES_HMI");
            isReady = (ees_state == 100);
            return isReady;
        }

        /// <summary>
        /// VLC Mode
        /// </summary>
        /// <param name="machineName"></param>
        /// <param name="setValue"></param>
       
        void radioChecked_VLCModeDialog(object sender, EventArgs e)
        {
            Dictionary<string, string> vlcDet = (Dictionary<string, string>)sender;
            string vlcName = vlcDet["vlcName"];
            int vlcMode = int.Parse(vlcDet["vlcMode"]);
            objVLCDba.saveVLCMode(vlcName, vlcMode);
            
            setVLCModeBrush(vlcName);

        }
        void openVLCModeDialog(string machineName)
        {
            VLCModeDialog objVLCModeDialog = new VLCModeDialog();
            objVLCModeDialog.vlcName = machineName;
            int vlcMode = objVLCDba.getVLCMode(machineName); 
            objVLCModeDialog.updateModeEvent += new EventHandler(radioChecked_VLCModeDialog);
            objVLCModeDialog.initializeRadio(vlcMode);
            //objVLCModeDialog.po
            objVLCModeDialog.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            objVLCModeDialog.ShowDialog();

        }
        void initializeVLCMode()
        {
            VLC1.vlcBut.Content = "VLC1";
            VLC2.vlcBut.Content = "VLC2";
            VLC3.vlcBut.Content = "VLC3";
            VLC4.vlcBut.Content = "VLC4";
            VLC5.vlcBut.Content = "VLC5";
            VLC6.vlcBut.Content = "VLC6";
             string[] vlcArray = { "VLC1", "VLC2", "VLC3", "VLC4", "VLC5", "VLC6" };
            
            foreach (string vlcName in vlcArray)
            {
                setVLCModeBrush(vlcName);
            }
        }
        void setVLCModeBrush(string vlcName)
        {

            int vlcMode = 0;
            string gradiantBrush = null;
            if (statusGradient == null) statusGradient = new LinearGradientBrush();

            vlcMode = objVLCDba.getVLCMode(vlcName);
                if(vlcMode==0)
                {
                    gradiantBrush = "yellowGradiantBrush";
                }
                else if(vlcMode==1)
                {
                    gradiantBrush = "greenGradiantBrush";
                }
                else if (vlcMode == 2)
                {
                    gradiantBrush = "redGradiantBrush";
                }
                statusGradient = (LinearGradientBrush)this.Resources[gradiantBrush];
                if (vlcName == "VLC1")
                    VLC1.vlcBut.Background = statusGradient;
                else if (vlcName == "VLC2")
                    VLC2.vlcBut.Background = statusGradient;
                else if (vlcName == "VLC3")
                    VLC3.vlcBut.Background = statusGradient;
                else if (vlcName == "VLC4")
                    VLC4.vlcBut.Background = statusGradient;
                else if (vlcName == "VLC5")
                    VLC5.vlcBut.Background = statusGradient;
                else if (vlcName == "VLC6")
                    VLC6.vlcBut.Background = statusGradient;
        }


      

      

        private void VLC1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            

            VLCModeControl cntrl = (VLCModeControl)sender;
            
          //  openVLCModeDialog(cntrl.Name);
            string machineName = cntrl.Name;

             VLCModeDialog objVLCModeDialog = new VLCModeDialog();
            objVLCModeDialog.vlcName = machineName;
            int vlcMode = objVLCDba.getVLCMode(machineName);
            objVLCModeDialog.updateModeEvent += new EventHandler(radioChecked_VLCModeDialog);
            objVLCModeDialog.initializeRadio(vlcMode);
            objVLCModeDialog.StartPosition = System.Windows.Forms.FormStartPosition.Manual;

            Int32 relx=Math.Abs(Convert.ToInt32(cntrl.PointFromScreen(new System.Windows.Point(0, 0)).X));
            Int32 relY=Math.Abs(Convert.ToInt32(cntrl.PointFromScreen(new System.Windows.Point(0, 0)).Y));
            objVLCModeDialog.Left = relx - (objVLCModeDialog.Width/2);
           objVLCModeDialog.Top = relY + Convert.ToInt32(cntrl.Height);
           objVLCModeDialog.ShowDialog();
           
        }
      
        /// <summary>
        /// Path Priority Status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
      

     
       private void SetPathPriorityCheckOnScreen()
       {
           if (objERPDba.GetPathPriorityStatus())
           {
               pathPriorityCheck.IsChecked = true;
               pathPriorityCheck.Foreground = new SolidColorBrush(Colors.Green);

           }
           else
           {
               pathPriorityCheck.IsChecked = false;
               pathPriorityCheck.Foreground = new SolidColorBrush(Colors.Blue);
           }
       }
       private void pathPriorityCheck_Checked(object sender, RoutedEventArgs e)
       {
           objERPDba.SetPathPriorityStatus(true);
           SetPathPriorityCheckOnScreen();
       }

       private void pathPriorityCheck_Unchecked(object sender, RoutedEventArgs e)
       {
           objERPDba.SetPathPriorityStatus(false);
           SetPathPriorityCheckOnScreen();
       }

        /// <summary>
        /// stop refresh while scrolling
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
       private void dataGridView1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e)
       {
           if (timerToUpdateGrid.Enabled)
               timerToUpdateGrid.Enabled = false;
       }
       private void dataGridView1_Click(object sender, EventArgs e)
       {
           if (!timerToUpdateGrid.Enabled)
               timerToUpdateGrid.Enabled = true;
       }

      

       private void Image_MouseDown(object sender, EventArgs e)
       {
           winERPTasks objwinERPTasks = new winERPTasks();
           objwinERPTasks.Show();
       }

     
       /// <summary>
       /// EES Dynamic Exit Enbaled Status
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>


     
       private void SetEESDynamicExitCheckOnScreen()
       {
           if (objERPDba.GetEESDynamicExitStatus())
           {
               eesDynamicExitCheck.IsChecked = true;
               eesDynamicExitCheck.Foreground = new SolidColorBrush(Colors.Green);

           }
           else
           {
               eesDynamicExitCheck.IsChecked = false;
               eesDynamicExitCheck.Foreground = new SolidColorBrush(Colors.Blue);
           }
       }
       private void eesDynamicExit_Checked(object sender, RoutedEventArgs e)
       {
           objERPDba.SetEESDynamicExitStatus(true);
           SetEESDynamicExitCheckOnScreen();
       }

       private void eesDynamicExitCheck_Unchecked(object sender, RoutedEventArgs e)
       {
           objERPDba.SetEESDynamicExitStatus(false);
           SetEESDynamicExitCheckOnScreen();
       }


        /// <summary>
        /// waiting for EES
        /// </summary>
        /// <returns></returns>
       
       string GetWaitingTime(int eesNum)
       {
           string waitingTime = null;
           DateTime? carOutsideTime =objERPDba.GetCarOutsideTime(eesNum);
           DateTime? eesReadyTime = objERPDba.GetEESReadyTime(eesNum);
           TimeSpan? t1 = null;
           if (carOutsideTime != null)  
           {
               if (carOutsideTime < eesReadyTime)
               {
                   t1 = eesReadyTime - carOutsideTime;
                   waitingTime = t1.Value.Minutes + ":" + t1.Value.Seconds;
               }
               else
               {
                   t1 = DateTime.Now - carOutsideTime;
                   waitingTime = t1.Value.Minutes + ":" + t1.Value.Seconds;
               }
           }

           return waitingTime;
       }
    
       public bool GetVehicleDetectorStatus(Model.EESData objEESData)
       {
           bool status = false;
           if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();

           status = objOPCServerDirector.ReadTag<bool>(objEESData.machineChannel + "." + objEESData.machineCode + ".Vehicle_Detector");


           return status;

       }

     
       private void StartTimer(System.Timers.Timer timer)
       {
           try
           {
               timer.Start();
           }
           catch (Exception ex)
           {

           }
       }
       private void StopTimer(System.Timers.Timer timer)
        {
            try
            {
                timer.Stop();
            }
            catch (Exception ex)
            {

            }
        }

       private void RefreshGridData(object sender, System.Timers.ElapsedEventArgs e)
       {
           try
           {
               StopTimer(timerToUpdateGridData);
               objDataTable = GetERPTasks();
               StartTimer(timerToUpdateGridData);
           }
           catch (Exception errMsg)
           {
               StartTimer(timerToUpdateGridData);
           }
       }

       private void pathlockCheck_Checked(object sender, RoutedEventArgs e)
       {
           objERPDba.SetPathLockStatus(true);
           SetPathLockCheckOnScreen();
       }

       private void pathlockCheck_Unchecked(object sender, RoutedEventArgs e)
       {
           objERPDba.SetPathLockStatus(false);
           SetPathLockCheckOnScreen();
       }
      
       private void SetPathLockCheckOnScreen()
       {
           if (objERPDba.GetPathLockStatus())
           {
               pathlockCheck.IsChecked = true;
               pathlockCheck.Foreground = new SolidColorBrush(Colors.Green);

           }
           else
           {
               pathlockCheck.IsChecked = false;
               pathlockCheck.Foreground = new SolidColorBrush(Colors.Blue);
           }
       }

       private void EES_MouseDown(object sender, MouseButtonEventArgs e)
       {
           Ellipse objEllipse = (Ellipse)sender;
           int gateNumber = 0;
           if (int.TryParse(objEllipse.Name[objEllipse.Name.Length - 1].ToString(), out gateNumber))
           {
               kioskDataForm objForm = new kioskDataForm(gateNumber);
               objForm.Show();
           }
       }

       private void peak_slot_check_Checked(object sender, RoutedEventArgs e)
       {
           objERPDba.SetPeakHourEnabledStatus(true);
           SetPeakHourCheckOnScreen();
       }

       private void peak_slot_check_Unchecked(object sender, RoutedEventArgs e)
       {
           objERPDba.SetPeakHourEnabledStatus(false);
           SetPeakHourCheckOnScreen();
       }
      
       private void SetPeakHourCheckOnScreen()
       {
           if (objERPDba.GetPeakHourEnabledStatus())
           {
               peak_slot_check.IsChecked = true;
               peak_slot_check.Foreground = new SolidColorBrush(Colors.Green);

           }
           else
           {
               peak_slot_check.IsChecked = false;
               peak_slot_check.Foreground = new SolidColorBrush(Colors.Blue);
           }
       }

       private void entry_rot_check_Checked(object sender, RoutedEventArgs e)
       {
           objERPDba.SetEntryRotaionEnabledStatus(true);
           SetEntryRotaionCheckOnScreen();
       }

       private void entry_rot_check_Unchecked(object sender, RoutedEventArgs e)
       {
           objERPDba.SetEntryRotaionEnabledStatus(false);
           SetEntryRotaionCheckOnScreen();
       }
      
       private void SetEntryRotaionCheckOnScreen()
       {
           if (objERPDba.GetEntryRotaionEnabledStatus())
           {
               entry_rot_check.IsChecked = true;
               entry_rot_check.Foreground = new SolidColorBrush(Colors.Green);

           }
           else
           {
               entry_rot_check.IsChecked = false;
               entry_rot_check.Foreground = new SolidColorBrush(Colors.Blue);
           }
       }
       public bool GetInnerDoorBlockedStatus(string eesChannel,string eesCode)
       {
           bool status = false;
           if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();

           status = !objOPCServerDirector.ReadTag<bool>(eesChannel + "." + eesCode +"."+ OpcTags.EES_North_Side_Area_Laser_Blocked);


           return status;

       }
       private void dataGridView1_MouseDown(object sender,  System.Windows.Forms.MouseEventArgs e)
       {
          

           if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                
              
              
                int currentMouseOverRow = wgrid.dataGridView1.HitTest(e.X,e.Y).RowIndex;

                if (currentMouseOverRow >= 0)
                {
                  
                    wgrid.dataGridView1.Rows[currentMouseOverRow].Selected = true;
                    System.Windows.Forms.DataGridViewRow row = wgrid.dataGridView1.Rows[currentMouseOverRow];
                    string queueId=row.Cells["TRANS ID"].Value.ToString();
                    wgrid.dataGridView1.Focus();
               
                    System.Windows.Forms.MenuItem men=new System.Windows.Forms.MenuItem(queueId);
                    men.Enabled=false;
                  
                  

                    System.Windows.Forms.MenuItem menuItemHold = new System.Windows.Forms.MenuItem("Hold");
                    menuItemHold.Click += menuItem_Click;
                    menuItemHold.Tag = row;

                    System.Windows.Forms.MenuItem menuItemResume = new System.Windows.Forms.MenuItem("Resume");
                    menuItemResume.Click += menuItem_Click;
                    menuItemResume.Tag = row;

                    m.MenuItems.Clear();
                    m.MenuItems.Add(men);
                    m.MenuItems.Add(menuItemHold);
                    m.MenuItems.Add(menuItemResume);

                    m.Show(wgrid.dataGridView1, new System.Drawing.Point(e.X, e.Y));
                }

              

            }
       }

       void menuItem_Click(object sender, EventArgs e)
       {
           try
           {
               System.Windows.Forms.MenuItem item = (System.Windows.Forms.MenuItem)sender;
               System.Windows.Forms.DataGridViewRow row = (System.Windows.Forms.DataGridViewRow)item.Tag;
               int queueId = int.Parse(row.Cells["TRANS ID"].Value.ToString());
               switch (item.Text)
               {
                   case "Hold":
                       objERPDba.SetHoldReqFlagStatus(queueId, true);
                       break;
                   case "Resume":
                       objERPDba.SetHoldReqFlagStatus(queueId, false);
                       break;
                   default:
                       break;
               }
           }
           catch(Exception ex)
           {
               Console.WriteLine(ex.Message);
           }
          
       }
      
    }
}

