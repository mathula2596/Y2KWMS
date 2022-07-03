using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using Y2KWMS.Controller;

namespace Y2KWMS.View.Pages.Project.User
{
    /// <summary>
    /// Interaction logic for Index.xaml
    /// </summary>
    public partial class Index : Page
    {
        UserController userController;
        Model.User user;

        public Index(string email, string name, string type)
        {
            userController = new UserController();

            InitializeComponent();

         
            cmbSearchBy.Items.Add("ID");
            cmbSearchBy.Items.Add("FirstName");
            cmbSearchBy.Items.Add("LastName");
            cmbSearchBy.Items.Add("Phone");
            cmbSearchBy.Items.Add("Email");
            cmbSearchBy.Items.Add("Branch");
            cmbSearchBy.Items.Add("Status");
            cmbSearchBy.SelectedIndex = 0;

            load();
            user = new Model.User();
            user.Email = email;
            user.Type = type;
            user.FirstName = name;

            if (type != "ProjectHead")
            {
                editColumn.Visibility = Visibility.Collapsed;
            }
        }

        private void load()
        {
            DataTable dt = userController.load();
            dgListDetails.ItemsSource = dt.DefaultView;
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            string searchKey = cmbSearchBy.Text.ToLower();

            string searchValue = txtSearch.Text;
            
            DataTable dt = userController.search(searchKey,searchValue);
            dgListDetails.ItemsSource = dt.DefaultView;
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            string id = ((Button)sender).CommandParameter.ToString();
            userController.edit(id,user);


        }

       
    }
}
