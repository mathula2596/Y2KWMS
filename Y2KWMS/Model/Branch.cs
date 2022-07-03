using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2KWMS.Model
{
    class Branch
    {
        public Dictionary<int, string> branch;
        public Branch()
        {
            branch = new Dictionary<int, string>();
            branch.Add(1,"London");
            branch.Add(2, "Nottingham");
            branch.Add(3, "Birmingham");

        }
    }
}
