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

namespace ARCPSGUI.MachineUI
{
    /// <summary>
    /// Interaction logic for ucPS_new.xaml
    /// </summary>
    public partial class ucPS_new : UserControl
    {
        public string MachineCode { get; set; }
        public string MachineChannel { get; set; }

        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int MinXValue { get; set; }
        public int MaxXValue { get; set; }
        BGGroup bgGrp1 = null;

        public delegate void InvokeDelegate(bool status);
        public event EventHandler OnPositionChanged;

        OPCServerDirector objOPCServerDirector = null;

        GeneralDba objGeneralDba = null;
        PSDba objPSDba = null;
        System.Timers.Timer timerToUpdateStatus = null;

        

        public ucPS_new()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            if (objPSDba == null)
                objPSDba = new PSDba();
         
            if (!objPSDba.GetPSSwitchOffStatus(this.MachineCode))
            {
                AsynchReadSettingsForPS();
                //pallet.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                //  new InvokeDelegate(SetPalletPresentStatus), GetPalletPresentStatusFromOpc());
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
            disableGrid.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegate(SetDisableStatus), !objPSDba.GetPSEnabledStatus(MachineCode));
            switchOffRect.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new InvokeDelegate(ShowSwitchOffStatus),  objPSDba.GetPSSwitchOffStatus(this.MachineCode));
           
          
        }
        public bool AsynchReadSettingsForPS()
        {
           

            try
            {

                BGOPCServerManagement.GetBGOPCServer(Window.GetWindow(this)).AddGroup(this.MachineCode + "_PS_Group", true, 100, 1, new DataChangeEventHandler(bgGrp1_DataChanged), null, new OnBGSrvAddGroup(onAddGroup));

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
                itms[0] = new OPCItemDef(this.MachineChannel + "." + this.MachineCode + "." + OpcTags.PS_Shuttle_Aisle_Position_for_L2, true, 0, typeof(void));
                itms[1] = new OPCItemDef(this.MachineChannel + "." + this.MachineCode + "." + OpcTags.PS_PalletPresent, true, 1, typeof(void));
                grp.AddItems(itms, null, new OnBGGrpAddItems(onAddItems));
            }
        }
        private void bgGrp1_DataChanged(object sender, DataChangeEventArgs e)
        {
            try
            {
                foreach (OPCItemState rslt in e.sts)
                {
                    if (rslt.HandleClient == 0)
                    {
                        if (rslt.Quality == 0)
                            continue;
                        int resValue = int.Parse(rslt.DataValue.ToString());

                        if (resValue != 0)
                        {
                            PSData ps = new PSData();
                            ps.machineCode = this.MachineCode;
                            ps.position = resValue;
                            this.OnPositionChanged(ps, new EventArgs());
                        }


                    }
                    else if (rslt.HandleClient == 1)
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

       

        bool GetPalletPresentStatusFromOpc()
        {
            if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
            return objOPCServerDirector.ReadTag<bool>(MachineChannel + "." + MachineCode + "." + OpcTags.PS_PalletPresent);
        }
        void SetPalletPresentStatus(bool status)
        {
            this.pallet.Visibility = status ? Visibility.Visible : Visibility.Hidden;
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
            PSPop objPSPop = null;
            objPSPop = new PSPop();
            objPSPop.MachineCode = this.MachineCode;
            objPSPop.MachineChannel = this.MachineChannel;
            objPSPop.Show();
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
        /// <summary>
        /// Switch off
        /// </summary>
        /// 

        void ShowSwitchOffStatus(bool status)
        {
           
            this.switchOffRect.Visibility = status ? Visibility.Visible : Visibility.Hidden;
            
        }

      
    }
}
