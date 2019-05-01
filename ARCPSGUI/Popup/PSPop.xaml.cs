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
using ARCPSGUI.DB;

namespace ARCPSGUI.Popup
{
    /// <summary>
    /// Interaction logic for CMPop.xaml
    /// </summary>
    public partial class PSPop : Window
    {

        public string MachineCode { get; set; }
        public string MachineChannel { get; set; }

        PSDba objPSDba = null;
        GeneralDba objGeneralDba = null;
        public PSPop()
        {
          
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            if (objPSDba == null)
                objPSDba = new PSDba();
            lblMachineName.Content = this.MachineCode;
            SetTriggerStatus(objGeneralDba.GetMachineTriggerStatus(this.MachineCode));
            SetEnableStatus(objPSDba.GetPSEnabledStatus(this.MachineCode));
            ShowSwitchOffStatus();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }
      
        void SetEnableStatus(bool status)
        {
            this.chkEnable.IsChecked = status;
            this.chkDisable.IsChecked = !status;
        }

        private void chkStatus_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (objPSDba == null)
                objPSDba = new PSDba();
            if(radio.Name.Equals(chkEnable.Name))
            {
                objPSDba.SetPSEnabledStatus(this.MachineCode, true);
            }
            else if (radio.Name.Equals(chkDisable.Name))
            {
                objPSDba.SetPSEnabledStatus(this.MachineCode, false);
            }
        }

        /// <summary>
        /// Trigger
        /// </summary>
        /// <param name="status"></param>
        void SetTriggerStatus(bool status)
        {
            if (status)
            {
                this.triggerGrid.Visibility = status ? Visibility.Visible : Visibility.Hidden;
                this.machineCommand.Text = objGeneralDba.GetMachineTriggerCommand(this.MachineCode);
            }
            else
                this.triggerGrid.Visibility = Visibility.Hidden;
        }

        private void trigger_Click(object sender, RoutedEventArgs e)
        {
            objGeneralDba.SetTriggerStatus(this.MachineCode, false, 1);
            this.Close();
        }

        private void unlock_trigger_Click(object sender, RoutedEventArgs e)
        {
            objGeneralDba.SetTriggerStatus(this.MachineCode, false, 2);
            this.Close();
        }

        /// <summary>
        /// switch off 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void switchoffCheck_Checked(object sender, RoutedEventArgs e)
        {
            SetSwitchOffStatus(true);
            ShowSwitchOffStatus();
        }

        private void switchoffCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            SetSwitchOffStatus( false);
            ShowSwitchOffStatus();
        }

        void SetSwitchOffStatus(bool status)
        {
            objPSDba.SetPSSwitchOffStatus(this.MachineCode, status);
        }
        bool GetSwitchOffStatus()
        {
            return objPSDba.GetPSSwitchOffStatus(this.MachineCode);
        }
        void ShowSwitchOffStatus()
        {
            switchoffCheck.IsChecked = GetSwitchOffStatus();
        }
       
    }
}
