using System;
using System.Collections;
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
using ARCPSGUI.Model;

namespace ARCPSGUI.Popup
{
    /// <summary>
    /// Interaction logic for SlotPop.xaml
    /// </summary>
    public partial class SlotPop : Window
    {
        SlotData objSlotData = null;
        SlotDba objSlotDba = null;
        int minSize = 160;
        int mazSize = 550;
        public SlotPop()
        {
            InitializeComponent();
        }
        public void SetSlotData(SlotData objSlotData)
        {
            this.objSlotData = objSlotData;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
            objSlotData = objSlotDba.GetSlotDetails(objSlotData.SlotFloor, objSlotData.SlotAisle, objSlotData.SlotRow);
            this.slot_floor_text.Text = objSlotData.SlotFloor.ToString();
            this.slot_aisle_text.Text = objSlotData.SlotAisle.ToString();
            this.slot_row_text.Text = objSlotData.SlotRow.ToString();
            if(objSlotData.SlotStatus==0)
            {
                normalRadio.IsChecked = true;

            }
            else if(objSlotData.SlotStatus==1)
            {

            }
            else if (objSlotData.SlotStatus == 2)
            {
                objSlotData.ObjCarData = objSlotDba.GetSlotCarDetails(objSlotData.slotValue);
               // this.ucCarData.SetCarData(objSlotData.ObjCarData);
                carRadio.IsChecked = true;
            }
            else if (objSlotData.SlotStatus == 3)
            {
                palletRadio.IsChecked = true;
            }
            disableCheck.IsChecked = objSlotData.Disable;
            rehandleCheck.IsChecked = objSlotData.Rehandle;
            this.ucCarData.retrieveEventHandler += ucCarData_retrieveEventHandler;
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            this.ucCarData.retrieveEventHandler -= ucCarData_retrieveEventHandler;
        }
        void ucCarData_retrieveEventHandler(object sender, EventArgs e)
        {
            this.Close();
        }
        private void carRadio_Checked(object sender, RoutedEventArgs e)
        {
            if(this.objSlotData.ObjCarData!=null)
            {
                this.ucCarData.SetCarData(objSlotData.ObjCarData);
                this.ucCarData.Disable = objSlotData.Disable;
            }
            this.Width = mazSize;
        }

        private void normalRadio_Checked(object sender, RoutedEventArgs e)
        {
            this.Width = minSize;
        }

        private void palletRadio_Checked(object sender, RoutedEventArgs e)
        {
            this.Width = minSize;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
            if (objSlotData.ObjCarData == null)
                objSlotData.ObjCarData = new CarData();
            if ((bool)normalRadio.IsChecked)
            {
                objSlotData.SlotStatus = 0;

            }
            else if ((bool)carRadio.IsChecked)
            {
                objSlotData.SlotStatus = 2;
                objSlotData.ObjCarData = this.ucCarData.GetCarData();
            }
            else if ((bool)palletRadio.IsChecked)
            {
                objSlotData.SlotStatus = 3;
            }
            objSlotData.Disable=(bool)disableCheck.IsChecked;
            objSlotData.Rehandle = (bool)rehandleCheck.IsChecked;
            Hashtable result=objSlotDba.UpdateSlot(objSlotData);
            if(int.Parse(result["result"].ToString())!=1)
            {
                MessageBox.Show(result["resultMsg"].ToString());

            }
            else
            {
                this.Close();
            }
          
        }

        private void trnansBut_Click(object sender, RoutedEventArgs e)
        {
            transferPop objTransferPop = new transferPop();
            objTransferPop.FromFloor = objSlotData.SlotFloor;
            objTransferPop.FromAisle = objSlotData.SlotAisle;
            objTransferPop.FromRow = objSlotData.SlotRow;
            objTransferPop.ShowDialog();
        }

    

      

       
    }
}
