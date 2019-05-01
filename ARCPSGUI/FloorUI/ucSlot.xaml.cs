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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ARCPSGUI.Model;
using ARCPSGUI.Popup;
using ARCPSGUI.ProcessManager;

namespace ARCPSGUI.FloorUI
{
    /// <summary>
    /// Interaction logic for ucSlot.xaml
    /// </summary>
    public partial class ucSlot : UserControl
    {
        public SlotData slotData { get; set; }
        SlotProcess objSlotProcess = null;
        //public int Floor { get; set; }
        public int Aisle { get; set; }
        public int Row { get; set; }
        public ucSlot()
        {
            InitializeComponent();

        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            RotateSlotFace(Row);
        }
        public void RotateSlotFace(int row)
        {
            int angle = 180;

            if (row == 1 || row == 2)
            {

                TransformGroup tg = this.slotFrameGrid.RenderTransform as TransformGroup;
                RotateTransform rt = tg.Children[2] as RotateTransform;
                rt.Angle = angle;
            }


        }
        public void InitializeSlotData(SlotData objSlotData)
        {
            slotData = objSlotData;
            this.SetDisableStatus(objSlotData.Disable);
            this.SetRehandleStatus(objSlotData.Rehandle);
            if (objSlotData.SlotStatus == 0)
            {

                this.HideCar();
                this.HidePallet();
                this.HideLock();
                
            }
            else if (objSlotData.SlotStatus == 2) // car
            {
                this.HidePallet();
                this.HideLock();

                if (objSlotData.ObjCarData != null)
                {
                    this.ShowCar(objSlotData.ObjCarData.CarType);
                    if (!objSlotData.ObjCarData.IsRotated)
                    {
                        this.RotateCar(objSlotData.ObjCarData.CarType, 180);
                    }
                    else
                    {
                        this.RotateCar(objSlotData.ObjCarData.CarType, 0);
                    }
                }
                else
                {
                    this.ShowCar(1);
                }
            }
            else if (objSlotData.SlotStatus == 3) //pallet
            {
                this.HideCar();
                this.HideLock();
                this.ShowPallet();
            }
            else if (objSlotData.SlotStatus == 1) // block
            {
                this.ShowLock();
                ShowSlotStatus(objSlotData.PrevSlotStatus);
            }
            if(objSlotData.SlotFloor==1 && objSlotData.SlotAisle==22 &&  objSlotData.SlotRow==2)
            {
                this.Background = Brushes.Aqua;
            }
            
           
        }
        public void ShowSlotStatus(int slotStatus)
        {
            if (slotStatus == 0)
            {

                this.HideCar();
                this.HidePallet();
                

            }
            else if (slotStatus == 2) // car
            {
                this.HidePallet();
               
                this.ShowCar(1);
               
            }
            else if (slotStatus == 3) //pallet
            {
                this.HideCar();
             
                this.ShowPallet();
            }
        }
        public void RotateCar(int carType,int angle)
        {
            TransformGroup tg = null;
            if(carType==1)
            {
                 tg = this.lowCar.RenderTransform as TransformGroup;
            }
            else if(carType==2)
            {
                tg = this.highCar.RenderTransform as TransformGroup;
            }
            else if(carType==3)
            {
                tg = this.midCar.RenderTransform as TransformGroup;
            }
            
            RotateTransform rt = tg.Children[2] as RotateTransform;
            rt.Angle = angle;
          
        
        }
       
        public void ShowCar(int CarType)
        {
            if(CarType==1)
            {
                lowCar.Visibility = Visibility.Visible;
                midCar.Visibility = Visibility.Hidden;
                highCar.Visibility = Visibility.Hidden;

            }
            else if (CarType == 2)
            {
                lowCar.Visibility = Visibility.Hidden;
                midCar.Visibility = Visibility.Hidden;
                highCar.Visibility = Visibility.Visible;

            }
            else if (CarType == 3)
            {
                lowCar.Visibility = Visibility.Hidden;
                midCar.Visibility = Visibility.Visible;
                highCar.Visibility = Visibility.Hidden;

            }
        }
        public void HideCar()
        {
            lowCar.Visibility = Visibility.Hidden;
            midCar.Visibility = Visibility.Hidden;
            highCar.Visibility = Visibility.Hidden;
        }
        public void SetDisableStatus(bool isDisabled)
        {
            disableFace.Visibility = isDisabled ? Visibility.Visible : Visibility.Hidden;
        }
        public void SetRehandleStatus(bool isRehandle)
        {
            rehandleFace.Visibility = isRehandle ? Visibility.Visible : Visibility.Hidden;
        }
        public void HidePallet()
        {
            this.pallet.Visibility = Visibility.Hidden;
        }
        public void ShowPallet()
        {
            this.pallet.Visibility = Visibility.Visible;
        }
        public void HideLock()
        {
            this.lockImg.Visibility = Visibility.Hidden;
        }
        public void ShowLock()
        {
            this.lockImg.Visibility = Visibility.Visible;
        }

        private void UserControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
           
           
           
             
            SlotPop objSlotPop = new SlotPop();
            objSlotPop.SetSlotData(slotData);
            objSlotPop.Show();

        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
           // System.Windows.Controls.Button btn = sender as System.Windows.Controls.Button;
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                   
                    var dataObj = new DataObject();
                    dataObj.SetData("DragSource", this.slotData);
                   
                        DragDrop.DoDragDrop(this,
                                    dataObj,
                                    DragDropEffects.Copy);

                }


            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UserControl_PreviewDrop(object sender, DragEventArgs e)
        {
            if (objSlotProcess == null)
                objSlotProcess = new SlotProcess();

           
                if (e.Data.GetDataPresent("DragSource"))
                {
                    SlotData dragSlotData = (SlotData)e.Data.GetData("DragSource");
                    objSlotProcess.DoTransferCar(dragSlotData.SlotFloor, dragSlotData.SlotAisle, 
                        dragSlotData.SlotRow, slotData.SlotFloor, slotData.SlotAisle, slotData.SlotRow);
                }
           
        }
        

    }
}
