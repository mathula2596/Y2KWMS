using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Y2KWMS.Model;

namespace Y2KWMS.Controller
{
    class DiscussionController
    {
        QueryController queryController;
        public DataTable getProject()
        {
            queryController = new QueryController();

            DataTable dt;
            dt = queryController.getProjectForDiscussion();

            return dt;
        }

        public void save(Discussion discussion)
        {
            DateTime today = DateTime.Now;
            string[] fields = { "projectId", "userId", "message", "date" };
            string[] values = { discussion.ProjectId.ToString(), discussion.UserId.ToString(), discussion.Message, today.ToString() };
            queryController.save("discussion",fields,values,false);
        }

        public DataTable loadDiscussion(int projectId)
        {
            DataTable dt = queryController.searchDiscussion(projectId);
            return dt;
        }
    }
}
