using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Y2KWMS.Model;

namespace Y2KWMS.Controller
{
    class SubTaskController
    {
        QueryController queryController;
        Helper helper;

        public string getId()
        {
            queryController = new QueryController();
            string id = queryController.getLastId("subTask");
            return id;
        }

        public DataTable getProject()
        {
            queryController = new QueryController();
            helper = new Helper();

            string[] fields = { "id", "title" };
            DataTable dt;
            string[] status = { helper.projectStatus[0], helper.projectStatus[1] };
            dt = queryController.searchIn("project", "status", status, fields);

            return dt;
        }
        public DataTable getTask(string projectId)
        {
            queryController = new QueryController();
            helper = new Helper();

            string[] fields = { "id", "title" };
            DataTable dt;
            string[] status = { helper.projectStatus[0], helper.projectStatus[1] };
            dt = queryController.searchIn("task", "status", status, fields, "projectId = " + projectId);

            return dt;
        }
        public DataTable getTeamMembers(string taskId)
        {
            queryController = new QueryController();

            string[] fields = { "id", "teamMemberId" };
            DataTable dt;
          
            dt = queryController.searchInteger("taskTeam", "taskId", int.Parse(taskId), fields);
            string[] teamMembers = new string[dt.Rows.Count];
           // MessageBox.Show(teamMembers[0]);
            for (int i = 0; i< dt.Rows.Count; i++)
            {
                teamMembers[i] = dt.Rows[i][1].ToString();
            }
           
            string [] userFields = { "id", "firstname", "lastname" };
            dt = queryController.searchIn("users", "id", teamMembers, userFields);
            return dt;
        }

