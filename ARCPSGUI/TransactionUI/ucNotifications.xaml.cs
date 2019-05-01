using ARCPSGUI.DB;
using ARCPSGUI.Model;
using ARCPSGUI.StaticGlobalClass;
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

namespace ARCPSGUI.TransactionUI
{
    /// <summary>
    /// Interaction logic for ucNotifications.xaml
    /// </summary>
    public partial class ucNotifications : UserControl
    {
        GeneralDba objGeneralDba = null;
        List<string> machines = null;
       // List<string> notifCategories = new List<string>() { ""};
        string filterMachineLabel = "Select Machine";
        string filterCategoryLabel = "Select Category";

        public ucNotifications()
        {
            InitializeComponent();
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (objGeneralDba == null)
                objGeneralDba = new GeneralDba();
            machines = objGeneralDba.GetAllMachines();

            machines.Add(filterMachineLabel);
            filterMachine.ItemsSource = machines;
            filterMachine.SelectedValue = filterMachineLabel;

            List<string> notifCategories = Enum.GetNames(typeof(NotificationData.errorCategory)).ToList();
            notifCategories.Add(filterCategoryLabel);
            filterCategory.ItemsSource = notifCategories;
            filterCategory.SelectedValue = filterCategoryLabel;
            filterStartDate.Value = CommonMethods.AbsoluteStart(DateTime.Now);
            filterEndDate.Value = CommonMethods.AbsoluteEnd(DateTime.Now);
            FilterGrid();
        }
        private void searchBut_Click(object sender, RoutedEventArgs e)
        {
            FilterGrid();
        }
        void FilterGrid()
        {
            NotificationData filterNotificationData = new NotificationData();
            filterNotificationData.MachineCode = filterMachine.Text.Equals(filterMachineLabel) ? null : filterMachine.Text;
            NotificationData.errorCategory category = NotificationData.errorCategory.NA;
            Enum.TryParse(filterCategory.Text, out category);
            filterNotificationData.category = category;
            filterNotificationData.FilterNotifyStart = DateTime.Parse(filterStartDate.Value.ToString());
            filterNotificationData.FilterNotifyEnd = DateTime.Parse(filterEndDate.Value.ToString());
            notificationGrid.ItemsSource = objGeneralDba.GetNotifications(filterNotificationData);
        }
        
    }
}
