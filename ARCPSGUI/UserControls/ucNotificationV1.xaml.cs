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

using ARCPSGUI.OPC;
using ARCPSGUI.DB;

namespace ARCPSGUI.Controls
{
    /// <summary>
    /// Interaction logic for ucNotificationV1.xaml
    /// </summary>
    public partial class ucNotificationV1 : UserControl
    {
       public string channel;
       public string machineName;
       public string tagName;
       public string errorCode;
       


        public event EventHandler OnDoubleClick = null;
        public string uniqueValue = "";
        public frmHome g_frmHome = null;
        private bool isAutoMode;
        public bool IsAutoMode
        {
            get { return isAutoMode; }
            set { isAutoMode = value;
           
            }
        }
        private bool isDiable;
        public bool IsDiable
        {
            get { return isDiable; }
            set { isDiable = value;
            
            }
        }

        public bool isWaitingForCommandDone;

        public int MachineType = 0; //1 -lcm/ucm, 2-vlc,3-ps,4-pst,5-pvl,6-ees

        System.Timers.Timer currentNotify = new System.Timers.Timer();

        private bool isError;
        public bool IsError
        {
            get { return isError; }
            set { 
                isError = value;
               
            }
        }

        public bool ShowGreenBorder
        {
            set {
                if (value)
                {
                    brd.Style = (Style)FindResource("bgMessage"); 
                }
            }
        }

        private bool isPowerOn;
        public bool IsPowerOn
        {
            get { return isPowerOn; }
            set
            {
                isPowerOn = value;
                if (value == true)
                {
                    lblAuto.IsEnabled = true;
                    txtbMessage.IsEnabled = true;
                    lblStatus.IsEnabled = true;
                    brd.IsEnabled = true;
                    //btn.IsEnabled = true;
                    brd.BorderBrush = Brushes.Red;

                    lblAuto.Foreground = Brushes.Black;
                    txtbMessage.Foreground = Brushes.DarkRed;

                    lblMachineName.Foreground = Brushes.Black;
                
                }
                else
                {
                    lblAuto.IsEnabled = false;
                    txtbMessage.IsEnabled = true;
                    lblStatus.IsEnabled = false;
                    brd.IsEnabled = false;
                   // btn.IsEnabled = false;
                    brd.BorderBrush = Brushes.Gray;
                   // lblStatus.Fill = Brushes.Gray;
                    lblAuto.Foreground = Brushes.Gray;
                    txtbMessage.Foreground = Brushes.DarkRed;
                    lblMachineName.Foreground = Brushes.Black;
                    //imgError1.Background = Brushes.Red;
                    //imgError2.Background = Brushes.Red;
                    // imgPowerOff.Source = MachineImages.ImgPowerOff;
                   // imgPowerOff.Fill = Brushes.Red;


                }
            }
        }

        public ucNotificationV1(bool isError = true)
        {
            InitializeComponent();

            channel="";
            machineName = "";
            tagName = "";
            errorCode = "";

            if(!isError)
                brd.Style = (Style)FindResource("brdGreenBorder"); 

            currentNotify.Elapsed += new System.Timers.ElapsedEventHandler(currentNotify_Elapsed);
            currentNotify.Interval = 1000;
        }

        void currentNotify_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.Visibility == System.Windows.Visibility.Visible)
                this.Dispatcher.BeginInvoke(new Action(() => this.Visibility = System.Windows.Visibility.Hidden));
            else
                this.Dispatcher.BeginInvoke(new Action(() => this.Visibility = System.Windows.Visibility.Visible));
        }

        public void HasBlinkingEffect()
        {
            currentNotify.Start();
        }
        
        public string MachineName
        {
            set
            {
                lblMachineName.Content = value;
            }
        }         

        public void ErrorMessage(string message)
        { 
            txtbMessage.Text = message;

        }

        public void StatusMessage(string message)
        {
           // lblStatus.Content = message;
        }

        public void AutoManualMessage(string message)
        {
            lblAuto.Content = message;
        }

        private void txtbMessage_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //if (e.ButtonState == MouseButtonState.Released && e.ChangedButton == MouseButton.Left)
            //{
            //    if (OnDoubleClick != null)
            //        OnDoubleClick(this.Name + "," + lblMachineName.Content, e);
            //}

            ////if (OnDoubleClick != null)
            ////    OnDoubleClick(this.Name, e);
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Released && e.ChangedButton == MouseButton.Left)
            {
                if (OnDoubleClick != null && ConfirmBeforeDelete())
                    OnDoubleClick(this.Name + "," + lblMachineName.Content, e);
            }
        }

        bool ConfirmBeforeDelete()
        {
            bool remove = false;
            try
            {
                bool autoMode = false;
                Int16 errorCode = 0;
                bool isInL2 = false;

                OPCServerDirector opcd = new OPCServerDirector();
               // if(machineName.Contains("LCM") || machineName.Contains("UCM"))
                    autoMode = opcd.ReadTag<bool>(channel + "." + machineName + "." + "Auto_Mode");

                if (machineName.Contains("LCM") || machineName.Contains("UCM") || machineName.Contains("PS_"))
                    errorCode = opcd.ReadTag<Int16>(channel + "." + machineName + "." + "L2_Error_Data_Register");
                else if (machineName.Contains("VLC") || machineName.Contains("PVL") || machineName.Contains("EES") || machineName.Contains("PST_"))
                    errorCode = opcd.ReadTag<Int16>(channel + "." + machineName + "." + "L2_ErrCode");

                Connection dbpm = new Connection();
                if (machineName.Contains("LCM") || machineName.Contains("UCM"))
                    isInL2 = dbpm.HasCMInL2Mode(machineName);
                else if (machineName.Contains("VLC"))
                    isInL2 = dbpm.HasVLCInL2Mode(machineName);
                else if (machineName.Contains("PS_"))
                    isInL2 = dbpm.HasPSInL2Mode(machineName);
                else if (machineName.Contains("EES"))
                    isInL2 = dbpm.HasEESTnL2Mode(machineName);
                else if (machineName.Contains("PVL"))
                    isInL2 = dbpm.HasPVLTnL2Mode (machineName);
                else if (machineName.Contains("PST_"))
                    isInL2 = dbpm.HASPSTnL2Mode (machineName);

                string message = "";
                message = autoMode == false ? "Not in auto mode." : message;
                message = errorCode != 0 ? " Error code = " + errorCode + ".": message;
                message = isInL2 == false ? " Not in L2 mode." : message;



                //if (!string.IsNullOrEmpty(message))
                //    if (!string.IsNullOrEmpty(message) &&
                //        MessageBox.Show(message + "Do you want to remove this notification.", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question)
                //          == MessageBoxResult.Yes)
                //        remove = true;

                if (MessageBox.Show(message + "Do you want to remove this notification.", "Information", MessageBoxButton.YesNo, MessageBoxImage.Question)
                           == MessageBoxResult.Yes)
                {

                    remove = true;
                }
            }
            finally { }
            return remove;
        }
    }
}
