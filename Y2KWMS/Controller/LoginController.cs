using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Y2KWMS.Controller;
using Y2KWMS.Model;


namespace Y2KWMS.Controller
{
    class LoginController
    {
        QueryController queryController;
        public bool login(Model.User user)
        {
            queryController = new QueryController();
            bool login = queryController.login(user);
            return login;
        }
    }
}
