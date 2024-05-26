using ReportPortal.BL.Models;

namespace ReportPortal.BL.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<ProjectDto>> GetAllByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ProjectDto> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<ProjectCreatedDto> CreateAsync(ProjectForCreationDto accountForCreationDto, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
