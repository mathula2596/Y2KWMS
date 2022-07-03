using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2KWMS.Model
{
    class Discussion
    {
        private int id;
        private string message;
        private int projectId;
        private int userId;

        public Discussion(int id, string message, int projectId, int userId)
        {
            this.Id = id;
            this.Message = message;
            this.ProjectId = projectId;
            this.UserId = userId;
        }

        public Discussion(string message, int projectId, int userId)
        {
            this.Message = message;
            this.ProjectId = projectId;
            this.UserId = userId;
        }

        public Discussion()
        {

        }

        public int Id { get => id; set => id = value; }
        public string Message { get => message; set => message = value; }
        public int ProjectId { get => projectId; set => projectId = value; }
        public int UserId { get => userId; set => userId = value; }
    }
}
