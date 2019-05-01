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
    public partial class CMModePop : Window
    {

        public string MachineCode { get; set; }
        public int MachineMode { get; set; }

        CMDba objCMDba = null;
        public CMModePop()
        {
          
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           
            if (objCMDba == null)
                objCMDba = new CMDba();
            lblMachineName.Content = "LCM" + this.MachineCode[this.MachineCode.Length-1];

            SetCMMode(MachineMode);
           
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        void SetCMMode(int MachineMode)
        {
            switch(MachineMode)
            {
                case 0:
                    chkMix.IsChecked = true;
                    break;
                case 1:
                    chkEntry.IsChecked = true;
                    break;
                case 2:
                    chkExit.IsChecked = true;
                    break;
                default:
                    chkMix.IsChecked = true;
                    break;

            }
            
        }

        private void chkStatus_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            if (objCMDba == null)
                objCMDba = new CMDba();
            int newMode = 0;
            if (radio.Name.Equals(chkMix.Name))
            {
                newMode = 0;
               
            }
            else if (radio.Name.Equals(chkEntry.Name))
            {
                newMode = 1;
            }
            else if (radio.Name.Equals(chkExit.Name))
            {
                newMode = 2;
            }
            if (newMode == MachineMode)
                return;
            objCMDba.SetCMMode(this.MachineCode, newMode);
            this.Close();
        }

        
       
    }
}
