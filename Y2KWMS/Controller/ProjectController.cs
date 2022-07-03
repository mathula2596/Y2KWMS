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
    class ProjectController
    {
        QueryController queryController;
        public string getId()
        {
            queryController = new QueryController();
            string id = queryController.getLastId("project");
            return id;
        } 
        public DataTable getTeam(string branch)
        {
            queryController = new QueryController();
            string[] fields = { "id", "name" };
            DataTable dt;
            if (branch != null)
            {
                dt = queryController.search("team","branch",branch,fields);
            }
            else
            {

                dt = queryController.search("team", "branch", null, fields);
               
            }

            return dt;
        }

        public void create(Model.Project project)
        {
            string[] field = { "title", "description", "duration", "branch","createdDate","startDate","expectedEndDate","endDate","percentageComplete","status" };
            string[] values = { project.Title,project.Description,project.Duration.ToString(),project.Branch,project.CreatedDate.ToString(),project.StartDate.ToString(),project.ExpectedEndDate.ToString(),project.EndDate.ToString(),project.PercentageComplete.ToString(),project.Status};

            queryController = new QueryController();
            queryController.save("project", field, values);

            string teamName = null;
            string teamLeaderId = null;
            string [] teamLeaderMail = new string[project.TeamId.Length];

            for (int i = 0; i < project.TeamId.Length; i++)
            {
                string[] projectTeamField = { "projectId", "teamId"  };
                string[] projectTeamValues = { project.Id.ToString(), project.TeamId[i].ToString() };
                queryController.save("projectTeam", projectTeamField, projectTeamValues, false);

                teamName += queryController.findName("team", project.TeamId[i].ToString(), "name") + ", ";
                teamLeaderId = queryController.findName("team", project.TeamId[i].ToString(), "teamLeader");
                teamLeaderMail[i] = queryController.findName("users", teamLeaderId, "email");


            }
            string body = "<table style='border - collapse: collapse; width: 100 %; ' border='1'><tbody><tr><td style='width: 50 %; '>Project Title</td><td style='width: 50 %; '>" + project.Title + "</td></tr><tr><td style='width: 50 %; '>Project Description</td><td style='width: 50 %;'>" + project.Description + "</td></tr><tr><td style='width: 50 %; '>Start Date</td><td style='width: 50 %; '>" + project.StartDate + "</td></tr><tr><td style='width: 50 %; '>Duration(Months)</td><td style='width: 50 %; '>" + project.Duration + "</td></tr><tr><td style='width: 50 %; '>Status</td><td style='width: 50 %; '>" + project.Status + "</td></tr><tr><td style='width: 50 %; '>Teams</td><td style='width: 50 %; '>" + teamName + "</td></tr></tbody></table>";

            queryController.sendEmail(teamLeaderMail, "New Project Assigned to You", body);


        }

        public DataTable load(string type,string email)
        {
            queryController = new QueryController();
            return queryController.viewProject(type,email);
        }

        public void view(string id, User user)
        {
            //View.Dashboard dashboard = new View.Dashboard();
            View.PopupWindow popupWindow = new View.PopupWindow(user);
            popupWindow.Show();
            popupWindow.PagesNavigation.Navigate(new View.Pages.Project.Show(int.Parse(id)));
        }

        public void edit(string id, User user)
        {
            View.PopupWindow popupWindow = new View.PopupWindow(user);
            popupWindow.Show();
            popupWindow.PagesNavigation.Navigate(new View.Pages.Project.Create(id,user));
        }

        public Project editData(string id)
        {
            queryController = new QueryController();
            string[] fields = { "id","title", "description", "duration", "branch", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status" };
            Project project = new Project();
            DBConnection con = new DBConnection();
            try
            {
                con.connection.Open();
                SqlDataReader sdr = queryController.find("project", id).ExecuteReader();

                while (sdr.Read())
                {
                    project.Title = sdr[1].ToString();
                    project.Description = sdr[2].ToString();
                    project.Duration = int.Parse(sdr[3].ToString());
                    project.PercentageComplete = float.Parse(sdr[9].ToString());
                    project.CreatedDate = Convert.ToDateTime(sdr[5].ToString());
                    project.StartDate = Convert.ToDateTime(sdr[6].ToString());
                    project.ExpectedEndDate = Convert.ToDateTime(sdr[7].ToString());
                    project.EndDate = Convert.ToDateTime(sdr[8].ToString());
                    project.Branch = sdr[4].ToString();
                    project.Status = sdr[10].ToString();

                }

                DataTable dt = queryController.findField("projectTeam", id, "teamId", "projectId");
                int[] teamId = new int[dt.Rows.Count];
                for(int i = 0; i < dt.Rows.Count;i++)
                {
                    teamId[i] = int.Parse(dt.Rows[i][0].ToString());
                }
               
                string[] teamFields = { "id", "name" };
                dt = queryController.searchInInteger("team", "id", teamId, teamFields);
                int[] teamData = new int[dt.Rows.Count];
                string[] teamDataName = new string[dt.Rows.Count];

                //MessageBox.Show(dt.Rows.Count.ToString());
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    teamData[i] = int.Parse(dt.Rows[i][0].ToString());
                    teamDataName[i] = dt.Rows[i][1].ToString();

                }
                project.TeamId = teamData;
                project.TeamName = teamDataName;
                con.connection.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\nCheck the Data");
            }

            return project;
        }
        public void update(Project project)
        {
            queryController = new QueryController();
           
            string[] fields = { "title", "description", "duration", "branch", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status" };

            string[] values = { project.Title, project.Description, project.Duration.ToString(), project.Branch, project.CreatedDate.ToString(), project.StartDate.ToString(), project.ExpectedEndDate.ToString(), project.EndDate.ToString(), project.PercentageComplete.ToString(), project.Status };

            queryController.update("project", fields, values, project.Id);

            string teamName = null;
            string teamLeaderId = null;
            string[] teamLeaderMail = new string[project.TeamId.Length];
            queryController.deleteMultiple("projectTeam", project.Id, "projectId");

            for (int i = 0; i < project.TeamId.Length; i++)
            {
                string[] projectTeamField = { "projectId", "teamId" };
                string[] projectTeamValues = { project.Id.ToString(), project.TeamId[i].ToString() };

                queryController.save("projectTeam", projectTeamField, projectTeamValues, false);

                teamName += queryController.findName("team", project.TeamId[i].ToString(), "name") + ", ";
                teamLeaderId = queryController.findName("team", project.TeamId[i].ToString(), "teamLeader");
                teamLeaderMail[i] = queryController.findName("users", teamLeaderId, "email");


            }
            string body = "<table style='border - collapse: collapse; width: 100 %; ' border='1'><tbody><tr><td style='width: 50 %; '>Project Title</td><td style='width: 50 %; '>" + project.Title + "</td></tr><tr><td style='width: 50 %; '>Project Description</td><td style='width: 50 %;'>" + project.Description + "</td></tr><tr><td style='width: 50 %; '>Start Date</td><td style='width: 50 %; '>" + project.StartDate + "</td></tr><tr><td style='width: 50 %; '>Duration(Months)</td><td style='width: 50 %; '>" + project.Duration + "</td></tr><tr><td style='width: 50 %; '>Status</td><td style='width: 50 %; '>" + project.Status + "</td></tr><tr><td style='width: 50 %; '>Teams</td><td style='width: 50 %; '>" + teamName + "</td></tr></tbody></table>";

            queryController.sendEmail(teamLeaderMail, "Project Assigned to You was updated", body);




        }

        public DataTable search(string searchKey, string searchValue, string type, string email)
        {
            queryController = new QueryController();
            string[] fields = {"id", "title", "description", "duration", "branch", "createdDate", "startDate", "expectedEndDate", "endDate", "percentageComplete", "status" };
           
            return queryController.searchProject("project", searchKey, searchValue, type, email,fields);
        }

    }
}
