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
using System.Threading.Tasks;
using OPCDA.NET;
using ARCPSGUI.DiagnosticScreens;
using ARCPSGUI.Model;

namespace ARCPSGUI.MachineUI
{
    /// <summary>
    /// Interaction logic for ucCM.xaml
    /// </summary>
    [Serializable]
    public partial class ucCM : UserControl,IDisposable
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
        public event EventHandler OnLcmL2RotFalseAlarmTriggered;

        OPCServerDirector objOPCServerDirector = null;
        CMDba objCMDba = null;
        GeneralDba objGeneralDba = null;
        
       

        System.Timers.Timer timerToUpdateStatus = null;
        public ucCM()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objCMDba == null)
                objCMDba = new CMDba();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

           
            AsynchReadSettingsForCM();

            SetRotationStatus(false);
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
            if (this.MachineCode.Contains("LCM"))
                rotateGrid.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegate(SetRotationStatus), !objCMDba.GetCMRotationStatus(MachineCode));
            
           
           
        }
        public bool AsynchReadSettingsForCM()
        {
          
            try
            {
              
                BGOPCServerManagement.GetBGOPCServer(Window.GetWindow(this)).AddGroup(this.MachineCode+"_CM_Group", true, 100, 1, new DataChangeEventHandler(bgGrp1_DataChanged), null, new OnBGSrvAddGroup(onAddGroup));

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
            if (ex== null)
            
            {
                bgGrp1 = grp;
                int size = 2;
                if (this.MachineCode.Contains("LCM"))
                    size = 3;
                OPCItemDef[] itms = new OPCItemDef[size];
               
                itms[0] = new OPCItemDef(this.MachineChannel + "." + this.MachineCode + "." + OpcTags.CM_Position_for_L2, true, 0, typeof(void));
                string remCode = getREMCode(this.MachineCode);
                itms[1] = new OPCItemDef(this.MachineChannel + "." + remCode + "." + OpcTags.CM_Pallet_Present_on_REM, true, 1, typeof(void));
                if(size==3)
                    itms[2] = new OPCItemDef(this.MachineChannel + "." + this.MachineCode + "." + OpcTags.LCM_L2_ROT_FALSE_ALARM, true, 2, typeof(void));
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
                        int resValue = int.Parse(rslt.DataValue.ToString());

                        if (resValue != 0)
                        {
                            CMData cm = new CMData();
                            cm.machineCode = this.MachineCode;
                            cm.position = resValue;
                            this.OnPositionChanged(cm, new EventArgs());
                        }

                    }
                    else if (rslt.HandleClient == 1)
                    {
                        bool resValue = bool.Parse(rslt.DataValue.ToString());
                        pallet.Dispatcher.BeginInvoke(new InvokeDelegate(SetPalletPresentStatus), resValue);
                    }
                    else if (rslt.HandleClient == 3)
                    {
                        bool resValue = bool.Parse(rslt.DataValue.ToString());
                        NotificationData objNotificationData = new NotificationData();
                        objNotificationData.category = NotificationData.errorCategory.ERROR;
                        objNotificationData.ErrorCode = "TT";
                        objNotificationData.MachineCode = this.MachineCode;
                        objNotificationData.IsCleared = !resValue;
                        this.OnLcmL2RotFalseAlarmTriggered(objNotificationData, new EventArgs());
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
                this.BorderThickness = new Thickness(0.5); 
            }
            else
            {
                this.BorderThickness = new Thickness(0); 
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

            
            objCMPop.Show();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            ScaleMachine(1.5F);

        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
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
            ScaleCardIdLabel(3F);
        }
    

        private void cardIdLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            ScaleCardIdLabel(1F);
        }
        void ScaleCardIdLabel(float scaleParam)
        {
            TransformGroup tg = cardIdLabel.RenderTransform as TransformGroup;
            ScaleTransform rt = tg.Children[0] as ScaleTransform;
            rt.ScaleX = scaleParam;
            rt.ScaleY = scaleParam;
        }

        void SetRotationStatus(bool status)
        {
            this.rotateGrid.Visibility = status ? Visibility.Visible : Visibility.Hidden;
        }
        public void Dispose()
        {
            timerToUpdateStatus.Stop();
            timerToUpdateStatus.Dispose();
        }
    }
}
