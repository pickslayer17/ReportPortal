using ReportPortal.DAL.Models.RunProjectManagement;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        /// <summary>Projects the given user is a member of (multitenancy scoping for non-admins).</summary>
        Task<IEnumerable<Project>> GetForUserAsync(int userId, CancellationToken cancellationToken = default);
    }
}
