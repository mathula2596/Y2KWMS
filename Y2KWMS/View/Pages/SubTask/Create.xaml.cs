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

namespace Y2KWMS.View.Pages.SubTask
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Page
    {
        SubTaskController subTaskController;
        Helper helper;
        string updateId = null;
        QueryController queryController;
        public Create(string email, string name, string type)
        {
            InitializeComponent();
            subTaskController = new SubTaskController();
            txtId.Text = loadId();
            dpCreateDate.SelectedDate = loadCurrentDate();
            dpStartDate.SelectedDate = loadCurrentDate();
            dpEndDate.SelectedDate = loadCurrentDate();
            helper = new Helper();
            dataForComboBox(type);
            getProject();
            //getTask();
            txtPercentageComplete.Text = "0.0";
        }

        public Create(string id, Model.User user)
        {
            InitializeComponent();
            subTaskController = new SubTaskController();
            txtId.Text = id;
            
            helper = new Helper();
            dataForComboBox(user.Type);

            //getTeamMembers(null);
            updateId = id;
            btnClear.Visibility = Visibility.Hidden;

            txtblockTitle.Text = "Update Sub Task";
            btnSave.Content = "Update";
            getDataForEdit(id);
        }

        private void getDataForEdit(string id)
        {
            Model.SubTask subTask = subTaskController.editData(id);
            txtTitle.Text = subTask.Title;
            txtDescription.Text = subTask.Description;
            txtDuration.Text = subTask.Duration.ToString();
            txtPercentageComplete.Text = subTask.PercentageComplete.ToString();
            dpCreateDate.SelectedDate = subTask.CreatedDate;
            dpStartDate.SelectedDate = subTask.StartDate;
            dpExpectedEndDate.SelectedDate = subTask.ExpectedEndDate;
            dpEndDate.SelectedDate = subTask.EndDate;
            cmbProject.SelectedItem = subTask.ProjectId;
            cmbTask.SelectedItem = subTask.TaskId;
            cmbStatus.SelectedItem = subTask.Status;
            cmbTeamMember.SelectedItem = subTask.TeamMemberId;

            getProjectForEdit(subTask.ProjectId.ToString());
            getTaskForEdit(subTask.TaskId.ToString());
            getTeamMemberForEidt(subTask.Id.ToString(), subTask.TeamMemberId.ToString());
        }

        private void getProjectForEdit(string id)
        {
            if (!IsInitialized) return;

            DataTable dt = subTaskController.getProject();
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
                        index = i + 1;
                    }
                    cmbProject.Items.Add(dt.Rows[i][0] + " - " + dt.Rows[i][1]);
                }

                cmbProject.SelectedIndex = index;
            }
            else
            {
                cmbProject.Items.Clear();
                cmbProject.Items.Add("Select Project");
                cmbProject.SelectedIndex = 0;
            }
        }
        private void getProject()
        {
            if (!IsInitialized) return;

            DataTable dt = subTaskController.getProject();
            if (dt.Rows.Count > 0)
            {

                cmbProject.Items.Clear();
                cmbProject.Items.Add("Select Project");
                cmbProject.SelectedIndex = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
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
        private void getTask(string id)
        {
            if (!IsInitialized) return;

            DataTable dt = subTaskController.getTask(id);
            if (dt.Rows.Count > 0)
            {

                cmbTask.Items.Clear();
                cmbTask.Items.Add("Select Task");
                cmbTask.SelectedIndex = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmbTask.Items.Add(dt.Rows[i][0] + " - " + dt.Rows[i][1]);
                }
            }
            else
            {
                cmbTask.Items.Clear();
                cmbTask.Items.Add("Select Task");
                cmbTask.SelectedIndex = 0;
            }
        }

        private void getTaskForEdit(string id)
        {
            if (!IsInitialized) return;

            DataTable dt = subTaskController.getTask(id);
            if (dt.Rows.Count > 0)
            {

                cmbTask.Items.Clear();
                cmbTask.Items.Add("Select Task");
                cmbTask.SelectedIndex = 0;

                int index = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (id == dt.Rows[i][0].ToString())
                    {
                        index = i + 1;
                    }
                    cmbTask.Items.Add(dt.Rows[i][0] + " - " + dt.Rows[i][1]);
                }
                cmbTask.SelectedIndex = index;

            }
            else
            {
                cmbTask.Items.Clear();
                cmbTask.Items.Add("Select Task");
                cmbTask.SelectedIndex = 0;
            }
        }
        private void getTeamMembers(string id)
        {
            if (!IsInitialized) return;

            DataTable dt = subTaskController.getTeamMembers(id);
            if (dt.Rows.Count > 0)
            {

                cmbTeamMember.Items.Clear();
                cmbTeamMember.Items.Add("Select Team Members");
                cmbTeamMember.SelectedIndex = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmbTeamMember.Items.Add(dt.Rows[i][0] + " - " + dt.Rows[i][1]+" " + dt.Rows[i][2]);
                }
            }
            else
            {
                cmbTeamMember.Items.Clear();
                cmbTeamMember.Items.Add("Select Team Members");
                cmbTeamMember.SelectedIndex = 0;
            }
        }

        private void getTeamMemberForEidt(string id,string teamMember)
        {
            if (!IsInitialized) return;

            DataTable dt = subTaskController.getTeamMembers(id);
            if (dt.Rows.Count > 0)
            {

                cmbTeamMember.Items.Clear();
                cmbTeamMember.Items.Add("Select Team Members");
                cmbTeamMember.SelectedIndex = 0;

                int index = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (teamMember == dt.Rows[i][0].ToString())
                    {
                        index = i + 1;
                    }
                    cmbTeamMember.Items.Add(dt.Rows[i][0] + " - " + dt.Rows[i][1] + " " + dt.Rows[i][2]);
                }
                cmbTeamMember.SelectedIndex = index;

            }
            else
            {
                cmbTeamMember.Items.Clear();
                cmbTeamMember.Items.Add("Select Team Members");
                cmbTeamMember.SelectedIndex = 0;
            }
        }
        private string loadId()
        {
            return subTaskController.getId();
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
                dpExpectedEndDate.SelectedDate = startDate.AddDays(months);
                dpEndDate.SelectedDate = startDate.AddMonths(months);
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

        private void cmbProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized) return;

            if (cmbProject.SelectedIndex > 0)
            {
                getTask(cmbProject.SelectedItem.ToString().Split('-')[0]);
            }
        }

        private void cmbTask_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized) return;
            if (cmbTask.SelectedIndex > 0)
            {
                getTeamMembers(cmbTask.SelectedItem.ToString().Split('-')[0]);
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
            if (!helper.comboBoxValidation(cmbTeamMember))
            {
                textBoxName += "Team";
                cmbTeamMemberBorder.BorderThickness = new Thickness(1);
                cmbTeamMemberBorder.BorderBrush = Brushes.Red;
                cmbTeamMember.ToolTip = "Check your Team";
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

            if (!helper.comboBoxValidation(cmbTask))
            {
                textBoxName += "Task";
                cmbTaskBorder.BorderThickness = new Thickness(1);
                cmbTaskBorder.BorderBrush = Brushes.Red;
                cmbTask.ToolTip = "Check your Task";
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
                    Model.SubTask subTask = new Model.SubTask(int.Parse(txtId.Text), txtTitle.Text, txtDescription.Text, int.Parse(txtDuration.Text), int.Parse(cmbTeamMember.SelectedItem.ToString().Split('-')[0]), int.Parse(cmbProject.SelectedItem.ToString().Split('-')[0]), int.Parse(cmbTask.SelectedItem.ToString().Split('-')[0]), dpCreateDate.SelectedDate.Value, dpStartDate.SelectedDate.Value, dpExpectedEndDate.SelectedDate.Value, dpEndDate.SelectedDate.Value, float.Parse(txtPercentageComplete.Text), cmbStatus.Text);

                    subTaskController.create(subTask);

                    clear();


                }
                else
                {

                    Model.SubTask subTask = new Model.SubTask(int.Parse(txtId.Text), txtTitle.Text, txtDescription.Text, int.Parse(txtDuration.Text), int.Parse(cmbTeamMember.SelectedItem.ToString().Split('-')[0]), int.Parse(cmbProject.SelectedItem.ToString().Split('-')[0]), int.Parse(cmbTask.SelectedItem.ToString().Split('-')[0]), dpCreateDate.SelectedDate.Value, dpStartDate.SelectedDate.Value, dpExpectedEndDate.SelectedDate.Value, dpEndDate.SelectedDate.Value, float.Parse(txtPercentageComplete.Text), cmbStatus.Text);

                    subTaskController.update(subTask);

                }


            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }
        private void clear()
        {
            txtId.Text = loadId();
            txtDescription.Clear();
            txtTitle.Clear();
            cmbStatus.SelectedIndex = 1;
            cmbProject.SelectedIndex = 0;
            cmbTask.SelectedIndex = 0;
            txtDuration.Clear();
            txtPercentageComplete.Clear();
            dpCreateDate.SelectedDate = loadCurrentDate();
            dpStartDate.SelectedDate = loadCurrentDate();
            dpEndDate.SelectedDate = null;
            dpExpectedEndDate.SelectedDate = null;
            cmbTeamMember.SelectedIndex = 0;
            dpEndDate.SelectedDate = loadCurrentDate();

        }
    }
}
