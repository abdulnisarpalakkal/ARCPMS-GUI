using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ARCPSGUI.DB;
using ARCPSGUI.FloorUI.floorMachines;
using ARCPSGUI.Model;
using ARCPSGUI.OPC;
using ARCPSGUI.FloorUI.Service;

namespace ARCPSGUI.FloorUI
{
    /// <summary>
    /// Interaction logic for ucFloor1.xaml
    /// </summary>
    public partial class ucFloor6 : UserControl, FloorUIService
    {
        public int Floor { get; set; }
        SlotDba objSlotDba = null;
        GeneralDba objGeneralDba = null;
        //OPCServerDirector objOPCServerDirector = null;
        public ucFloor6()
        {
            InitializeComponent();
            Floor = 6;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            InitialUpdateAllSlots();
            objSlotDba.triggerSlotUpdate += new EventHandler(Handle_triggerSlotUpdate);
            objGeneralDba.UnRegisterDBNotification();
            objSlotDba.RegisterSlotDetailsWrtFloorNotification(Floor);

            InitializeCMSettings();
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            objSlotDba.triggerSlotUpdate -= new EventHandler(Handle_triggerSlotUpdate);
            objGeneralDba.UnRegisterDBNotification();
            TerminateCMSettings();
        }
        public void Handle_triggerSlotUpdate(object sender, EventArgs e)
        {
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
            SlotData objSlotData = (SlotData)sender;
            if (objSlotData.SlotFloor == Floor)
            {
                CarData objCarData = objSlotDba.GetSlotCarDetails(objSlotData.slotValue);
                objSlotData.ObjCarData = objCarData;
                slotPanel.Dispatcher.BeginInvoke(new Action(() =>
                {
                    UpdateSlotInScreen(objSlotData);
                }));
            }

        }
        public void InitialUpdateAllSlots()
        {
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
            List<SlotData> slotDataList = objSlotDba.GetSlotDetailsWrtFloor(Floor);
            foreach (SlotData objSlotData in slotDataList)
            {
                UpdateSlotInScreen(objSlotData);
            }
        }
        public void UpdateSlotInScreen(SlotData objSlotData)
        {
            IEnumerable<ucSlot> slotList = null;
            if (objSlotData.SlotRow == 1)
            {
                slotList = row1Panel.Children.OfType<ucSlot>();

            }
            else if (objSlotData.SlotRow == 2)
            {
                slotList = row2Panel.Children.OfType<ucSlot>();
            }
            else if (objSlotData.SlotRow == 3)
            {
                slotList = row3Panel.Children.OfType<ucSlot>();
            }
            else if (objSlotData.SlotRow == 4)
            {
                slotList = row4Panel.Children.OfType<ucSlot>();
            }
            else if (objSlotData.SlotRow == 6)
            {
                slotList = row6Panel.Children.OfType<ucSlot>();
            }

            ucSlot floorSlot = slotList.Where(a => a.Aisle == objSlotData.SlotAisle && a.Row == objSlotData.SlotRow).First()
                 as ucSlot;

            if (floorSlot != null)
            {
                floorSlot.InitializeSlotData(objSlotData);


            }
        }

        /// <summary>
        /// CM Manipulation
        /// </summary>
        public void InitializeCMSettings()
        {
            UCM1.lblMachineNumber.Content = "1";
            //Task.Factory.StartNew(new Action(() => TranslateCM(UCM1)));
            UCM1.OnPositionChanged += new EventHandler(ucCM_OnPositionChanged);

            UCM2.lblMachineNumber.Content = "2";
            //Task.Factory.StartNew(new Action(() => TranslateCM(UCM2)));
            UCM2.OnPositionChanged += new EventHandler(ucCM_OnPositionChanged);

            UCM3.lblMachineNumber.Content = "3";
            //Task.Factory.StartNew(new Action(() => TranslateCM(UCM3)));
            UCM3.OnPositionChanged += new EventHandler(ucCM_OnPositionChanged);
            
          
        }
        public void TerminateCMSettings()
        {
           
            UCM1.OnPositionChanged -= new EventHandler(ucCM_OnPositionChanged);

            UCM2.OnPositionChanged -= new EventHandler(ucCM_OnPositionChanged);

          
            UCM3.OnPositionChanged -= new EventHandler(ucCM_OnPositionChanged);


        }
        public void ucCM_OnPositionChanged(object sender, EventArgs e)
        {
            CMData cm = (CMData)sender;
            cmPanel.Dispatcher.BeginInvoke(new Action(() =>
            {
                SetCMPosition(cm.machineCode, cm.position);
            }));
        }
        public void SetCMPosition(string cmCode, int position)
        {
            ucFloorCM objCM = GetCMObject(cmCode);
            TranslateCMPosition(objCM, position);

        }
        public  ucFloorCM GetCMObject(string cmCode)
        {
            

            ucFloorCM returnCM = null;
            IEnumerable<ucFloorCM> cmList = null;

            cmList = cmPanel.Children.OfType<ucFloorCM>();
            returnCM = cmList.Where(a => a.MachineCode == cmCode).First()
               as ucFloorCM;

            return returnCM;

        }
        //public  void TranslateCM(ucFloorCM objCM)
        //{
        //    if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
        //    int pos = 0;
        //    if (objOPCServerDirector.IsMachineQualityHealthy(objCM.MachineChannel + "." +
        //        objCM.MachineCode + "." + OpcTags.CM_Position_for_L2) == OPCDA.qualityBits.good)
        //        pos = objOPCServerDirector.ReadTag<Int16>(objCM.MachineChannel + "." + objCM.MachineCode + "." + OpcTags.CM_Position_for_L2);
        //    if (pos != 0)
        //    {
        //        objCM.Dispatcher.BeginInvoke(new Action(() =>
        //        {
        //            TranslateCMPosition(objCM, pos);
        //        }));
                
        //    }
        //}

        public void TranslateCMPosition(ucFloorCM objCM, int position)
        {
            float aspectRatio = 0;
            aspectRatio = Math.Abs(objCM.MinXValue - objCM.MaxXValue) / Math.Abs(objCM.MinValue - objCM.MaxValue);

            TransformGroup tg = objCM.RenderTransform as TransformGroup;
            TranslateTransform rt = tg.Children[3] as TranslateTransform;

            //   rt.X =  Math.Abs(position - objCM.MinValue) * aspectRatio;
            rt.X = (position - objCM.MinValue) * aspectRatio;
        }
    }
}
