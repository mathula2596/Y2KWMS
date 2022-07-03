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

namespace Y2KWMS.View.Pages.Team
{
    /// <summary>
    /// Interaction logic for Index.xaml
    /// </summary>
    public partial class Index : Page
    {
        TeamController teamController;
        Model.User user;
        public Index(string email, string name, string type)
        {
            InitializeComponent();
            teamController = new TeamController();
            load();
            user = new Model.User();
            user.Email = email;
            user.Type = type;
            user.FirstName = name;
        }

        private void load()
        {
            DataTable dt = teamController.load();
            dgListDetails.ItemsSource = dt.DefaultView;
        }
    }
}
