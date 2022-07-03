using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Y2KWMS.Model;
using Y2KWMS.View;

namespace Y2KWMS.Controller
{
    public class QueryController
    {
        DBConnection con;
        public static TextInfo textFormatter = new CultureInfo("en-UK", false).TextInfo;
        SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt;
       
        public QueryController()
        {
            con = new DBConnection();
            try
            {
                con.connection.Open();
                con.connection.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message,"DB Error",MessageBoxButton.OK,MessageBoxImage.Error);
                Application.Current.Shutdown();
                throw new Exception(e.Message+"\n"+"Please check the DB connection");
                //Application.Current.Shutdown();
            }
            
        }
        public void save(string table_name, string[] fields, string[] values, bool message = true)
        {
            con = new DBConnection(); 

            var field = String.Join(",", fields);

            string concatParameterSymbol = null;

            concatParameterSymbol = field.Replace(",", ",@");
            concatParameterSymbol = String.Concat("@", concatParameterSymbol);

            string[] parameterField = concatParameterSymbol.Split(',');

            cmd = new SqlCommand("Insert into " + table_name + " (" + field + ")values (" + concatParameterSymbol + ")", con.connection);
            for (int i = 0; i < values.Length; i++)
            {
                cmd.Parameters.AddWithValue(parameterField[i], values[i]);
            }
            con.connection.Open();
            int result = cmd.ExecuteNonQuery();
            if (message)
            {
                if (result >= 1)
                {
                    MessageBox.Show("Data Added Successfully!", "Success Message", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Data Added Unsuccessfully!", "Failed Message", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
           
            con.connection.Close();
            
        }

        public string getLastId(string table_name)
        {
            con = new DBConnection();
            
            cmd = new SqlCommand("SELECT ISNULL(MAX(id),999)+1 FROM "+table_name+"", con.connection);
            con.connection.Open();
            string id = cmd.ExecuteScalar().ToString();
            con.connection.Close();

            return id;
           
        }

        public DataTable view(string table_name, string[] fields = null)
        {
            con = new DBConnection();
            //https://www.codeproject.com/Questions/5161216/Make-method-with-string-with-optional-default-valu
            fields = fields ?? new string[] { "*" };
            var field = String.Join(",", fields);
            da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name, con.connection);
            con.connection.Open();
            dt = new DataTable();
            
            if(fields.Length >0)
            {
                for(int i = 0; i<fields.Length; i++)
                {
                    dt.Columns.Add(textFormatter.ToTitleCase(fields[i]), typeof(string));
                }
            }
           
            da.Fill(dt);
            con.connection.Close();

            return dt;
        }

        public DataTable search(string table_name, string searchKey, string searchValue, string[] fields = null)
        {
            con = new DBConnection();

            fields = fields ?? new string[] { "*" };
            var field = String.Join(",", fields);
            if (searchValue != null)
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name + " where " + searchKey + " like " + "'%" + searchValue + "%'", con.connection);
            }
            else
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name, con.connection);
            }
            
            con.connection.Open();
            dt = new DataTable();

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    dt.Columns.Add(textFormatter.ToTitleCase(fields[i]), typeof(string));
                }
            }

            da.Fill(dt);
            con.connection.Close();

            return dt;
        }
        public DataTable searchProject(string table_name, string searchKey, string searchValue, string type, string email, string[] fields = null, string[] fieldsDataType = null)
        {
            con = new DBConnection();

            fields = fields ?? new string[] { "*" };
            var field = String.Join(",", fields);


            dt = new DataTable();
            dt = findFieldString("users", email, "id", "email");

            string userId = dt.Rows[0][0].ToString();

            if (searchValue != null && searchKey != "createdDate" && searchKey != "startDate" && searchKey != "expectedEndDate" && searchKey != "endDate")
            {
                if (type == "ProjectHead")
                {
                    da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name + " where " + searchKey + " like " + "'%" + searchValue + "%'", con.connection);
                }
                else if (type == "TeamLeader")
                {
                    da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        da = new SqlDataAdapter("SELECT projectId from ProjectTeam where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        string[] projectId = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            projectId[i] = dt.Rows[i][0].ToString();
                        }

                        if (projectId.Length > 0)
                        {
                            da = new SqlDataAdapter("SELECT * from project where id IN (" + string.Join(",", projectId) + ") and " + searchKey + " like " + "'%" + searchValue + "%'", con.connection);
                            dt = new DataTable();
                            da.Fill(dt);
                            //MessageBox.Show(dt.Rows.Count.ToString());
                        }
                    }
                    else
                    {
                        da = new SqlDataAdapter("SELECT teamId from teamMember where teamMemberId=" + userId, con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            da = new SqlDataAdapter("SELECT projectId from ProjectTeam where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                            dt = new DataTable();
                            da.Fill(dt);
                            string[] projectId = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                projectId[i] = dt.Rows[i][0].ToString();
                            }

                            if (projectId.Length > 0)
                            {
                                da = new SqlDataAdapter("SELECT * from project where id IN (" + string.Join(",", projectId) + ") and " + searchKey + " like " + "'%" + searchValue + "%'", con.connection);
                                dt = new DataTable();
                                da.Fill(dt);
                                MessageBox.Show(dt.Rows.Count.ToString());
                            }


                        }
                    }
                }
            }
            else if (searchValue != null && (searchKey == "createdDate" || searchKey == "startDate" || searchKey == "expectedEndDate" || searchKey == "endDate"))
            {
                DateTime search = DateTime.Parse(searchValue);


                if (type == "ProjectHead")
                {
                    da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name + " where " + searchKey + " = " + "'" + search.ToString() + "'", con.connection);
                }
                else if (type == "TeamLeader")
                {
                    da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        da = new SqlDataAdapter("SELECT projectId from ProjectTeam where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        string[] projectId = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            projectId[i] = dt.Rows[i][0].ToString();
                        }

                        if (projectId.Length > 0)
                        {
                            da = new SqlDataAdapter("SELECT * from project where id IN (" + string.Join(",", projectId) + ") and " + searchKey + " = " + "'" + search.ToString() + "'", con.connection);
                            dt = new DataTable();
                            da.Fill(dt);
                            //MessageBox.Show(dt.Rows.Count.ToString());
                        }
                    }
                    else
                    {
                        da = new SqlDataAdapter("SELECT teamId from teamMember where teamMemberId=" + userId, con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            da = new SqlDataAdapter("SELECT projectId from ProjectTeam where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                            dt = new DataTable();
                            da.Fill(dt);
                            string[] projectId = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                projectId[i] = dt.Rows[i][0].ToString();
                            }

                            if (projectId.Length > 0)
                            {
                                da = new SqlDataAdapter("SELECT * from project where id IN (" + string.Join(",", projectId) + ") and " + searchKey + " = " + "'" + search.ToString() + "'", con.connection);
                                dt = new DataTable();
                                da.Fill(dt);
                                MessageBox.Show(dt.Rows.Count.ToString());
                            }


                        }
                    }
                }
            }
            else
            {
                if (type == "ProjectHead")
                {
                    da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name, con.connection);
                }
                else if (type == "TeamLeader")
                {
                    da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        da = new SqlDataAdapter("SELECT projectId from ProjectTeam where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        string[] projectId = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            projectId[i] = dt.Rows[i][0].ToString();
                        }

                        if (projectId.Length > 0)
                        {
                            da = new SqlDataAdapter("SELECT * from project where id IN (" + string.Join(",", projectId) + ")", con.connection);
                            dt = new DataTable();
                            da.Fill(dt);
                            //MessageBox.Show(dt.Rows.Count.ToString());
                        }
                    }
                    else
                    {
                        da = new SqlDataAdapter("SELECT teamId from teamMember where teamMemberId=" + userId, con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            da = new SqlDataAdapter("SELECT projectId from ProjectTeam where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                            dt = new DataTable();
                            da.Fill(dt);
                            string[] projectId = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                projectId[i] = dt.Rows[i][0].ToString();
                            }

                            if (projectId.Length > 0)
                            {
                                da = new SqlDataAdapter("SELECT * from project where id IN (" + string.Join(",", projectId) + ")" , con.connection);
                                dt = new DataTable();
                                da.Fill(dt);
                                MessageBox.Show(dt.Rows.Count.ToString());
                            }


                        }
                    }
                }
            }

            con.connection.Close();
                con.connection.Open();
                dt = new DataTable();

                dt.Columns.Add(textFormatter.ToTitleCase("id"), typeof(string));
                dt.Columns.Add(textFormatter.ToTitleCase("title"), typeof(string));
                dt.Columns.Add(textFormatter.ToTitleCase("description"), typeof(string));
                dt.Columns.Add(textFormatter.ToTitleCase("duration"), typeof(string));
                dt.Columns.Add(textFormatter.ToTitleCase("branch"), typeof(string));
                dt.Columns.Add(textFormatter.ToTitleCase("createdDate"), typeof(DateTime));
                dt.Columns.Add(textFormatter.ToTitleCase("startDate"), typeof(DateTime));
                dt.Columns.Add(textFormatter.ToTitleCase("expectedEndDate"), typeof(DateTime));
                dt.Columns.Add(textFormatter.ToTitleCase("endDate"), typeof(DateTime));
                dt.Columns.Add(textFormatter.ToTitleCase("percentageComplete"), typeof(string));
                dt.Columns.Add(textFormatter.ToTitleCase("status"), typeof(string));


                da.Fill(dt);
                con.connection.Close();

                return dt;
            
        }
        public DataTable searchTask(string table_name, string searchKey, string searchValue, string[] fields = null, string[] fieldsDataType = null)
        {
            con = new DBConnection();

            fields = fields ?? new string[] { "*" };
            var field = String.Join(",", fields);

            string userId = Environment.GetEnvironmentVariable("id");
            string type = Environment.GetEnvironmentVariable("type");

            // MessageBox.Show(type);



            if (searchKey != null)
            {
                if(searchKey != "project")
                {
                    searchKey = "task." + searchKey;
                }
                else
                {
                    searchKey = "project.title";

                }
            }

            if (searchValue != null && searchKey != "task.createdDate" && searchKey != "task.startDate" && searchKey != "task.expectedEndDate" && searchKey != "task.endDate")
            {

                if (type == "ProjectHead")
                {
                    da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description,task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where " + searchKey + " like " + "'%" + searchValue + "%'", con.connection);
                }
                else if (type == "TeamLeader")
                {

                    da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        da = new SqlDataAdapter("SELECT teamMemberId from teamMember where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        string[] teamMemberId = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            teamMemberId[i] = dt.Rows[i][0].ToString();
                        }

                        if (teamMemberId.Length > 0)
                        {
                            da = new SqlDataAdapter("SELECT taskId from taskTeam where teamMemberId  IN (" + string.Join(",", teamMemberId) + ")", con.connection);
                            dt = new DataTable();
                            da.Fill(dt);
                            string[] taskId = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                taskId[i] = dt.Rows[i][0].ToString();
                            }

                            if (taskId.Length > 0)
                            {
                               
                                da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where task.id IN (" + string.Join(",", taskId) + ") and "+searchKey + " like " + "'%" + searchValue + "%'", con.connection);

                            }

                            //MessageBox.Show(dt.Rows.Count.ToString());
                        }


                    }
                }
                else
                {

                    da = new SqlDataAdapter("SELECT taskId from taskTeam where teamMemberId  = " + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    string[] taskId = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        taskId[i] = dt.Rows[i][0].ToString();
                    }

                    if (taskId.Length > 0)
                    {
                        da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where task.id IN (" + string.Join(",", taskId) + ") and " + searchKey + " like " + "'%" + searchValue + "%'", con.connection);

                    }


                }




            }
            else if (searchValue != null && (searchKey == "task.createdDate" || searchKey == "task.startDate" || searchKey == "task.expectedEndDate" || searchKey == "task.endDate"))
            {
                DateTime search = DateTime.Parse(searchValue);

                if (type == "ProjectHead")
                {
                  

                    da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description,task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where " + searchKey + " = " + "'" + search.ToString() + "'", con.connection);
                }
                else if (type == "TeamLeader")
                {

                    da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        da = new SqlDataAdapter("SELECT teamMemberId from teamMember where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        string[] teamMemberId = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            teamMemberId[i] = dt.Rows[i][0].ToString();
                        }

                        if (teamMemberId.Length > 0)
                        {
                            da = new SqlDataAdapter("SELECT taskId from taskTeam where teamMemberId  IN (" + string.Join(",", teamMemberId) + ")", con.connection);
                            dt = new DataTable();
                            da.Fill(dt);
                            string[] taskId = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                taskId[i] = dt.Rows[i][0].ToString();
                            }

                            if (taskId.Length > 0)
                            {
                               

                                da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where task.id IN (" + string.Join(",", taskId) + ") and " + searchKey + " = " + "'" + search.ToString() + "'", con.connection);

                            }

                        }


                    }
                }
                else
                {

                    da = new SqlDataAdapter("SELECT taskId from taskTeam where teamMemberId  = " + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    string[] taskId = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        taskId[i] = dt.Rows[i][0].ToString();
                    }

                    if (taskId.Length > 0)
                    {
                        da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where task.id IN (" + string.Join(",", taskId) + ") and " + searchKey + " = " + "'" + search.ToString() + "'", con.connection);

                    }


                }
               
            }
            else
            {
                if (type == "ProjectHead")
                {
                    da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId", con.connection);
                }
                else if (type == "TeamLeader")
                {

                    da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        da = new SqlDataAdapter("SELECT teamMemberId from teamMember where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        string[] teamMemberId = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            teamMemberId[i] = dt.Rows[i][0].ToString();
                        }

                        if (teamMemberId.Length > 0)
                        {
                            da = new SqlDataAdapter("SELECT taskId from taskTeam where teamMemberId  IN (" + string.Join(",", teamMemberId) + ")", con.connection);
                            dt = new DataTable();
                            da.Fill(dt);
                            string[] taskId = new string[dt.Rows.Count];
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                taskId[i] = dt.Rows[i][0].ToString();
                            }

                            if (taskId.Length > 0)
                            {
                                da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where task.id IN (" + string.Join(",", taskId) + ")", con.connection);

                            }

                        }


                    }
                }
                else
                {

                    da = new SqlDataAdapter("SELECT taskId from taskTeam where teamMemberId  = " + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    string[] taskId = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        taskId[i] = dt.Rows[i][0].ToString();
                    }

                    if (taskId.Length > 0)
                    {
                        da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where task.id IN (" + string.Join(",", taskId) + ")", con.connection);

                    }


                }
            }

            con.connection.Open();
            dt = new DataTable();

            dt.Columns.Add(textFormatter.ToTitleCase("id"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("projectId"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("title"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("description"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("duration"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("branch"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("createdDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("startDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("expectedEndDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("endDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("percentageComplete"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("status"), typeof(string));


            da.Fill(dt);
            con.connection.Close();

            return dt;
        }

        public DataTable searchSubTask(string table_name, string searchKey, string searchValue, string[] fields = null, string[] fieldsDataType = null)
        {
            con = new DBConnection();
             string userId = Environment.GetEnvironmentVariable("id");
            string type = Environment.GetEnvironmentVariable("type");

            fields = fields ?? new string[] { "*" };
            var field = String.Join(",", fields);

            if (searchKey != null)
            {
                if (searchKey != "project" && searchKey != "task")
                {
                    searchKey = "subTask." + searchKey;
                }
                else if(searchKey == "task")
                {
                    searchKey = "task.title";

                }
                else
                {
                    searchKey = "project.title";
                }
            }



            if (searchValue != null && searchKey != "subTask.createdDate" && searchKey != "subTask.startDate" && searchKey != "subTask.expectedEndDate" && searchKey != "subTask.endDate")
            {

                if (type == "ProjectHead")
                {
                    da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where " + searchKey + " like " + "'%" + searchValue + "%'", con.connection);
                }
                else if (type == "TeamLeader")
                {

                    da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        da = new SqlDataAdapter("SELECT teamMemberId from teamMember where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        string[] teamMemberId = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            teamMemberId[i] = dt.Rows[i][0].ToString();
                        }

                        if (teamMemberId.Length > 0)
                        {

                            da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where subTask.teamMemberId IN (" + string.Join(",", teamMemberId) + ") and " + searchKey + " like " + "'%" + searchValue + "%'", con.connection);

                            //MessageBox.Show(dt.Rows.Count.ToString());
                        }


                    }
                }
                else
                {

                    da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where subTask.teamMemberId = " + userId +" and "+ searchKey + " like " + "'%" + searchValue + "%'", con.connection);


                }


              
            }
            else if (searchValue != null && (searchKey == "subTask.createdDate" || searchKey == "subTask.startDate" || searchKey == "subTask.expectedEndDate" || searchKey == "subTask.endDate"))
            {
                DateTime search = DateTime.Parse(searchValue);
                if (type == "ProjectHead")
                {

                    da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where " + searchKey + " = " + "'" + search.ToString() + "'", con.connection);
                }
                else if (type == "TeamLeader")
                {

                    da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        da = new SqlDataAdapter("SELECT teamMemberId from teamMember where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        string[] teamMemberId = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            teamMemberId[i] = dt.Rows[i][0].ToString();
                        }

                        if (teamMemberId.Length > 0)
                        {
                            da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where subTask.teamMemberId IN (" + string.Join(",", teamMemberId) + ") and " + searchKey + " = " + "'" + search.ToString() + "'", con.connection);

                            //MessageBox.Show(dt.Rows.Count.ToString());
                        }


                    }
                }
                else
                {

                    da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where subTask.teamMemberId = " + userId + " and " + searchKey + " = " + "'" + search.ToString() + "'", con.connection);

                }

            }
            else
            {

                if (type == "ProjectHead")
                {

                    da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId ", con.connection);
                }
                else if (type == "TeamLeader")
                {

                    da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        da = new SqlDataAdapter("SELECT teamMemberId from teamMember where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        string[] teamMemberId = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            teamMemberId[i] = dt.Rows[i][0].ToString();
                        }

                        if (teamMemberId.Length > 0)
                        {
                            da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where subTask.teamMemberId IN (" + string.Join(",", teamMemberId) + ")" , con.connection);

                            //MessageBox.Show(dt.Rows.Count.ToString());
                        }


                    }
                }
                else
                {

                    da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where subTask.teamMemberId = " + userId, con.connection);


                }


            }

            con.connection.Open();
            dt = new DataTable();

            dt.Columns.Add(textFormatter.ToTitleCase("id"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("projectId"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("taskId"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("title"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("description"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("duration"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("branch"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("createdDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("startDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("expectedEndDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("endDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("percentageComplete"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("status"), typeof(string));


            da.Fill(dt);
            con.connection.Close();

            return dt;
        }
        public DataTable searchInteger(string table_name, string searchKey, int searchValue, string[] fields = null)
        {
            con = new DBConnection();

            fields = fields ?? new string[] { "*" };
            var field = String.Join(",", fields);
            if (searchValue >0)
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name + " where " + searchKey + " = " +  searchValue, con.connection);
            }
            else
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name, con.connection);
            }

            con.connection.Open();
            dt = new DataTable();

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    dt.Columns.Add(textFormatter.ToTitleCase(fields[i]), typeof(string));
                }
            }

            da.Fill(dt);
            con.connection.Close();

            return dt;
        }

        public DataTable searchIn(string table_name, string searchKey, string [] searchValue, string[] fields = null,string condition =null)
        {
            con = new DBConnection();

            fields = fields ?? new string[] { "*" };
            var field = String.Join(",", fields);

            string searchInValues = "'" + string.Join("', '", searchValue) + "'";

            if (searchValue != null)
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name + " where " + searchKey + " IN " + "(" + searchInValues + ")", con.connection);
            }
            else
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name, con.connection);
            }
            if(condition!=null)
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name + " where  "+condition+" and " + searchKey + " IN " + "(" + searchInValues + ")", con.connection);
            }
            con.connection.Open();
            dt = new DataTable();

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    dt.Columns.Add(textFormatter.ToTitleCase(fields[i]), typeof(string));
                }
            }

            da.Fill(dt);
            con.connection.Close();

            return dt;
        }
        public DataTable searchInInteger(string table_name, string searchKey, int[] searchValue, string[] fields = null, string condition = null)
        {
            con = new DBConnection();

            fields = fields ?? new string[] { "*" };
            var field = String.Join(",", fields);

            string searchInValues = string.Join(", ", searchValue);
            //MessageBox.Show(searchInValues);

            if (searchValue != null)
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name + " where " + searchKey + " IN " + "(" + searchInValues + ")", con.connection);
            }
            else
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name, con.connection);
            }
            if (condition != null)
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name + " where  " + condition + " and " + searchKey + " IN " + "(" + searchInValues + ")", con.connection);
            }
            con.connection.Open();
            dt = new DataTable();

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    dt.Columns.Add(textFormatter.ToTitleCase(fields[i]), typeof(string));
                }
            }

            da.Fill(dt);
            con.connection.Close();

            return dt;
        }

        public void update(string table_name, string[] fields, string[] values, int id, string condition = null)
        {
            con = new DBConnection();
            string update = null;

            for (int i = 0; i < values.Length; i++)
            {
                update = update + fields[i] + " = '" + values[i] + "', ";
            }

            SqlCommand cmd = new SqlCommand("update " + table_name + " set " + update.Remove(update.Length - 2) + " where id=" + id + " " + condition, con.connection);

            con.connection.Open();
            if(cmd.ExecuteNonQuery() >= 1)
            {
                MessageBox.Show("Data Updated Successfully!", "Success Message", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Data Updated Unsuccessfully!", "Failed Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            con.connection.Close();

        }

        public SqlCommand find(string table_name, string value)
        {
            con = new DBConnection();
            con.connection.Open();

            cmd = new SqlCommand("SELECT * FROM " + table_name + " where id = "+ int.Parse(value), con.connection);
            
            return cmd;

        }

        public string findName(string table_name, string value, string fieldName)
        {
            con = new DBConnection();
            con.connection.Open();

            cmd = new SqlCommand("SELECT "+ fieldName + " FROM " + table_name + " where id = " + int.Parse(value), con.connection);
            string name = cmd.ExecuteScalar().ToString();
            return name;

        }

        public DataTable findField(string table_name, string value, string fieldName, string searchField)
        {
            con = new DBConnection();
            con.connection.Open();

            da = new SqlDataAdapter("SELECT " + fieldName + " FROM " + table_name + " where "+ searchField + " = " + int.Parse(value), con.connection);
            dt = new DataTable();
            da.Fill(dt);
            return dt;

        }

        public DataTable findFieldString(string table_name, string value, string fieldName, string searchField)
        {
            con = new DBConnection();
            con.connection.Open();

            da = new SqlDataAdapter("SELECT " + fieldName + " FROM " + table_name + " where " + searchField + " = '" + value+"'", con.connection);
            dt = new DataTable();
            da.Fill(dt);
            return dt;

        }

        public void delete(string table_name, int id)
        {
            con = new DBConnection();
            con.connection.Open();

            SqlCommand cmd = new SqlCommand("delete " + table_name + " where id=" + id, con.connection);

            if (cmd.ExecuteNonQuery() >= 1)
            {
                MessageBox.Show("Data Deleted Successfully!", "Success Message", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Data Deleted Unsuccessfully!", "Failed Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            con.connection.Close();
        }
        public void deleteMultiple(string table_name, int idValue, string key)
        {
            con = new DBConnection();
            con.connection.Open();

            SqlCommand cmd = new SqlCommand("delete " + table_name + " where "+key+"=" + idValue, con.connection);
            cmd.ExecuteNonQuery();
            con.connection.Close();
        }

        public DataTable searchWithCondition(string table_name, string searchKey, string searchValue, string condition, string[] fields = null)
        {
            con = new DBConnection();

            fields = fields ?? new string[] { "*" };
            var field = String.Join(",", fields);
            if (searchValue.Length > 0)
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name + " where " + searchKey + " like " + "'%" + searchValue + "%' " + condition, con.connection);
            }
            else
            {
                da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name + " where 1 " + condition, con.connection);
            }

            con.connection.Open();
            dt = new DataTable();

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    dt.Columns.Add(textFormatter.ToTitleCase(fields[i]), typeof(string));
                }
            }

            da.Fill(dt);
            con.connection.Close();

            return dt;
        }

        public DataTable searchWithJoinCondition(string table_name, string searchKey, string searchValue, string condition, string table_name2, string searchKey2, string searchValue2, string operatorName = "and",string[] fields = null)
        {
            con = new DBConnection();

            fields = fields ?? new string[] { "*" };
            var field = String.Join(",", fields);
            
            da = new SqlDataAdapter("SELECT " + field + " FROM " + table_name  +" Inner join " + table_name2 + " on " +table_name + ".id = " + table_name2 + ".id" + " where " + table_name + "."+searchKey + " = " + "'"+searchValue+"' " + operatorName+" " + table_name2 + "." + searchKey2 + " = " + "'" + searchValue2 + "'  " + condition, con.connection);
          

            con.connection.Open();
            dt = new DataTable();

            if (fields.Length > 0)
            {
                for (int i = 0; i < fields.Length; i++)
                {
                    dt.Columns.Add(textFormatter.ToTitleCase(fields[i]), typeof(string));
                }
            }

            da.Fill(dt);
            con.connection.Close();

            return dt;
        }

        public DataTable searchNewTeamLeader(string branch)
        {
            con = new DBConnection();

            if(branch == null)
            {

                 da = new SqlDataAdapter("SELECT DISTINCT users.id as id, users.firstname as firstname, users.lastname as lastname from users inner join team on users.id != team.teamLeader where users.type = 'TeamLeader' ", con.connection);

               
            }
            else
            {
                da = new SqlDataAdapter("SELECT DISTINCT users.id as id, users.firstname as firstname, users.lastname as lastname from users inner join team on users.id != team.teamLeader where users.type = 'TeamLeader' and users.branch = '" + branch + "'", con.connection);
            }


            con.connection.Open();
            dt = new DataTable();

            dt.Columns.Add(textFormatter.ToTitleCase("id"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("firstname"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("lastname"), typeof(string));
            da.Fill(dt);
            if (dt.Rows.Count<=0)
            {
                if (branch == null)
                {

                    da = new SqlDataAdapter("SELECT DISTINCT users.id as id, users.firstname as firstname, users.lastname as lastname from users left join team on users.id != team.teamLeader where users.type = 'TeamLeader' ", con.connection);


                }
                else
                {
                    da = new SqlDataAdapter("SELECT DISTINCT users.id as id, users.firstname as firstname, users.lastname as lastname from users left join team on users.id != team.teamLeader where users.type = 'TeamLeader' and users.branch = '" + branch + "'", con.connection);
                }
            }
            da.Fill(dt);

            con.connection.Close();

            return dt;
        }

        public DataTable searchNewTeamMember(string branch)
        {
            con = new DBConnection();

          da = new SqlDataAdapter("SELECT DISTINCT users.id as id, users.firstname as firstname, users.lastname as lastname from users inner join teamMember on users.id = teamMember.teamMemberId where users.type = 'TeamMember'", con.connection);


            con.connection.Open();
            dt = new DataTable();
            
          

            //MessageBox.Show(dt.Rows.Count.ToString());
            da.Fill(dt);
            string[] id = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {

                id[i] = dt.Rows[i][0].ToString();
            }

            string ids = String.Join(",", id);
            //MessageBox.Show(ids);
            if(id.Length <=0)
            {
                if (branch != null)
                {
                    da = new SqlDataAdapter("SELECT id, firstname , lastname from users where type = 'TeamMember' and branch = '" + branch + "'", con.connection);
                }
                else
                {
                    da = new SqlDataAdapter("SELECT id, firstname , lastname from users where type = 'TeamMember'", con.connection);
                }
            }
            else
            {
                if (branch != null)
                {
                    da = new SqlDataAdapter("SELECT id, firstname , lastname from users where type = 'TeamMember' and id NOT IN  (" + ids + ") and branch = '" + branch + "'", con.connection);
                }
                else
                {
                    da = new SqlDataAdapter("SELECT id, firstname , lastname from users where type = 'TeamMember' and id NOT IN  (" + ids + ")", con.connection);
                }
            }
            
            dt = new DataTable();

            dt.Columns.Add(textFormatter.ToTitleCase("id"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("firstname"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("lastname"), typeof(string));
            da.Fill(dt);
            con.connection.Close();

            return dt;
        }

        public DataTable viewTeam()
        {

            con = new DBConnection();
            
            da = new SqlDataAdapter("SELECT  team.id as id, team.name as name,team.branch as branch,team.teamLeader as teamLeader,team.status as status,teamMember.teamMemberId as teamMember,teamMember.teamId as teamId from teamMember inner join team on team.id = teamMember.teamId", con.connection);

            con.connection.Open();
            dt = new DataTable();

           
            dt.Columns.Add(textFormatter.ToTitleCase("id"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("name"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("branch"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("status"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("teamLeader"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("teamMember"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("teamId"), typeof(string));

            da.Fill(dt);
            con.connection.Close();

            return dt;
        }

        public bool sendEmail(string [] email, string subject, string body)
        {
            try
            {
                for(int i = 0; i<email.Length;i++)
                {
                    using (MailMessage mail = new MailMessage())
                    {
                        mail.From = new MailAddress("mathu1152@gmail.com");
                        mail.To.Add(email[i]);
                        mail.Subject = subject;
                        mail.Body = body;
                        mail.IsBodyHtml = true;

                        using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                        {
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new NetworkCredential("mathu1152@gmail.com", "dmumpan1883");
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                        }

                    }
                }
                return true;

            }
            catch (Exception ex)
            {
                return false;

                //MessageBox.Show(ex.Message);
            }
        }

        public DataTable viewProject(string type,string email)
        {

            con = new DBConnection();
            dt = new DataTable();
            con.connection.Open();

            dt = findFieldString("users",email,"id","email");

            string userId = dt.Rows[0][0].ToString();

            if (type == "ProjectHead")
            {
                da = new SqlDataAdapter("SELECT * from project", con.connection);
            }
            else if(type == "TeamLeader")
            {

                da = new SqlDataAdapter("SELECT id from team where teamLeader="+userId, con.connection);
                dt = new DataTable();
                da.Fill(dt);
                if(dt.Rows.Count > 0)
                {
                    da = new SqlDataAdapter("SELECT projectId from ProjectTeam where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    string[] projectId = new string[dt.Rows.Count];
                    for(int i=0;i< dt.Rows.Count;i++)
                    {
                        projectId[i] = dt.Rows[i][0].ToString();
                    }

                    if(projectId.Length>0)
                    {
                        da = new SqlDataAdapter("SELECT * from project where id IN (" + string.Join(",",projectId) +")", con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        //MessageBox.Show(dt.Rows.Count.ToString());
                    }

                    
                }
            }
            else
            {
                da = new SqlDataAdapter("SELECT teamId from teamMember where teamMemberId=" + userId, con.connection);
                dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    da = new SqlDataAdapter("SELECT projectId from ProjectTeam where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    string[] projectId = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        projectId[i] = dt.Rows[i][0].ToString();
                    }

                    if (projectId.Length > 0)
                    {
                        da = new SqlDataAdapter("SELECT * from project where id IN (" + string.Join(",", projectId) + ")", con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        //MessageBox.Show(dt.Rows.Count.ToString());
                    }


                }
            }


            dt = new DataTable();


            dt.Columns.Add(textFormatter.ToTitleCase("id"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("title"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("description"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("duration"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("branch"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("createdDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("startDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("expectedEndDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("endDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("percentageComplete"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("status"), typeof(string));

            da.Fill(dt);
            con.connection.Close();

            return dt;
        }
        public DataTable getProjectDetails(int projectId)
        {

            con = new DBConnection();

            da = new SqlDataAdapter("SELECT  * from project inner join projectTeam on project.id = projectTeam.projectId where project.id = " + projectId, con.connection);

            con.connection.Open();
            dt = new DataTable();
            da.Fill(dt);
            con.connection.Close();

            return dt;
        }
        public DataTable getTaskDetails(int taskId)
        {

            con = new DBConnection();

            da = new SqlDataAdapter("SELECT  task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where task.id = " + taskId, con.connection);

            con.connection.Open();
            dt = new DataTable();
            da.Fill(dt);
            con.connection.Close();

            return dt;
        }

        public DataTable getSubTaskDetails(int subTaskId)
        {

            con = new DBConnection();

            da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status, subTask.teamMemberId as teamMember, subTask.projectId as project from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where subTask.id = " + subTaskId, con.connection);

            con.connection.Open();
            dt = new DataTable();
            da.Fill(dt);
            con.connection.Close();

            return dt;
        }
        public DataTable getProjectTeamDetails(int projectId)
        {

            con = new DBConnection();

            da = new SqlDataAdapter("SELECT  id,teamId from projectTeam where projectId = " + projectId, con.connection);

            con.connection.Open();
            dt = new DataTable();
            da.Fill(dt);
            string[] teamIds = new string[dt.Rows.Count];
            for(int i=0; i<dt.Rows.Count; i++)
            {
                teamIds[i] = dt.Rows[i][1].ToString(); 
            }

            string teamIdInValues = "'" + string.Join("', '", teamIds) + "'";
            da = new SqlDataAdapter("SELECT teamLeader from team where id IN (" + teamIdInValues+")", con.connection);
            dt = new DataTable();
            da.Fill(dt);
           // MessageBox.Show(dt.Columns.Count.ToString());
            string[] userIdLeader = new string[dt.Rows.Count];
            
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //MessageBox.Show(dt.Rows[i][0].ToString() + " - " + dt.Rows[i][1].ToString());
                userIdLeader[i] = dt.Rows[i][0].ToString();
               // userIdMember[i] = dt.Rows[i][1].ToString();
                
                //MessageBox.Show(dt.Rows[i][0].ToString());
               
            }

            da = new SqlDataAdapter("SELECT teamMemberId from teamMember where teamId IN (" + teamIdInValues + ")", con.connection);
            dt = new DataTable();
            da.Fill(dt);
            string[] userIdMember = new string[dt.Rows.Count];


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //MessageBox.Show(dt.Rows[i][0].ToString() + " - " + dt.Rows[i][1].ToString());
                //userIdLeader[i] = dt.Rows[i][0].ToString();
                userIdMember[i] = dt.Rows[i][0].ToString();

                //MessageBox.Show(dt.Rows[i][0].ToString() );

            }

            var all = userIdLeader.Union(userIdMember).ToArray();
            all = all.Where(a => a != null && a != "").ToArray();

            string[] fields = { "id", "firstname", "lastname" };
            dt = searchIn("users","id",all,fields);

            con.connection.Close();

            return dt;
        }

        public DataTable getTaskTeamDetails(int taskId)
        {

            con = new DBConnection();

            da = new SqlDataAdapter("SELECT  id,teamMemberId from taskTeam where taskId = " + taskId, con.connection);

            con.connection.Open();
            dt = new DataTable();
            da.Fill(dt);
            string[] teamMemberIds = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                teamMemberIds[i] = dt.Rows[i][1].ToString();
            }

            string teamMemberIdInValues = "'" + string.Join("', '", teamMemberIds) + "'";

            da = new SqlDataAdapter("SELECT teamId from teamMember where teamMemberId IN (" + teamMemberIdInValues + ")", con.connection);
            dt = new DataTable();
            da.Fill(dt);
            string[] teamIds = new string[dt.Rows.Count];

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                teamIds[i] = dt.Rows[i][0].ToString();
            }

            string teamIdInValues = "'" + string.Join("', '", teamIds) + "'";

            da = new SqlDataAdapter("SELECT teamLeader from team where id IN (" + teamIdInValues + ")", con.connection);
            dt = new DataTable();
            da.Fill(dt);
            string[] teamLeaderId = new string[dt.Rows.Count];


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                teamLeaderId[i] = dt.Rows[i][0].ToString();

            }

            var all = teamLeaderId.Union(teamMemberIds).ToArray();
            all = all.Where(a => a != null && a != "").ToArray();

            string[] fields = { "id", "firstname", "lastname" };
            dt = searchIn("users", "id", all, fields);

            con.connection.Close();

            return dt;
        }

        public DataTable viewTask()
        {

            con = new DBConnection();

            string userId = Environment.GetEnvironmentVariable("id");
            string type = Environment.GetEnvironmentVariable("type");
           // MessageBox.Show(type);
            if (type == "ProjectHead")
            {
                da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId", con.connection);
            }
            else if (type == "TeamLeader")
            {

                da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    da = new SqlDataAdapter("SELECT teamMemberId from teamMember where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    string[] teamMemberId = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        teamMemberId[i] = dt.Rows[i][0].ToString();
                    }

                    if (teamMemberId.Length > 0)
                    {
                        da = new SqlDataAdapter("SELECT taskId from taskTeam where teamMemberId  IN (" + string.Join(",", teamMemberId) + ")", con.connection);
                        dt = new DataTable();
                        da.Fill(dt);
                        string[] taskId = new string[dt.Rows.Count];
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            taskId[i] = dt.Rows[i][0].ToString();
                        }

                        if(taskId.Length > 0)
                        {
                            da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where task.id IN (" + string.Join(",", taskId) + ")", con.connection);
                          
                        }
                       
                        //MessageBox.Show(dt.Rows.Count.ToString());
                    }


                }
            }
            else
            {
               
                da = new SqlDataAdapter("SELECT taskId from taskTeam where teamMemberId  = " + userId , con.connection);
                dt = new DataTable();
                da.Fill(dt);
                string[] taskId = new string[dt.Rows.Count];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    taskId[i] = dt.Rows[i][0].ToString();
                }

                if (taskId.Length > 0)
                {
                    da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId where task.id IN (" + string.Join(",", taskId) + ")", con.connection);
                   
                }

                     
            }

           // da = new SqlDataAdapter("SELECT task.id as id, task.title as title, task.description as description, task.duration as duration, project.title as projectId, task.createdDate as createdDate, task.startDate as startDate, task.expectedEndDate as expectedEndDate, task.endDate as endDate, task.percentageComplete as percentageComplete, task.status as status from task inner join project on project.id = task.projectId", con.connection);

            con.connection.Open();
            dt = new DataTable();


            dt.Columns.Add(textFormatter.ToTitleCase("id"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("title"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("description"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("duration"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("projectId"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("createdDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("startDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("expectedEndDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("endDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("percentageComplete"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("status"), typeof(string));

            da.Fill(dt);
            con.connection.Close();

            return dt;
        }

        public DataTable viewSubTask()
        {

            con = new DBConnection();


            string userId = Environment.GetEnvironmentVariable("id");
            string type = Environment.GetEnvironmentVariable("type");
            // MessageBox.Show(type);
            if (type == "ProjectHead")
            {
                da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId", con.connection);
            }
            else if (type == "TeamLeader")
            {

                da = new SqlDataAdapter("SELECT id from team where teamLeader=" + userId, con.connection);
                dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    da = new SqlDataAdapter("SELECT teamMemberId from teamMember where teamId=" + dt.Rows[0][0].ToString(), con.connection);
                    dt = new DataTable();
                    da.Fill(dt);
                    string[] teamMemberId = new string[dt.Rows.Count];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        teamMemberId[i] = dt.Rows[i][0].ToString();
                    }

                    if (teamMemberId.Length > 0)
                    {

                        da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where subTask.teamMemberId IN (" + string.Join(",", teamMemberId) + ")", con.connection);

                        //MessageBox.Show(dt.Rows.Count.ToString());
                    }


                }
            }
            else
            {


                da = new SqlDataAdapter("SELECT subTask.id as id, subTask.title as title, subTask.description as description, subTask.duration as duration, project.title as projectId, task.title as taskId, subTask.createdDate as createdDate, subTask.startDate as startDate, subTask.expectedEndDate as expectedEndDate, subTask.endDate as endDate, subTask.percentageComplete as percentageComplete, subTask.status as status from subTask inner join project on project.id = subTask.projectId inner join task on task.id = subTask.taskId where subTask.teamMemberId = " + userId, con.connection);


            }


            con.connection.Open();
            dt = new DataTable();


            dt.Columns.Add(textFormatter.ToTitleCase("id"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("title"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("description"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("duration"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("projectId"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("taskId"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("createdDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("startDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("expectedEndDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("endDate"), typeof(DateTime));
            dt.Columns.Add(textFormatter.ToTitleCase("percentageComplete"), typeof(string));
            dt.Columns.Add(textFormatter.ToTitleCase("status"), typeof(string));

            da.Fill(dt);
            con.connection.Close();

            return dt;
        }
        public bool login(Model.User user)
        {
            con = new DBConnection();


            da = new SqlDataAdapter("SELECT COUNT(*) FROM users WHERE email='" + user.Email + "' AND password='" + user.Password + "' and status='Active'", con.connection);
            con.connection.Open();
            dt = new DataTable();
            da.Fill(dt);
            
            if (dt.Rows[0][0].ToString() == "1")
            {
                da = new SqlDataAdapter("SELECT * FROM users WHERE email='" + user.Email + "' AND password='" + user.Password + "' and status='Active'", con.connection);

                dt = new DataTable();
                da.Fill(dt);

               

                user.Password = user.Password;
                user.Email = user.Email;
                user.Type = dt.Rows[0][8].ToString();
                user.FirstName = dt.Rows[0][1].ToString();
                user.LastName = dt.Rows[0][2].ToString();

                Environment.SetEnvironmentVariable("email", user.Email);
                Environment.SetEnvironmentVariable("id", dt.Rows[0][0].ToString());
                Environment.SetEnvironmentVariable("type", dt.Rows[0][8].ToString());
                Environment.SetEnvironmentVariable("firstname", dt.Rows[0][1].ToString());
                Environment.SetEnvironmentVariable("lastname", dt.Rows[0][2].ToString());

                Dashboard dahsboard = new Dashboard(user);
                dahsboard.Show();

               // PopupWindow popupWindow = new PopupWindow(user);
                return true;
            }
            else
            {
                //MessageBox.Show("Invalid Email or Password");
                return false;
            }
        }

        public List<Expander> viewReport()
        {
            con = new DBConnection();
            List<Expander> expanders = new List<Expander>();
            Expander dynamicExpander;
            da = new SqlDataAdapter("SELECT * from project", con.connection);
            dt = new DataTable();
            da.Fill(dt);
            var converter = new System.Windows.Media.BrushConverter();
            TextBlock textBlock;
            //DateTime dateTime;
            if (dt.Rows.Count > 0)
            {
                                
                for (int project = 0; project < dt.Rows.Count; project++)
                {
                    textBlock = new TextBlock();
                    textBlock.Text = dt.Rows[project][0].ToString() + " - " + dt.Rows[project][1].ToString();
                    textBlock.FontWeight = FontWeights.UltraBold;
                    textBlock.Background = (Brush)converter.ConvertFromString("#CEE8FB");
                    textBlock.Width = 800;
                    textBlock.Padding = new Thickness(10);
                    
                    dynamicExpander = new Expander();
                    dynamicExpander.Width = 800;
                    dynamicExpander.Header = textBlock;
                    dynamicExpander.Padding = new Thickness(10);
                    dynamicExpander.HorizontalAlignment = HorizontalAlignment.Left;
                    dynamicExpander.IsExpanded = false;
                    dynamicExpander.FontSize = 14;
                    dynamicExpander.Content = "Description: " + dt.Rows[project][2].ToString() + "\n" + 
                        "Percentage Complete: " + dt.Rows[project][9].ToString() + "\n" + 
                        "Status: " + dt.Rows[project][10].ToString()+"\n"+
                        "Users: " + "\n";

                    SqlDataAdapter team = new SqlDataAdapter("SELECT projectTeam.teamId as teamId, team.name as teamName, team.teamLeader as teamLeader, teamMember.teamMemberId as teamMember, users.id, users.firstname,users.lastname from projectTeam inner join team on team.id = projectTeam.teamId inner join teamMember on teamMember.teamId = team.id inner join users on users.id = teamMember.teamMemberId where projectTeam.projectId=" + dt.Rows[project][0].ToString(), con.connection);
                    DataTable teamDataTable = new DataTable();
                    team.Fill(teamDataTable);
                    if(teamDataTable.Rows.Count > 0)
                    {
                        for(int teamIndex = 0; teamIndex<teamDataTable.Rows.Count;teamIndex++)
                        {

                            dynamicExpander.Content += teamDataTable.Rows[teamIndex][1].ToString()+ " -" + teamDataTable.Rows[teamIndex][4].ToString() + " - " + teamDataTable.Rows[teamIndex][5].ToString() + " "+ teamDataTable.Rows[teamIndex][6].ToString()+"\n";
                        }
                    }
                    expanders.Add(dynamicExpander);

                    SqlDataAdapter taskDataAdapter = new SqlDataAdapter("SELECT * from task where projectId=" + dt.Rows[project][0].ToString(), con.connection);

                    DataTable taskDataTable = new DataTable();
                    taskDataAdapter.Fill(taskDataTable);

                    if (taskDataTable.Rows.Count > 0)
                    {
                        for (int task = 0; task < taskDataTable.Rows.Count; task++)
                        {
                            textBlock = new TextBlock();
                            textBlock.Text = taskDataTable.Rows[task][0].ToString() + " - " +taskDataTable.Rows[task][1].ToString();
                            textBlock.FontWeight = FontWeights.UltraBold;
                            textBlock.Background = (Brush)converter.ConvertFromString("#BFBFC4");
                            textBlock.Width = 730;
                            textBlock.Padding = new Thickness(10);

                            dynamicExpander = new Expander();

                            dynamicExpander.Header = textBlock;
                            dynamicExpander.FontSize = 14;
                            dynamicExpander.HorizontalAlignment = HorizontalAlignment.Left;
                            dynamicExpander.Margin = new Thickness(20, 0, 0, 0);
                            dynamicExpander.IsExpanded = false;
                            dynamicExpander.Padding = new Thickness(10);



                            dynamicExpander.Content = "Description: " + taskDataTable.Rows[task][2].ToString() +"\n"+
                                "Percentage Complete: " + taskDataTable.Rows[task][9].ToString() + "\n" +
                                "Status: " + taskDataTable.Rows[task][10].ToString() + "\n" +
                                "Users: " + "\n";

                            SqlDataAdapter teamTask = new SqlDataAdapter("SELECT taskTeam.teamMemberId as teamMemberId, team.name as teamName, team.teamLeader as teamLeader, users.id, users.firstname,users.lastname from taskTeam inner join teamMember on teamMember.teamMemberId = taskTeam.teamMemberId inner join team on teamMember.teamId = team.id inner join task on task.id = taskTeam.taskId inner join users on users.id = teamMember.teamMemberId where taskTeam.taskId=" + dt.Rows[task][0].ToString(), con.connection);

                            DataTable teamTaskDataTable = new DataTable();
                            teamTask.Fill(teamTaskDataTable);

                            if (teamTaskDataTable.Rows.Count > 0)
                            {
                                for (int teamTaskIndex = 0; teamTaskIndex < teamTaskDataTable.Rows.Count; teamTaskIndex++)
                                {

                                    dynamicExpander.Content += teamTaskDataTable.Rows[teamTaskIndex][1].ToString() + " -" + teamTaskDataTable.Rows[teamTaskIndex][3].ToString() + " - " + teamTaskDataTable.Rows[teamTaskIndex][4].ToString() + " " + teamTaskDataTable.Rows[teamTaskIndex][5].ToString() + "\n";
                                }
                            }

                            expanders.Add(dynamicExpander);

                            SqlDataAdapter subTaskDataAdapter = new SqlDataAdapter("SELECT * from subTask where taskId=" + taskDataTable.Rows[task][0].ToString(), con.connection);
                            DataTable subTaskDataTable = new DataTable();
                            subTaskDataAdapter.Fill(subTaskDataTable);

                            if(subTaskDataTable.Rows.Count>0)
                            {
                                for (int subTask = 0; subTask < subTaskDataTable.Rows.Count; subTask++)
                                {
                                    textBlock = new TextBlock();
                                    textBlock.Text = subTaskDataTable.Rows[subTask][0].ToString() + " - " + subTaskDataTable.Rows[subTask][1].ToString();
                                    textBlock.FontWeight = FontWeights.UltraBold;
                                    textBlock.Background = (Brush)converter.ConvertFromString("#EEEEEE");
                                    textBlock.Width = 710;
                                    textBlock.Padding = new Thickness(10);

                                    dynamicExpander = new Expander();

                                    dynamicExpander.Margin = new Thickness(40,0,0,0);
                                    dynamicExpander.HorizontalAlignment = HorizontalAlignment.Left;

                                    dynamicExpander.IsExpanded = false;

                                    dynamicExpander.Header = textBlock;
                                    dynamicExpander.FontSize = 14;
                                    dynamicExpander.Padding = new Thickness(10);

                                    dynamicExpander.Content = "Description: " + subTaskDataTable.Rows[subTask][2].ToString() + "\n" +
                      "Percentage Complete: " + subTaskDataTable.Rows[subTask][11].ToString() + "\n" +
                      "Status: " + subTaskDataTable.Rows[subTask][12].ToString() + "\n" +
                      "Users: " + "\n";

                                    SqlDataAdapter teamSubTask = new SqlDataAdapter("SELECT subTask.teamMemberId as teamMemberId, team.name as teamName, team.teamLeader as teamLeader, users.id, users.firstname,users.lastname from subTask inner join teamMember on teamMember.teamMemberId = subTask.teamMemberId inner join team on teamMember.teamId = team.id inner join users on users.id = teamMember.teamMemberId where subTask.id=" + subTaskDataTable.Rows[subTask][0].ToString(), con.connection);

                                    DataTable teamSubTaskDataTable = new DataTable();
                                    teamSubTask.Fill(teamSubTaskDataTable);

                                    if (teamSubTaskDataTable.Rows.Count > 0)
                                    {
                                        for (int teamSubTaskIndex = 0; teamSubTaskIndex < teamSubTaskDataTable.Rows.Count; teamSubTaskIndex++)
                                        {

                                            dynamicExpander.Content += teamSubTaskDataTable.Rows[teamSubTaskIndex][1].ToString() + " -" + teamSubTaskDataTable.Rows[teamSubTaskIndex][3].ToString() + " - " + teamSubTaskDataTable.Rows[teamSubTaskIndex][4].ToString() + " " + teamSubTaskDataTable.Rows[teamSubTaskIndex][5].ToString() + "\n";
                                        }
                                    }

                                    expanders.Add(dynamicExpander);
                                   

                                }
                            }
                        }
                    }
                       
                }


            }

            return expanders;

        }

        public void updatePassword(User user)
        {
            con = new DBConnection();

            string userId = Environment.GetEnvironmentVariable("id");
            SqlCommand cmd = new SqlCommand("update users set password = '"+user.Password+"' where id=" + userId, con.connection);

            con.connection.Open();
            if (cmd.ExecuteNonQuery() >= 1)
            {
                MessageBox.Show("Password Updated Successfully!", "Success Message", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Password Updated Unsuccessfully!", "Failed Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            con.connection.Close();
        }

        public DataTable searchDiscussion(int projectId)
        {
            con = new DBConnection();
           
            da = new SqlDataAdapter("SELECT users.firstname,users.lastname, discussion.message,discussion.userId,discussion.date,discussion.projectId from discussion inner join users on users.id = discussion.userId where discussion.projectId = " + projectId+ "order by discussion.date DESC", con.connection);

            dt = new DataTable();
            da.Fill(dt);

               
            return dt;

        }

        public DataTable getProjectForDiscussion()
        {
            con = new DBConnection();
            string userId = Environment.GetEnvironmentVariable("id");
            string type = Environment.GetEnvironmentVariable("type");
            if (type == "TeamMember")
            {
                da = new SqlDataAdapter("SELECT teamId from teamMember where teamMemberId = " + userId, con.connection);

                dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    da = new SqlDataAdapter("SELECT project.id, project.title from project inner join projectTeam on projectTeam.projectId = project.id where projectTeam.teamId = " + dt.Rows[0][0].ToString(), con.connection);

                }

            }
            else if (type == "TeamLeader")
            {
                da = new SqlDataAdapter("SELECT id from team where teamLeader = " + userId, con.connection);

                dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    da = new SqlDataAdapter("SELECT project.id, project.title from project inner join projectTeam on projectTeam.projectId = project.id where projectTeam.teamId = " + dt.Rows[0][0].ToString(), con.connection);

                }

            }
            else
            {
                da = new SqlDataAdapter("SELECT id,title from project", con.connection);
            }
            dt = new DataTable();
            da.Fill(dt);
            return dt;

        }

    }
}
