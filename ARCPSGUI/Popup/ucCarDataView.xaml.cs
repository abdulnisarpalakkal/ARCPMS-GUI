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
using ARCPSGUI.Model;
using ARCPSGUI.Popup;
using Xceed.Wpf.Toolkit;
using ARCPSGUI.DB;

namespace ARCPSGUI.Popup
{
    /// <summary>
    /// Interaction logic for ucCarDataView.xaml
    /// </summary>
    public partial class ucCarDataView : UserControl
    {
       //DateTimePicker
        CarData objCarData = null;
        Dictionary<int, string> carTypeList = new Dictionary<int, string>() { { 1, "Low" }, { 2, "High" }, { 3, "Mid" } };
        GeneralDba objGeneralDba = null;
        public bool Disable { get; set; }
        public EventHandler retrieveEventHandler;
            //carTypeList[1] = "Low";
            //carTypeList[2] = "High";
            //carTypeList[3] = "Mid";
        public ucCarDataView()
        {
            InitializeComponent();
            carTypeCompo.ItemsSource = carTypeList;
            carTypeCompo.DisplayMemberPath = "Value";
            carTypeCompo.SelectedValuePath = "Key";
            carTypeCompo.SelectedValue = 2;
            entryTimePicker.Value = System.DateTime.Now;
            //entryTimePicker.SelectedDateFormat=DatePickerFormat.
            
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //Console.WriteLine(objCarData.CardId);

            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
              
        }
        public void SetCarData(CarData objCarData)
        {
            this.objCarData = objCarData;
            if (objCarData != null)
            {
                cardIdText.Text = objCarData.CardId;
                plateText.Text = objCarData.Plate;
                patronNameText.Text = objCarData.PatronName;
                washCheck.IsChecked = objCarData.Wash;
                carTypeCompo.SelectedValue = objCarData.CarType;
                rotationCheck.IsChecked = objCarData.IsRotated;
                entryEESText.Text = objCarData.EntryGate;
                //entryTimePicker.SelectedDate = objCarData.EntryTime;
                entryTimePicker.Value = objCarData.EntryTime;
                entryTimePicker.IsEnabled = false;
                carTypeCompo.IsEnabled = false;
            }
            //else
            //{
            //    carTypeCompo.SelectedValue = 2;
            //}

        }
        public CarData GetCarData()
        {
            if (this.objCarData == null)
                this.objCarData = new CarData();
            objCarData.CardId=cardIdText.Text;
            objCarData.Plate = plateText.Text;
            objCarData.PatronName= patronNameText.Text ;
            objCarData.Wash= (bool)washCheck.IsChecked;
            objCarData.CarType = (int)carTypeCompo.SelectedValue;
            objCarData.IsRotated = (bool)rotationCheck.IsChecked;
            //Convert.ToDateTime(entryTimePicker.Text);
            objCarData.EntryTime =  (DateTime)entryTimePicker.Value;
            return objCarData;

        }

        private void photoBut_Click(object sender, RoutedEventArgs e)
        {
            PhotoPop objPhotoPop = new PhotoPop(this.objCarData);
            objPhotoPop.Show();

        }

        private void retrieveBut_Click(object sender, RoutedEventArgs e)
        {
            string message = null;
            bool success = false;
            if (Disable)
            {
                message = "If you want to retrieve the car, you should manually enable the slot.";
                System.Windows.MessageBox.Show(message, "Confirmaton", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                message = "Do you want to retrieve the car?";

            if (!Disable && System.Windows.MessageBox.Show(message, "Confirmaton", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                if (objGeneralDba.InsertQueue(objCarData.CardId, 0))
                    this.retrieveEventHandler(sender, new EventArgs());
                else
                    System.Windows.MessageBox.Show("Error when sending retrieval request");
            }
           
        }

       

       
    }
}
