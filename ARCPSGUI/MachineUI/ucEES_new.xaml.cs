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

namespace ARCPSGUI.MachineUI
{
    /// <summary>
    /// Interaction logic for ucEES_new.xaml
    /// </summary>
    public partial class ucEES_new : UserControl
    {
       

        
          public string MachineCode { get; set; }
        public string MachineChannel { get; set; }
        public decimal QueueId { get; set; }

        BGGroup bgGrp1 = null;

        OPCServerDirector objOPCServerDirector = null;

        public delegate void InvokeDelegate(bool status);
         public delegate void InvokeDelegateForString(string cardId);
        public event EventHandler OnPositionChanged;

        EESDba objEESDba = null;
        GeneralDba objGeneralDba = null;
        System.Timers.Timer timerToUpdateStatus = null;
        public ucEES_new()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objEESDba == null)
                objEESDba = new EESDba();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            SetEESName();
            try
            {
                AsynchReadSettingsForEES();
                //pallet.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                //    new InvokeDelegate(SetPalletPresentStatus), GetPalletPresentStatusFromOpc());
            }
            catch (Exception errMsg)
            {

            }
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

        void timerToUpdateStatus_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.Background, new InvokeDelegate(SetLockedStatus), objEESDba.GetEESBlockedStatus(MachineCode));
            disableGrid.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegate(SetDisableStatus), !objEESDba.GetEESEnabledStatus(MachineCode));
            
            this.QueueId = objEESDba.GetEESQueueId(this.MachineCode);
            cardIdLabel.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegateForString(SetCardId), objGeneralDba.GetCardIdFromQueue(this.QueueId));
        
           
        }
        public bool AsynchReadSettingsForEES()
        {
           


            try
            {

                BGOPCServerManagement.GetBGOPCServer(Window.GetWindow(this)).AddGroup(this.MachineCode + "_EES_Group", true, 100, 1, new DataChangeEventHandler(bgGrp1_DataChanged), null, new OnBGSrvAddGroup(onAddGroup));

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
                itms[0] = new OPCItemDef(this.MachineChannel + "." + this.MachineCode + "." + OpcTags.EES_Pallet_Present_Prox_NE, true, 0, typeof(void));
                itms[1] = new OPCItemDef(this.MachineChannel + "." + this.MachineCode + "." + OpcTags.EES_Pallet_Present_Prox_SW, true, 1, typeof(void));
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
                        bool resValue = bool.Parse(rslt.DataValue.ToString());
                        pallet.Dispatcher.BeginInvoke(new InvokeDelegate(SetPalletPresentNEStatus), resValue);

                    }
                    else if (rslt.HandleClient == 1)
                    {
                        bool resValue = bool.Parse(rslt.DataValue.ToString());
                        pallet.Dispatcher.BeginInvoke(new InvokeDelegate(SetPalletPresentSWStatus), resValue);
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

       
        bool GetPalletPresentStatusFromOpc()
        {
            if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
            bool status = false;
            status=objOPCServerDirector.ReadTag<bool>(MachineChannel + "." + MachineCode + "." + OpcTags.EES_Pallet_Present_Prox_SW);
            return status || objOPCServerDirector.ReadTag<bool>(MachineChannel + "." + MachineCode + "." + OpcTags.EES_Pallet_Present_Prox_NE);
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
        void SetLockedStatus(bool status)
        {
            //this.eesLock.Visibility = status ? Visibility.Visible : Visibility.Hidden;\
            if (status)
            {
                this.BorderThickness = new Thickness(0.5);
            }
            else
            {
                this.BorderThickness = new Thickness(0);
            }
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
            //usr.BorderThickness = new Thickness(1, 1, 1, 1);
            ScaleMachine(1.1F);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
           // UserControl usr = sender as UserControl;
            //usr.BorderThickness = new Thickness(0, 0, 0, 0);
            ScaleMachine(1F);
        }
       private int GetEESNumber()
        {
            char cLastCharacter = MachineCode[MachineCode.Length - 1];
           return (int)Char.GetNumericValue(cLastCharacter);
        }
        private void SetEESName()
       {
           ees_name_label.Content = "EES" + GetEESNumber();
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
            EESPop objEESPop = null;
            objEESPop = new EESPop();
            objEESPop.MachineCode = this.MachineCode;
            objEESPop.MachineChannel = this.MachineChannel;
            objEESPop.Show();
        }

        private void UserControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReallocatePop objReallocatePop = null;
            objReallocatePop = new ReallocatePop();
            if (objEESDba == null)
                objEESDba = new EESDba();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

            bool isBlocked = false;
            int requestType = -1;
            isBlocked = objEESDba.GetEESBlockedStatus(this.MachineCode);
            this.QueueId = objEESDba.GetEESQueueId(this.MachineCode);
            requestType = this.QueueId != 0 ? objGeneralDba.GetRequestType(this.QueueId) : -1;
            if (isBlocked && QueueId != 0 && (requestType == 1 || requestType == 0 || requestType == 5 || requestType == 6)) //entry or exit
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
