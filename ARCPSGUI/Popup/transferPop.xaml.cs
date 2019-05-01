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
using ARCPSGUI.ProcessManager;

namespace ARCPSGUI.Popup
{
    /// <summary>
    /// Interaction logic for transferPop.xaml
    /// </summary>
    public partial class transferPop : Window
    {
        public int FromFloor { get; set; }
        public int FromAisle { get; set; }
        public int FromRow { get; set; }
        SlotProcess objSlotProcess = null;
        public transferPop()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (objSlotProcess == null)
                objSlotProcess = new SlotProcess();
            try
            {
                objSlotProcess.DoTransferCar(FromFloor, FromAisle,
                          FromRow, int.Parse(toFloor.Text), int.Parse(toAisle.Text), int.Parse(toRow.Text));
            }
            catch(FormatException ex)
            {
                MessageBox.Show("Please enter valid inputs");
            }
        }
    }
}
