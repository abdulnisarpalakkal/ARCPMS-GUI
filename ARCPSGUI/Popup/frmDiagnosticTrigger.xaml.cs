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
using Oracle.DataAccess.Client;
using System.Data;
using ARCPSGUI.DB;
using ARCPSGUI.OPC;

namespace ARCPSGUI.Popup
{
    /// <summary>
    /// Interaction logic for frmDiagnosticTrigger.xaml
    /// </summary>
    public partial class frmDiagnosticTrigger : Window
    {
        public string channel;
        public string machineName;
        public string errorTag;
        public string machineAliasName;

        public event EventHandler OnTriggered;

         ErrorDba objErrorDba = null;

        public frmDiagnosticTrigger()
        {
            InitializeComponent();
     
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OnLoad();
        }
        void OnLoad()
        {
            if (objErrorDba == null)
                objErrorDba = new ErrorDba();
            lblMachineName.Content = machineName;
            string machineCommand = objErrorDba.RetrieveTriggerDetails(machineName);
            lblMachineCommand.Content = machineCommand;
            lblMachineCommand.Tag = machineCommand;
            int pathStage = objErrorDba.RetrievePathStage(machineName);
            if (pathStage == 0)
            {
                optimizeCheck.IsEnabled = false;
                optimizeSlotCheck.IsEnabled = false;
            }

        }

        
       

        void OnTriggerClick()
        {
            try
            {
                string machineCommand = "";
                    Connection con = new Connection();
                    
                    con.UpdateErrorDetails(this.machineName,1);
                    if (optimizeCheck.IsChecked.Value || optimizeSlotCheck.IsChecked.Value)
                    {
                        con.UpdateOptimizePathStatus(this.machineName, optimizeCheck.IsChecked.Value ? 1 : 0, optimizeSlotCheck.IsChecked.Value ? 1 : 0);
                        optimizeCheck.IsChecked = false;
                        optimizeSlotCheck.IsChecked = false;
                    }
            }
            catch (Exception errMsg)
            {
            }
        }

        private void btnUnLock_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Please check commanded aisle and physical aisle are same", "Inform", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string machineCommand = "";
                machineCommand = Convert.ToString(lblMachineCommand.Tag);
               // UpdateLockStatus(machineName);
                Connection con = new Connection();
                con.UpdateErrorDetails(this.machineName, 2);
                if (optimizeCheck.IsChecked.Value || optimizeSlotCheck.IsChecked.Value)
                {
                    con.UpdateOptimizePathStatus(this.machineName, optimizeCheck.IsChecked.Value ? 1 : 0, optimizeSlotCheck.IsChecked.Value ? 1 : 0);
                    optimizeCheck.IsChecked = false;
                    optimizeSlotCheck.IsChecked = false;
                }
                if (OnTriggered != null) OnTriggered(machineCommand, new EventArgs());
                this.Close();
            }
        }
       
        private void btnTrigger_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you want to continue", "Inform", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                string machineCommand = "";
                machineCommand = Convert.ToString(lblMachineCommand.Tag);
                OnTriggerClick();
                if (OnTriggered != null) OnTriggered(machineCommand, new EventArgs());
                this.Close();
            }
        }

       
    }
}
