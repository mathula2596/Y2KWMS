using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Y2KWMS.Model
{
    public class Project
    {
        private int id;
        private string title;
        private string description;
        private int duration;
        private string branch;
        private int [] teamId;
        private string[] teamName;
        private DateTime createdDate;
        private DateTime startDate;
        private DateTime expectedEndDate;
        private DateTime endDate;
        private float percentageComplete;
        private string status;

        public Project()
        {

        }
        public Project(string title, string description, int duration, string branch, int[] teamId, DateTime createdDate, DateTime startDate, DateTime expectedEndDate, DateTime endDate, float percentageComplete, string status)
        {
            this.title = title;
            this.description = description;
            this.duration = duration;
            this.branch = branch;
            this.teamId = teamId;
            this.createdDate = createdDate;
            this.startDate = startDate;
            this.expectedEndDate = expectedEndDate;
            this.endDate = endDate;
            this.percentageComplete = percentageComplete;
            this.status = status;
        }

        public Project(int id, string title, string description, int duration, string branch, int[] teamId, DateTime createdDate, DateTime startDate, DateTime expectedEndDate, DateTime endDate, float percentageComplete, string status)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.duration = duration;
            this.branch = branch;
            this.teamId = teamId;
            this.createdDate = createdDate;
            this.startDate = startDate;
            this.expectedEndDate = expectedEndDate;
            this.endDate = endDate;
            this.percentageComplete = percentageComplete;
            this.status = status;
        }

        public Project(int id, string title, string description, int duration, string branch, int[] teamId, string[] teamName, DateTime createdDate, DateTime startDate, DateTime expectedEndDate, DateTime endDate, float percentageComplete, string status)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.duration = duration;
            this.branch = branch;
            this.teamId = teamId;
            this.TeamName = teamName;
            this.createdDate = createdDate;
            this.startDate = startDate;
            this.expectedEndDate = expectedEndDate;
            this.endDate = endDate;
            this.percentageComplete = percentageComplete;
            this.status = status;
        }

        public int Id { get => id; set => id = value; }
        public string Title { get => title; set => title = value; }
        public string Description { get => description; set => description = value; }
        public int Duration { get => duration; set => duration = value; }
        public string Branch { get => branch; set => branch = value; }
        public int[] TeamId { get => teamId; set => teamId = value; }
        public DateTime CreatedDate { get => createdDate; set => createdDate = value; }
        public DateTime StartDate { get => startDate; set => startDate = value; }
        public DateTime ExpectedEndDate { get => expectedEndDate; set => expectedEndDate = value; }
        public DateTime EndDate { get => endDate; set => endDate = value; }
        public float PercentageComplete { get => percentageComplete; set => percentageComplete = value; }
        public string Status { get => status; set => status = value; }
        public string[] TeamName { get => teamName; set => teamName = value; }
    }
}
