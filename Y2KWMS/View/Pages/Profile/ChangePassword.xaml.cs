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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Y2KWMS.Model;
using Y2KWMS.Controller;

namespace Y2KWMS.View.Pages.Profile
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Page
    {
        Helper helper;
        UserController userController;
        public ChangePassword()
        {
            InitializeComponent();
            helper = new Helper();
            userController = new UserController();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Password.Length > 0)
            {
                if (!helper.passwordValidation(txtPassword.Password))
                {
                    txtPassword.BorderBrush = Brushes.Red;
                    txtPassword.ToolTip = "Password must have one uppercase letter, numbers, symbols and length must be more than 8";
                }
                else
                {
                    User user = new User(txtPassword.Password);
                    userController.updatePassword(user);
                    txtPassword.Clear();

                }

            }
        }
    }
}
