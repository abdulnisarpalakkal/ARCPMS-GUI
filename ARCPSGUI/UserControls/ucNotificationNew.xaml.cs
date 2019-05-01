using ARCPSGUI.Model;
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
    /// Interaction logic for ucNotificationNew.xaml
    /// </summary>
    public partial class ucNotificationNew : UserControl
    {
        public NotificationData notification { get; set; }
        LinearGradientBrush statusGradient = null;
        public ucNotificationNew()
        {
            InitializeComponent();
        }
        public void SetNotificationData(NotificationData notification)
        {
            this.notification = notification;
            SetToView();
        }
        private void SetToView()
        {
            errorGrid.Visibility = Visibility.Hidden;
            manualGrid.Visibility = Visibility.Hidden;
            triggerGrid.Visibility = Visibility.Hidden;
            disableGrid.Visibility = Visibility.Hidden;

            machineCode.Text = notification.MachineCode;
            notifButton.Opacity = 0.9;
           
            switch(notification.category)
            {
                case NotificationData.errorCategory.ERROR:
                    errorGrid.Visibility = Visibility.Visible;
                    statusGradient = (LinearGradientBrush)this.Resources["notificationErrorBack"];
                    errorCode.Text = notification.ErrorCode.ToString();
                    break;
                case NotificationData.errorCategory.MANUAL:
                    manualGrid.Visibility = Visibility.Visible;
                    statusGradient = (LinearGradientBrush)this.Resources["notificationManualBack"];
                    break;
                case NotificationData.errorCategory.TRIGGER:
                    triggerGrid.Visibility = Visibility.Visible;
                    statusGradient = (LinearGradientBrush)this.Resources["notificationTriggerBack"];
                    break;
                case NotificationData.errorCategory.DISABLE:
                    disableGrid.Visibility = Visibility.Visible;
                    statusGradient = (LinearGradientBrush)this.Resources["notificationManualBack"];
                    notifButton.Opacity = 0.4;
                    break;
                default:
                    break;
            }
            notifButton.Background = statusGradient;


        }



    }
}
