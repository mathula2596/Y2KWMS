using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2KWMS.Model
{
    class Team
    {
        private int id;
        private string name;
        private string status;
        private int teamLeader;
        private int[] teamMember;
        private string branch;

        public Team(int id, string name, string status, int teamLeader, int[] teamMember, string branch)
        {
            this.id = id;
            this.name = name;
            this.status = status;
            this.teamLeader = teamLeader;
            this.teamMember = teamMember;
            this.branch = branch;
        }

        public Team(string name, string status, int teamLeader, int[] teamMember, string branch)
        {
            this.name = name;
            this.status = status;
            this.teamLeader = teamLeader;
            this.teamMember = teamMember;
            this.branch = branch;
        }

        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Status { get => status; set => status = value; }
        public int TeamLeader { get => teamLeader; set => teamLeader = value; }
        public int[] TeamMember { get => teamMember; set => teamMember = value; }
        public string Branch { get => branch; set => branch = value; }
    }
}
