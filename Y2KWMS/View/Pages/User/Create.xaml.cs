using System;
using System.Collections.Generic;
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
using Y2KWMS.Model;
using Y2KWMS.Controller;
using System.Text.RegularExpressions;

namespace Y2KWMS.View.Pages.Project.User
{
    /// <summary>
    /// Interaction logic for Create.xaml
    /// </summary>
    public partial class Create : Page 
    {
        Helper helper;
        Branch branch;
        UserController userController;
        public const string DefaultPassword = "Y2Kcompany@2022";
        public string updateId = null;
        public Create(string id, Model.User user)
        {
            InitializeComponent();
            helper = new Helper();
            branch = new Branch();
            userController = new UserController();
            dataForComboBox();
            txtId.Text = id;
            txtblockTitle.Text = "Update User";
            btnRegister.Content = "Update";
            getDataForEdit(id);
            updateId = id;
            btnClear.Visibility = Visibility.Hidden;

          


        }

        private void getDataForEdit(string id)
        {
            Model.User user = userController.editData(id);
            txtFirstName.Text = user.FirstName;
            txtLastName.Text = user.LastName;
            txtMobile.Text = user.Mobile;
            txtAddress.Text = user.Address;
            txtEmail.Text = user.Email;
            txtEmail.IsReadOnly = true;
            cmbBranch.SelectedItem = user.Branch;
            cmbStatus.SelectedItem = user.Status;
            cmbType.SelectedItem = user.Type;
        }
        public Create(string email, string name, string type)
        {
            InitializeComponent();
            helper = new Helper();
            branch = new Branch();
            userController = new UserController();
            dataForComboBox();
            txtId.Text = loadId();
            txtPassword.Password = DefaultPassword;

        }

        public Create()
        {
            InitializeComponent();
          

        }

