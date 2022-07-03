using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace Y2KWMS.View.Pages.Task
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Page
    {
        TaskController taskController;
        Helper helper;
        string updateId = null;
        QueryController queryController;

        public Create(string email, string name, string type)
        {
            InitializeComponent();
            taskController = new TaskController();
            txtId.Text = loadId();
            dpCreateDate.SelectedDate = loadCurrentDate();
            dpStartDate.SelectedDate = loadCurrentDate();
            dpEndDate.SelectedDate = loadCurrentDate();
            helper = new Helper();
            dataForComboBox(type);
            getProject();
            txtPercentageComplete.Text = "0.0";
        }

        public Create(string id, Model.User user)
        {
            InitializeComponent();
            taskController = new TaskController();
            txtId.Text = id;
            dpCreateDate.SelectedDate = loadCurrentDate();
            dpStartDate.SelectedDate = loadCurrentDate();
            dpEndDate.SelectedDate = loadCurrentDate();
            helper = new Helper();
            dataForComboBox(user.Type);
            
            //getTeamMembers(null);
            updateId = id;
            btnClear.Visibility = Visibility.Hidden;

            txtblockTitle.Text = "Update Task";
            btnSave.Content = "Update";
            getDataForEdit(id);
        }

        private void getDataForEdit(string id)
        {
            Model.Task task = taskController.editData(id);
            txtTitle.Text = task.Title;
            txtDescription.Text = task.Description;
            txtDuration.Text = task.Duration.ToString();
            txtPercentageComplete.Text = task.PercentageComplete.ToString();
            dpCreateDate.SelectedDate = task.CreatedDate;
            dpStartDate.SelectedDate = task.StartDate;
            dpExpectedEndDate.SelectedDate = task.ExpectedEndDate;
            dpEndDate.SelectedDate = task.EndDate;
            cmbProject.SelectedItem = task.ProjectId;
            cmbStatus.SelectedItem = task.Status;

            for (int i = 0; i < task.TeamMemberId.Length; i++)
            {
                lbTeamMembers.Items.Add(task.TeamMemberId[i].ToString() + " - " + task.TeamName[i].ToString());
            }
            getProjectForEdit(task.ProjectId.ToString());
        }



        private void getProject()
        {
            if (!IsInitialized) return;
           
            DataTable dt = taskController.getProject();
            if (dt.Rows.Count > 0)
            {

                cmbProject.Items.Clear();
                cmbProject.Items.Add("Select Project");
                cmbProject.SelectedIndex = 0;
                //int index = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //index = cmbProject.Items.IndexOf()
                    cmbProject.Items.Add(dt.Rows[i][0] + " - " + dt.Rows[i][1]);
                }
            }
            else
            {
                cmbProject.Items.Clear();
                cmbProject.Items.Add("Select Project");
                cmbProject.SelectedIndex = 0;
            }
        }
        private void getProjectForEdit(string id)
        {
            if (!IsInitialized) return;

            DataTable dt = taskController.getProject();
            if (dt.Rows.Count > 0)
            {

                cmbProject.Items.Clear();
                cmbProject.Items.Add("Select Project");
                cmbProject.SelectedIndex = 0;
                int index = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (id == dt.Rows[i][0].ToString())
                    {
                        index = i+1;
                    }
                    cmbProject.Items.Add(dt.Rows[i][0] + " - " + dt.Rows[i][1]);
                }
                    //MessageBox.Show(index.ToString());

                cmbProject.SelectedIndex = index;
            }
            else
            {
                cmbProject.Items.Clear();
                cmbProject.Items.Add("Select Project");
                cmbProject.SelectedIndex = 0;
            }
        }
        private string loadId()
        {
            return taskController.getId();
        }

        private DateTime loadCurrentDate()
        {
            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("en-GB");
            return localDate;
        }

        private void dataForComboBox(string type)
        {

            int arrayLenght = 0;
            if (type == "ProjectHead")
            {
                arrayLenght = helper.projectStatus.Length;
            }
            else
            {
                arrayLenght = helper.projectUserStatus.Length;

            }
            for (int i = 0; i < arrayLenght; i++)
            {
                if (type == "ProjectHead")
                {
                    cmbStatus.Items.Add(helper.projectStatus[i]);
                }
                else
                {
                    cmbStatus.Items.Add(helper.projectUserStatus[i]);
                }
                cmbStatus.SelectedIndex = 1;
            }

        }

        private void getExpectedEndDate()
        {
            if (txtDuration.Text.Length > 0)
            {
                int months = int.TryParse(txtDuration.Text, out months) ? months : 1;
                DateTime startDate = dpStartDate.SelectedDate.Value;
                dpExpectedEndDate.SelectedDate = startDate.AddDays(months*7);
                dpEndDate.SelectedDate = startDate.AddMonths(months*7);
            }
            else
            {
                dpExpectedEndDate.SelectedDate = null;
            }
        }

        private void txtDuration_KeyUp(object sender, KeyEventArgs e)
        {
            getExpectedEndDate();
        }

        private void dpStartDate_MouseLeave(object sender, MouseEventArgs e)
        {
            getExpectedEndDate();
        }

        private void clear()
        {
            txtId.Text = loadId();
            txtDescription.Clear();
            txtTitle.Clear();
            cmbStatus.SelectedIndex = 1;
            cmbProject.SelectedIndex = 0;
            cmbTeamMembers.SelectedIndex = 0;
            txtDuration.Clear();
            txtPercentageComplete.Clear();
            dpCreateDate.SelectedDate = loadCurrentDate();
            dpStartDate.SelectedDate = loadCurrentDate();
            dpEndDate.SelectedDate = null;
            dpExpectedEndDate.SelectedDate = null;
            lbTeamMembers.Items.Clear();
            dpEndDate.SelectedDate = loadCurrentDate();

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }

        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (lbTeamMembers.SelectedIndex >= 0)
            {
                lbTeamMembers.Items.RemoveAt(lbTeamMembers.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select one element", "Warning Message", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void cmbProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized) return;
            if(cmbProject.SelectedIndex > 0)
            {
                getTeamMembers(cmbProject.SelectedItem.ToString());
            }
        }

        private void getTeamMembers(string project)
        {
            int projectId = int.Parse(project.Split('-')[0]);
            DataTable dt = taskController.getTeamMembers(projectId);
            if (dt.Rows.Count > 0)
            {

                cmbTeamMembers.Items.Clear();
                cmbTeamMembers.Items.Add("Select Team Members");
                cmbTeamMembers.SelectedIndex = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmbTeamMembers.Items.Add(dt.Rows[i][0] + " - " + dt.Rows[i][1]+" "+ dt.Rows[i][2]);
                }
            }
            else
            {
                cmbTeamMembers.Items.Clear();
                cmbTeamMembers.Items.Add("Select Team Members");
                cmbTeamMembers.SelectedIndex = 0;
            }
        }

        private void cmbTeamMembers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized) return;

            if (cmbTeamMembers.SelectedIndex > 0)
            {
                lbTeamMembers.Items.Add(cmbTeamMembers.SelectedItem.ToString());
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string textBoxName = null;
            if (!helper.notNullValidation(txtTitle.Text))
            {
                textBoxName += "Title";
                txtTitle.BorderBrush = Brushes.Red;
                txtTitle.ToolTip = "Check your Title";

            }

            if (!helper.notNullValidation(txtDescription.Text))
            {
                textBoxName += "Description";
                txtDescription.BorderBrush = Brushes.Red;
                txtDescription.ToolTip = "Check your Description";
            }
            if (!helper.numberOnlyValidation(txtDuration.Text))
            {
                textBoxName += "Duration";
                txtDuration.BorderBrush = Brushes.Red;
                txtDuration.ToolTip = "Check your Duration";
            }
            if (!helper.listBoxValidation(lbTeamMembers.Items.Count))
            {
                textBoxName += "Team";
                cmbTeamMembersBorder.BorderThickness = new Thickness(1);
                cmbTeamMembersBorder.BorderBrush = Brushes.Red;
                cmbTeamMembers.ToolTip = "Check your Team";
            }
            if (!dpExpectedEndDate.SelectedDate.HasValue)
            {
                textBoxName += "Date";
                dpExpectedEndDate.BorderThickness = new Thickness(1);
                dpExpectedEndDate.BorderBrush = Brushes.Red;
                dpExpectedEndDate.ToolTip = "Check your Expected End Date";
            }


            if (!helper.comboBoxValidation(cmbProject))
            {
                textBoxName += "Project";
                cmbProjectBorder.BorderThickness = new Thickness(1);
                cmbProjectBorder.BorderBrush = Brushes.Red;
                cmbProject.ToolTip = "Check your Project";
            }

            if (!helper.comboBoxValidation(cmbStatus))
            {
                textBoxName += "Status";
                cmbStatusBorder.BorderThickness = new Thickness(1);
                cmbStatusBorder.BorderBrush = Brushes.Red;
                cmbStatus.ToolTip = "Check your Status";
            }

            if (!helper.floatNumbeOnlyValidation(txtPercentageComplete.Text))
            {
                textBoxName += "Percentage Completed";
                txtPercentageComplete.BorderBrush = Brushes.Red;
                txtPercentageComplete.ToolTip = "Check your Percentage Completed";
            }
            //MessageBox.Show(cmbType.Text);
            if (textBoxName == null)
            {
                //MessageBox.Show(updateId);
                if (updateId == null)
                {

                    int[] teamMemberId = new int[lbTeamMembers.Items.Count];

                    for (int i = 0; i < lbTeamMembers.Items.Count; i++)
                    {
                        lbTeamMembers.SelectAll();

                        teamMemberId[i] = int.Parse(lbTeamMembers.SelectedItems[i].ToString().Split('-')[0]);
                    }
                    Model.Task task = new Model.Task(int.Parse(txtId.Text), txtTitle.Text, txtDescription.Text, int.Parse(txtDuration.Text),int.Parse(cmbProject.SelectedItem.ToString().Split('-')[0]) ,teamMemberId, dpCreateDate.SelectedDate.Value, dpStartDate.SelectedDate.Value, dpExpectedEndDate.SelectedDate.Value, dpEndDate.SelectedDate.Value, float.Parse(txtPercentageComplete.Text), cmbStatus.Text);
                   
                    taskController.create(task);

                    clear();


                }
                else
                {
                    int[] teamMemberId = new int[lbTeamMembers.Items.Count];

                    for (int i = 0; i < lbTeamMembers.Items.Count; i++)
                    {
                        lbTeamMembers.SelectAll();

                        teamMemberId[i] = int.Parse(lbTeamMembers.SelectedItems[i].ToString().Split('-')[0]);
                    }

                    Model.Task task = new Model.Task(int.Parse(txtId.Text), txtTitle.Text, txtDescription.Text, int.Parse(txtDuration.Text), int.Parse(cmbProject.SelectedItem.ToString().Split('-')[0]), teamMemberId, dpCreateDate.SelectedDate.Value, dpStartDate.SelectedDate.Value, dpExpectedEndDate.SelectedDate.Value, dpEndDate.SelectedDate.Value, float.Parse(txtPercentageComplete.Text), cmbStatus.Text);

                    taskController.update(task);
                    
                }


            }
        }
    }
}
