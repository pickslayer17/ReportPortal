using Models.Dto;
using ReportPortal.BL.Services.Interfaces;

namespace ReportPortal.Services.Interfaces
{
    public interface IUserService : IServiceBase<UserDto>
    {
        /// <summary>
        /// Returns the user plus everyone who shares a project with them (non-admin scoping).
        /// </summary>
        Task<IEnumerable<UserDto>> GetColleaguesAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Changes the user's password. Returns false when the supplied current password is wrong.
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the user's email. Throws <see cref="DAL.Exceptions.EmailAlreadyExistsException"/>
        /// if the email is already used by another user.
        /// </summary>
        Task<UserDto> UpdateProfileAsync(int userId, string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a user to a project (idempotent). Throws if the user or project does not exist.
        /// </summary>
        Task AddMemberAsync(int projectId, int userId, CancellationToken cancellationToken = default);

        /// <summary>Removes a user from a project (no-op if not a member).</summary>
        Task RemoveMemberAsync(int projectId, int userId, CancellationToken cancellationToken = default);

        /// <summary>Returns all members of a project.</summary>
        Task<IEnumerable<UserDto>> GetProjectMembersAsync(int projectId, CancellationToken cancellationToken = default);
    }
}
