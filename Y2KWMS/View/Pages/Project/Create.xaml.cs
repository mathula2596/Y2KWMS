using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
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

namespace Y2KWMS.View.Pages.Project
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Page
    {
        ProjectController projectController;
        Helper helper;
        Branch branch;
        string updateId = null;

        public Create(string email,string name,string type)
        {
            InitializeComponent();
            projectController = new ProjectController();
            txtId.Text = loadId();
            dpCreateDate.SelectedDate = loadCurrentDate();
            dpStartDate.SelectedDate = loadCurrentDate();
            dpEndDate.SelectedDate = loadCurrentDate();
            helper = new Helper();
            branch = new Branch();
           
            getTeam(null);
            txtPercentageComplete.Text = "0.0";
            // MessageBox.Show(email);

            dataForComboBox(type);




        }

        public Create(string id, Model.User user)
        {
            InitializeComponent();
            projectController = new ProjectController();
            txtId.Text = id;
            dpCreateDate.SelectedDate = loadCurrentDate();
            dpStartDate.SelectedDate = loadCurrentDate();
            dpEndDate.SelectedDate = loadCurrentDate();
            helper = new Helper();
            branch = new Branch();
            dataForComboBox(user.Type);
            getTeam(null);
            updateId = id;
            btnClear.Visibility = Visibility.Hidden;

            txtblockTitle.Text = "Update Project";
            btnSave.Content = "Update";
            getDataForEdit(id);

        }
        private void getDataForEdit(string id)
        {
            Model.Project project = projectController.editData(id);
            txtTitle.Text = project.Title;
            txtDescription.Text = project.Description;
            txtDuration.Text = project.Duration.ToString();
            txtPercentageComplete.Text = project.PercentageComplete.ToString();
            dpCreateDate.SelectedDate = project.CreatedDate;
            dpStartDate.SelectedDate = project.StartDate;
            dpExpectedEndDate.SelectedDate = project.ExpectedEndDate;
            dpEndDate.SelectedDate = project.EndDate;
            cmbBranch.SelectedItem = project.Branch;
            cmbStatus.SelectedItem = project.Status;
            
            for(int i = 0; i<project.TeamId.Length; i++)
            {
                lbTeam.Items.Add(project.TeamId[i].ToString() + " - " + project.TeamName[i].ToString());
            }
        }

        private string loadId()
        {
            return projectController.getId();
        }

        private DateTime loadCurrentDate()
        {
            DateTime localDate = DateTime.Now;
            var culture = new CultureInfo("en-GB");
            return localDate;
        }

        private void txtDuration_KeyUp(object sender, KeyEventArgs e)
        {
            getExpectedEndDate();

        }

        private void getExpectedEndDate()
        {
            if (txtDuration.Text.Length > 0)
            {
                int months = int.TryParse(txtDuration.Text, out months) ? months : 1;
                DateTime startDate = dpStartDate.SelectedDate.Value;
                dpExpectedEndDate.SelectedDate = startDate.AddMonths(months);
                dpEndDate.SelectedDate = startDate.AddMonths(months);
            }
            else
            {
                dpExpectedEndDate.SelectedDate = null;
            }
        }

        private void dataForComboBox(string type)
        {
            for (int i = 1; i <= branch.branch.Count; i++)
            {
                cmbBranch.Items.Add(branch.branch[i]);
            }

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

        private void getTeam(string branch = null)
        {
            if (!IsInitialized) return;
            string branchName = branch;

            DataTable dt = projectController.getTeam(branchName);
            if (dt.Rows.Count > 0)
            {

                cmbTeam.Items.Clear();
                cmbTeam.Items.Add("Select Team");
                cmbTeam.SelectedIndex = 0;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmbTeam.Items.Add(dt.Rows[i][0] + " - " + dt.Rows[i][1]);
                }
            }
            else
            {
                cmbTeam.Items.Clear();
                cmbTeam.Items.Add("Select Team");
                cmbTeam.SelectedIndex = 0;
            }
        }

        private void cmbBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getTeam(cmbBranch.SelectedItem.ToString());
        }

        private void cmbTeam_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized) return;

            if (cmbTeam.SelectedIndex > 0)
            {
                lbTeam.Items.Add(cmbTeam.SelectedItem.ToString());
            }

        }

        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (lbTeam.SelectedIndex >= 0)
            {
                lbTeam.Items.RemoveAt(lbTeam.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select one element", "Warning Message", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            if (!helper.listBoxValidation(lbTeam.Items.Count))
            {
                textBoxName += "Team";
                cmbTeamBorder.BorderThickness = new Thickness(1);
                cmbTeamBorder.BorderBrush = Brushes.Red;
                cmbTeam.ToolTip = "Check your Team";
            }
            if (!dpExpectedEndDate.SelectedDate.HasValue)
            {
                textBoxName += "Date";
                dpExpectedEndDate.BorderThickness = new Thickness(1);
                dpExpectedEndDate.BorderBrush = Brushes.Red;
                dpExpectedEndDate.ToolTip = "Check your Expected End Date";
            }


            if (!helper.comboBoxValidation(cmbBranch))
            {
                textBoxName += "Branch";
                cmbBranchBorder.BorderThickness = new Thickness(1);
                cmbBranchBorder.BorderBrush = Brushes.Red;
                cmbBranch.ToolTip = "Check your Branch";
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

                    int[] teamId = new int[lbTeam.Items.Count];

                    for (int i = 0; i < lbTeam.Items.Count; i++)
                    {
                        lbTeam.SelectAll();
                        
                        teamId[i] = int.Parse(lbTeam.SelectedItems[i].ToString().Split('-')[0]);
                    }

                    Model.Project project = new Model.Project(int.Parse(txtId.Text),txtTitle.Text,txtDescription.Text,int.Parse(txtDuration.Text),cmbBranch.Text,teamId,dpCreateDate.SelectedDate.Value,dpStartDate.SelectedDate.Value,dpExpectedEndDate.SelectedDate.Value,dpEndDate.SelectedDate.Value,float.Parse(txtPercentageComplete.Text),cmbStatus.Text);
                    projectController.create(project);

                    clear();


                }
                else
                {
                    int[] teamId = new int[lbTeam.Items.Count];

                    for (int i = 0; i < lbTeam.Items.Count; i++)
                    {
                        lbTeam.SelectAll();

                        teamId[i] = int.Parse(lbTeam.SelectedItems[i].ToString().Split('-')[0]);
                    }

                    Model.Project project = new Model.Project(int.Parse(txtId.Text), txtTitle.Text, txtDescription.Text, int.Parse(txtDuration.Text), cmbBranch.Text, teamId, dpCreateDate.SelectedDate.Value, dpStartDate.SelectedDate.Value, dpExpectedEndDate.SelectedDate.Value, dpEndDate.SelectedDate.Value, float.Parse(txtPercentageComplete.Text), cmbStatus.Text);

                    projectController.update(project);

                    
                }


            }
        }
        private void clear()
        {
            txtId.Text = loadId();
            txtDescription.Clear();
            txtTitle.Clear();
            cmbBranch.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 1;
            //cmbBranch.SelectedIndex = 0;
            txtDuration.Clear();
            txtPercentageComplete.Text = "0.0";
            dpCreateDate.SelectedDate = loadCurrentDate();
            dpStartDate.SelectedDate = loadCurrentDate();
            dpEndDate.SelectedDate = null;
            dpExpectedEndDate.SelectedDate = null;
            lbTeam.Items.Clear();
            dpEndDate.SelectedDate = loadCurrentDate();

        }

        private void dpStartDate_MouseLeave(object sender, MouseEventArgs e)
        {
            getExpectedEndDate();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }
    }
}
