using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Y2KWMS.Controller
{
    class TeamController
    {
        QueryController queryController;
        public DataTable getTeamLeaders(string branch)
        {
            queryController = new QueryController();
            string[] fields = { "id", "firstname", "lastname"};
            DataTable dt;
            if (branch != null)
            {
                dt = queryController.searchNewTeamLeader(branch);
                //MessageBox.Show(dt.Rows.Count.ToString());
            }
            else
            {

                dt = queryController.searchNewTeamLeader(null);
                //dt = queryController.search("users", "type", "TeamLeader", fields);
                //MessageBox.Show(dt.Rows.Count.ToString());
            }
            
            return dt;
        }

        public DataTable getTeamMembers(string branch)
        {
            queryController = new QueryController();
            //string[] fields = { "id", "firstname", "lastname" };
            DataTable dt;
            if (branch != null)
            {
                dt = queryController.searchNewTeamMember(branch);
            }
            else
            {
                dt = queryController.searchNewTeamMember(null);
                //dt = queryController.search("users", "type", "TeamMember", fields);
            }
            //MessageBox.Show(dt.Rows.Count.ToString());
            return dt;
        }

        public string getId()
        {
            queryController = new QueryController();
            string id = queryController.getLastId("team");
            return id;
        }

        public void create(Model.Team team)
        {
            string[] field = { "name", "branch", "teamLeader", "status" };
            string[] values = { team.Name, team.Branch, team.TeamLeader.ToString(),team.Status};

            queryController = new QueryController();
            queryController.save("team", field, values);

            

            for(int i=0; i<team.TeamMember.Length;i++)
            {
                string[] teamMemberField = { "teamId", "teamMemberId" };
                string[] teamMemberValues = { team.Id.ToString(), team.TeamMember[i].ToString() };
                queryController.save("teamMember", teamMemberField, teamMemberValues,false);
            }
        }

        public DataTable load()
        {
            queryController = new QueryController();
            //string[] fields = { "id", "firstname", "lastname", "phone", "address", "email", "branch", "type", "status" };
            return queryController.viewTeam();
        }

    }
}
