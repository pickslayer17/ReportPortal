using ReportPortal.DAL.Enums;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ProjectStatus ProjectStatus { get; set; }
        public virtual ICollection<Run> Runs { get; set; }
    }
}
