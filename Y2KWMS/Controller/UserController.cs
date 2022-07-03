using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Y2KWMS.Model;


namespace Y2KWMS.Controller
{
    class UserController : Window
    {
        QueryController queryController;
        public string getId()
        {
            queryController = new QueryController();
            string id = queryController.getLastId("users");
            return id;
        }
        public void create(User user)
        {
            string[] field = { "firstname", "lastname","phone","address","email","password","branch","type","status" };
            string[] values = {user.FirstName,user.LastName,user.Mobile,user.Address,user.Email,user.Password,user.Branch,user.Type,user.Status};

            queryController = new QueryController();
            queryController.save("users", field, values);
        }

        public DataTable load()
        {
            queryController = new QueryController();
            string[] fields = { "id", "firstname", "lastname", "phone", "address", "email", "branch", "type", "status" };
            return queryController.view("users", fields);
        }

        public DataTable search(string searchKey, string searchValue)
        {
            queryController = new QueryController();
            string[] fields = { "id", "firstname", "lastname", "phone", "address", "email", "branch", "type", "status" };
            return queryController.search("users", searchKey,searchValue,fields);
        }

        public void edit(string id, User user)
        {
            View.PopupWindow popupWindow = new View.PopupWindow(user);
            popupWindow.Show();
            popupWindow.PagesNavigation.Navigate(new View.Pages.Project.User.Create(id,user));

          /*  View.Dashboard dashboard = new View.Dashboard();
            dashboard.Show();
            dashboard.PagesNavigation.Navigate(new View.Pages.Project.User.Create(id));*/
        } 
        public User editData(string id)
        {
            queryController = new QueryController();
            string[] fields = { "id", "firstname", "lastname", "phone", "address", "email", "branch", "type", "status" };
            User user = new User();
            DBConnection con = new DBConnection();
            try
            {
                con.connection.Open();
                SqlDataReader sdr = queryController.find("users", id).ExecuteReader();
            
                while (sdr.Read())
                {
                    user.FirstName = sdr[1].ToString();
                    user.LastName = sdr[2].ToString();
                    user.Mobile = sdr[3].ToString();
                    user.Address = sdr[4].ToString();
                    user.Email = sdr[5].ToString();
                    user.Password = sdr[6].ToString();
                    user.Branch = sdr[7].ToString();
                    user.Type = sdr[8].ToString();
                    user.Status = sdr[9].ToString();
                }
                con.connection.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message +"\nCheck the Data");
            }
            
            return user;
        }

        public void update(User user)
        {
            queryController = new QueryController();
            if (user.Password != null)
            {
                string[] field = { "firstname", "lastname", "phone", "address", "email", "password", "branch", "type", "status" };
                string[] values = { user.FirstName, user.LastName, user.Mobile, user.Address, user.Email, user.Password, user.Branch, user.Type, user.Status };
                queryController.update("users", field, values, user.Id);
            }
            else
            {
                string[] field = { "firstname", "lastname", "phone", "address", "email", "branch", "type", "status" };
                string[] values = { user.FirstName, user.LastName, user.Mobile, user.Address, user.Email, user.Branch, user.Type, user.Status };
                queryController.update("users", field, values, user.Id);
            }



        } 
        public void updatePassword(User user)
        {
            queryController = new QueryController();
           
            queryController.updatePassword(user);
           



        }
    }
}
