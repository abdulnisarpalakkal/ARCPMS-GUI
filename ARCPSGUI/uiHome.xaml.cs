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

using ARCPSGUI.ConfigurationUI;
using ARCPSGUI.DiagnosticScreens;
using ARCPSGUI.TransactionUI;
using ARCPSGUI.DB;
using System.Data;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using ARCPSGUI.OPC;
using System.Diagnostics;
using ARCPSGUI.utility;

namespace ARCPSGUI
{
    /// <summary>
    /// Interaction logic for uiHome.xaml
    /// </summary>
    public partial class uiHome : UserControl,IDisposable
    {
       
        // System.IO.StreamWriter ipls = null;

        public event EventHandler triggerSetPoints;
        public event EventHandler triggerPMSDiagnostic;
        public event EventHandler triggerErpTask;
        public event EventHandler triggerCurrentParks;
        public event EventHandler triggerParkingDiagnostic;

        public event EventHandler triggerFloorL9;
        public event EventHandler triggerFloorL8;
        public event EventHandler triggerFloorL7;
        public event EventHandler triggerFloorL6;
        public event EventHandler triggerFloorL5;
        public event EventHandler triggerFloorL4;
        public event EventHandler triggerFloorL3;
        public event EventHandler triggerFloorL2;
        public event EventHandler triggerFloorL1;

        public event EventHandler triggerHistory;
        public event EventHandler triggerLockStatus;
        public event EventHandler triggerVLCTask;

        public event EventHandler triggerVLC;
        public event EventHandler triggerPMS;
        public event EventHandler triggerPMSTransTask;
        public event EventHandler triggerLCMTransTask;
        public event EventHandler triggerUCMTransTask;

        public event EventHandler triggerRunTimeTable;
        public event EventHandler triggerAlarmView;
        public event EventHandler triggerErrorMasterView;

        public event EventHandler triggerConfigurationView;
        public event EventHandler triggerCarWashView;
        public event EventHandler triggerDemoMode;

        public event EventHandler triggerAuthentication;
        public event EventHandler triggerMemberDetails;

        public event EventHandler triggerAbortView;
        public event EventHandler triggerPMSUnlock;
        //new
        public event EventHandler triggerSetCMWindow;
        public event EventHandler triggerSimulation;

        frmHome g_frmHome = null;
        
        OracleConnection con = null;
        OracleDependency dep = null;

        //To show current time.
        System.Timers.Timer timerUpdateTodaysCount = null;

        public enum Machine
        {
            NotAssigned = 0,
            EES = 1,
            LCM = 2,
            VLC = 3,
            UCM = 4,
            Rotate = 5,
            PVL = 6,
            PS = 7,
            PST = 8
        }

        object synchIPLSFileWrite = new object();
        public uiHome(frmHome home)
        {
            InitializeComponent();
            
            try
            {
                SetFromIcon();
                ShowSlotSummary();
                ViewSlotDetails();
                //ProcSnapshotNotification();
                DisplayTodayCarParkRetrieval();
                this.g_frmHome = home;
               // this.timerUpdateTodaysCount = new System.Timers.Timer();
               // this.timerUpdateTodaysCount.Interval = 5000;
               // this.timerUpdateTodaysCount.Enabled = true;
               // this.timerUpdateTodaysCount.Start();

                this.g_frmHome.OnTriggerSnapShotNotification += new EventHandler(g_frmHome_OnTriggerSnapShotNotification);
               //// this.g_frmHome.OnTriggerCustomerNotification += new EventHandler(g_frmHome_OnTriggerCustomerNotification);
               // this.timerUpdateTodaysCount.Elapsed += new System.Timers.ElapsedEventHandler(timerUpdateTodaysCount_Elapsed);
            }
            catch (Exception errMsg)
            {
                //MessageBox.Show(errMsg.Message);
            }
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
            this.timerUpdateTodaysCount = new System.Timers.Timer();
            this.timerUpdateTodaysCount.Interval = 5000;
            this.timerUpdateTodaysCount.Enabled = true;
            this.timerUpdateTodaysCount.Start();
            ViewSlotDetails();
            this.g_frmHome.OnTriggerSnapShotNotification += new EventHandler(g_frmHome_OnTriggerSnapShotNotification);
            // this.g_frmHome.OnTriggerCustomerNotification += new EventHandler(g_frmHome_OnTriggerCustomerNotification);
            this.timerUpdateTodaysCount.Elapsed += new System.Timers.ElapsedEventHandler(timerUpdateTodaysCount_Elapsed);
        }
        

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.timerUpdateTodaysCount.Elapsed -= new System.Timers.ElapsedEventHandler(timerUpdateTodaysCount_Elapsed);
            this.g_frmHome.OnTriggerSnapShotNotification -= new EventHandler(g_frmHome_OnTriggerSnapShotNotification);
            StopTimer(this.timerUpdateTodaysCount);
            this.timerUpdateTodaysCount.Dispose();
           
