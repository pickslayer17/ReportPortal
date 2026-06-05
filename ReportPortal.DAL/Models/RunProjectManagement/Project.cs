using ReportPortal.DAL.Enums;
using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public virtual ICollection<Run> Runs { get; set; }
        public virtual ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
    }
}
