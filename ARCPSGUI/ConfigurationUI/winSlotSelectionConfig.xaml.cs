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
using ARCPSGUI.DB;
using ARCPSGUI.Model;
using ARCPSGUI.utility;

namespace ARCPSGUI.ConfigurationUI
{
    /// <summary>
    /// Interaction logic for ucSlotSelectionConfig.xaml
    /// </summary>
    public partial class winSlotSelectionConfig : Window
    {
        SlotDba objSlotDba = null;
       enum SELECTION_TYPE
       {
           DEFAULT=1,
           PEAK=2,
           CUSTOM=3
       }
        public winSlotSelectionConfig()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ShowCurrentSlotSelectionData();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {

        }
        private void customizeRadio_Checked(object sender, RoutedEventArgs e)
        {
            LoadCustomizedZoneData();
        }

        private void customizeRadio_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void peakHourRadio_Checked(object sender, RoutedEventArgs e)
        {
            LoadPeakHourZoneData();
        }

        private void peakHourRadio_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void defaultRadio_Checked(object sender, RoutedEventArgs e)
        {
            LoadDefaultZoneData();
        }

        private void defaultRadio_Unchecked(object sender, RoutedEventArgs e)
        {

        }
        private void LoadDefaultZoneData()
        {
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
            List<EESZoneData> zoneList = null;
            zoneList = objSlotDba.GetEESDefaultZoneList();
            PopulateZoneData(zoneList);
            expandNumberText.text1.Text = "6";
            expandZoneCheck.IsChecked = true;


            basementGroup.IsEnabled = false;
            nonBasementGroup.IsEnabled = false;
            expandGroup.IsEnabled = false;
            defaultRadio.IsChecked = true;
           

            

        }
        private void LoadPeakHourZoneData()
        {
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
            List<EESZoneData> zoneList = null;
            zoneList = objSlotDba.GetEESPeakHourZoneList();
            PopulateZoneData(zoneList);
           
            expandNumberText.text1.Text = "";
            expandZoneCheck.IsChecked = false;


            basementGroup.IsEnabled = false;
            nonBasementGroup.IsEnabled = false;
            expandGroup.IsEnabled = false;
            peakHourRadio.IsChecked = true;
            

        }
        private void LoadCustomizedZoneData()
        {
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
            List<EESZoneData> zoneList = null;
            zoneList = objSlotDba.GetEESCustomizedZoneList();
            PopulateZoneData(zoneList);

            expandNumberText.text1.Text = objSlotDba.GetZoneExpandNumber().ToString();
            expandZoneCheck.IsChecked = objSlotDba.GetZoneExpandStatus();


            basementGroup.IsEnabled = true;
            nonBasementGroup.IsEnabled = true;
            expandGroup.IsEnabled = true;
            customizeRadio.IsChecked = true;


        }
        private void PopulateZoneData(List<EESZoneData> zoneList)
        {
            foreach (EESZoneData objEESZoneData in zoneList)
            {
                switch (objEESZoneData.EESName)
                {
                    case "EES1":
                        EES1_zone_start.text1.Text = objEESZoneData.BaseZoneStart.ToString();
                        EES1_zone_end.text1.Text = objEESZoneData.BaseZoneEnd.ToString();
                        EES1_zone_start_nonbase.text1.Text = objEESZoneData.NonBaseZoneStart.ToString();
                        EES1_zone_end_nonbase.text1.Text = objEESZoneData.NonBaseZoneEnd.ToString();

                        EES1_base_ref_aisle.text1.Text = objEESZoneData.BaseRefAisle.ToString();
                        EES1_nonbase_ref_aisle.text1.Text = objEESZoneData.NonBaseRefAisle.ToString();
                        break;
                    case "EES2":
                        EES2_zone_start.text1.Text = objEESZoneData.BaseZoneStart.ToString();
                        EES2_zone_end.text1.Text = objEESZoneData.BaseZoneEnd.ToString();
                        EES2_zone_start_nonbase.text1.Text = objEESZoneData.NonBaseZoneStart.ToString();
                        EES2_zone_end_nonbase.text1.Text = objEESZoneData.NonBaseZoneEnd.ToString();

                        EES2_base_ref_aisle.text1.Text = objEESZoneData.BaseRefAisle.ToString();
                        EES2_nonbase_ref_aisle.text1.Text = objEESZoneData.NonBaseRefAisle.ToString();
                        break;
                    case "EES3":
                        EES3_zone_start.text1.Text = objEESZoneData.BaseZoneStart.ToString();
                        EES3_zone_end.text1.Text = objEESZoneData.BaseZoneEnd.ToString();
                        EES3_zone_start_nonbase.text1.Text = objEESZoneData.NonBaseZoneStart.ToString();
                        EES3_zone_end_nonbase.text1.Text = objEESZoneData.NonBaseZoneEnd.ToString();

                        EES3_base_ref_aisle.text1.Text = objEESZoneData.BaseRefAisle.ToString();
                        EES3_nonbase_ref_aisle.text1.Text = objEESZoneData.NonBaseRefAisle.ToString();
                        break;
                    case "EES4":
                        EES4_zone_start.text1.Text = objEESZoneData.BaseZoneStart.ToString();
                        EES4_zone_end.text1.Text = objEESZoneData.BaseZoneEnd.ToString();
                        EES4_zone_start_nonbase.text1.Text = objEESZoneData.NonBaseZoneStart.ToString();
                        EES4_zone_end_nonbase.text1.Text = objEESZoneData.NonBaseZoneEnd.ToString();

                        EES4_base_ref_aisle.text1.Text = objEESZoneData.BaseRefAisle.ToString();
                        EES4_nonbase_ref_aisle.text1.Text = objEESZoneData.NonBaseRefAisle.ToString();
                        break;
                    case "EES5":
                        EES5_zone_start.text1.Text = objEESZoneData.BaseZoneStart.ToString();
                        EES5_zone_end.text1.Text = objEESZoneData.BaseZoneEnd.ToString();
                        EES5_zone_start_nonbase.text1.Text = objEESZoneData.NonBaseZoneStart.ToString();
                        EES5_zone_end_nonbase.text1.Text = objEESZoneData.NonBaseZoneEnd.ToString();

                        EES5_base_ref_aisle.text1.Text = objEESZoneData.BaseRefAisle.ToString();
                        EES5_nonbase_ref_aisle.text1.Text = objEESZoneData.NonBaseRefAisle.ToString();
                        break;
                    case "EES6":
                        EES6_zone_start.text1.Text = objEESZoneData.BaseZoneStart.ToString();
                        EES6_zone_end.text1.Text = objEESZoneData.BaseZoneEnd.ToString();
                        EES6_zone_start_nonbase.text1.Text = objEESZoneData.NonBaseZoneStart.ToString();
                        EES6_zone_end_nonbase.text1.Text = objEESZoneData.NonBaseZoneEnd.ToString();

                        EES6_base_ref_aisle.text1.Text = objEESZoneData.BaseRefAisle.ToString();
                        EES6_nonbase_ref_aisle.text1.Text = objEESZoneData.NonBaseRefAisle.ToString();
                        break;
                    case "EES7":
                        EES7_zone_start.text1.Text = objEESZoneData.BaseZoneStart.ToString();
                        EES7_zone_end.text1.Text = objEESZoneData.BaseZoneEnd.ToString();
                        EES7_zone_start_nonbase.text1.Text = objEESZoneData.NonBaseZoneStart.ToString();
                        EES7_zone_end_nonbase.text1.Text = objEESZoneData.NonBaseZoneEnd.ToString();

                        EES7_base_ref_aisle.text1.Text = objEESZoneData.BaseRefAisle.ToString();
                        EES7_nonbase_ref_aisle.text1.Text = objEESZoneData.NonBaseRefAisle.ToString();
                        break;
                    case "EES8":
                        EES8_zone_start.text1.Text = objEESZoneData.BaseZoneStart.ToString();
                        EES8_zone_end.text1.Text = objEESZoneData.BaseZoneEnd.ToString();
                        EES8_zone_start_nonbase.text1.Text = objEESZoneData.NonBaseZoneStart.ToString();
                        EES8_zone_end_nonbase.text1.Text = objEESZoneData.NonBaseZoneEnd.ToString();

                        EES8_base_ref_aisle.text1.Text = objEESZoneData.BaseRefAisle.ToString();
                        EES8_nonbase_ref_aisle.text1.Text = objEESZoneData.NonBaseRefAisle.ToString();
                        break;
                    case "EES9":
                        EES9_zone_start.text1.Text = objEESZoneData.BaseZoneStart.ToString();
                        EES9_zone_end.text1.Text = objEESZoneData.BaseZoneEnd.ToString();
                        EES9_zone_start_nonbase.text1.Text = objEESZoneData.NonBaseZoneStart.ToString();
                        EES9_zone_end_nonbase.text1.Text = objEESZoneData.NonBaseZoneEnd.ToString();

                        EES9_base_ref_aisle.text1.Text = objEESZoneData.BaseRefAisle.ToString();
                        EES9_nonbase_ref_aisle.text1.Text = objEESZoneData.NonBaseRefAisle.ToString();
                        break;
                    default:
                        break;
                }
            }
        }

