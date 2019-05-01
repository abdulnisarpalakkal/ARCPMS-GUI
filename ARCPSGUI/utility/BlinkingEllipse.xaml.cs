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
    /// Interaction logic for BlinkingEllipse.xaml
    /// </summary>
    public partial class BlinkingEllipse : UserControl
    {
        private bool blinking;
        public BlinkingEllipse()
        {
            InitializeComponent();
        }
        public bool IsBlinking
        {
            get
            {
                return blinking;
            }

            set
            {
                //if (value)
                //{
                //    VisualStateManager.GoToState(this, "Blinking", true);
                //}
                //else
                //{
                //    VisualStateManager.GoToState(this, "Stopped", true);
                //}

                //this.blinking = value;
            }
        }	
    }
}
