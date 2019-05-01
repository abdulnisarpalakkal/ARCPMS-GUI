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
using ARCPSGUI.Controls;
using ARCPSGUI.DB;
using ARCPSGUI.Popup;
using ARCPSGUI.Model;


namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for frmErpDetail.xaml
    /// </summary>
    public partial class frmErpDetail : Window
    {
        public event EventHandler OnDelete;
        public event EventHandler OnComplete;
        Connection objConnection = null;
        GeneralDba objGeneralDba = null;
        ERPDba objERPDba = null;


        int queueId;
        string mode;
        #region Setters and getters
        
        
        public int QueueId
        {
            get
            {
                return this.queueId;
            }
            set {
                this.queueId = value;
            }
        }

        public string  Mode
        {
            get
            {
                return this.mode;
            }
            set
            {
                this.mode = value;
            }
        }

        public string Name
        {
            set {
                txtName.Text = value;
            }
        }

        public string CustomerId
        {
            set
            {
                txtCustomerId.Text = value;
            }
        }

        public string Plate
        {
            set
            {
                txtPlate.Text = value;
            }
        }

        public string Type
        {
            set
            {
                txtType.Text = value;
            }
        }

        public string StartTime
        {
            set
            {
                txtStartTime.Text = value;
            }
        }

        public string CarWash
        {
            set
            {
                txtCarWash.Text = value;
            }
        }

        public string WashStatus
        {
            set
            {
                txtWashStatus.Text = value;
            }
        }

        public string Timer
        {
            set
            {
                txtTimer.Text = value;
            }
        }

        public string Rotation
        {
            set
            {
                txtRotation.Text = value;
            }
        }

        public string Gate { set; get; }
        public string Cmd { set; get; }
      

        #endregion

        public frmErpDetail()
        {
            InitializeComponent();
            if (objERPDba == null)
                objERPDba = new ERPDba();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            result = queueId + "," + mode;
            if (this.OnDelete != null) this.OnDelete(result, e);
        }

        private void btnComplete_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            result = queueId + "," + mode;
            if (this.OnComplete != null) this.OnComplete(result, e);
        }
        public void setTriggerStatus()
        {
            if(objConnection==null) objConnection=new Connection();
          
            bool isTriggerEnabled = objConnection.IsTriggerEnabledUsingQueueId(QueueId);
            if (isTriggerEnabled)
                btnTrigger.Visibility = Visibility.Visible;
            else
                btnTrigger.Visibility = Visibility.Hidden;

            
        }
        public void SetHoldStatus()
        {
            if (objGeneralDba == null) objGeneralDba = new GeneralDba();

            if (objGeneralDba.GetHoldFlagStatus(QueueId))
            {
                btnHold.Visibility = Visibility.Hidden;
                btnResume.Visibility = Visibility.Visible;
            }
            else
            {
                btnHold.Visibility = Visibility.Visible;
                btnResume.Visibility = Visibility.Hidden;
            }


           
        }

        private void btnTrigger_Click(object sender, RoutedEventArgs e)
        {
            if(objConnection==null) objConnection=new Connection();
            string machine = null;
            machine = objConnection.GetTriggerEnabledMachineUsingQueueId(QueueId);
            frmDiagnosticTrigger frmDiagTrigger = new frmDiagnosticTrigger();
            frmDiagTrigger.machineName = machine;

            frmDiagTrigger.Show();
            this.Close();


        }

        private void btnCancelCarwash_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnHold_Click(object sender, RoutedEventArgs e)
        {
            if (objERPDba == null) objERPDba = new ERPDba();
            objERPDba.SetHoldReqFlagStatus(QueueId, true);
            this.Close();
        }

        private void btnResume_Click(object sender, RoutedEventArgs e)
        {
            if (objERPDba == null) objERPDba = new ERPDba();
            objERPDba.SetHoldReqFlagStatus(QueueId, false);
            this.Close();
        }

        private void btnPark_Click(object sender, RoutedEventArgs e)
        {
            string result = "";
            
            if (!string.IsNullOrEmpty(Cmd) && (Cmd.Equals("Payment") || Cmd.Equals("Taking Photo")) && !string.IsNullOrEmpty(Gate))
            {
                  
                   if (MessageBox.Show("Do you want to return back this car with id   " + queueId + "?", "Return Car", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                   {
                       objERPDba.CompleteTransaction(queueId);
                       objERPDba.AbortTransaction(queueId);
                       QueueData objQueueData = new QueueData();
                       objQueueData.eesNumber = (int)Char.GetNumericValue(Gate[Gate.Length - 1]);
                       objQueueData.plateNumber = this.txtPlate.Text;
                       objQueueData.patronName = this.txtName.Text;
                       objQueueData.customerId = txtCustomerId.Text;
                       objQueueData.isRotate = txtRotation.Text.Equals("TRUE");
                       this.Close();


                       kioskDataForm objForm = new kioskDataForm(objQueueData.eesNumber);
                       objForm.SetRequestData(objQueueData);
                       
                       objForm.Show();

                   }


                   
                
            }
            else
            {
                MessageBox.Show("You can do this operation only when car reached in the gate in exit.");
            }
        }

       

    }
}
