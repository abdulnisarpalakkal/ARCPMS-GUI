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
    /// Interaction logic for ucPST_new.xaml
    /// </summary>
    public partial class ucPST_new : UserControl
    {
        public string MachineCode { get; set; }
        public string MachineChannel { get; set; }

        BGGroup bgGrp1 = null;

        public delegate void InvokeDelegate(int status);
        public delegate void DisableInvokeDelegate(bool status);
        public event EventHandler OnPositionChanged;
        System.Timers.Timer timerToUpdateStatus = null;
        PSTDba objPSTDba = null;

        OPCServerDirector objOPCServerDirector = null;

        public ucPST_new()
        {
            InitializeComponent();
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objPSTDba == null)
                objPSTDba = new PSTDba();
            AsynchReadSettingsForPST();
            palletGrid.Dispatcher.BeginInvoke(DispatcherPriority.Background,
              new InvokeDelegate(SetPalletPresentStatus), GetPalletCountFromOpc());

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
            disableGrid.Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new DisableInvokeDelegate(SetDisableStatus), !objPSTDba.GetPSTEnabledStatus(MachineCode));

        }
       
        public bool AsynchReadSettingsForPST()
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
                OPCItemDef[] itms = new OPCItemDef[1];
                itms[0] = new OPCItemDef(this.MachineChannel + "." + this.MachineCode + "." + OpcTags.PST_Pallet_Count, true, 0, typeof(void));
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
                        palletGrid.Dispatcher.BeginInvoke(new InvokeDelegate(SetPalletPresentStatus), resValue);

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

      
        int GetPalletCountFromOpc()
        {
            if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
            int cnt = 0;
            try
            {
               
                cnt = objOPCServerDirector.ReadTag<Int16>(MachineChannel + "." + MachineCode + "." + OpcTags.PST_Pallet_Count);
            }
            catch (Exception errMsg)
            {
                //Console.WriteLine(errMsg.Message);
            }
            return cnt;
        }

        void SetPalletPresentStatus(int count)
        {

            
            this.pallet1.Visibility = count>=1 ? Visibility.Visible : Visibility.Hidden;
            this.pallet2.Visibility = count>=2 ? Visibility.Visible : Visibility.Hidden;
            this.pallet3.Visibility = count>=3 ? Visibility.Visible : Visibility.Hidden;
            this.pallet4.Visibility = count>=4 ? Visibility.Visible : Visibility.Hidden;
        }
        void SetDisableStatus(bool status)
        {
            this.disableGrid.Visibility = status ? Visibility.Visible : Visibility.Hidden;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            PSTPop objPSTPop = null;
            objPSTPop = new PSTPop();
            objPSTPop.MachineCode = this.MachineCode;
            objPSTPop.MachineChannel = this.MachineChannel;
            objPSTPop.Show();
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
