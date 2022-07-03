using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Y2KWMS.Model;

namespace Y2KWMS.Model
{
    class Login : User
    {
        public Login(string email, string password) : base(email,password)
        {

        }
    }
}