        private string loadId()
        {
            return userController.getId();
        }
        private void dataForComboBox()
        {
            for(int i=1; i<=branch.branch.Count;i++)
            {
                cmbBranch.Items.Add(branch.branch[i]);
            }

            for (int i = 0; i < helper.status.Length; i++)
            {
                cmbStatus.Items.Add(helper.status[i]);
                cmbStatus.SelectedIndex = 1;
            }

            for (int i = 0; i < helper.type.Length; i++)
            {
                cmbType.Items.Add(helper.type[i]);
                cmbType.SelectedIndex = 2;
            }
        }
        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {

            string textBoxName = null;
            if(!helper.textOnlyValidation(txtFirstName.Text))
            {
                textBoxName += "First Name";
                txtFirstName.BorderBrush = Brushes.Red;
                txtFirstName.ToolTip = "Check your First Name";

            }

            if (!helper.textOnlyValidation(txtLastName.Text))
            {
                textBoxName += "Last Name";
                txtLastName.BorderBrush = Brushes.Red;
                txtLastName.ToolTip = "Check your Last Name";
            }
            if (!helper.mobileNumberOnlyValidation(txtMobile.Text))
            {
                textBoxName += "Mobile Number";
                txtMobile.BorderBrush = Brushes.Red;
                txtMobile.ToolTip = "Check your Mobile Number (Use Country Code) ";
            }
            if (!helper.emailValidation(txtEmail.Text))
            {
                textBoxName += "Email";
                txtEmail.BorderBrush = Brushes.Red;
                txtEmail.ToolTip = "Check your Email";
            }
            if (!helper.notNullValidation(txtAddress.Text))
            {
                textBoxName += "Address";
                txtAddress.BorderBrush = Brushes.Red;
                txtAddress.ToolTip = "Check your Address";
            }
         
            if (!helper.comboBoxValidation(cmbBranch))
            {
                textBoxName += "Branch";
                cmbBranchBorder.BorderThickness = new Thickness(1);
                cmbBranchBorder.BorderBrush = Brushes.Red;
                cmbBranch.ToolTip = "Check your Branch";
            }
            if (!helper.comboBoxValidation(cmbType))
            {
                textBoxName += "User Type";
                cmbTypeBorder.BorderThickness = new Thickness(1);
                cmbTypeBorder.BorderBrush = Brushes.Red;
                cmbType.ToolTip = "Check your User Type";
            }
            if (!helper.comboBoxValidation(cmbStatus))
            {
                textBoxName += "Status";
                cmbStatusBorder.BorderThickness = new Thickness(1);
                cmbStatusBorder.BorderBrush = Brushes.Red;
                cmbStatus.ToolTip = "Check your Status";
            }
            //MessageBox.Show(cmbType.Text);
            if (textBoxName == null)
            {
                //MessageBox.Show(updateId);
                if(updateId == null)
                {
                    if (!helper.passwordValidation(txtPassword.Password))
                    {
                        //textBoxName += "Password";
                        txtPassword.BorderBrush = Brushes.Red;
                        txtPassword.ToolTip = "Password must have one uppercase letter, numbers, symbols and length must be more than 8";
                    }
                    else
                    {
                        if(cmbType.SelectedIndex == 1)
                        {
                            ProjectHead projectHead = new ProjectHead(txtFirstName.Text, txtLastName.Text, txtMobile.Text, txtAddress.Text, txtEmail.Text, txtPassword.Password, cmbBranch.Text, cmbType.Text, cmbStatus.Text);
                            userController.create(projectHead);
                            clear();

                        }
                        else if(cmbType.SelectedIndex == 2)
                        {
                            TeamLeader teamLeader = new TeamLeader(txtFirstName.Text, txtLastName.Text, txtMobile.Text, txtAddress.Text, txtEmail.Text, txtPassword.Password, cmbBranch.Text, cmbType.Text, cmbStatus.Text);
                            userController.create(teamLeader);
                            clear();
                        }
                        else
                        {
                            TeamMember teamMember = new TeamMember(txtFirstName.Text, txtLastName.Text, txtMobile.Text, txtAddress.Text, txtEmail.Text, txtPassword.Password, cmbBranch.Text, cmbType.Text, cmbStatus.Text);
                            userController.create(teamMember);
                            clear();
                        }

                    }
                   
                }
                else
                {
                    if(txtPassword.Password.Length > 0)
                    {
                        if (!helper.passwordValidation(txtPassword.Password))
                        {
                            //textBoxName += "Password";
                            txtPassword.BorderBrush = Brushes.Red;
                            txtPassword.ToolTip = "Password must have one uppercase letter, numbers, symbols and length must be more than 8";
                        }
                        else
                        {
                            userTypeSelection();


                        }
                        
                    }
                    else
                    {

                        userTypeSelection();

                    }
                }
               

            }
           
        }

        private void userTypeSelection()
        {
            if (cmbType.SelectedIndex == 1)
            {
                ProjectHead projectHead = new ProjectHead(txtId.Text, txtFirstName.Text, txtLastName.Text, txtMobile.Text, txtAddress.Text, txtEmail.Text, txtPassword.Password, cmbBranch.Text, cmbType.Text, cmbStatus.Text);
                userController.update(projectHead);

            }
            else if (cmbType.SelectedIndex == 2)
            {
                TeamLeader teamLeader = new TeamLeader(txtId.Text, txtFirstName.Text, txtLastName.Text, txtMobile.Text, txtAddress.Text, txtEmail.Text, txtPassword.Password, cmbBranch.Text, cmbType.Text, cmbStatus.Text);
                userController.update(teamLeader);
            }
            else
            {
                TeamMember teamMember = new TeamMember(txtId.Text, txtFirstName.Text, txtLastName.Text, txtMobile.Text, txtAddress.Text, txtEmail.Text, txtPassword.Password, cmbBranch.Text, cmbType.Text, cmbStatus.Text);
                userController.update(teamMember);
            }
        }

        private void clear()
        {
            txtId.Text = loadId();
            txtFirstName.Clear();
            txtLastName.Clear();
            txtMobile.Text = "+";
            txtAddress.Clear();
            txtEmail.Clear();
            txtPassword.Clear();
            cmbBranch.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 1;
            cmbType.SelectedIndex = 1;
            
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
           
            if(txtblockTitle.Text == "Update User")
            {
                getDataForEdit(txtId.Text);
            }
            else
            {
                clear();
            }
        }

       
    }
}
