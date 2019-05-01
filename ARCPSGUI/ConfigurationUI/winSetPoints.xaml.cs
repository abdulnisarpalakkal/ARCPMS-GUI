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

namespace ARCPSGUI.ConfigurationUI
{
    /// <summary>
    /// Interaction logic for winSetPoints.xaml
    /// </summary>
    public partial class winSetPoints : Window
    {
        public winSetPoints()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            objSetPoints.triggerSetPointsUpdate += objSetPoints_triggerSetPointsUpdate;
        }


        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            objSetPoints.triggerSetPointsUpdate -= objSetPoints_triggerSetPointsUpdate;
        }

        void objSetPoints_triggerSetPointsUpdate(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
