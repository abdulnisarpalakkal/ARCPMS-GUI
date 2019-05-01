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
using System.Data;

namespace ARCPSGUI.Security
{
    /// <summary>
    /// Interaction logic for frmAuthenticationWindow.xaml
    /// </summary>
    public partial class frmAuthenticationWindow : Window
    {
        public event EventHandler OnCloseRequest = null;
        
         bool isCurrentUserReLogin = false;
         public frmAuthenticationWindow(bool isCurrentUserReLogin = false)
        {
            InitializeComponent();
            this.isCurrentUserReLogin = isCurrentUserReLogin;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.isCurrentUserReLogin)
            {
                txtUserName.Text = Security.GetUserName(Security.currentUserId);
                txtUserName.IsEnabled = false;
                btnCancel.Content = "Close";
            }
            else
               txtUserName.Focus();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Security.currentUserId = 0;
            if (!this.isCurrentUserReLogin && Security.GetUserId(txtUserName.Text.Trim(), txtPwd.Password.Trim()) > 0) //if authentication successed then close this form.
                this.Close();
            else if (this.isCurrentUserReLogin && Security.CheckAuthentication(txtUserName.Text.Trim(), txtPwd.Password.Trim()) > 0) //if authentication successed then close this form.
                this.Close();
            else
            {
                MessageBoxResult dresult = MessageBox.Show("Invalid username/password. Do you want to continue?",
                       "Information", MessageBoxButton.YesNo, MessageBoxImage.Information);

                if (dresult == MessageBoxResult.Yes)
                {
                    txtUserName.Text = "";
                    txtPwd.Password = "";
                    txtUserName.Focus();
                }
                else if (dresult == MessageBoxResult.No)
                {
                    if (this.OnCloseRequest != null)
                    {
                        this.Close();
                        this.OnCloseRequest(sender, e);
                    }
                }
            }
            

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.OnCloseRequest != null)
            {
                this.Close();
                this.OnCloseRequest(sender, e);
            }
            else
            {
                this.DialogResult = true;
                this.Close();
               
            }

        }

        private void txtPwd_PasswordChanged(object sender, RoutedEventArgs e)
        {

        }

       
    }
}
