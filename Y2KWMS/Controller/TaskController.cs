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
    class TaskController
    {
        QueryController queryController;
        Helper helper;
        public string getId()
        {
            queryController = new QueryController();
            string id = queryController.getLastId("task");
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
        public DataTable getTeamMembers(int id)
        {
            queryController = new QueryController();
            helper = new Helper();

            string[] fields = { "id", "teamId" };
            DataTable dt;
            string[] status = { helper.projectStatus[0], helper.projectStatus[1] };
            dt = queryController.search("projectTeam", "projectId", id.ToString(), fields);
            string[] teamIds = new string[dt.Rows.Count];
            for(int i = 0; i<dt.Rows.Count; i++)
            {
                teamIds[i] = dt.Rows[i][1].ToString();
               
            }
            string[] teamMemberIdFields = { "id", "teamMemberId" };
            dt = queryController.searchIn("teamMember", "teamId", teamIds, teamMemberIdFields);
            string[] teamMemberId = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                teamMemberId[i] = dt.Rows[i][1].ToString();

            }
            string[] teamMemberDetails = { "id", "firstname","lastname" };
            dt = queryController.searchIn("users", "id", teamMemberId, teamMemberDetails);
            return dt;
        }

        public void create(Model.Task task)
        {
            string[] field = { "title", "description", "duration", "projectId", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status" };
            string[] values = { task.Title, task.Description, task.Duration.ToString(), task.ProjectId.ToString(), task.CreatedDate.ToString(), task.StartDate.ToString(), task.ExpectedEndDate.ToString(), task.EndDate.ToString(), task.PercentageComplete.ToString(), task.Status };

            queryController = new QueryController();
            queryController.save("task", field, values);

            string teamMembersName = null;
            DataTable teamId;
            DataTable teamLeaderId;

            string[] teamMemberMail = new string[task.TeamMemberId.Length];


            teamId = queryController.findField("projectTeam", task.ProjectId.ToString(), "teamId","projectId");
            string[] teamIdArray = new string[teamId.Rows.Count];
            for (int i = 0; i<teamId.Rows.Count;i++)
            {
                teamIdArray[i] = teamId.Rows[i][0].ToString();
            }
            string[] teamFields = { "id", "teamLeader" };
            teamLeaderId = queryController.searchIn("team", "id",teamIdArray, teamFields);
            string[] teamLeaderMail = new string[teamLeaderId.Rows.Count];
            string teamMemberIdView = null;
            for (int i = 0; i < task.TeamMemberId.Length; i++)
            {
                string[] taskTeamField = { "taskId", "teamMemberId" };
                string[] taskTeamValues = { task.Id.ToString(), task.TeamMemberId[i].ToString() };
                queryController.save("taskTeam", taskTeamField, taskTeamValues, false);

                teamMemberIdView += task.TeamMemberId[i].ToString() + ",";
                 teamMemberMail[i] = queryController.findName("users", task.TeamMemberId[i].ToString(), "email");


            }
            for(int i = 0; i< teamLeaderId.Rows.Count;i++)
            {
                teamLeaderMail[i] = queryController.findName("users", teamLeaderId.Rows[i][1].ToString(), "email");
            }
            string body = "<table style='border - collapse: collapse; width: 100 %; ' border='1'><tbody><tr><td style='width: 50 %; '>Task Title</td><td style='width: 50 %; '>" + task.Title + "</td></tr><tr><td style='width: 50 %; '>Task Description</td><td style='width: 50 %;'>" + task.Description + "</td></tr><tr><td style='width: 50 %; '>Start Date</td><td style='width: 50 %; '>" + task.StartDate + "</td></tr><tr><td style='width: 50 %; '>Duration(Weeks)</td><td style='width: 50 %; '>" + task.Duration + "</td></tr><tr><td style='width: 50 %; '>Status</td><td style='width: 50 %; '>" + task.Status + "</td></tr><tr><td style='width: 50 %; '>Team Members</td><td style='width: 50 %; '>" + teamMemberIdView + "</td></tr></tbody></table>";

            queryController.sendEmail(teamMemberMail, "New Task Assigned to You", body);
            queryController.sendEmail(teamLeaderMail, "New Task Assigned to Your Team or Created to Your Project", body);



        }

        public DataTable load()
        {
            queryController = new QueryController();
            return queryController.viewTask();
        }

        public DataTable search(string searchKey, string searchValue)
        {
            queryController = new QueryController();
            string[] fields = { "id", "title", "description", "duration", "projectId", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status" };

            return queryController.searchTask("task", searchKey, searchValue, fields);
        }

        public void view(string id, User user)
        {
            View.PopupWindow popupWindow = new View.PopupWindow(user);
            popupWindow.Show();
            popupWindow.PagesNavigation.Navigate(new View.Pages.Task.Show(int.Parse(id)));
        }

        public void edit(string id, User user)
        {
            View.PopupWindow popupWindow = new View.PopupWindow(user);
            popupWindow.Show();
            popupWindow.PagesNavigation.Navigate(new View.Pages.Task.Create(id,user));
        }

        public Model.Task editData(string id)
        {
            queryController = new QueryController();
            string[] fields = { "id", "title", "description", "duration", "projectId", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status" };
            Model.Task task = new Model.Task();
            DBConnection con = new DBConnection();
            try
            {
                con.connection.Open();
                SqlDataReader sdr = queryController.find("task", id).ExecuteReader();

                while (sdr.Read())
                {
                    task.Title = sdr[1].ToString();
                    task.Description = sdr[2].ToString();
                    task.Duration = int.Parse(sdr[3].ToString());
                    task.ProjectId = int.Parse(sdr[4].ToString());
                    task.CreatedDate = Convert.ToDateTime(sdr[5].ToString());
                    task.StartDate = Convert.ToDateTime(sdr[6].ToString());
                    task.ExpectedEndDate = Convert.ToDateTime(sdr[7].ToString());
                    task.EndDate = Convert.ToDateTime(sdr[8].ToString());
                    task.PercentageComplete = float.Parse(sdr[9].ToString());
                    task.Status = sdr[10].ToString();

                }

                DataTable dt = queryController.findField("taskTeam", id, "teamMemberId", "taskId");
                int[] teamId = new int[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    teamId[i] = int.Parse(dt.Rows[i][0].ToString());
                }

                /*string[] teamFields = { "id", "name" };
                dt = queryController.searchInInteger("team", "id", teamId, teamFields);
                int[] teamData = new int[dt.Rows.Count];
                string[] teamDataName = new string[dt.Rows.Count];

                //MessageBox.Show(dt.Rows.Count.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    teamData[i] = int.Parse(dt.Rows[i][0].ToString());
                    teamDataName[i] = dt.Rows[i][1].ToString();

                }*/
                string[] teamFields = { "id", "firstname","lastname" };

                dt = queryController.searchInInteger("users", "id", teamId, teamFields);
                string[] teamDataName = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    teamDataName[i] = dt.Rows[i][1].ToString() + " - " + dt.Rows[i][2].ToString();

                }
                task.TeamMemberId = teamId;
                task.TeamName = teamDataName;
                con.connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\nCheck the Data");
            }

            return task;
        }

        public void update(Model.Task task)
        {
            queryController = new QueryController();

            string[] field = { "title", "description", "duration", "projectId", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status" };

            string[] values = { task.Title, task.Description, task.Duration.ToString(), task.ProjectId.ToString(), task.CreatedDate.ToString(), task.StartDate.ToString(), task.ExpectedEndDate.ToString(), task.EndDate.ToString(), task.PercentageComplete.ToString(), task.Status };

            queryController.update("task", field, values, task.Id);

            string teamName = null;
            string teamLeaderId = null;
          

            queryController.deleteMultiple("taskTeam", task.Id, "taskId");

            string[] teamMemberField = { "teamId" };
            DataTable dt = queryController.searchInInteger("teamMember", "teamId", task.TeamMemberId, teamMemberField);
            int[] teamIds = new int[dt.Rows.Count];
            string[] teamLeaderMail = new string[teamIds.Length];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                teamIds[i] = int.Parse(dt.Rows[i][0].ToString());
                
            }

            int [] finalTeamIds = teamIds.Distinct().ToArray();

            for (int i = 0; i < finalTeamIds.Length; i++)
            {
                
                teamLeaderId = queryController.findName("team", finalTeamIds[i].ToString(), "teamLeader");
                teamLeaderMail[i] = queryController.findName("users", teamLeaderId, "email");
                

            }

            string[] teamMemberMail = new string[task.TeamMemberId.Length];
            for (int i = 0; i < task.TeamMemberId.Length; i++)
            {
                string[] taskTeamField = { "taskId", "teamMemberId" };
                string[] taskTeamValues = { task.Id.ToString(), task.TeamMemberId[i].ToString() };
                queryController.save("taskTeam", taskTeamField, taskTeamValues, false);

                teamMemberMail[i] = queryController.findName("users", task.TeamMemberId[i].ToString(), "email");
                //  MessageBox.Show(teamMemberMail[i]);
                


            }
            // string[] teamLeaderMail = new string[teamLeaderId.Rows.Count];
            /*string teamMemberIdView = null;
            for (int i = 0; i < task.TeamMemberId.Length; i++)
            {

                teamMemberIdView += task.TeamMemberId[i].ToString() + ",";
                //teamMemberMail[i] = queryController.findName("users", task.TeamMemberId[i].ToString(), "email");


            }
            for (int i = 0; i < teamLeaderId.Rows.Count; i++)
            {
                teamLeaderMail[i] = queryController.findName("users", teamLeaderId.Rows[i][1].ToString(), "email");
            }*/
            string body = "<table style='border - collapse: collapse; width: 100 %; ' border='1'><tbody><tr><td style='width: 50 %; '>Task Title</td><td style='width: 50 %; '>" + task.Title + "</td></tr><tr><td style='width: 50 %; '>Task Description</td><td style='width: 50 %;'>" + task.Description + "</td></tr><tr><td style='width: 50 %; '>Start Date</td><td style='width: 50 %; '>" + task.StartDate + "</td></tr><tr><td style='width: 50 %; '>Duration(Weeks)</td><td style='width: 50 %; '>" + task.Duration + "</td></tr><tr><td style='width: 50 %; '>Status</td><td style='width: 50 %; '>" + task.Status + "</td></tr><tr><td style='width: 50 %; '>Team Members</td><td style='width: 50 %; '>" + string.Join(",", task.TeamMemberId) + "</td></tr></tbody></table>";
            //MessageBox.Show(teamMemberMail[0].ToString());
            //MessageBox.Show(teamLeaderMail[0].ToString());

            queryController.sendEmail(teamMemberMail, "Task Assigned to You was Updated", body);
            queryController.sendEmail(teamLeaderMail, "Task Assigned to Your Team or to Your Project was updated", body);




        }
    }
}
