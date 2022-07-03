using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Y2KWMS.Controller;
using MaterialDesignThemes.Wpf;

namespace Y2KWMS.View
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        LoginController loginController; 
        public Login()
        {
            InitializeComponent();
            DataContext = this;
            loginController = new LoginController();

        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void loginBtn_Click(object sender, RoutedEventArgs e)
        {
            if(txtUsername.Text == null)
            {
                txtUsername.BorderBrush = Brushes.Red;
                txtUsername.ToolTip = "Enter the email";
            }

            if (txtPassword.Password == null)
            {
                txtPassword.BorderBrush = Brushes.Red;
                txtPassword.ToolTip = "Enter the password";
            }
            if (txtUsername.Text != null && txtPassword.Password != null)
            {
                Model.Login login = new Model.Login(txtUsername.Text, txtPassword.Password);
                bool loginSuccess = loginController.login(login);
                if(loginSuccess)
                {
                    this.Close();
                }
                else
                {
                    txbError.Text = "Enter valid email and password";
                    txtUsername.Clear();
                    txtPassword.Clear();
                }
            }
           
        }
    }
}
