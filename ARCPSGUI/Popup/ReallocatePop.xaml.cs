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
    /// Interaction logic for ReallocatePop.xaml
    /// </summary>
    public partial class ReallocatePop : Window
    {
        public decimal QueueId { get; set; }
        public string MachineCode { get; set; }
        public string MachineChannel { get; set; }
        public string CardId { get; set; }
        public string CustomerName{ get; set; }
        public int RequestType { get; set; }

        CMDba objCMDba = null;
        VLCDba objVLCDba = null;
        EESDba objEESDba = null;
        GeneralDba objGeneralDba = null;
        public ReallocatePop()
        {
          
            InitializeComponent();
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

            RequestType=objGeneralDba.GetRequestType(QueueId);
            if (RequestType != 3 && RequestType != 1 && RequestType != 0)
                transfer_but.Visibility = Visibility.Hidden;
            if (RequestType == 3)
            {
                refresh_but.Visibility = Visibility.Hidden;
                restart_but.Visibility = Visibility.Hidden;
            }
            if (RequestType == 0)
            {
                retrive_but.Visibility = Visibility.Hidden;
            }
            lblMachineName.Content = this.MachineCode;


            CardId=objGeneralDba.GetCardIdFromQueue(QueueId);
            cardIdLabel.Content = CardId;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void refresh_but_Click(object sender, RoutedEventArgs e)
        {
             if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

            //3: for reallocation
             objGeneralDba.SetReallocateData(this.QueueId,this.MachineCode,3);
             this.Close();
        }
        private void restart_but_Click(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            MessageBoxResult messageBoxResult 
                    = System.Windows.MessageBox.Show("Are you sure?", "RESTART Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                //3: for reallocation
                objGeneralDba.SetReallocateData(this.QueueId, "SLOT", 3);
                this.Close();
            }
          

        }

        private void transfer_but_Click(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

            
         if(RequestType==3)
         {
             MessageBoxResult messageBoxResult
                     = System.Windows.MessageBox.Show("Do you want to transfer?", "Transfer Confirmation", System.Windows.MessageBoxButton.YesNo);
             if (messageBoxResult == MessageBoxResult.Yes)
             {
                 //4: for transfer
                 objGeneralDba.SetReallocateData(this.QueueId, this.MachineCode, 4);
                 this.Close();
             }
         }
         else
         {
             ChangeTotransferPop objPop = new ChangeTotransferPop();
             objPop.MachineCode = this.MachineCode;
             objPop.QueueId = this.QueueId;
             objPop.ShowDialog();
             
             this.Close();
         }

              
            
        }

        private void retrive_but_Click(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            if (RequestType != 0)
            {
                MessageBoxResult messageBoxResult
                        = System.Windows.MessageBox.Show("Do you want to Retrieve?", "Retrieve Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    //4: for transfer
                    objGeneralDba.InsertQueue(this.CardId, 0);
                    this.Close();
                }
            }
            
        }
     
       
    }
}
