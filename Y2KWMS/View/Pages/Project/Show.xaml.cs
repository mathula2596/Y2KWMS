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

namespace Y2KWMS.View.Pages.Project
{
    /// <summary>
    /// Interaction logic for Show.xaml
    /// </summary>
    public partial class Show : Page
    {
        QueryController queryController;
        public Show(int projectId)
        {
            InitializeComponent();
            queryController = new QueryController();
            getProjectDetails(projectId);
        }

        private void getProjectDetails(int projectId)
        {
            DataTable projectDataTable = queryController.getProjectDetails(projectId);
            txbId.Text = projectDataTable.Rows[0][0].ToString();
            txbTitle.Text = projectDataTable.Rows[0][1].ToString();
            txbDesctiption.Text = projectDataTable.Rows[0][2].ToString();
            txbDuration.Text = projectDataTable.Rows[0][3].ToString();
            txbAllocatedBranch.Text = projectDataTable.Rows[0][4].ToString();
            txbCreateDate.Text = Convert.ToDateTime(projectDataTable.Rows[0][5].ToString()).ToString("d-MMM-yyyy");
            txbStartDate.Text = Convert.ToDateTime(projectDataTable.Rows[0][6].ToString()).ToString("d-MMM-yyyy");
            txbExpectedEndDate.Text = Convert.ToDateTime(projectDataTable.Rows[0][7].ToString()).ToString("d-MMM-yyyy");
            txbEndDate.Text = Convert.ToDateTime(projectDataTable.Rows[0][8].ToString()).ToString("d-MMM-yyyy");
            txbPercentageCompleted.Text = projectDataTable.Rows[0][9].ToString();
            txbStatus.Text = projectDataTable.Rows[0][10].ToString();


            DataTable usersDataTable = queryController.getProjectTeamDetails(projectId);
            txbAllocatedTeam.Text = "";
            for (int i = 0; i< usersDataTable.Rows.Count;i++)
            {
                txbAllocatedTeam.Text += usersDataTable.Rows[i][0].ToString() + " - " + usersDataTable.Rows[i][1].ToString() + " " + usersDataTable.Rows[i][2].ToString()+"\n";
            }

        }
    }
}
