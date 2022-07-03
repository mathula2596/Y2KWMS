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

namespace Y2KWMS.View.Pages.Team
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Page
    {
        Helper helper;
        Branch branch;
        TeamController teamController;
        string updateId = null;
       // TeamMember teamMember;
        //List<TeamMember> objTeamMemberList;
        public static TextInfo textFormatter = new CultureInfo("en-UK", false).TextInfo;
        public Create(string email, string name, string type)
        {
            InitializeComponent();
            
            helper = new Helper();
            branch = new Branch();
            teamController = new TeamController();

            dataForComboBox();

            getTeamLeaders();
            txtId.Text = loadId();
            getTeamMembers();
           // objTeamMemberList = new List<TeamMember>();
        }

        private string loadId()
        {
            return teamController.getId();
        }

        private void dataForComboBox()
        {
            for (int i = 1; i <= branch.branch.Count; i++)
            {
                cmbBranch.Items.Add(branch.branch[i]);
            }

            for (int i = 0; i < helper.status.Length; i++)
            {
                cmbStatus.Items.Add(helper.status[i]);
                cmbStatus.SelectedIndex = 1;
            }
          
        }

        private void getTeamLeaders(string branch = null)
        {
            if (!IsInitialized) return;
            string branchName = branch;
            teamController = new TeamController();
          

            DataTable dt = teamController.getTeamLeaders(branchName);
            if(dt.Rows.Count > 0)
            {
                
                cmbTeamLeader.Items.Clear();
                cmbTeamLeader.Items.Add("Select TeamLeader");
                cmbTeamLeader.SelectedIndex = 0;
               
                for (int i = 0; i< dt.Rows.Count; i++)
                {
                    cmbTeamLeader.Items.Add(dt.Rows[i][0].ToString() + " - " + textFormatter.ToTitleCase(dt.Rows[i][1].ToString()) + " " + textFormatter.ToTitleCase(dt.Rows[i][2].ToString()));
                }
            }
            else
            {
                cmbTeamLeader.Items.Clear();
                cmbTeamLeader.Items.Add("Select TeamLeader");
                cmbTeamLeader.SelectedIndex = 0;
            }
        }

        private void getTeamMembers(string branch = null)
        {
            if (!IsInitialized) return;
            string branchName = branch;
            teamController = new TeamController();
            //objTeamMemberList = new List<TeamMember>();
            DataTable dt = teamController.getTeamMembers(branchName);
            //objTeamMemberList.Clear();
            if (dt.Rows.Count > 0)
            {

                cmbTeamMembers.Items.Clear();
                cmbTeamMembers.Items.Add("Select TeamMember");
                cmbTeamMembers.SelectedIndex = 0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    cmbTeamMembers.Items.Add(dt.Rows[i][0].ToString() + " - " + textFormatter.ToTitleCase(dt.Rows[i][1].ToString()) + " " + textFormatter.ToTitleCase(dt.Rows[i][2].ToString()));

                }

            }
            else
            {
                cmbTeamMembers.Items.Add("Select TeamMember");
                cmbTeamMembers.SelectedIndex = 0;
                cmbTeamMembers.Items.Clear();

            }

        }

      
        private void cmbBranch_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           // MessageBox.Show(cmbBranch.SelectedIndex.ToString());
            if(cmbBranch.SelectedIndex >0)
            {
                getTeamLeaders(cmbBranch.SelectedItem.ToString());
                getTeamMembers(cmbBranch.SelectedItem.ToString());
            }
            else
            {
                getTeamLeaders();
                getTeamMembers();
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

        private void btnRemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if(lbTeamMembers.SelectedIndex >= 0)
            {
                lbTeamMembers.Items.RemoveAt(lbTeamMembers.SelectedIndex);
            }
            else
            {
                MessageBox.Show("Please select one element", "Warning Message", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //MessageBox.Show(lbTeamMembers.SelectedIndex.ToString());
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            string textBoxName = null;
            //if (!helper.textOnlyValidation(txtName.Text))
            //{
            //    textBoxName += "Team Name";
            //    txtName.BorderBrush = Brushes.Red;
            //    txtName.ToolTip = "Check your Team Name";

            //}
            if (!helper.comboBoxValidation(cmbStatus))
            {
                textBoxName += "Status";
                cmbStatusBorder.BorderThickness = new Thickness(1);
                cmbStatusBorder.BorderBrush = Brushes.Red;
                cmbStatus.ToolTip = "Check your Status";
            }
           
            if (!helper.comboBoxValidation(cmbTeamLeader))
            {
                textBoxName += "Team Leader";
                cmbTeamLeaderBorder.BorderThickness = new Thickness(1);
                cmbTeamLeaderBorder.BorderBrush = Brushes.Red;
                cmbTeamLeader.ToolTip = "Select the Team Leader";
            }
            if (!helper.comboBoxValidation(cmbBranch))
            {
                textBoxName += "Branch";
                cmbBranchBorder.BorderThickness = new Thickness(1);
                cmbBranchBorder.BorderBrush = Brushes.Red;
                cmbBranch.ToolTip = "Select the Team Branch";
            }  
            if (!helper.listBoxValidation(lbTeamMembers.Items.Count))
            {
                textBoxName += "Team Members";
                cmbTeamMemberBorder.BorderThickness = new Thickness(1);
                cmbTeamMemberBorder.BorderBrush = Brushes.Red;
                cmbTeamMembers.ToolTip = "Select the Team Members";
            }
            if (textBoxName == null)
            {
                //MessageBox.Show(updateId);
                if (updateId == null)
                {
                    string[] teamLeaderId = cmbTeamLeader.SelectedItem.ToString().Split('-');
                    //string[] listTeamMemberIds = new string[lbTeamMembers.Items.Count];
                    int[] teamMemberId = new int[lbTeamMembers.Items.Count];

                    for (int i = 0; i<lbTeamMembers.Items.Count;i++)
                    {
                        lbTeamMembers.SelectAll();
                        //listTeamMemberIds[i] = lbTeamMembers.Items.IndexOf(i).ToString();
                        //MessageBox.Show(lbTeamMembers.SelectedItems[i].ToString());
                        teamMemberId[i] = int.Parse(lbTeamMembers.SelectedItems[i].ToString().Split('-')[0]);
                    }


                    Model.Team team = new Model.Team(int.Parse(txtId.Text),txtName.Text,cmbStatus.Text, int.Parse(teamLeaderId[0]), teamMemberId,cmbBranch.Text);
                    teamController.create(team);
                    clear();

                }

            }
            else
            {
               
            }
        }

        private void clear()
        {
            txtId.Text = loadId();
            txtName.Clear();
            cmbBranch.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 1;
            cmbTeamLeader.SelectedIndex = 0;
            cmbTeamMembers.SelectedIndex = 0;
            lbTeamMembers.Items.Clear();

        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clear();
        }
    }
}
