using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.Models.UserManagement
{
    /// <summary>
    /// Join entity linking a user to a project they are a member of.
    /// Drives multitenancy: a non-admin only sees/touches projects they belong to.
    /// </summary>
    public class UserProject
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
    }
}
