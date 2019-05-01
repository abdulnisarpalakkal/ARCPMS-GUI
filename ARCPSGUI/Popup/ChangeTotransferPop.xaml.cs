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
using ARCPSGUI.DB;

namespace ARCPSGUI.Popup
{
    /// <summary>
    /// Interaction logic for transferPop.xaml
    /// </summary>
    public partial class ChangeTotransferPop : Window
    {
     
        GeneralDba objGeneralDba = null;
        public decimal QueueId { set; get; }
        public string MachineCode { set; get; }
        public ChangeTotransferPop()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            try
            {
                string retMsg = objGeneralDba.SaveChangeToTransferSlot(QueueId, int.Parse(toFloor.Text), int.Parse(toAisle.Text), int.Parse(toRow.Text));
                if (retMsg.Equals("Success"))
                {
                    objGeneralDba.SetReallocateData(QueueId, MachineCode, 4);
                    this.Close();
                }
                else
                    MessageBox.Show(retMsg);
            }
            catch(FormatException ex)
            {
                MessageBox.Show("Please enter valid inputs");
            }
        }
    }
}