            // this.g_frmHome.OnTriggerCustomerNotification += new EventHandler(g_frmHome_OnTriggerCustomerNotification);
            
        }
        void timerUpdateTodaysCount_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try {
            Task.Factory.StartNew(new Action(() => DisplayTodayCarParkRetrieval()));
            }
            catch (Exception errMsg)
            {
                //MessageBox.Show(errMsg.Message);
            }
        }

        void OnSnapShotNotification(object sender, OracleNotificationEventArgs eventArgs)
        {
            ViewSlotDetails();
        }
        //void g_frmHome_OnTriggerCustomerNotification(object sender, EventArgs e)
        //{
        //      // Task.Factory.StartNew(new Action(() => DisplayTodayCarParkRetrieval()));
        //}

        void DisplayElectronicBoard()
        {
            //try
            //{
            //    Connection con = null;
            //    Task.Factory.StartNew(new Action(() =>
            //    {
                   
                       
            //            /* "02" - facility (for auto or manual)
            //             * 16 digit as zero
            //             * 4 digit as occupied
            //             * 4 digit useless
            //             * 4 digit as total
            //             * 4 digit as useless
            //             * new line character.
            //             * 0200000000000000000669000008340000
            //             * sample : 02 0000000000000000 0669 0000 0834 0000
            //             */
            //            con = new Connection();
            //            con.GetTotalValidCapacity(out this.totalOccupied, out  this.totalValidCapacity);
            //            string totalOcc = this.totalOccupied.ToString().PadLeft(4, '0');
            //            using (System.IO.StreamWriter ipls = new System.IO.StreamWriter(@"Z:\\ipls.000"))
            //            {
            //                ipls.WriteLine("020000000000000000" + totalOcc + "0000" + this.totalValidCapacity + "0000");
            //                ipls.Flush();
            //                ipls.Close();
            //            }
                        
                   
            //    }));
            //}
            //catch (Exception errMsg)
            //{ 
            
            //}
            //finally
            //{ 
            
            //}
        }

        void g_frmHome_OnTriggerSnapShotNotification(object sender, EventArgs e)
        {
            ViewSlotDetails();
            DisplayElectronicBoard();
        }

        private void btnL4_Click(object sender, RoutedEventArgs e)
        {
            triggerFloorL4(sender, e);

            //frmFloorCtrlDisplay frmFloorDisplay = new frmFloorCtrlDisplay();
            //frmFloorDisplay.AddFloor(4);
            //// frmFloorDisplay.caption = "B1 Floor - Entry/Exit";
            //frmFloorDisplay.Show();
        }

        private void btnL3_Click(object sender, RoutedEventArgs e)
        {
            triggerFloorL3(sender, e);

            //frmFloorCtrlDisplay frmFloorDisplay = new frmFloorCtrlDisplay();
            //frmFloorDisplay.AddFloor(3);
            ////  frmFloorDisplay.caption = "B2 Floor - L3";
            //frmFloorDisplay.Show();
        }

        private void btnSetPoints_Click(object sender, RoutedEventArgs e)
        {
            triggerSetPoints(sender, e);
            //uiSetPoints setPoints = new uiSetPoints();
            //setPoints.Show();
        }

        private void btnTEMP_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow frmmain = new MainWindow();
            //frmmain.Show();
        }

        private void btnPMSDiagnostic_Click(object sender, RoutedEventArgs e)
        {
            triggerPMSDiagnostic(sender, e);
            //frmPalletManagementDiagnostic pmdiag = new frmPalletManagementDiagnostic();
            //pmdiag.Show();
        }

        private void btnDiagnostic_Click(object sender, RoutedEventArgs e)
        {
            triggerParkingDiagnostic(sender, e);
            //frmDiagnosticScreen frmDiagScreen = new frmDiagnosticScreen();
            //frmDiagScreen.Show();
        }

        private void btnLockStatus_Click(object sender, RoutedEventArgs e)
        {
            triggerLockStatus(sender, e);
            //frmLockStatus lockStatus = new frmLockStatus();
            //lockStatus.Show();
        }

        private void btnL5_Click(object sender, RoutedEventArgs e)
        {

            triggerFloorL5(sender, e);

            //frmFloorCtrlDisplay frmFloorDisplay = new frmFloorCtrlDisplay();
            //frmFloorDisplay.AddFloor(5);
            //  frmFloorDisplay.caption = "B2 Floor - L3";
            //frmFloorDisplay.Show();
        }

        private void btnERPTasks_Click(object sender, RoutedEventArgs e)
        {
            triggerErpTask(sender, e);
            //frmERP erp = new frmERP();
            //erp.Show();
        }

        private void btnL2_Click(object sender, RoutedEventArgs e)
        {
            triggerFloorL2(sender, e);
            //frmFloorCtrlDisplay frmFloorDisplay = new frmFloorCtrlDisplay();
            //frmFloorDisplay.AddFloor(2);
            ////  frmFloorDisplay.caption = "B2 Floor - L3";
            //frmFloorDisplay.Show();
        }

        private void btnCurrentParks_Click(object sender, RoutedEventArgs e)
        {
            triggerCurrentParks(sender, e);
        }

        private void btnL7_Click(object sender, RoutedEventArgs e)
        {
            triggerFloorL7(sender, e);
        }

        private void btnL6_Click(object sender, RoutedEventArgs e)
        {
            triggerFloorL6(sender, e);
        }

        private void btnL9_Click(object sender, RoutedEventArgs e)
        {
            triggerFloorL9(sender, e);
        }

        private void btnL8_Click(object sender, RoutedEventArgs e)
        {
            triggerFloorL8(sender, e);
        }

        private void btnL1_Click(object sender, RoutedEventArgs e)
        {
            triggerFloorL1(sender, e);
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            triggerHistory(sender, e);
        }

        private void btnVLC_Click(object sender, RoutedEventArgs e)
        {
            triggerVLC(sender, e);
        }

        private void btnPMS_Click(object sender, RoutedEventArgs e)
        {
            triggerPMS(sender, e);
        }

        private void btnPMSTasks_Click(object sender, RoutedEventArgs e)
        {
            triggerPMSTransTask(sender, e);
        }

        private void btnVLCTask_Click(object sender, RoutedEventArgs e)
        {
            triggerVLCTask(sender, e);
        }

        private void btnLCMTask_Click(object sender, RoutedEventArgs e)
        {
            triggerLCMTransTask(sender, e);
        }

        private void btnUCMTask_Click(object sender, RoutedEventArgs e)
        {
            triggerUCMTransTask(sender, e);
        }

        private void btnCarWashList_Click(object sender, RoutedEventArgs e)
        {
            triggerCarWashView(sender, e);
        }

        private void btnFloorOverview_Click(object sender, RoutedEventArgs e)
        {
            triggerRunTimeTable(sender, e);
        }

        private void btnAlarm_Click(object sender, RoutedEventArgs e)
        {
            triggerAlarmView(sender, e);
        }

     

        private void btnErrorMaster_Click(object sender, RoutedEventArgs e)
        {
            triggerErrorMasterView(sender, e);
        }

        private void btnConfiguration_Click(object sender, RoutedEventArgs e)
        {
            triggerConfigurationView(sender, e);
        }

        private void btnDemoMode_Click(object sender, RoutedEventArgs e)
        {
            triggerDemoMode(sender, e);
        }

        private void btnAuthentication_Click(object sender, RoutedEventArgs e)
        {
            triggerAuthentication(sender, e);
        }

        private void btnMemberDetails_Click(object sender, RoutedEventArgs e)
        {
            triggerMemberDetails(sender, e);
        }

        private void btnAbortView_Click(object sender, RoutedEventArgs e)
        {
            triggerAbortView(sender, e);
        }

        private void btnPMSUnLock_Click(object sender, RoutedEventArgs e)
        {
            triggerPMSUnlock(sender, e);
        }


        void ViewSlotDetails()
        {
            //GetSlotCountByLevel
            try
            {
                Connection dbcon = new Connection();
                DataTable dt = dbcon.GetSlotCountByLevel();

                Int32 floorLevel = 0;
                Int32 slotcount = 0;
                Int32 slotStatus = 0;

                int totalFreeSpaceLow = 0;
                int totalFreeSpaceHigh = 0;
                int totalUsedSpaceLow = 0;
                int totalUsedSpaceHigh = 0;
                int totalDisabledSlot = 0;
                int totalPalletBundle = 0;

                foreach (DataRow drow in dt.Rows)
                {
                    Int32.TryParse(Convert.ToString(drow["LevelNo"]), out floorLevel);
                    Int32.TryParse(Convert.ToString(drow["Tcount"]), out slotcount);
                    Int32.TryParse(Convert.ToString(drow["SlotStatus"]), out slotStatus);
                    switch (floorLevel)
                    {
                        case 1:
                            
                            if (slotStatus == 2) //free space low car
                            {
                                //lblL1FreeSpaceLow.Content = slotcount;
                                lblL1FreeSpaceLow.Dispatcher.Invoke(new Action(() => lblL1FreeSpaceLow.Content = slotcount));
                                totalFreeSpaceLow += slotcount;
                            }
                            else if (slotStatus == 3) //Convert.ToString(drow["SlotStatus"]).Contains("3")) //free space high car
                            {
                                lblL1FreeSpaceHigh.Dispatcher.Invoke(new Action(() => lblL1FreeSpaceHigh.Content = slotcount));
                               // lblL1FreeSpaceHigh.Content = slotcount;
                                totalFreeSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 4) //Convert.ToString(drow["SlotStatus"]).Contains("4")) //Used space low car
                            {
                                lblL1UsedSpaceLow.Dispatcher.Invoke(new Action(() => lblL1UsedSpaceLow.Content = slotcount));
                                //lblL1UsedSpaceLow.Content = slotcount;
                                totalUsedSpaceLow += slotcount;
                            }
                            else if (slotStatus == 5) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL1UsedSpaceHigh.Dispatcher.Invoke(new Action(() => lblL1UsedSpaceHigh.Content = slotcount));
                               // lblL1UsedSpaceHigh.Content = slotcount;
                                totalUsedSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 6)  
                            {
                                lblL1Disabled.Dispatcher.Invoke(new Action(() => lblL1Disabled.Content = slotcount));
                                totalDisabledSlot += slotcount;
                            }
                            else if (slotStatus == 7) //PB
                            {
                                lblL1PB.Dispatcher.Invoke(new Action(() => lblL1PB.Content = slotcount));
                                totalPalletBundle += slotcount;
                            }
                            break;

                        case 2:

                            if (slotStatus == 2) //(Convert.ToString(drow["SlotStatus"]).Contains("2")) //free space low car
                            {
                                lblL2FreeSpaceLow.Dispatcher.Invoke(new Action(() => lblL2FreeSpaceLow.Content = slotcount));
                               // lblL2FreeSpaceLow.Content = slotcount;
                                totalFreeSpaceLow += slotcount;
                            }
                            else if (slotStatus == 3) //(Convert.ToString(drow["SlotStatus"]).Contains("3")) //free space high car
                            {
                                lblL2FreeSpaceHigh.Dispatcher.Invoke(new Action(() => lblL2FreeSpaceHigh.Content = slotcount));
                               // lblL2FreeSpaceHigh.Content = slotcount;
                                totalFreeSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 4) //(Convert.ToString(drow["SlotStatus"]).Contains("4")) //Used space low car
                            {
                                lblL2UsedSpaceLow.Dispatcher.Invoke(new Action(() => lblL2UsedSpaceLow.Content = slotcount));
                               // lblL2UsedSpaceLow.Content = slotcount;
                                totalUsedSpaceLow += slotcount;
                            }
                            else if (slotStatus == 5) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL2UsedSpaceHigh.Dispatcher.Invoke(new Action(() => lblL2UsedSpaceHigh.Content = slotcount));
                               // lblL2UsedSpaceHigh.Content = slotcount;
                                totalUsedSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 6) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL2Disabled.Dispatcher.Invoke(new Action(() => lblL2Disabled.Content = slotcount));
                                totalDisabledSlot += slotcount;
                            }
                            else if (slotStatus == 7) //PB
                            {
                                lblL2PB.Dispatcher.Invoke(new Action(() => lblL2PB.Content = slotcount));
                                totalPalletBundle += slotcount;
                            }
                            break;

                        case 3:
                            if (slotStatus == 2) // (Convert.ToString(drow["SlotStatus"]).Contains("2")) //free space low car
                            {
                                lblL3FreeSpaceLow.Dispatcher.Invoke(new Action(() => lblL3FreeSpaceLow.Content = slotcount));
                               // lblL3FreeSpaceLow.Content = slotcount;
                                totalFreeSpaceLow += slotcount;
                            }

                            else if (slotStatus == 3) //(Convert.ToString(drow["SlotStatus"]).Contains("3")) //free space high car
                            {
                                lblL3FreeSpaceHigh.Dispatcher.Invoke(new Action(() => lblL3FreeSpaceHigh.Content = slotcount));
                                //lblL3FreeSpaceHigh.Content = slotcount;
                                totalFreeSpaceHigh += slotcount;
                            }

                            else if (slotStatus == 4) //(Convert.ToString(drow["SlotStatus"]).Contains("4")) //Used space low car
                            {
                                lblL3UsedSpaceLow.Dispatcher.Invoke(new Action(() => lblL3UsedSpaceLow.Content = slotcount));
                               // lblL3UsedSpaceLow.Content = slotcount;
                                totalUsedSpaceLow += slotcount;
                            }
                            else if (slotStatus == 5) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL3UsedSpaceHigh.Dispatcher.Invoke(new Action(() => lblL3UsedSpaceHigh.Content = slotcount));
                              //  lblL3UsedSpaceHigh.Content = slotcount;
                                totalUsedSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 6) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL3Disabled.Dispatcher.Invoke(new Action(() => lblL3Disabled.Content = slotcount));
                                totalDisabledSlot += slotcount;
                            }
                            else if (slotStatus == 7) //PB
                            {
                                lblL3PB.Dispatcher.Invoke(new Action(() => lblL3PB.Content = slotcount));
                                totalPalletBundle += slotcount;
                            }
                            break;

                        case 4:
                            if (slotStatus == 2) //(Convert.ToString(drow["SlotStatus"]).Contains("2")) //free space low car
                            {
                                lblL4FreeSpaceLow.Dispatcher.Invoke(new Action(() => lblL4FreeSpaceLow.Content = slotcount));
                               // lblL4FreeSpaceLow.Content = slotcount;
                                totalFreeSpaceLow += slotcount;
                            }

                            else if (slotStatus == 3) //(Convert.ToString(drow["SlotStatus"]).Contains("3")) //free space high car
                            {
                                lblL4FreeSpaceHigh.Dispatcher.Invoke(new Action(() => lblL4FreeSpaceHigh.Content = slotcount));
                              //  lblL4FreeSpaceHigh.Content = slotcount;
                                totalFreeSpaceHigh += slotcount;
                            }

                            else if (slotStatus == 4) //(Convert.ToString(drow["SlotStatus"]).Contains("4")) //Used space low car
                            {
                                lblL4UsedSpaceLow.Dispatcher.Invoke(new Action(() => lblL4UsedSpaceLow.Content = slotcount));
                               // lblL4UsedSpaceLow.Content = slotcount;
                                totalUsedSpaceLow += slotcount;
                            }
                            else if (slotStatus == 5) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL4UsedSpaceHigh.Dispatcher.Invoke(new Action(() => lblL4UsedSpaceHigh.Content = slotcount));
                               // lblL4UsedSpaceHigh.Content = slotcount;
                                totalUsedSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 6) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL4Disabled.Dispatcher.Invoke(new Action(() => lblL4Disabled.Content = slotcount));
                                totalDisabledSlot += slotcount;
                            }
                            else if (slotStatus == 7) //PB
                            {
                                lblL4PB.Dispatcher.Invoke(new Action(() => lblL4PB.Content = slotcount));
                                totalPalletBundle += slotcount;
                            }
                            break;

                        case 5:
                            if (slotStatus == 2) //(Convert.ToString(drow["SlotStatus"]).Contains("2")) //free space low car
                            {
                                lblL5FreeSpaceLow.Dispatcher.Invoke(new Action(() => lblL5FreeSpaceLow.Content = slotcount));
                               // lblL5FreeSpaceLow.Content = slotcount;
                                totalFreeSpaceLow += slotcount;
                            }

                            else if (slotStatus == 3) //Convert.ToString(drow["SlotStatus"]).Contains("3")) //free space high car
                            {
                                lblL5FreeSpaceHigh.Dispatcher.Invoke(new Action(() => lblL5FreeSpaceHigh.Content = slotcount));
                               // lblL5FreeSpaceHigh.Content = slotcount;
                                totalFreeSpaceHigh += slotcount;
                            }

                            else if (slotStatus == 4) //(Convert.ToString(drow["SlotStatus"]).Contains("4")) //Used space low car
                            {
                                lblL5UsedSpaceLow.Dispatcher.Invoke(new Action(() => lblL5UsedSpaceLow.Content = slotcount));
                                //lblL5UsedSpaceLow.Content = slotcount;
                                totalUsedSpaceLow += slotcount;
                            }
                            else if (slotStatus == 5) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL5UsedSpaceHigh.Dispatcher.Invoke(new Action(() => lblL5UsedSpaceHigh.Content = slotcount));
                               // lblL5UsedSpaceHigh.Content = slotcount;
                                totalUsedSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 6) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL5Disabled.Dispatcher.Invoke(new Action(() => lblL5Disabled.Content = slotcount));
                                totalDisabledSlot += slotcount;
                            }
                            else if (slotStatus == 7) //PB
                            {
                                lblL5PB.Dispatcher.Invoke(new Action(() => lblL5PB.Content = slotcount));
                                totalPalletBundle += slotcount;
                            }
                            break;

                        case 6:
                            if (slotStatus == 2) //(Convert.ToString(drow["SlotStatus"]).Contains("2")) //free space low car
                            {
                                lblL6FreeSpaceLow.Dispatcher.Invoke(new Action(() => lblL6FreeSpaceLow.Content = slotcount));
                               // lblL6FreeSpaceLow.Content = slotcount;
                                totalFreeSpaceLow += slotcount;
                            }
                            else if (slotStatus == 3) //(Convert.ToString(drow["SlotStatus"]).Contains("3")) //free space high car
                            {
                                lblL6FreeSpaceHigh.Dispatcher.Invoke(new Action(() => lblL6FreeSpaceHigh.Content = slotcount));
                              //  lblL6FreeSpaceHigh.Content = slotcount;
                                totalFreeSpaceHigh += slotcount;
                            }

                            else if (slotStatus == 4) // (Convert.ToString(drow["SlotStatus"]).Contains("4")) //Used space low car
                            {
                                lblL6UsedSpaceLow.Dispatcher.Invoke(new Action(() => lblL6UsedSpaceLow.Content = slotcount));
                              //  lblL6UsedSpaceLow.Content = slotcount;
                                totalUsedSpaceLow += slotcount;
                            }

                            else if (slotStatus == 5) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL6UsedSpaceHigh.Dispatcher.Invoke(new Action(() => lblL6UsedSpaceHigh.Content = slotcount));
                               // lblL6UsedSpaceHigh.Content = slotcount;
                                totalUsedSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 6) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL6Disabled.Dispatcher.Invoke(new Action(() => lblL6Disabled.Content = slotcount));
                                totalDisabledSlot += slotcount;
                            }
                            else if (slotStatus == 7) //PB
                            {
                                lblL6PB.Dispatcher.Invoke(new Action(() => lblL6PB.Content = slotcount));
                                totalPalletBundle += slotcount;
                            }
                            break;

                        case 7:
                            if (slotStatus == 2) //(Convert.ToString(drow["SlotStatus"]).Contains("2")) //free space low car
                            {
                                lblL7FreeSpaceLow.Dispatcher.Invoke(new Action(() => lblL7FreeSpaceLow.Content = slotcount));
                                totalFreeSpaceLow += slotcount;
                            }
                            else if (slotStatus == 3) //(Convert.ToString(drow["SlotStatus"]).Contains("3")) //free space high car
                            {
                                lblL7FreeSpaceHigh.Dispatcher.Invoke(new Action(() => lblL7FreeSpaceHigh.Content = slotcount));
                                totalFreeSpaceHigh += slotcount;
                            }

                            else if (slotStatus == 4) //(Convert.ToString(drow["SlotStatus"]).Contains("4")) //Used space low car
                            {
                                lblL7UsedSpaceLow.Dispatcher.Invoke(new Action(() => lblL7UsedSpaceLow.Content = slotcount));
                              //  lblL7UsedSpaceLow.Content = slotcount;
                                totalUsedSpaceLow += slotcount;
                            }

                            else  if (slotStatus == 5) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL7UsedSpaceHigh.Dispatcher.Invoke(new Action(() => lblL7UsedSpaceHigh.Content = slotcount));
                               // lblL7UsedSpaceHigh.Content = slotcount;
                                totalUsedSpaceHigh += slotcount;

                            }
                            else if (slotStatus == 6) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL7Disabled.Dispatcher.Invoke(new Action(() => lblL7Disabled.Content = slotcount));
                                totalDisabledSlot += slotcount;
                            }
                            else if (slotStatus == 7) //PB
                            {
                                lblL7PB.Dispatcher.Invoke(new Action(() => lblL7PB.Content = slotcount));
                                totalPalletBundle += slotcount;
                            }
                            break;

                        case 8:
                            if (slotStatus == 2) //Convert.ToString(drow["SlotStatus"]).Contains("2")) //free space low car
                            {
                                lblL8FreeSpaceLow.Dispatcher.Invoke(new Action(() => lblL8FreeSpaceLow.Content = slotcount));
                                //lblL8FreeSpaceLow.Content = slotcount;
                                totalFreeSpaceLow += slotcount;
                            }

                            else if (slotStatus == 3) //(Convert.ToString(drow["SlotStatus"]).Contains("3")) //free space high car
                            {
                                lblL8FreeSpaceHigh.Dispatcher.Invoke(new Action(() => lblL8FreeSpaceHigh.Content = slotcount));
                              //  lblL8FreeSpaceHigh.Content = slotcount;
                                totalFreeSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 4) //(Convert.ToString(drow["SlotStatus"]).Contains("4")) //Used space low car
                            {
                                lblL8UsedSpaceLow.Dispatcher.Invoke(new Action(() => lblL8UsedSpaceLow.Content = slotcount));
                               // lblL8UsedSpaceLow.Content = slotcount;
                                totalUsedSpaceLow += slotcount;
                            }

                            else if (slotStatus == 5) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL8UsedSpaceHigh.Dispatcher.Invoke(new Action(() => lblL8UsedSpaceHigh.Content = slotcount));
                              //  lblL8UsedSpaceHigh.Content = slotcount;
                                totalUsedSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 6) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL8Disabled.Dispatcher.Invoke(new Action(() => lblL8Disabled.Content = slotcount));
                                totalDisabledSlot += slotcount;
                            }
                            else if (slotStatus == 7) //PB
                            {
                                lblL8PB.Dispatcher.Invoke(new Action(() => lblL8PB.Content = slotcount));
                                totalPalletBundle += slotcount;
                            }
                            break;


                        case 9:
                            if (slotStatus ==2) //(Convert.ToString(drow["SlotStatus"]).Contains("2")) //free space low car
                            {
                                lblL9FreeSpaceLow.Dispatcher.Invoke(new Action(() => lblL9FreeSpaceLow.Content = slotcount));
                               // lblL9FreeSpaceLow.Content = slotcount;
                                totalFreeSpaceLow += slotcount;
                            }
                            else if (slotStatus == 3) //(Convert.ToString(drow["SlotStatus"]).Contains("3")) //free space high car
                            {
                                lblL9FreeSpaceHigh.Dispatcher.Invoke(new Action(() => lblL9FreeSpaceHigh.Content = slotcount));
                              //  lblL9FreeSpaceHigh.Content = slotcount;
                                totalFreeSpaceHigh += slotcount;
                            }

                            else if (slotStatus == 4) //(Convert.ToString(drow["SlotStatus"]).Contains("4")) //Used space low car
                            {
                                lblL9UsedSpaceLow.Dispatcher.Invoke(new Action(() => lblL9UsedSpaceLow.Content = slotcount));
                               // lblL9UsedSpaceLow.Content = slotcount;
                                totalUsedSpaceLow += slotcount;
                            }

                            else if (slotStatus == 5) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL9UsedSpaceHigh.Dispatcher.Invoke(new Action(() => lblL9UsedSpaceHigh.Content = slotcount));
                               // lblL9UsedSpaceHigh.Content = slotcount;
                                totalUsedSpaceHigh += slotcount;
                            }
                            else if (slotStatus == 6) //(Convert.ToString(drow["SlotStatus"]).Contains("5")) //Used space high car
                            {
                                lblL9Disabled.Dispatcher.Invoke(new Action(() => lblL9Disabled.Content = slotcount));
                                totalDisabledSlot += slotcount;
                            }
                            else if (slotStatus == 7) //PB
                            {
                                lblL9PB.Dispatcher.Invoke(new Action(() => lblL9PB.Content = slotcount));
                                totalPalletBundle += slotcount;
                            }
                            break;
                        default:
                            break;

                    }
                }

                lblTotalFreeSpaceLowCar.Dispatcher.BeginInvoke(new Action(() => lblTotalFreeSpaceLowCar.Content = totalFreeSpaceLow));
                lblTotalFreeSpaceHighCar.Dispatcher.BeginInvoke(new Action(() => lblTotalFreeSpaceHighCar.Content = totalFreeSpaceHigh));
                lblTotalUsedSpaceLowCar.Dispatcher.BeginInvoke(new Action(() => lblTotalUsedSpaceLowCar.Content = totalUsedSpaceLow));
                lblTotalUsedSpaceHighCar.Dispatcher.BeginInvoke(new Action(() => lblTotalUsedSpaceHighCar.Content = totalUsedSpaceHigh));
                lblTotalDisabled.Dispatcher.BeginInvoke(new Action(() => lblTotalDisabled.Content = totalDisabledSlot));
                lblTotalPB.Dispatcher.BeginInvoke(new Action(() => lblTotalPB.Content = totalPalletBundle));

                lblTotalCapacity.Dispatcher.BeginInvoke(new Action(() => lblTotalCapacity.Content = (totalUsedSpaceHigh + totalUsedSpaceLow) + "/1191"));
        
            
            }
            catch (Exception errMsg)
            {

            }
            finally
            {

            }
        }

        void ShowSlotSummary()
        {

            lblL9.Text = "Low Car - 135";
            lblL8.Text = "Low Car - 135";
            lblL7.Text = "Low Car - 128";

            lblL6.Text = "Low Car - 115";
            lblL5.Text = "Low Car - 113";
            lblL4.Text = "Low Car -  29" + System.Environment.NewLine + "High Car - 80";

            lblL3.Text = "Low Car - 156";
            lblL2.Text = "Low Car - 152";
            lblL1.Text = "High Car - 148";
            lblTotalCapacity.Content = "1191";
           
        }
        
        void ProcSnapshotNotification()
        {
            string query = "select * from L2_PROC_SNAPSHOT"; //order by AISLE";

            DataTable dt = new DataTable();
            dt.TableName = "PathDetails";
            using (con = new OracleConnection( Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();

                using (OracleCommand command = new OracleCommand(query))
                {
                    command.CommandText = query;
                    dep = new OracleDependency(command);
                    command.Notification.IsNotifiedOnce = false;
                    dep.OnChange += new OnChangeEventHandler(OnSnapShotNotification);

                    command.Connection = con;

                    OracleDataAdapter dadapter = new OracleDataAdapter(command);
                    dep.QueryBasedNotification = false;

                    dadapter.Fill(dt);
                }
            }
        }

        void DisplayTodayCarParkRetrieval()
        {
            string query = "select * from VW_NOWTOTALCARPARKEDRETIREVED";
            try
            {
                //DataTable dt = new DataTable();
                //dt.TableName = "PathDetails";
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader())
                        {
                            if (!dreader.IsClosed)
                            {
                                while (dreader.Read())
                                {
                                    if (dreader["IsParkedFlag"].ToString() == "1")
                                        lblTodayTotalCarParked.Dispatcher.Invoke(new Action(() =>
                                            lblTodayTotalCarParked.Content = Convert.ToString(dreader["TotalCarParkedRetrieved"])));

                                    if (dreader["IsParkedFlag"].ToString() == "0")
                                        lblTodayTotalCarRetrieved.Dispatcher.Invoke(new Action(() =>
                                            {
                                                  lblTodayTotalCarRetrieved.Content = Convert.ToString(dreader["TotalCarParkedRetrieved"]);
                                                
                                            }));
                                }
                            }
                        }
                    }
                }
            }
            catch(Exception errMsg)
            {
            }
            finally
            { 
            
            }
        }

       public void Dispose()
        {
       //     this.timerUpdateTodaysCount.Elapsed -= new System.Timers.ElapsedEventHandler(timerUpdateTodaysCount_Elapsed);
       //     this.g_frmHome.OnTriggerSnapShotNotification -= new EventHandler(g_frmHome_OnTriggerSnapShotNotification);
       //     StopTimer(this.timerUpdateTodaysCount);
       //     this.timerUpdateTodaysCount.Dispose();
        }

       private void btnSynch_Click(object sender, RoutedEventArgs e)
       {
           if (MessageBox.Show("Do you want to continue with Synch operation?", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question)
               == MessageBoxResult.Yes)
           {
           
               UpdateMachineValuesToDB();
               MessageBox.Show("Synch has completed", "Information", MessageBoxButton.OK);
           }
       }

       private void UpdateMachineValuesToDB()
       {
           DataTable dtResult = new DataTable();
           Connection con = new Connection();
           OPCServerDirector opcd = new OPCServerDirector();

           dtResult = con.GetAllMachineTagsForSynch();
           int coutner = 1;
           try
           {               
                   foreach (DataRow dRow in dtResult.Rows)
                   {
                      
                       coutner += 1;

                       string channel = dRow["channel"].ToString();
                       string machine = dRow["machine"].ToString();

                       Console.WriteLine("machine ={0}", machine);

                       if (machine.Contains("LCM") || machine.Contains("UCM"))
                       {
                           if (opcd.IsMachineHealthy(channel + "." + machine + "." + "L2_Min_Window_Limit") == true)
                           {
                               //Read min window.
                               Int16 minWindow = opcd.ReadTag<Int16>(channel + "." + machine + "." + "L2_Min_Window_Limit");

                               //Read max window.
                               Int16 maxWindow = opcd.ReadTag<Int16>(channel + "." + machine + "." + "L2_Max_Window_Limit");

                               //read current position.
                               Int16 destinationAisle = opcd.ReadTag<Int16>(channel + "." + machine + "." + "Aisle_Data_From_Barcode_in_Dec");

                               //read current row.
                               Int16 currentRow = opcd.ReadTag<Int16>(channel + "." + machine + "." + "L2_Destination_Row");

                               //read auto mode & auto ready.
                               bool autoMode = false;

                               if (machine.Contains("LCM"))
                                   autoMode = opcd.IsCMMachineInAutoMode(channel, machine, "LCM");
                               else if (machine.Contains("UCM"))
                                   autoMode = opcd.IsCMMachineInAutoMode(channel, machine, "UCM");

                               con.UpdateMachineValues(machine, minWindow, maxWindow, destinationAisle, currentRow, autoMode);
                           }
                       }
                       else if (machine.Contains("VLC"))
                       {
                           if (opcd.IsMachineHealthy(channel + "." + machine + "." + "Auto_Mode") == true)
                           {
                               //read auto mode & auto ready.
                               bool autoMode = false;
                               autoMode = opcd.ReadTag<bool>(channel + "." + machine + "." + "Auto_Mode");
                               autoMode &= opcd.ReadTag<bool>(channel + "." + machine + "." + "Auto_Ready");
                               con.UpdateVLCAutoMode(autoMode, machine);
                           }
                       }
                       else if (machine.Contains("EES"))
                       {
                           if (opcd.IsMachineHealthy(channel + "." + machine + "." + "Auto_Mode") == true)
                           {
                               //read auto mode & auto ready.
                               bool autoMode = false;
                               autoMode = opcd.ReadTag<bool>(channel + "." + machine + "." + "Auto_Mode");
                               autoMode &= opcd.ReadTag<bool>(channel + "." + machine + "." + "Auto_Ready");
                               con.UpdateEESAutoMode(autoMode, machine);
                           }
                       }

                   }

               //car wash ready updated.
                bool isCarWashReady =  opcd.ReadTag<bool>("CH003.VLC_Drive_03.CarWash_Ready");
                if (isCarWashReady)
                    con.UpdateCarWashFinishTrigger(0);
              
           }
           catch (Exception errMsg)
           {
             //  Console.WriteLine(errMsg.Message);
           }
       }

       private void btnEES_Click(object sender, RoutedEventArgs e)
       {

       }

       void SetFromIcon()
       {
           try
           {
               Uri iconConfig = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\Config.ico", UriKind.RelativeOrAbsolute);
               imgConfig.Source = BitmapFrame.Create(iconConfig);

               Uri iconSetPoints = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\SetPoints.ico", UriKind.RelativeOrAbsolute);
              imgSetPoints.Source = BitmapFrame.Create(iconSetPoints);

              //Uri iconSynch = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\Synch.ico", UriKind.RelativeOrAbsolute);
              //imgSynch.Source = BitmapFrame.Create(iconSynch);

              Uri iconTask = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\Tasks.ico", UriKind.RelativeOrAbsolute);

             // Uri iconVLCTask = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\Tasks.ico", UriKind.RelativeOrAbsolute);
              imgVLCTask.Source = BitmapFrame.Create(iconTask);

             // Uri iconLCMTask = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\LCMTask.ico", UriKind.RelativeOrAbsolute);
              imgLCMTask.Source = BitmapFrame.Create(iconTask);

            //  Uri iconUCMTask = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\UCMTask.ico", UriKind.RelativeOrAbsolute);
              imgUCMTask.Source = BitmapFrame.Create(iconTask);
           

              Uri iconAlarm = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\Alarm.ico", UriKind.RelativeOrAbsolute);
              imgAlarm.Source = BitmapFrame.Create(iconAlarm);

               Uri iconErrorMaster = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\ErrorMaster.ico", UriKind.RelativeOrAbsolute);
               imgErrorMaster.Source = BitmapFrame.Create(iconErrorMaster);

               Uri iconERPDiag = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\ERPDiagnostic.ico", UriKind.RelativeOrAbsolute);
               imgERPDiagnostic.Source = BitmapFrame.Create(iconERPDiag);

               Uri iconPMSDiag = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\PMSDiagnostic.ico", UriKind.RelativeOrAbsolute);
               imgPMSDiag.Source = BitmapFrame.Create(iconPMSDiag);

               Uri iconHistory = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\History.ico", UriKind.RelativeOrAbsolute);
               imgHistory.Source = BitmapFrame.Create(iconHistory);

               Uri iconCarWashList = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\CarWashList.ico", UriKind.RelativeOrAbsolute);
               imgCarWashList.Source = BitmapFrame.Create(iconCarWashList);

              // Uri iconPMSTask = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\PMSTask.ico", UriKind.RelativeOrAbsolute);
               imgPMSTask.Source = BitmapFrame.Create(iconTask);

              // Uri iconERPTask = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\Tasks.ico", UriKind.RelativeOrAbsolute);
               imgErpTask.Source = BitmapFrame.Create(iconTask);

               Uri iconCurrentParks = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\CurrentParks.ico", UriKind.RelativeOrAbsolute);
               imgCurrentParks.Source = BitmapFrame.Create(iconCurrentParks);

               Uri iconDemoMode = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\DemoMode.ico", UriKind.RelativeOrAbsolute);
               imgDemoMode.Source = BitmapFrame.Create(iconDemoMode);

               Uri iconSecurity = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\Security.ico", UriKind.RelativeOrAbsolute);
               imgSecurity.Source = BitmapFrame.Create(iconSecurity);

               Uri iconMemberDetails = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\MemeberInfo.ico", UriKind.RelativeOrAbsolute);
               imgMemberDetails.Source = BitmapFrame.Create(iconMemberDetails);

               Uri iconKioskSimulation = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\Simulation.ico", UriKind.RelativeOrAbsolute);
               imgSimulation.Source = BitmapFrame.Create(iconKioskSimulation);

               Uri iconRunTimeTable = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\RunTimeTable.ico", UriKind.RelativeOrAbsolute);
               imgRunTimeTable.Source = BitmapFrame.Create(iconRunTimeTable);
           }
           catch (Exception errMsg)
           {

           }
           finally { }
       }

        private void btnDemoEntry_Click(object sender, RoutedEventArgs e)
       {
          
           triggerSimulation(sender, e);
       }

        private void CMConfig_Click(object sender, RoutedEventArgs e)
        {
           

            triggerSetCMWindow(sender, e);
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

        private void referesh_l2_but_Click(object sender, RoutedEventArgs e)
        {
            RefreshProcedureCall();
        }
        public void RefreshProcedureCall()
        {
            using (OracleConnection con = new OracleConnection(Connection.connectionString))
            {
                if (con.State == ConnectionState.Closed) con.Open();
                OracleCommand command = con.CreateCommand();
                string sql = "REFRESH_PROCEDURE";
                command.CommandText = sql;
                command.CommandType = CommandType.StoredProcedure;
                command.ExecuteNonQuery();
            }
        }

    }
}
