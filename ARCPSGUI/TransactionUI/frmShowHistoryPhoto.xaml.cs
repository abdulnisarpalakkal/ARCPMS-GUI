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
using ARCPSGUI.StaticGlobalClass;

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for frmShowHistoryPhoto.xaml
    /// </summary>
    public partial class frmShowHistoryPhoto : Window
    {
        public frmShowHistoryPhoto()
        {
            InitializeComponent();
        }

        public string EntryEESNorthPhotoPath
        {
            set
            {
                EntryEESImgNorth.Source = !string.IsNullOrEmpty(value) && File.Exists(GlobalData.eesImagePath + @"\" + value) ? new BitmapImage(new Uri(GlobalData.eesImagePath + @"\" + value)) : null; 
            }
        }
        public string EntryEESSouthPhotoPath
        {
            set
            {
                EntryEESImgSouth.Source = !string.IsNullOrEmpty(value) && File.Exists(GlobalData.eesImagePath + @"\" + value) ? new BitmapImage(new Uri(GlobalData.eesImagePath + @"\" + value)) : null; 
            }
        }

        public string ExitEESNorthPhotoPath
        {
            set
            {
                ExitEESImgNorth.Source = !string.IsNullOrEmpty(value) && File.Exists(GlobalData.eesImagePath + @"\" + value) ? new BitmapImage(new Uri(GlobalData.eesImagePath + @"\" + value)) : null; 
            }
        }
        public string ExitEESSouthPhotoPath
        {
            set
            {
                ExitEESImgSouth.Source = !string.IsNullOrEmpty(value) && File.Exists(GlobalData.eesImagePath + @"\" + value) ? new BitmapImage(new Uri(GlobalData.eesImagePath + @"\" + value)) : null; 
            }
        }
    }
}
