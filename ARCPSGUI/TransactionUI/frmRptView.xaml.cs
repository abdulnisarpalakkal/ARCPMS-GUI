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
using System.Data;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for frmRptView.xaml
    /// </summary>
    public partial class frmRptView : Window
    {
        ARCPSGUI.Controls.ucRptViewer rptview = new Controls.ucRptViewer();
        public frmRptView()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            hostRpt.Child = rptview;
        }

        public void LoadCurrentParksReport(DataSet ds)
        {
            rptview.LoadCurrentParksReport(ds);
        }
        public void LoadHistoryReport(DataSet ds)
        {
            rptview.LoadHistoryReport(ds);
        }
        public void LoadDelayedHistoryReport(DataSet ds)
        {
            rptview.LoadDelayedHistoryReport(ds);
        }
    }
}
