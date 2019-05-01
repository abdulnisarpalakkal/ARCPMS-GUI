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

namespace ARCPSGUI.Popup
{
    /// <summary>
    /// Interaction logic for PMSAbortPop.xaml
    /// </summary>
    public partial class PMSAbortPop : Window
    {
        GeneralDba objGeneralDba = null;
        public PMSAbortPop()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            decimal transId = 0;
            if (Decimal.TryParse(transIdText.Text, out transId))
            {
                if (objGeneralDba.CompletePMSTask(transId))
                {
                    MessageBox.Show("Transaction completed successfully ");
                    this.Close();
                }
                else
                    MessageBox.Show("Error in completing transaction");
            }
        }
    }
}
