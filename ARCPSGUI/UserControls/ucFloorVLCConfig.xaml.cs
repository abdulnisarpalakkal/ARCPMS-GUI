using ARCPSGUI.DB;
using ARCPSGUI.Model;
using ARCPSGUI.utility;
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

namespace ARCPSGUI.UserControls
{
    /// <summary>
    /// Interaction logic for ucCMWindowConfig.xaml
    /// </summary>
    public partial class ucFloorVLCConfig : UserControl
    {
       
        GeneralDba objGeneralDba = null;
        public int floor { get; set; }
        public ucFloorVLCConfig()
        {
            InitializeComponent();

        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            _floor.Text = "FLOOR "+floor;

            VLC_Drive_01.IsChecked = objGeneralDba.GetFloorVLCStatus(floor, VLC_Drive_01.Name);
            VLC_Drive_02.IsChecked = objGeneralDba.GetFloorVLCStatus(floor, VLC_Drive_02.Name);
            VLC_Drive_03.IsChecked = objGeneralDba.GetFloorVLCStatus(floor, VLC_Drive_03.Name);

            VLC_Drive_04.IsChecked = objGeneralDba.GetFloorVLCStatus(floor, VLC_Drive_04.Name);
            VLC_Drive_05.IsChecked = objGeneralDba.GetFloorVLCStatus(floor, VLC_Drive_05.Name);
            VLC_Drive_06.IsChecked = objGeneralDba.GetFloorVLCStatus(floor, VLC_Drive_06.Name);

            VLC_Drive_01.Checked += VLC_Drive_Checked;
            VLC_Drive_01.Unchecked += VLC_Drive_Unchecked;

            VLC_Drive_02.Checked += VLC_Drive_Checked;
            VLC_Drive_02.Unchecked += VLC_Drive_Unchecked;

            VLC_Drive_03.Checked += VLC_Drive_Checked;
            VLC_Drive_03.Unchecked += VLC_Drive_Unchecked;

            VLC_Drive_04.Checked += VLC_Drive_Checked;
            VLC_Drive_04.Unchecked += VLC_Drive_Unchecked;

            VLC_Drive_05.Checked += VLC_Drive_Checked;
            VLC_Drive_05.Unchecked += VLC_Drive_Unchecked;

            VLC_Drive_06.Checked += VLC_Drive_Checked;
            VLC_Drive_06.Unchecked += VLC_Drive_Unchecked;


        
        }

        

        private void VLC_Drive_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            objGeneralDba.UpdateFloorVLCStatus(floor,box.Name,true);
            box.IsChecked = objGeneralDba.GetFloorVLCStatus(floor, box.Name);
        }

        private void VLC_Drive_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            objGeneralDba.UpdateFloorVLCStatus(floor, box.Name, false);
            box.IsChecked = objGeneralDba.GetFloorVLCStatus(floor, box.Name);
        }

        
        
        
     
    }
}
