using Models.Dto;
using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface ISubprojectService
    {
        Task<SubprojectDto> CreateAsync(int projectId, string name, CancellationToken cancellationToken = default);
        Task<IEnumerable<SubprojectDto>> GetForProjectAsync(int projectId, CancellationToken cancellationToken = default);

        /// <summary>Adds a user to a subproject (must already be a member of the parent project).</summary>
        Task AddMemberAsync(int subprojectId, int userId, CancellationToken cancellationToken = default);
        Task RemoveMemberAsync(int subprojectId, int userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserDto>> GetMembersAsync(int subprojectId, CancellationToken cancellationToken = default);
    }
}