        private void EES_zone_start_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (objSlotDba == null)
                    objSlotDba = new SlotDba();
                string sourceName = null;
                string EESName = null;
                string textValue;
                int number = 0;
                LabelTextBox objTextBox = sender as LabelTextBox;

                sourceName = objTextBox.Name;
                EESName = sourceName.Split('_')[0];
                if (int.TryParse(objTextBox.text1.Text, out number))
                    objSlotDba.SetCustomizedBaseZoneStart(EESName, number);
                objTextBox.text1.Text = objSlotDba.GetCustomizedBaseZoneStart(EESName).ToString();
            }
        }

        private void EES_zone_end_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (objSlotDba == null)
                    objSlotDba = new SlotDba();
                string sourceName = null;
                string EESName = null;
                string textValue;
                int number = 0;
                LabelTextBox objTextBox = sender as LabelTextBox;

                sourceName = objTextBox.Name;
                EESName = sourceName.Split('_')[0];
                if (int.TryParse(objTextBox.text1.Text, out number))
                    objSlotDba.SetCustomizedBaseZoneEnd(EESName, number);
                objTextBox.text1.Text = objSlotDba.GetCustomizedBaseZoneEnd(EESName).ToString();
            }
        }

        private void EES_zone_start_nonbase_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (objSlotDba == null)
                    objSlotDba = new SlotDba();
                string sourceName = null;
                string EESName = null;
                string textValue;
                int number = 0;
                LabelTextBox objTextBox = sender as LabelTextBox;

                sourceName = objTextBox.Name;
                EESName = sourceName.Split('_')[0];
                if (int.TryParse(objTextBox.text1.Text, out number))
                    objSlotDba.SetCustomizedNonBaseZoneStart(EESName, number);
                objTextBox.text1.Text = objSlotDba.GetCustomizedNonBaseZoneStart(EESName).ToString();
            }
        }

        private void EES_zone_end_nonbase_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (objSlotDba == null)
                    objSlotDba = new SlotDba();
                string sourceName = null;
                string EESName = null;
                string textValue;
                int number = 0;
                LabelTextBox objTextBox = sender as LabelTextBox;

                sourceName = objTextBox.Name;
                EESName = sourceName.Split('_')[0];
                if (int.TryParse(objTextBox.text1.Text, out number))
                    objSlotDba.SetCustomizedNonBaseZoneEnd(EESName, number);
                objTextBox.text1.Text = objSlotDba.GetCustomizedNonBaseZoneEnd(EESName).ToString();
            }
        }

        private void expandZoneCheck_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)customizeRadio.IsChecked)
            {
                if (objSlotDba == null)
                    objSlotDba = new SlotDba();
                objSlotDba.SetZoneExpandStatus(true);
                expandZoneCheck.IsChecked = objSlotDba.GetZoneExpandStatus();
            }
        }

        private void expandZoneCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            if ((bool)customizeRadio.IsChecked)
            {
                if (objSlotDba == null)
                    objSlotDba = new SlotDba();
                CheckBox expandCheck = (CheckBox)sender;
                objSlotDba.SetZoneExpandStatus((bool)expandCheck.IsChecked);
                expandZoneCheck.IsChecked = objSlotDba.GetZoneExpandStatus();
            }
        }

        private void expandNumberText_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (objSlotDba == null)
                    objSlotDba = new SlotDba();
                int expandNumber = 0;
                if (int.TryParse(expandNumberText.text1.Text, out expandNumber))
                    objSlotDba.SetZoneExpandNumber(expandNumber);
                expandNumberText.text1.Text = objSlotDba.GetZoneExpandNumber().ToString();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
               MessageBoxResult messageBoxResult 
                    = System.Windows.MessageBox.Show("Do you want to change slot selection logic?", "Slot selection Confirmation", System.Windows.MessageBoxButton.YesNo);
               if (messageBoxResult == MessageBoxResult.Yes)
               {
                   if (objSlotDba == null)
                       objSlotDba = new SlotDba();
                   int selType = (int)SELECTION_TYPE.DEFAULT;
                   if ((bool)peakHourRadio.IsChecked)
                       selType = (int)SELECTION_TYPE.PEAK;
                   else if ((bool)customizeRadio.IsChecked)
                       selType = (int)SELECTION_TYPE.CUSTOM;
                   objSlotDba.SetSlotSelectionType(selType);
                   ShowCurrentSlotSelectionData();
               }
        }
        private void ShowCurrentSlotSelectionData()
        {
            if (objSlotDba == null)
                objSlotDba = new SlotDba();
            SELECTION_TYPE selEnum = (SELECTION_TYPE)objSlotDba.GetSlotSelectionType();
            if (selEnum == SELECTION_TYPE.CUSTOM)
            {
                LoadCustomizedZoneData();
                selectionTypeText.Text = "CUSTOM";
            }
            else if (selEnum == SELECTION_TYPE.PEAK)
            {
                LoadPeakHourZoneData();
                selectionTypeText.Text = "PEAK HOUR";
            }
            else
            {
                LoadDefaultZoneData();
                selectionTypeText.Text = "DEFAULT";
            }
        }

        private void EES_base_ref_aisle_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (objSlotDba == null)
                    objSlotDba = new SlotDba();
                string sourceName = null;
                string EESName = null;
                string textValue;
                int number = 0;
                LabelTextBox objTextBox = sender as LabelTextBox;

                sourceName = objTextBox.Name;
                EESName = sourceName.Split('_')[0];
                if (int.TryParse(objTextBox.text1.Text, out number))
                    objSlotDba.SetCustomizedBaseRefAisle(EESName, number);
                objTextBox.text1.Text = objSlotDba.GetCustomizedBaseRefAisle(EESName).ToString();
            }
        }

        private void EES_nonbase_ref_aisle_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (objSlotDba == null)
                    objSlotDba = new SlotDba();
                string sourceName = null;
                string EESName = null;
                string textValue;
                int number = 0;
                LabelTextBox objTextBox = sender as LabelTextBox;

                sourceName = objTextBox.Name;
                EESName = sourceName.Split('_')[0];
                if (int.TryParse(objTextBox.text1.Text, out number))
                    objSlotDba.SetCustomizedNonBaseRefAisle(EESName, number);
                objTextBox.text1.Text = objSlotDba.GetCustomizedNonBaseRefAisle(EESName).ToString();
            }
        }
     
    }
}
