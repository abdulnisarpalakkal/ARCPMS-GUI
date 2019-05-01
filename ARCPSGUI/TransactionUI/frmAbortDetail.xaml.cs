using ARCPSGUI.DB;
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

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for frmErpDetail.xaml
    /// </summary>
    public partial class frmAbortDetail : Window
    {
        public event EventHandler OnDelete;
        public event EventHandler OnComplete;

        int queueId;
        string mode;
        int modeInDigit;
        int typeInDigit;
        int carWashInDigit;
        int washStatusInDigit;
        int rotationInDigit;
        Connection objdbProcCon = null;
        CustomerDba objCustomerDba = null;
        SlotDba objSlotDba = null;

        public int CustomerPkId
        {
            get
            {
                return Convert.ToInt32(customerPkId.Text);
            }
            set
            {
                customerPkId.Text = Convert.ToString(value);
            }
        }
        public int TypeInDigit
        {
            get
            {
                int retType = 2;
                switch(Type)
                {
                    case "HIGH":
                        retType = 2;
                        break;
                    case "MID":
                        retType = 3;
                        break;
                    case "LOW":
                         retType = 1;
                         break;
                }
                return retType;
            }
           
        }
        public int CarWashInDigit
        {
            get
            {
                return CarWash == "YES" ? 1 : 0;
            }
           
        }
        public int WashStatusInDigit
        {
            get
            {
                //switch (WashStatus)
                //{
                //    case "IN QUEUE":
                //        return 0;
                //    case "IN PROCESS":
                //        return 2;
                //    case "COMPLETED":
                //        return 1;
                //    default:
                //        return 1;
                //}
                return CarWash == "COMPLETED" ? 1 : 0;
            }
            
        }

        public int RotationInDigit
        {
            get
            {
                return Rotation == "TRUE" ? 1 : 0;
            }
          
        }



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

       

        public string CustomerId
        {
            set
            {
                txtCustomerId.Text = value;
            }
            get
            {
                return txtCustomerId.Text;
            }
        }
        public string Name
        {
            set
            {
                txtName.Text = value;
            }
            get
            {
                return txtName.Text;
            }
        }
        public string Plate
        {
            set
            {
                txtPlate.Text = value;
            }
            get
            {
                return txtPlate.Text;
            }
        }

        public string Type
        {
            set
            {
                txtType.Text = value;
            }
            get
            {
                return txtType.Text;
            }
        }

        public string StartTime
        {
            set
            {
                txtStartTime.Text = value;
            }
            get
            {
                return txtStartTime.Text;
            }
        }

        public string CarWash
        {
            set
            {
                txtCarWash.Text = value;
            }
            get
            {
                return txtCarWash.Text;
            }
        }

        public string WashStatus
        {
            set
            {
                txtWashStatus.Text = value;
            }
            get
            {
                return txtWashStatus.Text;
            }
        }

       

        public string Rotation
        {
            set
            {
                txtRotation.Text = value;
            }
            get
            {
                return txtRotation.Text;
            }
        }
        public string Gate
        {
            set
            {
                txtGate.Text = value;
            }
            get
            {
                return txtGate.Text;
            }
        }
        public string Floor
        {
            set
            {
                txtFloor.Text = value;
            }
            get
            {
                return txtFloor.Text;
            }
        }
        public string Aisle
        {
            set
            {
                txtAisle.Text = value;
            }
            get
            {
                return txtAisle.Text;
            }
        }
        public string Row
        {
            set
            {
                txtRow.Text = value;
            }
            get
            {
                return txtRow.Text;
            }
        }
        

        public frmAbortDetail()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (objdbProcCon == null)
                objdbProcCon = new Connection();

            if (objCustomerDba == null)
                objCustomerDba = new CustomerDba();
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            saveCustomerDataToSlot();
        }
        public void saveCustomerDataToSlot()
        {
            

            string sql = "";

            //frmAbortDetail objfromAbortDetail = new frmAbortDetail();
           


            //Check car is already parked with this id.
            int cardIdExist = objdbProcCon.HasCardIdExistInParking(CustomerId);


            //If car is already parked with this id, then alert the user and exit this method without saving.
            if (cardIdExist == 1)
            {
                MessageBox.Show("Card ID = " + CustomerId + " has already parked", "Information", MessageBoxButton.OK);
                return;
            }
            Boolean isValidSlot = objdbProcCon.isVacantValidSlotForParking(Convert.ToInt32(Floor),
                       Convert.ToInt32(Aisle), Convert.ToInt32(Row),
                       Convert.ToInt32(TypeInDigit));
            if (!isValidSlot)
            {
                MessageBox.Show("Slot is not Valid", "Information", MessageBoxButton.OK);
                return;
            }
            Boolean isWashSlot = objdbProcCon.isWashingSlot(Convert.ToInt32(Floor),
                       Convert.ToInt32(Aisle), Convert.ToInt32(Row));
            if (isWashSlot)
            {
                MessageBox.Show("Slot is already selected for car wash", "Information", MessageBoxButton.OK);
                return;
            }



            cardIdExist = 0;

            //Check parking request already exist for this cardId.
            cardIdExist = new Connection().HasCarDetailsExist(CustomerId);


            // DateTime startTime = Convert.ToDateTime(DateTime.Now);
            string startTimeString = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");



            objCustomerDba.RevertRetrievedStatus(CustomerPkId, startTimeString);

            objSlotDba.updateCarDataToSlot(int.Parse(Floor), int.Parse(Aisle), int.Parse(Row), CustomerId);

            MessageBox.Show("Saved  ", "Information", MessageBoxButton.OK);
        }

        

    }
}
