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
    /// Interaction logic for ucPVL_new.xaml
    /// </summary>
    public partial class ucPVL_new : UserControl
    {
       
          public string MachineCode { get; set; }
        public string MachineChannel { get; set; }

        BGGroup bgGrp1 = null;

        public delegate void InvokeDelegate(bool status);
        public event EventHandler OnPositionChanged;

        OPCServerDirector objOPCServerDirector = null;

        GeneralDba objGeneralDba = null;
        PVLDba objPVLDba = null;
        System.Timers.Timer timerToUpdateStatus = null;
        public ucPVL_new()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            if (objPVLDba == null)
                objPVLDba = new PVLDba();
            try
            {
                AsynchReadSettingsForPVL();
                pallet.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                   new InvokeDelegate(SetPalletPresentStatus), GetPalletPresentStatusFromOpc());
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
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (bgGrp1 != null)
            {
                bgGrp1.Dispose();
                bgGrp1 = null;
            }
            this.timerToUpdateStatus.Stop();
            this.timerToUpdateStatus.Dispose();
        }
        void timerToUpdateStatus_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            triggerStatus.Dispatcher.BeginInvoke(DispatcherPriority.Background, new InvokeDelegate(SetTriggerStatus), objGeneralDba.GetMachineTriggerStatus(MachineCode));
            disableGrid.Dispatcher.BeginInvoke(DispatcherPriority.Background, new InvokeDelegate(SetDisableStatus), !objPVLDba.GetPVLEnabledStatus(MachineCode));

        }
       
        public bool AsynchReadSettingsForPVL()
        {
            try
            {

                BGOPCServerManagement.GetBGOPCServer(Window.GetWindow(this)).AddGroup(this.MachineCode + "_PVL_Group", true, 100, 1, new DataChangeEventHandler(bgGrp1_DataChanged), null, new OnBGSrvAddGroup(onAddGroup));

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
                OPCItemDef[] itms = new OPCItemDef[1];
                string deckCode = getDeckCode(this.MachineCode);
                itms[0] = new OPCItemDef(this.MachineChannel + "." + deckCode + "." + OpcTags.PVL_Deck_Pallet_Present, true, 0, typeof(void));
                
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
                        pallet.Dispatcher.BeginInvoke(new InvokeDelegate(SetPalletPresentStatus), resValue);

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


        public void AsynchReadListenerForPVL(object sender, OPCDA.NET.RefreshEventArguments arg)
        {
            OPCDA.NET.OPCItemState res = arg.items[0].OpcIRslt;
            
            try
            {
                if (arg.Reason == OPCDA.NET.RefreshEventReason.DataChanged)
                {            // data changes
                    if (HRESULTS.Succeeded(res.Error))
                    {

                        OPCDA.NET.ItemDef opcItemDef = (OPCDA.NET.ItemDef)arg.items.GetValue(0);

                        string[] iterateItemName = opcItemDef.OpcIDef.ItemID.Split(new Char[] { '.' });

                        if (iterateItemName.Length == 3)
                        {
                            // string machineCode = iterateItemName[1].ToString();
                            string command = iterateItemName[2].ToString();
                            if (command.Equals(OpcTags.PVL_Deck_Pallet_Present))
                            {
                                bool resValue = bool.Parse(res.DataValue.ToString());
                                pallet.Dispatcher.BeginInvoke(new InvokeDelegate(SetPalletPresentStatus), resValue);
                            }
                           


                        }
                    }
                }
            }
            catch (Exception errMsg)
            {
                Console.WriteLine(errMsg.Message);
            }
        }
        bool GetPalletPresentStatusFromOpc()
        {
            if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();

            string deckCode = getDeckCode(this.MachineCode);
            return objOPCServerDirector.ReadTag<bool>(MachineChannel + "." + MachineCode + "." + OpcTags.PVL_Deck_Pallet_Present);
        }

        void SetPalletPresentStatus(bool status)
        {


            this.pallet.Visibility = status ? Visibility.Visible : Visibility.Hidden;
           
        }
        public string getDeckCode(string machineCode)
        {
            string deckCode = null;
            deckCode = machineCode.Replace("Drive", "Deck");
           
            //string[] cmParts = machineCode.Split('_');

            //remCode = "REM" + "_" + "FLR" + cmParts[1][cmParts[1].Length - 1] + "_" + cmParts[2];
            return deckCode;
        }

        void SetTriggerStatus(bool status)
        {
            this.triggerStatus.Visibility = status ? Visibility.Visible : Visibility.Hidden;
        }
        void SetDisableStatus(bool status)
        {
            this.disableGrid.Visibility = status ? Visibility.Visible : Visibility.Hidden;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PVLPop objPVLPop = null;
            objPVLPop = new PVLPop();
            objPVLPop.MachineCode = this.MachineCode;
            objPVLPop.MachineChannel = this.MachineChannel;
            objPVLPop.Show();
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            UserControl usr = sender as UserControl;
            usr.BorderThickness = new Thickness(1, 1, 1, 1);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            UserControl usr = sender as UserControl;
            usr.BorderThickness = new Thickness(0, 0, 0, 0);
        }
       
    }
}
