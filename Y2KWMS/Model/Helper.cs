using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Y2KWMS.Model
{
    class Helper
    {
        public string[] status;
        public string[] projectStatus;
        public string[] projectUserStatus;


        public string[] type;
        public Helper()
        {
            status = new string[2];
            status[0] = "Active";
            status[1] = "Inactive";

            type = new string[3];
            type[0] = "ProjectHead";
            type[1] = "TeamLeader";
            type[2] = "TeamMember";

            projectStatus = new string[5];
            projectStatus[0] = "Not Started";
            projectStatus[1] = "In Progress";
            projectStatus[2] = "Completed";
            projectStatus[3] = "Closed";
            projectStatus[4] = "Cancelled";

            projectUserStatus = new string[3];
            projectUserStatus[0] = "Not Started";
            projectUserStatus[1] = "In Progress";
            projectUserStatus[2] = "Completed";
           


        }

        public bool textOnlyValidation(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !Regex.Match(text, "^[a-zA-Z]*$").Success)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool mobileNumberOnlyValidation(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !Regex.Match(text, @"^([\+][1-9]{2,3}[-]?|[0])?[1-9][0-9]{8,8}$").Success)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool numberOnlyValidation(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !Regex.Match(text, @"^[0-9]*$").Success)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool floatNumbeOnlyValidation(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !Regex.Match(text, @"^[0-9]*(?:\.[0-9]*)?$").Success)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public bool emailValidation(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !Regex.Match(text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$").Success)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool passwordValidation(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || !Regex.Match(text, @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&])[A-Za-z\d@$!%*#?&]{8,}$").Success)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool notNullValidation(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
      

        public bool comboBoxValidation(ComboBox text)
        {
            if (text.SelectedIndex <=0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool listBoxValidation(int listBox)
        {
            if (listBox <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

