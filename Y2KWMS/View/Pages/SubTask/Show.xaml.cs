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

namespace Y2KWMS.View.Pages.SubTask
{
    /// <summary>
    /// Interaction logic for Show.xaml
    /// </summary>
    public partial class Show : Page
    {
        QueryController queryController;
        public Show(int subTaskId)
        {
            InitializeComponent();
            queryController = new QueryController();
            getSubTaskDetails(subTaskId);
        }

        private void getSubTaskDetails(int subTaskId)
        {
            DataTable taskDataTable = queryController.getSubTaskDetails(subTaskId);
            txbId.Text = taskDataTable.Rows[0][0].ToString();
            txbTitle.Text = taskDataTable.Rows[0][1].ToString();
            txbDesctiption.Text = taskDataTable.Rows[0][2].ToString();
            txbDuration.Text = taskDataTable.Rows[0][3].ToString();
            txbProject.Text = taskDataTable.Rows[0][4].ToString();
            txbTask.Text = taskDataTable.Rows[0][5].ToString();

            txbCreateDate.Text = Convert.ToDateTime(taskDataTable.Rows[0][6].ToString()).ToString("d-MMM-yyyy");
            txbStartDate.Text = Convert.ToDateTime(taskDataTable.Rows[0][7].ToString()).ToString("d-MMM-yyyy");
            txbExpectedEndDate.Text = Convert.ToDateTime(taskDataTable.Rows[0][8].ToString()).ToString("d-MMM-yyyy");
            txbEndDate.Text = Convert.ToDateTime(taskDataTable.Rows[0][9].ToString()).ToString("d-MMM-yyyy");
            txbPercentageCompleted.Text = taskDataTable.Rows[0][10].ToString();
            txbStatus.Text = taskDataTable.Rows[0][11].ToString();

            DataTable teamId;
            DataTable teamLeaderId;


            teamId = queryController.findField("projectTeam", taskDataTable.Rows[0][13].ToString(), "teamId", "projectId");

            string[] teamIdArray = new string[teamId.Rows.Count];

            for (int i = 0; i < teamId.Rows.Count; i++)
            {
                teamIdArray[i] = teamId.Rows[i][0].ToString();

            }

            string[] teamFields = { "id", "teamLeader" };

            teamLeaderId = queryController.searchIn("team", "id", teamIdArray, teamFields);

            DataTable teamLeaderDataTable;

            string[] teamUserFields = { "id", "firstname", "lastname" };

            txbAllocatedTeam.Text = "";

            for (int i = 0; i < teamLeaderId.Rows.Count; i++)
            {
                teamLeaderDataTable = queryController.search("users", "id", teamLeaderId.Rows[i][1].ToString(), teamUserFields);
                for (int x = 0; x < teamLeaderDataTable.Rows.Count; x++)
                {
                    txbAllocatedTeam.Text += teamLeaderDataTable.Rows[x][0].ToString() + " - " + teamLeaderDataTable.Rows[x][1].ToString() + " " + teamLeaderDataTable.Rows[x][2].ToString() + "\n";

                }
            }

            DataTable dt;
            dt = queryController.search("users", "id", taskDataTable.Rows[0][12].ToString(), teamUserFields);

           
            

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                txbAllocatedTeam.Text += dt.Rows[i][0].ToString() + " - " + dt.Rows[i][1].ToString() + " " + dt.Rows[i][2].ToString() + "\n";

            }


          
          

            //string[] teamLeaderMail = new string[teamLeaderId.Rows.Count];



          



           

        }
    }
}
