using System;
using System.Collections.Generic;
using System.Data;
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

namespace Y2KWMS.View.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        DiscussionController discussionController;
        Discussion discussion;
        Helper helper;
        public HomePage()
        {
            InitializeComponent();
            discussionController = new DiscussionController();
            discussion = new Discussion();
            helper = new Helper();
            getProject();
        }

        private void getProject()
        {
            if (!IsInitialized) return;

            DataTable dt = discussionController.getProject();
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

        private void loadDiscussion(string project)
        {
            int projectId = int.Parse(project.Split('-')[0]);
            DataTable dt = discussionController.loadDiscussion(projectId);
            TextBlock textBlock;
            TextBox textBox;
            for(int i = 0; i<dt.Rows.Count;i++)
            {
                textBlock = new TextBlock();
                textBlock.Text = dt.Rows[i][0].ToString() + " " + dt.Rows[i][1].ToString();
                textBlock.FontSize = 14;
                textBlock.FontWeight = FontWeights.UltraBold;
                textBlock.Margin = new Thickness(0, 10, 0, 0);


                textBox = new TextBox();
                textBox.Text = dt.Rows[i][2].ToString();
                textBox.FontSize = 14;
                textBox.FontWeight = FontWeights.Bold;
                textBox.FontStyle = FontStyles.Italic;
                textBox.IsEnabled = false;
                textBox.Width = 750;
                textBox.Margin = new Thickness(0, 10, 0, 0);
                //textBox.Height = 50;
                textBox.Padding = new Thickness(10);
                textBox.TextWrapping = TextWrapping.Wrap;


                content.Children.Add(textBlock);
                content.Children.Add(textBox);

            }
        }
        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            string validation = null;
            if (!helper.comboBoxValidation(cmbProject))
            {
                validation += "Project";
                cmbProjectBorder.BorderThickness = new Thickness(1);
                cmbProjectBorder.BorderBrush = Brushes.Red;
                cmbProject.ToolTip = "Select the Project";
            }
            if (!helper.notNullValidation(txtComment.Text))
            {
                validation += "Comment";
                txtComment.BorderThickness = new Thickness(1);
                txtComment.BorderBrush = Brushes.Red;
                txtComment.ToolTip = "Type your message";
            }
            if(validation == null)
            {
                int userId = int.Parse(Environment.GetEnvironmentVariable("id"));
                discussion = new Discussion(txtComment.Text,int.Parse(cmbProject.SelectedItem.ToString().Split('-')[0]), userId);
                discussionController.save(discussion);
                //cmbProject.SelectedIndex = 0;
                txtComment.Clear();
                content.Children.Clear();
                loadDiscussion(cmbProject.SelectedItem.ToString());

            }

        }

        private void cmbProject_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbProject.SelectedIndex > 0)
            {
                loadDiscussion(cmbProject.SelectedItem.ToString());
            }
            
        }
    }
}
