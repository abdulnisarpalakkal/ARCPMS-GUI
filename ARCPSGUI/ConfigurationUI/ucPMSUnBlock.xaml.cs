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

namespace ARCPSGUI.Popup
{
    /// <summary>
    /// Interaction logic for ucPMSUnBlock.xaml
    /// </summary>
    public partial class ucPMSUnBlock : UserControl
    {
        Connection dbpm = new Connection(); 
        public ucPMSUnBlock()
        {
            InitializeComponent();
        }

        void ReadPSLockStatus()
        {
            bool isBlocked = false;
            Style lockStyle = null;

            //ps1
             isBlocked =  dbpm.GetMachineLockStatus("PS_FLR4_01",1);
             lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                               : (Style)FindResource("UnLockLabelStyle");
            lblPsh1LockStatus.Style = lockStyle;

            //ps2
            isBlocked = dbpm.GetMachineLockStatus("PS_FLR4_02", 1);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblPsh2LockStatus.Style = lockStyle;

            //ps3
            isBlocked = dbpm.GetMachineLockStatus("PS_FLR4_03", 1);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblPsh3LockStatus.Style = lockStyle;

            //ps4
            isBlocked = dbpm.GetMachineLockStatus("PS_FLR4_04", 1);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblPsh4LockStatus.Style = lockStyle;

        }

        void ReadPSTLockStatus()
        {
            bool isBlocked = false;
            Style lockStyle = null;

            //pst1
            isBlocked = dbpm.GetMachineLockStatus("PST_FLR4_01", 2);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblPst1LockStatus.Style = lockStyle;

            //pst2
            isBlocked = dbpm.GetMachineLockStatus("PST_FLR4_02", 2);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblPst2LockStatus.Style = lockStyle;

            //pst3
            isBlocked = dbpm.GetMachineLockStatus("PST_FLR4_03", 2);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblPst3LockStatus.Style = lockStyle;

            //pst4
            isBlocked = dbpm.GetMachineLockStatus("PST_FLR4_04", 2);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblPst4LockStatus.Style = lockStyle;
        }

        void ReadPVLLockStatus()
        {
            bool isBlocked = false;
            Style lockStyle = null;

            //pvl1
            isBlocked = dbpm.GetMachineLockStatus("PVL_Drive_01", 3);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblPvl1LockStatus.Style = lockStyle;

            //pvl2
            isBlocked = dbpm.GetMachineLockStatus("PVL_Drive_02", 3);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblPvl2LockStatus.Style = lockStyle;

            //pvl3
            isBlocked = dbpm.GetMachineLockStatus("PVL_Drive_03", 3);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblPvl3LockStatus.Style = lockStyle;
        }

        void ReadEESLockStatus()
        {
            bool isBlocked = false;
            Style lockStyle = null;

            //EES1
            isBlocked = dbpm.GetMachineLockStatus("EES_FLR4_01", 4);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblEES1LockStatus.Style = lockStyle;

            //EES2
            isBlocked = dbpm.GetMachineLockStatus("EES_FLR4_02", 4);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblEES2LockStatus.Style = lockStyle;

            //EES3
            isBlocked = dbpm.GetMachineLockStatus("EES_FLR4_03", 4);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblEES3LockStatus.Style = lockStyle;


            //EES4
            isBlocked = dbpm.GetMachineLockStatus("EES_FLR4_04", 4);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblEES4LockStatus.Style = lockStyle;

            //EES5
            isBlocked = dbpm.GetMachineLockStatus("EES_FLR4_05", 4);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblEES5LockStatus.Style = lockStyle;

            //EES6
            isBlocked = dbpm.GetMachineLockStatus("EES_FLR4_06", 4);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblEES6LockStatus.Style = lockStyle;

            //EES7
            isBlocked = dbpm.GetMachineLockStatus("EES_FLR4_07", 4);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblEES7LockStatus.Style = lockStyle;

            //EES8
            isBlocked = dbpm.GetMachineLockStatus("EES_FLR4_08", 4);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblEES8LockStatus.Style = lockStyle;

            //EES9
            isBlocked = dbpm.GetMachineLockStatus("EES_FLR4_09", 4);
            lockStyle = isBlocked ? (Style)FindResource("LockLabelStyle")
                              : (Style)FindResource("UnLockLabelStyle");
            lblEES9LockStatus.Style = lockStyle;

        }

        void UpdateBlockStatus(string machineName, int type)
        {
            if (MessageBox.Show("Do you want to continue?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                dbpm.UpdateMachineBlockStatus(machineName, type);
            }
        }

        private void btnPsh1_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PS_FLR4_01", 1);
            ReadPSLockStatus();
        }

        private void btnPsh2_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PS_FLR4_02", 1);
            ReadPSLockStatus();
        }

        private void btnPsh3_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PS_FLR4_03", 1);
            ReadPSLockStatus();
        }

        private void btnPsh4_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PS_FLR4_04", 1);
            ReadPSLockStatus();
        }

        private void btnPst1_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PST_FLR4_01", 2);
            ReadPSTLockStatus();
        }

        private void btnPst2_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PST_FLR4_02", 2);
            ReadPSTLockStatus();
        }

        private void btnPst3_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PST_FLR4_03", 2);
            ReadPSTLockStatus();
        }

        private void btnPst4_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PST_FLR4_04", 2);
            ReadPSTLockStatus();
        }

        private void btnPvl1_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PVL_Drive_01", 3);
            ReadPVLLockStatus();
        }

        private void btnPvl2_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PVL_Drive_02", 3);
            ReadPVLLockStatus();
        }

        private void btnPvl3_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("PVL_Drive_03", 3);
            ReadPVLLockStatus();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ReadPSLockStatus();
            ReadPSTLockStatus();
            ReadPVLLockStatus();
            ReadEESLockStatus();
        }

        private void btEES1_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("EES_FLR4_01", 4);
            ReadEESLockStatus();
        }

        private void btEES2_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("EES_FLR4_02", 4);
            ReadEESLockStatus();
        }

        private void btEES3_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("EES_FLR4_03", 4);
            ReadEESLockStatus();
        }

        private void btEES4_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("EES_FLR4_04", 4);
            ReadEESLockStatus();
        }

        private void btEES5_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("EES_FLR4_05", 4);
            ReadEESLockStatus();
        }

        private void btEES6_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("EES_FLR4_06", 4);
            ReadEESLockStatus();
        }

        private void btEES7_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("EES_FLR4_07", 4);
            ReadEESLockStatus();
        }

        private void btEES8_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("EES_FLR4_08", 4);
            ReadEESLockStatus();
        }

        private void btEES9_Click(object sender, RoutedEventArgs e)
        {
            UpdateBlockStatus("EES_FLR4_09", 4);
            ReadEESLockStatus();
        }

    }
}
