using ReportPortal.DAL.Models.RunProjectManagement;
using ReportPortal.DAL.Models.UserManagement;

namespace ReportPortal.DAL.Repositories.Interfaces
{
    public interface ISubprojectRepository
    {
        Task<int> InsertAsync(Subproject subproject, CancellationToken cancellationToken = default);

        /// <summary>Returns the subproject or null if it does not exist.</summary>
        Task<Subproject> GetByIdAsync(int subprojectId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Subproject>> GetForProjectAsync(int projectId, CancellationToken cancellationToken = default);

        // --- membership (governs reviewer eligibility) ---
        Task AddMemberAsync(int userId, int subprojectId, CancellationToken cancellationToken = default);
        Task RemoveMemberAsync(int userId, int subprojectId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetMembersAsync(int subprojectId, CancellationToken cancellationToken = default);
    }
}
