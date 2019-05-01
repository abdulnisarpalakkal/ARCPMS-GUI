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

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for ucWaitHistView.xaml
    /// </summary>
    public partial class ucWaitHistView : UserControl
    {
        EESWaitDba objEESWaitDba = null;
        List<string> gateList = new List<string>(new string[] { "","EES1", "EES2", "EES3", "EES4", "EES5", "EES6", "EES7", "EES8", "EES9" });
        Dictionary<int, string> locationDictionary = new Dictionary<int, string>() 
        { 
            {0,""},
            {1,"Outside gate"},
            {2,"Inside gate"},
            {3,"On kiosk"}  
        };
        public ucWaitHistView()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

           
            gateCompo.ItemsSource = gateList;
            //gateCompo.i
            locationCompo.ItemsSource = locationDictionary;
            EESWaitData objEESWaitData = new EESWaitData();
            objEESWaitData.WaitGate = "";
            objEESWaitData.WaitLocationId = 0;
            objEESWaitData.WaitUpdateTime = System.DateTime.Now;

            waitDatePicker.SelectedDate = objEESWaitData.WaitUpdateTime;
            gateCompo.SelectedItem = "";
            locationCompo.SelectedValue = 0;

            SetEESWaitList(objEESWaitData);
        }

        void SetEESWaitList(EESWaitData objEESWaitData)
        {
            if (objEESWaitDba == null)
                objEESWaitDba = new EESWaitDba();
            waitListGrid.ItemsSource = objEESWaitDba.GetEESWaitList(objEESWaitData);
        }

        private void searchBut_Click(object sender, RoutedEventArgs e)
        {
            EESWaitData objEESWaitSearchData = new EESWaitData();
            objEESWaitSearchData.WaitGate = (string)gateCompo.SelectedItem;
            
            objEESWaitSearchData.WaitLocationId = (int)locationCompo.SelectedValue;
            objEESWaitSearchData.WaitUpdateTime = waitDatePicker.SelectedDate ?? DateTime.Now;
            SetEESWaitList(objEESWaitSearchData);
        }

        private void RefreshBut_Click(object sender, RoutedEventArgs e)
        {
            EESWaitData objEESWaitData = new EESWaitData();
            objEESWaitData.WaitGate = "";
            objEESWaitData.WaitLocationId = 0;
            objEESWaitData.WaitUpdateTime = System.DateTime.Now;

            waitDatePicker.SelectedDate = objEESWaitData.WaitUpdateTime;
            gateCompo.SelectedItem = "";
            locationCompo.SelectedValue = 0;

            SetEESWaitList(objEESWaitData);
        }



        
    }
}
