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
    /// Interaction logic for PVLPop.xaml
    /// </summary>
    public partial class PVLPop : Window
    {

        public string MachineCode { get; set; }
        public string MachineChannel { get; set; }

        PVLDba objPVLDba = null;
        GeneralDba objGeneralDba = null;
        public PVLPop()
        {
          
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            if (objPVLDba == null)
                objPVLDba = new PVLDba();
            lblMachineName.Content = this.MachineCode;
            SetTriggerStatus(objGeneralDba.GetMachineTriggerStatus(this.MachineCode));
            SetEnableStatus(objPVLDba.GetPVLEnabledStatus(this.MachineCode));
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
            if (objPVLDba == null)
                objPVLDba = new PVLDba();
            if(radio.Name.Equals(chkEnable.Name))
            {
                objPVLDba.SetPVLEnabledStatus(this.MachineCode, true);
            }
            else if (radio.Name.Equals(chkDisable.Name))
            {
                objPVLDba.SetPVLEnabledStatus(this.MachineCode, false);
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


       
    }
}
