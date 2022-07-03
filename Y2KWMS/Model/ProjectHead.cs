using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2KWMS.Model
{
    class ProjectHead : User
    {
        public ProjectHead(string firstName, string lastName, string mobile, string address, string email, string password, string branch, string type, string status):base(firstName,lastName,mobile,address,email,password,branch,type,status)
        {
            base.Type = "ProjectHead";
        }

        public ProjectHead(string id, string firstName, string lastName, string mobile, string address, string email, string password, string branch, string type, string status) : base(id,firstName, lastName, mobile, address, email, password, branch, type, status)
        {
            base.Type = "ProjectHead";
        }
    }
}
