using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IProjectService : IServiceBase<ProjectDto>
    {
        /// <summary>Projects the given user is a member of (non-admin scoping).</summary>
        Task<IEnumerable<ProjectDto>> GetForUserAsync(int userId, CancellationToken cancellationToken = default);
    }
}
