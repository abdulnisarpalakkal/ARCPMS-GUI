using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Threading.Tasks;

using System.Threading;
using ARCPSGUI.DB;
using ARCPSGUI.Model;
using ARCPSGUI.OPC;


namespace ARCPSGUI.Popup
{
    public partial class kioskDataForm : Form
    {
        int gateNumber = 0;
        GeneralDba objGeneralDba = null;
        OPCServerDirector objOPCServerDirector = null;
        EESDba objEESDba = null;
        public kioskDataForm(int gateNumber)
        {
            this.gateNumber = gateNumber;

            InitializeComponent();
            titleLabel.Text = "EES" + this.gateNumber;
           
        }

        public void SetRequestData(QueueData objQueueData)
        {
            cardIdText.Text = objQueueData.customerId;
            nameText.Text = objQueueData.patronName;
            plateText.Text = objQueueData.plateNumber;
            rotationCheck.Checked = objQueueData.isRotate;
        }

        private void submitBut_Click(object sender, EventArgs e)
        {
            QueueData.CAR_TYPE cartTypeEnum = GetCarType();
            QueueData objQueueData = new QueueData();
            objQueueData.customerId = cardIdText.Text;
            objQueueData.patronName = nameText.Text;
            objQueueData.plateNumber = plateText.Text;
            objQueueData.carType = cartTypeEnum;
            objQueueData.needWash = carwashCheck.Checked;
            objQueueData.isRotate = rotationCheck.Checked;
            Task.Factory.StartNew(() => InsertingRequest(objQueueData));
            this.Close();
        }
       
        void InsertingRequest(QueueData objQueueData)
        {
            if (objEESDba == null)
                objEESDba = new EESDba();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();

            QueueData objQueueData1 = new QueueData();
            string eesChannel = null;
            string eesCode = null;
            objEESDba.getEESParameters(gateNumber, out eesChannel, out eesCode);
            objQueueData1.eesNumber = gateNumber;
            objQueueData1.requestType = 1;
            objQueueData1.customerId = objQueueData.customerId;
            objQueueData1.patronName = objQueueData.patronName;
            objQueueData1.plateNumber = objQueueData.plateNumber;
            objQueueData1.needSimulation = objQueueData.needSimulation;
            objQueueData1.needWash = objQueueData.needWash;
            objQueueData1.isRotate = objQueueData.isRotate;
            objQueueData1.carType = objQueueData.carType;
            //if (IsHighCar(eesChannel, eesCode))
            //{
            //    objQueueData1.carType = kioskSimulation.QueueData.CAR_TYPE.high;
            //}
            //else
            //{
            //    objQueueData1.carType = objQueueData.carType;
            //}

            objGeneralDba.InsertQueueForSimulation(objQueueData1);
        }


        private QueueData.CAR_TYPE GetCarType()
        {
            QueueData.CAR_TYPE enumCarType = QueueData.CAR_TYPE.high;
            if (lowRadio.Checked)
            {
                enumCarType = QueueData.CAR_TYPE.low;
            }
            else if (mediumRadio.Checked)
            {
                enumCarType = QueueData.CAR_TYPE.medium;
            }
            return enumCarType;
        }
        public bool IsHighCar(string eesChannel, string eesCode)
        {
            bool isHighCar = true;
            if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
            try
            {
                    //for low car is true, for high car its false.
                isHighCar = !objOPCServerDirector.ReadTag<bool>(eesChannel+"."+ eesCode+"."+"Lower_Height_Sensor_Blocked");
               
            }
            finally
            {

            }

            return isHighCar;
        }

        private void checkBut_Click(object sender, EventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            if(!string.IsNullOrEmpty(cardIdText.Text))
            {
                MemberData objMemberData = objGeneralDba.GetMemberDetailsUsingCardId(cardIdText.Text);
                nameText.Text = objMemberData.memberName;
                plateText.Text = objMemberData.PlateNo;
            }
        }

        private void kioskDataForm_Load(object sender, EventArgs e)
        {
            lowRadio.Checked = true;
        }
                

         
    }
}
