using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2KWMS.Model
{
    class TeamMember : User
    {
        public TeamMember(string firstName, string lastName, string mobile, string address, string email, string password, string branch, string type, string status) : base(firstName, lastName, mobile, address, email, password, branch, type, status)
        {
            base.Type = "TeamMember";
        }

        public TeamMember(string id, string firstName, string lastName, string mobile, string address, string email, string password, string branch, string type, string status) : base(id, firstName, lastName, mobile, address, email, password, branch, type, status)
        {
            base.Type = "TeamMember";
        }

        public TeamMember(string id, string firstName, string lastName)
        {
            base.Id = int.Parse(id);
            base.FirstName = firstName;
            base.LastName = lastName;
        }
    }
}
