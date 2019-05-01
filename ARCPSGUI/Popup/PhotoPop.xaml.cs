using ARCPSGUI.Model;
using ARCPSGUI.StaticGlobalClass;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for PhotoPop.xaml
    /// </summary>
    public partial class PhotoPop : Window
    {
        CarData objCardata { set; get; }
        public PhotoPop(CarData objCardata)
        {
            InitializeComponent();
            this.objCardata = objCardata;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            eesName.Text = objCardata.EntryGate;
           
            NortImg.Source = !string.IsNullOrEmpty(objCardata.EntryNorthImg) && File.Exists(GlobalData.eesImagePath + @"\" + objCardata.EntryNorthImg) ? new BitmapImage(new Uri(GlobalData.eesImagePath + @"\" + objCardata.EntryNorthImg)) : null;
            SouthImg.Source = !string.IsNullOrEmpty(objCardata.EntrySouthImg) && File.Exists(GlobalData.eesImagePath + @"\" + objCardata.EntrySouthImg) ? new BitmapImage(new Uri(GlobalData.eesImagePath + @"\" + objCardata.EntrySouthImg)) : null; 
            
        }

    }
}
