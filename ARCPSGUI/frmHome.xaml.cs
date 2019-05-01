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
using System.Windows.Shapes;

using ARCPSGUI.OPC;


using ARCPSGUI.StaticGlobalClass;
using ARCPSGUI.ConfigurationUI;
using ARCPSGUI.DiagnosticScreens;
using ARCPSGUI.TransactionUI;
using System.Threading.Tasks;
using System.Data;
using ARCPSGUI.DB;
using OPC;

using OPCDA;
using ARCPSGUI.Controls;
using Oracle.DataAccess.Client;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using ARCPSGUIStyle;
using System.Diagnostics;
using ARCPSGUI.Security;
using ARCPSGUI.MachineRuntimeTable;
using ARCPSGUI.utility;
using ARCPSGUI.chart;
using ARCPSGUI.FloorUI;
using ARCPSGUI.DB;

using ARCPSGUI.UserControls;
using ARCPSGUI.Popup;
using System.Threading;
using ARCPSGUI.Model;
using System.Configuration;
using System.Windows.Threading;
//using vlofcwl;
namespace ARCPSGUI
{
    /// <summary>
    ///  Home screen of gui.
    /// </summary>
    public partial class frmHome : Window
    {
        #region Member Variables
     
        //gui progress exe.
        System.Diagnostics.Process guiExe = null;
        
        //home ui.
       // ucParkingDiagnostic_new homeui = null;
        
        //This variables hold current loaded ui.
        UIElement currentLoadedUI = null;

        //Advosol groud declaration.
        //int DAUpdateRate = 1;
        //OPCDA.NET.RefreshGroup uGrp;

        //oracle dependency creation.
       // OracleDependency dep = null;
        OracleConnection con = null;
        Connection dbcon = new Connection();

        //EES Queue table notification creation.
        //public event EventHandler OnTriggerEESQueueNotificaiton;

        //Slot path table notification creation.
       // public event EventHandler OnTriggerSlotPathNotificaiton;

        //Path details table notification creation.
        public event EventHandler OnTriggerPathDetailsNotification;

        //Snap shot table notificaiton creation.
        public event EventHandler OnTriggerSnapShotNotification;

        //Customer table notification.
       //public event EventHandler OnTriggerCustomerNotification;
        
        //exit ees estimated time for normal retrieval.
        int exitEesExitmateAddTime = 0;

        //exit ees estimated time for mobile retrieval.
        int exitEesEstimateAddTimeForMob = 0;

        //To show current time.
        System.Timers.Timer currentTime = null;

        //xml for showing retrieval information on display.
       // string displayxml = "";

        //xml for showing car wash estimation information on entry kiosk.
        string carWashEstxml = "";

        //To update display screen.
      // System.Timers.Timer timerForDisplayScreenUpdate = null;

        public event EventHandler OnErpRefresh;
        int countErpRefresh = 0;
       
        GeneralDba objGeneralDba = null;
        CMDba objCMDba = null;
        VLCDba objVLCDba = null;
        EESDba objEESDba = null;

        PSDba objPSDba = null;
        PSTDba objPSTDba = null;
        PVLDba objPVLDba = null;

        ErrorDba objErrorDba = null;
        Thread updateDataFromOpcListener = null;
        DispatcherTimer timerToClose = null;
        public delegate void InvokeDelegate();
        #endregion
      