        public void create(Model.SubTask subTask)
        {
            string[] field = { "title", "description", "duration", "projectId", "taskId","teamMemberId", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status" };

            string[] values = { subTask.Title, subTask.Description, subTask.Duration.ToString(), subTask.ProjectId.ToString(), subTask.TaskId.ToString(), subTask.TeamMemberId.ToString(), subTask.CreatedDate.ToString(), subTask.StartDate.ToString(), subTask.ExpectedEndDate.ToString(), subTask.EndDate.ToString(), subTask.PercentageComplete.ToString(), subTask.Status };

            queryController = new QueryController();
            queryController.save("subTask", field, values);

            //string teamMembersName = null;
            DataTable teamId;
            DataTable teamLeaderId;

           // string teamMemberMail = new string[];


            teamId = queryController.findField("projectTeam", subTask.ProjectId.ToString(), "teamId", "projectId");

            string[] teamIdArray = new string[teamId.Rows.Count];

            for (int i = 0; i < teamId.Rows.Count; i++)
            {
                teamIdArray[i] = teamId.Rows[i][0].ToString();
            }
            string[] teamFields = { "id", "teamLeader" };
            teamLeaderId = queryController.searchIn("team", "id", teamIdArray, teamFields);
            string[] teamLeaderMail = new string[teamLeaderId.Rows.Count];
            
           
            for (int i = 0; i < teamLeaderId.Rows.Count; i++)
            {
                teamLeaderMail[i] = queryController.findName("users", teamLeaderId.Rows[i][1].ToString(), "email");
            }

            string [] teamMemberMail = { queryController.findName("users", subTask.TeamMemberId.ToString(), "email") };

            string body = "<table style='border - collapse: collapse; width: 100 %; ' border='1'><tbody><tr><td style='width: 50 %; '>Sub Task Title</td><td style='width: 50 %; '>" + subTask.Title + "</td></tr><tr><td style='width: 50 %; '>Sub Task Description</td><td style='width: 50 %;'>" + subTask.Description + "</td></tr><tr><td style='width: 50 %; '>Start Date</td><td style='width: 50 %; '>" + subTask.StartDate + "</td></tr><tr><td style='width: 50 %; '>Duration(Days)</td><td style='width: 50 %; '>" + subTask.Duration + "</td></tr><tr><td style='width: 50 %; '>Status</td><td style='width: 50 %; '>" + subTask.Status + "</td></tr><tr><td style='width: 50 %; '>Team Members</td><td style='width: 50 %; '>" + subTask.TeamMemberId + "</td></tr></tbody></table>";

            queryController.sendEmail(teamMemberMail, "New Task Assigned to You", body);
            queryController.sendEmail(teamLeaderMail, "New Task Assigned to Your Team or Created to Your Project", body);

        }
        public DataTable load()
        {
            queryController = new QueryController();
            return queryController.viewSubTask();
        }

        public DataTable search(string searchKey, string searchValue)
        {
            queryController = new QueryController();
            string[] fields = { "id", "title", "description", "duration", "projectId", "taskId", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status" };

            return queryController.searchSubTask("subTask", searchKey, searchValue, fields);
        }

        public void view(string id, User user)
        {
           View.PopupWindow popupWindow = new View.PopupWindow(user);
            popupWindow.Show();
            popupWindow.PagesNavigation.Navigate(new View.Pages.SubTask.Show(int.Parse(id)));
        }

        public void edit(string id, User user)
        {
           View.PopupWindow popupWindow = new View.PopupWindow(user);
            popupWindow.Show();
            popupWindow.PagesNavigation.Navigate(new View.Pages.SubTask.Create(id,user));
        }

        public SubTask editData(string id)
        {
            queryController = new QueryController();
            //string[] fields = { "id", "title", "description", "duration", "projectId", "taskId", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status", "teamMemberId" };
            SubTask subTask = new SubTask();
            DBConnection con = new DBConnection();
            try
            {
                con.connection.Open();
                SqlDataReader sdr = queryController.find("subTask", id).ExecuteReader();

                while (sdr.Read())
                {
                    subTask.Title = sdr[1].ToString();
                    subTask.Description = sdr[2].ToString();
                    subTask.Duration = int.Parse(sdr[3].ToString());
                    subTask.ProjectId = int.Parse(sdr[4].ToString());
                    subTask.TaskId = int.Parse(sdr[5].ToString());
                    subTask.TeamMemberId = int.Parse(sdr[6].ToString());
                    subTask.CreatedDate = Convert.ToDateTime(sdr[7].ToString());
                    subTask.StartDate = Convert.ToDateTime(sdr[8].ToString());
                    subTask.ExpectedEndDate = Convert.ToDateTime(sdr[9].ToString());
                    subTask.EndDate = Convert.ToDateTime(sdr[10].ToString());
                    subTask.PercentageComplete = float.Parse(sdr[11].ToString());
                    subTask.Status = sdr[12].ToString();


                }

                
                string[] teamFields = { "id", "firstname", "lastname" };
                DataTable dt;
                dt = queryController.search("users", "id", subTask.TeamMemberId.ToString(), teamFields);
                string teamDataName = null;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    teamDataName = dt.Rows[i][1].ToString() + " - " + dt.Rows[i][2].ToString();

                }
                subTask.TeamMemberId = subTask.TeamMemberId;
                subTask.TeamName = teamDataName;
                con.connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\nCheck the Data");
            }

            return subTask;
        }

        public void update(SubTask subTask)
        {
            queryController = new QueryController();

            string[] field = { "title", "description", "duration", "projectId", "taskId", "teamMemberId", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status" };

            string[] values = { subTask.Title, subTask.Description, subTask.Duration.ToString(), subTask.ProjectId.ToString(), subTask.TaskId.ToString(), subTask.TeamMemberId.ToString(), subTask.CreatedDate.ToString(), subTask.StartDate.ToString(), subTask.ExpectedEndDate.ToString(), subTask.EndDate.ToString(), subTask.PercentageComplete.ToString(), subTask.Status };

            queryController.update("subTask", field, values, subTask.Id);






            DataTable teamId;
            DataTable teamLeaderId;

            teamId = queryController.findField("projectTeam", subTask.ProjectId.ToString(), "teamId", "projectId");

            string[] teamIdArray = new string[teamId.Rows.Count];

            for (int i = 0; i < teamId.Rows.Count; i++)
            {
                teamIdArray[i] = teamId.Rows[i][0].ToString();
            }
            string[] teamFields = { "id", "teamLeader" };
            teamLeaderId = queryController.searchIn("team", "id", teamIdArray, teamFields);

            string[] teamLeaderMail = new string[teamLeaderId.Rows.Count];


            for (int i = 0; i < teamLeaderId.Rows.Count; i++)
            {
                teamLeaderMail[i] = queryController.findName("users", teamLeaderId.Rows[i][1].ToString(), "email");
            }

            string[] teamMemberMail = { queryController.findName("users", subTask.TeamMemberId.ToString(), "email") };

            string body = "<table style='border - collapse: collapse; width: 100 %; ' border='1'><tbody><tr><td style='width: 50 %; '>Sub Task Title</td><td style='width: 50 %; '>" + subTask.Title + "</td></tr><tr><td style='width: 50 %; '>Sub Task Description</td><td style='width: 50 %;'>" + subTask.Description + "</td></tr><tr><td style='width: 50 %; '>Start Date</td><td style='width: 50 %; '>" + subTask.StartDate + "</td></tr><tr><td style='width: 50 %; '>Duration(Days)</td><td style='width: 50 %; '>" + subTask.Duration + "</td></tr><tr><td style='width: 50 %; '>Status</td><td style='width: 50 %; '>" + subTask.Status + "</td></tr><tr><td style='width: 50 %; '>Team Members</td><td style='width: 50 %; '>" + subTask.TeamMemberId + "</td></tr></tbody></table>";

            queryController.sendEmail(teamMemberMail, "Sub Task Assigned to You was Updated", body);
            queryController.sendEmail(teamLeaderMail, "Sub Task Assigned to Your Team or Created to Your Project was Updated", body);

        }
    }
}
