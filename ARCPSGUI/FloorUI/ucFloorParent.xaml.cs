using ARCPSGUI.DB;
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

namespace ARCPSGUI.FloorUI
{
    /// <summary>
    /// Interaction logic for ucFloorParent.xaml
    /// </summary>
    public partial class ucFloorParent : UserControl
    {
        public int Floor { get; set; }
        Object floorObj = null;
        PVLDba objPVLDba = null;

        public ucFloorParent(int floor)
        {
            InitializeComponent();
            Floor = floor;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           
            
            floorNoText.Text = Floor.ToString();
            floorViewBox.Child = GetFloorObject() as UIElement;
            ucMachineJob.Floor = Floor;
            LoadInitialValuesForPVL();
        }

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //   // floorViewBox.Child.
         
        //    floorObj = null;
        //    floorViewBox.Child = GetFloorObject() as UIElement;
        //}
        public Object GetFloorObject()
        {
            if (Floor == 1)
            {
                if (floorObj == null)
                    floorObj = new ucFloor1();

            }
            if (Floor == 2)
            {
                if (floorObj == null)
                    floorObj = new ucFloor2();

            }
            if (Floor == 3)
            {
                if (floorObj == null)
                    floorObj = new ucFloor3();

            }
            else if (Floor == 4)
            {
                if (floorObj == null)
                    floorObj = new ucFloor4();
            }
            else if (Floor == 5)
            {
                

                if (floorObj == null)
                    floorObj = new ucFloor5();
            }
            else if (Floor == 6)
            {
                if (floorObj == null)
                    floorObj = new ucFloor6();
            }
            else if (Floor == 7)
            {
                if (floorObj == null)
                    floorObj = new ucFloor7();
            }
            else if (Floor == 8)
            {
                if (floorObj == null)
                    floorObj = new ucFloor8();
            }
            else if (Floor == 9)
            {
                if (floorObj == null)
                    floorObj = new ucFloor9();
            }
            if (Floor == 5 || Floor == 6 || Floor == 7)
            {
                pvlConfigGrid.Visibility = Visibility.Visible;
            }
            else
            {
                pvlConfigGrid.Visibility = Visibility.Hidden;
            }
            return floorObj;
        }
        private void LoadInitialValuesForPVL()
        {
            if (objPVLDba == null)
                objPVLDba = new PVLDba();
            string sourceName = null;
            string machineCode = null;
            bool isMin = false;
            int window = 0;
            foreach (LabelTextBox tb in FindVisualChildren<LabelTextBox>(pvlConfigGrid))
            {
                sourceName = tb.sourceName;
                machineCode = sourceName.Split(':')[0];
                isMin = sourceName.Split(':')[1] == "MIN";
                if (isMin)
                    window = objPVLDba.GetPVLMinSlotRange(machineCode);
                else
                    window = objPVLDba.GetPVLMaxSlotRange(machineCode);
                tb.text1.Text = window.ToString();
            }
        }

        private void labelText_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            LabelTextBox objTextBox = sender as LabelTextBox;


            if (e.Key == Key.Enter)
            {
                if (objPVLDba == null)
                    objPVLDba = new PVLDba();
                string sourceName = null;
                string machineCode = null;
                bool isMin = false;
                int window = 0;
                string textValue;
                sourceName = objTextBox.sourceName;
                machineCode = sourceName.Split(':')[0];
                isMin = sourceName.Split(':')[1] == "MIN";
                textValue = objTextBox.text1.Text;
                if (int.TryParse(textValue, out window))
                {
                    if (isMin)
                    {
                        objPVLDba.SetPVLMinSlotRange(machineCode, window);
                        window = objPVLDba.GetPVLMinSlotRange(machineCode);
                    }
                    else
                    {
                        objPVLDba.SetPVLMaxSlotRange(machineCode, window);
                        window = objPVLDba.GetPVLMaxSlotRange(machineCode);
                    }
                    objTextBox.text1.Text = window.ToString();
                }

            }
        }
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void resetBut_Click(object sender, RoutedEventArgs e)
        {
            if (objPVLDba == null)
                objPVLDba = new PVLDba();
            objPVLDba.ResetPVLZone();
            LoadInitialValuesForPVL();
        }
      
    }
}
