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

namespace ARCPSGUI.MachineUI
{
    /// <summary>
    /// Interaction logic for ucVLC_new.xaml
    /// </summary>
    public partial class ucVLC_new : UserControl
    {
        public string MachineCode { get; set; }
        public string MachineChannel { get; set; }
        public decimal QueueId { get; set; }

        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int MinYValue { get; set; }
        public int MaxYValue { get; set; }

        BGGroup bgGrp1 = null;
        public event EventHandler OnPositionChanged;
        public delegate void InvokeDelegate(bool status);
        public delegate void InvokeDelegateForString(string cardId);
        OPCServerDirector objOPCServerDirector = null;

        System.Timers.Timer timerToUpdateStatus = null;
        VLCDba objVLCDba = null;
        GeneralDba objGeneralDba = null;
        public ucVLC_new()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objVLCDba == null)
                objVLCDba = new VLCDba();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            Task.Factory.StartNew(new Action(() => UserControl_Initialized())); 
            AsynchReadSettingsForVLC();
           
            
            this.timerToUpdateStatus = new System.Timers.Timer();
            this.timerToUpdateStatus.Enabled = true;
            this.timerToUpdateStatus.Interval = 3000;
            this.timerToUpdateStatus.Start();
            this.timerToUpdateStatus.Elapsed += new System.Timers.ElapsedEventHandler(timerToUpdateStatus_Elapsed);

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
        }
        private void UserControl_Initialized()
        {
            //pallet.Dispatcher.BeginInvoke(DispatcherPriority.Background,
            //    new InvokeDelegate(SetPalletPresentStatus), GetPalletPresentStatusFromOpc());

        }
        void timerToUpdateStatus_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new InvokeDelegate(SetLockedStatus), objVLCDba.GetVLCBlockedStatus(MachineCode));
            triggerStatus.Dispatcher.BeginInvoke(DispatcherPriority.Background, new InvokeDelegate(SetTriggerStatus), objGeneralDba.GetMachineTriggerStatus(MachineCode));
            disableGrid.Dispatcher.BeginInvoke(DispatcherPriority.Background, new InvokeDelegate(SetDisableStatus), !objVLCDba.GetVLCEnabledStatus(MachineCode));

            this.QueueId = objVLCDba.GetVLCQueueId(this.MachineCode);
            cardIdLabel.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegateForString(SetCardId), objGeneralDba.GetCardIdFromQueue(this.QueueId));
        }
       
        public bool AsynchReadSettingsForVLC()
        {



            try
            {

                BGOPCServerManagement.GetBGOPCServer(Window.GetWindow(this)).AddGroup(this.MachineCode + "_VLC_Group", true, 100, 1, new DataChangeEventHandler(bgGrp1_DataChanged), null, new OnBGSrvAddGroup(onAddGroup));

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
                string deckCode = getDeckCode(MachineCode);
                itms[0] = new OPCItemDef(this.MachineChannel + "." + deckCode + "." + OpcTags.VLC_North_Pallet_Present_Prox, true, 0, typeof(void));
                itms[1] = new OPCItemDef(this.MachineChannel + "." + deckCode + "." + OpcTags.VLC_South_Pallet_Present_Prox, true, 1, typeof(void));
                grp.AddItems(itms, null, new OnBGGrpAddItems(onAddItems));
            }
        }
        private void bgGrp1_DataChanged(object sender, DataChangeEventArgs e)
        {

            foreach (OPCItemState rslt in e.sts)
            {
                try
                {
                    if (rslt.Quality == 0)
                        continue;
                    if (rslt.HandleClient == 0)
                    {
                        bool resValue = bool.Parse(rslt.DataValue.ToString());
                        pallet.Dispatcher.BeginInvoke(new InvokeDelegate(SetPalletPresentNEStatus), resValue);

                    }
                    else if (rslt.HandleClient == 1)
                    {
                        bool resValue = bool.Parse(rslt.DataValue.ToString());
                        pallet.Dispatcher.BeginInvoke(new InvokeDelegate(SetPalletPresentSWStatus), resValue);
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

        }
        private int[] LastItemHandles;
        private void onAddItems(BGException ex, OPCItemResult[] rslt, int[] srvHandles, object tag)
        {
            if (ex == null)
            {
                LastItemHandles = srvHandles;
            }
        }


       

        bool GetPalletPresentStatusFromOpc()
        {
           
            if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
            bool status = false;
            string deckCode = getDeckCode(MachineCode);
            if (objOPCServerDirector.IsMachineQualityHealthy(MachineChannel + "." +
                deckCode + "." + OpcTags.VLC_North_Pallet_Present_Prox) == OPCDA.qualityBits.good)
            {
                status = objOPCServerDirector.ReadTag<bool>(MachineChannel + "." + deckCode + "." + OpcTags.VLC_North_Pallet_Present_Prox);
                status= status || objOPCServerDirector.ReadTag<bool>(MachineChannel + "." + deckCode + "." + OpcTags.VLC_South_Pallet_Present_Prox);
            }
            return status;
        }
        void SetPalletPresentStatus(bool status)
        {
            this.pallet.Visibility = status ? Visibility.Visible : Visibility.Hidden;
        }
        void SetPalletPresentNEStatus(bool status)
        {


            this.pallet.NE_Grid.Visibility = status ? Visibility.Visible : Visibility.Hidden;

        }
        void SetPalletPresentSWStatus(bool status)
        {


            this.pallet.SW_Grid.Visibility = status ? Visibility.Visible : Visibility.Hidden;

        }

        public string getDeckCode(string machineCode)
        {
            string deckCode = null;
            deckCode = machineCode.Replace("Drive", "Deck");

            //string[] cmParts = machineCode.Split('_');

            //remCode = "REM" + "_" + "FLR" + cmParts[1][cmParts[1].Length - 1] + "_" + cmParts[2];
            return deckCode;
        }
        void SetLockedStatus(bool status)
        {
            //this.vlcLock.Visibility = status ? Visibility.Visible : Visibility.Hidden;\
            if (status)
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
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
           // UserControl usr = sender as UserControl;
           // usr.BorderThickness = new Thickness(1, 1, 1, 1);
            ScaleMachine(1.5F);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            //UserControl usr = sender as UserControl;
           // usr.BorderThickness = new Thickness(0, 0, 0, 0);
            ScaleMachine(1F);
        }
        void ScaleMachine(float scaleParam)
        {
            TransformGroup tg = this.RenderTransform as TransformGroup;
            ScaleTransform rt = tg.Children[0] as ScaleTransform;
            rt.ScaleX = scaleParam;
            rt.ScaleY = scaleParam;
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            VLCPop objVLCPop = null;
            objVLCPop = new VLCPop();
            objVLCPop.MachineCode = this.MachineCode;
            objVLCPop.MachineChannel = this.MachineChannel;
            objVLCPop.Show();
        }

        private void UserControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReallocatePop objReallocatePop = null;
            objReallocatePop = new ReallocatePop();
            if (objVLCDba == null)
                objVLCDba = new VLCDba();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

            bool isBlocked = false;
            int requestType = -1;
            isBlocked = objVLCDba.GetVLCBlockedStatus(this.MachineCode);
            this.QueueId = objVLCDba.GetVLCQueueId(this.MachineCode);
            requestType = this.QueueId != 0 ? objGeneralDba.GetRequestType(this.QueueId) : -1;
            if (isBlocked && QueueId != 0 && (requestType == 1 || requestType == 0 || requestType == 5 || requestType == 6))
            {

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
