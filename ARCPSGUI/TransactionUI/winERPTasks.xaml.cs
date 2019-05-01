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
using System.Windows.Threading;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for winERPTasks.xaml
    /// </summary>
    /// 
    public partial class winERPTasks : Window
    {
        //double actualSize = 100;
        DispatcherTimer _resizeTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 0, 3000), IsEnabled = false };
        public winERPTasks()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //actualSize = panelGrid.ActualHeight;
            //_resizeTimer.Tick += _resizeTimer_Tick;
           // this.erpUC.detach_erp.Visibility = Visibility.Hidden;
           
        }
        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
           // _resizeTimer.Tick -= _resizeTimer_Tick;
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _resizeTimer.IsEnabled = true;
            _resizeTimer.Stop();
            _resizeTimer.Start();
           // actualSize = 15;
         
        }

        private void panelGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
           
        }
        void _resizeTimer_Tick(object sender, EventArgs e)
        {
            
            _resizeTimer.IsEnabled = false;
          
            //Vector xy = erpUC.EES1.PointToScreen(new Point(erpUC.EES1.ActualWidth, erpUC.EES1.ActualHeight)) - erpUC.EES1.PointToScreen(new Point(0, 0));
            //Console.WriteLine(xy);
          
            //double adjustHeight = (erpUC.EES1.Height / xy.Y) * erpUC.EES1.Height;
            //double adjustWidth = (erpUC.EES1.Width / xy.X) * erpUC.EES1.Width;

            //IEnumerable uiCol = erpUC.eesPanel.Children.OfType<Ellipse>();
            //foreach (Ellipse ui in uiCol)
            //{
            //    ui.Height = adjustHeight;
            //    ui.Width = adjustWidth;
            //}


            
        }

      
      

       
    }
}
