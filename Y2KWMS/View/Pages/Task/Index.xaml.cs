using System;
using System.Collections.Generic;
using System.Data;
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
using Y2KWMS.Model;


namespace Y2KWMS.View.Pages.Task
{
    /// <summary>
    /// Interaction logic for Index.xaml
    /// </summary>
    public partial class Index : Page
    {
        TaskController taskController;
        Model.User user;

        public Index(string email, string name, string type)
        {
            InitializeComponent();
            taskController = new TaskController();
            cmbSearchBy.Items.Add("ID");
            cmbSearchBy.Items.Add("Project");
            cmbSearchBy.Items.Add("Title");
            cmbSearchBy.Items.Add("Status");
            cmbSearchBy.Items.Add("CreatedDate");
            cmbSearchBy.Items.Add("StartDate");
            cmbSearchBy.Items.Add("ExpectedEndDate");
            cmbSearchBy.Items.Add("EndDate");


            //cmbSearchBy.Items.Add("Team");
            cmbSearchBy.SelectedIndex = 0;

            load();
            user = new Model.User();
            user.Email = email;
            user.Type = type;
            user.FirstName = name;
            //searchBox();

            if (type == "TeamMember")
            {
                editColumn.Visibility = Visibility.Collapsed;
            }
        }

        private void load()
        {
            DataTable dt = taskController.load();
            dgListDetails.ItemsSource = dt.DefaultView;
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            string searchKey = cmbSearchBy.Text.ToLower();

           
            string searchValue = txtSearch.Text;

            DataTable dt = taskController.search(searchKey, searchValue);
            dgListDetails.ItemsSource = dt.DefaultView;
        }

        private void txtDateSearch_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            string searchValue = null;
            if (txtDateSearch.SelectedDate.HasValue)
            {
                searchValue = txtDateSearch.SelectedDate.Value.ToString();
            }

            string searchKey = cmbSearchBy.Text.ToLower();

            if (searchKey == "createddate")
            {
                searchKey = "createdDate";
            }
            if (searchKey == "startdate")
            {
                searchKey = "startDate";
            }
            if (searchKey == "expectedenddate")
            {
                searchKey = "expectedEndDate";
            }
            if (searchKey == "enddate")
            {
                searchKey = "endDate";
            }
            DataTable dt = taskController.search(searchKey, searchValue);
            dgListDetails.ItemsSource = dt.DefaultView;
        }

        private void cmbSearchBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            searchBox();
        }

        private void searchBox()
        {
            //MessageBox.Show(cmbSearchBy.SelectedIndex.ToString());
            if (cmbSearchBy.SelectedIndex >= 0)
            {
                if (cmbSearchBy.SelectedIndex > 2)
                {
                    txtSearch.Visibility = Visibility.Collapsed;
                    txtDateSearch.Visibility = Visibility.Visible;
                }
                else
                {
                    txtDateSearch.Visibility = Visibility.Collapsed;
                    txtSearch.Visibility = Visibility.Visible;

                }
            }
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            string id = ((Button)sender).CommandParameter.ToString();

            taskController.view(id,user);
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            string id = ((Button)sender).CommandParameter.ToString();
            taskController.edit(id,user);
        }
    }
}
