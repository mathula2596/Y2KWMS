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
using Y2KWMS.View.Pages;
using Y2KWMS.Model;

//using Y2KWMS.View.Pages.Project;

namespace Y2KWMS.View
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public string email;
        public string name;
        public Dashboard(Model.User user)
        {
            InitializeComponent();
            email = user.Email;
            name = user.FirstName + " " + user.LastName; ;
            txtEmail.Content = user.Email;
            txtName.Content = user.FirstName + " " + user.LastName;
           // MessageBox.Show(user.Type);
            txtType.Content = user.Type;

            if(user.Type == "TeamLeader")
            {
                radioNewUser.Visibility = Visibility.Collapsed;
                radioNewTeam.Visibility = Visibility.Collapsed;
                radioNewProject.Visibility = Visibility.Collapsed;
                radioNewTeam.Visibility = Visibility.Collapsed;

            }
            else if (user.Type == "TeamMember")
            {
                radioNewUser.Visibility = Visibility.Collapsed;
                radioNewTeam.Visibility = Visibility.Collapsed;
                radioNewProject.Visibility = Visibility.Collapsed;
                radioNewTask.Visibility = Visibility.Collapsed;
                radioNewSubTask.Visibility = Visibility.Collapsed;
                radioNewTeam.Visibility = Visibility.Collapsed;


            }
        }
       
        private void explandCollaspeSubMenu(Grid gridName = null)
        {

            if(gridName != null)
            {
                if (gridName.Visibility == Visibility.Collapsed)
                {
                    subMenuProject.Visibility = Visibility.Collapsed;
                    subMenuTask.Visibility = Visibility.Collapsed;
                    subMenuSubTask.Visibility = Visibility.Collapsed;
                    subMenuUser.Visibility = Visibility.Collapsed;
                    subMenuTeam.Visibility = Visibility.Collapsed;
                    gridName.Visibility = Visibility.Visible;
                }
                else
                {
                    gridName.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                subMenuProject.Visibility = Visibility.Collapsed;
                subMenuTask.Visibility = Visibility.Collapsed;
                subMenuSubTask.Visibility = Visibility.Collapsed;
                subMenuUser.Visibility = Visibility.Collapsed;
                subMenuTeam.Visibility = Visibility.Collapsed;

            }

        }
        private void radioHome_Click(object sender, RoutedEventArgs e)
        {
            explandCollaspeSubMenu();
            PagesNavigation.Navigate(new HomePage());
        }

        
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }
        private void loadPage(string folder, string page)
        {
            
            PagesNavigation.Navigate(new System.Uri("View/Pages/"+folder+"/"+ page + ".xaml", UriKind.RelativeOrAbsolute));
        }
        private void radioProject_Click(object sender, RoutedEventArgs e)
        {
            explandCollaspeSubMenu(subMenuProject);
        }

        private void radioNewProject_Click(object sender, RoutedEventArgs e)
        {
           // loadPage("Project","Create");
            PagesNavigation.Navigate(new View.Pages.Project.Create(txtEmail.Content.ToString(),txtName.Content.ToString(),txtType.Content.ToString()));

        }

        private void radioViewProjects_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new View.Pages.Project.Index(txtEmail.Content.ToString(), txtName.Content.ToString(), txtType.Content.ToString()));

            //loadPage("Project", "Index");
        }

        private void radioTask_Click(object sender, RoutedEventArgs e)
        {
            explandCollaspeSubMenu(subMenuTask);
        }

        private void radioNewTask_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new View.Pages.Task.Create(txtEmail.Content.ToString(), txtName.Content.ToString(), txtType.Content.ToString()));

            //loadPage("Task", "Create");
        }

        private void radioViewTasks_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new View.Pages.Task.Index(txtEmail.Content.ToString(), txtName.Content.ToString(), txtType.Content.ToString()));

            //loadPage("Task", "Index");
        }

        private void radioSubTask_Click(object sender, RoutedEventArgs e)
        {
            explandCollaspeSubMenu(subMenuSubTask);
        }

        private void radioNewSubTask_Click(object sender, RoutedEventArgs e)
        {
           // loadPage("SubTask", "Create");
            PagesNavigation.Navigate(new View.Pages.SubTask.Create(txtEmail.Content.ToString(), txtName.Content.ToString(), txtType.Content.ToString()));


        }

        private void radioViewSubTasks_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new View.Pages.SubTask.Index(txtEmail.Content.ToString(), txtName.Content.ToString(), txtType.Content.ToString()));

            //loadPage("SubTask", "Index");

        }

        private void radioUser_Click(object sender, RoutedEventArgs e)
        {
            explandCollaspeSubMenu(subMenuUser);
        }

        private void radioNewUser_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new View.Pages.Project.User.Create(txtEmail.Content.ToString(), txtName.Content.ToString(), txtType.Content.ToString()));

            //loadPage("User", "Create");

        }

        private void radioViewUsers_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new View.Pages.Project.User.Index(txtEmail.Content.ToString(), txtName.Content.ToString(), txtType.Content.ToString()));


            //loadPage("User", "Index");

        }

        private void radioReport_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new View.Pages.Report.Index());

        }

        /*private void radioMessage_Click(object sender, RoutedEventArgs e)
        {

        }

        private void radioProfile_Click(object sender, RoutedEventArgs e)
        {

        }*/

        private void radioChangePassword_Click(object sender, RoutedEventArgs e)
        {
            PagesNavigation.Navigate(new View.Pages.Profile.ChangePassword());

        }

        private void dashboard_Loaded(object sender, RoutedEventArgs e)
        {
            loadPage("Home", "Index");
        }

        private void radioNewTeam_Click(object sender, RoutedEventArgs e)
        {
           // loadPage("Team", "Create");
            PagesNavigation.Navigate(new View.Pages.Team.Create(txtEmail.Content.ToString(), txtName.Content.ToString(), txtType.Content.ToString()));

        }

        private void radioViewTeam_Click(object sender, RoutedEventArgs e)
        {
            //loadPage("Team", "Index");
            PagesNavigation.Navigate(new View.Pages.Team.Index(txtEmail.Content.ToString(), txtName.Content.ToString(), txtType.Content.ToString()));

        }

        private void radioTeam_Click(object sender, RoutedEventArgs e)
        {
            explandCollaspeSubMenu(subMenuTeam);
        }

        private void radioLogout_Click(object sender, RoutedEventArgs e)
        {
            
            Login login = new Login();
            login.Show();
            this.Close();
        }
    }
}
