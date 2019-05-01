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
using ARCPSGUI.MachineUI;
using ARCPSGUI.OPC;
using OPC;
using OPCDA.NET;
using System.Threading.Tasks;
using ARCPSGUI.ConfigurationUI;
using ARCPSGUI.TransactionUI;
using ARCPSGUI.Model;

namespace ARCPSGUI.DiagnosticScreens
{
    /// <summary>
    /// Interaction logic for ucParkingDiagnostic_new.xaml
    /// </summary>
    public partial class ucParkingDiagnostic_new : UserControl
    {
        frmHome g_frmHome = null;
        //OPCDA.NET.RefreshGroup uGrp;
        //int DAUpdateRate = 1;
        //OPCServerDirector objOPCServerDirector = null;
        public delegate void InvokeDelegate(string machineCode,int cmPos);
        public delegate void MachineInvokeDelegateCM(MachineUI.ucCM objCM,int cmPos);
        public delegate void MachineInvokeDelegateVLC(MachineUI.ucVLC_new objVLC, int cmPos);
        public delegate void MachineInvokeDelegatePS(MachineUI.ucPS_new objPS, int cmPos);
        
        public ucParkingDiagnostic_new(frmHome frmHome)
        {
            InitializeComponent();
            this.g_frmHome=frmHome;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeCMSettings();
            InitializeVLCSettings();
            InitializePSSettings();
            
        }
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            TerminateCMSettings();
            TerminateVLCSettings();
            TerminatePSSettings();
            //objOPCServerDirector = null;
        }
        //private void UserControl_Initialized()
        //{
        //    garrageGrid.Dispatcher.BeginInvoke(new MachineInvokeDelegate(InitializeCMSettings));
        //    garrageGrid.Dispatcher.BeginInvoke(new MachineInvokeDelegate(InitializeVLCSettings));
        //    garrageGrid.Dispatcher.BeginInvoke(new MachineInvokeDelegate(InitializePSSettings));
            
