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
    public partial class CMPop : Window
    {

        public string MachineCode { get; set; }
        public string MachineChannel { get; set; }

        CMDba objCMDba = null;
        GeneralDba objGeneralDba = null;
        public CMPop()
        {
          
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            if (objCMDba == null)
                objCMDba = new CMDba();
            lblMachineName.Content = this.MachineCode;
            
            SetTriggerStatus(objGeneralDba.GetMachineTriggerStatus(this.MachineCode));
            SetEnableStatus(objCMDba.GetCMEnabledStatus(this.MachineCode));
            if (this.MachineCode.Contains("LCM"))
                SetRotatoinStatusToView();
            else
                chkRotation.Visibility = Visibility.Hidden;
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
            if (objCMDba == null)
                objCMDba = new CMDba();
            if(radio.Name.Equals(chkEnable.Name))
            {
                objCMDba.SetCMEnabledStatus(this.MachineCode,true);
            }
            else if (radio.Name.Equals(chkDisable.Name))
            {
                objCMDba.SetCMEnabledStatus(this.MachineCode, false);
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
            objGeneralDba.SetTriggerStatus(this.MachineCode, false,1 );
            this.Close();
        }

        private void unlock_trigger_Click(object sender, RoutedEventArgs e)
        {
            objGeneralDba.SetTriggerStatus(this.MachineCode,false,2);
            this.Close();
        }

       

        void SetRotatoinStatusToView()
        {

            this.chkRotation.IsChecked = objCMDba.GetCMRotationStatus(this.MachineCode);
            
        }

        private void chkRotation_Checked(object sender, RoutedEventArgs e)
        {
            
            if (objCMDba == null)
                objCMDba = new CMDba();
            objCMDba.SetCMRotationStatus(this.MachineCode, true);
            SetRotatoinStatusToView();
           
        }

        private void chkRotation_Unchecked(object sender, RoutedEventArgs e)
        {
            if (objCMDba == null)
                objCMDba = new CMDba();
            objCMDba.SetCMRotationStatus(this.MachineCode, false);
            SetRotatoinStatusToView();
            
        }

       
    }
}
