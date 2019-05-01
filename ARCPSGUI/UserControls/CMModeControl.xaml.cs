using ARCPSGUI.DB;
using ARCPSGUI.Popup;
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

namespace ARCPSGUI.Controls
{
    /// <summary>
    /// Interaction logic for VLCModeControl.xaml
    /// </summary>
    public partial class CMModeControl : UserControl
    {

        CMDba objCMDba = null;
        public string MachineCode { get; set; }
        public int MachineMode { get; set; }
        LinearGradientBrush statusGradient = null;
        string gradiantBrush = null;
        public CMModeControl()
        {
            if (objCMDba == null)
                objCMDba = new CMDba();
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (statusGradient == null) 
                statusGradient = new LinearGradientBrush();
            try
            {
                SetMode();
                this.butMode.Content = "LCM" + this.MachineCode[this.MachineCode.Length - 1];

            }
            catch(  NullReferenceException ex)
            {

            }
        }
        public void SetMode()
        {
            MachineMode = objCMDba.GetCMMode(this.MachineCode);
            switch (MachineMode)
           {
               case 0:
                   gradiantBrush = "yellowGradiantBrush";
                   break;
               case 1:
                   gradiantBrush = "greenGradiantBrush";
                   break;
               case 2:
                   gradiantBrush = "redGradiantBrush";
                   break;
               default:
                   break;

           }
            statusGradient = (LinearGradientBrush)this.Resources[gradiantBrush];
            butMode.Background = statusGradient;
        }

        private void butMode_Click(object sender, RoutedEventArgs e)
        {
            CMModePop pop = new CMModePop();
            pop.MachineCode = this.MachineCode;
            pop.MachineMode = this.MachineMode;
            Point relativePoint=this.TransformToAncestor(Application.Current.MainWindow)
                              .Transform(new Point(0, 0));
            pop.Left = relativePoint.X;
            pop.Top = relativePoint.Y + this.Height + this.Height;
            if (pop.Left > (SystemParameters.PrimaryScreenWidth-pop.Width))
                pop.Left = SystemParameters.PrimaryScreenWidth - pop.Width;
            pop.ShowDialog();
            SetMode();
        }

     
    }
}
