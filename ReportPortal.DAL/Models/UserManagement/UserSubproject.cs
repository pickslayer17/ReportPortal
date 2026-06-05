using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.Models.UserManagement
{
    /// <summary>
    /// Join entity for subproject membership (many-to-many). A user can belong to several
    /// subprojects. Only subproject members are eligible to be assigned as reviewers for that
    /// subproject's tests.
    /// </summary>
    public class UserSubproject
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int SubprojectId { get; set; }
        public virtual Subproject Subproject { get; set; }
    }
}