        //}
        void InitializeCMSettings()
        {
            try
            {
                foreach (MachineUI.ucCM objCM in FindVisualChildren<MachineUI.ucCM>(this))
                {

                    MachineUI.ucCM objCMClone = objCM;
                    Task.Factory.StartNew(new Action(() => TranslateCM(objCMClone)));

                    objCM.OnPositionChanged += new EventHandler(ucCM_OnPositionChanged);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
        }
        void InitializeVLCSettings()
        {
            try
            {
                foreach (MachineUI.ucVLC_new objVLC in FindVisualChildren<MachineUI.ucVLC_new>(this))
                {

                //    Task.Factory.StartNew(new Action(() =>
                //    {

                //        if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
                //        int pos = 0;
                //        if (objOPCServerDirector.IsMachineQualityHealthy(objVLC.MachineChannel + "." +
                //            objVLC.MachineCode + "." + OpcTags.VLC_At_Floor) == OPCDA.qualityBits.good)
                //            pos = objOPCServerDirector.ReadTag<Int16>(objVLC.MachineChannel + "." + objVLC.MachineCode + "." + OpcTags.VLC_At_Floor);
                //        //if (pos != 0)
                //        //    TranslateVLCPosition(objVLC, pos);
                //        if (pos != 0)
                //            objVLC.Dispatcher.BeginInvoke(new MachineInvokeDelegateVLC(TranslateVLCPosition), objVLC, pos);
                //    }));
                    objVLC.OnPositionChanged += new EventHandler(objVLC_OnPositionChanged);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
        }
        void InitializePSSettings()
        {
            try
            {
                foreach (MachineUI.ucPS_new objPS in FindVisualChildren<MachineUI.ucPS_new>(this))
                {


                //    //if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
                //    //int pos=0;
                //    //if (objOPCServerDirector.IsMachineQualityHealthy(objPS.MachineChannel + "." +
                //    //    objPS.MachineCode + "." + OpcTags.PS_Shuttle_Aisle_Position_for_L2) == OPCDA.qualityBits.good)
                //    //    pos = objOPCServerDirector.ReadTag<Int16>(objPS.MachineChannel + "." + objPS.MachineCode + "." + OpcTags.PS_Shuttle_Aisle_Position_for_L2);
                //    //if (pos != 0)
                //    //    TranslatePSPosition(objPS, pos);


                //    Task.Factory.StartNew(new Action(() =>
                //    {

                //        if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
                //        int pos = 0;
                //        if (objOPCServerDirector.IsMachineQualityHealthy(objPS.MachineChannel + "." +
                //      objPS.MachineCode + "." + OpcTags.PS_Shuttle_Aisle_Position_for_L2) == OPCDA.qualityBits.good)
                //            pos = objOPCServerDirector.ReadTag<Int16>(objPS.MachineChannel + "." + objPS.MachineCode + "." + OpcTags.PS_Shuttle_Aisle_Position_for_L2);

                //        if (pos != 0)
                //            objPS.Dispatcher.BeginInvoke(new MachineInvokeDelegatePS(TranslatePSPosition), objPS, pos);
                //    }));


                    objPS.OnPositionChanged += new EventHandler(ucPS_OnPositionChanged);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
            }
        }
        void TerminateCMSettings()
        {
            foreach (MachineUI.ucCM objCM in FindVisualChildren<MachineUI.ucCM>(this))
            {
                objCM.OnPositionChanged -= new EventHandler(ucCM_OnPositionChanged);
            }
        }
       
        void TerminateVLCSettings()
        {
            foreach (MachineUI.ucVLC_new objVLC in FindVisualChildren<MachineUI.ucVLC_new>(this))
            {
                objVLC.OnPositionChanged -= new EventHandler(objVLC_OnPositionChanged);
                
            }
        }
        void TerminatePSSettings()
        {
            foreach (MachineUI.ucPS_new objPS in FindVisualChildren<MachineUI.ucPS_new>(this))
            {
                objPS.OnPositionChanged -= new EventHandler(ucPS_OnPositionChanged);
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
      /// <summary>
      /// CM
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
        void ucCM_OnPositionChanged(object sender, EventArgs e)
        {


            CMData cm = (CMData)sender;
            garrageGrid.Dispatcher.BeginInvoke(new InvokeDelegate(SetCMPosition), cm.machineCode, cm.position);

        }
       
        void SetCMPosition(string cmCode, int position)
        {
            MachineUI.ucCM  objCM=GetCMObject(cmCode);
            TranslateCMPosition(objCM, position);

        }
        MachineUI.ucCM GetCMObject(string cmCode)
        {
            MachineUI.ucCM returnCM=null;
            foreach (MachineUI.ucCM objCM in FindVisualChildren<MachineUI.ucCM>(this))
            {
                if(objCM.MachineCode.Equals(cmCode))
                {
                    returnCM= objCM;
                }
            }
            return returnCM;
            
        }
        void TranslateCM(MachineUI.ucCM objCM)
        {
            //if (objOPCServerDirector == null) objOPCServerDirector = new OPCServerDirector();
            //int pos = 0;
            //if (objOPCServerDirector.IsMachineQualityHealthy(objCM.MachineChannel + "." +
            //    objCM.MachineCode + "." + OpcTags.CM_Position_for_L2) == OPCDA.qualityBits.good)
            //    pos = objOPCServerDirector.ReadTag<Int16>(objCM.MachineChannel + "." + objCM.MachineCode + "." + OpcTags.CM_Position_for_L2);
            //if (pos != 0)
            //{
            //    objCM.Dispatcher.BeginInvoke(new MachineInvokeDelegateCM(TranslateCMPosition), objCM, pos);
            //    //TranslateCMPosition(objCM, pos);
            //}
        }

        void TranslateCMPosition(MachineUI.ucCM objCM, int position)
        {
            float aspectRatio = 0;
            aspectRatio = Math.Abs(objCM.MinXValue - objCM.MaxXValue) / Math.Abs(objCM.MinValue - objCM.MaxValue);

            TransformGroup tg = objCM.RenderTransform as TransformGroup;
            TranslateTransform rt = tg.Children[3] as TranslateTransform;

         //   rt.X =  Math.Abs(position - objCM.MinValue) * aspectRatio;
            rt.X = (position - objCM.MinValue) * aspectRatio;
        }




        /// <summary>
        /// VLC
        /// </summary>
        /// <returns></returns>
        

        void objVLC_OnPositionChanged(object sender, EventArgs e)
        {
            VLCData vlc = (VLCData)sender;


            garrageGrid.Dispatcher.BeginInvoke(new InvokeDelegate(SetVLCPosition), vlc.machineCode, vlc.floor);
        }
        void SetVLCPosition(string vlcCode, int position)
        {
            MachineUI.ucVLC_new objVLC = GetVLCObject(vlcCode);
            TranslateVLCPosition(objVLC, position);

        }
        MachineUI.ucVLC_new GetVLCObject(string vlcCode)
        {
            MachineUI.ucVLC_new returnVLC = null;
            foreach (MachineUI.ucVLC_new objVLC in FindVisualChildren<MachineUI.ucVLC_new>(this))
            {
                if (objVLC.MachineCode.Equals(vlcCode))
                {
                    returnVLC = objVLC;
                }
            }
            return returnVLC;

        }
        void TranslateVLCPosition(MachineUI.ucVLC_new objVLC, int position)
        {
            float aspectRatio = 0;
            aspectRatio = GetAspectRatio(objVLC.MaxYValue, objVLC.MinYValue, objVLC.MaxValue, objVLC.MinValue);

            TransformGroup tg = objVLC.RenderTransform as TransformGroup;
            TranslateTransform rt = tg.Children[3] as TranslateTransform;

           
            rt.Y = (position - objVLC.MinValue) * aspectRatio;
        }
        float GetAspectRatio(int bigMax, int bigMin, int smallMax, int smallMin)
        {
            return (bigMax - bigMin) / (smallMax - smallMin);
        }



        /// <summary>
        /// PS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ucPS_OnPositionChanged(object sender, EventArgs e)
        {


            PSData ps = (PSData)sender;

            garrageGrid.Dispatcher.BeginInvoke(new InvokeDelegate(SetPSPosition), ps.machineCode, ps.position);

        }

        void SetPSPosition(string psCode, int position)
        {
            MachineUI.ucPS_new objPS = GetPSObject(psCode);
            TranslatePSPosition(objPS, position);

        }
        MachineUI.ucPS_new GetPSObject(string psCode)
        {
            MachineUI.ucPS_new returnCM = null;
            foreach (MachineUI.ucPS_new objPS in FindVisualChildren<MachineUI.ucPS_new>(this))
            {
                if (objPS.MachineCode.Equals(psCode))
                {
                    returnCM = objPS;
                }
            }
            return returnCM;

        }
        void TranslatePSPosition(MachineUI.ucPS_new objPS, int position)
        {
            float aspectRatio = 0;
            //aspectRatio = Math.Abs(objPS.MinXValue - objPS.MaxXValue) / Math.Abs(objCM.MinValue - objCM.MaxValue);
            aspectRatio=GetAspectRatio(objPS.MaxXValue, objPS.MinXValue, objPS.MaxValue, objPS.MinValue);
            TransformGroup tg = objPS.RenderTransform as TransformGroup;
            TranslateTransform rt = tg.Children[3] as TranslateTransform;
            rt.X = (position - objPS.MinValue) * aspectRatio;
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            winERPTasks objwinERPTasks = new winERPTasks();
            objwinERPTasks.Show();
        }

       

       

    }
}
