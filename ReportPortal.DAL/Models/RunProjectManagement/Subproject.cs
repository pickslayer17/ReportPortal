using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.DAL.Models.RunProjectManagement
{
    /// <summary>
    /// Grouping layer between a project and its runs (Project -> Subproject -> Run).
    /// Access/visibility stays at the project level; subproject membership only governs
    /// who can be assigned as a reviewer for tests in that subproject.
    /// </summary>
    public class Subproject
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public virtual ICollection<Run> Runs { get; set; } = new List<Run>();
        public virtual ICollection<UserSubproject> UserSubprojects { get; set; } = new List<UserSubproject>();
    }
}
