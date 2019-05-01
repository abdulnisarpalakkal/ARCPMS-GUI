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
using ARCPSGUI.utility;

using ARCPSGUI.DB;

namespace ARCPSGUI.ConfigurationUI
{
    /// <summary>
    /// Interaction logic for CMWindowLimitConfig.xaml
    /// </summary>
    public partial class CMWindowLimitConfig : UserControl
    {
        Connection objConnection = null;
        public CMWindowLimitConfig()
        {
            InitializeComponent();
            LoadInitialValues();
        }

        private void LoadInitialValues()
        {
            if(objConnection==null) objConnection=new Connection();
            string sourceName = null;
            string machineCode = null;
            bool isMin = false;
            int window = 0;
            foreach (LabelTextBox tb in FindVisualChildren<LabelTextBox>(mainGrid))
            {
                sourceName = tb.sourceName;
                machineCode=sourceName.Split(':')[0];
                isMin = sourceName.Split(':')[1]=="MIN";
                if (isMin)
                    window = objConnection.GetCMMinConfigWindow(machineCode);
                else
                    window = objConnection.GetCMMaxConfigWindow(machineCode);
                tb.text1.Text = window.ToString();
            }
        }


        private void labelText_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            LabelTextBox objTextBox=sender as LabelTextBox;

            
            if (e.Key == Key.Enter)
            {
                if(objConnection==null) objConnection=new Connection();
                string sourceName = null;
                string machineCode = null;
                bool isMin = false;
                int window = 0;
                string textValue;
                sourceName = objTextBox.sourceName;
                machineCode = sourceName.Split(':')[0];
                isMin = sourceName.Split(':')[1] == "MIN";
                textValue=objTextBox.text1.Text;
                int.TryParse(textValue, out window);
                if (isMin)
                    objConnection.UpdateCMMinConfigWindow(machineCode,window);
                else
                    objConnection.UpdateCMMaxConfigWindow(machineCode, window);

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

        private void reset_but_Click(object sender, RoutedEventArgs e)
        {
            if (objConnection == null) objConnection = new Connection();
            objConnection.ResetCMConfigWindow();
            LoadInitialValues();
        }
    }
}
