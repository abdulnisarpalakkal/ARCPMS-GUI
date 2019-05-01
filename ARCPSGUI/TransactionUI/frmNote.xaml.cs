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
using Oracle.DataAccess.Client;
using System.Data;
using ARCPSGUI.DB;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for frmNote.xaml
    /// </summary>
    public partial class frmNote : Window
    {
        int customerKey = 0;
        CustomerDba objCustomerDba = null;

        public int CustomerKey
        {
            set {

                customerKey = value;
            
            }
        }

     
        public frmNote()
        {
            InitializeComponent();
            if (objCustomerDba == null)
                objCustomerDba = new CustomerDba();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Do you want to save records?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    objCustomerDba.SaveNoteToParkHistory(txtNote.Text.Trim(), customerKey);
                    this.Close();
                }

            }
            catch (Exception errMsg)
            {
                MessageBox.Show(errMsg.Message);
            }
        }

        public void GetNotes()
        {
            try
            {

                txtNote.Text = objCustomerDba.GetNoteFromParkHistory(customerKey);
               
            }
            catch (Exception errMsg)
            { 
            
            }
        }
    }
}
