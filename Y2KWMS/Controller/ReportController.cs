using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Y2KWMS.Controller
{
    class ReportController
    {
        QueryController queryController;
        public List<Expander> load()
        {
            queryController = new QueryController();
            return queryController.viewReport();
        }

    }
}
