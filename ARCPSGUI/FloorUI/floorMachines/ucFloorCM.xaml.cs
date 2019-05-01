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
using System.Windows.Threading;
using ARCPSGUI.DB;
using ARCPSGUI.OPC;
using ARCPSGUI.Popup;
using OPC;
using OPCDA.NET;
using ARCPSGUI.Model;

namespace ARCPSGUI.FloorUI.floorMachines
{
    /// <summary>
    /// Interaction logic for ucFloorCM.xaml
    /// </summary>
    public partial class ucFloorCM : UserControl
    {
        public string MachineCode { get; set; }
        public string MachineChannel { get; set; }
        public decimal QueueId { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int MinXValue { get; set; }
        public int MaxXValue { get; set; }
        BGGroup bgGrp1 = null;

        public delegate void InvokeDelegate(bool status);
        public delegate void InvokeDelegateForString(string cardId);
        public event EventHandler OnPositionChanged;

        OPCServerDirector objOPCServerDirector = null;
        CMDba objCMDba = null;
        GeneralDba objGeneralDba = null;
        
       

        System.Timers.Timer timerToUpdateStatus = null;
        public ucFloorCM()
        {
            InitializeComponent();
        }

       

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objCMDba == null)
                objCMDba = new CMDba();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

            try
            {
                AsynchReadSettingsForCM();

            }
            catch(Exception ex)
            {

            }
            this.timerToUpdateStatus = new System.Timers.Timer();
            this.timerToUpdateStatus.Enabled = true;
            this.timerToUpdateStatus.Interval = 3000;
            this.timerToUpdateStatus.Start();
            this.timerToUpdateStatus.Elapsed += new System.Timers.ElapsedEventHandler(timerToUpdateStatus_Elapsed);

            


        }
        private void UserControl_Initialized()
        {
            pallet.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegate(SetPalletPresentStatus), GetPalletPresentStatusFromOpc());
          
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bgGrp1 != null)
                {
                    bgGrp1.Dispose();
                    bgGrp1 = null;
                }

                timerToUpdateStatus.Stop();
                timerToUpdateStatus.Dispose();
                objCMDba = null;
                objGeneralDba = null;
            }
            catch (Exception ex)
            {

            }

        }

        void timerToUpdateStatus_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
           // cmLock.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegate(SetLockedStatus), objCMDba.GetCMBlockedStatus(MachineCode));
            if (objCMDba == null)
                objCMDba = new CMDba();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

            this.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegate(SetLockedStatus), objCMDba.GetCMBlockedStatus(MachineCode));
            triggerStatus.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegate(SetTriggerStatus), objGeneralDba.GetMachineTriggerStatus(MachineCode));
            disableGrid.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegate(SetDisableStatus), !objCMDba.GetCMEnabledStatus(MachineCode));
            this.QueueId = objCMDba.GetCMQueueId(this.MachineCode);
            cardIdLabel.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegateForString(SetCardId), objGeneralDba.GetCardIdFromQueue(this.QueueId));
            
           
           
        }
        

        public bool AsynchReadSettingsForCM()
        {



            try
            {

                BGOPCServerManagement.GetBGOPCServer(Window.GetWindow(this)).AddGroup(this.MachineCode + "_FLOOR_Group", true, 100, 1, new DataChangeEventHandler(bgGrp1_DataChanged), null, new OnBGSrvAddGroup(onAddGroup));

            }
            catch (Exception errMsg)
            {

                Console.WriteLine(errMsg.Message);
            }
            finally
            {

            }

            return true;

        }
        private void onAddGroup(BGException ex, BGGroup grp, object tag)
        {
            if (ex == null)
            {
                bgGrp1 = grp;
                OPCItemDef[] itms = new OPCItemDef[2];
                itms[0] = new OPCItemDef(this.MachineChannel + "." + this.MachineCode + "." + OpcTags.CM_Position_for_L2, true, 0, typeof(void));
                string remCode = getREMCode(this.MachineCode);
                itms[1] = new OPCItemDef(this.MachineChannel + "." + remCode + "." + OpcTags.CM_Pallet_Present_on_REM, true, 1, typeof(void));
                grp.AddItems(itms, null, new OnBGGrpAddItems(onAddItems));
            }
        }
        private void bgGrp1_DataChanged(object sender, DataChangeEventArgs e)
        {
            try
            {
                foreach (OPCItemState rslt in e.sts)
                {
                    if (rslt.Quality == 0)
                        continue;
                    if (rslt.HandleClient == 0)
                    {
                        CMData cm = new CMData();
                        cm.machineCode = this.MachineCode;
                        cm.position = int.Parse(rslt.DataValue.ToString());
                        this.OnPositionChanged(cm, new EventArgs());

                    }
                    else if (rslt.HandleClient == 1)
                    {
                        pallet.Dispatcher.BeginInvoke(new InvokeDelegate(SetPalletPresentStatus), bool.Parse(rslt.DataValue.ToString()));
                    }

                }
            }
            catch (NullReferenceException ex)
            {

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private int[] LastItemHandles;
        private void onAddItems(BGException ex, OPCItemResult[] rslt, int[] srvHandles, object tag)
        {
            if (ex == null)
            {
                LastItemHandles = srvHandles;
            }
        }
        

       
        public string getREMCode(string machineCode)
        {
            string remCode = null;
            //if (machineCode.Contains("LCM"))
            //    remCode = machineCode.Replace("LCM", "REM");
            //else if (machineCode.Contains("UCM"))
            //    remCode = machineCode.Replace("UCM", "REM");
            string[] cmParts = machineCode.Split('_');

            remCode = "REM" + "_" + "FLR" + cmParts[1][cmParts[1].Length - 1] + "_" + cmParts[2];
            return remCode;
        }
        bool GetPalletPresentStatusFromOpc()
        {
            if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
            string remCode = getREMCode(this.MachineCode);

            bool isPresent = false;
            if (objOPCServerDirector.IsMachineQualityHealthy(MachineChannel + "." +
                remCode + "." + OpcTags.CM_Pallet_Present_on_REM) == OPCDA.qualityBits.good)
                isPresent=objOPCServerDirector.ReadTag<bool>(MachineChannel + "." + remCode + "." + OpcTags.CM_Pallet_Present_on_REM);
            return isPresent;
        }
        void SetPalletPresentStatus(bool status)
        {
            this.pallet.Visibility=status ? Visibility.Visible : Visibility.Hidden;
        }
        void SetLockedStatus(bool status)
        {
            //this.cmLock.Visibility = status ? Visibility.Visible : Visibility.Hidden;\
            if(status)
            {
                cvCMBody.Style = (Style)FindResource("cmWorking");
            }
            else
            {
                cvCMBody.Style = (Style)FindResource("cmIdeal");
            }
        }
        void SetTriggerStatus(bool status)
        {
            this.triggerStatus.Visibility = status ? Visibility.Visible : Visibility.Hidden;
        }
        void SetDisableStatus(bool status)
        {
            this.disableGrid.Visibility = status ? Visibility.Visible : Visibility.Hidden;
        }
        void SetCardId(string cardId)
        {
            if (!string.IsNullOrEmpty(cardId))
            {
                this.cardIdLabel.Visibility = Visibility.Visible;
                this.cardIdLabel.Content = cardId;
            }
            else
                this.cardIdLabel.Visibility = Visibility.Hidden;
        }
        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            CMPop objCMPop = null;
            objCMPop = new CMPop();
            objCMPop.MachineCode = this.MachineCode;
            objCMPop.MachineChannel = this.MachineChannel;

            
            //Int32 relx = Math.Abs(Convert.ToInt32(usr.PointFromScreen(new System.Windows.Point(0, 0)).X));
            //Int32 relY = Math.Abs(Convert.ToInt32(usr.PointFromScreen(new System.Windows.Point(0, 0)).Y));
            //objCMPop.Left = relx;
            //objCMPop.Top = relY + Convert.ToInt32(usr.Height);
            objCMPop.Show();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            //this.BorderThickness = new Thickness(1,1,1,1);
            ScaleMachine(1.5F);

        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            //this.BorderThickness = new Thickness(0, 0, 0, 0);
            ScaleMachine(1F);
        }

        void ScaleMachine( float scaleParam)
        {
            TransformGroup tg = this.RenderTransform as TransformGroup;
            ScaleTransform rt = tg.Children[0] as ScaleTransform;
            rt.ScaleX = scaleParam;
            rt.ScaleY = scaleParam;
        }

        private void UserControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            if (objCMDba == null)
                objCMDba = new CMDba();
             if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

            bool isBlocked = false;
            int requestType = -1;
            isBlocked = objCMDba.GetCMBlockedStatus(this.MachineCode);
            this.QueueId = objCMDba.GetCMQueueId(this.MachineCode);
            requestType = this.QueueId != 0 ? objGeneralDba.GetRequestType(this.QueueId) : -1;
            if (isBlocked && QueueId != 0 && (requestType == 1 || requestType == 0 || requestType == 5 || requestType == 6 || requestType == 3)) //entry or exit
            {
                ReallocatePop objReallocatePop = null;
                objReallocatePop = new ReallocatePop();
                objReallocatePop.MachineCode = this.MachineCode;
                objReallocatePop.MachineChannel = this.MachineChannel;
                objReallocatePop.QueueId = this.QueueId;
                objReallocatePop.Show();
            }
        }

        private void cardIdLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            ScaleCardIdLabel(4F);
        }
    

        private void cardIdLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            //this.BorderThickness = new Thickness(0, 0, 0, 0);
            ScaleCardIdLabel(1F);
        }
        void ScaleCardIdLabel(float scaleParam)
        {
            TransformGroup tg = cardIdLabel.RenderTransform as TransformGroup;
            ScaleTransform rt = tg.Children[0] as ScaleTransform;
            rt.ScaleX = scaleParam;
            rt.ScaleY = scaleParam;
        }

    }
}
