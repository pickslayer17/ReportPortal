using ReportPortal.DAL.Models.UserManagement;
using ReportPortal.DAL.Repositories.Interfaces;

namespace ReportPortal.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Returns the current user plus everyone who shares at least one project with them.
        /// Used to scope user listings for non-admins (multitenancy).
        /// </summary>
        Task<IEnumerable<User>> GetUsersSharingProjectsAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>Adds the user to the project. No-op if the membership already exists (idempotent).</summary>
        Task AddUserToProjectAsync(int userId, int projectId, CancellationToken cancellationToken = default);

        /// <summary>Removes the user from the project. No-op if the membership does not exist.</summary>
        Task RemoveUserFromProjectAsync(int userId, int projectId, CancellationToken cancellationToken = default);

        /// <summary>Returns all users that are members of the given project.</summary>
        Task<IEnumerable<User>> GetProjectMembersAsync(int projectId, CancellationToken cancellationToken = default);
    }
}
