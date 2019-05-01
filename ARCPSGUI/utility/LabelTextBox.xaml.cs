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

namespace ARCPSGUI.utility
{
    /// <summary>
    /// Interaction logic for LabelTextBox.xaml
    /// </summary>
    public partial class LabelTextBox : UserControl
    {
        private string LabelToTextBoxStyle= "LableToTextBox";
        private string TextToLabelBoxStyle = "TextToLabelBox";
        private TextBox objTextBox = null;
        public string sourceName { get; set; }
        public LabelTextBox()
        {
            InitializeComponent();

        }
        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            objTextBox = sender as TextBox;
            objTextBox.Style = (Style)this.Resources[LabelToTextBoxStyle];
        }

        private void text1_KeyUp(object sender, KeyEventArgs e)
        {
            objTextBox = sender as TextBox;
            if (e.Key == Key.Enter)
            {
                objTextBox.Style = (Style)this.Resources[TextToLabelBoxStyle];
            }
        }

      

        private void text_LostFocus(object sender, RoutedEventArgs e)
        {
            objTextBox = sender as TextBox;
            objTextBox.Style = (Style)this.Resources[TextToLabelBoxStyle];
        }
    }
}