        public frmHome()
        {
            InitializeComponent();
            string globalErrorMessage = "";
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            try
            {
                StartGui();

                //Connect opc server.
                DoRefXMLCreation("Connection opc server...");
                OPCServerManagement.StartOPCServer(out globalErrorMessage);
                //BGOPCServerManagement.StartBGOPCServer(this);


                DoRefXMLCreation("Register event in oracle db...");
                new Connection().RegisterEventInOracleDB();
                Connection con = new Connection();

                //Load image and icon.
                DoRefXMLCreation("Set Icon...");
                //LoadImagesAndIcons();
                SetIcon();


            

                AddHomeUI();





                //Get retrieval display xml path.
                // displayxml = con.GetDisplayXMLPath();

                //Get car wash estimation xml path.
                carWashEstxml = con.GetEstCarWashTimeXMLPath();

                //get exit ees estimated time for normal retrieval.
                this.exitEesExitmateAddTime = con.GetExitEESEstimateTime();

                //get exit ees estimated time for mobile retrieval.
                this.exitEesEstimateAddTimeForMob = con.GetMobExitEstimateTime();

                //assign ees image path into global variable.
                GlobalData.GetEESImagePath();


                //DoRefXMLCreation("Creating notificaitons...");
                //InitializeMachinesNotification();

                //DoRefXMLCreation("Register notificaitons from opc server...");
                //RegisterNotificationFromOPCServer();

                DoRefXMLCreation("Register notificaitons from oracle database...");
                CreateOracleNotification();

                DoRefXMLCreation("Initialize Display XML...");
                CreateCarWashEstXML();

                DoRefXMLCreation("Loading to cache...");
                //DoOnLoad();

                lblGeneratorMode.Visibility = System.Windows.Visibility.Hidden;

                currentTime = new System.Timers.Timer();
                currentTime.Interval = 1000;
                currentTime.Start();
                currentTime.Elapsed += new System.Timers.ElapsedEventHandler(currentTime_Elapsed);


              
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
            finally { }
        }

       

        /// <summary>
        /// start progress bar.
        /// </summary>
        void StartGui()
        {
            guiExe = new Process();
            string filePath = System.Windows.Forms.Application.StartupPath;
            guiExe.StartInfo.FileName = filePath + @"\" + "ARCPSPRGBAR.exe";
            guiExe.Start();
        }

        void RegisterMenuButtonClick(uiHome menuUi)
        {
            try
            {
                menuUi.triggerSetPoints += new EventHandler(homeui_triggerSetPoints);
              
                menuUi.triggerErpTask += new EventHandler(homeui_triggerErpTask);
                menuUi.triggerCurrentParks += new EventHandler(homeui_triggerCurrentParks);
                menuUi.triggerParkingDiagnostic += new EventHandler(homeui_triggerParkingDiagnostic);
                //homeui.triggerParkingDiagnostic += new EventHandler(homeui_triggerParkingDiagnostic_new);


                menuUi.triggerLockStatus += new EventHandler(homeui_triggerLockStatus);

                menuUi.triggerFloorL9 += new EventHandler(homeui_triggerFloorL9);
                menuUi.triggerFloorL8 += new EventHandler(homeui_triggerFloorL8);
                menuUi.triggerFloorL7 += new EventHandler(homeui_triggerFloorL7);
                menuUi.triggerFloorL6 += new EventHandler(homeui_triggerFloorL6);
                menuUi.triggerFloorL5 += new EventHandler(homeui_triggerFloorL5);
                menuUi.triggerFloorL4 += new EventHandler(homeui_triggerFloorL4);
                menuUi.triggerFloorL3 += new EventHandler(homeui_triggerFloorL3);
                menuUi.triggerFloorL2 += new EventHandler(homeui_triggerFloorL2);
                menuUi.triggerFloorL1 += new EventHandler(homeui_triggerFloorL1);

                menuUi.triggerHistory += new EventHandler(homeui_triggerHistory);
               
                menuUi.triggerPMSTransTask += new EventHandler(homeui_triggerPMSTransTask);
                menuUi.triggerUCMTransTask += new EventHandler(homeui_triggerUCMTransTask);

                menuUi.triggerAlarmView += new EventHandler(homeui_triggerAlarmView);
                menuUi.triggerErrorMasterView += new EventHandler(homeui_triggerErrorMasterView);
                menuUi.triggerConfigurationView += new EventHandler(homeui_triggerConfigurationView);
                menuUi.triggerCarWashView += new EventHandler(homeui_triggerCarWashView);
                menuUi.triggerDemoMode += new EventHandler(homeui_triggerDemoMode);

                menuUi.triggerAuthentication += new EventHandler(homeui_triggerAuthentication);
                menuUi.triggerMemberDetails += new EventHandler(homeui_triggerMemberDetails);
                menuUi.triggerRunTimeTable += new EventHandler(homeui_triggerRunTimeTable);

                menuUi.triggerAbortView += new EventHandler(homeui_triggerAbortView);
                menuUi.triggerPMSUnlock += new EventHandler(homeui_triggerPMSUnlock);
                menuUi.triggerSetCMWindow += new EventHandler(homeui_triggerSetCMWindow);
                menuUi.triggerSimulation += new EventHandler(homeui_triggerSimulation);
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }

        }

        //void LoadImagesAndIcons()
        //{
        //    string errMessage = "";
        //    try
        //    {
        //        errMessage = MachineImages.LoadMachineImages();
        //        errMessage += SlotImages.LoadSlotImages();
        //        SetIcon();
        //    }
        //    catch (Exception errMsg)
        //    {
        //        MessageBox.Show(errMsg.Message, "LoadImagesAndIcons");
        //        throw errMsg;
        //    }
        //}

        void InitializeMachinesNotification()
        {
            try
            {
                InitialAssignOfCMStatus();
                InitialAssignOfVLCStatus();

                InitialAssignOfPSStatus();
                InitialAssignOfPSTStatus();
                InitialAssignOfPVLStatus();
                InitialAssignOfEESStatus();
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
        }

        //void CreateOracleNotification()
        //{
        //    try
        //    {
        //        //EesQueueNotification();
        //        //SlotPathNotification();
        //        //PathDetailsNotification();
        //        // CustomerNotification();
        //        CarWashQueueNotification();
        //        ConfigNotification();
        //    }
        //    catch (Exception errMsg)
        //    {
        //        throw errMsg;
        //    }
        //}
      

        void homeui_triggerAbortView(object sender, EventArgs e)
        {
            try
            {
              

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                ucAbortedTransactionView abortedView = new ucAbortedTransactionView();
                abortedView.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                abortedView.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(abortedView);
                Grid.SetColumn(abortedView, 0);
                Grid.SetRow(abortedView, 0);
                currentLoadedUI = abortedView;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerDemoMode(object sender, EventArgs e)
        {
            try
            {
                if (Security.Security.HasReadAccess("guiDemoMode") == StaticGlobalClass.GlobalData.SecurityType.Read)
                {

                }
                else
                {
                    MessageBox.Show("Access Denied", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                //grdContainer.Children.Remove(homeui);
                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                ucDemoMode demoMode = null;
                if (CacheUI.IsValueCached("DemoMode") == false)
                {
                    demoMode = new ucDemoMode();
                    CacheUI.Add("DemoMode", demoMode);
                }
                demoMode = CacheUI.Get("DemoMode") as ucDemoMode;
                demoMode.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                demoMode.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(demoMode);
                Grid.SetColumn(demoMode, 0);
                Grid.SetRow(demoMode, 0);
                currentLoadedUI = demoMode;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerCarWashView(object sender, EventArgs e)
        {
            try
            {
               

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
             
                ucWashingTrans washingTrans = new ucWashingTrans();
                washingTrans.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                washingTrans.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(washingTrans);
                Grid.SetColumn(washingTrans, 0);
                Grid.SetRow(washingTrans, 0);
                currentLoadedUI = washingTrans;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerErrorMasterView(object sender, EventArgs e)
        {
            try
            {
              
                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                ucErrorMaster errorMaster = new ucErrorMaster();
                errorMaster.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                errorMaster.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(errorMaster);
                Grid.SetColumn(errorMaster, 0);
                Grid.SetRow(errorMaster, 0);
                currentLoadedUI = errorMaster;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerAlarmView(object sender, EventArgs e)
        {
            try
            {
            

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
               // ucAlarmView alarmView = new ucAlarmView();
                ucNotifications notificationView = new ucNotifications();
                notificationView.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                notificationView.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(notificationView);
                Grid.SetColumn(notificationView, 0);
                Grid.SetRow(notificationView, 0);
                currentLoadedUI = notificationView;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerPMSUnlock(object sender, EventArgs e)
        {
            try
            {
              
                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }

                ucPMSUnBlock pmsUnblock = new ucPMSUnBlock();
                pmsUnblock.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                pmsUnblock.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(pmsUnblock);
                Grid.SetColumn(pmsUnblock, 0);
                Grid.SetRow(pmsUnblock, 0);
                currentLoadedUI = pmsUnblock;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerSetCMWindow(object sender, EventArgs e)
        {
            try
            {
              
                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                CMWindowLimitConfig windowConfig = new CMWindowLimitConfig();
                windowConfig.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                windowConfig.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(windowConfig);
                Grid.SetColumn(windowConfig, 0);
                Grid.SetRow(windowConfig, 0);
                currentLoadedUI = windowConfig;
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);
            }
        }
        void homeui_triggerSimulation(object sender, EventArgs e)
        {
            
            System.Diagnostics.Process kioskSimulation = null;
            kioskSimulation = new Process();
            kioskSimulation.StartInfo.FileName = System.Windows.Forms.Application.StartupPath + @"\SIMULATION\KioskSimulation.exe";
            kioskSimulation.Start();
        }
        
        void currentTime_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                lblToday.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        lblToday.Content = System.DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");
                    }));
            }
            catch (Exception errMsg)
            {

            }
        }
        
      
       
       
        void homeui_triggerHistory(object sender, EventArgs e)
        {
            try
            {
               

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                //ucHistory history = new ucHistory();
                ucParkHistory history = new ucParkHistory();
                history.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                history.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(history);
                Grid.SetColumn(history, 0);
                Grid.SetRow(history, 0);
                currentLoadedUI = history;
            }
            catch (Exception errMsg)
            {

            }
        }

        #region Definition of floor menu click event.
        void homeui_triggerFloorL9(object sender, EventArgs e)
        {
            try
            {
                

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }



                ucFloorParent diplayFloor = null; // new uiPMSDiagnostic();
                if (CacheUI.IsValueCached("FloorL9") == false)
                {
                    CacheUI.Add("FloorL9", new ucFloorParent(9));
                }

                diplayFloor = CacheUI.Get("FloorL9") as ucFloorParent;
                diplayFloor.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                diplayFloor.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(diplayFloor);
                Grid.SetColumn(diplayFloor, 0);
                Grid.SetRow(diplayFloor, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = diplayFloor;




            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerFloorL8(object sender, EventArgs e)
        {
            try
            {
                

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }



                ucFloorParent diplayFloor = null; // new uiPMSDiagnostic();
                if (CacheUI.IsValueCached("FloorL8") == false)
                {
                    CacheUI.Add("FloorL8", new ucFloorParent(8));
                }

                diplayFloor = CacheUI.Get("FloorL8") as ucFloorParent;
                diplayFloor.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                diplayFloor.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(diplayFloor);
                Grid.SetColumn(diplayFloor, 0);
                Grid.SetRow(diplayFloor, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = diplayFloor;




            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerFloorL7(object sender, EventArgs e)
        {
            try
            {
                

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }



                ucFloorParent diplayFloor = null; // new uiPMSDiagnostic();
                if (CacheUI.IsValueCached("FloorL7") == false)
                {
                    CacheUI.Add("FloorL7", new ucFloorParent(7));
                }

                diplayFloor = CacheUI.Get("FloorL7") as ucFloorParent;
                diplayFloor.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                diplayFloor.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(diplayFloor);
                Grid.SetColumn(diplayFloor, 0);
                Grid.SetRow(diplayFloor, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = diplayFloor;




            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerFloorL6(object sender, EventArgs e)
        {
            try
            {
               

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }



                ucFloorParent diplayFloor = null; // new uiPMSDiagnostic();
                if (CacheUI.IsValueCached("FloorL6") == false)
                {
                    CacheUI.Add("FloorL6", new ucFloorParent(6));
                }

                diplayFloor = CacheUI.Get("FloorL6") as ucFloorParent;
                diplayFloor.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                diplayFloor.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(diplayFloor);
                Grid.SetColumn(diplayFloor, 0);
                Grid.SetRow(diplayFloor, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = diplayFloor;




            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerFloorL5(object sender, EventArgs e)
        {
            try
            {
               

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }



                ucFloorParent diplayFloor = null; // new uiPMSDiagnostic();
                if (CacheUI.IsValueCached("FloorL5") == false)
                {
                    CacheUI.Add("FloorL5", new ucFloorParent(5));
                }

                diplayFloor = CacheUI.Get("FloorL5") as ucFloorParent;
                diplayFloor.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                diplayFloor.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(diplayFloor);
                Grid.SetColumn(diplayFloor, 0);
                Grid.SetRow(diplayFloor, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = diplayFloor;




            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerFloorL4(object sender, EventArgs e)
        {
            try
            {
                

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                


                ucFloorParent diplayFloor = null; // new uiPMSDiagnostic();
                if (CacheUI.IsValueCached("FloorL4") == false)
                {
                    CacheUI.Add("FloorL4", new ucFloorParent(4));
                }

                diplayFloor = CacheUI.Get("FloorL4") as ucFloorParent;
                //diplayFloor = new ucFloorParent(4);
                diplayFloor.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                diplayFloor.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(diplayFloor);
                Grid.SetColumn(diplayFloor, 0);
                Grid.SetRow(diplayFloor, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = diplayFloor;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerFloorL3(object sender, EventArgs e)
        {
            try
            {
               

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                


                ucFloorParent diplayFloor = null; // new uiPMSDiagnostic();
                if (CacheUI.IsValueCached("FloorL3") == false)
                {
                    CacheUI.Add("FloorL3", new ucFloorParent(3));
                }

                diplayFloor = CacheUI.Get("FloorL3") as ucFloorParent;
                diplayFloor.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                diplayFloor.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(diplayFloor);
                Grid.SetColumn(diplayFloor, 0);
                Grid.SetRow(diplayFloor, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = diplayFloor;




            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerFloorL2(object sender, EventArgs e)
        {
            try
            {
                

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }



                ucFloorParent diplayFloor = null; // new uiPMSDiagnostic();
                if (CacheUI.IsValueCached("FloorL2") == false)
                {
                    CacheUI.Add("FloorL2", new ucFloorParent(2));
                }

                diplayFloor = CacheUI.Get("FloorL2") as ucFloorParent;
                diplayFloor.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                diplayFloor.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(diplayFloor);
                Grid.SetColumn(diplayFloor, 0);
                Grid.SetRow(diplayFloor, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = diplayFloor;




            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerFloorL1(object sender, EventArgs e)
        {
            try
            {
               

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                


                ucFloorParent diplayFloor = null; // new uiPMSDiagnostic();
                if (CacheUI.IsValueCached("FloorL1") == false)
                {
                    CacheUI.Add("FloorL1", new ucFloorParent(1));
                }

                diplayFloor = CacheUI.Get("FloorL1") as ucFloorParent;
                diplayFloor.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                diplayFloor.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(diplayFloor);
                Grid.SetColumn(diplayFloor, 0);
                Grid.SetRow(diplayFloor, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = diplayFloor;
               

              

            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerPeakHourChart(object sender, EventArgs e)
        {
            try
            {
               

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                ucPeakHourChart peakHourChart = new ucPeakHourChart();

                peakHourChart.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                peakHourChart.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(peakHourChart);
                Grid.SetColumn(peakHourChart, 0);
                Grid.SetRow(peakHourChart, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = peakHourChart;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerDurationChart(object sender, EventArgs e)
        {
            try
            {
               

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                ucTransactionDurationChart durationChart = new ucTransactionDurationChart();

                durationChart.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                durationChart.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(durationChart);
                Grid.SetColumn(durationChart, 0);
                Grid.SetRow(durationChart, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = durationChart;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerWaitHistory(object sender, EventArgs e)
        {
            try
            {
               

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                ucWaitHistView waitHistory = new ucWaitHistView();

                waitHistory.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                waitHistory.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(waitHistory);
                Grid.SetColumn(waitHistory, 0);
                Grid.SetRow(waitHistory, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = waitHistory;

               

            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerDBLog(object sender, EventArgs e)
        {
            try
            {
               

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                ucDBLogView logView = new ucDBLogView();

                logView.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                logView.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(logView);
                Grid.SetColumn(logView, 0);
                Grid.SetRow(logView, 0);
                //diplayFloor.DoOnLoad();
                currentLoadedUI = logView;



            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerSetVLCConfigWindow(object sender, EventArgs e)
        {
            try
            {

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                // CMWindowLimitConfig windowConfig = new CMWindowLimitConfig();
                ucFloorVLCConfigDiagnostic windowConfig = new ucFloorVLCConfigDiagnostic();
                windowConfig.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                windowConfig.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(windowConfig);
                Grid.SetColumn(windowConfig, 0);
                Grid.SetRow(windowConfig, 0);
                currentLoadedUI = windowConfig;
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);
            }
        }

        #endregion

        #region Lock and Diagnostic menu click event.
        void homeui_triggerLockStatus(object sender, EventArgs e)
        {
            try
            {
               
                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                ucPMSUnBlock lockStatus = new ucPMSUnBlock();
                lockStatus.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                lockStatus.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(lockStatus);
                Grid.SetColumn(lockStatus, 0);
                Grid.SetRow(lockStatus, 0);
                currentLoadedUI = lockStatus;
            }
            catch (Exception errMsg)
            {

            }
        }
     
        void homeui_triggerParkingDiagnostic(object sender, EventArgs e)
        {
            try
            {
                //bool isCached = true;
                //UnRegisterNotificationForChildControl(currentLoadedUI);
              

                //if (grdContainer.Children.Contains(currentLoadedUI) == true)
                //{
                //    grdContainer.Children.Remove(currentLoadedUI);
                //}

                //ucParkingDiagnostic parkingDiagnostic = null; // new uiPMSDiagnostic();
                //if (CacheUI.IsValueCached("PARD") == false)
                //{
                //    parkingDiagnostic = new ucParkingDiagnostic(this);
                //    CacheUI.Add("PARD", parkingDiagnostic);
                //    parkingDiagnostic.DoOnLoad();
                //    isCached = false;
                //}

                //parkingDiagnostic = CacheUI.Get("PARD") as ucParkingDiagnostic;
                //parkingDiagnostic.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                //parkingDiagnostic.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                ////pmsDiagnostic.Width = grdContainer.Width - 80;
                ////pmsDiagnostic.Height = 700;
                //grdContainer.Children.Add(parkingDiagnostic);
                //Grid.SetColumn(parkingDiagnostic, 0);
                //Grid.SetRow(parkingDiagnostic, 0);
                //parkingDiagnostic.RegisterNotification();
                //if (isCached)
                //{
                //    parkingDiagnostic.InitialReRead();
                //    parkingDiagnostic.AddHandler();
                //}
                //currentLoadedUI = parkingDiagnostic;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerParkingDiagnostic_new(object sender, EventArgs e)
        {
            try
            {
                bool isCached = true;
                UnRegisterNotificationForChildControl(currentLoadedUI);
                

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }

                ucParkingDiagnostic_new parkingDiagnostic = null; // new uiPMSDiagnostic();
                if (CacheUI.IsValueCached("PARD_NEW") == false)
                {
                    parkingDiagnostic = new ucParkingDiagnostic_new(this);
                    CacheUI.Add("PARD_NEW", parkingDiagnostic);
                    //parkingDiagnostic.DoOnLoad();
                    isCached = false;
                }

                parkingDiagnostic = CacheUI.Get("PARD_NEW") as ucParkingDiagnostic_new;
                //parkingDiagnostic = new ucParkingDiagnostic_new(this);
                parkingDiagnostic.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                parkingDiagnostic.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(parkingDiagnostic);

                currentLoadedUI = parkingDiagnostic;
            }
            catch (Exception errMsg)
            {

            }
        }
       


      
     
        #endregion

        #region Transaction menu click event

        //ERP Task menu trigger.
        public void homeui_triggerErpTask(object sender, EventArgs e)
        {
            try
            {
              

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
              

                uiERPTasks erpTask = null;
                if (CacheUI.IsValueCached("ERPTask") == false)
                {
                    erpTask = new uiERPTasks(this);
                    CacheUI.Add("ERPTask", erpTask);
                }
                

                //comment : 20-May-2013
                //else if (currentLoadedUI is ucParkingDiagnostic)
                  //  UnRegisterNotificationForChildControl(currentLoadedUI);

                erpTask = CacheUI.Get("ERPTask") as uiERPTasks;

                //Add : 20-May-2013
                UnRegisterNotificationForChildControl(currentLoadedUI);

                //uiERPTasks erpTask = new uiERPTasks(this);
                erpTask.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                erpTask.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;

                grdContainer.Children.Add(erpTask);
                Grid.SetColumn(erpTask, 0);
                Grid.SetRow(erpTask, 0);
                //erpTask.InitializeTimer();
                currentLoadedUI = erpTask;
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);

            }
        }

        //PMS Task menu trigger.
        public void homeui_triggerPMSTransTask(object sender, EventArgs e)
        {
            try
            {
              
                frmPMSTask.ShowPMSTask(this);
             
            }
            catch (Exception errMsg)
            {

            }
        }

        //PMS Task menu on home ui.
        private void btnPMSTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                    homeui_triggerPMSTransTask(sender, e);
            }
            catch (Exception errMsg)
            {

            }
        }
      
        //ERP Task menu on home ui.
        private void btnErpTask_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                  //UnRegisterNotificationForChildControl(currentLoadedUI);

                if (grdContainer.Children.Contains(currentLoadedUI) == true && this.currentLoadedUI is uiERPTasks == false)
                {
                    UnRegisterNotificationForChildControl(currentLoadedUI);
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                if (this.currentLoadedUI is uiERPTasks == false)
                    homeui_triggerErpTask(sender, e);
            }
            catch (Exception errMsg)
            {

            }
        }

        //Current park menu.
        void homeui_triggerCurrentParks(object sender, EventArgs e)
        {
            try
            {
             

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                uiCurrentTransaction currentParks = new uiCurrentTransaction();
                currentParks.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                currentParks.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                //setPoints.Width = 900;
                //setPoints.Height = 500;
                grdContainer.Children.Add(currentParks);
                Grid.SetColumn(currentParks, 0);
                Grid.SetRow(currentParks, 0);
                currentParks.DoOnLoad();
                currentLoadedUI = currentParks;
            }
            catch (Exception errMsg)
            {

            }
        }

        //UCM Transaction Task menu.
        void homeui_triggerUCMTransTask(object sender, EventArgs e)
        {
            try
            {
               
                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                ucUCMTask ucmTrans = new ucUCMTask(this);
                ucmTrans.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                ucmTrans.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(ucmTrans);
                Grid.SetColumn(ucmTrans, 0);
                Grid.SetRow(ucmTrans, 0);
                //history.DoOnLoad();
                currentLoadedUI = ucmTrans;
            }
            catch (Exception errMsg)
            {

            }
        }
     
        //LCM Transaction Task menu.
      
     
       
        #endregion

        #region Other menu click event

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            AddHomeUI();
        }

        void homeui_triggerSetPoints(object sender, EventArgs e)
        {
            try
            {
              

                winSetPoints setPoints = new winSetPoints();
                setPoints.Show();
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);
            }
        }
        void homeui_triggerAuthentication(object sender, EventArgs e)
        {
            try
            {
                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                frmAuthenticationWindow frmAuthentication = new frmAuthenticationWindow(true);

                if (frmAuthentication.ShowDialog() == false)
                {
                    //grdContainer.Children.Remove(homeui);

                    ucUser user = new ucUser();
                    user.Height = 300;
                    user.Width = 800;
                    grdContainer.Children.Add(user);
                    Grid.SetColumn(user, 0);
                    Grid.SetRow(user, 0);
                    currentLoadedUI = user;
                }
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerMemberDetails(object sender, EventArgs e)
        {
            try
            {
              

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
              
                ucMemberData memberDetails = new ucMemberData();
                grdContainer.Children.Add(memberDetails);
                Grid.SetColumn(memberDetails, 0);
                Grid.SetRow(memberDetails, 0);
                currentLoadedUI = memberDetails;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerRunTimeTable(object sender, EventArgs e)
        {
            try
            {
                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                
                ucMachineRunTimeTableView guiRunTimeTable = new ucMachineRunTimeTableView();
                grdContainer.Children.Add(guiRunTimeTable);
                Grid.SetColumn(guiRunTimeTable, 0);
                Grid.SetRow(guiRunTimeTable, 0);
                currentLoadedUI = guiRunTimeTable;
            }
            catch (Exception errMsg)
            {

            }
        }
        void homeui_triggerConfigurationView(object sender, EventArgs e)
        {
            try
            {
                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }

                ucMachineMaintenanceExcelMaping configui = new ucMachineMaintenanceExcelMaping();
                configui.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                configui.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(configui);
                Grid.SetColumn(configui, 0);
                Grid.SetRow(configui, 0);
                currentLoadedUI = configui;
            }
            catch (Exception errMsg)
            {

            }
        }
        private void btnLockStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }
                homeui_triggerLockStatus(sender, e);
            }
            catch (Exception errMsg)
            {

            }
        }
        #endregion
           
        #region Memeber Methods

        public void DoOnLoad()
        {
            try
            {
                for (int i = 1; i <= 9; i++)
                {
                    DoRefXMLCreation("Loading floor " + i + " into cache");
                 
                }

                DoRefXMLCreation("Loading demo mode into cache.");
                if (CacheUI.IsValueCached("DemoMode") == false)
                {
                    ucDemoMode demoMode = new ucDemoMode();
                    demoMode.OnDemoMode += (s, e) =>
                    {
                        string[] value = Convert.ToString(s).Split(',');
                        AddNotifiedForDemoMachine(value[0], value[1] == "T");
                    };
                    CacheUI.Add("DemoMode", demoMode);
                }

               
                btnCarWashFinish.IsEnabled = false;
                DoRefXMLCreation("Finalizing...");
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
        }

        void AddHomeUI()
        {
            try
            {


                homeui_triggerParkingDiagnostic_new(new Object(), new EventArgs());

              

            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
        }

        void AddMenuUI()
        {
            try
            {




                bool isCached = true;
                UnRegisterNotificationForChildControl(currentLoadedUI);
             

                if (grdContainer.Children.Contains(currentLoadedUI) == true)
                {
                    grdContainer.Children.Remove(currentLoadedUI);
                }




                uiHome menuUi = null;
                if (CacheUI.IsValueCached("menu") == false)
                {
                    menuUi = new uiHome(this);
                    CacheUI.Add("menu", menuUi);
                    RegisterMenuButtonClick(menuUi);
                    
                }

                menuUi = CacheUI.Get("menu") as uiHome;
               
                menuUi.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                menuUi.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                grdContainer.Children.Add(menuUi);

                currentLoadedUI = menuUi;
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
        }

        /// <summary>
        /// Un-register notificaiton, of the ui which is removed from current loaded ui.
        /// </summary>
        /// <param name="uiElement"></param>
        void UnRegisterNotificationForChildControl(UIElement uiElement)
        {
            try
            {
              

                if (uiElement is uiERPTasks)
                {
                    //this.Dispatcher.Invoke((Action)(() =>
                    //{
                        ((uiERPTasks)uiElement).Dispose();
                    //}));
                    
                }
              
                if (uiElement is uiVLCTask)
                {
                    ((uiVLCTask)uiElement).Dispose();
                }
                if (uiElement is ucLCMTask)
                {
                    ((ucLCMTask)uiElement).Dispose();
                }
                if (uiElement is ucUCMTask)
                {
                    ((ucUCMTask)uiElement).Dispose();
                }
               
               
            }
            catch (Exception errMsg)
            {

            }

        }

        public void InsertIntoAlarmTable(string machine, Int16 errorCode, string message, string floor, string machineModel)
        {
            int level = 0;
            try
            {
                if (string.IsNullOrEmpty(message) && errorCode == 0) return;

                int.TryParse(floor, out level);

                if (level == 0) level = 4;

                using (OracleConnection con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();
                    using (OracleCommand command = con.CreateCommand())
                    {
                        command.Connection = con;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "INSERT_ALARAM";
                        command.Parameters.Add("P_INITDATETIME", OracleDbType.TimeStamp, System.DateTime.Now, ParameterDirection.Input);
                        command.Parameters.Add("P_MACHINENAME", OracleDbType.Varchar2, 20, machine, ParameterDirection.Input);

                        command.Parameters.Add("P_ERRORCODE", OracleDbType.Int32, 20, errorCode, ParameterDirection.Input);

                        if (string.IsNullOrEmpty(message) == false)
                            command.Parameters.Add("p_MESSAGE", OracleDbType.Varchar2, 200, message, ParameterDirection.Input);
                        else
                            command.Parameters.Add("p_MESSAGE", OracleDbType.Varchar2, 200, DBNull.Value, ParameterDirection.Input);

                        command.Parameters.Add("p_FLOOR", OracleDbType.Int32, 20, level, ParameterDirection.Input);
                        command.Parameters.Add("p_MACHINE_MODEL", OracleDbType.Varchar2, 50, machineModel, ParameterDirection.Input);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception errMsg)
            {
            }
            finally
            {

            }

        }

        /// <summary>
        /// Create xml for retrieval information on display.
        /// </summary>
        /// <returns></returns>
        //public int CreateDispalyXML()
        //{
        //    int RetVal = 0;

        //    try
        //    {
        //        string pXMLDataFileName = displayxml;// "C:\\Inetpub\\wwwroot\\Data\\Data.xml";

        //        DataTable dtSource = GetCurrentParks();
        //        if(dtSource !=null && dtSource.Rows.Count > 0)
        //        {
        //            using (FileStream fs = new FileStream(pXMLDataFileName, FileMode.Create))
        //            {

        //                using (XmlTextWriter w = new XmlTextWriter(fs, null))
        //                {

        //                    w.Formatting = Formatting.Indented;
        //                    w.Indentation = 5;

        //                    w.WriteStartDocument();
        //                    w.WriteStartElement("ODBCXMLData");


        //                    //   DataTable dtSource = GetCurrentParks();

        //                    string eesName = "";
        //                    string eesNumber = "";
        //                    DateTime dtstartTime = System.DateTime.Now;
        //                    DateTime dtestTime = System.DateTime.Now;
        //                    string stTime = "";
        //                    string estTime = "";
        //                    string washingStatus = "";
                            
        //                    foreach (DataRow drow in dtSource.Rows)
        //                    {
        //                        eesName = "";
        //                        eesNumber = "";
        //                        dtstartTime = System.DateTime.Now;
        //                        dtestTime = System.DateTime.Now;
        //                        stTime = "";
        //                        estTime = "";

        //                        w.WriteStartElement("ODBCTagRow");
        //                        if (Convert.ToString(drow["CMD"]).Contains("Payment") == true)
        //                            eesName = Convert.ToString(drow["REES_Num"]).Trim();


        //                        if (string.IsNullOrEmpty(eesName) == false && eesName.Length == 4)
        //                            eesNumber = eesName.Substring(3, 1);

        //                        DateTime.TryParse(Convert.ToString(drow["ENTRYTIME"]), out dtstartTime);
        //                        DateTime.TryParse(Convert.ToString(drow["EXITTIME"]), out dtestTime);

        //                        stTime = dtstartTime.ToString("dd/MMM/yy hh:mm");

        //                        if (Convert.ToString(drow["RETRIEVAL_TYPE"]) == "SMS")
        //                            estTime = dtestTime.AddMinutes(this.exitEesEstimateAddTimeForMob).ToString("dd/MM/yy HH:mm");
        //                        else
        //                            estTime = dtestTime.AddMinutes(this.exitEesExitmateAddTime).ToString("dd/MM/yy HH:mm");

        //                        washingStatus = Convert.ToString(drow["WASHSTATUS"]);

        //                        w.WriteAttributeString("Car_ID", Convert.ToString(drow["Car_ID"]));
        //                        w.WriteAttributeString("Patron_Name", Convert.ToString(drow["PATRON_NAME"]));
        //                        w.WriteAttributeString("REES_Num", eesNumber);
        //                        w.WriteAttributeString("EntryTime", dtstartTime.ToString("dd/MM/yy HH:mm"));
        //                        w.WriteAttributeString("ExitTime", estTime);
        //                        w.WriteAttributeString("CWStatus", washingStatus);

        //                        w.WriteEndElement();//ODBCTagRow
        //                    }
        //                    w.WriteEndElement();//ODBCXMLData
        //                    w.Close();

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception errMsg)
        //    {

        //        RetVal = 1;
        //    }
        //    return RetVal;
        //}
        //DataTable GetCurrentParks()
        //{
        //    string query = "";
        //    DataTable dt = new DataTable();
        //    try
        //    {
        //        query = " SELECT * FROM rpmsadmin.vw_DisplayBoard";


        //        dt.TableName = "DisplayBoard";
        //        using (OracleConnection con = new OracleConnection( Connection.connectionString))
        //        {
        //            if (con.State == ConnectionState.Closed) con.Open();

        //            using (OracleCommand command = new OracleCommand(query))
        //            {
        //                command.CommandText = query;
        //                command.Connection = con;
        //                OracleDataAdapter dadapter = new OracleDataAdapter(command);
        //                dadapter.Fill(dt);
        //            }
        //        }
        //    }
        //    catch (Exception errMsg)
        //    {
        //        Console.WriteLine(errMsg.Message);
        //    }
        //    finally
        //    { }

        //    return dt;
        //}
    
        /// <summary>
        ///  Create xml for car wash estimation information on kiosk display.
        /// </summary>
        public void CreateCarWashEstXML()
        {
            try
            {
                string inEnglishEstmationTime = "";
                string inArabicEstmationTime = "";

                Connection.GetEstimationTimeForWashing(out inEnglishEstmationTime, out inArabicEstmationTime);

                using (FileStream fs = new FileStream(carWashEstxml, FileMode.Create))
                {
                    using (XmlTextWriter w = new XmlTextWriter(fs, null))
                    {

                        w.Formatting = Formatting.Indented;
                        w.Indentation = 5;

                        w.WriteStartDocument();
                        w.WriteStartElement("KioskConfig");
                        w.WriteStartElement("EstCarwashTime");
                        w.WriteValue(inEnglishEstmationTime);
                        w.WriteEndElement();
                        w.WriteStartElement("EstCarwashTimeArabic");
                        w.WriteValue(inArabicEstmationTime);
                        w.WriteEndElement();
                        w.WriteEndElement();
                        w.Close();

                    }
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);
            }
            finally { }

        }

        /// <summary>
        /// Create xml for showing status on progress bar.
        /// </summary>
        /// <param name="message"></param>
        void DoRefXMLCreation(string message)
        {
            try
            {
                DataTable dt = new DataTable("LoadingMsg");
                dt.Columns.Add("Message");
                DataRow drow = dt.NewRow();
                drow[0] = message;
                dt.Rows.Add(drow);
                string filePath = System.Windows.Forms.Application.StartupPath;
                dt.WriteXml(filePath + @"\LoadingMsg.xml");
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }


        }

        void SetIcon()
        {
            try
            {
                Uri iconUri = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\Car.ico", UriKind.RelativeOrAbsolute);
                this.Icon = BitmapFrame.Create(iconUri);

                //Uri iconERPDiag = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\ERPDiagnostic.ico", UriKind.RelativeOrAbsolute);
                //imgERPDiag.Source = BitmapFrame.Create(iconERPDiag);

                //Uri iconPMSDiag = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\PMSDiagnostic.ico", UriKind.RelativeOrAbsolute);
                //imgPMDDiag.Source = BitmapFrame.Create(iconPMSDiag);

                Uri iconHome = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\home.ico", UriKind.RelativeOrAbsolute);
                imgHome.Source = BitmapFrame.Create(iconHome);

                Uri iconClose = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\Close.ico", UriKind.RelativeOrAbsolute);
                imgClose.Source = BitmapFrame.Create(iconClose);

                Uri nasaLogo = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\NASALogo.jpg", UriKind.RelativeOrAbsolute);
                imgNasaLogo.Source = BitmapFrame.Create(nasaLogo);

                Uri mrslogo = new Uri(System.Windows.Forms.Application.StartupPath + @"\Icons\imgMrsLogo.png", UriKind.RelativeOrAbsolute);
                imgMrsLogo.Source = BitmapFrame.Create(mrslogo);
            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message + ", " + errMsg.Source);
            }
            finally { 
            }
            
        }
        
        #endregion
        
      
        /// <summary>
        /// Register notification from opc server.
        /// </summary>
        //private void RegisterNotificationFromOPCServer()
        //{
        //    DataTable dtResult = new DataTable();
        //    Connection con = new Connection();
        //    dtResult = con.GetMachineAlertTags();

        //    try
        //    {

        //        //add a periodic data callback group and add one item to the group
        //        OPCDA.NET.RefreshEventHandler dch = new OPCDA.NET.RefreshEventHandler(GetAlert);
        //        uGrp = new OPCDA.NET.RefreshGroup(ARCPSGUI.OPC.OPCServerManagement.opcServer, DAUpdateRate, dch);
        //        int rtc = ARCPSGUI.OPC.OPCServerManagement.rtc;
        //        OPCServerDirector opcd = new OPCServerDirector();

        //        foreach (DataRow dRow in dtResult.Rows)
        //        {

        //            string channel = dRow["channel"].ToString();
        //            string machine = dRow["machine"].ToString();
        //            string tagName = dRow["tag_name"].ToString();

        //            if ((machine.Contains("LCM") || machine.Contains("UCM")) && con.HasCMInL2Mode(machine))
        //                rtc = uGrp.Add(channel + "." + machine + "." + tagName);
        //            else if (!machine.Contains("LCM") && !machine.Contains("UCM"))
        //                rtc = uGrp.Add(channel + "." + machine + "." + tagName);
        //        }

        //        //create fire alarm.
        //        rtc = uGrp.Add("CH433.EES_FLR4_03.FireAlarm");

        //        //create Generator Mode.
        //        rtc = uGrp.Add("CH431.EES_FLR4_01.ATS_On_Genrator_Power");

        //        //create car wash finish.
        //        rtc = uGrp.Add("CH003.VLC_Drive_03.CW_Fin");
        //    }
        //    catch (Exception errMsg)
        //    {
        //        MessageBox.Show(errMsg.Message + ", " + errMsg.Source,"RegisterNotificationFromOPCServer");
        //    }
        //}

        /// <summary>
        /// OPC handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void GetAlert(object sender, OPCDA.NET.RefreshEventArguments arg)
        {
            try
            {
                if (this.Dispatcher.CheckAccess())
                {
                    this.Dispatcher.BeginInvoke(new OPCDA.NET.RefreshEventHandler(GetAlert), new object[] { sender, arg });
                    return;
                }

                OPCDA.NET.OPCItemState res = arg.items[0].OpcIRslt;
                if (arg.Reason == OPCDA.NET.RefreshEventReason.DataChanged)
                {
                    if (HRESULTS.Succeeded(res.Error))
                    {
                        OPCDA.NET.ItemDef opcItemDef = (OPCDA.NET.ItemDef)arg.items.GetValue(0);

                        if (opcItemDef.OpcIDef.ItemID.Contains("ATS_On_Genrator_Power"))
                            DisplayGeneratorModeNotificaiton(Convert.ToBoolean(res.DataValue));
                        else if (opcItemDef.OpcIDef.ItemID.Contains("CH433.EES_FLR4_03.FireAlarm"))
                            DisplayFireAlarmNotificaiton(Convert.ToBoolean(res.DataValue));
                        else if (opcItemDef.OpcIDef.ItemID.Contains("CH003.VLC_Drive_03.CW_Fin"))
                            CarWashNotificationControl(); //EnableCarWashFinishTrigger(Convert.ToBoolean(res.DataValue));
                        else
                            AddNotifiedControlsV1(opcItemDef.OpcIDef.ItemID);
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
        }

       

        /// <summary>
        /// Notification creation.
        /// </summary>
        /// <param name="instruction"></param>
        /// <param name="isInL2Mode"></param>
        /// <param name="waitingCommandDone"></param>
        /// <param name="isCarAtEESInitialCheck"></param>
        void AddNotifiedControlsV1(string instruction, bool isInL2Mode = true, string waitingCommandDone = "",
          bool isCarAtEESInitialCheck = false)
        {
            string machine = "";
            string tagName = "";
            string machineShortName = "";
            string[] split = null;
            string floor = "";
            string machieUniqueName = "";
            string message = "";
            string channel = "";
            bool autoMode = false;
            Int16 errorCode = 0;
            string statusMessage = "";
            bool isPowerOn = false;
            string automanual = "";
            string machineModel = "";
            bool blinkingEffect = false;
            bool _isCarAtEES = false;
            bool carSenseAlarm = false;
            bool lcmRotationDoneAlarm = false;
            bool needToShowCommandDoneFaileAlarm = false;
            OPCServerDirector opcd = new OPCServerDirector();

            try
            {
                split = instruction.Split('.');

                if (split.Count() == 3)
                {
                    channel = split[0].ToString().Trim();
                    machine = split[1].ToString().Trim();
                    tagName = split[2].ToString().Trim();

                    new Connection().GetMachineShortName(machine, out machineShortName, out floor);

                    machieUniqueName = machineShortName.Trim() + floor;
                }


                if (machineShortName.Contains("LCM")) machineModel = "LCM";
                else if (machineShortName.Contains("UCM")) machineModel = "LCM";
                else if (machineShortName.Contains("PS")) machineModel = "PS";
                else if (machineShortName.Contains("PVL")) machineModel = "PVL";
                else if (machineShortName.Contains("REM")) machineModel = "REM";
                else if (machineShortName.Contains("EES")) machineModel = "EES";
                else if (machineShortName.Contains("VLC")) machineModel = "VLC";
                else if (machineShortName.Contains("PST1")) machineModel = "PST100";
                else if (machineShortName.Contains("PST2")) machineModel = "PST1000";
                else if (machineShortName.Contains("PST3")) machineModel = "PST1000";
                else if (machineShortName.Contains("PST4")) machineModel = "PST1000";

                if (machine.Contains("LCM") || machine.Contains("UCM") || machine.Contains("PS_") || machine.Contains("REM_"))
                {
                    isPowerOn = opcd.IsMachineQualityHealthy(channel + "." + machine + "." + "Auto_Mode") == qualityBits.good ? true : false;
                    if (isPowerOn == true)
                    {
                        if (!machine.Contains("REM_"))
                            autoMode = opcd.ReadTag<bool>(channel + "." + machine + "." + "Auto_Mode");
                        else if (machine.Contains("REM_"))
                        {
                            string tmp = new Connection().GetMachineRelatedToREM(machine);
                            if (string.IsNullOrEmpty(tmp) == false && (tmp.Contains("LCM") || tmp.Contains("UCM")))
                                autoMode = HasCMInAutoMode(tmp);
                        }

                        if (machine.Contains("LCM") || machine.Contains("UCM") || machine.Contains("PS_"))
                            errorCode = opcd.ReadTag<Int16>(channel + "." + machine + "." + "L2_Error_Data_Register");

                        if (machine.Contains("LCM"))
                            lcmRotationDoneAlarm = opcd.ReadTag<bool>(channel + "." + machine + "." + "L2_ROT_FALSE_ALARM");

                        if (machine.Contains("PS_FLR4"))
                            needToShowCommandDoneFaileAlarm = opcd.ReadTag<bool>(channel + "." + machine + "." + "L2_CMD_DONE_ALM");

                        if (machine.Contains("REM_") && errorCode == 0)
                        {
                            errorCode = opcd.ReadTag<Int16>(channel + "." + machine + "." + "L2_Error");
                        }


                    }
                }
                else if (machine.Contains("VLC") || machine.Contains("PVL"))
                {
                    isPowerOn = opcd.IsMachineQualityHealthy(channel + "." + machine + "." + "Auto_Mode") == qualityBits.good ? true : false;
                    if (isPowerOn == true)
                    {
                        autoMode = opcd.ReadTag<bool>(channel + "." + machine + "." + "Auto_Mode");
                        if (machine.Contains("VLC")) autoMode = HasVLCInAutoMode(machine);
                        errorCode = opcd.ReadTag<Int16>(channel + "." + machine + "." + "L2_ErrCode");
                    }
                }
                else if (machine.Contains("EES"))
                {
                    isPowerOn = opcd.IsMachineQualityHealthy(channel + "." + machine + "." + "Auto_Mode") == qualityBits.good ? true : false;
                    if (isPowerOn == true)
                    {
                        autoMode = opcd.ReadTag<bool>(channel + "." + machine + "." + "Auto_Mode");
                        errorCode = opcd.ReadTag<Int16>(channel + "." + machine + "." + "L2_ErrCode");
                    }
                }
                else if (machine.Contains("PST_"))
                {
                    isPowerOn = opcd.IsMachineQualityHealthy(channel + "." + machine + "." + "Auto_Mode") == qualityBits.good ? true : false;
                    if (isPowerOn == true)
                    {
                        autoMode = opcd.ReadTag<bool>(channel + "." + machine + "." + "Auto_Mode");
                        errorCode = opcd.ReadTag<Int16>(channel + "." + machine + "." + "L2_ErrCode");
                    }
                }

                if (machine.Contains("LCM") || machine.Contains("UCM"))
                    isInL2Mode = dbcon.HasCMInL2Mode(machine);
                else if (machine.Contains("VLC"))
                    isInL2Mode = dbcon.HasVLCInL2Mode(machine);
                else if (machine.Contains("PST_"))
                    isInL2Mode = dbcon.HASPSTnL2Mode(machine);
                else if (machine.Contains("PS_"))
                    isInL2Mode = dbcon.HasPSInL2Mode(machine);
                else if (machine.Contains("PVL"))
                    isInL2Mode = dbcon.HasPVLTnL2Mode(machine);
                else if (machine.Contains("EES"))
                {
                    isInL2Mode = dbcon.HasEESTnL2Mode(machine);

                    if (tagName.Contains("CAR_AT_EES_ALARM") || isCarAtEESInitialCheck)
                    {
                        _isCarAtEES = opcd.ReadTag<bool>(channel + "." + machine + "." + tagName);
                        blinkingEffect = _isCarAtEES;
                    }
                    else if (tagName.Contains("CAR_SENSE_ALARM"))
                    {
                        carSenseAlarm = opcd.ReadTag<bool>(channel + "." + machine + "." + tagName);
                        blinkingEffect = carSenseAlarm;
                    }
                }


                if (errorCode > 0 && machine.Contains("REM") == false)
                    message = string.IsNullOrEmpty(message) ? "Error Code = " + errorCode : System.Environment.NewLine + "Error Code = " + errorCode;
                else if (errorCode > 0 && machine.Contains("REM") == true)
                    message = string.IsNullOrEmpty(message) ? "REM Error = " + errorCode : System.Environment.NewLine + "REM Code = " + errorCode;
                else if (tagName.Contains("CAR_AT_EES_ALARM") || isCarAtEESInitialCheck)
                    message = _isCarAtEES ? "Is Waiting." : "";
                else if (tagName.Contains("CAR_SENSE_ALARM"))
                    message = carSenseAlarm ? "sense the car." : "";
                else
                    message = "";

                if (isInL2Mode == false)
                    statusMessage = "Not In L2";
                if (isPowerOn == false && machine.Contains("REM") == false)
                    statusMessage = "Power Off";

                if (autoMode == true)
                    automanual = "Auto";
                else
                    automanual = "Manual";

                if (machine.Contains("LCM") && lcmRotationDoneAlarm)
                {
                    automanual = "Check Rotation";
                    message = " of " + machine;
                }
                if (machine.Contains("PS_FLR4") && needToShowCommandDoneFaileAlarm)
                    message = "Check Command Done";


                if (tagName.Contains("CAR_AT_EES_ALARM") || isCarAtEESInitialCheck)
                    automanual = _isCarAtEES ? "Car At EES" : automanual;
                else if (tagName.Contains("CAR_SENSE_ALARM"))
                    automanual = carSenseAlarm ? "Failed to " : automanual;

                stkMachineNotification.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ucNotificationV1 ctrlNotification = null;
                    var vbListNotification = stkMachineNotification.Children.OfType<Viewbox>();

                    if (autoMode == false || errorCode > 0 || isInL2Mode == false || isPowerOn == false 
                        || !string.IsNullOrEmpty(message)
                        || (machine.Contains("LCM") && lcmRotationDoneAlarm))
                    {
                        if (vbListNotification != null)
                        {
                            var vbCtrl = from t in vbListNotification
                                         where t.Name == machieUniqueName
                                         select t;
                            if (vbCtrl.Count() > 0)
                            {
                                Viewbox vbNotification = vbCtrl.First() as Viewbox;
                                if (vbNotification != null && vbNotification.Child != null)
                                {
                                    ctrlNotification = vbNotification.Child as ucNotificationV1;

                                }
                            }
                        }

                        if (ctrlNotification == null)
                        {
                            ctrlNotification = new ucNotificationV1();

                            ctrlNotification.OnDoubleClick += new EventHandler(ctrlNotification_OnDoubleClick);
                            ctrlNotification.Name = machieUniqueName;
                            Viewbox vb = new System.Windows.Controls.Viewbox();
                            vb.Child = ctrlNotification;
                            vb.Name = machieUniqueName;
                            vb.Uid = machieUniqueName;
                            ctrlNotification.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                            ctrlNotification.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                            vb.Height = 55;
                            vb.Width = 90;
                            vb.Stretch = Stretch.Fill;
                            ctrlNotification.channel = channel;
                            ctrlNotification.machineName = machine;
                            stkMachineNotification.Children.Insert(0, vb);

                            if (blinkingEffect)
                                ctrlNotification.HasBlinkingEffect();

                        }

                        if (carSenseAlarm) ctrlNotification.ShowGreenBorder = true;

                        if (floor != "0")
                            ctrlNotification.lblMachineName.Content = "L-" + floor + " -" + machineShortName;
                        else
                            ctrlNotification.lblMachineName.Content = machineShortName + "   ";

                        ctrlNotification.IsAutoMode = autoMode;
                        ctrlNotification.IsError = (errorCode > 0);

                        ctrlNotification.IsDiable = !isInL2Mode;
                        ctrlNotification.IsPowerOn = isPowerOn;

                        ctrlNotification.lblCommandDoneStatus.Content = waitingCommandDone;

                        if (string.IsNullOrEmpty(statusMessage) == false)
                            ctrlNotification.ErrorMessage(statusMessage);
                        else
                            ctrlNotification.ErrorMessage(message);

                        ctrlNotification.AutoManualMessage(automanual);

                        if (isPowerOn == false)
                        {
                            ctrlNotification.IsEnabled = false;
                        }

                        string alamMessage = "";
                        if (string.IsNullOrEmpty(statusMessage) == false) alamMessage = statusMessage;
                        else if (string.IsNullOrEmpty(automanual) == false) alamMessage = statusMessage;

                        InsertIntoAlarmTable(machineShortName, errorCode, alamMessage, floor, machineModel);
                    }
                    else if (autoMode == true && errorCode == 0 && isInL2Mode == true && isPowerOn == true)
                    {
                        var vbListNotification1 = stkMachineNotification.Children.OfType<Viewbox>();

                        if (vbListNotification1 != null)
                        {
                            var vbCtrl = from t in vbListNotification1
                                         where t.Name == machieUniqueName
                                         select t;
                            if (vbCtrl.Count() > 0)
                            {
                                Viewbox vbNotification = vbCtrl.First() as Viewbox;

                                if (vbNotification != null)
                                {
                                    stkMachineNotification.Children.Remove(vbNotification);
                                }
                            }
                        }
                    }
                }));
            }
            catch (Exception errMsg)
            {

            }
        }
        
        /// <summary>
        /// Display generator mode notification.
        /// </summary>
        /// <param name="isInGeneratorMode"></param>
        void DisplayGeneratorModeNotificaiton(bool isInGeneratorMode)
        {
            try
            {
                
                lblGeneratorMode.Dispatcher.BeginInvoke(new Action(() =>
                {
                   lblGeneratorMode.Visibility = isInGeneratorMode ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                }));
            }
            catch (Exception errMsg)
            {
            }
        }

        /// <summary>
        /// Display fire alarm notification.
        /// </summary>
        /// <param name="isFireAlarm"></param>
        void DisplayFireAlarmNotificaiton(bool isFireAlarm)
        {
            try
            {
                lblGeneratorMode.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lblGeneratorMode.Visibility = isFireAlarm ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
                    lblGeneratorMode.Content = "Fire Alarm";
                }));
            }
            catch (Exception errMsg)
            {

            }
        }

        /// <summary>
        /// Add notification of demo mode machine.
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="isInDemoMode"></param>
        void AddNotifiedForDemoMachine(string machine, bool isInDemoMode)
        {
            try
            {
                stkMachineNotification.Dispatcher.BeginInvoke(new Action(() =>
                {
                    ucNotificationV1 ctrlNotification = null;
                    Viewbox vbNotification = null;

                    var vbListNotification = stkMachineNotification.Children.OfType<Viewbox>();

                    var vbCtrlList = from t in vbListNotification
                                     where t.Name == machine
                                     select t;

                    if (vbCtrlList.Count() > 0) vbNotification = vbCtrlList.First() as Viewbox;

                    if (vbNotification == null && isInDemoMode)
                    {
                        ctrlNotification = new ucNotificationV1(false);
                        ctrlNotification.Name = machine;
                        Viewbox vb = new System.Windows.Controls.Viewbox();
                        vb.Child = ctrlNotification;
                        vb.Name = machine;
                        vb.Uid = machine;
                        ctrlNotification.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        ctrlNotification.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                        vb.Height = 65;
                        vb.Width = 100;
                        vb.Stretch = Stretch.Fill;
                        ctrlNotification.machineName = machine;
                        ctrlNotification.lblMachineName.Content = machine;
                        ctrlNotification.lblCommandDoneStatus.Content = "Demo Mode";
                        stkMachineNotification.Children.Insert(0, vb);

                    }
                    else if (vbNotification != null && !isInDemoMode)
                    {
                        stkMachineNotification.Children.Remove(vbNotification);
                    }
                }));
            }
            catch (Exception errMsg)
            {
            }
            finally
            {

            }
        }

        /// <summary>
        /// Remove notificaiton on user click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ctrlNotification_OnDoubleClick(object sender, EventArgs e)
        {
            try
            {
                string uniqueName = "";
                string machineName = "";

                string[] split = Convert.ToString(sender).Split(',');
                if (split.Length == 2)
                {
                    uniqueName = split[0];
                    machineName = split[1];
                }

                if (string.IsNullOrEmpty(uniqueName) == false)
                {
                    var vbListNotification1 = stkMachineNotification.Children.OfType<Viewbox>();
                    if (vbListNotification1 != null)
                    {
                        var vbCtrl = from t in vbListNotification1
                                     where t.Name == uniqueName
                                     select t;
                        if (vbCtrl.Count() > 0)
                        {
                            Viewbox vbNotification = vbCtrl.First() as Viewbox;

                            if (vbNotification != null)
                            {
                                stkMachineNotification.Children.Remove(vbNotification);
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
            }
            finally { }
        }

        void RunTimeTableAlarmNotification(List<string> lstMachineName)
        {
            try
            {
                //stkMachineNotification.Dispatcher.BeginInvoke(new Action(() =>
                //{
                    foreach (string machineName in lstMachineName)
                    {
                        ucNotificationV1 ctrlNotification = null;
                        Viewbox vbNotification = null;

                        var vbListNotification = stkMachineNotification.Children.OfType<Viewbox>();

                        //var vbCtrlList = from t in vbListNotification
                        //                 where t.Name == machine
                        //                 select t;

                        //  if (vbCtrlList.Count() > 0) vbNotification = vbCtrlList.First() as Viewbox;

                        //if (vbNotification == null)
                        //{
                        ctrlNotification = new ucNotificationV1(false);
                        ctrlNotification.Name = machineName;
                        Viewbox vb = new System.Windows.Controls.Viewbox();
                        vb.Child = ctrlNotification;
                        vb.Name = machineName;
                        vb.Uid = machineName;
                        ctrlNotification.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                        ctrlNotification.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                        vb.Height = 65;
                        vb.Width = 100;
                        vb.Stretch = Stretch.Fill;
                        ctrlNotification.machineName = machineName;
                        ctrlNotification.lblMachineName.Content = machineName;
                        ctrlNotification.lblCommandDoneStatus.Content = "Reset";
                        ctrlNotification.txtbMessage.Text = "Run Time Table";
                        stkMachineNotification.Children.Insert(0, vb);
                    }
                   // }
                    //else if (vbNotification != null && !isInDemoMode)
                    //{
                    //    stkMachineNotification.Children.Remove(vbNotification);
                    //}
                //}));
            }
            catch (Exception errMsg)
            {
            }
            finally
            {

            }
        }

        void CarWashNotificationControl()
        {
            try
            {
                bool isCarWashFinished = dbcon.IsCarWashFinished();
                if (isCarWashFinished && dbcon.GetCarPresentInCarWash() == true)
                {
                    btnCarWashFinish.Dispatcher.Invoke(new Action(() =>
                    {
                        btnCarWashFinish.IsEnabled = true;
                        btnCarWashFinish.Template = FindResource("GreenGlassButton") as System.Windows.Controls.ControlTemplate;
                    }));
                }
                else if (dbcon.IsCarWashReady())
                {
                    btnCarWashFinish.Dispatcher.Invoke(new Action(() =>
                    {
                        btnCarWashFinish.IsEnabled = false;
                        btnCarWashFinish.Template = FindResource("GlassButton") as System.Windows.Controls.ControlTemplate;
                    }));
                }


            }
            catch (Exception errMsg)
            {

            }
        
        }

       
        
        #region Initialize assign of machine status
        public void InitialAssignOfCMStatus()
        {
            string query = "";
            try
            {
                query = "select DISTINCT channel, MACHINE_CODE,STATUS FROM L2_LCM_UCM_MASTER T INNER JOIN l2_opc_tag_master OPC "
                            + " ON T.MACHINE_CODE = OPC.MACHINE WHERE (OPC.MACHINE LIKE 'LCM%' OR OPC.MACHINE LIKE 'UCM%') AND STATUS != 2 ORDER BY MACHINE_CODE";

                bool isInL2Mode = false;
                string instruction = "";
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;

                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dreader.Read())
                            {
                                instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                                isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;
                                AddNotifiedControlsV1(instruction, isInL2Mode);
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
            finally { }
        }
        public void InitialAssignOfVLCStatus()
        {
            try
            {
                string query = "select DISTINCT channel, MACHINE_CODE,STATUS FROM L2_VLC_MASTER T INNER JOIN l2_opc_tag_master OPC "
                               + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE LIKE 'VLC%' AND STATUS != 2 ORDER BY MACHINE_CODE";
                bool isInL2Mode = false;
                string instruction = "";
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dreader.Read())
                            {
                                instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                                isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;
                                AddNotifiedControlsV1(instruction, isInL2Mode);
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
        }
        public void InitialAssignOfPSStatus()
        {
            try
            {
                string query = "select DISTINCT channel, MACHINE_CODE,STATUS FROM L2_PS_MASTER T INNER JOIN l2_opc_tag_master OPC "
                               + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE LIKE 'PS_%' AND STATUS != 2  ORDER BY MACHINE_CODE";
                bool isInL2Mode = false;
                string instruction = "";
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        // dep = new OracleDependency(command);
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dreader.Read())
                            {
                                instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                                isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;
                                AddNotifiedControlsV1(instruction, isInL2Mode);
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
        }
        public void InitialAssignOfPSTStatus()
        {
            try
            {
                string query = "select DISTINCT channel, MACHINE_CODE,STATUS FROM L2_PST_MASTER T INNER JOIN l2_opc_tag_master OPC "
                            + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE LIKE 'PST_%' AND STATUS != 2 ORDER BY MACHINE_CODE";
                bool isInL2Mode = false;
                string instruction = "";
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        //   dep = new OracleDependency(command);
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dreader.Read())
                            {
                                instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                                isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;
                                AddNotifiedControlsV1(instruction, isInL2Mode);
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
            // AddNotifiedControlsV1(instruction, isInL2Mode);
        }
        public void InitialAssignOfPVLStatus()
        {
            try
            {
                string query = "select channel, MACHINE_CODE, STATUS FROM L2_PVL_MASTER T INNER JOIN l2_opc_tag_master OPC "
                              + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE LIKE 'EES%' AND STATUS != 2 ORDER BY MACHINE_CODE";
                bool isInL2Mode = false;
                string instruction = "";
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        // dep = new OracleDependency(command);
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dreader.Read())
                            {
                                instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                                isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;
                                AddNotifiedControlsV1(instruction, isInL2Mode);
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
            // AddNotifiedControlsV1(instruction, isInL2Mode);
        }
        public void InitialAssignOfEESStatus()
        {
            try
            {
                string query = "select DISTINCT channel, MACHINE_CODE, STATUS FROM L2_EES_MASTER T INNER JOIN l2_opc_tag_master OPC "
                              + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE LIKE 'EES%' AND STATUS != 2 ORDER BY MACHINE_CODE";
                bool isInL2Mode = false;
                string instruction = "";
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        // dep = new OracleDependency(command);
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dreader.Read())
                            {
                                instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "CAR_AT_EES_ALARM";

                                isInL2Mode = Convert.ToInt32(dreader[2]) != 2 ? false : true;
                                AddNotifiedControlsV1(instruction, isInL2Mode);
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
            //AddNotifiedControlsV1(instruction, isInL2Mode);
        }
        public void CarAtEESNotification()
        {
            try
            {
                string query = "select DISTINCT channel, MACHINE_CODE FROM L2_EES_MASTER T INNER JOIN l2_opc_tag_master OPC "
                              + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE LIKE 'EES%' AND tag_name = 'CAR_AT_EES_ALARM' ORDER BY MACHINE_CODE";
                bool isInL2Mode = false;
                string instruction = "";
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        // dep = new OracleDependency(command);
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            while (dreader.Read())
                            {
                                instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "CAR_AT_EES_ALARM";
                                AddNotifiedControlsV1(instruction, true, "", true);
                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
            //AddNotifiedControlsV1(instruction, isInL2Mode);
        }
        #endregion

        public bool HasCMInAutoMode(string machineName)
        {
            bool isInAutoMode = false;
            try
            {
                string query = "select DISTINCT channel, MACHINE_CODE,AUTO_MODE FROM L2_LCM_UCM_MASTER T INNER JOIN l2_opc_tag_master OPC "
                             + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE = '" + machineName + "'";


                string instruction = "";
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (dreader.HasRows == true)
                            {
                                dreader.Read();
                                instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                                isInAutoMode = Convert.ToInt32(dreader[2]) != 1 ? false : true;

                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
            return isInAutoMode;
        }
        public bool HasEESInAutoMode(string machineName)
        {
            string query = "select DISTINCT channel, MACHINE_CODE,IS_AUTOMODE FROM L2_EES_MASTER T INNER JOIN l2_opc_tag_master OPC "
                         + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE = '" + machineName + "'";

            bool isInAutoMode = false;
            string instruction = "";

            try
            {
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (dreader.HasRows == true)
                            {
                                dreader.Read();
                                instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                                isInAutoMode = Convert.ToInt32(dreader[2]) != 1 ? false : true;

                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
            return isInAutoMode;
        }
        public bool HasVLCInAutoMode(string machineName)
        {
            string query = "select DISTINCT channel, MACHINE_CODE,IS_AUTOMODE FROM L2_VLC_MASTER T INNER JOIN l2_opc_tag_master OPC "
                         + " ON T.MACHINE_CODE = OPC.MACHINE WHERE OPC.MACHINE = '" + machineName + "'";

            bool isInAutoMode = false;
            string instruction = "";
            try
            {
                using (con = new OracleConnection( Connection.connectionString))
                {
                    if (con.State == ConnectionState.Closed) con.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        command.Connection = con;
                        using (OracleDataReader dreader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (dreader.HasRows == true)
                            {
                                dreader.Read();
                                instruction = Convert.ToString(dreader[0]) + "." + Convert.ToString(dreader[1]) + "." + "TAG";
                                isInAutoMode = Convert.ToInt32(dreader[2]) != 1 ? false : true;

                            }
                        }
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
            return isInAutoMode;
        }

        /// <summary>
        /// Create notification for machine enable/disable status from diagnostic ui.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnChange(object sender, EventArgs e)
        {
            try
            {
                string[] split;
                split = sender.ToString().Split(new Char[] { ',' });
                string channel = "";
                string machineCode = "";
                Int32 status = 0;
                bool isMachineInL2 = false;
                if (split != null && split.Count() == 3)
                {
                    channel = split[0].ToString();
                    machineCode = split[1].ToString();
                    status = Convert.ToInt32(split[2]);
                    if (status == 2)
                        isMachineInL2 = true;
                    else
                        isMachineInL2 = false;

                    AddNotifiedControlsV1(channel + "." + machineCode + "." + "tag", isMachineInL2);
                }
            }
            catch (Exception errMsg)
            {

            }
        }

        //void EesQueueNotification()
        //{
        //    string query = "select * from L2_EES_QUEUE"; 
        //    DataTable dt = new DataTable();
        //    try
        //    {
              
        //        dt.TableName = "EESQUEUE";
        //        using (OracleConnection conEEsQueue = new OracleConnection( Connection.connectionString))
        //        {
        //            if (conEEsQueue.State == ConnectionState.Closed) conEEsQueue.Open();

        //            using (OracleCommand command = new OracleCommand(query))
        //            {
        //                command.CommandText = query;
        //                OracleDependency depEESQueue = new OracleDependency(command);
        //                command.Notification.IsNotifiedOnce = false;
        //                depEESQueue.OnChange += new OnChangeEventHandler(OnEESQueueNotification);
        //                command.Connection = conEEsQueue;

        //                OracleDataAdapter dadapter = new OracleDataAdapter(command);
        //                depEESQueue.QueryBasedNotification = false;
        //                dadapter.Fill(dt);
        //            }
        //        }
        //    }
        //    catch (Exception errMsg)
        //    {

        //    }
        //}

        //void SlotPathNotification()
        //{
        //    string query = "select * from L2_SLOT_PATH"; //order by AISLE";
        //    try
        //    {
        //        DataTable dt = new DataTable();
        //        dt.TableName = "SLOTPATH";
        //        using (OracleConnection conSlotPath = new OracleConnection( Connection.connectionString))
        //        {
        //            if (conSlotPath.State == ConnectionState.Closed) conSlotPath.Open();

        //            using (OracleCommand command = new OracleCommand(query))
        //            {
        //                command.CommandText = query;
        //                OracleDependency depSlotPath = new OracleDependency(command);
        //                command.Notification.IsNotifiedOnce = false;
        //                depSlotPath.OnChange += new OnChangeEventHandler(OnSlotPathNotification);
        //                command.Connection = conSlotPath;

        //                OracleDataAdapter dadapter = new OracleDataAdapter(command);
        //                depSlotPath.QueryBasedNotification = false;
        //                dadapter.Fill(dt);
        //            }
        //        }
        //    }
        //    catch (Exception errMsg)
        //    {

        //    }
        //}
        void PathDetailsNotification()
        {
            string query = "select * from L2_PATH_DETAILS"; 
            try
            {
                DataTable dt = new DataTable();
                dt.TableName = "PathDetails";
                using (OracleConnection conPathDetails = new OracleConnection( Connection.connectionString))
                {
                    if (conPathDetails.State == ConnectionState.Closed) conPathDetails.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                        OracleDependency depPathDetail = new OracleDependency(command);
                        command.Notification.IsNotifiedOnce = false;
                        depPathDetail.OnChange += new OnChangeEventHandler(OnPathDetailsNotification);

                        command.Connection = conPathDetails;
                        // command.AddRowid = true;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                        depPathDetail.QueryBasedNotification = false;

                        dadapter.Fill(dt);
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
        }
        void CarWashQueueNotification()
        {
            string query = "select * from L2_CAR_WASH_QUEUE"; //order by AISLE";
            try
            {
                DataTable dt = new DataTable();
                dt.TableName = "L2_CAR_WASH_QUEUE";
                using (OracleConnection conCarWash = new OracleConnection( Connection.connectionString))
                {
                    if (conCarWash.State == ConnectionState.Closed) conCarWash.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                         Connection.CarWashDependency = new OracleDependency(command);
                        command.Notification.IsNotifiedOnce = false;
                         Connection.CarWashDependency.OnChange += new OnChangeEventHandler(OnCarWashQueueNotification);
                        command.Connection = conCarWash;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                         Connection.CarWashDependency.QueryBasedNotification = false;
                        dadapter.Fill(dt);
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
        }
        void ConfigNotification()
        {
            string query = "select * from L2_CONFIG_MASTER"; //order by AISLE";
            try
            {
                DataTable dt = new DataTable();
                dt.TableName = "L2_CONFIG_MASTER";
                using (OracleConnection conConfigMaster = new OracleConnection( Connection.connectionString))
                {
                    if (conConfigMaster.State == ConnectionState.Closed) conConfigMaster.Open();

                    using (OracleCommand command = new OracleCommand(query))
                    {
                        command.CommandText = query;
                         Connection.ConfigMasterDependency = new OracleDependency(command);
                        command.Notification.IsNotifiedOnce = false;
                         Connection.ConfigMasterDependency.OnChange += new OnChangeEventHandler(OnConfigNotification);
                        command.Connection = conConfigMaster;
                        OracleDataAdapter dadapter = new OracleDataAdapter(command);
                         Connection.ConfigMasterDependency.QueryBasedNotification = false;
                        dadapter.Fill(dt);
                    }
                }
            }
            catch (Exception errMsg)
            {

            }
        }
        //void CustomerNotification()
        //{
        //    string query = "select * from L2_CUSTOMERS";
        //    DataTable dt = new DataTable();
        //    try
        //    {

        //        dt.TableName = "CUSTOMERS";
        //        using (OracleConnection conCustomer = new OracleConnection( Connection.connectionString))
        //        {
        //            if (conCustomer.State == ConnectionState.Closed) conCustomer.Open();

        //            using (OracleCommand command = new OracleCommand(query))
        //            {
        //                command.CommandText = query;
        //                OracleDependency depCustomer = new OracleDependency(command);
        //                command.Notification.IsNotifiedOnce = false;
        //                depCustomer.OnChange += new OnChangeEventHandler(OnCustomerNotification);
        //                command.Connection = conCustomer;

        //                OracleDataAdapter dadapter = new OracleDataAdapter(command);
        //                depCustomer.QueryBasedNotification = false;
        //                dadapter.Fill(dt);
        //            }
        //        }
        //    }
        //    catch (Exception errMsg)
        //    {

        //    }
        //}

        //void OnEESQueueNotification(object sender, OracleNotificationEventArgs eventArgs)
        //{
        //    try
        //    {
        //        if (OnTriggerEESQueueNotificaiton != null)
        //            OnTriggerEESQueueNotificaiton(sender, eventArgs);

        //        //Task.Factory.StartNew(new Action(() => CreateDispalyXML()));
        //    }
        //    catch (Exception errMsg)
        //    {

        //    }
        //}
   
        //void OnSlotPathNotification(object sender, OracleNotificationEventArgs eventArgs)
        //{
        //    try
        //    {
        //        if (OnTriggerSlotPathNotificaiton != null)
        //            OnTriggerSlotPathNotificaiton(sender, eventArgs);

        //       // Task.Factory.StartNew(new Action(() => CreateDispalyXML()));
        //    }
        //    finally
        //    { }
        //}
        void OnPathDetailsNotification(object sender, OracleNotificationEventArgs eventArgs)
        {
            try
            {
                if (OnTriggerPathDetailsNotification != null)
                    OnTriggerPathDetailsNotification(sender, eventArgs);

            }
            finally { }
        }
      
        //void OnCustomerNotification(object sender, OracleNotificationEventArgs eventArgs)
        //{
        //    try
        //    {
        //        if (OnTriggerCustomerNotification != null)
        //            OnTriggerCustomerNotification(sender, eventArgs);
        //    }
        //    catch (Exception errMsg)
        //    {

        //    }
        //}

        void OnCarWashQueueNotification(object sender, OracleNotificationEventArgs eventArgs)
        {
            try
            {
                Task.Factory.StartNew(new Action(() => CreateCarWashEstXML())); //CreateCarWashEstXML();
            }
            catch (Exception errMsg)
            {

            }
        }
        void OnConfigNotification(object sender, OracleNotificationEventArgs eventArgs)
        {
            CarWashNotificationControl();
            /*
            try
            {
                bool isCarWashFinished = dbcon.IsCarWashFinished();
                if (isCarWashFinished && dbcon.GetCarPresentInCarWash() == true)
                {
                    btnCarWashFinish.Dispatcher.Invoke(new Action(() =>
                        {
                            btnCarWashFinish.IsEnabled = true;
                            btnCarWashFinish.Template = FindResource("GreenGlassButton") as System.Windows.Controls.ControlTemplate;
                        }));
                }
                else if(dbcon.IsCarWashReady())
                {
                    btnCarWashFinish.Dispatcher.Invoke(new Action(() =>
                    {
                        btnCarWashFinish.IsEnabled = false;
                        btnCarWashFinish.Template = FindResource("GlassButton") as System.Windows.Controls.ControlTemplate;
                    }));
                }

            
            }
            catch (Exception errMsg)
            {

            }
             * */
        }

        #region Event Definition
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (guiExe != null)
                guiExe.Kill();
            lblNasaMessage.Visibility = System.Windows.Visibility.Hidden;
            /*
            string licenceKey = "";
            string message = "";

            licenceKey = ConfigurationManager.AppSettings["ValidationKey"].ToString();
            LicValidator lv = new LicValidator();
            long daysLeft = lv.validateLicense(licenceKey, "FCLNASA2013",false);

            lblNasaMessage.Visibility = System.Windows.Visibility.Hidden;
            if (daysLeft == -4)
            {
                lblNasaMessage.Visibility = System.Windows.Visibility.Visible;
                message = "Key has been expired due to system configuration change or try to bypass license. Please contact application vendor";
                MessageBox.Show(message);
                lblNasaMessage.Visibility = message;
                Application.Exit();
            }
            else if (daysLeft == -3)
            {
                lblNasaMessage.Visibility = System.Windows.Visibility.Visible;
                message = "This key is not designed for this application";
                MessageBox.Show(message);
                lblNasaMessage.Text = message;
                Application.Exit();
            }
            else if (daysLeft == -2)
            {
                lblNasaMessage.Visibility = System.Windows.Visibility.Visible;
                message = "This key not registered under this PC, please contact application vendor";
                MessageBox.Show(message);
                lblNasaMessage.Text = message;
                Application.Exit();
            }
            else if (daysLeft == -1)
            {
                lblNasaMessage.Visibility = System.Windows.Visibility.Visible;
                message = "Invalid key, please contact application vendor";
                MessageBox.Show(message);
                lblNasaMessage.Text = message;
                Application.Exit();
            }
            else if (daysLeft == 0)
            {
                lblNasaMessage.Visibility = System.Windows.Visibility.Visible;
                message = "Key has been expired, please contact application vendor";
                MessageBox.Show(message);
                lblNasaMessage.Text = message;
                Application.Exit();
            }
            else if (daysLeft < 10)
            {
                lblNasaMessage.Visibility = System.Windows.Visibility.Visible;
                message = "Key will expire with in " + daysLeft + " days, please contact application vendor to renew";
                MessageBox.Show(message);
                lblNasaMessage.Text = message;
            }
         */
                frmAuthenticationWindow frmAuthentication = new frmAuthenticationWindow();
                frmAuthentication.OnCloseRequest += new EventHandler(frmAuthentication_OnCloseRequest);
                frmAuthentication.ShowDialog();
                lblCurrentUser.Content = Security.Security.GetUserName(Security.Security.currentUserId);
            
        }
        void frmAuthentication_OnCloseRequest(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

            objGeneralDba.UnRegisterGeneralDBNotification();
            OPCServerManagement.dispose();
            BGOPCServerManagement.dispose();
            if (guiExe != null && !guiExe.HasExited) guiExe.Kill();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to close", "Information", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                this.Close();

        }
        //private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        //{
           


           
            
        //    if (guiExe != null && !guiExe.HasExited) guiExe.Kill();
        //}
        //private void btnClose_Click(object sender, RoutedEventArgs e)
        //{
        //    if (MessageBox.Show("Do you want to close", "Information", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
        //    {
        //        if (objGeneralDba == null)
        //            objGeneralDba = new GeneralDba();

        //        objGeneralDba.UnRegisterGeneralDBNotification();
        //        //if (grdContainer.Children.Contains(currentLoadedUI) == true)
        //        //{
        //        //    grdContainer.Children.Remove(currentLoadedUI);
        //        //}
        //        grdContainer.Children.Clear();
        //        this.timerToClose = new DispatcherTimer();
        //        this.timerToClose.Interval = new TimeSpan(6000);
        //        this.timerToClose.Tick += new EventHandler(new EventHandler(timerToClose_Elapsed));
        //        this.timerToClose.Start();
               
                
        //    }

        //}
        int cnt = 0;
        void timerToClose_Elapsed(Object sender, EventArgs e)
        {
            cnt++;
            if (cnt == 1)
                return;
            timerToClose.Stop();
            this.Close();
            //Task.Factory.StartNew(new Action(() =>
            //        {
            //            this.Close();
            //        }));
            //this.Dispatcher.BeginInvoke(new InvokeDelegate(new Action(() =>
            //{
            //    this.Close();
            //})));
        }
        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Maximized;
        }
        private void Window_StateChanged(object sender, EventArgs e)
        {
            this.WindowState = System.Windows.WindowState.Maximized;
        }
        private void btnLogOff_Click(object sender, RoutedEventArgs e)
        {
            lblCurrentUser.Content = "";
            Security.Security.lstSecuredItem.Clear();
            frmAuthenticationWindow frmAuthentication = new frmAuthenticationWindow();
            frmAuthentication.OnCloseRequest += new EventHandler(frmAuthentication_OnCloseRequest);
            frmAuthentication.ShowDialog();
            lblCurrentUser.Content = Security.Security.GetUserName(Security.Security.currentUserId);
            if (currentLoadedUI is ucUser)
                ((ucUser)currentLoadedUI).DoOnLoad();
        }
        private void btnCarWashFinish_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Check the car wash door has opened.", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question)
                 == MessageBoxResult.Yes)
                {
                    if (dbcon.GetCarPresentInCarWash() == true)
                    {
                        dbcon.UpdateCarWashFinishTrigger(2);
                        btnCarWashFinish.IsEnabled = false;
                        btnCarWashFinish.Template = FindResource("GlassButton") as System.Windows.Controls.ControlTemplate;
                    }
                    else
                        MessageBox.Show("L2 logic cannot get car from the car wash slot. It seems that L2 logic may not process this car wash.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception errMsg)
            {

            }
        }
        #endregion

        private void erp_Click(object sender, RoutedEventArgs e)
        {
            //if (grdContainer.Children.Contains(currentLoadedUI) == true && this.currentLoadedUI is uiERPTasks == false)
            //{
            //    UnRegisterNotificationForChildControl(currentLoadedUI);
            //    grdContainer.Children.Remove(currentLoadedUI);
            //}
            //if (this.currentLoadedUI is uiERPTasks == false)
            //    homeui_triggerErpTask(sender, e);

            try
            {

                uiERPTasks erpTask = new uiERPTasks(this);

                grdContainer.Children.Clear();
                grdContainer.Children.Add(erpTask);
                currentLoadedUI = erpTask;
            
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);

            }

        }

        private void pm_but_Click(object sender, RoutedEventArgs e)
        {
            //if (grdContainer.Children.Contains(currentLoadedUI) == true )
            //{
            //    UnRegisterNotificationForChildControl(currentLoadedUI);
            //    grdContainer.Children.Remove(currentLoadedUI);
            //}
            homeui_triggerPMSTransTask(sender,e);
        }

        private void cw_but_Click(object sender, RoutedEventArgs e)
        {
           // if (this.currentLoadedUI is uiCar == false)
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerCarWashView(sender, e);
        }

        private void cp_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerCurrentParks(sender, e);
        }

        private void ht_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerHistory(sender, e);
        }

        private void ab_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerAbortView(sender, e);
        }

        private void al_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerAlarmView(sender, e);
        }

        private void md_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerMemberDetails(sender, e);
        }

        private void sp_but_Click(object sender, RoutedEventArgs e)
        {
          
            homeui_triggerSetPoints(sender, e);
        }

        private void ed_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerParkingDiagnostic_new(sender, e);
            
        }

       

        private void cc_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerSetCMWindow(sender, e);
        }

        private void sm_but_Click(object sender, RoutedEventArgs e)
        {
           
            homeui_triggerSimulation(sender, e);
        }

        private void f1_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerFloorL1(sender, e);
        }

        private void f2_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerFloorL2(sender, e);
        }

        private void f3_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerFloorL3(sender, e);
        }

        private void f4_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerFloorL4(sender, e);
        }

        private void f5_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerFloorL5(sender, e);
        }

        private void f6_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerFloorL6(sender, e);
        }

        private void f7_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerFloorL7(sender, e);
        }

        private void f8_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerFloorL8(sender, e);
        }

        private void f9_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerFloorL9(sender, e);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void PH_but1_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerPeakHourChart(sender, e);
        }

        private void DU_but1_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerDurationChart(sender, e);
        }

        private void wait_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerWaitHistory(sender, e);
        }

        private void log_but1_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerDBLog(sender, e);
        }

        private void menu_but_Click(object sender, RoutedEventArgs e)
        {
            AddMenuUI();
        }

        private void vlc_but_Click(object sender, RoutedEventArgs e)
        {
            if (grdContainer.Children.Contains(currentLoadedUI) == true)
            {
                UnRegisterNotificationForChildControl(currentLoadedUI);
                grdContainer.Children.Remove(currentLoadedUI);
            }
            homeui_triggerSetVLCConfigWindow(sender, e);
        }

         #region OPC and Oracle Notificaton registration And Alarm Creation
     
        void CreateOracleNotification()
        {
            if (objErrorDba == null)
                objErrorDba = new ErrorDba();
            if (objCMDba == null)
                objCMDba = new CMDba();
            if (objVLCDba == null)
                objVLCDba = new VLCDba();
            if (objEESDba == null)
                objEESDba = new EESDba();
            if (objPSDba == null)
                objPSDba = new PSDba();
            if (objPSTDba == null)
                objPSTDba = new PSTDba();
            if (objPVLDba == null)
                objPVLDba = new PVLDba();
            try
            {
                if (ConfigurationManager.AppSettings["ALARM_TRIGGER"] == "1")
                {
                    objErrorDba.RegisterTriggerNotification();
                    objErrorDba.triggerMachineTriggered += new EventHandler(Handle_MachineTriggered);
                }
                //if (ConfigurationManager.AppSettings["ALARM_DISABLE"] == "1")
                //{
                //    objCMDba.RegisterDisabledNotification();
                //    objCMDba.disableMachineTriggered += new EventHandler(Handle_MachineTriggered);
                //    objVLCDba.RegisterDisabledNotification();
                //    objVLCDba.disableMachineTriggered += new EventHandler(Handle_MachineTriggered);

                //    objEESDba.RegisterDisabledNotification();
                //    objEESDba.disableMachineTriggered += new EventHandler(Handle_MachineTriggered);
                //    objPSDba.RegisterDisabledNotification();
                //    objPSDba.disableMachineTriggered += new EventHandler(Handle_MachineTriggered);
                //    objPSTDba.RegisterDisabledNotification();
                //    objPSTDba.disableMachineTriggered += new EventHandler(Handle_MachineTriggered);

                //    objPVLDba.RegisterDisabledNotification();
                //    objPVLDba.disableMachineTriggered += new EventHandler(Handle_MachineTriggered);
                //}
                ConfigNotification();
            }
            catch (Exception errMsg)
            {
                throw errMsg;
            }
        }
        //private void RegisterNotificationFromOPCServer()
        //{
        //    if (objCMDba == null)
        //        objCMDba = new CMDba();
        //    if (objVLCDba == null)
        //        objVLCDba = new VLCDba();
        //    if (objEESDba == null)
        //        objEESDba = new EESDba();
        //    if (objPSDba == null)
        //        objPSDba = new PSDba();
        //    if (objPSTDba == null)
        //        objPSTDba = new PSTDba();
        //    if (objPVLDba == null)
        //        objPVLDba = new PVLDba();


        //    List<CMData> cmList = null;
        //    List<VLCData> vlcList = null;
        //    List<EESData> eesList = null;

        //    List<PSData> psList = null;
        //    List<PSTData> pstList = null;
        //    List<PVLData> pvlList = null;

        //    bool manualAlarmEnabled = ConfigurationManager.AppSettings["ALARM_MANUAL"] == "1";
        //    bool errorAlarmEnabled = ConfigurationManager.AppSettings["ALARM_ERROR"] == "1";


        //    try
        //    {

        //        //add a periodic data callback group and add one item to the group
        //        OPCDA.NET.RefreshEventHandler dch = new OPCDA.NET.RefreshEventHandler(NotificationListener);
        //        uGrp = new OPCDA.NET.RefreshGroup(ARCPSGUI.OPC.OPCServerManagement.opcServer, DAUpdateRate, dch);
        //        int rtc = ARCPSGUI.OPC.OPCServerManagement.rtc;
        //        OPCServerDirector opcd = new OPCServerDirector();

        //        cmList = objCMDba.GetCMList();
        //        vlcList = objVLCDba.GetVLCList();
        //        eesList = objEESDba.GetEESList();

        //        psList = objPSDba.GetPSList();
        //        pstList = objPSTDba.GetPSTList();
        //        pvlList = objPVLDba.GetPVLList();



        //        foreach (CMData cm in cmList)
        //        {
        //            if (manualAlarmEnabled)
        //            {
        //                if (cm.machineCode.Contains("LCM"))
        //                    rtc = uGrp.Add(cm.cmChannel + "." + cm.machineCode + "." + OpcTags.LCM_L2_AUTO_READY);
        //                else
        //                    rtc = uGrp.Add(cm.cmChannel + "." + cm.machineCode + "." + OpcTags.UCM_L2_AUTO_READY);
        //            }
        //            if (errorAlarmEnabled)
        //                rtc = uGrp.Add(cm.cmChannel + "." + cm.machineCode + "." + OpcTags.CM_L2_Error_Data_Register);

        //        }
        //        foreach (VLCData vlc in vlcList)
        //        {
        //            if (manualAlarmEnabled)
        //                rtc = uGrp.Add(vlc.machineChannel + "." + vlc.machineCode + "." + OpcTags.VLC_Auto_Ready);
        //            if (errorAlarmEnabled)
        //                rtc = uGrp.Add(vlc.machineChannel + "." + vlc.machineCode + "." + OpcTags.VLC_L2_ErrCode);
        //        }

        //        foreach (EESData ees in eesList)
        //        {
        //            if (errorAlarmEnabled)
        //                rtc = uGrp.Add(ees.machineChannel + "." + ees.machineCode + "." + OpcTags.EES_L2_ErrCode);
        //            if (manualAlarmEnabled)
        //                rtc = uGrp.Add(ees.machineChannel + "." + ees.machineCode + "." + OpcTags.EES_Auto_Ready);
        //        }

        //        foreach (PSData ps in psList)
        //        {
        //            if (errorAlarmEnabled)
        //                rtc = uGrp.Add(ps.machineChannel + "." + ps.machineCode + "." + OpcTags.PS_L2_Error_Data_Register);
        //            if (manualAlarmEnabled)
        //                rtc = uGrp.Add(ps.machineChannel + "." + ps.machineCode + "." + OpcTags.PS_L2_Auto_Ready_Bit);
        //        }
        //        foreach (PSTData pst in pstList)
        //        {
        //            if (errorAlarmEnabled)
        //                rtc = uGrp.Add(pst.machineChannel + "." + pst.machineCode + "." + OpcTags.PST_L2_ErrCode);
        //            if (manualAlarmEnabled)
        //                rtc = uGrp.Add(pst.machineChannel + "." + pst.machineCode + "." + OpcTags.PST_Auto_Ready);
        //        }
        //        foreach (PVLData pvl in pvlList)
        //        {
        //            if (errorAlarmEnabled)
        //                rtc = uGrp.Add(pvl.machineChannel + "." + pvl.machineCode + "." + OpcTags.PVL_L2_ErrCode);
        //            if (manualAlarmEnabled)
        //                rtc = uGrp.Add(pvl.machineChannel + "." + pvl.machineCode + "." + OpcTags.PVL_Auto_Ready);
        //        }

        //    }
        //    catch (Exception errMsg)
        //    {
        //        MessageBox.Show(errMsg.Message + ", " + errMsg.Source, "RegisterNotificationFromOPCServer");
        //    }
        //}
        //private void NotificationListener(object sender, OPCDA.NET.RefreshEventArguments arg)
        //{
        //    try
        //    {
        //        if (this.Dispatcher.CheckAccess())
        //        {
        //            this.Dispatcher.BeginInvoke(new OPCDA.NET.RefreshEventHandler(NotificationListener), new object[] { sender, arg });
        //            return;
        //        }

        //        OPCDA.NET.OPCItemState res = arg.items[0].OpcIRslt;
        //        if (arg.Reason == OPCDA.NET.RefreshEventReason.DataChanged)
        //        {
        //            if (HRESULTS.Succeeded(res.Error))
        //            {
        //                OPCDA.NET.ItemDef opcItemDef = (OPCDA.NET.ItemDef)arg.items.GetValue(0);

        //                updateDataFromOpcListener = new Thread(delegate()
        //                {
        //                    PopNotification(opcItemDef.OpcIDef.ItemID, res.DataValue);
        //                });
        //                updateDataFromOpcListener.IsBackground = true;
        //                updateDataFromOpcListener.Start();
        //            }
        //        }
        //    }
        //    catch (Exception errMsg)
        //    {

        //    }
        //}


        public void PopNotification<T>(string OPCItem, T resValue)
        {

            NotificationData objNotificationData = new NotificationData();
            objNotificationData.MachineCode = OPCItem.Split(new Char[] { '.' })[1];
            objNotificationData.OPCTag = OPCItem.Split(new Char[] { '.' })[2];
            objNotificationData.category = NotificationData.errorCategory.NA;


            if (objNotificationData.OPCTag.ToUpper().Contains("ERR"))
            {
                objNotificationData.category = NotificationData.errorCategory.ERROR;
                objNotificationData.ErrorCode = resValue.ToString();
                objNotificationData.IsCleared = objNotificationData.ErrorCode.Equals("0");

            }
            else if (objNotificationData.OPCTag.ToUpper().Contains("AUTO"))
            {
                objNotificationData.category = NotificationData.errorCategory.MANUAL;
                objNotificationData.IsCleared = Convert.ToBoolean(resValue) == true;

            }
            ShowNotification(objNotificationData);

        }
        public void ShowNotification(NotificationData objNotificationData)
        {


            stkMachineNotification.Dispatcher.BeginInvoke(new Action(() =>
            {
                var vbListNotification = stkMachineNotification.Children.OfType<ucNotificationNew>();
                ucNotificationNew notification = null;
                if (vbListNotification != null)
                {
                    var vbCtrl = from t in vbListNotification
                                 where t.notification.MachineCode == objNotificationData.MachineCode && t.notification.category == objNotificationData.category
                                 select t;

                    if (vbCtrl.Count() > 0)
                    {
                        notification = vbCtrl.First() as ucNotificationNew;
                        stkMachineNotification.Children.Remove(notification);
                    }
                }
                if (!objNotificationData.IsCleared)
                {
                    if (notification == null)
                        notification = new ucNotificationNew();
                    notification.SetNotificationData(objNotificationData);
                    stkMachineNotification.Children.Insert(0, notification);
                    objGeneralDba.SaveNotification(objNotificationData);
                }
            }));


        }
        private void notifClearBut_Click(object sender, RoutedEventArgs e)
        {
            stkMachineNotification.Children.Clear();
        }

        public void Handle_MachineTriggered(object sender, EventArgs e)
        {

            NotificationData objNotificationData = (NotificationData)sender;

            ShowNotification(objNotificationData);

        }
        public void Handle_LCM_L2_ROT_FALSE_ALARM_Triggered(object sender, EventArgs e)
        {

            NotificationData objNotificationData = (NotificationData)sender;

            ShowNotification(objNotificationData);

        }
	#endregion


    }
}

